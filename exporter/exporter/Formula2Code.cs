using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using XLParser.Web.XLParserVersions.v120;
using System.Linq;
using NPOI.SS.UserModel;

namespace exporter
{
    public struct CellCoord
    {
        public int row;
        public int col;
        public CellCoord(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public static string ToString(CellCoord cc)
        {
            return "{" + cc.row + "," + cc.col + "}";
        }
    }

    public static class Formula2Code
    {
        static string specialFormulaCode()
        {
            if (processingformula.StartsWith("SUMPRODUCT("))
            {
                string[] arr = processingformula.Substring(11, processingformula.Length - 12).Split('*');
                if (arr.Length != 3)
                    throw new Exception("SUMPRODUCT 只支持数据检索,限定同一张表内使用第二行和第一列匹配," + processingsheet.SheetName + "," + processingcoord);
                string tabname = string.Empty;
                string proname = string.Empty;
                CellCoord coord = new CellCoord(-1, -1);
                foreach (string str in arr)
                {
                    string[] tmp1 = str.Trim(' ', ')', '(').Split('!');
                    string tname = tmp1[0].Contains(']') ? tmp1[0].Split(']')[1] : tmp1[0];
                    if (string.IsNullOrEmpty(tabname))
                        tabname = tname;
                    else if (tabname != tname)
                        throw new Exception("SUMPRODUCT 只支持数据检索,限定【同一张表内】使用第二行和第一列匹配," + processingsheet.SheetName + "," + processingcoord);

                    string[] tmp2 = tmp1[1].Split('=');
                    // 数据矩阵忽略
                    if (tmp2.Length == 1) continue;
                    if (tmp2[1].StartsWith("\""))
                        proname = tmp2[1].Trim('\"');
                    else
                    {
                        int row, col;
                        GetCoordinate(tmp2[1], out row, out col);
                        ICell cell = processingsheet.GetRow(row - 1).GetCell(col - 1);
                        if (cell.CellType == CellType.String)
                            proname = cell.StringCellValue;
                        else
                            coord = new CellCoord(row, col);
                    }
                }
                if (coord.col < 0 || string.IsNullOrEmpty(proname))
                    throw new Exception("SUMPRODUCT 只支持数据检索,限定同一张表内使用第二行和第一列匹配," + processingsheet.SheetName + "," + processingcoord);
                return CodeTemplate.Get("func_code_SUMPRODUCTSEARCH")
                    .Replace("ARGS[0]", CodeTemplate.Get("template_cell").Replace("[ROW]", coord.row.ToString()).Replace("[COL]", coord.col.ToString()))
                    .Replace("[PARAM1]", CodeTemplate.curlang == CodeTemplate.Langue.Go ? (tabname.Substring(0, 1).ToUpper() + tabname.Substring(1)) : tabname)
                    .Replace("[PARAM2]", CodeTemplate.curlang == CodeTemplate.Langue.Go ? (proname.Substring(0, 1).ToUpper() + proname.Substring(1)) : proname);

            }
            return string.Empty;
        }

        static ISheet processingsheet = null;
        static string processingformula = string.Empty;
        static string processingcoord = string.Empty;
        static List<CellCoord> aboutCells = new List<CellCoord>();
        public static string Translate(ISheet sheet, string formula, string coord, out List<CellCoord> about)
        {
            processingsheet = sheet;
            processingcoord = coord;
            processingformula = formula;
            string script = specialFormulaCode();
            if (string.IsNullOrEmpty(script))
                script = ToCode(ExcelFormulaParser.Parse(formula));
            about = new List<CellCoord>(aboutCells);
            aboutCells.Clear();
            return script;
        }

        static string FormatScript(string format, List<string> args)
        {
            // 支持任意参数数量的需要拓展format
            if (format.Contains("@"))
            {
                string[] arr = format.Split('@');
                if (arr.Length != 3)
                    throw new Exception("script format error, " + format);

                StringBuilder sb = new StringBuilder(arr[0]);
                for (int i = 0; i < args.Count; i++)
                    if (!format.Contains("ARGS[" + i + "]"))
                        sb.Append(arr[1].Replace("ARGSX", "ARGS[" + i + "]"));
                sb.Append(arr[2]);
                format = sb.ToString();
            }

            string code = format;
            for (int i = 0; i < args.Count; i++)
                code = code.Replace("ARGS[" + i + "]", args[i]);
            return code;
        }

