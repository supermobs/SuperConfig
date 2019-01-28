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

        static List<string> dataTypes = new List<string>() { "int", "long", "string", "double", "float", "[]int", "[]long", "[]string", "[]double", "[]float" };
        static Dictionary<string, DataStruct> datas = new Dictionary<string, DataStruct>();
        class DataStruct
        {
            public readonly string name;
            public bool isnew = true;
            public int labelindex = 0;
            public List<string> files = new List<string>();

            public DataStruct(string name)
            {
                this.name = name;
                datas.Add(name, this);
                dataLabelModifys = new Dictionary<int, Dictionary<int, object>>[Cache.labels.Count];
                for (int i = 0; i < Cache.labels.Count; i++)
                    dataLabelModifys[i] = new Dictionary<int, Dictionary<int, object>>();
            }

            public List<string> keys = new List<string>();
            public List<string> keyNames = new List<string>();
            public List<string> types = new List<string>();
            public List<int> cols = new List<int>();
            public Dictionary<string, string[]> groups = new Dictionary<string, string[]>();
            public Dictionary<string, int[]> groupindexs = new Dictionary<string, int[]>();

            public List<int> ids = new List<int>();
            public List<List<object>> dataContent = new List<List<object>>();
            public Dictionary<int, Dictionary<int, object>>[] dataLabelModifys;

            public string ApplyModify()
            {
                for (int i = 0; i < Cache.labels.Count; i++)
                {
                    var dm = dataLabelModifys[i];
                    var e = dm.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var dindex = ids.IndexOf(e.Current.Key);
                        if (dindex < 0)
                            return name + "表的" + Cache.labels[i] + "标签表，有一个主表不存在的id，id=" + e.Current.Key;
                        foreach (var p in e.Current.Value)
                            dataContent[dindex][p.Key] = p.Value;
                    }
                }
                return string.Empty;
            }
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

        static object GetCodeValue(ISheet sheet, CustomWorkbook book, ICell cell, string type, out string error)
        {
            error = string.Empty;
            try
            {
                object codevalue = null;
                if (CustomWorkbook.evaluateSheets.Contains(sheet.SheetName) && cell != null && cell.CellType == CellType.Formula)
                {
                    book.evaluator.DebugEvaluationOutputForNextEval = true;
                    CellValue cellValue = book.evaluator.Evaluate(cell);
                    switch (type)
                    {
                        case "int":
                            codevalue = cellValue.CellType == CellType.Numeric ? Convert.ToInt32(cellValue.NumberValue) :
                                cellValue.CellType != CellType.String || string.IsNullOrEmpty(cellValue.StringValue) ? 0 : int.Parse(cellValue.StringValue); break;
                        case "long":
                            codevalue = cellValue.CellType == CellType.Numeric ? Convert.ToInt64(cellValue.NumberValue) :
                                cellValue.CellType != CellType.String || string.IsNullOrEmpty(cellValue.StringValue) ? 0 : long.Parse(cellValue.StringValue); break;
                        case "string":
                            codevalue = cellValue.CellType == CellType.String ? cellValue.StringValue : cellValue.ToString(); break;
                        case "double":
                        case "float":
                        case "float64":
                            codevalue = cellValue.CellType == CellType.Numeric ? cellValue.NumberValue :
                                cellValue.CellType != CellType.String || string.IsNullOrEmpty(cellValue.StringValue) ? 0 : double.Parse(cellValue.StringValue); break;
                        default:
                            if (type.StartsWith("[]"))
                            {
                                string[] arr = (cellValue.CellType == CellType.Numeric ? cellValue.NumberValue.ToString() : (cellValue.CellType == CellType.String ? cellValue.StringValue : "")).Split('|');
                                if (arr.Length == 1 && string.IsNullOrEmpty(arr[0])) arr = new string[] { };
                                switch (type.Substring(2))
                                {
                                    case "int":
                                        int[] v = new int[arr.Length];
                                        for (int ii = 0; ii < arr.Length; ii++) v[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : int.Parse(arr[ii]);
                                        codevalue = v;
                                        break;
                                    case "long":
                                        long[] v64 = new long[arr.Length];
                                        for (int ii = 0; ii < arr.Length; ii++) v64[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : long.Parse(arr[ii]);
                                        codevalue = v64;
                                        break;
                                    case "string":
                                        codevalue = arr;
                                        break;
                                    case "double":
                                    case "float":
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

                    switch (type)
                    {
                        case "int":
                            codevalue = ct == CellType.Numeric ? Convert.ToInt32(cell.NumericCellValue) :
                                (ct == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue) ? int.Parse(cell.StringCellValue) : 0);
                            break;
                        case "long":
                            codevalue = ct == CellType.Numeric ? Convert.ToInt64(cell.NumericCellValue) :
                                (ct == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue)) ? long.Parse(cell.StringCellValue) : 0; break;
                        case "string":
                            codevalue = ct == CellType.Numeric ? cell.NumericCellValue.ToString() :
                                (ct == CellType.String ? cell.StringCellValue : "");
                            break;
                        case "double":
                        case "float":
                        case "float64":
                            codevalue = ct == CellType.Numeric ? cell.NumericCellValue :
                                (ct == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue) ? double.Parse(cell.StringCellValue) : 0);
                            break;
                        default:
                            if (type.StartsWith("[]"))
                            {
                                string[] arr = (ct == CellType.Numeric ? cell.NumericCellValue.ToString() : (ct == CellType.String ? cell.StringCellValue : "")).Split('|');
                                if (arr.Length == 1 && string.IsNullOrEmpty(arr[0])) arr = new string[] { };
                                switch (type.Substring(2))
                                {
                                    case "int":
                                        int[] v = new int[arr.Length];
                                        for (int ii = 0; ii < arr.Length; ii++) v[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : int.Parse(arr[ii]);
                                        codevalue = v;
                                        break;
                                    case "long":
                                        long[] v64 = new long[arr.Length];
                                        for (int ii = 0; ii < arr.Length; ii++) v64[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : long.Parse(arr[ii]);
                                        codevalue = v64;
                                        break;
                                    case "string":
                                        codevalue = arr;
                                        break;
                                    case "double":
                                    case "float":
                                    case "float64":
                                        double[] vv = new double[arr.Length];
                                        for (int ii = 0; ii < arr.Length; ii++) vv[ii] = string.IsNullOrEmpty(arr[ii]) ? 0 : double.Parse(arr[ii]);
                                        codevalue = vv;
                                        break;
                                }
                            }
                            break;
                    }
                }
                return codevalue;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                error = "数据格式有误， 第" + (cell.RowIndex + 1) + "行第" + (cell.ColumnIndex + 1) + "列， SheetName = " + sheet.SheetName + "，FileNames = " + book.fileName;
                return null;
            }
        }

        static string DealWithDataLabelSheet(ISheet sheet, CustomWorkbook book)
        {
            string[] larr = sheet.SheetName.Split('_');
            string tableName = sheet.SheetName.Substring(0, sheet.SheetName.LastIndexOf('_'));
            int labelindex = Cache.labels.IndexOf(larr[larr.Length - 1]);
            // 不要这个标签的内容
            if (labelindex < 0)
                return string.Empty;

            // 等待原始数据导出
            DataStruct data;
            while (!datas.TryGetValue(tableName, out data))
                Thread.Sleep(TimeSpan.FromSeconds(0.1));

            lock (data)
            {
                // 标签表头
                IRow engRow = sheet.GetRow(1);
                IRow tRow = sheet.GetRow(3);
                List<int> keyindexs = new List<int>();
                List<int> cols = new List<int>();
                List<string> types = new List<string>();
                for (int i = 0; i < engRow.LastCellNum; i++)
                {
                    if (engRow.GetCell(i) == null || engRow.GetCell(i).CellType != CellType.String || string.IsNullOrEmpty(engRow.GetCell(i).StringCellValue))
                        continue;
                    string key = engRow.GetCell(i).StringCellValue;
                    int keyindex = data.keys.IndexOf(key);
                    if (keyindex < 0)
                        continue;
                    keyindexs.Add(keyindex);
                    string type = (tRow == null || tRow.GetCell(i) == null) ? " " : tRow.GetCell(i).StringCellValue;
                    types.Add(type);
                    if (!dataTypes.Contains(type))
                        return "未知的数据类型" + type + "，SheetName = " + tableName + "，FileName = " + book.fileName;
                    cols.Add(i);
                }

                for (int i = 4; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || row.FirstCellNum > 0 || row.GetCell(0) == null || row.GetCell(0).CellType == CellType.Blank)
                        continue;

                    ICell cell = row.GetCell(cols[0]);
                    string err;
                    object codevalue = GetCodeValue(sheet, book, cell, types[0], out err);
                    if (!string.IsNullOrEmpty(err))
                        return err;
                    int id = (int)codevalue;
                    if (id == 0) continue;
                    if (data.dataLabelModifys[labelindex].ContainsKey(id))
                        return "id冲突，表名" + sheet.SheetName + ",id=" + id;
                    data.dataLabelModifys[labelindex].Add(id, new Dictionary<int, object>());

                    for (int j = 1; j < cols.Count; j++)
                    {
                        cell = row.GetCell(cols[j]);
                        if (cell != null && cell.CellType == CellType.String && cell.StringCellValue == "*")
                            continue;
                        // 修改内容
                        codevalue = GetCodeValue(sheet, book, cell, types[j], out err);
                        if (!string.IsNullOrEmpty(err))
                            return err;
                        data.dataLabelModifys[labelindex][id].Add(keyindexs[j], codevalue);
                    }
                }
            }
            return string.Empty;
        }

        static string DealWithDataSheet(ISheet sheet, CustomWorkbook book)
        {
            string tableName = sheet.SheetName;
            if (tableName.StartsWith("_"))
                return string.Empty;

            string[] larr = tableName.Split('_');
            if (tableName.Contains("_") && larr[larr.Length - 1] == larr[larr.Length - 1].ToUpper())
            {
                try
                {
                    return DealWithDataLabelSheet(sheet, book);
                }
                catch (Exception e)
                {
                    return "deal with label sheet error : " + book.fileName + " - " + tableName + "\n" + e.Message + "\n" + e.StackTrace;
                }
            }

            DataStruct data;
            lock (datas)
            {
                if (!datas.TryGetValue(tableName, out data))
                    data = new DataStruct(tableName);
            }
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
                            if (cell.CellType == CellType.String && !string.IsNullOrEmpty(cell.StringCellValue))
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
                        string err;
                        object codevalue = GetCodeValue(sheet, book, cell, data.types[j], out err);
                        if (!string.IsNullOrEmpty(err))
                            return err;
                        values.Add(codevalue);
                    }

                    int id = (int)values[0];
                    if (id == 0) continue;
                    if (id == -1)
                    {
                        string key = book.fileName + " " + tableName + " ";
                        for (int iii = 1; iii < values.Count; iii++)
                            key += values[iii].ToString();
                        do
                        {
                            key += " ";
                            id = 99000000 + Math.Abs(key.GetHashCode()) % 1000000;
                        } while (ids.Contains(id));
                        values[0] = id;
                    }
                    if (ids.Contains(id))
                        return "索引冲突 [" + values[0] + "]，SheetName = " + tableName + "，FileNames = " + string.Join(",", data.files);

                    bool useful = true;
                    var idcell = row.GetCell(data.cols[0]);

                    // 先注释掉，不然导不出那些加备注的行
                    //IComment idcom = null;
                    //while (true) { try { idcom = idcell.CellComment; break; } catch { } }
                    //if (idcom != null)
                    //useful = new List<string>(idcom.String.String.Split('\n')).Intersect(Cache.labels).Count() > 0;
                    if (useful)
                    {
                        // 添加id
                        ids.Add((int)values[0]);
                        // 添加数据
                        dataContent.Add(values);
                    }
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
                            error = sheet.SheetName + ":" + ex.ToString();
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

            // 应用标签修改
            var enumerator = datas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string err = enumerator.Current.Value.ApplyModify();
                if (!string.IsNullOrEmpty(err))
                    return err;
            }

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