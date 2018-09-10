using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace exporter
{
    public enum CustomWorkbookType
    {
        Export,
        Referenced
    }

    public class CustomWorkbook
    {
        public static List<CustomWorkbook> allBooks = new List<CustomWorkbook>();
        static Dictionary<string, IFormulaEvaluator> evaluatorEnv = new Dictionary<string, IFormulaEvaluator>();

        public static List<string> evaluateSheets = new List<string>();

        public string fileName { get; private set; }
        public IFormulaEvaluator evaluator { get; private set; }
        public IWorkbook workbook { get; private set; }
        public CustomWorkbookType type { get; private set; }

        CustomWorkbook(FileInfo file)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                fileName = file.Name;

                FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);

                // 表的格式
                if (file.Extension == ".xlsx")
                {
                    var book = new XSSFWorkbook(fs);
                    foreach (var link in book.ExternalLinksTable)
                    {
                        string[] arr = link.LinkedFileName.Split('/');
                        if (arr.Length > 1)
                            link.LinkedFileName = arr[arr.Length - 1];
                    }
                    workbook = book;
                    evaluator = new XSSFFormulaEvaluator(workbook);
                }
                else if (file.Extension == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                    evaluator = new HSSFFormulaEvaluator(workbook);
                }
                else
                {
                    // csv
                    workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet(file.Name.Substring(0, file.Name.Length - 4));
                    string[] lines = File.ReadAllLines(file.FullName);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        IRow row = sheet.CreateRow(i);
                        string[] values = lines[i].Split(',');
                        row.CreateCell(0).SetCellValue(int.Parse(values[0]));
                        for (int j = 1; j < values.Length; j++)
                            row.CreateCell(j).SetCellValue(values[j]);
                    }
                    evaluator = new XSSFFormulaEvaluator(workbook);
                }
                fs.Close();

                lock (allBooks)
                {
                    allBooks.Add(this);
                    evaluatorEnv.Add(file.Name, evaluator);
                }
            });
        }

        public static void Init(string excelpath)
        {
            evaluatorEnv = new Dictionary<string, IFormulaEvaluator>();
            allBooks = new List<CustomWorkbook>();
            int totalCount = 0;

            // 启用公式重算的接口
            evaluateSheets = new List<string>();
            string evaConfPath = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + "evaconfig";
            if (File.Exists(evaConfPath))
                evaluateSheets = new List<string>(File.ReadAllLines(evaConfPath));

            // 遍历导出目录
            List<string> readfiles = new List<string>();
            foreach (var file in new DirectoryInfo(excelpath).GetFiles())
            {
                if (file.Name.StartsWith("~$") || (file.Extension != ".xlsx" && file.Extension != ".xls"))
                    continue;
                foreach (var fname in Cache.GetNoCacheAbout(file))
                {
                    if (!readfiles.Contains(fname))
                        readfiles.Add(fname);
                }
            }
            foreach (var fname in readfiles)
            {
                var file = new FileInfo(excelpath + "/" + fname);
                CustomWorkbook book = new CustomWorkbook(file);
                book.type = CustomWorkbookType.Export;
                totalCount++;
            }

            // 引用表
            string refConfPath = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + "refconfig";
            if (File.Exists(refConfPath))
            {
                foreach (string reffile in File.ReadAllLines(refConfPath))
                {
                    var file = new FileInfo(reffile);
                    CustomWorkbook book = new CustomWorkbook(file);
                    book.type = CustomWorkbookType.Referenced;
                    totalCount++;
                }
            }

            // 等待完成
            while (allBooks.Count < totalCount)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));

            // 设置公式环境
            foreach (var book in allBooks)
                book.evaluator.SetupReferencedWorkbooks(evaluatorEnv);


            Cache.PrepareToExport();
        }
    }
}
