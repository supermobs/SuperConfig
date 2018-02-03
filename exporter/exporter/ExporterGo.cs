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
                if (cell1 == null || cell2 == null || cell1.CellType == CellType.Blank || cell2.CellType == CellType.Blank)
                    continue;
                if (cell1.CellType != CellType.String || cell2.CellType != CellType.String)
                    throw new System.Exception("检查输入第" + (i + 1) + "行，sheetname=" + sheet.SheetName);
                string note = cell1.StringCellValue;
                string name = cell2.StringCellValue;
                if (string.IsNullOrEmpty(note) || string.IsNullOrEmpty(name) || name.StartsWith("_"))
                    continue;

                string sheetName = sheet.SheetName.Substring(3);
                string SheetName = sheetName.Substring(0, 1).ToUpper() + sheetName.Substring(1);
                sb.AppendLine("func (ins *" + SheetName + "FormulaSheet) Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() float32 {//" + note);
                sb.AppendLine("return ins.get(" + ((i + 1) * 1000 + col + 3) + ")");
                sb.AppendLine("}");

                if (canWrite)
                {
                    sb.AppendLine("func (ins *" + SheetName + "FormulaSheet) Set" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "(v float32) {//" + note);
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
            string sheetName = sheet.SheetName.Substring(3);
            string SheetName = sheetName.Substring(0, 1).ToUpper() + sheetName.Substring(1);

            sb.AppendLine("package config");
            sb.AppendLine("type " + SheetName + "FormulaSheet struct {");
            sb.AppendLine("formulaSheet");
            sb.AppendLine("}");

            sb.AppendLine("var " + sheetName + "FormaulaTemplate *formulaSheetTemplate");
            sb.AppendLine("func loadFormula" + SheetName + "() {");
            sb.AppendLine(sheetName + "FormaulaTemplate = new(formulaSheetTemplate)");
            sb.AppendLine(sheetName + "FormaulaTemplate.datas = make(map[int32]float32)");
            sb.AppendLine(sheetName + "FormaulaTemplate.relation = make(map[int32][]int32)");
            sb.AppendLine(sheetName + "FormaulaTemplate.funcs = make(map[int32]func(*formulaSheet) float32)");

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
                        sb.AppendLine(sheetName + "FormaulaTemplate.datas[" + ((rownum + 1) * 1000 + colnum + 1) + "] = " + (cell.CellType == CellType.Boolean ? (cell.BooleanCellValue ? 1 : 0).ToString() : cell.NumericCellValue.ToString()));
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.AppendLine(sheetName + "FormaulaTemplate.funcs[" + ((rownum + 1) * 1000 + colnum + 1) + "] = func(ins *formulaSheet) float32 {");
                        sb.AppendLine("return " + Formula2Code.Translate(sheet, cell.CellFormula, cell.ToString(), out about));
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
                sb.AppendLine(sheetName + "FormaulaTemplate.relation[" + (item.Key.row * 1000 + item.Key.col) + "] = []int32{" + string.Join(",", item.Value.Select(c => { return c.row * 1000 + c.col; })) + "}");
            sb.AppendLine("}");

            // 创建
            sb.AppendLine("func New" + SheetName + "Formula() *" + SheetName + "FormulaSheet {");
            sb.AppendLine("formula:= new(" + SheetName + "FormulaSheet)");
            sb.AppendLine("formula.template = " + sheetName + "FormaulaTemplate");
            sb.AppendLine("formula.datas = make(map[int32]float32)");
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
                sb.AppendLine("sheet *" + SheetName + "FormulaSheet");
                sb.AppendLine("line int32");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.AppendLine(item.propertys[i] + " float32 // " + item.notes[i]);
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
                sb.AppendLine("func (ins *" + SheetName + "FormulaSheet) Get" + item.name + "Enumerator() *" + item.fullName + " {");
                sb.AppendLine("enumerator := &" + item.fullName + "{}");
                sb.AppendLine("enumerator.sheet = ins");
                sb.AppendLine("return enumerator");
                sb.AppendLine("}");
            }

            // 结果
            formulaContents.Add(SheetName, sb.ToString());
            return string.Empty;
        }

        public static string ExportGo(string codeExportDir, string configExportDir)
        {
            // 目录清理
            if (Directory.Exists(configExportDir))
                new DirectoryInfo(configExportDir).GetFiles().ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            else
                Directory.CreateDirectory(configExportDir);
            if (!Directory.Exists(codeExportDir)) Directory.CreateDirectory(codeExportDir);
            new DirectoryInfo(codeExportDir).GetFiles("data_*.go").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            new DirectoryInfo(codeExportDir).GetFiles("formula_*.go").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });

            // 类型转换
            Dictionary<string, string> typeconvert = new Dictionary<string, string>();
            typeconvert.Add("int", "int32");
            typeconvert.Add("string", "string");
            typeconvert.Add("double", "float32");
            typeconvert.Add("[]int", "[]int32");
            typeconvert.Add("[]string", "[]string");
            typeconvert.Add("[]double", "[]float32");
            // 索引类型转换
            Dictionary<string, string> mapTypeConvert = new Dictionary<string, string>();
            mapTypeConvert.Add("int32", "int32");
            mapTypeConvert.Add("string", "string");
            mapTypeConvert.Add("float32", "string");

            int goWriteCount = 0;
            List<string> loadFormulaFuncs = new List<string>();
            Dictionary<string, string> loadTableFuncs = new Dictionary<string, string>();

            // 写公式
            foreach (var formula in formulaContents)
            {
                File.WriteAllText(codeExportDir + "formula_" + formula.Key.ToLower() + ".go", formula.Value, new UTF8Encoding(false));
                Interlocked.Increment(ref goWriteCount);
                lock (loadFormulaFuncs) loadFormulaFuncs.Add("loadFormula" + formula.Key);
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
                    sb.AppendLine(")");

                    data.files.Sort();
                    sb.AppendLine("// " + string.Join(",", data.files));
                    sb.AppendLine("type " + bigname + "Table struct {");
                    sb.AppendLine("Name string");
                    sb.AppendLine("Datas map[int32]*" + bigname + "Config");
                    sb.AppendLine("Group *" + bigname + "TableGroup");
                    sb.AppendLine("}");

                    sb.AppendLine("type " + bigname + "TableGroup struct {");
                    foreach (var g in data.groups)
                    {
                        sb.Append(g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1));
                        sb.Append(" ");
                        foreach (var t in g.Value)
                            sb.Append("map[" + mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + "]");
                        sb.AppendLine("[]int32");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("type " + bigname + "Config struct {");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        sb.AppendLine(data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + " " + typeconvert[data.types[i]] + " " + "// " + data.keyNames[i]);
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("var _" + bigname + "Ins *" + bigname + "Table");
                    sb.AppendLine("func loadSheet" + bigname + "(data []byte) error{");
                    lock (loadTableFuncs) loadTableFuncs.Add(data.name, "loadSheet" + bigname);
                    sb.AppendLine("tmp:=new(" + bigname + "Table)");
                    sb.AppendLine("err := json.Unmarshal(data,tmp)");
                    sb.AppendLine("if err != nil { return err }");
                    sb.AppendLine("_" + bigname + "Ins=tmp");
                    sb.AppendLine("return nil");
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("func Get" + bigname + "Table() *" + bigname + "Table {");
                    sb.AppendLine("return _" + bigname + "Ins");
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    sb.AppendLine("func (ins *" + bigname + "Table) Get(id int32) *" + bigname + "Config {");
                    sb.AppendLine("data, ok:= ins.Datas[id]");
                    sb.AppendLine("if ok { return data }");
                    sb.AppendLine("return nil");
                    sb.AppendLine("}");

                    foreach (var g in data.groups)
                    {
                        sb.AppendLine("");
                        sb.Append("func(ins * " + bigname + "Table) Get_" + g.Key.Replace("|", "_") + "(");
                        foreach (var t in g.Value)
                            sb.Append(t.Substring(0, 1).ToUpper() + t.Substring(1) + " " + mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                        sb.Remove(sb.Length - 1, 1);
                        sb.AppendLine(") []*" + bigname + "Config {");

                        for (int i = 0; i < g.Value.Length; i++)
                        {
                            sb.Append("if tmp" + i + ", ok:= ");
                            if (i == 0)
                                sb.Append("ins.Group." + g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1));
                            else
                                sb.Append("tmp" + (i - 1));
                            sb.AppendLine("[" + g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1) + "]; ok {");
                        }

                        sb.AppendLine("ids:= tmp" + (g.Value.Length - 1));
                        sb.AppendLine("configs := make([]*" + bigname + "Config, len(ids))");
                        sb.AppendLine("for i, id := range ids {");
                        sb.AppendLine("configs[i] = ins.Get(id)");
                        sb.AppendLine("}");
                        sb.AppendLine("return configs");

                        for (int i = 0; i < g.Value.Length; i++)
                            sb.AppendLine("}");
                        sb.AppendLine("return make([]*" + bigname + "Config, 0) }");
                    }


                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        if (data.types[i] == "string" || data.types[i].StartsWith("[]"))
                            continue;
                        sb.AppendLine("func data_" + data.name + "_vlookup_" + (data.cols[i] + 1) + "(id float32) float32 {");
                        sb.AppendLine("return float32(Get" + bigname + "Table().Datas[int32(id)]." + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ")");
                        sb.AppendLine("}");
                    }

                    File.WriteAllText(codeExportDir + "data_" + data.name + ".go", sb.ToString());
                    Interlocked.Increment(ref goWriteCount);

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
                    data.dataContent.Sort((a, b) => { return (int)a[0] - (int)b[0]; });
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
                    textWriter.NewLine = "\r\n";
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    new JsonSerializer().Serialize(jsonWriter, config);
                    File.WriteAllText(configExportDir + data.name + ".json", textWriter.ToString(), new UTF8Encoding(false));

                    lock (results)
                        results.Add(string.Empty);
                });
            }


            // 格式化go代码
            while (goWriteCount < formulaContents.Count + datas.Values.Count)
                Thread.Sleep(10);

            // 写加载

            StringBuilder loadcode = new StringBuilder();
            loadcode.AppendLine("package config");
            loadcode.AppendLine("import (");
            loadcode.AppendLine("\"time\"");
            loadcode.AppendLine("\"comm\"");
            loadcode.AppendLine("\"fmt\"");
            loadcode.AppendLine("\"github.com/name5566/leaf/log\"");
            loadcode.AppendLine(")");
            loadcode.AppendLine("func init(){");
            loadcode.AppendLine("defer cfg_init_success()");
            var keys = new List<string>(loadTableFuncs.Keys);
            keys.Sort();
            foreach (var key in keys)
                loadcode.AppendLine("cfg_regisiter(\"" + key + "\", " + loadTableFuncs[key] + ")");
            loadcode.AppendLine("}");
            loadcode.AppendLine("");
            loadcode.AppendLine("func Load(){");
            loadcode.AppendLine("start:= time.Now()");
            loadFormulaFuncs.Sort();
            foreach (var str in loadFormulaFuncs)
                loadcode.AppendLine(str + "()");
            loadcode.AppendLine();
            loadcode.AppendLine("ret,err:= LoadConfig(false)");
            loadcode.AppendLine("if err != nil {");
            loadcode.AppendLine("log.Fatal(\"%v\", err)");
            loadcode.AppendLine("}");
            loadcode.AppendLine("comm.RecountUseTime(start, fmt.Sprintf(\"load config(% v) success!!!\", len(ret)))");
            loadcode.AppendLine("}");
            File.WriteAllText(codeExportDir + "load.go", loadcode.ToString());

            // 格式化go代码
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "gofmt";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.ErrorDialog = false;
            info.Arguments = "-w " + codeExportDir;
            Process.Start(info).WaitForExit();

            // 等待所有文件完成
            while (results.Count < datas.Values.Count * 2)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));

            return string.Empty;
        }
    }
}
