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
            this.isOutLua = new System.Windows.Forms.CheckBox();
            this.isOutGO = new System.Windows.Forms.CheckBox();
            this.isOutCS = new System.Windows.Forms.CheckBox();
            this.isOutTS = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.labelSelect = new System.Windows.Forms.ComboBox();
            this.divfolder = new System.Windows.Forms.CheckBox();

            this.groupBox_cs = new System.Windows.Forms.GroupBox();
			this.label_cs = new System.Windows.Forms.Label();
			this.groupBox_cscfg = new System.Windows.Forms.GroupBox();
			this.label_cscfg = new System.Windows.Forms.Label();
            this.groupBox_ts = new System.Windows.Forms.GroupBox();
			this.label_ts = new System.Windows.Forms.Label();
            this.groupBox_tscfg = new System.Windows.Forms.GroupBox();
			this.label_tscfg = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox_cs.SuspendLayout();
            this.groupBox_cscfg.SuspendLayout();
            this.groupBox_ts.SuspendLayout();
            this.groupBox_tscfg.SuspendLayout();
            this.SuspendLayout();

            int Current_Height = 12;
            int Height_Group_Folder = 68; // 目录选择的高度差
            int Height_Group_CheckBox = 22; // checkbox分组高度
            int Table_Index = 1;

            
            //
            // label1 excel
            //
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "select folder"; // 请选择excel目录
			this.label1.Click += new System.EventHandler(this.label_Click);
            //
            // groupBox1 excel
            //
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 52);
            this.groupBox1.TabIndex = Table_Index;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "excel"; // 配置表目录
            //
            // groupBox2 lua导出
            //
            Current_Height += Height_Group_Folder; // 高度增加
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 52);
            this.groupBox2.TabIndex = ++Table_Index;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "lua output"; // 客户端导出目录
            //
            // label2 lua导出
            //
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "select folder"; // 请选择excel目录
            this.label2.Click += new System.EventHandler(this.label_Click);
            //
            // groupBox3 go代码
            //
            Current_Height += Height_Group_Folder; // 高度增加
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 52);
            this.groupBox3.TabIndex = ++Table_Index;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "go script output"; // 服务端代码导出目录
            //
            // label3 go代码
            //
            this.label3.Location = new System.Drawing.Point(6, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(315, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "select folder"; // 请选择excel目录
            this.label3.Click += new System.EventHandler(this.label_Click);
            //
            // groupBox4 go配置表
            //
            Current_Height += Height_Group_Folder; // 高度增加
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 52);
            this.groupBox4.TabIndex = ++Table_Index;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "go config output"; // 服务端配置导出目录
            //
            // label4 go配置表
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
            Current_Height += Height_Group_Folder; // 高度增加
			this.groupBox_cs.Controls.Add(this.label_cs);
			this.groupBox_cs.Location = new System.Drawing.Point(14, Current_Height);
			this.groupBox_cs.Name = "groupBox_cs";
			this.groupBox_cs.Size = new System.Drawing.Size(327, 52);
			this.groupBox_cs.TabIndex = ++Table_Index;
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
            Current_Height += Height_Group_Folder; // 高度增加
			this.groupBox_cscfg.Controls.Add(this.label_cscfg);
            this.groupBox_cscfg.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox_cscfg.Name = "groupBox_cscfg";
            this.groupBox_cscfg.Size = new System.Drawing.Size(327, 52);
            this.groupBox_cscfg.TabIndex = ++Table_Index;
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
			// groupBox_ts
			//
            Current_Height += Height_Group_Folder; // 高度增加
			this.groupBox_ts.Controls.Add(this.label_ts);
			this.groupBox_ts.Location = new System.Drawing.Point(14, Current_Height);
			this.groupBox_ts.Name = "groupBox_ts";
			this.groupBox_ts.Size = new System.Drawing.Size(327, 52);
			this.groupBox_ts.TabIndex = ++Table_Index;
			this.groupBox_ts.TabStop = false;
			this.groupBox_ts.Text = "ts script output"; // ts code
			//
            // label_ts
            //
			this.label_ts.Location = new System.Drawing.Point(6, 17);
			this.label_ts.Name = "label_ts";
			this.label_ts.Size = new System.Drawing.Size(315, 32);
			this.label_ts.TabIndex = 0;
			this.label_ts.Text = "select folder"; // 请选择excel目录
			this.label_ts.Click += new System.EventHandler(this.label_Click);

            //
			// groupBox_tscfg
			//
            Current_Height += Height_Group_Folder; // 高度增加
			this.groupBox_tscfg.Controls.Add(this.label_tscfg);
            this.groupBox_tscfg.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox_tscfg.Name = "groupBox_tscfg";
            this.groupBox_tscfg.Size = new System.Drawing.Size(327, 52);
            this.groupBox_tscfg.TabIndex = ++Table_Index;
            this.groupBox_tscfg.TabStop = false;
            this.groupBox_tscfg.Text = "ts config output"; // ts config
            //
			// label_tscfg
			//
			this.label_tscfg.Location = new System.Drawing.Point(6, 17);
			this.label_tscfg.Name = "label_tscfg";
			this.label_tscfg.Size = new System.Drawing.Size(315, 32);
			this.label_tscfg.TabIndex = 0;
			this.label_tscfg.Text = "select folder"; // 请选择excel目录
			this.label_tscfg.Click += new System.EventHandler(this.label_Click);

            
            // ----------------- checkbox ----------------------

            //
            // divfolder 是否分目录
            //
            Current_Height += Height_Group_Folder; // 高度增加
            this.divfolder.AutoSize = true;
            this.divfolder.Location = new System.Drawing.Point(14, Current_Height);
            this.divfolder.Name = "divfolder";
            this.divfolder.Size = new System.Drawing.Size(108, 16);
            this.divfolder.TabIndex = ++Table_Index;
            this.divfolder.Text = "output divfolder"; //导出数据分目录
            this.divfolder.UseVisualStyleBackColor = true;
            //
            // cacheTog 是否用缓存
            //
            this.cacheTog.AutoSize = true;
            this.cacheTog.Location = new System.Drawing.Point(124, Current_Height);
            this.cacheTog.Name = "cacheTog";
            this.cacheTog.Size = new System.Drawing.Size(72, 16);
            this.cacheTog.TabIndex = ++Table_Index;
            this.cacheTog.Text = "use cached";
            this.cacheTog.UseVisualStyleBackColor = true;
            this.cacheTog.CheckedChanged += check_change;
            //
            // isOutLua
            //
            Current_Height += Height_Group_CheckBox; // 高度增加
            this.isOutLua.AutoSize = true;
            this.isOutLua.Location = new System.Drawing.Point(14, Current_Height);
            this.isOutLua.Name = "isOutLua";
            this.isOutLua.Size = new System.Drawing.Size(72, 16);
            this.isOutLua.TabIndex = ++Table_Index;
            this.isOutLua.Text = "out lua";
            this.isOutLua.UseVisualStyleBackColor = true;
            this.isOutLua.CheckedChanged += check_change;
            //
            // isOutGO
            //
            this.isOutGO.AutoSize = true;
            this.isOutGO.Location = new System.Drawing.Point(86, Current_Height);
            this.isOutGO.Name = "isOutGO";
            this.isOutGO.Size = new System.Drawing.Size(72, 16);
            this.isOutGO.TabIndex = ++Table_Index;
            this.isOutGO.Text = "out go";
            this.isOutGO.UseVisualStyleBackColor = true;
            this.isOutGO.CheckedChanged += check_change;
            //
            // isOutCS
            //
            this.isOutCS.AutoSize = true;
            this.isOutCS.Location = new System.Drawing.Point(158, Current_Height);
            this.isOutCS.Name = "isOutCS";
            this.isOutCS.Size = new System.Drawing.Size(72, 16);
            this.isOutCS.TabIndex = ++Table_Index;
            this.isOutCS.Text = "out cs";
            this.isOutCS.UseVisualStyleBackColor = true;
            this.isOutCS.CheckedChanged += check_change;
            //
            // isOutTS
            //
            this.isOutTS.AutoSize = true;
            this.isOutTS.Location = new System.Drawing.Point(230, Current_Height);
            this.isOutTS.Name = "isOutTS";
            this.isOutTS.Size = new System.Drawing.Size(72, 16);
            this.isOutTS.TabIndex = ++Table_Index;
            this.isOutTS.Text = "out ts";
            this.isOutTS.UseVisualStyleBackColor = true;
            this.isOutTS.CheckedChanged += check_change;

            
            // ----------------- checkbox ----------------------

            // ----------------- 导出标签选择 --------------------
            //
            // groupBox5 导出标签group
            //
            Current_Height += Height_Group_Folder; // 高度增加
            this.groupBox5.Controls.Add(this.labelSelect);
            this.groupBox5.Location = new System.Drawing.Point(14, Current_Height);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 52);
            this.groupBox5.TabIndex = ++Table_Index;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "output label"; //导表标签
            
            //
            // labelSelect 标签label
            //
            this.labelSelect.FormattingEnabled = true;
            this.labelSelect.Location = new System.Drawing.Point(8, 21);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(313, 20);
            this.labelSelect.TabIndex = 0;

            // ----------------- 导出标签选择 --------------------

            //
            // 导出按钮
            //
            Current_Height += Height_Group_Folder;
            this.button2.Location = new System.Drawing.Point(266, Current_Height);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = ++Table_Index;
            this.button2.Text = "Just Do It!"; // 导出
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.onBtnOutput);

            //
            // Form1
            //
            Current_Height += 80;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, Current_Height);
            this.Controls.Add(this.isOutTS);
            this.Controls.Add(this.isOutCS);
            this.Controls.Add(this.isOutGO);
            this.Controls.Add(this.isOutLua);
            this.Controls.Add(this.divfolder);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.cacheTog);
            this.Controls.Add(this.groupBox_cscfg);
			this.Controls.Add(this.groupBox_cs);
			this.Controls.Add(this.groupBox_ts);
			this.Controls.Add(this.groupBox_tscfg);
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
            this.groupBox_ts.ResumeLayout(false);
            this.groupBox_tscfg.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox_ts;
        private System.Windows.Forms.Label label_ts;
        private System.Windows.Forms.GroupBox groupBox_tscfg;
        private System.Windows.Forms.Label label_tscfg;
        private System.Windows.Forms.GroupBox groupBox_cs;
        private System.Windows.Forms.Label label_cs;
        private System.Windows.Forms.GroupBox groupBox_cscfg;
        private System.Windows.Forms.Label label_cscfg;
        private System.Windows.Forms.CheckBox cacheTog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox labelSelect;
        private System.Windows.Forms.CheckBox divfolder;

        private System.Windows.Forms.CheckBox isOutLua; // 是否导出lua
        private System.Windows.Forms.CheckBox isOutGO;
        private System.Windows.Forms.CheckBox isOutCS;
        private System.Windows.Forms.CheckBox isOutTS;
    }
}