        const char CHAR_VALUE_A = 'A';
        const char CHAR_VALUE_Z = 'Z';
        static void GetCoordinate(string cell, out int row, out int col)
        {
            cell = cell.Replace("$", "");
            col = 0;
            row = 0;
            for (int i = 0; i < cell.Length; i++)
            {
                int v = cell[i];
                if (v >= CHAR_VALUE_A && v <= CHAR_VALUE_Z)
                    col = col * 26 + v - CHAR_VALUE_A + 1;
                else
                {
                    row = int.Parse(cell.Substring(i));
                    break;
                }
            }
        }

        static string ToCode(ParseTreeNode node)
        {
            switch (node.ChildNodes.Count)
            {
                case 0: // 叶节点  函数、操作符、引用、固定数据
                    {
                        switch (node.Term.Name)
                        {
                            case GrammarNames.TokenExcelConditionalRefFunction:
                            case GrammarNames.TokenExcelRefFunction:
                            case GrammarNames.ExcelFunction:
                                {
                                    string key = "func_code_" + node.Token.Text;
                                    key = key.Substring(0, key.Length - 1);
                                    return CodeTemplate.Get(key);
                                }

                            case GrammarNames.TokenNumber:
                                return node.Token.ValueString;

                            case GrammarNames.TokenBool:
                                return node.Token.ValueString.ToLower();

                            case GrammarNames.TokenCell:
                                int row, col;
                                GetCoordinate(node.Token.ValueString, out row, out col);
                                aboutCells.Add(new CellCoord(row, col));
                                return CodeTemplate.Get("template_cell")
                                    .Replace("[ROW]", row.ToString()).Replace("[COL]", col.ToString());

                            case GrammarNames.TokenSheet:
                                return node.Token.ValueString.Substring(0, node.Token.ValueString.Length - 1);

                            case GrammarNames.TokenVRange:
                                if (!node.Token.ValueString.StartsWith("$A"))
                                    throw new Exception("只支持搜索第一列" + "\nformula = " + processingformula + "\ncoord = " + processingcoord);
                                return "";

                            case GrammarNames.TokenFileNameNumeric: // 文件名不重要，忽略
                                return "";

                            default:
                                if (node.Term.Flags.HasFlag(TermFlags.IsOperator))
                                {
                                    return CodeTemplate.Get("operator_code_" + node.Token.ValueString);
                                }
                                else
                                    throw new Exception("unkonw flag : " + node.Term.Name + "\nformula = " + processingformula + "\ncoord = " + processingcoord);
                        }
                    }

                case 1: // 单一节点直接向下找
                    return ToCode(node.ChildNodes[0]);

                case 2:
                    {
                        switch (node.Term.Name)
                        {
                            case GrammarNames.ReferenceFunctionCall:
                            case GrammarNames.FunctionCall:
                                // 函数调用，找出函数和参数组合代码
                                string funcformat = ToCode(node.ChildNodes[0]);
                                List<string> args = new List<string>();
                                for (int i = 0; i < node.ChildNodes[1].ChildNodes.Count; i++)
                                    args.Add(ToCode(node.ChildNodes[1].ChildNodes[i]));
                                return FormatScript(funcformat, args);

                            case GrammarNames.Reference:
                                return ToCode(node.ChildNodes[0]) + ToCode(node.ChildNodes[1]);

                            case GrammarNames.Prefix:
                                return string.Join("", node.ChildNodes.Select(ToCode));

                            default:
                                throw new Exception("unknow node term " + node.Term.Name + "\nformula =" + processingformula + "\ncoord = " + processingcoord);
                        }
                    }

                case 3: // 操作符，类似函数调用
                    {
                        string funcformat = ToCode(node.ChildNodes[1]);
                        List<string> args = new List<string>();
                        args.Add(ToCode(node.ChildNodes[0]));
                        args.Add(ToCode(node.ChildNodes[2]));
                        return FormatScript(funcformat, args);
                    }

                default:
                    throw new Exception("WTF! " + processingformula + "\ncoord = " + processingcoord);
            }
        }
    }
}
