namespace exporter
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cacheTog = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.labelSelect = new System.Windows.Forms.ComboBox();
            this.divfolder = new System.Windows.Forms.CheckBox();

            this.groupBox_cs = new System.Windows.Forms.GroupBox();
			this.label_cs = new System.Windows.Forms.Label();
			this.groupBox_cscfg = new System.Windows.Forms.GroupBox();
			this.label_cscfg = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox_cs.SuspendLayout();
            this.groupBox_cscfg.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(266, 476);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Just Do It!"; // 导出
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "select folder"; // 请选择excel目录
			this.label1.Click += new System.EventHandler(this.label_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 52);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "excel"; // 配置表目录
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(14, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 52);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "lua output"; // 客户端导出目录
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "select folder"; // 请选择excel目录
            this.label2.Click += new System.EventHandler(this.label_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(14, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 52);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "go script output"; // 服务端代码导出目录
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(315, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "select folder"; // 请选择excel目录
            this.label3.Click += new System.EventHandler(this.label_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(14, 186);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 52);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "go config output"; // 服务端配置导出目录
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(315, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "select folder"; // 请选择excel目录
            this.label4.Click += new System.EventHandler(this.label_Click);

            // 
			// groupBox_cs
			// 
			this.groupBox_cs.Controls.Add(this.label_cs);
			this.groupBox_cs.Location = new System.Drawing.Point(14, 244);
			this.groupBox_cs.Name = "groupBox_cs";
			this.groupBox_cs.Size = new System.Drawing.Size(327, 52);
			this.groupBox_cs.TabIndex = 14;
			this.groupBox_cs.TabStop = false;
			this.groupBox_cs.Text = "c# script output"; // c# code
			// 
            // label_cs
            // 
			this.label_cs.Location = new System.Drawing.Point(6, 17);
			this.label_cs.Name = "label_cs";
			this.label_cs.Size = new System.Drawing.Size(315, 32);
			this.label_cs.TabIndex = 0;
			this.label_cs.Text = "select folder"; // 请选择excel目录
			this.label_cs.Click += new System.EventHandler(this.label_Click);

			// 
			// groupBox_cscfg
			// 
			this.groupBox_cscfg.Controls.Add(this.label_cscfg);
            this.groupBox_cscfg.Location = new System.Drawing.Point(14, 302);
            this.groupBox_cscfg.Name = "groupBox_cscfg";
            this.groupBox_cscfg.Size = new System.Drawing.Size(327, 52);
            this.groupBox_cscfg.TabIndex = 15;
            this.groupBox_cscfg.TabStop = false;
            this.groupBox_cscfg.Text = "c# config output"; // c# config
            // 
			// label_cscfg
			// 
			this.label_cscfg.Location = new System.Drawing.Point(6, 17);
			this.label_cscfg.Name = "label_cscfg";
			this.label_cscfg.Size = new System.Drawing.Size(315, 32);
			this.label_cscfg.TabIndex = 0;
			this.label_cscfg.Text = "select folder"; // 请选择excel目录
			this.label_cscfg.Click += new System.EventHandler(this.label_Click);
			
            // 
            // cacheTog
            // 
            this.cacheTog.AutoSize = true;
            this.cacheTog.Location = new System.Drawing.Point(188, 360);
            this.cacheTog.Name = "cacheTog";
            this.cacheTog.Size = new System.Drawing.Size(72, 16);
            this.cacheTog.TabIndex = 16;
            this.cacheTog.Text = "use cached";
            this.cacheTog.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.labelSelect);
            this.groupBox5.Location = new System.Drawing.Point(14, 418);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 52);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "output label"; //导表标签
            // 
            // labelSelect
            // 
            this.labelSelect.FormattingEnabled = true;
            this.labelSelect.Location = new System.Drawing.Point(8, 21);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(313, 20);
            this.labelSelect.TabIndex = 0;
            // 
            // divfolder
            // 
            this.divfolder.AutoSize = true;
            this.divfolder.Location = new System.Drawing.Point(74, 360);
            this.divfolder.Name = "divfolder";
            this.divfolder.Size = new System.Drawing.Size(108, 16);
            this.divfolder.TabIndex = 18;
            this.divfolder.Text = "output divfolder"; //导出数据分目录
            this.divfolder.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 550);
            this.Controls.Add(this.divfolder);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.cacheTog);
            this.Controls.Add(this.groupBox_cscfg);
			this.Controls.Add(this.groupBox_cs);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.Text = "excel export tool"; // excel导出工具
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox_cs.ResumeLayout(false);
            this.groupBox_cscfg.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox_cs;
        private System.Windows.Forms.Label label_cs;
        private System.Windows.Forms.GroupBox groupBox_cscfg;
        private System.Windows.Forms.Label label_cscfg;
        private System.Windows.Forms.CheckBox cacheTog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox labelSelect;
        private System.Windows.Forms.CheckBox divfolder;
    }
}

