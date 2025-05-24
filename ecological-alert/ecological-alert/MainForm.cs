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

namespace ecological_alert
{
    public partial class Mainform : Form
    {

        public static Mainform pMainWin = null;
        //private readonly ShapefileLoader _shapefileLoader;
        private readonly ShpTimeAnimationController _shpAnimator;

        // 添加两个私有字段
        //private readonly ShpTimeAnimationController _shpAnimator;
        //private readonly ShapefileLoader _shapefileLoader;

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
    }
}
