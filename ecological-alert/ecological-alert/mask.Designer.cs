namespace ecological_alert
{
    partial class mask
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
            this.rasterLayersLabel = new System.Windows.Forms.Label();
            this.clbRasterLayers = new System.Windows.Forms.CheckedListBox();
            this.rasterVectorLabel = new System.Windows.Forms.Label();
            this.clbVectorLayers = new System.Windows.Forms.CheckedListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rasterLayersLabel
            // 
            this.rasterLayersLabel.AutoSize = true;
            this.rasterLayersLabel.Location = new System.Drawing.Point(0, 0);
            this.rasterLayersLabel.Name = "rasterLayersLabel";
            this.rasterLayersLabel.Size = new System.Drawing.Size(157, 15);
            this.rasterLayersLabel.TabIndex = 0;
            this.rasterLayersLabel.Text = "选择栅格图层（多选）";
            // 
            // clbRasterLayers
            // 
            this.clbRasterLayers.FormattingEnabled = true;
            this.clbRasterLayers.Location = new System.Drawing.Point(191, 0);
            this.clbRasterLayers.Name = "clbRasterLayers";
            this.clbRasterLayers.Size = new System.Drawing.Size(260, 84);
            this.clbRasterLayers.TabIndex = 1;
            // 
            // rasterVectorLabel
            // 
            this.rasterVectorLabel.AutoSize = true;
            this.rasterVectorLabel.Location = new System.Drawing.Point(12, 150);
            this.rasterVectorLabel.Name = "rasterVectorLabel";
            this.rasterVectorLabel.Size = new System.Drawing.Size(97, 15);
            this.rasterVectorLabel.TabIndex = 2;
            this.rasterVectorLabel.Text = "选择矢量图层";
            // 
            // clbVectorLayers
            // 
            this.clbVectorLayers.FormattingEnabled = true;
            this.clbVectorLayers.Location = new System.Drawing.Point(191, 130);
            this.clbVectorLayers.Name = "clbVectorLayers";
            this.clbVectorLayers.Size = new System.Drawing.Size(260, 84);
            this.clbVectorLayers.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(82, 313);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(267, 313);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // mask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 450);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.clbVectorLayers);
            this.Controls.Add(this.rasterVectorLabel);
            this.Controls.Add(this.clbRasterLayers);
            this.Controls.Add(this.rasterLayersLabel);
            this.Name = "mask";
            this.Text = "mask";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label rasterLayersLabel;
        private System.Windows.Forms.CheckedListBox clbRasterLayers;
        private System.Windows.Forms.Label rasterVectorLabel;
        private System.Windows.Forms.CheckedListBox clbVectorLayers;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}