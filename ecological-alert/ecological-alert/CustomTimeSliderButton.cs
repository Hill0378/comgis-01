using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using System.Linq;

namespace MakeACustomTimeControl2008
{
    public class CustomTimeSliderButton
    {
        private ITimeExtent m_myLayerTimeExtent = null;
        private ITimeDuration m_layerInterval = null;
        private TimeSliderDialog m_sliderDlg = null;
        private AxMapControl m_mapControl = null;
        private int m_startYear = 2014;
        private int m_endYear = 2024;
        private int m_currentYear = 2014;
        private IRasterLayer m_rasterLayer = null;
        private Timer m_animationTimer = null;
        private bool m_isPlaying = false;
        private List<IRasterLayer> m_rasterLayers = new List<IRasterLayer>(); // �����������

        public CustomTimeSliderButton()
        { }

        // �޸ĺ�ķ���������MapControl����
        public void Time(ILayer layer, AxMapControl mapControl)
        {
            if (mapControl == null)
            {
                MessageBox.Show("MapControl����Ϊ�գ�");
                return;
            }

            m_mapControl = mapControl;



            IFeatureLayer pFLyr = layer as IFeatureLayer;
            if (pFLyr == null)
            {
                MessageBox.Show("��Ч��Ҫ��ͼ�㣡");
                return;
            }

            ITimeData pTimeData = pFLyr as ITimeData;
            if (!pTimeData.SupportsTime)
            {
                MessageBox.Show("��ȷ��ͼ�����ʱ�����ԣ�");
                return;
            }

            pTimeData.UseTime = true;
            ITimeTableDefinition pTimeDataDef = pFLyr as ITimeTableDefinition;
            pTimeDataDef.StartTimeFieldName = "Data_Time";

            m_myLayerTimeExtent = pTimeData.GetFullTimeExtent();
            if (m_myLayerTimeExtent == null)
            {
                MessageBox.Show("��ȷ��ͼ�������ȷ��ʱ���ֶΣ�");
                return;
            }

            ITimeDataDisplay pTimeDataDisplayProperties = pFLyr as ITimeDataDisplay;
            esriTimeUnits LayerIntervalUnits = pTimeDataDisplayProperties.TimeIntervalUnits;
            double LayerInterval = pTimeDataDisplayProperties.TimeInterval;
            ITime startTime = m_myLayerTimeExtent.StartTime;
            ITime endTime = (ITime)((IClone)startTime).Clone();

            // ����ʱ�䵥λ���ʱ����
            switch (LayerIntervalUnits)
            {
                case esriTimeUnits.esriTimeUnitsYears:
                    ((ITimeOffsetOperator)endTime).AddYears(LayerInterval, false, true);
                    break;
                case esriTimeUnits.esriTimeUnitsMonths:
                    ((ITimeOffsetOperator)endTime).AddMonths(LayerInterval, false, true);
                    break;
                case esriTimeUnits.esriTimeUnitsDays:
                    ((ITimeOffsetOperator)endTime).AddDays(LayerInterval);
                    break;
                case esriTimeUnits.esriTimeUnitsHours:
                    ((ITimeOffsetOperator)endTime).AddHours(LayerInterval);
                    break;
                case esriTimeUnits.esriTimeUnitsMinutes:
                    ((ITimeOffsetOperator)endTime).AddMinutes(LayerInterval);
                    break;
                case esriTimeUnits.esriTimeUnitsSeconds:
                    ((ITimeOffsetOperator)endTime).AddSeconds(LayerInterval);
                    break;
            }

            ITimeExtent pTimeExt = new TimeExtent() as ITimeExtent;
            pTimeExt.SetExtent(startTime, endTime);
            m_layerInterval = pTimeExt.QueryTimeDuration();

            m_sliderDlg = new TimeSliderDialog(this);
            m_sliderDlg.Show();
        }

        public ITimeExtent GetTimeExtent()
        {
            return m_myLayerTimeExtent;
        }

        public void UpdateCurrentTime(double progress)
        {
            if (progress <= 0)
                progress = 0.05;
            else if (progress >= 100)
                progress = 0.95;

            if (m_myLayerTimeExtent == null || m_mapControl == null)
                return;

            // ����ͼ�㶯̬�仯��ʱ��
            ITimeDuration offsetToNewCurrentTime = m_myLayerTimeExtent.QueryTimeDuration();
            offsetToNewCurrentTime.Scale(progress);

            IMap pMap = m_mapControl.Map;
            IActiveView pActiveView = pMap as IActiveView;
            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            ITimeDisplay pTimeDisplay = pScreenDisplay as ITimeDisplay;

            ITime startTime = m_myLayerTimeExtent.StartTime;
            ITime endTime = (ITime)((IClone)startTime).Clone();
            ((ITimeOffsetOperator)endTime).AddDuration(m_layerInterval);

            ITimeExtent pTimeExt = new TimeExtent() as ITimeExtent;
            pTimeExt.SetExtent(startTime, endTime);
            pTimeExt.Empty = false;
            ((ITimeOffsetOperator)pTimeExt).AddDuration(offsetToNewCurrentTime);

            pTimeDisplay.TimeValue = pTimeExt as ITimeValue;
            pActiveView.Refresh();
        }


        private void HandleRasterTimeAnimation(IRasterLayer rasterLayer)
        {
            // ����ͼ�����ư��������Ϣ
            string layerName = rasterLayer.Name;
            if (layerName.Contains("2014") || layerName.Contains("2024"))
            {
                // �ռ�������ݵ�դ��ͼ��
                m_rasterLayers.Clear();
                for (int i = 0; i < m_mapControl.Map.LayerCount; i++)
                {
                    IRasterLayer rl = m_mapControl.Map.get_Layer(i) as IRasterLayer;
                    if (rl != null && rl.Name.Contains("NDVI"))
                    {
                        m_rasterLayers.Add(rl);
                        rl.Visible = false; // ��ʼȫ������
                    }
                }

                // ���������
                m_rasterLayers = m_rasterLayers.OrderBy(r => r.Name).ToList();

                if (m_rasterLayers.Count > 0)
                {
                    // ��ʾ��һ��ͼ��
                    m_rasterLayers[0].Visible = true;
                    m_mapControl.ActiveView.Refresh();

                    // ��������ʾ�������ƴ���
                    m_sliderDlg = new TimeSliderDialog(this);
                    m_sliderDlg.Show();
                }
            }
            else
            {
                MessageBox.Show("��ȷ��դ��ͼ�����ư��������Ϣ(��2014-2024)��");
            }
        }


        public void UpdateRasterFrame(int frameIndex)
        {
            if (frameIndex >= 0 && frameIndex < m_rasterLayers.Count)
            {
                // ��������դ��ͼ��
                foreach (var layer in m_rasterLayers)
                {
                    layer.Visible = false;
                }

                // ��ʾ��ǰ֡��ͼ��
                m_rasterLayers[frameIndex].Visible = true;
                m_mapControl.ActiveView.Refresh();
            }
        }


    }
}