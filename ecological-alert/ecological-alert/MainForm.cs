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
        private CustomTimeSliderButton customTime = new CustomTimeSliderButton();
        private ITimeExtent pLayerTimeExtent = null;
        private Int32 pCount = 0;

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

        private void 可用图层判断ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("没有可用的图层！");
            }
            else
            {
                ILayer pLayer = axMapControl1.get_Layer(0);
                string layerName = axMapControl1.get_Layer(0).Name;

                // 通过公共属性传递MapControl
                customTime.Time(pLayer, this.MapControl);
            }
        }

        private void openshp_Click(object sender, EventArgs e)
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
        }

        private void create_view_Click(object sender, EventArgs e)
        {
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
            if (pLayerTimeExtent == null) return;

            IMap pMap = axMapControl1.Map;
            IActiveView pActiveView = pMap as IActiveView;
            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            ITimeDisplay pTimeDisplay = pScreenDisplay as ITimeDisplay;

            ITime startTime = pLayerTimeExtent.StartTime;
            ITime endTime = (ITime)((IClone)startTime).Clone();

            // 每次递进12小时
            ((ITimeOffsetOperator)startTime).AddHours(12.0 * (pCount - 1));
            ((ITimeOffsetOperator)endTime).AddHours(12.0 * pCount);

            ITimeExtent pTimeExt = new TimeExtent() as ITimeExtent;
            pTimeExt.SetExtent(startTime, endTime);
            pTimeExt.Empty = false;

            pTimeDisplay.TimeValue = pTimeExt as ITimeValue;
            pActiveView.Refresh();

            pCount += 1;

            // 当前浏览时间已到达图层时态数据终止时间时，停止timer控件
            if (endTime.Compare(pLayerTimeExtent.EndTime) == 1)
            {
                timer1.Enabled = false;
                judge.Enabled = true;
            }
        }

        private void 栅格动画ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ndviFolderPath = @"D:\homework\GISkaifa\data";
            CustomTimeSliderButton timeSlider = new CustomTimeSliderButton();
            // 传递null作为图层参数，提供文件夹路径
            timeSlider.Time(null, axMapControl1, ndviFolderPath);
        }
    }
}
