namespace ecological_alert
{
    partial class MosaicDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.methodLabel = new System.Windows.Forms.Label();
            this.cboMethod = new System.Windows.Forms.ComboBox();
            this.Btbrowse = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.outputPathLabel = new System.Windows.Forms.Label();
            this.clbRasterLayers = new System.Windows.Forms.CheckedListBox();
            this.rasterLayersLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.methodLabel);
            this.panel1.Controls.Add(this.cboMethod);
            this.panel1.Controls.Add(this.Btbrowse);
            this.panel1.Controls.Add(this.outputTextBox);
            this.panel1.Controls.Add(this.outputPathLabel);
            this.panel1.Controls.Add(this.clbRasterLayers);
            this.panel1.Controls.Add(this.rasterLayersLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(388, 349);
            this.panel1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(99, 261);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(267, 261);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // methodLabel
            // 
            this.methodLabel.AutoSize = true;
            this.methodLabel.Location = new System.Drawing.Point(13, 165);
            this.methodLabel.Name = "methodLabel";
            this.methodLabel.Size = new System.Drawing.Size(67, 15);
            this.methodLabel.TabIndex = 8;
            this.methodLabel.Text = "镶嵌方法";
            // 
            // cboMethod
            // 
            this.cboMethod.FormattingEnabled = true;
            this.cboMethod.Location = new System.Drawing.Point(128, 162);
            this.cboMethod.Name = "cboMethod";
            this.cboMethod.Size = new System.Drawing.Size(121, 23);
            this.cboMethod.TabIndex = 7;
            // 
            // Btbrowse
            // 
            this.Btbrowse.Location = new System.Drawing.Point(301, 194);
            this.Btbrowse.Name = "Btbrowse";
            this.Btbrowse.Size = new System.Drawing.Size(75, 23);
            this.Btbrowse.TabIndex = 4;
            this.Btbrowse.Text = "浏览...";
            this.Btbrowse.UseVisualStyleBackColor = true;
            this.Btbrowse.Click += new System.EventHandler(this.Btbrowse_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(128, 191);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(150, 25);
            this.outputTextBox.TabIndex = 3;
            // 
            // outputPathLabel
            // 
            this.outputPathLabel.AutoSize = true;
            this.outputPathLabel.Location = new System.Drawing.Point(12, 194);
            this.outputPathLabel.Name = "outputPathLabel";
            this.outputPathLabel.Size = new System.Drawing.Size(97, 15);
            this.outputPathLabel.TabIndex = 2;
            this.outputPathLabel.Text = "输出文件路径";
            // 
            // clbRasterLayers
            // 
            this.clbRasterLayers.FormattingEnabled = true;
            this.clbRasterLayers.Location = new System.Drawing.Point(160, 38);
            this.clbRasterLayers.Name = "clbRasterLayers";
            this.clbRasterLayers.Size = new System.Drawing.Size(216, 104);
            this.clbRasterLayers.TabIndex = 1;
            // 
            // rasterLayersLabel
            // 
            this.rasterLayersLabel.AutoSize = true;
            this.rasterLayersLabel.Location = new System.Drawing.Point(-3, 38);
            this.rasterLayersLabel.Name = "rasterLayersLabel";
            this.rasterLayersLabel.Size = new System.Drawing.Size(157, 15);
            this.rasterLayersLabel.TabIndex = 0;
            this.rasterLayersLabel.Text = "选择栅格图层（多选）";
            // 
            // MosaicDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 349);
            this.Controls.Add(this.panel1);
            this.Name = "MosaicDialog";
            this.Text = "镶嵌";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btbrowse;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label outputPathLabel;
        private System.Windows.Forms.CheckedListBox clbRasterLayers;
        private System.Windows.Forms.Label rasterLayersLabel;
        private System.Windows.Forms.Label methodLabel;
        private System.Windows.Forms.ComboBox cboMethod;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}