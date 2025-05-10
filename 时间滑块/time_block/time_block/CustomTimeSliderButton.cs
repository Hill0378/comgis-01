using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using System.Windows.Forms;
using time_block;

namespace MakeACustomTimeControl2008
{
  public class CustomTimeSliderButton 
  {
    ITimeExtent m_myLayerTimeExtent = null;
    ITimeDuration m_layerInterval = null;
    TimeSliderDialog m_sliderDlg = null;

    public CustomTimeSliderButton()
    {
    }

    public void Time(ILayer layer)//传递图层并验证是否具有时间属性
    {
      IFeatureLayer pFLyr = layer as IFeatureLayer;
      ITimeData pTimeData = pFLyr as ITimeData;
      if (!pTimeData.SupportsTime)
      {
        MessageBox.Show("请确保图层具有时间属性！");
        return;
      }

      pTimeData.UseTime = true;

      //String slocalTimeZoneWId = pTimeZoneFactory.QueryLocalTimeZoneWindowsID();

      //ITimeReference pTimeReference = pTimeZoneFactory.CreateTimeReferenceFromWindowsID(slocalTimeZoneWId);

      if (!pTimeData.SupportsTime) return;

      pTimeData.UseTime = true;

      ITimeTableDefinition pTimeDataDef = pFLyr as ITimeTableDefinition;

      pTimeDataDef.StartTimeFieldName = "Data_Time";

      //pTimeDataDef.TimeReference = pTimeReference;

      //获取图层时态数据时间

      //m_myLayerTimeExtent = pTimeData.GetFullTimeExtent();


      m_myLayerTimeExtent = pTimeData.GetFullTimeExtent();//验证时间字段
      if (m_myLayerTimeExtent==null)
      {
          MessageBox.Show("请确保图层具有正确的时间字段！");
          return;
      }
      ITimeDataDisplay pTimeDataDisplayProperties = pFLyr as ITimeDataDisplay;
      esriTimeUnits LayerIntervalUnits = pTimeDataDisplayProperties.TimeIntervalUnits;
      double LayerInterval = pTimeDataDisplayProperties.TimeInterval;
      ITime startTime = m_myLayerTimeExtent.StartTime;
      string qq=startTime.ToString();
      ITime endTime = (ITime)((IClone)startTime).Clone();
    

      switch (LayerIntervalUnits)//选择时间间隔的单位
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

      ITimeExtent pTimeExt = new TimeExtentClass();
      pTimeExt.SetExtent(startTime, endTime);
      m_layerInterval = pTimeExt.QueryTimeDuration();


      m_sliderDlg = new TimeSliderDialog(this);
      m_sliderDlg.Show();
    }

    public ITimeExtent GetTimeExtent()
    {
      return m_myLayerTimeExtent;
    }

    public void UpdateCurrentTime(double progress)//动态显示
    {
      if (progress <= 0)
        progress = 0.05;
      else if (progress >= 100)
        progress = 0.95;

      //计算图层动态变化的时间
      ITimeDuration offsetToNewCurrentTime = m_myLayerTimeExtent.QueryTimeDuration();
      offsetToNewCurrentTime.Scale(progress);

     
      IMap pMap = Form1.pMainWin.axMapControl1.Map;
      IActiveView pActiveView = pMap as IActiveView;
      IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
      ITimeDisplay pTimeDisplay = pScreenDisplay as ITimeDisplay;

      ITime startTime = m_myLayerTimeExtent.StartTime;
      ITime endTime = (ITime)((IClone)startTime).Clone();
      ((ITimeOffsetOperator)endTime).AddDuration(m_layerInterval);
      ITimeExtent pTimeExt = new TimeExtentClass();
      pTimeExt.SetExtent(startTime, endTime);
      pTimeExt.Empty = false;
      ((ITimeOffsetOperator)pTimeExt).AddDuration(offsetToNewCurrentTime);
      pTimeDisplay.TimeValue = pTimeExt as ITimeValue;
      pActiveView.Refresh();

    }

   
  }

}
