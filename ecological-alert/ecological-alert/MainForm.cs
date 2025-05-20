using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using ESRI.ArcGIS.SpatialAnalyst;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geoprocessing;
using System.IO;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;




namespace ecological_alert
{
    public partial class Mainform : Form
    {


        public Mainform()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            //InitializeMapControl();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //axToolbarControl1.SetBuddyControl(axMapControl1);
            //axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void InitializeMapControl()
        {
            // 初始化地图控件
            axMapControl1 = new AxMapControl();
            axMapControl1.Dock = DockStyle.Fill;
            this.Controls.Add(axMapControl1);
        }

        private void 按掩膜提取ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            mask form = new mask(axMapControl1);

            form.ShowDialog();
        }

            private void 镶嵌ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MosaicDialog form = new MosaicDialog(axMapControl1);
            
                form.ShowDialog();
            
        }

    }
}



