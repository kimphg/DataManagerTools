namespace MergeBoat_Final
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_sf = new System.Windows.Forms.Button();
            this.txt_pathfd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_numFoder = new System.Windows.Forms.Label();
            this.txt_numFile = new System.Windows.Forms.Label();
            this.btn_startMerge = new System.Windows.Forms.Button();
            this.progressMeger = new System.Windows.Forms.ProgressBar();
            this.timer_thread = new System.Windows.Forms.Timer(this.components);
            this.btn_save = new System.Windows.Forms.Button();
            this.progressSaveFile = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_fd_sf = new System.Windows.Forms.TextBox();
            this.btn_openfd_sf = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_sf
            // 
            this.btn_sf.Location = new System.Drawing.Point(561, 58);
            this.btn_sf.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_sf.Name = "btn_sf";
            this.btn_sf.Size = new System.Drawing.Size(107, 37);
            this.btn_sf.TabIndex = 0;
            this.btn_sf.Text = "Select Folder";
            this.btn_sf.UseVisualStyleBackColor = true;
            this.btn_sf.Click += new System.EventHandler(this.btn_sf_Click);
            // 
            // txt_pathfd
            // 
            this.txt_pathfd.Location = new System.Drawing.Point(47, 58);
            this.txt_pathfd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_pathfd.Multiline = true;
            this.txt_pathfd.Name = "txt_pathfd";
            this.txt_pathfd.Size = new System.Drawing.Size(480, 37);
            this.txt_pathfd.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Số folder con : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Số folder đã xử lý:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txt_numFoder
            // 
            this.txt_numFoder.AutoSize = true;
            this.txt_numFoder.Location = new System.Drawing.Point(186, 124);
            this.txt_numFoder.Name = "txt_numFoder";
            this.txt_numFoder.Size = new System.Drawing.Size(76, 17);
            this.txt_numFoder.TabIndex = 4;
            this.txt_numFoder.Text = "num_foder";
            // 
            // txt_numFile
            // 
            this.txt_numFile.AutoSize = true;
            this.txt_numFile.Location = new System.Drawing.Point(186, 159);
            this.txt_numFile.Name = "txt_numFile";
            this.txt_numFile.Size = new System.Drawing.Size(153, 17);
            this.txt_numFile.TabIndex = 5;
            this.txt_numFile.Text = "num_folder_processed";
            // 
            // btn_startMerge
            // 
            this.btn_startMerge.Location = new System.Drawing.Point(305, 203);
            this.btn_startMerge.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_startMerge.Name = "btn_startMerge";
            this.btn_startMerge.Size = new System.Drawing.Size(107, 37);
            this.btn_startMerge.TabIndex = 6;
            this.btn_startMerge.Text = "Start Merge";
            this.btn_startMerge.UseVisualStyleBackColor = true;
            this.btn_startMerge.Click += new System.EventHandler(this.btn_startMerge_Click);
            // 
            // progressMeger
            // 
            this.progressMeger.Location = new System.Drawing.Point(47, 254);
            this.progressMeger.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressMeger.Name = "progressMeger";
            this.progressMeger.Size = new System.Drawing.Size(621, 23);
            this.progressMeger.TabIndex = 7;
            // 
            // timer_thread
            // 
            this.timer_thread.Tick += new System.EventHandler(this.timer_thread_Tick);
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(305, 132);
            this.btn_save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(107, 37);
            this.btn_save.TabIndex = 8;
            this.btn_save.Text = "Save File";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // progressSaveFile
            // 
            this.progressSaveFile.Location = new System.Drawing.Point(47, 182);
            this.progressSaveFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressSaveFile.Name = "progressSaveFile";
            this.progressSaveFile.Size = new System.Drawing.Size(621, 23);
            this.progressSaveFile.TabIndex = 9;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_startMerge);
            this.groupBox1.Controls.Add(this.progressMeger);
            this.groupBox1.Controls.Add(this.txt_pathfd);
            this.groupBox1.Controls.Add(this.txt_numFile);
            this.groupBox1.Controls.Add(this.btn_sf);
            this.groupBox1.Controls.Add(this.txt_numFoder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(710, 307);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Xử lý file tàu ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txt_fd_sf);
            this.groupBox2.Controls.Add(this.btn_openfd_sf);
            this.groupBox2.Controls.Add(this.btn_save);
            this.groupBox2.Controls.Add(this.progressSaveFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 347);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(710, 222);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Lưu lại file đã merge";
            // 
            // txt_fd_sf
            // 
            this.txt_fd_sf.Location = new System.Drawing.Point(46, 47);
            this.txt_fd_sf.Multiline = true;
            this.txt_fd_sf.Name = "txt_fd_sf";
            this.txt_fd_sf.Size = new System.Drawing.Size(480, 37);
            this.txt_fd_sf.TabIndex = 11;
            // 
            // btn_openfd_sf
            // 
            this.btn_openfd_sf.Location = new System.Drawing.Point(562, 47);
            this.btn_openfd_sf.Name = "btn_openfd_sf";
            this.btn_openfd_sf.Size = new System.Drawing.Size(106, 37);
            this.btn_openfd_sf.TabIndex = 10;
            this.btn_openfd_sf.Text = "Select Folder";
            this.btn_openfd_sf.UseVisualStyleBackColor = true;
            this.btn_openfd_sf.Click += new System.EventHandler(this.btn_openfd_sf_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Chọn đường dẫn folder toạ độ tàu : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Chọn đường dẫn lưu file :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 683);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_sf;
        private System.Windows.Forms.TextBox txt_pathfd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txt_numFoder;
        private System.Windows.Forms.Label txt_numFile;
        private System.Windows.Forms.Button btn_startMerge;
        private System.Windows.Forms.ProgressBar progressMeger;
        private System.Windows.Forms.Timer timer_thread;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.ProgressBar progressSaveFile;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_fd_sf;
        private System.Windows.Forms.Button btn_openfd_sf;
    }
}

