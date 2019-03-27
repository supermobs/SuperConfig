using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace exporter
{
    public static partial class Exporter
    {
        /// <summary>
        /// 声明CS
        /// </summary>
        static void AppendTSDeclara(ISheet sheet, int col, bool canWrite, StringBuilder sb)
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
                sb.Append("\t\tpublic Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() : number { //" + note + "\r\n");
                sb.Append("\t\t\treturn this.get(" + ((i + 1) * 1000 + col * 1 + 3) + ");\r\n");
                sb.Append("\t\t}\r\n");

                if (canWrite)
                {
                    sb.Append("\t\tpublic Set" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "(v:number) {//" + note + "\r\n");
                    sb.Append("\t\t\tthis.set(" + ((i + 1) * 1000 + col + 3) + ",v);\r\n");
                    sb.Append("\t\t}\r\n");
                }
            }
        }

        public static string DealWithFormulaSheetTS(ISheet sheet)
        {
            CodeTemplate.curlang = CodeTemplate.Langue.TS;

            StringBuilder sb = new StringBuilder();
            Dictionary<CellCoord, List<CellCoord>> abouts = new Dictionary<CellCoord, List<CellCoord>>();
            string sheetName = sheet.SheetName.Substring(3);
            string SheetName = sheetName.Substring(0, 1).ToUpper() + sheetName.Substring(1);

            string className = SheetName + "FormulaSheet";

            // module定义,让所有表都在一个模块里面
            sb.Append("/// <reference path=\"ConfigBase.ts\" />\r\n");
            sb.Append("namespace SuperConfig {\r\n");

            sb.Append("\texport function " + " New" + className + "():"+className + "{\r\n");
            sb.Append("\t\tvar formula = new " + className + "();\r\n");
            sb.Append("\t\tformula.Init();\r\n");
            sb.Append("\t\treturn formula;\r\n");
            sb.Append("\t}\r\n");

            //------

            // 开始生成这个配置表的算法类
            sb.Append("\texport class " + className + " extends FormulaSheet { //定义数据表类开始\r\n");
            sb.Append("\t\tpublic Init(){\r\n");

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
                        sb.Append("\t\t\tthis.datas.set(" + ((rownum + 1) * 1000 + colnum + 1) + "," + (cell.CellType == CellType.Boolean ? (cell.BooleanCellValue ? 1 : 0).ToString() : cell.NumericCellValue.ToString()) + ");\r\n");
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.Append("\t\t\tthis.funcs.set(" + ((rownum + 1) * 1000 + colnum + 1) + " , (ins) => {\r\n");

                        string content = Formula2Code.Translate(sheet, cell.CellFormula, cell.ToString(), out about);

                        // if (CodeTemplate.curlang == CodeTemplate.Langue.CS)
                        // {
                        //     content = FixFloat(content);
                        // }

                        sb.Append("\t\t\t\treturn " + content + ";\r\n");
                        sb.Append("\t\t\t});\r\n");

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
            {
                sb.Append("\t\t\tthis.relation.set(" + (item.Key.row * 1000 + item.Key.col) + ", [" + string.Join(",", item.Value.Select(c => { return c.row * 1000 + c.col; })) + "]);\r\n");
            }
            sb.Append("\t\t} // 初始化数据结束\r\n\r");


            // 声明
            AppendTSDeclara(sheet, 0, true, sb);
            AppendTSDeclara(sheet, 3, false, sb);

            // 枚举器
            // sb.Append("\r// 枚举器\r");
            // foreach (var item in FormulaEnumerator.GetList(sheet))
            // {
            //     // 写结构
            //     sb.Append("public struct " + item.fullName + " {\r\n");

            //     // 属性
            //     sb.Append("\tpublic " + className + " sheet;\r\n");
            //     sb.Append("\t int line;\r\n");
            //     for (int i = 0; i < item.propertys.Count; i++)
            //         sb.Append("\tfloat " + item.propertys[i] + "; // " + item.notes[i] + "\r\n");

            //     // 枚举方法
            //     sb.Append("public bool MoveNext() {\r\n");
            //     // MoveNext
            //     sb.Append("\tif (line <= 0) {\r\n");
            //     sb.Append("\t\tline = " + (item.start + 1) * 1000 + ";\r\n");
            //     sb.Append("\t} else {\r\n");
            //     sb.Append("\t\tline = line + " + item.div * 1000 + ";\r\n");
            //     sb.Append("}\r\n");
            //     sb.Append("\tif (line >= " + (item.end + 1) * 1000 + ") {\r\n");
            //     sb.Append("\t\treturn false;\r\n");
            //     sb.Append("}\r\n");
            //     sb.Append("\tif (sheet.get(line+" + (6 + 1000 * (item.key - 1)) + ") == 0 ) {\r\n");
            //     sb.Append("\t\treturn MoveNext();\r\n");
            //     sb.Append("}\r\n");
            //     for (int i = 0; i < item.propertys.Count; i++)
            //         sb.Append("" + item.propertys[i] + " = sheet.get(line+" + (6 + 1000 * i) + ");\r\n");
            //     sb.Append("\treturn true;\r\n");
            //     sb.Append("} // 枚举方法next结束\r\n");
            //     sb.Append("} // 枚举struct定义结束\r\n");

            //     // GetEnumerator
            //     sb.Append("public " + item.fullName + " Get" + item.name + "Enumerator(){\r\n");
            //     sb.Append("\tvar enumerator = new " + item.fullName + "();\r\n");
            //     sb.Append("\t\tenumerator.sheet = this;\r\n");
            //     sb.Append("\t\treturn enumerator;\r\n");
            //     sb.Append("}\r\n");
            // }

            sb.Append("\t}\r\n");

            // module结束符
            sb.Append("}\r\n");

            // 结果
            formulaContents.Add(SheetName, sb.ToString());

            return string.Empty;
        }

        public static string ExportTS(string codeExportDir, string configExportDir)
        {
            // 目录清理
            if (Directory.Exists(configExportDir))
            {
                new DirectoryInfo(configExportDir).GetFiles().ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            }
            else
            {
                Directory.CreateDirectory(configExportDir);
            }

            if (!Directory.Exists(codeExportDir)) Directory.CreateDirectory(codeExportDir);

            new DirectoryInfo(codeExportDir).GetFiles("data_*.ts").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            new DirectoryInfo(codeExportDir).GetFiles("formula_*.ts").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });

            // 类型转换
            Dictionary<string, string> typeconvert = new Dictionary<string, string>();
            typeconvert.Add("int", "number");
            typeconvert.Add("int32", "number");
            typeconvert.Add("string", "string");
            typeconvert.Add("double", "number");
            typeconvert.Add("float", "number");
            typeconvert.Add("long", "number");
            typeconvert.Add("number", "number");
            typeconvert.Add("[]int", "number[]");
            typeconvert.Add("[]int32", "number[]");
            typeconvert.Add("[]float", "number[]");
            typeconvert.Add("[]long", "number[]");
            typeconvert.Add("[]string", "string[]");
            typeconvert.Add("[]double", "number[]");

            // 索引类型转换
            Dictionary<string, string> mapTypeConvert = new Dictionary<string, string>();
            mapTypeConvert.Add("int", "number");
            mapTypeConvert.Add("int32", "number");
            mapTypeConvert.Add("string", "string");
            mapTypeConvert.Add("double", "string");
            mapTypeConvert.Add("float", "number");
            mapTypeConvert.Add("long", "number");
            mapTypeConvert.Add("float32", "string");
            mapTypeConvert.Add("number", "number");

            int goWriteCount = 0;
            List<string> loadfuncs = new List<string>();
            List<string> clearfuncs = new List<string>();

            // 写公式
            foreach (var formula in formulaContents)
            {
                File.WriteAllText(codeExportDir + "formula_" + formula.Key.ToLower() + ".ts", formula.Value, new UTF8Encoding(false));
                Interlocked.Increment(ref goWriteCount);
                // lock (loadfuncs) loadfuncs.Add("loadFormula" + formula.Key);
            }

            List<string> results = new List<string>();

            // 写代码文件
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
                    string bigname = data.name.Substring(0, 1).ToUpper() + data.name.Substring(1);
                    string tableClassName = bigname + "Table";
                    string configClassName = bigname + "Config";
                    string groupClassName = bigname + "TableGroup";

                    StringBuilder sb = new StringBuilder();

                    // 扩展Config类统一获取某个表的实例对象
                    sb.Append("/// <reference path=\"ConfigBase.ts\" />\r\n");
                    sb.Append("namespace SuperConfig {\r\n");

                    // 获取方法
                    sb.Append("\tvar "+ " _" + tableClassName +":"+tableClassName+ ";\r\n");
                    sb.Append("\texport function " + " Get" + tableClassName + "()"+ ":" + tableClassName + "{\r\n");
                    sb.Append(string.Format("\t\tif({0} == null)\r\n \t\t\tLoad{1}();\r\n", "_" + tableClassName, tableClassName));
                    sb.Append("\t\t\treturn _" + tableClassName + ";\r\n");
                    sb.Append("\t\t}\r\n");

                    // 加载方法
                    sb.Append("\texport function Load" + tableClassName + "(){\r\n");
                    // Laya.loader.getRes("res/config_data/laya.json");
                    // 配置表需要放到对应引擎的地址:res/config_data/
                    sb.Append(string.Format("\t\t{0} = LoadJsonFunc(\"{1}.json\");\r\n", "var json",data.name));
                    sb.Append(string.Format("\t\t _{0} = new {1}().init(json);\n",tableClassName,tableClassName));
                    sb.Append("\t}\r\n");

                    // 清理方
                    sb.Append(string.Format("\texport function Clear{0} () {{\r\n", tableClassName));
                    sb.Append(string.Format("\t\t_{0} = null;\r\n", tableClassName));
                    sb.Append("\t}\r\n");
                    lock (clearfuncs) clearfuncs.Add("this.Clear" + tableClassName);


                    //加载结束------

                    // group class
                    sb.Append("\texport class " + groupClassName + " {\r\n");
                    foreach (var g in data.groups)
                    {
                        // --------------------------------
                        sb.Append("\t// public ");
                        // Group的名称,合并参数后的,例如多个参数 ： a|b > a_b
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        sb.Append(gk+":");

                        //  Dictionary<int,Dic<int,Dic<int,[]int>>>
                        foreach (var t in g.Value)
                            //sb.Append("Dictionary<" + mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                            //这里先全部用string 现在用litjson 在下面类型判断的时候再去是否加tostring
                            sb.Append("Map<string" + ",");
                        //sb.Append("Dictionary<string" + ",");
                        sb.Append("number[]");
                        foreach (var t in g.Value)
                            sb.Append(">");

                        sb.Append(" ");
                        sb.Append(";\r\n");
                        // --------------------------------

                        sb.Append("\t\tpublic " + gk+":any;\n");

                        // per group value
                    }

                    // group 初始化方法
                    sb.Append("\t\tinit(d){\n");
                    foreach (var g in data.groups)
                    {
                        // 分组名称
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        sb.Append("\r\t\t\tthis." + gk+"=d."+gk+";\n");
                    }
                    sb.Append("\r\t\t\treturn this;\r\n");
                    sb.Append("\t\t}\r\n");

                    sb.Append("\t}\r\n"); // group class 结束

                    // config class
                    sb.Append("\texport class " + configClassName + " {\r\n");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        sb.Append("\t\tpublic " + " " + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ":"+typeconvert[data.types[i]]+ "; " + "// " + data.keyNames[i] + "\r\n");
                    }

                    // config初始化
                    sb.Append("\t\tinit(d){\n");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        var k = data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1);
                        sb.Append("\t\t\tthis." + k + "=d."+k+ "; " + "\n");
                    }
                    sb.Append("\r\t\t\t return this;\r\n");
                    sb.Append("\t\t}\r\n");

                    sb.Append("\t}\r\n\n"); // config class 结束

                    // table class
                    sb.Append("// " + string.Join(",", data.files) + "\r\n");
                    sb.Append("\texport class " + tableClassName + " {\r\n");
                    sb.Append("\t\tpublic Name:string;\r\n");
                    sb.Append(string.Format("\t\tpublic _Datas : Map<string, {0}>;\r\n", configClassName));
                    sb.Append(string.Format("\t\tpublic _Group:{0};\r\n", groupClassName));

                    // 写每一个group的缓存字典数据
                    foreach (var g in data.groups)
                    {
                        // Group的名称,合并参数后的
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        // --------------------------------
                        sb.Append("\t\tprivate " + gk+"_Cached:");
                        sb.Append("Map<string" + ",");
                        sb.Append(configClassName + "[]> " + " = new ");
                        sb.Append("Map<string" + "," + configClassName + "[]>();\r\n");
                        // --------------------------------
                    }

                    // init function 
                    sb.Append("\t\tinit(d){\n");
                    sb.Append("\r\r\t\t\tthis.Name = d.Name;\n");
                    var data_map_name = string.Format("Map<string, {0}>",configClassName);
                    sb.Append("\t\t\tthis._Datas = new " + data_map_name + "();\n");
                    sb.Append("\t\t\tlet keys = Object.keys(d._Datas);\n");
                    sb.Append("\t\t\tfor (let index = 0; index < keys.length; index++) {\n");
                    sb.Append("\t\t\t\tvar k = keys[index];\n");
                    sb.Append(string.Format("\t\t\t\tthis._Datas.set(k,new {0}().init(d._Datas[k]));\n", configClassName));
                    sb.Append("\t\t\t}\r\n");
                    
                    sb.Append(string.Format("\r\t\t\tthis._Group = new {0}().init(d._Group);\n", groupClassName));
                    sb.Append("\r\t\t\treturn this;\r\n");
                    sb.Append("\t\t}\r\n");

                    // get config function
                    sb.Append("\n\r\t\tpublic " + "Get(id:number) :" + configClassName + " {\r\n");
                    sb.Append("\t\t\tlet k:string = id.toString();\r\n");
                    sb.Append("\t\t\tif (this._Datas.has(k))\r\n");
                    sb.Append("\t\t\t\treturn this._Datas.get(k);\r\n");
                    sb.Append("\t\t\treturn null;\r\n");
                    sb.Append("\t\t}\r\n");

                    // group data function
                    foreach (var g in data.groups)
                    {
                        // Group的名称,合并参数后的
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);

                        sb.Append("\t\tpublic " + " Get_" + g.Key.Replace("|", "_") + "(");
                        foreach (var t in g.Value)
                            sb.Append(t.Substring(0, 1).ToUpper() + t.Substring(1) + ":" + mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                        sb.Remove(sb.Length - 1, 1); // 移除最后一个多余的,
                        sb.Append(")"+ ":" + configClassName + "[]"+ " {\r\n");

                        // 优化先在cach的字典里面判断
                        string cach_key = "";
                        foreach (var t in g.Value)
                        {
                            string vk = t.Substring(0, 1).ToUpper() + t.Substring(1);
                            cach_key += ("+"+vk+"+"+"\"_\"");
                        }
                        string cach_group_name = gk + "_Cached";
                        sb.Append("\t\t\tvar cach_key:string = " + cach_key + ";\r\n");
                        // sb.Append( "var ret:"+configClassName + "[]" + ";\r\n");
                        sb.Append("\t\t\tif(this." + cach_group_name + ".has(cach_key))\r\n");
                        sb.Append("\t\t\t\treturn this." + cach_group_name + ".get(cach_key);\r\n");

                        string oldDictName = "this._Group." + gk;
                        string oldKeyName = "";
                        for (int i = 0; i < g.Value.Length; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append("\t\t\tif (" + oldDictName + "[");
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                sb.Append(oldKeyName + ".toString()] ){\r\n");
                            }
                            else
                            {
                                string tempName = "tmp" + (i - 1);
                                sb.Append("\t\t\t\tvar " + tempName + " = " + oldDictName + "[" + oldKeyName + ".toString()];\r\n");
                                sb.Append("\t\t\t\tif (" + tempName + "[");
                                oldDictName = tempName;
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                sb.Append(oldKeyName + ".toString()] ){\r\n");
                            }
                        }

                        sb.Append("\t\t\t\tvar ids = " + oldDictName + "[" + oldKeyName + ".toString()];\r\n");
                        sb.Append("\t\t\t\tvar configs = [];\r\n");
                        sb.Append("\t\t\t\tfor (let i = 0; i < ids.length; i++) {\r\n");
                        sb.Append("\t\t\t\t\tvar id = ids[i];\r\n");
                        sb.Append("\t\t\t\t\tconfigs.push(this.Get(id));\r\n");
                        sb.Append("\t\t\t\t}\r\n");
                        // 缓存一下这次的结果
                        sb.Append("\t\t\t\tthis." + cach_group_name + "[cach_key]" + "=configs;\r\n");
                        sb.Append("\t\t\t\t\treturn configs;\r\n");
                        for (int i = 0; i < g.Value.Length; i++)
                            sb.Append("\t\t\t\t}\r\n");

                        sb.Append("\t\t\t\treturn null;\r\n");
                        sb.Append("\t\t}\r\n");
                    }

                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        if (data.types[i] == "string" || data.types[i].StartsWith("[]"))
                            continue;
                        sb.Append("\t\tpublic data_" + data.name + "_vlookup_" + (data.cols[i] + 1) + "(id:number) : number {\r\n");
                        sb.Append("\t\t\treturn Get" + tableClassName + "()._Datas.get(id.toString())." + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ";\r\n");
                        sb.Append("\t\t}\r\n");
                    }

                    sb.Append("\t}\r\n"); // table class结束

                    // module结束符
                    sb.Append("}\r\n");

                    File.WriteAllText(codeExportDir + "data_" + data.name + ".ts", sb.ToString());
                    Interlocked.Increment(ref goWriteCount);
                    lock (loadfuncs) loadfuncs.Add("this.Load" + tableClassName);

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
                            ll[data.keys[j].TitleToUpper()] = JToken.FromObject(line[j]);
                        datas[line[0].ToString()] = ll;
                    }

                    JObject group = new JObject();
                    config["_Group"] = group;
                    Dictionary<string, string[]>.Enumerator enumerator = data.groups.GetEnumerator();
                    while (enumerator.MoveNext())
                        group[enumerator.Current.Key.Replace("|", "_").TitleToUpper()] = new JObject();
                    foreach (var values in data.dataContent)
                    {
                        enumerator = data.groups.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            JObject cur = group[enumerator.Current.Key.Replace("|", "_").TitleToUpper()] as JObject;
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
                    var content = textWriter.ToString();
                    File.WriteAllText(configExportDir + data.name + ".json", content, new UTF8Encoding(false));

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

            // 扩展Config类统一获取某个表的实例对象
            loadcode.Append("/// <reference path=\"ConfigBase.ts\" />\r\n");
            loadcode.Append("namespace SuperConfig {\r\n");

            // load all
            loadcode.Append("\t function Load() {\r\n");
            foreach (var str in loadfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

            // clear all
            clearfuncs.Sort();
            loadcode.Append("\t function Clear() {\r\n");
            foreach (var str in clearfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

            loadcode.Append("}\r\n");
            File.WriteAllText(codeExportDir + "load.ts", loadcode.ToString());

            // 等待所有文件完成
            while (results.Count < datas.Values.Count * 2)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));

            return string.Empty;
        }
    }
}