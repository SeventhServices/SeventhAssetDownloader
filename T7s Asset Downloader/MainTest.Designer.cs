namespace T7s_Asset_Downloader
{
    partial class Main_Test
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
            this.listBoxResult = new System.Windows.Forms.ListBox();
            this.button_GetAllIndex = new System.Windows.Forms.Button();
            this.button_LoadAllIndex = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_LoadNowNewIndex = new System.Windows.Forms.Button();
            this.button_GetNowNewIndex = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_LoadConfing = new System.Windows.Forms.Button();
            this.button_GetConfing = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_DecryptAllFilesInNew = new System.Windows.Forms.Button();
            this.button_DecryptAllfiles = new System.Windows.Forms.Button();
            this.button_DownloadAllFiles = new System.Windows.Forms.Button();
            this.button_DownloadCheckFiles = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxResult
            // 
            this.listBoxResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxResult.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxResult.FormattingEnabled = true;
            this.listBoxResult.ItemHeight = 24;
            this.listBoxResult.Location = new System.Drawing.Point(372, 13);
            this.listBoxResult.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBoxResult.Name = "listBoxResult";
            this.listBoxResult.Size = new System.Drawing.Size(327, 386);
            this.listBoxResult.TabIndex = 0;
            // 
            // button_GetAllIndex
            // 
            this.button_GetAllIndex.BackColor = System.Drawing.SystemColors.Window;
            this.button_GetAllIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_GetAllIndex.Location = new System.Drawing.Point(18, 43);
            this.button_GetAllIndex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_GetAllIndex.Name = "button_GetAllIndex";
            this.button_GetAllIndex.Size = new System.Drawing.Size(135, 37);
            this.button_GetAllIndex.TabIndex = 1;
            this.button_GetAllIndex.Text = "获取全部索引";
            this.button_GetAllIndex.UseVisualStyleBackColor = false;
            this.button_GetAllIndex.Click += new System.EventHandler(this.Button_GetAllIndex_Click);
            // 
            // button_LoadAllIndex
            // 
            this.button_LoadAllIndex.BackColor = System.Drawing.SystemColors.Window;
            this.button_LoadAllIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_LoadAllIndex.Location = new System.Drawing.Point(176, 43);
            this.button_LoadAllIndex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LoadAllIndex.Name = "button_LoadAllIndex";
            this.button_LoadAllIndex.Size = new System.Drawing.Size(135, 37);
            this.button_LoadAllIndex.TabIndex = 2;
            this.button_LoadAllIndex.Text = "加载全部索引";
            this.button_LoadAllIndex.UseVisualStyleBackColor = false;
            this.button_LoadAllIndex.Click += new System.EventHandler(this.Button_LoadAllIndex_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_LoadNowNewIndex);
            this.groupBox1.Controls.Add(this.button_GetNowNewIndex);
            this.groupBox1.Controls.Add(this.button_LoadAllIndex);
            this.groupBox1.Controls.Add(this.button_GetAllIndex);
            this.groupBox1.Location = new System.Drawing.Point(23, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(329, 152);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "索引文件";
            // 
            // button_LoadNowNewIndex
            // 
            this.button_LoadNowNewIndex.BackColor = System.Drawing.SystemColors.Window;
            this.button_LoadNowNewIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_LoadNowNewIndex.Location = new System.Drawing.Point(175, 97);
            this.button_LoadNowNewIndex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LoadNowNewIndex.Name = "button_LoadNowNewIndex";
            this.button_LoadNowNewIndex.Size = new System.Drawing.Size(135, 37);
            this.button_LoadNowNewIndex.TabIndex = 4;
            this.button_LoadNowNewIndex.Text = "加载当前版本";
            this.button_LoadNowNewIndex.UseVisualStyleBackColor = false;
            // 
            // button_GetNowNewIndex
            // 
            this.button_GetNowNewIndex.BackColor = System.Drawing.SystemColors.Window;
            this.button_GetNowNewIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_GetNowNewIndex.Location = new System.Drawing.Point(17, 97);
            this.button_GetNowNewIndex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_GetNowNewIndex.Name = "button_GetNowNewIndex";
            this.button_GetNowNewIndex.Size = new System.Drawing.Size(135, 37);
            this.button_GetNowNewIndex.TabIndex = 3;
            this.button_GetNowNewIndex.Text = "获取当前版本";
            this.button_GetNowNewIndex.UseVisualStyleBackColor = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_LoadConfing);
            this.groupBox2.Controls.Add(this.button_GetConfing);
            this.groupBox2.Location = new System.Drawing.Point(27, 170);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(325, 82);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Confing文件";
            // 
            // button_LoadConfing
            // 
            this.button_LoadConfing.BackColor = System.Drawing.SystemColors.Window;
            this.button_LoadConfing.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_LoadConfing.Location = new System.Drawing.Point(172, 27);
            this.button_LoadConfing.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LoadConfing.Name = "button_LoadConfing";
            this.button_LoadConfing.Size = new System.Drawing.Size(135, 37);
            this.button_LoadConfing.TabIndex = 3;
            this.button_LoadConfing.Text = "加载Confing文件";
            this.button_LoadConfing.UseVisualStyleBackColor = false;
            // 
            // button_GetConfing
            // 
            this.button_GetConfing.BackColor = System.Drawing.SystemColors.Window;
            this.button_GetConfing.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_GetConfing.Location = new System.Drawing.Point(14, 27);
            this.button_GetConfing.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_GetConfing.Name = "button_GetConfing";
            this.button_GetConfing.Size = new System.Drawing.Size(135, 37);
            this.button_GetConfing.TabIndex = 2;
            this.button_GetConfing.Text = "获取Confing文件";
            this.button_GetConfing.UseVisualStyleBackColor = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_DecryptAllFilesInNew);
            this.groupBox3.Controls.Add(this.button_DecryptAllfiles);
            this.groupBox3.Controls.Add(this.button_DownloadAllFiles);
            this.groupBox3.Controls.Add(this.button_DownloadCheckFiles);
            this.groupBox3.Location = new System.Drawing.Point(23, 258);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 141);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "下载设置";
            // 
            // button_DecryptAllFilesInNew
            // 
            this.button_DecryptAllFilesInNew.BackColor = System.Drawing.SystemColors.Window;
            this.button_DecryptAllFilesInNew.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DecryptAllFilesInNew.Location = new System.Drawing.Point(175, 82);
            this.button_DecryptAllFilesInNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_DecryptAllFilesInNew.Name = "button_DecryptAllFilesInNew";
            this.button_DecryptAllFilesInNew.Size = new System.Drawing.Size(135, 37);
            this.button_DecryptAllFilesInNew.TabIndex = 6;
            this.button_DecryptAllFilesInNew.Text = "解密至新文件夹";
            this.button_DecryptAllFilesInNew.UseVisualStyleBackColor = false;
            // 
            // button_DecryptAllfiles
            // 
            this.button_DecryptAllfiles.BackColor = System.Drawing.SystemColors.Window;
            this.button_DecryptAllfiles.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DecryptAllfiles.Location = new System.Drawing.Point(18, 82);
            this.button_DecryptAllfiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_DecryptAllfiles.Name = "button_DecryptAllfiles";
            this.button_DecryptAllfiles.Size = new System.Drawing.Size(135, 37);
            this.button_DecryptAllfiles.TabIndex = 5;
            this.button_DecryptAllfiles.Text = "解密全部文件";
            this.button_DecryptAllfiles.UseVisualStyleBackColor = false;
            // 
            // button_DownloadAllFiles
            // 
            this.button_DownloadAllFiles.BackColor = System.Drawing.SystemColors.Window;
            this.button_DownloadAllFiles.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DownloadAllFiles.Location = new System.Drawing.Point(18, 27);
            this.button_DownloadAllFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_DownloadAllFiles.Name = "button_DownloadAllFiles";
            this.button_DownloadAllFiles.Size = new System.Drawing.Size(135, 37);
            this.button_DownloadAllFiles.TabIndex = 4;
            this.button_DownloadAllFiles.Text = "下载全部文件";
            this.button_DownloadAllFiles.UseVisualStyleBackColor = false;
            // 
            // button_DownloadCheckFiles
            // 
            this.button_DownloadCheckFiles.BackColor = System.Drawing.SystemColors.Window;
            this.button_DownloadCheckFiles.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DownloadCheckFiles.Location = new System.Drawing.Point(176, 27);
            this.button_DownloadCheckFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_DownloadCheckFiles.Name = "button_DownloadCheckFiles";
            this.button_DownloadCheckFiles.Size = new System.Drawing.Size(135, 37);
            this.button_DownloadCheckFiles.TabIndex = 3;
            this.button_DownloadCheckFiles.Text = "下载选中文件";
            this.button_DownloadCheckFiles.UseVisualStyleBackColor = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(714, 407);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBoxResult);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Main";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxResult;
        private System.Windows.Forms.Button button_GetAllIndex;
        private System.Windows.Forms.Button button_LoadAllIndex;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_LoadNowNewIndex;
        private System.Windows.Forms.Button button_GetNowNewIndex;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_LoadConfing;
        private System.Windows.Forms.Button button_GetConfing;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_DecryptAllFilesInNew;
        private System.Windows.Forms.Button button_DecryptAllfiles;
        private System.Windows.Forms.Button button_DownloadAllFiles;
        private System.Windows.Forms.Button button_DownloadCheckFiles;
    }
}

