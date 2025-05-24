namespace ecological_alert
{
    partial class SlopeForm
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
            this.lblSelectLayers = new System.Windows.Forms.Label();
            this.clbRasterLayers = new System.Windows.Forms.CheckedListBox();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSelectLayers
            // 
            this.lblSelectLayers.AutoSize = true;
            this.lblSelectLayers.Location = new System.Drawing.Point(51, 26);
            this.lblSelectLayers.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelectLayers.Name = "lblSelectLayers";
            this.lblSelectLayers.Size = new System.Drawing.Size(125, 12);
            this.lblSelectLayers.TabIndex = 0;
            this.lblSelectLayers.Text = "选择栅格图层（多选）";
            // 
            // clbRasterLayers
            // 
            this.clbRasterLayers.FormattingEnabled = true;
            this.clbRasterLayers.Location = new System.Drawing.Point(192, 26);
            this.clbRasterLayers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.clbRasterLayers.Name = "clbRasterLayers";
            this.clbRasterLayers.Size = new System.Drawing.Size(130, 84);
            this.clbRasterLayers.TabIndex = 1;
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(59, 138);
            this.lblOutputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(77, 12);
            this.lblOutputPath.TabIndex = 2;
            this.lblOutputPath.Text = "输出文件路径";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(148, 137);
            this.txtOutputPath.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(174, 21);
            this.txtOutputPath.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(333, 133);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(56, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "浏览...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(76, 184);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(272, 184);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SlopeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 225);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.lblOutputPath);
            this.Controls.Add(this.clbRasterLayers);
            this.Controls.Add(this.lblSelectLayers);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimizeBox = false;
            this.Name = "SlopeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Slope计算";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelectLayers;
        private System.Windows.Forms.CheckedListBox clbRasterLayers;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}