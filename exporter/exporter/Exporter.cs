using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace exporter
{
    public static partial class Exporter
    {
        static Dictionary<string, string> formulaContents = new Dictionary<string, string>();

        static List<string> dataTypes = new List<string>() { "int", "string", "double" };
        static Dictionary<string, DataStruct> datas = new Dictionary<string, DataStruct>();
        class DataStruct
        {
            public readonly string name;
            public bool isnew = true;
            public List<string> files = new List<string>();

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

        static string DealWithDataSheet(ISheet sheet, string fileName)
        {
            string tableName = sheet.SheetName;
            if (tableName.StartsWith("_"))
                return string.Empty;

            DataStruct data;
            if (!datas.TryGetValue(tableName, out data))
                data = new DataStruct(tableName);
            data.files.Add(fileName);

            //5、sheet第二行，字段英文名，不填写留空的列将被过滤掉，不予导出，第一列不可留空
            //6、sheet第三行，字段中文名
            //7、sheet第四行，字段类型，int整数、string字符串、double浮点数
            {
                IRow engRow = sheet.GetRow(1);
                IRow cnRow = sheet.GetRow(2);
                IRow tRow = sheet.GetRow(3);

                if (engRow == null || engRow.FirstCellNum != 0)
                    return "第一个字段不可以留空，SheetName = " + tableName + "，FileName = " + fileName;

                List<string> keys = new List<string>();
                List<string> keyNames = new List<string>();
                List<string> types = new List<string>();
                List<int> cols = new List<int>();
                for (int i = 0; i < engRow.LastCellNum; i++)
                {
                    if (engRow.GetCell(i) == null || engRow.GetCell(i).CellType != CellType.String || string.IsNullOrEmpty(engRow.GetCell(i).StringCellValue))
                        continue;
                    cols.Add(i);
                    keys.Add(engRow.GetCell(i).StringCellValue);
                    keyNames.Add((cnRow == null || cnRow.GetCell(i) == null) ? "" : cnRow.GetCell(i).StringCellValue.Replace("\n", " "));
                    types.Add(tRow.GetCell(i).StringCellValue);
                    if (!dataTypes.Contains(tRow.GetCell(i).StringCellValue))
                        return "未知的数据类型" + tRow.GetCell(i).StringCellValue + "，SheetName = " + tableName + "，FileName = " + fileName;
                }

                if (data.isnew)
                {
                    if (types[0] != "int")
                        return "表头错误，索引必须为int类型，SheetName = " + tableName + "，FileName = " + fileName;

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
                                return "找不到数据分组要的字段[" + dt + "]，SheetName = " + tableName + "，FileName = " + fileName;
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
                if (row == null || row.FirstCellNum > 0)
                    continue;

                List<object> values = new List<object>();
                for (int j = 0; j < data.cols.Count; j++)
                {
                    ICell cell = row.GetCell(data.cols[j]);
                    object codevalue = null;
                    switch (data.types[j])
                    {
                        case "int":
                            codevalue = cell == null ? 0 : Convert.ToInt32(cell.NumericCellValue); break;
                        case "string":
                            codevalue = cell == null ? "" : cell.ToString(); break;
                        case "double":
                            codevalue = cell == null ? 0 : cell.NumericCellValue; break;
                    }
                    values.Add(codevalue);
                }

                if (ids.Contains((int)values[0]))
                    return "索引冲突 [" + values[0] + "]，SheetName = " + tableName + "，FileNames = " + string.Join(",", data.files);
                // 添加id
                ids.Add((int)values[0]);
                // 添加数据
                dataContent.Add(values);
            }

            data.isnew = false;
            return string.Empty;
        }

        public static string ReadDataXlsx(string excelpath)
        {
            datas = new Dictionary<string, DataStruct>();

            foreach (var file in new DirectoryInfo(excelpath).GetFiles())
            {
                if (file.Name.StartsWith("公式.") || file.Name.StartsWith("~$"))
                    continue;
                
                FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IWorkbook workbook = file.Extension == ".xlsx" ? new XSSFWorkbook(fs) as IWorkbook : new HSSFWorkbook(fs) as IWorkbook;
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    string error = DealWithDataSheet(workbook.GetSheetAt(i), file.Name);
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }
                fs.Close();
            }
            return string.Empty;
        }

        public static string ReadFormulaXlsx(string excelpath, Func<ISheet, string> deal)
        {
            formulaContents = new Dictionary<string, string>();

            foreach (var file in new DirectoryInfo(excelpath).GetFiles())
            {
                if (!file.Name.StartsWith("公式."))
                    continue;

                FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IWorkbook workbook = file.Extension == ".xlsx" ? new XSSFWorkbook(fs) as IWorkbook : new HSSFWorkbook(fs) as IWorkbook;
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    string error = deal(workbook.GetSheetAt(i));
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }
                fs.Close();
            }
            return string.Empty;
        }
    }
}