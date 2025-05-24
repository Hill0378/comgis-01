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

        private void 缓冲区分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BufferForm form = new BufferForm(this.axMapControl1);
            form.ShowDialog();
        }

        private void 赋权重ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WeightForm form = new WeightForm(this.axMapControl1);
            form.ShowDialog();
        }

        private void 年间差异ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RasterSubtractForm form = new RasterSubtractForm(this.axMapControl1);
            form.ShowDialog();

        }

        private void slope计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlopeForm form = new SlopeForm(axMapControl1); // 注意替换为你的 MapControl 控件名
            form.ShowDialog();

        }


        private void 重分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            negation form = new negation(axMapControl1);

            form.ShowDialog();
        }

        
        private void 均值求差ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_MeanDifference form = new Form_MeanDifference(axMapControl1);
            form.ShowDialog();
        }

        private void 赋颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignColorForm form = new AssignColorForm(axMapControl1); // 传入地图控件
            form.ShowDialog();
        }
    }
}
