namespace ecological_alert
{
    partial class Mainform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.动态监测ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slope计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.年间差异ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.动画ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.预警ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.缓冲区分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.均值求差ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.按掩膜提取ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.赋权重ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.镶嵌ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重分类ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.赋颜色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.axMapControl1 = new ESRI.ArcGIS.Controls.AxMapControl();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.动态监测ToolStripMenuItem,
            this.预警ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(12, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(800, 46);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 动态监测ToolStripMenuItem
            // 
            this.动态监测ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slope计算ToolStripMenuItem,
            this.年间差异ToolStripMenuItem,
            this.动画ToolStripMenuItem});
            this.动态监测ToolStripMenuItem.Name = "动态监测ToolStripMenuItem";
            this.动态监测ToolStripMenuItem.Size = new System.Drawing.Size(122, 38);
            this.动态监测ToolStripMenuItem.Text = "动态监测";
            // 
            // slope计算ToolStripMenuItem
            // 
            this.slope计算ToolStripMenuItem.Name = "slope计算ToolStripMenuItem";
            this.slope计算ToolStripMenuItem.Size = new System.Drawing.Size(221, 38);
            this.slope计算ToolStripMenuItem.Text = "slope计算";
            this.slope计算ToolStripMenuItem.Click += new System.EventHandler(this.slope计算ToolStripMenuItem_Click);

            // 
            // 年间差异ToolStripMenuItem
            // 
            this.年间差异ToolStripMenuItem.Name = "年间差异ToolStripMenuItem";
            this.年间差异ToolStripMenuItem.Size = new System.Drawing.Size(221, 38);
            this.年间差异ToolStripMenuItem.Text = "年间差异";
            this.年间差异ToolStripMenuItem.Click += new System.EventHandler(this.年间差异ToolStripMenuItem_Click);

            // 
            // 动画ToolStripMenuItem
            // 
            this.动画ToolStripMenuItem.Name = "动画ToolStripMenuItem";
            this.动画ToolStripMenuItem.Size = new System.Drawing.Size(221, 38);
            this.动画ToolStripMenuItem.Text = "动画";
            // 
            // 预警ToolStripMenuItem
            // 
            this.预警ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.缓冲区分析ToolStripMenuItem,
            this.均值求差ToolStripMenuItem,
            this.按掩膜提取ToolStripMenuItem1,
            this.赋权重ToolStripMenuItem,
            this.镶嵌ToolStripMenuItem,
            this.重分类ToolStripMenuItem,
            this.赋颜色ToolStripMenuItem});
            this.预警ToolStripMenuItem.Name = "预警ToolStripMenuItem";
            this.预警ToolStripMenuItem.Size = new System.Drawing.Size(74, 38);
            this.预警ToolStripMenuItem.Text = "预警";
            // 
            // 缓冲区分析ToolStripMenuItem
            // 
            this.缓冲区分析ToolStripMenuItem.Name = "缓冲区分析ToolStripMenuItem";
            this.缓冲区分析ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.缓冲区分析ToolStripMenuItem.Text = "缓冲区分析";
            this.缓冲区分析ToolStripMenuItem.Click += new System.EventHandler(this.缓冲区分析ToolStripMenuItem_Click);
            // 
            // 均值求差ToolStripMenuItem
            // 
            this.均值求差ToolStripMenuItem.Name = "均值求差ToolStripMenuItem";
            this.均值求差ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.均值求差ToolStripMenuItem.Text = "均值求差";
            this.均值求差ToolStripMenuItem.Click += new System.EventHandler(this.均值求差ToolStripMenuItem_Click);
            // 
            // 按掩膜提取ToolStripMenuItem1
            // 
            this.按掩膜提取ToolStripMenuItem1.Name = "按掩膜提取ToolStripMenuItem1";
            this.按掩膜提取ToolStripMenuItem1.Size = new System.Drawing.Size(324, 38);
            this.按掩膜提取ToolStripMenuItem1.Text = "按掩膜提取";
            // 
            // 赋权重ToolStripMenuItem
            // 
            this.赋权重ToolStripMenuItem.Name = "赋权重ToolStripMenuItem";
            this.赋权重ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.赋权重ToolStripMenuItem.Text = "赋权重";
            this.赋权重ToolStripMenuItem.Click += new System.EventHandler(this.赋权重ToolStripMenuItem_Click);
            // 
            // 镶嵌ToolStripMenuItem
            // 
            this.镶嵌ToolStripMenuItem.Name = "镶嵌ToolStripMenuItem";
            this.镶嵌ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.镶嵌ToolStripMenuItem.Text = "镶嵌";
            // 
            // 重分类ToolStripMenuItem
            // 
            this.重分类ToolStripMenuItem.Name = "重分类ToolStripMenuItem";
            this.重分类ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.重分类ToolStripMenuItem.Text = "重分类";
            // 
            // 赋颜色ToolStripMenuItem
            // 
            this.赋颜色ToolStripMenuItem.Name = "赋颜色ToolStripMenuItem";
            this.赋颜色ToolStripMenuItem.Size = new System.Drawing.Size(324, 38);
            this.赋颜色ToolStripMenuItem.Text = "赋颜色";
            this.赋颜色ToolStripMenuItem.Click += new System.EventHandler(this.赋颜色ToolStripMenuItem_Click);

            // 
            // axToolbarControl1
            // 
            this.axToolbarControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.axToolbarControl1.Location = new System.Drawing.Point(0, 46);
            this.axToolbarControl1.Margin = new System.Windows.Forms.Padding(6);
            this.axToolbarControl1.Name = "axToolbarControl1";
            this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
            this.axToolbarControl1.Size = new System.Drawing.Size(800, 28);
            this.axToolbarControl1.TabIndex = 1;
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Location = new System.Drawing.Point(0, 47);
            this.axTOCControl1.Margin = new System.Windows.Forms.Padding(6);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(204, 344);
            this.axTOCControl1.TabIndex = 2;
            // 
            // axMapControl1
            // 
            this.axMapControl1.Location = new System.Drawing.Point(120, 47);
            this.axMapControl1.Margin = new System.Windows.Forms.Padding(6);
            this.axMapControl1.Name = "axMapControl1";
            this.axMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl1.OcxState")));
            this.axMapControl1.Size = new System.Drawing.Size(536, 344);
            this.axMapControl1.TabIndex = 3;
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(234, 136);
            this.axLicenseControl1.Margin = new System.Windows.Forms.Padding(6);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 4;
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.axLicenseControl1);
            this.Controls.Add(this.axMapControl1);
            this.Controls.Add(this.axTOCControl1);
            this.Controls.Add(this.axToolbarControl1);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Mainform";
            this.Text = "ecological-alert";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        private ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
        private System.Windows.Forms.ToolStripMenuItem 动态监测ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slope计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 年间差异ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 动画ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 预警ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 缓冲区分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 均值求差ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 按掩膜提取ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 赋权重ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 镶嵌ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重分类ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 赋颜色ToolStripMenuItem;
    }
}

