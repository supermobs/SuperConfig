using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using XLParser.Web.XLParserVersions.v120;
using System.Linq;

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
        static string processingformula = string.Empty;
        static string processingcoord = string.Empty;
        static List<CellCoord> aboutCells = new List<CellCoord>();
        public static string Translate(string formula, string coord, out List<CellCoord> about)
        {
            processingcoord = coord;
            processingformula = formula;
            string script = Tolua(ExcelFormulaParser.Parse(formula));
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

        static string Tolua(ParseTreeNode node)
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
                    return Tolua(node.ChildNodes[0]);

                case 2:
                    {
                        switch (node.Term.Name)
                        {
                            case GrammarNames.ReferenceFunctionCall:
                            case GrammarNames.FunctionCall:
                                // 函数调用，找出函数和参数组合代码
                                string funcformat = Tolua(node.ChildNodes[0]);
                                List<string> args = new List<string>();
                                for (int i = 0; i < node.ChildNodes[1].ChildNodes.Count; i++)
                                    args.Add(Tolua(node.ChildNodes[1].ChildNodes[i]));
                                return FormatScript(funcformat, args);

                            case GrammarNames.Reference:
                                return Tolua(node.ChildNodes[0]) + Tolua(node.ChildNodes[1]);

                            case GrammarNames.Prefix:
                                return string.Join("", node.ChildNodes.Select(Tolua));

                            default:
                                throw new Exception("unknow node term " + node.Term.Name + "\nformula =" + processingformula + "\ncoord = " + processingcoord);
                        }
                    }

                case 3: // 操作符，类似函数调用
                    {
                        string funcformat = Tolua(node.ChildNodes[1]);
                        List<string> args = new List<string>();
                        args.Add(Tolua(node.ChildNodes[0]));
                        args.Add(Tolua(node.ChildNodes[2]));
                        return FormatScript(funcformat, args);
                    }

                default:
                    throw new Exception("WTF! " + processingformula + "\ncoord = " + processingcoord);
            }
        }
    }
}
