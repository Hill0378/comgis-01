namespace ecological_alert
{
    partial class RasterSubtractForm
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
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.btnLoadLayers = new System.Windows.Forms.Button();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lstRasterLayers = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(229, 301);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(308, 35);
            this.txtOutputFolder.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(561, 301);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(139, 40);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "选择路径";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(149, 382);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(108, 39);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "执行";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(99, 304);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(130, 24);
            this.lblOutputFolder.TabIndex = 4;
            this.lblOutputFolder.Text = "输出路径：";
            // 
            // btnLoadLayers
            // 
            this.btnLoadLayers.Location = new System.Drawing.Point(505, 170);
            this.btnLoadLayers.Name = "btnLoadLayers";
            this.btnLoadLayers.Size = new System.Drawing.Size(139, 40);
            this.btnLoadLayers.TabIndex = 5;
            this.btnLoadLayers.Text = "加载图层";
            this.btnLoadLayers.UseVisualStyleBackColor = true;
            this.btnLoadLayers.Click += new System.EventHandler(this.btnLoadLayers_Click);
            // 
            // lstRasterLayers
            // 
            this.lstRasterLayers.FormattingEnabled = true;
            this.lstRasterLayers.ItemHeight = 24;
            this.lstRasterLayers.Location = new System.Drawing.Point(202, 62);
            this.lstRasterLayers.Name = "lstRasterLayers";
            this.lstRasterLayers.Size = new System.Drawing.Size(270, 148);
            this.lstRasterLayers.TabIndex = 6;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(532, 382);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 39);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // RasterSubtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lstRasterLayers);
            this.Controls.Add(this.btnLoadLayers);
            this.Controls.Add(this.lblOutputFolder);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtOutputFolder);
            this.Name = "RasterSubtractForm";
            this.Text = "年间作差";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.Button btnLoadLayers;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.ListBox lstRasterLayers;
        private System.Windows.Forms.Button btnCancel;
    }
}