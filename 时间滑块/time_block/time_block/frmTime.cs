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
    public partial class frmTime : Form
    {

        public frmTime(IMap Map)
        {
            InitializeComponent();
            pMap = Map;
            m_timeSlider.Maximum = 83;
            m_timeSlider.Minimum = 0;
        }

        IMap pMap;
        private IFeatureLayer _pLayer;
        public IFeatureLayer PLayer
        {
            get { return _pLayer; }
            set { _pLayer = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoReplay(_pLayer);
        }
        ITimeExtent pLayerTimeExtent = null;
        Int32 pCount = 0;
        private void DoReplay(IFeatureLayer pFLyr)
        {
            try
            {

                // 使用接口而不是具体类
                ITimeZoneFactory pTimeZoneFactory = (ITimeZoneFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.TimeZoneFactory"));

                ITimeData pTimeData = pFLyr as ITimeData;

                pTimeData.UseTime = true;

                String slocalTimeZoneWId = pTimeZoneFactory.QueryLocalTimeZoneWindowsID();

                ITimeReference pTimeReference = pTimeZoneFactory.CreateTimeReferenceFromWindowsID(slocalTimeZoneWId);

                if (!pTimeData.SupportsTime) return;

                pTimeData.UseTime = true;

                ITimeTableDefinition pTimeDataDef = pFLyr as ITimeTableDefinition;

                pTimeDataDef.StartTimeFieldName = "Date_Time";//设置时间属性为Date_time

                pTimeDataDef.TimeReference = pTimeReference;

                //获取图层时态数据时间

                pLayerTimeExtent = pTimeData.GetFullTimeExtent();

                //激活timer控件

                timer1.Enabled = true;

                pCount = 0;

            }

            catch (Exception Err)
            {

                string msg = Err.Message;

            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Display(pCount);
            pCount += 1;

            //当前浏览时间已到达图层时态数据终止时间时，停止timer控件

            //if (endTime.Compare(pLayerTimeExtent.EndTime) == 1)
            //{

            //    timer1.Enabled = false;

            //    button1.Enabled = true;

            //}
            if (pCount >= 83)
            {

                timer1.Enabled = false;

                button1.Enabled = true;

            }
            else m_timeSlider.Value = pCount;


        }

        private void Display(int count)
        {
            //IMap pMap = axMapControl1.Map;

            IActiveView pActiveView = pMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;

            ITimeDisplay pTimeDisplay = pScreenDisplay as ITimeDisplay;

            ITime startTime = pLayerTimeExtent.StartTime;

            ITime endTime = (ITime)((IClone)startTime).Clone();

            //每次递进24小时

            ((ITimeOffsetOperator)startTime).AddHours(24 * (count - 1));
            ((ITimeOffsetOperator)endTime).AddHours(24 * count);

            ITimeExtent pTimeExt = (ITimeExtent)Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.TimeExtent"));

            pTimeExt.SetExtent(startTime, endTime);

            pTimeExt.Empty = false;

            pTimeDisplay.TimeValue = pTimeExt as ITimeValue;

            pActiveView.Refresh();

        }

        private void m_timeSlider_ValueChanged(object sender, EventArgs e)
        {

            if (pLayerTimeExtent == null)
            {
                DoReplay(_pLayer);
                timer1.Enabled = false;
            }
            else
            {
                pCount = m_timeSlider.Value;
                Display(pCount);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
            }
            else
            {
                timer1.Enabled = true;
            }

        }
    }
}
