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
using System.Text.RegularExpressions;

namespace exporter
{
    public static partial class Exporter
    {
        static string FixFloat(string format)
        {
            Regex reg = new Regex("\\d+\\.\\d+(?!f)");
            return reg.Replace(format, match => match.Value + "f");
        }

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
                sb.Append("public float Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() { //" + note +"\r\n");
                sb.Append("\treturn get(" + ((i + 1) * 1000 + col * 1 + 3) + ");\r\n");
                sb.Append("}\r\n");

                if (canWrite)
                {
                    sb.Append("public void Set" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "(float v) {//" + note + "\r\n");
                    sb.Append("\tset(" + ((i + 1) * 1000 + col + 3) + ",v);\r\n");
                    sb.Append("}\r\n");
                }
            }
        }

        public static string DealWithFormulaSheetCS(ISheet sheet)
        {
            CodeTemplate.curlang = CodeTemplate.Langue.CS;

            StringBuilder sb = new StringBuilder();
            Dictionary<CellCoord, List<CellCoord>> abouts = new Dictionary<CellCoord, List<CellCoord>>();
            string sheetName = sheet.SheetName.Substring(3);
            string SheetName = sheetName.Substring(0, 1).ToUpper() + sheetName.Substring(1);

            string className = SheetName + "FormulaSheet";

            sb.Append("using System;\r\n");
            sb.Append("using UnityEngine;\r\n");
            sb.Append("using System.Collections;\r\n");
            sb.Append("using System.Collections.Generic;\r\n");
            sb.Append("\r\n");

            // 扩展Config类统一获取某个表的实例对象
            sb.Append("public partial class Config {\r\n");

            sb.Append("\tpublic static " + className + " New" + className +"(){\r\n" );
            sb.Append("\t\tvar formula = new " + className +"();\r\n");
            sb.Append("\t\tformula.Init();\r\n");
            sb.Append("\t\treturn formula;\r\n");
            sb.Append("\t}\r\n");
            sb.Append("}\r\n");

            //------

            // 开始生成这个配置表的算法类
            sb.Append("public class " + className + " : FormulaSheet { //定义数据表类开始\r\n");
            sb.Append("public void Init(){\r\n");

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
                        sb.Append("this.datas[" + ((rownum + 1) * 1000 + colnum + 1) + "] = " + (cell.CellType == CellType.Boolean ? (cell.BooleanCellValue ? 1 : 0).ToString() : cell.NumericCellValue.ToString()) +"f;\r\n");
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.Append("this.funcs[" + ((rownum + 1) * 1000 + colnum + 1) + "] = ins => {\r\n");

                        string content = Formula2Code.Translate(cell.CellFormula, cell.ToString(), out about);
                        if(CodeTemplate.curlang == CodeTemplate.Langue.CS)
                        {
                            content = FixFloat(content);
                        }

                        sb.Append("\treturn (float)" + content + ";\r\n");
                        sb.Append("};\r\n");

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
                sb.Append("this.relation[" + (item.Key.row * 1000 + item.Key.col) + "] = new int[]{" + string.Join(",", item.Value.Select(c => { return c.row * 1000 + c.col; })) + "};\r\n");
            sb.Append("} // 初始化数据结束\r\n");


            // 声明
            AppendCSDeclara(sheet, 0, true, sb);
            AppendCSDeclara(sheet, 3, false, sb);

            // 枚举器
            foreach (var item in FormulaEnumerator.GetList(sheet))
            {
                // 写结构
                sb.Append("public struct " + item.fullName + " {\r\n");

                // 属性
                sb.Append("\tpublic " + className + " sheet;\r\n");
                sb.Append("\t int line;\r\n");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.Append("\tfloat " + item.propertys[i] + "; // " + item.notes[i] + "\r\n");

                // 枚举方法
                sb.Append("public bool MoveNext() {\r\n");
                // MoveNext
                sb.Append("\tif (line <= 0) {\r\n");
                sb.Append("\t\tline = " + (item.start + 1) * 1000 + ";\r\n");
                sb.Append("\t} else {\r\n");
                sb.Append("\t\tline = line + " + item.div * 1000 + ";\r\n");
                sb.Append("}\r\n");
                sb.Append("\tif (line >= " + (item.end + 1) * 1000 + ") {\r\n");
                sb.Append("\t\treturn false;\r\n");
                sb.Append("}\r\n");
                sb.Append("\tif (sheet.get(line+" + (6 + 1000 * (item.key - 1)) + ") == 0 ) {\r\n");
                sb.Append("\t\treturn MoveNext();\r\n");
                sb.Append("}\r\n");
                for (int i = 0; i < item.propertys.Count; i++)
                    sb.Append("" + item.propertys[i] + " = sheet.get(line+" + (6 + 1000 * i) + ");\r\n");
                sb.Append("\treturn true;\r\n");
                sb.Append("} // 枚举方法next结束\r\n");
                sb.Append("} // 枚举struct定义结束\r\n");

                // GetEnumerator
                sb.Append("public " + item.fullName + " Get"+item.name + "Enumerator(){\r\n");
                sb.Append("\tvar enumerator = new " + item.fullName + "();\r\n");
                sb.Append("\t\tenumerator.sheet = this;\r\n");
                sb.Append("\t\treturn enumerator;\r\n");
                sb.Append("}\r\n");
            }

            sb.Append("}\r\n");

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
                    sb.Append("using System;\r\n");
                    sb.Append("using UnityEngine;\r\n");
                    sb.Append("using System.Collections;\r\n");
                    sb.Append("using System.Collections.Generic;\r\n");
                    sb.Append("using Newtonsoft.Json;\r\n");
                    sb.Append("using SuperMobs.Core;\r\n");
                    sb.Append("\r\n");

                    // 扩展Config类统一获取某个表的实例对象
                    sb.Append("public partial class Config {\r\n");

                    // 获取方法
                    sb.Append("\tstatic " + tableClassName + " _" +tableClassName + ";\r\n");
                    sb.Append("\tpublic static " + tableClassName + " Get" + tableClassName +"(){\r\n" );
                    sb.Append(string.Format("\tif({0} == null) Load{1}();\r\n","_"+tableClassName,tableClassName));
                    sb.Append("\t\treturn _" + tableClassName + ";\r\n");
                    sb.Append("\t}\r\n");

                    // 加载方法
                    sb.Append("\tpublic static void Load"+tableClassName+"(){\r\n");
                    sb.Append(string.Format("\t\tvar json = Service.Get<ILoaderService>().LoadConfig(\"{0}\");\r\n",data.name));
                    sb.Append(string.Format("\t\t{0} = JsonConvert.DeserializeObject<{1}>(json);\r\n","_"+tableClassName,tableClassName));
                    sb.Append("\t}\r\n");

                    // 清理方
                    sb.Append(string.Format("\tpublic static void Clear{0} () {{\r\n",tableClassName));
                    sb.Append(string.Format("\t\t_{0} = null;\r\n",tableClassName));
                    sb.Append("\t}\r\n");
                    lock (clearfuncs) clearfuncs.Add("Config.Clear" + tableClassName);

                    sb.Append("}\r\n");

                    //------

                    // group class
                    sb.Append("public class " + groupClassName + " {\r\n");
                    foreach (var g in data.groups)
                    {
                        sb.Append("\tpublic ");
                        //  Dictionary<int,Dic<int,Dic<int,[]int>>> 
                        foreach (var t in g.Value)
                            sb.Append("Dictionary<"+mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                        sb.Append("int[]");
                        foreach (var t in g.Value)
                            sb.Append(">");

                        sb.Append(" ");
                        sb.Append(g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1) + ";\r\n");
                        // per group value
                    }
                    sb.Append("}\r\n");

                    // config class
                    sb.Append("public class " + configClassName + " {\r\n");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        sb.Append("\tpublic " + typeconvert[data.types[i]] + " " + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + "; " + "// " + data.keyNames[i] + "\r\n");
                    }
                    sb.Append("}\r\n");

                    // table class
                    sb.Append("// " + string.Join(",", data.files) + "\r\n");
                    sb.Append("public class " + tableClassName + " {\r\n");
                    sb.Append("\tpublic string Name;\r\n");
                    sb.Append(string.Format("\tpublic Dictionary<int, {0}> _Datas;\r\n",configClassName));
                    sb.Append(string.Format("\tpublic {0} _Group;\r\n",groupClassName));


                    // get config function
                    sb.Append("public " + configClassName + " Get(int id) {\r\n");
                    sb.Append("\tif (_Datas.ContainsKey(id))\r\n");
                    sb.Append("\t\treturn _Datas[id];\r\n");
                    sb.Append("\treturn null;\r\n");
                    sb.Append("}\r\n");


                    // group data function
                    foreach (var g in data.groups)
                    {
                        sb.Append("\tpublic "+configClassName+"[]" +  " Get_" + g.Key.Replace("|", "_") + "(");
                        foreach (var t in g.Value)
                            sb.Append(mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + " " + t.Substring(0, 1).ToUpper() + t.Substring(1) + ",");
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(") {\r\n");


                        string oldDictName = "_Group."+g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        string oldKeyName = "";
                        for (int i = 0; i < g.Value.Length; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append("if (" + oldDictName + ".ContainsKey(");
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                sb.Append(oldKeyName + ") ){\r\n");
                            }
                            else
                            {
                                string tempName = "tmp" + (i - 1); 
                                sb.Append("var " + tempName + " = " + oldDictName + "[" + oldKeyName+"];\r\n");
                                sb.Append("if (" + tempName + ".ContainsKey(");
                                oldDictName = tempName;
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                sb.Append(oldKeyName + ") ){\r\n");
                            }
                        }

                        sb.Append("var ids = " + oldDictName + "[" + oldKeyName + "];\r\n");
                        sb.Append("var configs = new " + configClassName + "[" + oldDictName + ".Count];\r\n");
                        sb.Append("for (int i = 0; i < ids.Length; i++) {\r\n");
                        sb.Append("\tvar id = ids[i];\r\n");
                        sb.Append("\tconfigs[i] = Get(id);\r\n");
                        sb.Append("}\r\n");
                        sb.Append("return configs;\r\n");

                        for (int i = 0; i < g.Value.Length; i++)
                            sb.Append("}\r\n");
                        sb.Append("return new "+configClassName + "[0];\r\n");
                        sb.Append("}\r\n");
                    }

                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        if (data.types[i] == "string" || data.types[i].StartsWith("[]"))
                            continue;
                        sb.Append("\tpublic float data_" + data.name + "_vlookup_" + (data.cols[i] + 1) + "(float id) {\r\n");
                        sb.Append("\treturn (float)(Config.Get" + tableClassName + "()._Datas[(int)(id)]." + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ");\r\n");
                        sb.Append("}\r\n");
                    }

                    sb.Append("}\r\n"); // table class结束

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
            loadcode.Append("using System;\r\n");
            loadcode.Append("using UnityEngine;\r\n");
            loadcode.Append("using System.Collections;\r\n");
            loadcode.Append("using System.Collections.Generic;\r\n");
            loadcode.Append("\r\n");

            // 扩展Config类统一获取某个表的实例对象
            loadcode.Append("public partial class Config {\r\n");

            // load all
            loadcode.Append("\tpublic static void Load() {\r\n");
            foreach (var str in loadfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

            // clear all
            loadcode.Append("\tpublic static void Clear() {\r\n");
            foreach (var str in clearfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

            loadcode.Append("}\r\n");
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
