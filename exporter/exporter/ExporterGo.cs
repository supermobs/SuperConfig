using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace exporter
{
    public static partial class Exporter
    {
        static void AppendGoDeclara(ISheet sheet, int col, bool canWrite, StringBuilder sb)
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

                sb.AppendLine("func (ins *" + sheet.SheetName + "FormulaSheet) Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() float64 {//" + note);
                sb.AppendLine("return ins.get(" + ((i + 1) * 1000 + col + 3) + ")");
                sb.AppendLine("}");

                if (canWrite)
                {
                    sb.AppendLine("func (ins *" + sheet.SheetName + "FormulaSheet) Set" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "(v float64) {//" + note);
                    sb.AppendLine("ins.set(" + ((i + 1) * 1000 + col + 3) + ",v)");
                    sb.AppendLine("}");
                }
            }
        }

        public static string DealWithFormulaSheetGo(ISheet sheet)
        {
            CodeTemplate.curlang = CodeTemplate.Langue.Go;

            StringBuilder sb = new StringBuilder();
            Dictionary<CellCoord, List<CellCoord>> abouts = new Dictionary<CellCoord, List<CellCoord>>();


            sb.AppendLine("package config");
            sb.AppendLine("type " + sheet.SheetName + "FormulaSheet struct {");
            sb.AppendLine("formulaSheet");
            sb.AppendLine("}");

            sb.AppendLine("var " + sheet.SheetName + "FormaulaTemplate *formulaSheetTemplate");
            sb.AppendLine("func init() {");
            sb.AppendLine(sheet.SheetName + "FormaulaTemplate = new(formulaSheetTemplate)");
            sb.AppendLine(sheet.SheetName + "FormaulaTemplate.datas = make(map[int32]float64)");
            sb.AppendLine(sheet.SheetName + "FormaulaTemplate.relation = make(map[int32][]int32)");
            sb.AppendLine(sheet.SheetName + "FormaulaTemplate.funcs = make(map[int32]func(*formulaSheet) float64)");

            // 数据内容
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
                        sb.AppendLine(sheet.SheetName + "FormaulaTemplate.datas[" + ((rownum + 1) * 1000 + colnum + 1) + "] = " + (cell.CellType == CellType.Boolean ? (cell.BooleanCellValue ? 1 : 0).ToString() : cell.NumericCellValue.ToString()));
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.AppendLine(sheet.SheetName + "FormaulaTemplate.funcs[" + ((rownum + 1) * 1000 + colnum + 1) + "] = func(ins *formulaSheet) float64 {");
                        sb.AppendLine("return " + Formula2Code.Translate(cell.CellFormula, cell.ToString(), out about));
                        sb.AppendLine("}");

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
            foreach (var item in abouts)
                sb.AppendLine(sheet.SheetName + "FormaulaTemplate.relation[" + (item.Key.row * 1000 + item.Key.col) + "] = []int32{" + string.Join(",", item.Value.Select(c => { return c.row * 1000 + c.col; })) + "}");
            sb.AppendLine("}");

            // 创建
            sb.AppendLine("func New" + sheet.SheetName.Substring(0, 1).ToUpper() + sheet.SheetName.Substring(1) + "Formula() *" + sheet.SheetName + "FormulaSheet {");
            sb.AppendLine("formula:= new(" + sheet.SheetName + "FormulaSheet)");
            sb.AppendLine("formula.template = " + sheet.SheetName + "FormaulaTemplate");
            sb.AppendLine("formula.datas = make(map[int32]float64)");
            sb.AppendLine("return formula");
            sb.AppendLine("}");


            // 声明
            AppendGoDeclara(sheet, 0, true, sb);
            AppendGoDeclara(sheet, 3, false, sb);

            // 枚举器
            foreach (var item in FormulaEnumerator.GetList(sheet))
            {
                // 写结构
                sb.AppendLine("type " + item.fullName + " struct {");
                sb.AppendLine("sheet *" + sheet.SheetName + "FormulaSheet");
                sb.AppendLine("line int");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.AppendLine(item.propertys[i] + " float64 // " + item.notes[i]);
                sb.AppendLine("}");

                // MoveNext
                sb.AppendLine("func (ins *" + item.fullName + ") MoveNext() bool {");
                sb.AppendLine("if ins.line <= 0 {");
                sb.AppendLine("ins.line = " + (item.start + 1) * 1000);
                sb.AppendLine("} else {");
                sb.AppendLine("ins.line = ins.line + " + item.div * 1000);
                sb.AppendLine("}");
                sb.AppendLine("if ins.line >= " + (item.end + 1) * 1000 + " {");
                sb.AppendLine("return false");
                sb.AppendLine("}");
                sb.AppendLine("");
                sb.AppendLine("if ins.sheet.get(ins.line+" + (6 + 1000 * (item.key - 1)) + ") == 0 {");
                sb.AppendLine("return ins.MoveNext()");
                sb.AppendLine("}");
                sb.AppendLine("");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.AppendLine("ins." + item.propertys[i] + " = ins.sheet.get(ins.line+" + (6 + 1000 * i) + ")");
                sb.AppendLine("return true");
                sb.AppendLine("}");
                sb.AppendLine("");

                // GetEnumerator
                sb.AppendLine("func (ins *" + sheet.SheetName + "FormulaSheet) Get" + item.name + "Enumerator() float64 {");
                sb.AppendLine("enumerator := &" + item.fullName + "{}");
                sb.AppendLine("enumerator.sheet = ins");
                sb.AppendLine("return enumerator");
                sb.AppendLine("}");
            }

            // 结果
            formulaContents.Add(sheet.SheetName.Substring(0, 1).ToUpper() + sheet.SheetName.Substring(1), sb.ToString());
            return string.Empty;
        }

        public static string ExportGo(string codeExportDir, string configExportDir)
        {
            // 目录清理
            if (Directory.Exists(configExportDir)) Directory.Delete(configExportDir, true);
            Directory.CreateDirectory(configExportDir);
            if (!Directory.Exists(codeExportDir)) Directory.CreateDirectory(codeExportDir);
            new DirectoryInfo(codeExportDir).GetFiles("data_*.go").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            new DirectoryInfo(codeExportDir).GetFiles("formula_*.go").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });

            // 类型转换
            Dictionary<string, string> typeconvert = new Dictionary<string, string>();
            typeconvert.Add("int", "int32");
            typeconvert.Add("string", "string");
            typeconvert.Add("double", "float64");

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "gofmt";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.ErrorDialog = true;

            List<Process> fmting = new List<Process>();

            // 写公式
            foreach (var formula in formulaContents)
            {
                File.WriteAllText(codeExportDir + "formula_" + formula.Key.ToLower() + ".go", formula.Value, new UTF8Encoding(false));
                info.Arguments = " -w " + codeExportDir + "formula_" + formula.Key.ToLower() + ".go";
                fmting.Add(Process.Start(info));
            }

            List<string> results = new List<string>();

            // 写go
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
                    string bigname = data.name.Substring(0, 1).ToUpper() + data.name.Substring(1);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("package config");
                    sb.AppendLine("import (");
                    sb.AppendLine("\"encoding/json\"");
                    sb.AppendLine("\"io/ioutil\"");
                    sb.AppendLine("\"log\"");
                    sb.AppendLine(")");

                    sb.AppendLine("type " + bigname + "Table struct {");
                    sb.AppendLine("Name string");
                    sb.AppendLine("Datas map[int32]" + bigname + "Config");
                    sb.AppendLine("Group " + bigname + "TableGroup");
                    sb.AppendLine("}");

                    sb.AppendLine("type " + bigname + "TableGroup struct {");
                    foreach (var g in data.groups)
                    {
                        sb.Append(g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1));
                        sb.Append(" ");
                        foreach (var t in g.Value)
                            sb.Append("map[" + typeconvert[data.types[data.keys.IndexOf(t)]] + "]");
                        sb.AppendLine("[]int32");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("type " + bigname + "Config struct {");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        sb.AppendLine(data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + " " + typeconvert[data.types[i]] + " " + "\"" + data.keyNames[i] + "\"");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("var _" + bigname + "Ins *" + bigname + "Table");
                    sb.AppendLine("func init(){");
                    sb.AppendLine("data, err := ioutil.ReadFile(config_dir+\"" + data.name + ".json\")");
                    sb.AppendLine("if err != nil { log.Fatal(err) }");
                    sb.AppendLine("_" + bigname + "Ins=new(" + bigname + "Table)");
                    sb.AppendLine("err = json.Unmarshal(data, _" + bigname + "Ins)");
                    sb.AppendLine("if err != nil { log.Fatal(err) }");
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("func Get" + bigname + "Table() *" + bigname + "Table {");
                    sb.AppendLine("return _" + bigname + "Ins");
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("func (ins *" + bigname + "Table) Get(id int32) *" + bigname + "Config {");
                    sb.AppendLine("data, ok:= ins.Datas[id]");
                    sb.AppendLine("if ok { return &data }");
                    sb.AppendLine("return nil");
                    sb.AppendLine("}");

                    foreach (var g in data.groups)
                    {
                        sb.AppendLine("");
                        sb.Append("func(ins * " + bigname + "Table) Get_" + g.Key.Replace("|", "_") + "(");
                        foreach (var t in g.Value)
                            sb.Append(t.Substring(0, 1).ToUpper() + t.Substring(1) + " " + typeconvert[data.types[data.keys.IndexOf(t)]] + ",");
                        sb.Remove(sb.Length - 1, 1);
                        sb.AppendLine(") *" + bigname + "Config {");

                        sb.Append("ids:= ins.Group." + g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1));
                        foreach (var t in g.Value)
                            sb.Append("[" + t.Substring(0, 1).ToUpper() + t.Substring(1) + "]");
                        sb.AppendLine();

                        sb.AppendLine("if len(ids) != 1 { return nil }");
                        sb.AppendLine("return ins.Get(ids[0]) }");
                    }


                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        if (data.types[i] == "string")
                            continue;
                        sb.AppendLine("func data_" + data.name + "_vlookup_" + (data.cols[i] + 1) + "(id float64) float64 {");
                        sb.AppendLine("return float64(Get" + bigname + "Table().Datas[int32(id)]." + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ")");
                        sb.AppendLine("}");
                    }

                    File.WriteAllText(codeExportDir + "data_" + data.name + ".go", sb.ToString());

                    lock (fmting)
                    {
                        info.Arguments = " -w " + codeExportDir + "data_" + data.name + ".go";
                        fmting.Add(Process.Start(info));
                    }

                    lock (results)
                        results.Add(string.Empty);
                });
            }



            // 写json
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
                    JObject config = new JObject();
                    config["Name"] = data.name;

                    JObject datas = new JObject();
                    config["Datas"] = datas;
                    foreach (var line in data.dataContent)
                    {
                        JObject ll = new JObject();
                        for (int j = 0; j < data.keys.Count; j++)
                            ll[data.keys[j]] = JToken.FromObject(line[j]);
                        datas[line[0].ToString()] = ll;
                    }

                    JObject group = new JObject();
                    config["Group"] = group;
                    Dictionary<string, string[]>.Enumerator enumerator = data.groups.GetEnumerator();
                    while (enumerator.MoveNext())
                        group[enumerator.Current.Key.Replace("|", "_")] = new JObject();
                    foreach (var values in data.dataContent)
                    {
                        enumerator = data.groups.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            JObject cur = group[enumerator.Current.Key.Replace("|", "_")] as JObject;
                            string key = string.Empty;
                            for (int j = 0; j < enumerator.Current.Value.Length - 1; j++)
                            {
                                key = values[data.groupindexs[enumerator.Current.Key][j]].ToString();
                                if (cur[key] == null)
                                    cur[key] = new JObject();
                                cur = cur[key] as JObject;
                            }
                            key = values[data.groupindexs[enumerator.Current.Key][enumerator.Current.Value.Length - 1]].ToString();
                            if (cur[key] == null)
                                cur[key] = new JArray();
                            (cur[key] as JArray).Add(JToken.FromObject(values[0]));
                        }
                    }

                    StringWriter textWriter = new StringWriter();
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    new JsonSerializer().Serialize(jsonWriter, config);
                    File.WriteAllText(configExportDir + data.name + ".json", textWriter.ToString());

                    lock (results)
                        results.Add(string.Empty);
                });
            }

            while (fmting.Count < formulaContents.Count + datas.Values.Count)
                Thread.Sleep(10);

            foreach (var pro in fmting)
                pro.WaitForExit();
            while (results.Count < datas.Values.Count * 2)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            return string.Empty;
        }
    }
}
