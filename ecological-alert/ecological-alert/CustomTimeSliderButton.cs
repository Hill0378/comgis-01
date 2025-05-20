using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace MakeACustomTimeControl2008
{
    public class CustomTimeSliderButton
    {
        private ITimeExtent m_myLayerTimeExtent = null;
        private ITimeDuration m_layerInterval = null;
        private TimeSliderDialog m_sliderDlg = null;
        private AxMapControl m_mapControl = null;
        private bool m_isRasterSeries = false;
        private List<IRasterLayer> m_rasterLayers = new List<IRasterLayer>();
        private Dictionary<int, DateTime> m_yearToDate = new Dictionary<int, DateTime>();
        private int m_currentRasterIndex = 0;

        public CustomTimeSliderButton() { }

        public void Time(ILayer layer, AxMapControl mapControl, string folderPath = null)
        {
            if (mapControl == null)
            {
                MessageBox.Show("MapControl����Ϊ�գ�");
                return;
            }

            m_mapControl = mapControl;

            // ����TIFFʱ�����ж���
            if (!string.IsNullOrEmpty(folderPath))
            {
                InitializeNDVITimeSeries(folderPath);
                return;
            }

            // ����SHPʱ�䶯��
            if (layer != null)
            {
                ProcessFeatureLayer(layer);
            }
        }



        private void InitializeNDVITimeSeries(string folderPath)
        {
            m_isRasterSeries = true;

            // ��ȡ�ļ���������NDVI_YYYY.tif�ļ�
            var ndviFiles = Directory.GetFiles(folderPath, "NDVI_*.tif")
                                     .OrderBy(f => f)
                                     .ToList();

            if (ndviFiles.Count == 0)
            {
                MessageBox.Show("δ�ҵ�NDVI_YYYY.tif�ļ���");
                return;
            }

            // ����ÿ���դ��ͼ�㲢��ȡ���
            foreach (string filePath in ndviFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.StartsWith("NDVI_") && fileName.Length >= 9)
                {
                    string yearStr = fileName.Substring(5, 4);
                    if (int.TryParse(yearStr, out int year))
                    {
                        try
                        {
                            IRasterLayer rasterLayer = new RasterLayer() as IRasterLayer;
                            rasterLayer.CreateFromFilePath(filePath);
                            m_rasterLayers.Add(rasterLayer);
                            m_yearToDate[m_rasterLayers.Count - 1] = new DateTime(year, 1, 1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"�����ļ�{filePath}ʧ��: {ex.Message}");
                        }
                    }
                }
            }

            if (m_rasterLayers.Count == 0)
            {
                MessageBox.Show("δ�ҵ���Ч��NDVI��������ļ���");
                return;
            }

            // ����ʱ�䷶Χ���ӵ�һ�굽���һ�꣩
            ITime startTime = new Time() as ITime;
            startTime.SetFromDateTime(m_yearToDate[0]);

            ITime endTime = new Time() as ITime;
            endTime.SetFromDateTime(m_yearToDate[m_rasterLayers.Count - 1].AddYears(1));

            m_myLayerTimeExtent = new TimeExtent() as ITimeExtent;
            m_myLayerTimeExtent.SetExtent(startTime, endTime);

            // ����ʱ������1�꣩
            ITimeExtent yearExtent = new TimeExtent() as ITimeExtent;
            ITime yearStart = new Time() as ITime;
            yearStart.SetFromDateTime(new DateTime(2000, 1, 1));
            ITime yearEnd = new Time() as ITime;
            yearEnd.SetFromDateTime(new DateTime(2001, 1, 1));
            yearExtent.SetExtent(yearStart, yearEnd);
            m_layerInterval = yearExtent.QueryTimeDuration();

            // Ĭ����ʾ��һ��
            m_mapControl.Map.AddLayer(m_rasterLayers[0]);
            m_mapControl.ActiveView.Refresh();

            m_sliderDlg = new TimeSliderDialog(this);
            m_sliderDlg.Show();
        }

        private void ProcessFeatureLayer(ILayer layer)
        {
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

            if (m_isRasterSeries)
            {
                UpdateRasterTimeSeries(progress);
            }
            else
            {
                UpdateFeatureLayerTime(progress);
            }
        }

        private void UpdateRasterTimeSeries(double progress)
        {
            if (m_rasterLayers.Count == 0)
                return;

            // ���㵱ǰӦ����ʾ���������
            int newIndex = (int)(progress * (m_rasterLayers.Count - 1) / 100.0);
            newIndex = Math.Max(0, Math.Min(m_rasterLayers.Count - 1, newIndex));

            if (newIndex == m_currentRasterIndex)
                return;

            // �Ƴ���ǰͼ��
            IMap map = m_mapControl.Map;
            for (int i = map.LayerCount - 1; i >= 0; i--)
            {
                if (m_rasterLayers.Contains(map.get_Layer(i) as IRasterLayer))
                {
                    map.DeleteLayer(map.get_Layer(i));
                }
            }

            // �����ͼ��
            map.AddLayer(m_rasterLayers[newIndex]);
            m_currentRasterIndex = newIndex;

            // ����ʱ����ʾ
            UpdateTimeDisplay(m_yearToDate[newIndex]);

            // ����״̬����ʾ��ǰ���
            string year = m_yearToDate[newIndex].Year.ToString();
             m_sliderDlg.UpdateStatusLabel($"��ǰ���: {year}");
        }

        private void UpdateFeatureLayerTime(double progress)
        {
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

        private void UpdateTimeDisplay(DateTime date)
        {
            IActiveView activeView = m_mapControl.Map as IActiveView;
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;
            ITimeDisplay timeDisplay = screenDisplay as ITimeDisplay;


            ITimeExtent timeExtent = new TimeExtent() as ITimeExtent;
            ITime time = new Time() as ITime;
            // �޸ĵ��÷���
            time.SetTimeFromDateTime(date);
            timeExtent.SetExtent(time, time);

            // ����ʱ����ʾ
            timeDisplay.TimeValue = timeExtent as ITimeValue;
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }
    }
}