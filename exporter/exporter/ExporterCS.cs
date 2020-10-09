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
        static string FixFloat(string format)
        {
            Regex reg = new Regex("\\d+\\.\\d+(?!f)");
            return reg.Replace(format, match => match.Value + "f");
        }

        public static string TitleToUpper(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            char[] s = str.ToCharArray();
            char c = s[0];

            if ('a' <= c && c <= 'z')
                c = (char)(c & ~0x20);

            s[0] = c;

            return new string(s);
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
                sb.Append("public float Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() { //" + note + "\r\n");
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

        static int CmpLen(List<object> l, List<object> r)
        {
            var sr = (string)r[1];
            var sl = (string)l[1];
            return sr.Length.CompareTo(sl.Length);
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

            sb.Append("\tpublic static " + className + " New" + className + "(){\r\n");
            sb.Append("\t\tvar formula = new " + className + "();\r\n");
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
                        sb.Append("this.datas[" + ((rownum + 1) * 1000 + colnum + 1) + "] = " + (cell.CellType == CellType.Boolean ? (cell.BooleanCellValue ? 1 : 0).ToString() : cell.NumericCellValue.ToString()) + "f;\r\n");
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.Append("this.funcs[" + ((rownum + 1) * 1000 + colnum + 1) + "] = ins => {\r\n");

                        string content = Formula2Code.Translate(sheet, cell.CellFormula, cell.ToString(), out about);
                        if (CodeTemplate.curlang == CodeTemplate.Langue.CS)
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
                sb.Append("public " + item.fullName + " Get" + item.name + "Enumerator(){\r\n");
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

            // stream加载的接口
            Dictionary<string,string> typestream_read = new Dictionary<string, string>();
            typestream_read.Add("int","ReadInt32");
            typestream_read.Add("string","ReadString");
            typestream_read.Add("double","ReadDouble");
            typestream_read.Add("int32","ReadInt32");

            // 类型转换
            Dictionary<string, string> typeconvert = new Dictionary<string, string>();
            typeconvert.Add("int", "int");
            typeconvert.Add("int32", "int");
            typeconvert.Add("string", "string");
            typeconvert.Add("double", "double");
            typeconvert.Add("[]int", "int[]");
            typeconvert.Add("[]int32", "int[]");
            typeconvert.Add("[]string", "string[]");
            typeconvert.Add("[]double", "double[]");
            typeconvert.Add("[]float", "float[]");

            // 索引类型转换
            Dictionary<string, string> mapTypeConvert = new Dictionary<string, string>();
            mapTypeConvert.Add("int", "int");
            mapTypeConvert.Add("float", "float");
            mapTypeConvert.Add("int32", "int");
            mapTypeConvert.Add("string", "string");
            mapTypeConvert.Add("float32", "float");
            mapTypeConvert.Add("double", "double");

            int goWriteCount = 0;
            List<string> loadfuncs = new List<string>();
            List<string> clearfuncs = new List<string>();
            List<string> savefuncs = new List<string>();

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
                    string groupClassName = bigname + "TableGroup";
                    string jsonfilename = bigname.ToLower();

                    StringBuilder sb = new StringBuilder();
                    // 通过禁用一些warnning的提示
                    sb.Append("#pragma warning disable 0219 // variable assigned but not used.\r\n");
                    sb.Append("#pragma warning disable 0168 // variable declared but not used.\r\n");
                    sb.Append("#pragma warning disable 0414 // private field assigned but not used.\r\n");

                    sb.Append("using System;\r\n");
                    sb.Append("using UnityEngine;\r\n");
                    sb.Append("using System.Collections;\r\n");
                    sb.Append("using System.Collections.Generic;\r\n");

                    // 使用Newtonsoft的json解析
                    // sb.Append("#if UNITY_EDITOR\r\n");
                    // sb.Append("using Newtonsoft.Json;\r\n");
                    // sb.Append("#endif\r\n");

                    // stream 需要io
                    sb.Append("using System.IO;\r\n");

                    // 使用c#热更的引用
                    // sb.Append("#if !UNITY_EDITOR\r\n");
                    // sb.Append("using SuperMobs.CoreExport;\r\n");
                    // sb.Append("#endif\r\n");
                    sb.Append("\r\n");

                    // 扩展Config类统一获取某个表的实例对象
                    sb.Append("public partial class Config {\r\n");

                    // 获取方法
                    sb.Append("\tstatic " + tableClassName + " _" + tableClassName + ";\r\n");
                    sb.Append("\tpublic static " + tableClassName + " Get" + tableClassName + "(){\r\n");
                    sb.Append(string.Format("\tif({0} == null) Load{1}();\r\n", "_" + tableClassName, tableClassName));
                    sb.Append("\t\treturn _" + tableClassName + ";\r\n");
                    sb.Append("\t}\r\n");

                    // 加载方法
                    sb.Append("\tpublic static void Load" + tableClassName + "(){\r\n");

                    sb.Append(string.Format("\tif(_{0} != null) return;\r\n\r\n",tableClassName));

                    //sb.Append(string.Format("\t\tvar json = Service.Get<ILoaderService>().LoadConfig(\"{0}\");\r\n", data.name));
                    //sb.Append(string.Format("\t\t{0} = LitJson.JsonMapper.ToObject<{1}>(json);\r\n", "_" + tableClassName, tableClassName));
                    //sb.Append(string.Format("\t\t{0} = JsonConvert.DeserializeObject<{1}>(json);\r\n","_"+tableClassName,tableClassName));

                    // ! 二进制加载方法
                    sb.Append("\tstring filename=\"" + jsonfilename+ "\";\r\n");

                    // sb.Append("\t#if UNITY_EDITOR\r\n");
                    // sb.Append("\tstring js_path=PATH_ASSETS_FOLDER+\"/\" + filename + \".bytes\";\r\n");
                    // sb.Append("\tbyte[] bys = ExportUtils.LoadConfigBytes(js_path);\r\n");
                    // sb.Append("\t#else\r\n");
                    sb.Append("\tbyte[] bys = delegateLoadConfigBytes(filename);\r\n");
                    // sb.Append("\t#endif\r\n");

                    sb.Append(string.Format("\t_{0} = new {0}();\r\n",tableClassName));
                    sb.Append(string.Format("\t_{0}.Init(bys);\r\n",tableClassName));

                    sb.Append("\t}\r\n");

                    // 清理方
                    sb.Append(string.Format("\tpublic static void Clear{0} () {{\r\n", tableClassName));
                    sb.Append(string.Format("\t\t_{0} = null;\r\n", tableClassName));
                    sb.Append("\t}\r\n");
                    lock (clearfuncs) clearfuncs.Add("Config.Clear" + tableClassName);

                    // 保存一次stream二进制流接口
                    sb.Append(string.Format("\tpublic static void Save{0} () {{\r\n", tableClassName));
                    sb.Append("\tstring filename=\"" + jsonfilename+ "\";\r\n");
                    sb.Append("\tList<string> dirs = new List<string>(Directory.GetDirectories(PATH_ASSETS_FOLDER));\r\n");
                    sb.Append("\tdirs.Add(PATH_ASSETS_FOLDER);\r\n");

                    sb.Append("\tforeach(var dir in dirs){\r\n");
                    sb.Append("\t\tstring js_path=dir+\"/\" + filename + \".json\";\r\n");
                    sb.Append("\t\tif(File.Exists(js_path) == false) continue; \r\n");
                    sb.Append("\t\tstring js=delegateLoadConfigJson(js_path);\r\n");
                    sb.Append(string.Format("\t\tvar val= Newtonsoft.Json.JsonConvert.DeserializeObject<{0}>(js);\r\n",tableClassName));
                    sb.Append("\t\tvar bys=val.ToBytes();\r\n");
                    sb.Append("\t\tstring save_path=dir+\"/\" + filename + \".bytes\";\r\n");
                    sb.Append("\t\tFile.WriteAllBytes(save_path,bys);\r\n");
                    sb.Append("\t}\r\n");
                    sb.Append("\t}\r\n");

                    lock (savefuncs) savefuncs.Add("Config.Save" + tableClassName);
                    sb.Append("}\r\n"); // config扩展接口类结束

                    //------

                    // group class
                    sb.Append("public class " + groupClassName + " : StreamConfig " + " {\r\n");
                    foreach (var g in data.groups)
                    {
                        // --------------------------------
                        sb.Append("\tpublic ");
                        //  Dictionary<int,Dic<int,Dic<int,[]int>>>
                        foreach (string t in g.Value)
                            sb.Append("Dictionary<" + mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                            //这里先全部用string 现在用 litjson 在下面类型判断的时候再去是否加tostring
                            // sb.Append("Dictionary<string" + ",");
                        sb.Append("int[]");
                        foreach (var t in g.Value)
                            sb.Append(">");

                        sb.Append(" ");

                        // Group的名称,合并参数后的,例如多个参数 ： a|b > a_b
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);

                        sb.Append(gk + ";\r\n");
                        // --------------------------------
                        // per group value
                    }

                    // ! 根据需要跳过的某个位置的group获取类型嵌套的类型
                    Func<int,string[],string> _func_get_group_type_by_index = (_gi,_gv) => {
                        string gt = "";
                        for (int i = 0; i < _gv.Length; i++)
                        {
                            if(i < _gi)
                            {
                                continue;
                            }
                            string t = _gv[i];
                            gt += ("Dictionary<" + mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + ",");
                            //这里先全部用string 现在用 litjson 在下面类型判断的时候再去是否加tostring
                            // sb.Append("Dictionary<string" + ",");
                        }
                        gt += "int[]";
                        for (int i = 0; i < _gv.Length; i++)
                        {
                            if(i < _gi)
                            {
                                continue;
                            }
                            gt += ">";
                        }
                        return gt;
                    };
                    Func<int,string[],string> _func_get_group_keytype_by_index = (_gi,_gv)=>{
                        string gk = "";
                        for (int i = 0; i < _gv.Length; i++)
                        {
                            if(i < _gi)
                            {
                                continue;
                            }
                            string t = _gv[i];
                            gk = mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]];
                            break;
                        }
                        return gk;
                    };
                    
                    // ! > stream 读取
                    sb.Append("\tpublic override void FromStream(BinaryReader br){\r\n");
                    // 先定义一个长度
                    sb.Append("\t\tint _count = 0;\r\n");

                    foreach (var g in data.groups)
                    {
                        // Group的名称,合并参数后的,例如多个参数 ： a|b > a_b
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);

                        // 先获取这个group的类型
                        string gt = _func_get_group_type_by_index(0,g.Value);

                        // 初始化group长度
                        sb.Append("\t\t_count=br.ReadInt32();\r\n");
                        sb.Append("\t\t" + gk + "=new " + gt + "(_count);\r\n");
                        sb.Append("\t\tfor(int i=0; i < _count; i++){\r\n");
                        int _value_num = g.Value.Length;
                        if(_value_num == 1)
                        {
                            string _k = _func_get_group_keytype_by_index(0,g.Value);
                            sb.Append(string.Format("\t\t\t{0} k"+" = br.{1}();\r\n",_k,typestream_read[_k]));
                            sb.Append("\t\t\tint[] v"+"=null;\r\n");
                            sb.Append("\t\t\tReadArray(br,ref v"+");\r\n");
                            sb.Append("\t\t\t" + gk +"[k]=v;\r\n");
                        }
                        else
                        {
                            for (int i = 0; i < _value_num; i++)
                            {
                                // key
                                string _k = _func_get_group_keytype_by_index(i,g.Value);
                                sb.Append(string.Format("\t\t\t{0} k"+i +" = br.{1}();\r\n",_k,typestream_read[_k]));

                                if(i != _value_num -1)
                                {
                                    // 还没到最后
                                    sb.Append("\t\t\tint _count"+i+"=br.ReadInt32();\r\n");
                                    string _gt = _func_get_group_type_by_index(i+1,g.Value);
                                    sb.Append("\t\t\t"+_gt + " v"+i+ " = new "+_gt+"(_count"+i+");\r\n" );
                                    sb.Append("\t\t\tfor(int i"+i+ "=0; i"+i+" < _count"+i+"; i"+i+"++) {\r\n");
                                } 
                                else
                                {
                                    // 最后一个是int[]
                                    sb.Append("\t\t\tint[] v"+i+"=null;\r\n");
                                    sb.Append("\t\t\tReadArray(br,ref v"+i+");\r\n");
                                }
                            }
                            // 加反括号和设置v
                            for (int i = _value_num-1; i >= 1; i--)
                            {
                                sb.Append("\t\t\tv"+(i-1)+"[k"+(i)+"]=v"+(i)+";\r\n");
                                sb.Append("}\r\n");
                            }
                            
                            // 设置最外面那个字典是第一个值
                            sb.Append("\t\t\t"+gk+"[k0]=v0;\r\n");
                        }
                        sb.Append("}\r\n");
                    }
                    sb.Append("}\r\n");


                    // ! > stream 写入
                    sb.Append("\tpublic override void ToStream(BinaryWriter bw){\r\n");
                    foreach (var g in data.groups)
                    {
                        // Group的名称,合并参数后的,例如多个参数 ： a|b > a_b
                        string gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        string gt = _func_get_group_type_by_index(0,g.Value);

                        // 先写长度
                        sb.Append(string.Format("\t\tif({0} == null) {0} = new {1}();\r\n",gk,gt));
                        sb.Append("\t\t" + "bw.Write(" +gk + ".Count" + ");\r\n");
                        // 开始写内容
                        int _value_num = g.Value.Length;
                        sb.Append("\t\tforeach(var item1 in " + gk + ") {\r\n");
                        sb.Append("\t\tbw.Write(item1.Key);\r\n");
                        if(_value_num == 1)
                        {
                            sb.Append("\t\tint[] v=item1.Value;\r\n");
                            sb.Append("\t\tWriteArray(bw,ref v);\r\n");
                        }
                        else if(_value_num > 1)
                        {
                            sb.Append("\t\tbw.Write(item1.Value.Count);\r\n");
                            // 需要多次foreach去写入
                            for (int i = 2; i <= _value_num; i++)
                            {
                                sb.Append("\t\t\tforeach(var item" + i + " in item" + (i -1) +".Value){\r\n" );
                                sb.Append("\t\t\tbw.Write(item" +i+ ".Key" + ");\r\n");
                                if(i != _value_num)
                                {
                                    // 不是最后一个继续foreach,所以还是写长度
                                    sb.Append("\t\t\tbw.Write(item" +i+ ".Value.Count" + ");\r\n");
                                }
                                else
                                {
                                    // 最后一个设置那个int[]了
                                    sb.Append("\t\t\tint[] v=item"+i +".Value;\r\n");
                                    sb.Append("\t\t\tWriteArray(bw,ref v);\r\n");
                                }
                            }

                            // 再加那个反括号
                            for (int i = 2; i <= _value_num; i++)
                            {
                                sb.Append("\t\t}\r\n");
                            }
                        }
                        else
                        {
                            throw new Exception("the group is error value size is not morethan 1    group =" + gk);
                        }
                        sb.Append("\t}\r\n");
                    }
                    sb.Append("}\r\n");


                    sb.Append("}\r\n");

                    // config class
                    sb.Append("public class " + configClassName + " : StreamConfig " + " {\r\n");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        sb.Append("\tpublic " + typeconvert[data.types[i]] + " " + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + "; " + "// " + data.keyNames[i] + "\r\n");
                    }
                    // ! > steam加载 读取
                    sb.Append("\tpublic override void FromStream(BinaryReader br){\r\n");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        string t = typeconvert[data.types[i]];
                        string n = data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1);
                        if(t.Contains("[]"))
                        {
                            // 数组
                            sb.Append("\t" + "ReadArray(br,ref " + n + ");\r\n");
                        }
                        else
                        {
                            sb.Append("\t" + n + "=br." + typestream_read[data.types[i]] + "();\r\n");
                        }
                    }
                    sb.Append("}\r\n");
                    // ! > stream 写入
                    sb.Append("\tpublic override void ToStream(BinaryWriter bw){\r\n");
                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        string t = typeconvert[data.types[i]];
                        string n = data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1);
                        if(t.Contains("[]"))
                        {
                            sb.Append("\t" + "WriteArray(bw,ref " + n + ");\r\n");
                        }
                        else if(t.Contains("string"))
                        {
                            sb.Append("\tbw.Write(!string.IsNullOrEmpty(" + n + ") ? " + n + ":\"\"" + ");\r\n");
                        }
                        else
                        {
                            sb.Append("\tbw.Write(" + n + ");\r\n");
                        }
                    }
                    sb.Append("}\r\n");


                    sb.Append("}\r\n");

                    // table class
                    sb.Append("// " + string.Join(",", data.files) + "\r\n");
                    sb.Append("public class " + tableClassName + " : StreamConfig " + " {\r\n");
                    sb.Append("\tpublic string Name;\r\n");
                    sb.Append(string.Format("\tpublic Dictionary<int, {0}> _Datas;\r\n", configClassName));
                    sb.Append(string.Format("\tpublic {0} _Group;\r\n", groupClassName));

                    // 写每一个group的缓存字典数据
                    foreach (var g in data.groups)
                    {
                        // Group的名称,合并参数后的
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);
                        // --------------------------------
                        sb.Append("\tprivate ");
                        sb.Append("Dictionary<string" + ",");
                        sb.Append(configClassName + "[]> " + gk + "_Cached" + " = new ");
                        sb.Append("Dictionary<string" + "," + configClassName + "[]>();\r\n");
                        // --------------------------------
                    }

                    // 增加二进制流动态读取的结构和对象
                    sb.Append("\tprivate Dictionary<int, int> _Data_Pos;\r\n");
                    sb.Append("\tprivate BinaryReader _br;\r\n");
                    sb.Append("\tprivate long _start_data_pos;\r\n");
                    sb.Append("\tprivate byte[] _bytes;\r\n");
                    sb.Append("\tprivate bool isInitAll;\r\n");
                    sb.Append("\tpublic int Count;\r\n");

                    if (data.funcDatas.Count > 0)
                    {
                        foreach (var item in data.funcDatas)
                        {
                            var value = item.Value;
                            sb.Append(string.Format("\tprivate Dictionary<int, Action<{0}>> _data_{1};\r\n", value.funcType, value.funcName));
                        }
                    }

                    sb.Append(string.Format("\tpublic Dictionary<int, {0}> GetFullDatas()",configClassName) + "{\r\n");
                    sb.Append("\t\tif (isInitAll == false){ \r\n");
                    sb.Append("\t\t\tisInitAll = true;\r\n");
                    sb.Append("\t\t\tforeach (var item in _Data_Pos){ ReadConfig(item.Key); }\r\n");
                    sb.Append("\t}\r\n");
                    sb.Append("\t\treturn _Datas;\r\n");
                    sb.Append("}\r\n");

                    sb.Append("\tpublic void Init(byte[] bs){\r\n");
                    sb.Append("\t\t_bytes = bs;\r\n");
                    sb.Append("\t\t_br = new BinaryReader(new MemoryStream(bs));\r\n");
                    sb.Append("\t\tthis.FromBytes(bs);\r\n");
                    sb.Append("}\r\n");

                    sb.Append("\t" + configClassName + " ReadConfig(int id) { \r\n");
                    sb.Append("\t\t" + configClassName + " cfg = null;\r\n");
                    sb.Append("\t\tif (_Datas.TryGetValue(id, out cfg)) return cfg;\r\n");
                    sb.Append("\t\tif (_Data_Pos.ContainsKey(id) == false) return null;\r\n");
                    sb.Append("\t\tcfg = new " + configClassName + "();\r\n");
                    sb.Append("\t\tint p = _Data_Pos[id];\r\n");
                    sb.Append("\t\t_br.BaseStream.Position = _start_data_pos + p;\r\n");
                    sb.Append("\t\tcfg.FromStream(_br);\r\n");
                    sb.Append("\t\t_Datas[id] = cfg; \r\n");
                    sb.Append("\t\treturn cfg;\r\n");
                    sb.Append("}\r\n");

                    // ! > stream 加载
                    sb.Append("\tpublic override void FromStream(BinaryReader br){\r\n");
                    sb.Append("\t\tName=br.ReadString();\r\n");
                    sb.Append("\t\t_Group=new " + groupClassName+"();\r\n");
                    sb.Append("\t\t_Group.FromStream(br);\r\n");
                    sb.Append("\t\tint _datas_count=br.ReadInt32();\r\n");
                    sb.Append("\t\tCount = _datas_count;\r\n");
                    sb.Append(string.Format("\t\t_Datas=new Dictionary<int, {0}>();\r\n",configClassName));
                    sb.Append("\t\t_Data_Pos = new Dictionary<int, int>(_datas_count);\r\n");
                    sb.Append("\t\tfor(int i=0; i < _datas_count; i++){\r\n");
                    sb.Append("\t\tint _id=br.ReadInt32();\r\n");
                    sb.Append("\t\tint _pos=br.ReadInt32();\r\n");
                    sb.Append("\t\t_Data_Pos[_id] = _pos;\r\n");
                    sb.Append("\t\t}\r\n");
                    sb.Append("\t\t_start_data_pos = br.BaseStream.Position;\r\n");
                    sb.Append("}\r\n");

                    // ! < stream 写入
                    sb.Append("\tpublic override void ToStream(BinaryWriter bw){\r\n");
                    sb.Append("\t\tbw.Write(Name);\r\n");
                    sb.Append("\t\t_Group.ToStream(bw);\r\n");
                    sb.Append("\t\tbw.Write(_Datas.Count);\r\n");

                    sb.Append("\t\tList<int> _ids = new List<int>();\r\n");
                    sb.Append("\t\tList<int> _idlens = new List<int>();\r\n");
                    sb.Append("\t\tList<byte[]> _idbys = new List<byte[]>();\r\n");
                    sb.Append("\t\tint _idpos = 0;\r\n");
                    sb.Append("\t\tforeach(var item in _Datas){\r\n");
                    sb.Append("\t\t\t_ids.Add(item.Key);\r\n");
                    sb.Append("\t\t\tvar bs = item.Value.ToBytes();\r\n");
                    sb.Append("\t\t\t_idbys.Add(bs);\r\n");
                    sb.Append("\t\t\t_idlens.Add(_idpos);\r\n");
                    sb.Append("\t\t\t_idpos += bs.Length;\r\n");
                    sb.Append("\t}\r\n");

                    sb.Append("\t\tfor (int i = 0; i < _ids.Count; i++){\r\n");
                    sb.Append("\t\t\tbw.Write(_ids[i]);\r\n");
                    sb.Append("\t\t\tbw.Write(_idlens[i]);\r\n");
                    sb.Append("\t}\r\n");

                    sb.Append("\t\tfor (int i = 0; i < _ids.Count; i++){\r\n");
                    sb.Append("\t\t\tbw.Write(_idbys[i]);\r\n");
                    sb.Append("\t}\r\n");

                    sb.Append("}\r\n");

                    // get config function
                    sb.Append("public " + configClassName + " Get(int id) {\r\n");
                    sb.Append("\t\treturn ReadConfig(id);\r\n");
                    sb.Append("}\r\n");

                    if (data.funcDatas.Count > 0)
                    {
                        Console.WriteLine(data.name);
                        foreach (var item in data.funcDatas)
                        {
                            var value = item.Value;
                            string envName = string.Format("env_{0}_{1}_cs", data.name, value.funcName);
                            DataStruct env = null;
                            foreach (var e in datas)
                            {
                                if (e.Value.name == envName)
                                {
                                    env = e.Value;
                                    env.dataContent.Sort(CmpLen);
                                    break;
                                }
                            }
                            sb.Append(string.Format("\tpublic Action<{0}> Get_{1}(int id) {{\r\n", value.funcType, value.funcName));
                            sb.Append(string.Format("\t\tif (_data_{0} == null) {{\r\n", value.funcName));
                            sb.Append(string.Format("\t\t\t_data_{0} = new Dictionary<int, Action<{1}>>();\r\n", value.funcName, value.funcType));
                            var list = item.Value.data;
                            for (int i = 0; i < list.Count; ++i)
                            {
                                var id = data.dataContent[i][0];
                                // sb.Append(string.Format("\t\t\t/* {0} */", list[i])); // 翻译文字
                                Console.WriteLine(i + " " + id);
                                sb.Append(string.Format("\t\t\t_data_{0}[{1}] = delegate({2}) {{\r\n", value.funcName, id, value.funcTypeParam));
                                Console.WriteLine(list[i]);
                                var trans_list = list[i].Replace("\n", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); // aaa=bbb;ccc=ddd+eee;
                                foreach (var line in trans_list)
                                {
                                    Console.WriteLine(line);
                                    var eqarr = line.Split('=');
                                    sb.Append(string.Format("\t\t\t\t// {0}\r\n", line));
                                    sb.Append(string.Format("\t\t\t\t{0}={1};\r\n", env.TranslateLeft(eqarr[0]), env.TranslateRight(eqarr[1])));
                                }
                                sb.Append("\t\t\t};\r\n"); // enddelegate
                            }
                            sb.Append("\t\t}\r\n"); // endif
                            sb.Append(string.Format("\t\treturn _data_{0}[id];\r\n", value.funcName));
                            sb.Append("\t}\r\n");
                            sb.Append("\r\n");
                        }
                    }

                    // group data function
                    foreach (var g in data.groups)
                    {
                        // Group的名称,合并参数后的
                        var gk = g.Key.Substring(0, 1).ToUpper() + g.Key.Replace("|", "_").Substring(1);

                        sb.Append("\tpublic " + configClassName + "[]" + " Get_" + g.Key.Replace("|", "_") + "(");
                        foreach (var t in g.Value)
                            sb.Append(mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(t)]]] + " " + t.Substring(0, 1).ToUpper() + t.Substring(1) + ",");
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(") {\r\n");



                        // 优化先在cach的字典里面判断
                        string cach_key = "string.Empty";
                        foreach (var t in g.Value)
                        {
                            string vk = t.Substring(0, 1).ToUpper() + t.Substring(1);
                            cach_key += ("+"+vk+"+"+"\"_\"");
                        }
                        string cach_group_name = gk + "_Cached";
                        sb.Append("string cach_key = " + cach_key + ";\r\n");
                        sb.Append(configClassName + "[] ret;\r\n");
                        sb.Append("if(" + cach_group_name + ".TryGetValue(cach_key,out ret))\r\n");
                        sb.Append("\treturn ret;\r\n");

                        string oldDictName = "_Group." + gk;
                        string oldKeyName = "";
                        for (int i = 0; i < g.Value.Length; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append("if (" + oldDictName + ".ContainsKey(");
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                // ! .ToString()
                                sb.Append(oldKeyName + ") ){\r\n");
                            }
                            else
                            {
                                string tempName = "tmp" + (i - 1);
                                // ! .ToString()
                                sb.Append("var " + tempName + " = " + oldDictName + "[" + oldKeyName + "];\r\n");
                                sb.Append("if (" + tempName + ".ContainsKey(");
                                oldDictName = tempName;
                                oldKeyName = g.Value[i].Substring(0, 1).ToUpper() + g.Value[i].Substring(1);
                                // ! .ToString()
                                sb.Append(oldKeyName + ") ){\r\n");
                            }
                        }

                        // ! .ToString()
                        sb.Append("var ids = " + oldDictName + "[" + oldKeyName + "];\r\n");
                        sb.Append("var configs = new " + configClassName + "[ids.Length];\r\n");
                        sb.Append("for (int i = 0; i < ids.Length; i++) {\r\n");
                        sb.Append("\tvar id = ids[i];\r\n");
                        sb.Append("\tconfigs[i] = Get(id);\r\n");
                        sb.Append("}\r\n");
                        // 缓存一下这次的结果
                        sb.Append("\t" + cach_group_name + "[cach_key]" + "=configs;\r\n");
                        sb.Append("return configs;\r\n");
                        for (int i = 0; i < g.Value.Length; i++)
                            sb.Append("}\r\n");

                        sb.Append("return new " + configClassName + "[0];\r\n");
                        sb.Append("}\r\n");
                    }

                    for (int i = 0; i < data.keys.Count; i++)
                    {
                        if (data.types[i] == "string" || data.types[i].StartsWith("[]"))
                            continue;
                        sb.Append("\tpublic float data_" + data.name + "_vlookup_" + (data.cols[i] + 1) + "(int id) {\r\n");
                        // ! id.ToString()
                        sb.Append("\treturn (float)(Get(id)." + data.keys[i].Substring(0, 1).ToUpper() + data.keys[i].Substring(1) + ");\r\n");
                        sb.Append("}\r\n");
                    }

                    sb.Append("}\r\n"); // table class结束

                    // ---------------------写这个table class的扩展便捷获取方法数组的SingleOne----------------------
                    sb.Append("public static class " + tableClassName + "ExternFunc" + " {\r\n");
                    sb.Append("\tpublic static " + configClassName + " SingleOne (this " + configClassName + "[] arr){ \r\n");
                    sb.Append("\t\tif(arr != null && arr.Length > 0) return arr[0];\r\n");
                    sb.Append("\t\treturn null;\r\n");
                    sb.Append("\t}\r\n");
                    sb.Append("}\r\n"); // table extern func class结束

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


            // 格式化代码
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
            clearfuncs.Sort();
            loadcode.Append("\tpublic static void Clear() {\r\n");
            foreach (var str in clearfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

            // save all
            savefuncs.Sort();
            loadcode.Append("\tpublic static void Save() {\r\n");
            foreach (var str in savefuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

            loadcode.Append("}\r\n");
            File.WriteAllText(codeExportDir + "load.cs", loadcode.ToString());

            // 等待所有文件完成
            while (results.Count < datas.Values.Count * 2)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));

            return string.Empty;
        }
    }
}
