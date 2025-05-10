

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
      m_parent.UpdateCurrentTime(0.01 * (double)(m_timeSlider.Value));//����UpdateCurrentTime����ʾ��̬Ч��

    }

    private void DatePicker_ValueChanged(object sender, EventArgs e)
    {
      //long ticks = m_datePicker.Value.Ticks;
      //long minTicks = m_datePicker.MinDate.Ticks;
      //long maxTicks = m_datePicker.MaxDate.Ticks;
      //double progress = ((double)(ticks - minTicks)) / ((double)(maxTicks - minTicks));
      //m_parent.UpdateCurrentTime(progress);

    }

    private void button1_Click(object sender, EventArgs e)//����timer
    {
        timer1.Enabled = true;
        Steptime = 0;
    }

      private int Steptime =0;//����ʱ��������
    private void timer1_Tick(object sender, EventArgs e)
    {


        if (Steptime == 10000)//���Ϊ100000����ʱ����
        {
            timer1.Enabled = false;
        }
        else
        {
            Steptime = Steptime + 1000;//��ʱ�������ۻ�ֵ�����timeSlider��valueֵ����������change�¼�
            m_timeSlider.Value = 100 * Steptime / 10000;
        }
        

    }

    private void button2_Click(object sender, EventArgs e)//ֹͣʱ���¼�
    {
        timer1.Enabled = false;
    }
  }
}
