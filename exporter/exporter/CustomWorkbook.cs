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
        ExportData,
        ExportFormal,
        Referenced
    }

    public class CustomWorkbook
    {
        public static List<CustomWorkbook> allBooks = new List<CustomWorkbook>();
        static Dictionary<string, IFormulaEvaluator> evaluatorEnv = new Dictionary<string, IFormulaEvaluator>();

        static List<string> evaluateBooks = new List<string>();

        public string fileName { get; private set; }
        public IFormulaEvaluator evaluator { get; private set; }
        public IWorkbook workbook { get; private set; }
        public bool evaluate { get; private set; }
        public CustomWorkbookType type { get; private set; }

        CustomWorkbook(FileInfo file)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                fileName = file.Name;
                evaluate = evaluateBooks.Contains(fileName);

                FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // 表的格式
                if (file.Extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
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
                    //MemoryStream ms = new MemoryStream();
                    //workbook.Write(ms);
                    //File.WriteAllBytes(file.FullName + ".xlsx", ms.ToArray());
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
            evaluateBooks = new List<string>();
            string evaConfPath = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + "evaconfig";
            if (File.Exists(evaConfPath))
                evaluateBooks.AddRange(File.ReadAllLines(evaConfPath));

            // 遍历导出目录
            foreach (var file in new DirectoryInfo(excelpath).GetFiles())
            {
                if (file.Name.StartsWith("~$") || (file.Extension != ".xlsx" && file.Extension != ".xls"))
                    continue;

                CustomWorkbook book = new CustomWorkbook(file);
                book.type = file.Name.StartsWith("公式.") ? CustomWorkbookType.ExportFormal : CustomWorkbookType.ExportData;
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
        }
    }
}
