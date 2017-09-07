using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace exporter
{
    public static partial class Exporter
    {
        static string GetDeclaras(ISheet sheet, int col)
        {
            List<string> declaras = new List<string>();
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                    continue;
                ICell cell1 = row.GetCell(col, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                ICell cell2 = row.GetCell(col + 1, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                if (cell1 == null || cell2 == null)
                    continue;
                if (cell1.CellType != CellType.String || cell2.CellType != CellType.String)
                    throw new System.Exception("检查输入第" + (i + 1) + "行");
                string note = cell1.StringCellValue;
                string name = cell2.StringCellValue;
                if (string.IsNullOrEmpty(note) || string.IsNullOrEmpty(name) || name.StartsWith("_"))
                    continue;
                declaras.Add(string.Format("{0} = {1}--[[{2}]]", name, i + 1, note));
            }
            return string.Join(",\n", declaras);
        }

        public static string DealWithFormulaSheetLua(ISheet sheet)
        {
            CodeTemplate.curlang = CodeTemplate.Langue.Lua;

            StringBuilder sb = new StringBuilder();
            Dictionary<CellCoord, List<CellCoord>> abouts = new Dictionary<CellCoord, List<CellCoord>>();

            // 开头
            sb.AppendLine(CodeTemplate.Get("template_title")
                .Replace("[SHEET_NAME]", sheet.SheetName.Substring(3))
                .Replace("[USEFUL_ROW_COUNT]", (sheet.LastRowNum + 1).ToString())
                );

            // 声明
            sb.AppendLine("\n-- declares");
            sb.AppendLine(CodeTemplate.Get("template_input").Replace("[CONTENT]", GetDeclaras(sheet, 0)));
            sb.AppendLine(CodeTemplate.Get("template_output").Replace("[CONTENT]", GetDeclaras(sheet, 3)));

            // 抓取所有数据
            sb.AppendLine("\n-- all datas");
            for (int rownum = 0; rownum <= sheet.LastRowNum; rownum++)
            {
                IRow row = sheet.GetRow(rownum);
                if (row == null)
                    continue;

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    ICell cell = row.Cells[i];
                    int colnum = cell.ColumnIndex;

                    // in、out声明忽略
                    if (colnum == 0 || colnum == 1 || colnum == 3 || colnum == 4)
                        continue;

                    if (cell.CellType == CellType.Boolean || cell.CellType == CellType.Numeric)
                    {
                        sb.AppendLine(CodeTemplate.Get("template_value")
                            .Replace("[ROW]", (rownum + 1).ToString())
                            .Replace("[COL]", (colnum + 1).ToString())
                            .Replace("[VALUE]", cell.CellType == CellType.Boolean ? cell.BooleanCellValue.ToString().ToLower() : cell.NumericCellValue.ToString())
                            );
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.AppendLine(CodeTemplate.Get("template_func")
                            .Replace("[ROW]", (rownum + 1).ToString())
                            .Replace("[COL]", (colnum + 1).ToString())
                            .Replace("[CONTENT]", Formula2Code.Translate(cell.CellFormula, cell.ToString(), out about))
                            );

                        CellCoord cur = new CellCoord(rownum + 1, colnum + 1);
                        foreach (CellCoord cc in about)
                        {
                            if (!abouts.ContainsKey(cc))
                                abouts.Add(cc, new List<CellCoord>());
                            if (!abouts[cc].Contains(cur))
                                abouts[cc].Add(cur);
                        }
                    }
                }
            }

            // 数据影响关联递归统计
            bool change;
            do
            {
                change = false;
                foreach (var item in abouts)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        if (abouts.ContainsKey(item.Value[i]))
                        {
                            foreach (var c in abouts[item.Value[i]])
                            {
                                if (!item.Value.Contains(c))
                                {
                                    item.Value.Add(c);
                                    change = true;
                                }
                            }
                        }
                    }
                }
            } while (change);

            // 数据影响关联
            sb.AppendLine("\n-- cell data relation");
            foreach (var item in abouts)
            {
                sb.AppendLine(CodeTemplate.Get("template_about")
                    .Replace("[ROW]", item.Key.row.ToString())
                    .Replace("[COL]", item.Key.col.ToString())
                    .Replace("[CONTENT]", string.Join(",", item.Value.Select(CellCoord.ToString)))
                    );
            }

            // 枚举器
            sb.AppendLine("\n-- enumerator");
            sb.AppendLine("sheet.enumerator = {}");
            foreach (var item in FormulaEnumerator.GetList(sheet))
            {
                sb.AppendLine("sheet.enumerator." + item.name + " = {}");
                sb.AppendLine("sheet.enumerator." + item.name + ".start = " + (item.start + 1));
                sb.AppendLine("sheet.enumerator." + item.name + ".over = " + (item.end + 1));
                sb.AppendLine("sheet.enumerator." + item.name + ".div = " + item.div);
                sb.AppendLine("sheet.enumerator." + item.name + ".key = " + (item.key - 1));
                sb.AppendLine("sheet.enumerator." + item.name + ".propertys = {");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.AppendLine("\"" + item.propertys[i] + "\", --" + item.notes[i]);
                sb.AppendLine("}");
            }

            // 结果
            formulaContents.Add(sheet.SheetName.Substring(3), sb.ToString());
            return string.Empty;
        }


        public static string ExportLua(string exportDir)
        {
            // 公式写入文件
            string formulaDir = exportDir + "formula" + Path.DirectorySeparatorChar;
            if (Directory.Exists(formulaDir)) Directory.Delete(formulaDir, true);
            Directory.CreateDirectory(formulaDir);
            foreach (var formula in formulaContents)
                File.WriteAllText(formulaDir + formula.Key.ToLower() + ".lua", formula.Value, new UTF8Encoding(false));

            // 数据写入文件
            string dataDir = exportDir + "data" + Path.DirectorySeparatorChar;
            if (Directory.Exists(dataDir)) Directory.Delete(dataDir, true);
            Directory.CreateDirectory(dataDir);

            List<string> results = new List<string>();
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
                    List<string> groupDeclaras = new List<string>();
                    Dictionary<string, List<string>> groupids = new Dictionary<string, List<string>>();

                    foreach (var values in data.dataContent)
                    {
                        Dictionary<string, string[]>.Enumerator enumerator = data.groups.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            string groupDeclara = "data.groups[\"" + enumerator.Current.Key + "\"]";
                            if (!groupDeclaras.Contains(groupDeclara))
                                groupDeclaras.Add(groupDeclara);
                            for (int j = 0; j < enumerator.Current.Value.Length; j++)
                            {
                                object cv = values[data.groupindexs[enumerator.Current.Key][j]];
                                groupDeclara += "[" + (cv is string ? "\"" + cv + "\"" : cv) + "]";
                                if (!groupDeclaras.Contains(groupDeclara))
                                    groupDeclaras.Add(groupDeclara);
                            }
                            if (!groupids.ContainsKey(groupDeclara))
                                groupids.Add(groupDeclara, new List<string>());
                            groupids[groupDeclara].Add(values[0].ToString());
                        }
                    }

                    Dictionary<string, List<string>> groupsItemCount = new Dictionary<string, List<string>>();
                    foreach (var key in groupids.Keys)
                    {
                        string[] arr = key.Split(']');
                        string title = "";
                        for (int i = 0; i < arr.Length - 2; i++)
                        {
                            title += arr[i] + "]";
                            if (!groupsItemCount.ContainsKey(title))
                                groupsItemCount[title] = new List<string>();
                            if (!groupsItemCount[title].Contains(title + arr[i + 1] + "]"))
                                groupsItemCount[title].Add(title + arr[i + 1] + "]");
                        }
                    }

                    File.WriteAllText(dataDir + data.name.ToLower() + ".lua",
                        CodeTemplate.Get("template_data")
                        .Replace("[DATA_TABLE_NAME]", data.name)
                        .Replace("[DATA_KEYS]", string.Join(",\n", data.keys.Select(engname => { return "\"" + engname + "\"--[[" + data.keyNames[data.keys.IndexOf(engname)] + "]]"; })))
                        .Replace("[DATA_COLS]", string.Join("\n", data.cols.Select(i => { return "data.cols[" + (i + 1) + "] = " + (data.cols.IndexOf(i) + 1); })))
                        .Replace("[DATA_IDS]", string.Join(",", data.ids))
                        .Replace("[DECLARA_GROUPS]", string.Join("\n", groupDeclaras.Select(gd => { return gd + " = {}"; })))
                        .Replace("[DATA_GROUPS]", string.Join("\n", groupids.Select(o => { return o.Key + " = {" + string.Join(",", o.Value) + "}"; })))
                        .Replace("[DATA_GROUPS_COUNT]", string.Join("\n", groupsItemCount.Select(o =>
                        {
                            string nkey = o.Key.Replace("data.groups[", "data.groupscount[");
                            return nkey + " = {}\n" + nkey + ".count = " + o.Value.Count;
                        })))
                        .Replace("[DATA_LINES]", string.Join("\n", data.dataContent.Select(values =>
                        {
                            return "data.lines[" + values[0] + "] = {" + string.Join(",", values.Select(v =>
                            {
                                if (v is string) return "[[" + v + "]]";
                                if (v is string[]) return "{[[" + string.Join("]],[[", v as string[]) + "]]}";
                                if (v is int[]) return "{" + string.Join(",", v as int[]) + "}";
                                if (v is double[]) return "{" + string.Join(",", v as double[]) + "}";
                                return v;
                            })) + "}";
                        })))
                        , new UTF8Encoding(false));

                    lock (results)
                        results.Add(string.Empty);
                });
            }

            while (results.Count < datas.Values.Count)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            return string.Empty;
        }
    }
}
