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

namespace time_block
{
    public partial class Form1 : Form
    {
        public static Form1 pMainWin = null;
        public Form1()
        {
            InitializeComponent();
            pMainWin = this;
        }
        CustomTimeSliderButton customTime = new CustomTimeSliderButton();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("没有可用的图层！");
            }
            else
            {
                ILayer pLayer = axMapControl1.get_Layer(0);
                string s = axMapControl1.get_Layer(0).Name;
                customTime.Time(pLayer);
            }

        }

        private void axToolbarControl1_OnMouseDown(object sender, IToolbarControlEvents_OnMouseDownEvent e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "shapefile文件(*.shp)|*.shp";
            openFileDialog1.Title = "打开矢量数据";
            openFileDialog1.Multiselect = false;
            DialogResult pDialogResult = openFileDialog1.ShowDialog();
            if (pDialogResult != DialogResult.OK)
                return;
            string pPath = openFileDialog1.FileName;
            string pFolder = System.IO.Path.GetDirectoryName(pPath);
            string pFileName = System.IO.Path.GetFileName(pPath);
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pFolder, 0);
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            IFeatureClass pFC = pFeatureWorkspace.OpenFeatureClass(pFileName);
            IFeatureLayer pFLayer = new ESRI.ArcGIS.Carto.FeatureLayer();
            pFLayer.FeatureClass = pFC;
            pFLayer.Name = pFC.AliasName;
            ILayer pLayer = pFLayer as ILayer;
            IMap pMap = axMapControl1.Map;
            pMap.AddLayer(pLayer);
            axMapControl1.ActiveView.Refresh();
            axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmTime p = new frmTime(axMapControl1.Map);
            p.PLayer = axMapControl1.get_Layer(0) as IFeatureLayer;
            p.Show();
        }
        ITimeExtent pLayerTimeExtent = null;

        Int32 pCount = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            IMap pMap = axMapControl1.Map;

            IActiveView pActiveView = pMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;

            ITimeDisplay pTimeDisplay = pScreenDisplay as ITimeDisplay;

            ITime startTime = pLayerTimeExtent.StartTime;

            ITime endTime = (ITime)((IClone)startTime).Clone();

            //每次递进12小时

            ((ITimeOffsetOperator)startTime).AddHours(12.0 * (pCount - 1));
            ((ITimeOffsetOperator)endTime).AddHours(12.0 * pCount);

            ITimeExtent pTimeExt = new TimeExtentClass();

            pTimeExt.SetExtent(startTime, endTime);

            pTimeExt.Empty = false;

            pTimeDisplay.TimeValue = pTimeExt as ITimeValue;

            pActiveView.Refresh();

            pCount += 1;

            //当前浏览时间已到达图层时态数据终止时间时，停止timer控件

            if (endTime.Compare(pLayerTimeExtent.EndTime) == 1)
            {

                timer1.Enabled = false;

                button1.Enabled = true;

            }

        }
    }
}
