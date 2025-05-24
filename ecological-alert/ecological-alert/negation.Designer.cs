namespace ecological_alert
{
    partial class negation
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
            this.rasterVectorLabel1 = new System.Windows.Forms.Label();
            this.clbVectorLayers1 = new System.Windows.Forms.CheckedListBox();
            this.rasterVectorLabel2 = new System.Windows.Forms.Label();
            this.clbVectorLayers2 = new System.Windows.Forms.CheckedListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rasterVectorLabel1
            // 
            this.rasterVectorLabel1.AutoSize = true;
            this.rasterVectorLabel1.Location = new System.Drawing.Point(12, 42);
            this.rasterVectorLabel1.Name = "rasterVectorLabel1";
            this.rasterVectorLabel1.Size = new System.Drawing.Size(67, 15);
            this.rasterVectorLabel1.TabIndex = 3;
            this.rasterVectorLabel1.Text = "输入要素";
            // 
            // clbVectorLayers1
            // 
            this.clbVectorLayers1.FormattingEnabled = true;
            this.clbVectorLayers1.Location = new System.Drawing.Point(150, 26);
            this.clbVectorLayers1.Name = "clbVectorLayers1";
            this.clbVectorLayers1.Size = new System.Drawing.Size(260, 84);
            this.clbVectorLayers1.TabIndex = 4;
            // 
            // rasterVectorLabel2
            // 
            this.rasterVectorLabel2.AutoSize = true;
            this.rasterVectorLabel2.Location = new System.Drawing.Point(12, 174);
            this.rasterVectorLabel2.Name = "rasterVectorLabel2";
            this.rasterVectorLabel2.Size = new System.Drawing.Size(67, 15);
            this.rasterVectorLabel2.TabIndex = 5;
            this.rasterVectorLabel2.Text = "更新要素";
            // 
            // clbVectorLayers2
            // 
            this.clbVectorLayers2.FormattingEnabled = true;
            this.clbVectorLayers2.Location = new System.Drawing.Point(150, 156);
            this.clbVectorLayers2.Name = "clbVectorLayers2";
            this.clbVectorLayers2.Size = new System.Drawing.Size(260, 84);
            this.clbVectorLayers2.TabIndex = 6;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(49, 304);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(245, 304);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // negation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 450);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.clbVectorLayers2);
            this.Controls.Add(this.rasterVectorLabel2);
            this.Controls.Add(this.clbVectorLayers1);
            this.Controls.Add(this.rasterVectorLabel1);
            this.Name = "negation";
            this.Text = "negation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label rasterVectorLabel1;
        private System.Windows.Forms.CheckedListBox clbVectorLayers1;
        private System.Windows.Forms.Label rasterVectorLabel2;
        private System.Windows.Forms.CheckedListBox clbVectorLayers2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}