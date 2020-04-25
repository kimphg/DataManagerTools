namespace ReadFileJsonFirst
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_start = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_sf = new System.Windows.Forms.Button();
            this.btn_openfd_density = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_fd_density = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_pathfd = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(401, 200);
            this.btn_start.Margin = new System.Windows.Forms.Padding(2);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(80, 30);
            this.btn_start.TabIndex = 8;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(17, 254);
            this.progress.Margin = new System.Windows.Forms.Padding(2);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(466, 19);
            this.progress.TabIndex = 10;
            this.progress.Click += new System.EventHandler(this.progress_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 279);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "--";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(404, 313);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_sf
            // 
            this.btn_sf.Location = new System.Drawing.Point(404, 41);
            this.btn_sf.Margin = new System.Windows.Forms.Padding(2);
            this.btn_sf.Name = "btn_sf";
            this.btn_sf.Size = new System.Drawing.Size(80, 30);
            this.btn_sf.TabIndex = 2;
            this.btn_sf.Text = "Select Folder";
            this.btn_sf.UseVisualStyleBackColor = true;
            this.btn_sf.Click += new System.EventHandler(this.btn_sf_Click);
            // 
            // btn_openfd_density
            // 
            this.btn_openfd_density.Location = new System.Drawing.Point(404, 113);
            this.btn_openfd_density.Margin = new System.Windows.Forms.Padding(2);
            this.btn_openfd_density.Name = "btn_openfd_density";
            this.btn_openfd_density.Size = new System.Drawing.Size(80, 30);
            this.btn_openfd_density.TabIndex = 4;
            this.btn_openfd_density.Text = "Select Folder";
            this.btn_openfd_density.UseVisualStyleBackColor = true;
            this.btn_openfd_density.Click += new System.EventHandler(this.btn_openfd_density_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 85);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Chọn đường dẫn file mật độ ";
            // 
            // txt_fd_density
            // 
            this.txt_fd_density.Location = new System.Drawing.Point(17, 113);
            this.txt_fd_density.Margin = new System.Windows.Forms.Padding(2);
            this.txt_fd_density.Multiline = true;
            this.txt_fd_density.Name = "txt_fd_density";
            this.txt_fd_density.Size = new System.Drawing.Size(361, 31);
            this.txt_fd_density.TabIndex = 5;
            this.txt_fd_density.TextChanged += new System.EventHandler(this.txt_fd_density_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Chọn đường đẫn folder file  của tàu";
            // 
            // txt_pathfd
            // 
            this.txt_pathfd.Location = new System.Drawing.Point(17, 41);
            this.txt_pathfd.Margin = new System.Windows.Forms.Padding(2);
            this.txt_pathfd.Multiline = true;
            this.txt_pathfd.Name = "txt_pathfd";
            this.txt_pathfd.Size = new System.Drawing.Size(361, 31);
            this.txt_pathfd.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 366);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_fd_density);
            this.Controls.Add(this.btn_openfd_density);
            this.Controls.Add(this.txt_pathfd);
            this.Controls.Add(this.btn_sf);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_sf;
        private System.Windows.Forms.Button btn_openfd_density;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_fd_density;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_pathfd;
    }
}

