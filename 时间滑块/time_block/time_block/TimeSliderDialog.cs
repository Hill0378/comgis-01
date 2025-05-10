

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace MakeACustomTimeControl2008
{
  public partial class TimeSliderDialog : Form
  {
    private CustomTimeSliderButton m_parent = null;

    public TimeSliderDialog(CustomTimeSliderButton parent)
   
    {
      InitializeComponent();
     m_parent = parent;
      ITimeExtent timeExtent = m_parent.GetTimeExtent();
      m_datePicker.MinDate = new DateTime(timeExtent.StartTime.QueryTicks());
      m_datePicker.MaxDate = new DateTime(timeExtent.EndTime.QueryTicks());
      m_datePicker.Value = m_datePicker.MinDate;
    }

    private void TimeSlider_ValueChanged(object sender, EventArgs e)
    {
      m_parent.UpdateCurrentTime(0.01 * (double)(m_timeSlider.Value));//调用UpdateCurrentTime来显示动态效果

    }

    private void DatePicker_ValueChanged(object sender, EventArgs e)
    {
      //long ticks = m_datePicker.Value.Ticks;
      //long minTicks = m_datePicker.MinDate.Ticks;
      //long maxTicks = m_datePicker.MaxDate.Ticks;
      //double progress = ((double)(ticks - minTicks)) / ((double)(maxTicks - minTicks));
      //m_parent.UpdateCurrentTime(progress);

    }

    private void button1_Click(object sender, EventArgs e)//触发timer
    {
        timer1.Enabled = true;
        Steptime = 0;
    }

      private int Steptime =0;//设置时间间隔步长
    private void timer1_Tick(object sender, EventArgs e)
    {


        if (Steptime == 10000)//最大为100000毫秒时结束
        {
            timer1.Enabled = false;
        }
        else
        {
            Steptime = Steptime + 1000;//把时间间隔的累积值换算成timeSlider的value值，触发它的change事件
            m_timeSlider.Value = 100 * Steptime / 10000;
        }
        

    }

    private void button2_Click(object sender, EventArgs e)//停止时间事件
    {
        timer1.Enabled = false;
    }
  }
}
