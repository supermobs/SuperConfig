using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace exporter
{
    public partial class Form1 : Form
    {
        public Form1(string[] args)
        {
            LoadPaths();

            if (args.Length > 0)
            {
                if (Export())
                    Console.WriteLine("Complete");
                Environment.Exit(0);
            }
            else
                InitializeComponent();
        }

        string[] paths = new string[4];
        string pathConfigFile = "pathconfig";
        void LoadPaths()
        {
            pathConfigFile = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + pathConfigFile;
            if (File.Exists(pathConfigFile))
                paths = File.ReadAllLines(pathConfigFile);
        }

        bool Export()
        {
            return
                    // 读取xlsx
                    CheckError(Exporter.ReadDataXlsx(paths[0]))
                    // 读 lua 公式
                    && CheckError(Exporter.ReadFormulaXlsx(paths[0], Exporter.DealWithFormulaSheetLua))
                    // 导出lua文件
                    && CheckError(Exporter.ExportLua(paths[1]))
                    // 读 go 公式
                    && CheckError(Exporter.ReadFormulaXlsx(paths[0], Exporter.DealWithFormulaSheetGo))
                    // 导出go文件
                    && CheckError(Exporter.ExportGo(paths[2], paths[3]));
        }

        List<Label> labels = new List<Label>();
        FolderBrowserDialog folderBrowserDialog;

        private void Form1_Load(object sender, EventArgs e)
        {
            labels.AddRange(new Label[] { label1, label2, label3, label4 });

            for (int index = 0; index < paths.Length; index++)
            {
                if (Directory.Exists(paths[index]))
                    labels[index].Text = paths[index];
                else
                    paths[index] = string.Empty;
            }

            folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择路径";
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
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < paths.Length; index++)
            {
                if (string.IsNullOrEmpty(paths[index]))
                {
                    MessageBox.Show("请设置好路径先!");
                    return;
                }
            }

            if (Export())
                MessageBox.Show("导出完成");
        }

        private void label_Click(object sender, EventArgs e) { SelectDir(labels.IndexOf((Label)sender)); }
    }
}
