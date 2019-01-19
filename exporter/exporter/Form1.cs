using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace exporter
{
    public partial class Form1 : Form
    {
        bool _isOutLua = false;
        bool _isOutGo = false;
        bool _isOutCs = false;
        bool _isOutTs = false;

        public Form1(string[] args)
        {
            Console.WriteLine("初始化路径...");
            LoadPaths();

            Console.WriteLine("初始化缓存...");
            List<string> argslist = new List<string>(args);
            int labelindex = 0;
            foreach (string arg in argslist)
                if (arg.StartsWith("label-"))
                    labelindex = int.Parse(arg.Substring(6));

            // 缓存初始化
            Cache.Init(labellist[labelindex], argslist.Contains("divfloder") ? labelNames[labelindex].Split(':')[0] : "", argslist.Contains("cache"));

            // 导出语言初始化
            _isOutLua = argslist.Contains("out_lua");
            _isOutGo = argslist.Contains("out_go");
            _isOutCs = argslist.Contains("out_cs");
            _isOutTs = argslist.Contains("out_ts");


            if (argslist.Contains("nowindow"))
            {
                Console.WriteLine("启动导表程序...");
                if (Export())
                {
                    Console.WriteLine("Complete");
                    Environment.Exit(0);
                }
            }
            else
            {
                InitializeComponent();
                cacheTog.Checked = Cache.enable;
                isOutLua.Checked = _isOutLua;
                isOutGO.Checked = _isOutGo;
                isOutCS.Checked = _isOutCs;
                isOutTS.Checked = _isOutTs;

                foreach (string ln in labelNames)
                    labelSelect.Items.Add(ln);

                labelSelect.SelectedIndex = 0;
            }
        }

        const int PATH_LEN = 8;
        List<string> paths = new List<string>(PATH_LEN);
        List<List<string>> labellist = new List<List<string>>();
        List<string> labelNames = new List<string>();
        string pathConfigFile = "pathconfig";

        void LoadPaths()
        {
            pathConfigFile = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + pathConfigFile;

            if (File.Exists(pathConfigFile))
            {
                var ps = File.ReadAllLines(pathConfigFile);
                paths.AddRange(ps);
                for (int i = paths.Count-1; i < PATH_LEN; i++)
                {
                    paths.Add("");                    
                }
            }

            string labelcfg = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + "labels";
            string[] arr;
            if (File.Exists(labelcfg))
            {
                arr = File.ReadAllLines(labelcfg);
            }
            else
            {
                arr = new string[] { "default" };
                File.WriteAllLines(labelcfg, arr);
            }

            for (int i = 0; i < arr.Length; i++)
            {
                labelNames.Add(arr[i]);
                string[] ls = arr[i].Split(':');
                if (ls.Length == 2)
                    labellist.Add(new List<string>(ls[1].Split(',')));
                else
                    labellist.Add(new List<string>());
            }
        }

        bool Export()
        {
            try
            {
                DateTime start = DateTime.Now;
                List<string> readfiles;
                CustomWorkbook.Init(paths[0], out readfiles);
                if (readfiles.Count < 10)
                    readfiles.ForEach(Console.WriteLine);
                Console.WriteLine("读入" + readfiles.Count + "张表," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");

                start = DateTime.Now;
                if (!CheckError(Exporter.ReadDataXlsx())) return false;
                Console.WriteLine("读取xlsx, " + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");

                // * lua
                if(_isOutLua)
                {
                start = DateTime.Now;
                if (!CheckError(Exporter.ReadFormulaXlsx(Exporter.DealWithFormulaSheetLua))) return false;
                Console.WriteLine("lua公式, " + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");

                start = DateTime.Now;
                if (!CheckError(Exporter.ExportLua(paths[1]))) return false;
                Console.WriteLine("导出lua文件," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");
                }

                // * go
                if(_isOutGo)
                {
                start = DateTime.Now;
                if (!CheckError(Exporter.ReadFormulaXlsx(Exporter.DealWithFormulaSheetGo))) return false;
                Console.WriteLine("go公式," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");

                start = DateTime.Now;
                if (!CheckError(Exporter.ExportGo(paths[2], paths[3]))) return false;
                Console.WriteLine("导出go文件," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");
                }

                // * c#
                if(_isOutCs)
                {
                start = DateTime.Now;
                if (!CheckError(Exporter.ReadFormulaXlsx(Exporter.DealWithFormulaSheetCS))) return false;
                Console.WriteLine("c#公式," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");

                start = DateTime.Now;
                if (!CheckError(Exporter.ExportCS(paths[4], paths[5]))) return false;
                Console.WriteLine("导出c#文件," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");
                }

                // * typescript
                if(_isOutTs)
                {
                start = DateTime.Now;
                if (!CheckError(Exporter.ReadFormulaXlsx(Exporter.DealWithFormulaSheetTS))) return false;
                Console.WriteLine("ts公式," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");

                start = DateTime.Now;
                if (!CheckError(Exporter.ExportTS(paths[6], paths[7]))) return false;
                Console.WriteLine("导出ts文件," + (DateTime.Now - start).TotalSeconds.ToString("0.00") + "秒");
                }

                Cache.SaveCache();
                Console.WriteLine("存储缓存");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return false;
        }

        List<Label> labels = new List<Label>();
        FolderBrowserDialog folderBrowserDialog;

        private void Form1_Load(object sender, EventArgs e)
        {
            labels.AddRange(new Label[] { label1, label2, label3, label4 ,label_cs,label_cscfg,label_ts,label_tscfg});

            for (int index = 0; index < paths.Count; index++)
            {
                if (Directory.Exists(paths[index]))
                    labels[index].Text = paths[index];
                else
                    paths[index] = string.Empty;
            }

            folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "select path";
            folderBrowserDialog.ShowNewFolderButton = true;
        }

        private void SelectDir(int index)
        {
            Label label = labels[index];
            if (Directory.Exists(label.Text))
                folderBrowserDialog.SelectedPath = label.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string dir = folderBrowserDialog.SelectedPath + Path.DirectorySeparatorChar;
                label.Text = paths[index] = dir;
                File.WriteAllLines(pathConfigFile, paths);
            }
        }

        bool CheckError(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                Console.WriteLine(error);
                return false;
            }
            return true;
        }

        private void onBtnOutput(object sender, EventArgs e)
        {
            for (int index = 0; index < PATH_LEN; index++)
            {
                if (string.IsNullOrEmpty(paths[index]))
                {
                    MessageBox.Show("please set the path first!");
                    return;
                }
            }

            Cache.Init(labellist[labelSelect.SelectedIndex], divfolder.Checked ? labelNames[labelSelect.SelectedIndex].Split(':')[0] : "", cacheTog.Checked);
            DateTime start = DateTime.Now;
            if (Export())
            {
                Cache.SaveCache();
                MessageBox.Show("finish" + (DateTime.Now - start).TotalSeconds);
            }
        }

        void check_change(object sender, EventArgs e) {
            var cb = (CheckBox)sender;
            if(cb.Name == "isOutLua") {
                _isOutLua = cb.Checked;
            }
            else if(cb.Name == "isOutGO") {
                _isOutGo = cb.Checked;
            }
            else if(cb.Name == "isOutCS") {
                _isOutCs = cb.Checked;
            }
            else if(cb.Name == "isOutTS") {
                _isOutTs = cb.Checked;
            }
        }

        private void label_Click(object sender, EventArgs e) { SelectDir(labels.IndexOf((Label)sender)); }
    }
}
