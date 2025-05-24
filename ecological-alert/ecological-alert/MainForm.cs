using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MakeACustomTimeControl2008;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
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


        public static Mainform pMainWin = null;
        private readonly ShpTimeAnimationController _shpAnimator;


        public Mainform()
        {
            InitializeComponent();
            pMainWin = this;
        }

        // 公开MapControl的公共属性
        public AxMapControl MapControl
        {
            get { return axMapControl1; }
        }       

        private void Form1_Load(object sender, EventArgs e)
        {
            axToolbarControl1.SetBuddyControl(axMapControl1);
            axTOCControl1.SetBuddyControl(axMapControl1);
        }      

        private void create_view_Click(object sender, EventArgs e)
        {
            
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "shapefile文件(*.shp)|*.shp";
            openFileDialog1.Title = "打开矢量数据";
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            string pPath = openFileDialog1.FileName;
            string pFolder = System.IO.Path.GetDirectoryName(pPath);
            string pFileName = System.IO.Path.GetFileName(pPath);

            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pFolder, 0);
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            IFeatureClass pFC = pFeatureWorkspace.OpenFeatureClass(pFileName);

            IFeatureLayer pFLayer = new FeatureLayer();
            pFLayer.FeatureClass = pFC;
            pFLayer.Name = pFC.AliasName;

            IMap pMap = axMapControl1.Map;
            pMap.AddLayer(pFLayer as ILayer);
            axMapControl1.ActiveView.Refresh();
            axTOCControl1.SetBuddyControl(axMapControl1);

            if (axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("请先加载图层！");
                return;
            }

            frmTime p = new frmTime(axMapControl1.Map);
            p.PLayer = axMapControl1.get_Layer(0) as IFeatureLayer;
            p.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            // 计时器事件现在由ShpTimeAnimationController处理
        }

        private void 栅格动画ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 栅格动画展示
            RasterBandAnimation.ShowRasterAnimation(axMapControl1);

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
