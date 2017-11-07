using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace exporter
{
    class FormulaEnumerator
    {
        public int start;
        public int end;
        public int div;
        public int key;
        public string name;
        public string fullName;
        public List<string> propertys;
        public List<string> notes;

        public static List<FormulaEnumerator> GetList(ISheet sheet)
        {
            Dictionary<string, FormulaEnumerator> dict = new Dictionary<string, FormulaEnumerator>();
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                try
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    ICell cell = row.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (cell == null || cell.CellType != CellType.String) continue;
                    string[] arr = cell.StringCellValue.Split('_');

                    if (arr[0] == "LS" && arr.Length == 4)
                    {
                        var item = new FormulaEnumerator();
                        if (int.TryParse(arr[1], out item.div) && int.TryParse(arr[2], out item.key))
                        {
                            dict.Add(arr[3], item);
                            item.start = i + 1;
                            item.name = arr[3].Substring(0, 1).ToUpper() + arr[3].Substring(1);
                            item.fullName = sheet.SheetName.Substring(3).ToLower() + "_" + arr[3];

                            item.propertys = new List<string>();
                            item.notes = new List<string>();
                            for (int j = i + 1; j < i + 1 + item.div; j++)
                            {
                                string str = sheet.GetRow(j).GetCell(4).StringCellValue;
                                if (str.StartsWith("_")) str = str.Substring(1);
                                item.propertys.Add(str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 2));
                                str = sheet.GetRow(j).GetCell(3).StringCellValue;
                                if (str.EndsWith("1")) str = str.Substring(0, str.Length - 1);
                                item.notes.Add(str);
                            }
                        }
                    }

                    if (arr[0] == "LE" && arr.Length == 2 && dict.ContainsKey(arr[1]))
                    {
                        dict[arr[1]].end = i;
                    }
                }
                catch
                {
                    throw new Exception("公式表获取枚举时出错，" + sheet.SheetName + " 第" + i + "行");
                }
            }

            return new List<FormulaEnumerator>(dict.Values);
        }
    }


    public static partial class Exporter
    {
        static Dictionary<string, string> formulaContents = new Dictionary<string, string>();

        static List<string> dataTypes = new List<string>() { "int", "string", "double", "[]int", "[]string", "[]double" };
        static Dictionary<string, DataStruct> datas = new Dictionary<string, DataStruct>();
        class DataStruct
        {
            public readonly string name;
            public bool isnew = true;
            public List<string> files = new List<string>();
            public int autoid = 99000000;

            public DataStruct(string name)
            {
                this.name = name;
                datas.Add(name, this);
            }

            public List<string> keys = new List<string>();
            public List<string> keyNames = new List<string>();
            public List<string> types = new List<string>();
            public List<int> cols = new List<int>();
            public Dictionary<string, string[]> groups = new Dictionary<string, string[]>();
            public Dictionary<string, int[]> groupindexs = new Dictionary<string, int[]>();

            public List<int> ids = new List<int>();
            public List<List<object>> dataContent = new List<List<object>>();
        }

        static void Compare(List<string> a, List<string> b, string msg)
        {
            if (a.Count != b.Count) throw new Exception(msg);
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i])
                    throw new Exception(msg);
        }
        static void Compare(List<int> a, List<int> b, string msg)
        {
            if (a.Count != b.Count) throw new Exception(msg);
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i])
                    throw new Exception(msg);
        }

        static string DealWithDataSheet(ISheet sheet, CustomWorkbook book)
        {
            string tableName = sheet.SheetName;
            if (tableName.StartsWith("_"))
                return string.Empty;

            DataStruct data;
            if (!datas.TryGetValue(tableName, out data))
                lock (datas)
                    data = new DataStruct(tableName);
            lock (data)
            {
                data.files.Add(book.fileName);

                //5、sheet第二行，字段英文名，不填写留空的列将被过滤掉，不予导出，第一列不可留空
                //6、sheet第三行，字段中文名
                //7、sheet第四行，字段类型，int整数、string字符串、double浮点数
                try
                {
                    IRow engRow = sheet.GetRow(1);
                    IRow cnRow = sheet.GetRow(2);
                    IRow tRow = sheet.GetRow(3);

                    if (engRow == null || engRow.FirstCellNum != 0)
                        return "第一个字段不可以留空，SheetName = " + tableName + "，FileName = " + book.fileName;

                    List<string> keys = new List<string>();
                    List<string> keyNames = new List<string>();
                    List<string> types = new List<string>();
                    List<int> cols = new List<int>();
                    for (int i = 0; i < engRow.LastCellNum; i++)
                    {
                        if (engRow.GetCell(i) == null || engRow.GetCell(i).CellType != CellType.String || string.IsNullOrEmpty(engRow.GetCell(i).StringCellValue))
                            continue;
                        cols.Add(i);
                        string key = engRow.GetCell(i).StringCellValue;
                        if (keys.Contains(key)) return "字段名重复 " + key + "，SheetName = " + tableName + "，FileName = " + book.fileName;
                        keys.Add(key);
                        keyNames.Add((cnRow == null || cnRow.GetCell(i) == null) ? "" : cnRow.GetCell(i).StringCellValue.Replace("\n", " "));
                        string type = (tRow == null || tRow.GetCell(i) == null) ? " " : tRow.GetCell(i).StringCellValue;
                        types.Add(type);
                        if (!dataTypes.Contains(type))
                            return "未知的数据类型" + type + "，SheetName = " + tableName + "，FileName = " + book.fileName;
                    }

                    if (data.isnew)
                    {
                        if (types[0] != "int")
                            return "表头错误，索引必须为int类型，SheetName = " + tableName + "，FileName = " + book.fileName;

                        data.keys = keys;
                        data.keyNames = keyNames;
                        data.types = types;
                        data.cols = cols;
                    }
                    else
                    {
                        string error = "表头不一致，SheetName = " + tableName + "，FileNames = " + string.Join(",", data.files);
                        Compare(keys, data.keys, error);
                        Compare(keyNames, data.keyNames, error);
                        Compare(types, data.types, error);
                        Compare(cols, data.cols, error);
                    }
                }
                catch (Exception ex)
                {
                    return "表头错误，SheetName = " + tableName + "，FileName = " + book.fileName + "\n" + ex.ToString() + "\n" + ex.StackTrace;
                }

                // 读取表头
                //4、sheet第一行，填写字段名数据分组，可以进行多字段联合分组("|"分隔)，有分组逻辑的数据必须进行分组
                {
                    List<string> groups = new List<string>();
                    IRow row = sheet.GetRow(0);
                    if (row != null)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            ICell cell = row.Cells[i];
                            if (!string.IsNullOrEmpty(cell.StringCellValue))
                                groups.Add(cell.StringCellValue);
                        }
                    }

                    if (data.isnew)
                    {
                        foreach (string g in groups)
                        {
                            List<int> indexs = new List<int>();
                            string[] arr = g.Split('|');
                            foreach (string dt in arr)
                            {
                                int index = data.keys.IndexOf(dt);
                                if (index == -1)
                                    return "找不到数据分组要的字段[" + dt + "]，SheetName = " + tableName + "，FileName = " + book.fileName;
                                indexs.Add(index);
                            }
                            data.groups.Add(g, arr);
                            data.groupindexs.Add(g, indexs.ToArray());
                        }
                    }
                    else
                    {
                        if (groups.Count != data.groups.Count)
                            return "数据分组声明不一致，SheetName = " + tableName + "，FileNames = " + string.Join(",", data.files);
                        foreach (string g in groups)
                            if (!data.groups.ContainsKey(g))
                                return "数据分组声明不一致，SheetName = " + tableName + "，FileNames = " + string.Join(",", data.files);
                    }
                }


                // 8、sheet第五行开始是表的数据，首字段不填写视为无效数据
                List<int> ids = data.ids;
                List<List<object>> dataContent = data.dataContent;
                for (int i = 4; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || row.FirstCellNum > 0 || row.GetCell(0) == null || row.GetCell(0).CellType == CellType.Blank)
                        continue;

                    List<object> values = new List<object>();
                    for (int j = 0; j < data.cols.Count; j++)
                    {
                        ICell cell = row.GetCell(data.cols[j]);
                        try
                        {
                            object codevalue = null;
                            if (CustomWorkbook.evaluateSheets.Contains(sheet.SheetName) && cell != null && cell.CellType == CellType.Formula)
                            {
                                book.evaluator.DebugEvaluationOutputForNextEval = true;
                                CellValue cellValue = book.evaluator.Evaluate(cell);
                                switch (data.types[j])
                                {
                                    case "int":
                                        codevalue = cellValue.CellType == CellType.Numeric ? Convert.ToInt32(cellValue.NumberValue) :
                                            cellValue.CellType != CellType.String || string.IsNullOrEmpty(cellValue.StringValue) ? 0 : int.Parse(cellValue.StringValue); break;
                                    case "string":
                                        codevalue = cellValue.CellType == CellType.String ? cellValue.StringValue : cellValue.ToString(); break;
                                    case "double":
                                        codevalue = cellValue.CellType == CellType.Numeric ? cellValue.NumberValue :
                                            cellValue.CellType != CellType.String || string.IsNullOrEmpty(cellValue.StringValue) ? 0 : double.Parse(cellValue.StringValue); break;
                                    default:
                                        if (data.types[j].StartsWith("[]"))
                                        {
                                            string[] arr = (cellValue.CellType == CellType.Numeric ? cellValue.NumberValue.ToString() : (cellValue.CellType == CellType.String ? cellValue.StringValue : "")).Split('|');
                                            if (arr.Length == 1 && string.IsNullOrEmpty(arr[0])) arr = new string[] { };
                                            switch (data.types[j].Substring(2))
                                            {
                                                case "int":
                                                    int[] v = new int[arr.Length];
                                                    for (int ii = 0; ii < arr.Length; ii++) v[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : int.Parse(arr[ii]);
                                                    codevalue = v;
                                                    break;
                                                case "string":
                                                    codevalue = arr;
                                                    break;
                                                case "double":
                                                    double[] vv = new double[arr.Length];
                                                    for (int ii = 0; ii < arr.Length; ii++) vv[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : double.Parse(arr[ii]);
                                                    codevalue = vv;
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                CellType ct = CellType.Blank;
                                if (cell != null)
                                {
                                    if (cell.CellType == CellType.Formula)
                                        ct = cell.CachedFormulaResultType;
                                    else
                                        ct = cell.CellType;
                                }

                                switch (data.types[j])
                                {
                                    case "int":
                                        codevalue = ct == CellType.Numeric ? Convert.ToInt32(cell.NumericCellValue) :
                                            (ct == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue) ? int.Parse(cell.StringCellValue) : 0);
                                        break;
                                    case "string":
                                        codevalue = ct == CellType.Numeric ? cell.NumericCellValue.ToString() :
                                            (ct == CellType.String ? cell.StringCellValue : "");
                                        break;
                                    case "double":
                                        codevalue = ct == CellType.Numeric ? cell.NumericCellValue :
                                            (ct == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue) ? double.Parse(cell.StringCellValue) : 0);
                                        break;
                                    default:
                                        if (data.types[j].StartsWith("[]"))
                                        {
                                            string[] arr = (ct == CellType.Numeric ? cell.NumericCellValue.ToString() : (ct == CellType.String ? cell.StringCellValue : "")).Split('|');
                                            if (arr.Length == 1 && string.IsNullOrEmpty(arr[0])) arr = new string[] { };
                                            switch (data.types[j].Substring(2))
                                            {
                                                case "int":
                                                    int[] v = new int[arr.Length];
                                                    for (int ii = 0; ii < arr.Length; ii++) v[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : int.Parse(arr[ii]);
                                                    codevalue = v;
                                                    break;
                                                case "string":
                                                    codevalue = arr;
                                                    break;
                                                case "double":
                                                    double[] vv = new double[arr.Length];
                                                    for (int ii = 0; ii < arr.Length; ii++) vv[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : double.Parse(arr[ii]);
                                                    codevalue = vv;
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                            values.Add(codevalue);
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex);
                            return "数据格式有误， 第" + (cell.RowIndex + 1) + "行第" + (cell.ColumnIndex + 1) + "列， SheetName = " + tableName + "，FileNames = " + book.fileName;
                        }
                    }

                    int id = (int)values[0];
                    //if (id == 0) // id=0忽略，方便公式生成id
                    //    continue;
                    if (id == -1)
                    {
                        id = ++data.autoid;
                        values[0] = id;
                    }
                    if (ids.Contains(id))
                        return "索引冲突 [" + values[0] + "]，SheetName = " + tableName + "，FileNames = " + string.Join(",", data.files);
                    // 添加id
                    ids.Add((int)values[0]);
                    // 添加数据
                    dataContent.Add(values);
                }

                data.isnew = false;
                return string.Empty;
            }
        }

        public static string ReadDataXlsx()
        {
            datas = new Dictionary<string, DataStruct>();

            List<string> results = new List<string>();
            int totalCount = 0;

            foreach (var book in CustomWorkbook.allBooks)
            {
                if (book.type != CustomWorkbookType.Export)
                    continue;

                for (int i = 0; i < book.workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = book.workbook.GetSheetAt(i);
                    if (sheet.SheetName.StartsWith("(F)") || sheet.SheetName.StartsWith("_"))
                        continue;
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        string error;
                        try
                        {
                            error = DealWithDataSheet(sheet, book);
                        }
                        catch (Exception ex)
                        {
                            error = ex.ToString();
                        }
                        lock (results)
                            results.Add(error);
                    });
                    totalCount++;
                }
            }

            while (results.Count < totalCount)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));

            foreach (string error in results)
                if (!string.IsNullOrEmpty(error))
                    return error;

            return string.Empty;
        }

        public static string ReadFormulaXlsx(Func<ISheet, string> deal)
        {
            formulaContents = new Dictionary<string, string>();

            foreach (var book in CustomWorkbook.allBooks)
            {
                if (book.type != CustomWorkbookType.Export)
                    continue;

                for (int i = 0; i < book.workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = book.workbook.GetSheetAt(i);
                    if (!sheet.SheetName.StartsWith("(F)"))
                        continue;
                    string error = deal(sheet);
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }
            }

            return string.Empty;
        }
    }
}