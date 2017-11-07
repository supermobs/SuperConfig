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

        /// <summary>
        /// 声明CS
        /// </summary>
        static void AppendCSDeclara(ISheet sheet, int col, bool canWrite, StringBuilder sb)
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
                sb.AppendLine("public float Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() { //" + note);
                sb.AppendLine("\treturn get(" + ((i + 1) * 1000 + col * 1 + 3) + ");");
                sb.AppendLine("}\n");

                if (canWrite)
                {
                    sb.AppendLine("public void Set" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "(float v) {//" + note);
                    sb.AppendLine("\tset(" + ((i + 1) * 1000 + col + 3) + ",v);");
                    sb.AppendLine("}\n");
                }
            }
        }

        public static string DealWithFormulaSheetCS(ISheet sheet)
        {
            CodeTemplate.curlang = CodeTemplate.Langue.Go;

            StringBuilder sb = new StringBuilder();
            Dictionary<CellCoord, List<CellCoord>> abouts = new Dictionary<CellCoord, List<CellCoord>>();
            string sheetName = sheet.SheetName.Substring(3);
            string SheetName = sheetName.Substring(0, 1).ToUpper() + sheetName.Substring(1);

            string className = SheetName + "FormulaSheet";

            sb.AppendLine("using System;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("\n");

            // 扩展Config类统一获取某个表的实例对象
            sb.AppendLine("public partial class Config {");

            sb.AppendLine("\tpublic static " + className + " New" + className +"(){ " );
            sb.AppendLine("\t\tvar formula = new " + className +"();");
            sb.AppendLine("\t\treturn formula;");
            sb.AppendLine("\t}");

            sb.AppendLine("}\n");

            //------

            // 开始生成这个配置表的算法类
            sb.AppendLine("public class " + className + " : FormulaSheet { //定义数据表类开始");

            sb.AppendLine("public void Init(){");

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
                        sb.AppendLine("this.datas[" + ((rownum + 1) * 1000 + colnum + 1) + "] = " + (cell.CellType == CellType.Boolean ? (cell.BooleanCellValue ? 1 : 0).ToString() : cell.NumericCellValue.ToString()) +"f;");
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.AppendLine("this.funcs[" + ((rownum + 1) * 1000 + colnum + 1) + "] = ins => {");
                        sb.AppendLine("\treturn (float)" + Formula2Code.Translate(cell.CellFormula, cell.ToString(), out about) + ";");
                        sb.AppendLine("};\n");

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
                sb.AppendLine("this.relation[" + (item.Key.row * 1000 + item.Key.col) + "] = new int[]{" + string.Join(",", item.Value.Select(c => { return c.row * 1000 + c.col; })) + "};");
            sb.AppendLine("} // 初始化数据结束");


            // 声明
            AppendCSDeclara(sheet, 0, true, sb);
            AppendCSDeclara(sheet, 3, false, sb);

            // 枚举器
            foreach (var item in FormulaEnumerator.GetList(sheet))
            {
                // 写结构
                sb.AppendLine("public struct " + item.fullName + " {");

                // 属性
                sb.AppendLine("\tpublic " + className + " sheet;");
                sb.AppendLine("\t int line;");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.AppendLine("\tfloat " + item.propertys[i] + "; // " + item.notes[i]);

                // 枚举方法
                sb.AppendLine("public bool MoveNext() {");
                // MoveNext
                sb.AppendLine("\tif (line <= 0) {");
                sb.AppendLine("\t\tline = " + (item.start + 1) * 1000 + ";");
                sb.AppendLine("\t} else {");
                sb.AppendLine("\t\tline = line + " + item.div * 1000 + ";");
                sb.AppendLine("}\n");
                sb.AppendLine("\tif (line >= " + (item.end + 1) * 1000 + ") {");
                sb.AppendLine("\t\treturn false;");
                sb.AppendLine("}\n");
                sb.AppendLine("\tif (sheet.get(line+" + (6 + 1000 * (item.key - 1)) + ") == 0 ) {");
                sb.AppendLine("\t\treturn MoveNext();");
                sb.AppendLine("}");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.AppendLine("" + item.propertys[i] + " = sheet.get(line+" + (6 + 1000 * i) + ");");
                sb.AppendLine("\treturn true;");
                sb.AppendLine("} // 枚举方法next结束");
                sb.AppendLine("} // 枚举struct定义结束");

                // GetEnumerator
                sb.AppendLine("public " + item.fullName + " Get"+item.name + "Enumerator(){");
                sb.AppendLine("\tvar enumerator = new " + item.fullName + "();");
                sb.AppendLine("\t\tenumerator.sheet = this;");
                sb.AppendLine("\t\treturn enumerator;");
                sb.AppendLine("}\n");
            }

            sb.AppendLine("\n\n}");

            // 结果
            formulaContents.Add(SheetName, sb.ToString());
            return string.Empty;
        }

        public static string ExportCS(string codeExportDir, string configExportDir)
        {
            // 目录清理
            if (Directory.Exists(configExportDir))
                new DirectoryInfo(configExportDir).GetFiles().ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            else
                Directory.CreateDirectory(configExportDir);
            if (!Directory.Exists(codeExportDir)) Directory.CreateDirectory(codeExportDir);
            new DirectoryInfo(codeExportDir).GetFiles("data_*.cs").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            new DirectoryInfo(codeExportDir).GetFiles("formula_*.cs").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });

            // 类型转换
            Dictionary<string, string> typeconvert = new Dictionary<string, string>();
            typeconvert.Add("int", "int");
            typeconvert.Add("int32", "int");
            typeconvert.Add("string", "string");
            typeconvert.Add("double", "float");
            typeconvert.Add("[]int", "int[]");
            typeconvert.Add("[]int32", "int[]");
            typeconvert.Add("[]string", "string[]");
            typeconvert.Add("[]double", "float[]");

            // 索引类型转换
            Dictionary<string, string> mapTypeConvert = new Dictionary<string, string>();
            mapTypeConvert.Add("int", "int");
            mapTypeConvert.Add("float", "float");
            mapTypeConvert.Add("int32","int");
            mapTypeConvert.Add("string", "string");
            mapTypeConvert.Add("float32", "string");

            int goWriteCount = 0;
            List<string> loadfuncs = new List<string>();
            List<string> clearfuncs = new List<string>();

            // 写公式
            foreach (var formula in formulaContents)
            {
                File.WriteAllText(codeExportDir + "formula_" + formula.Key.ToLower() + ".cs", formula.Value, new UTF8Encoding(false));
                Interlocked.Increment(ref goWriteCount);
                // lock (loadfuncs) loadfuncs.Add("loadFormula" + formula.Key);
            }

            List<string> results = new List<string>();

            // 写cs
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
                    string bigname = data.name.Substring(0, 1).ToUpper() + data.name.Substring(1);
                    string tableClassName = bigname + "Table";
                    string configClassName = bigname + "Config";
                    string groupClassName = bigname+"TableGroup";

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("using System;");
                    sb.AppendLine("using UnityEngine;");
                    sb.AppendLine("using System.Collections;");
                    sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine("using Newtonsoft.Json;");
                    sb.AppendLine("using SuperMobs.Core;");
                    sb.AppendLine("\n");

                    // 扩展Config类统一获取某个表的实例对象
                    sb.AppendLine("public partial class Config {");

                    // 获取方法
                    sb.AppendLine("\tstatic " + tableClassName + " _" +tableClassName + ";");
                    sb.AppendLine("\tpublic static " + tableClassName + " Get" + tableClassName +"(){ " );
                    sb.AppendLine(string.Format("\tif({0} == null) Load{1}();","_"+tableClassName,tableClassName));
                    sb.AppendLine("\t\treturn _" + tableClassName + ";");
                    sb.AppendLine("\t}");

                    // 加载方法
                    sb.AppendLine("\tpublic static void Load"+tableClassName+"(){");
                    sb.AppendLine(string.Format("\t\tvar json = Service.Get<ILoaderService>().LoadConfig(\"{0}\");",data.name));
                    sb.AppendLine(string.Format("\t\t{0} = JsonConvert.DeserializeObject<{1}>(json);","_"+tableClassName,tableClassName));
                    sb.AppendLine("\t}");

                    // 清理方法
                    sb.AppendLine(string.Format("\tpublic static void Clear{0} () {{",tableClassName));
                    sb.AppendLine(string.Format("\t\t_{0} = null;",tableClassName));
                    sb.AppendLine("\t}");
                    lock (clearfuncs) clearfuncs.Add("Config.Clear" + tableClassName);

                    sb.AppendLine("}\n");

                    //------

                    // group class
                    sb.AppendLine("public class " + groupClassName + " {");
                    foreach (var g in data.groups)
                    {
                        sb.Append("\t");
                        sb.Append("public ");
                        //  Dictionary<int,Dic<int,Dic<int,[]int>>> 
                        foreach (var t in g.Value)
                            sb.Append("Dictionary<"+mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                        sb.Append("int[]");
                        foreach (var t in g.Value)
                            sb.Append(">");

                        sb.Append(" ");
                        sb.AppendLine(g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1) + ";");
                        // per group value
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    // config class
                    sb.AppendLine("public class " + configClassName + " {");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        sb.AppendLine("\tpublic " + typeconvert[data.types[i]] + " " + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + "; " + "// " + data.keyNames[i]);
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    // table class
                    sb.AppendLine("// " + string.Join(",", data.files));
                    sb.AppendLine("public class " + tableClassName + " {");
                    sb.AppendLine("\tpublic string Name;");
                    sb.AppendLine(string.Format("\tpublic Dictionary<int, {0}> _Datas;",configClassName));
                    sb.AppendLine(string.Format("\tpublic {0} _Group;",groupClassName));
                    sb.AppendLine("");

                    // get config function
                    sb.AppendLine("public " + configClassName + " Get(int id) {");
                    sb.AppendLine("\tif (_Datas.ContainsKey(id))");
                    sb.AppendLine("\t\treturn _Datas[id];");
                    sb.AppendLine("\treturn null;");
                    sb.AppendLine("}");
                    sb.AppendLine("");

                    // group data function
                    foreach (var g in data.groups)
                    {
                        sb.AppendLine("");
                        sb.Append("public "+configClassName+"[]" +  " Get_" + g.Key.Replace("|", "_") + "(");
                        foreach (var t in g.Value)
                            sb.Append(mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + " " + t.Substring(0, 1).ToUpper() + t.Substring(1) + ",");
                        sb.Remove(sb.Length - 1, 1);
                        sb.AppendLine(") {");


                        string oldDictName = "_Group."+g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        string oldKeyName = "";
                        for (int i = 0; i < g.Value.Length; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append("if (" + oldDictName + ".ContainsKey(");
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                sb.AppendLine(oldKeyName + ") ){");
                            }
                            else
                            {
                                string tempName = "tmp" + (i - 1); 
                                sb.AppendLine("var " + tempName + " = " + oldDictName + "[" + oldKeyName+"];");
                                sb.Append("if (" + tempName + ".ContainsKey(");
                                oldDictName = tempName;
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                sb.AppendLine(oldKeyName + ") ){");
                            }
                        }

                        sb.AppendLine("var ids = " + oldDictName + "[" + oldKeyName + "];");
                        sb.AppendLine("var configs = new " + configClassName + "[" + oldDictName + ".Count];");
                        sb.AppendLine("for (int i = 0; i < ids.Length; i++) {");
                        sb.AppendLine("\tvar id = ids[i];");
                        sb.AppendLine("\tconfigs[i] = Get(id);");
                        sb.AppendLine("}");
                        sb.AppendLine("return configs;");

                        for (int i = 0; i < g.Value.Length; i++)
                            sb.AppendLine("}");
                        sb.AppendLine("return new "+configClassName + "[0];");
                        sb.AppendLine("}");
                    }

                    sb.AppendLine("");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        if (data.types[i] == "string" || data.types[i].StartsWith("[]"))
                            continue;
                        sb.AppendLine("public float data_" + data.name + "_vlookup_" + (data.cols[i] + 1) + "(float id) {");
                        sb.AppendLine("\treturn (float)(Config.Get" + tableClassName + "()._Datas[(int)(id)]." + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ");");
                        sb.AppendLine("}");
                    }

                    sb.AppendLine("}"); // table class结束

                    File.WriteAllText(codeExportDir + "data_" + data.name + ".cs", sb.ToString());
                    Interlocked.Increment(ref goWriteCount);
                    lock (loadfuncs) loadfuncs.Add("Config.Load" + tableClassName);

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
                    config["_Datas"] = datas;
                    foreach (var line in data.dataContent)
                    {
                        JObject ll = new JObject();
                        for (int j = 0; j < data.keys.Count; j++)
                            ll[data.keys[j]] = JToken.FromObject(line[j]);
                        datas[line[0].ToString()] = ll;
                    }

                    JObject group = new JObject();
                    config["_Group"] = group;
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


            // 格式化go代码
            while (goWriteCount < formulaContents.Count + datas.Values.Count)
                Thread.Sleep(10);

            // 写加载
            loadfuncs.Sort();
            StringBuilder loadcode = new StringBuilder();
            loadcode.AppendLine("using System;");
            loadcode.AppendLine("using UnityEngine;");
            loadcode.AppendLine("using System.Collections;");
            loadcode.AppendLine("using System.Collections.Generic;");
            loadcode.AppendLine("\n");

            // 扩展Config类统一获取某个表的实例对象
            loadcode.AppendLine("public partial class Config {");

            // load all
            loadcode.AppendLine("\tpublic static void Load() {");
            foreach (var str in loadfuncs)
                loadcode.AppendLine("\t" + str + "();");
            loadcode.AppendLine("}");

            // clear all
            loadcode.AppendLine("\tpublic static void Clear() {");
            foreach (var str in clearfuncs)
                loadcode.AppendLine("\t" + str + "();");
            loadcode.AppendLine("}");

            loadcode.AppendLine("}");
            File.WriteAllText(codeExportDir + "load.cs", loadcode.ToString());

            // 格式化go代码
            // ProcessStartInfo info = new ProcessStartInfo();
            // info.FileName = "gofmt";
            // info.WindowStyle = ProcessWindowStyle.Hidden;
            // info.UseShellExecute = true;
            // info.ErrorDialog = false;
            // info.Arguments = "-w " + codeExportDir;
            // Process.Start(info).WaitForExit();

            // 等待所有文件完成
            while (results.Count < datas.Values.Count * 2)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));

            return string.Empty;
        }
    }
}
