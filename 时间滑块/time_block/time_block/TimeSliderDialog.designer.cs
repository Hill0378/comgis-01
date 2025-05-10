// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at &lt;your ArcGIS install location&gt;/DeveloperKit10.0/userestrictions.txt.
// 

namespace MakeACustomTimeControl2008
{
  partial class TimeSliderDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.m_timeSlider = new System.Windows.Forms.TrackBar();
        this.m_datePicker = new System.Windows.Forms.DateTimePicker();
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.timer1 = new System.Windows.Forms.Timer(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.m_timeSlider)).BeginInit();
        this.SuspendLayout();
        // 
        // m_timeSlider
        // 
        this.m_timeSlider.Location = new System.Drawing.Point(15, 9);
        this.m_timeSlider.Maximum = 100;
        this.m_timeSlider.Name = "m_timeSlider";
        this.m_timeSlider.Size = new System.Drawing.Size(260, 45);
        this.m_timeSlider.TabIndex = 1;
        this.m_timeSlider.ValueChanged += new System.EventHandler(this.TimeSlider_ValueChanged);
        // 
        // m_datePicker
        // 
        this.m_datePicker.CustomFormat = "MM/dd/yyyy HH:mm:ss";
        this.m_datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        this.m_datePicker.Location = new System.Drawing.Point(51, 54);
        this.m_datePicker.Name = "m_datePicker";
        this.m_datePicker.Size = new System.Drawing.Size(200, 21);
        this.m_datePicker.TabIndex = 2;
        this.m_datePicker.ValueChanged += new System.EventHandler(this.DatePicker_ValueChanged);
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(295, 9);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(57, 30);
        this.button1.TabIndex = 3;
        this.button1.Text = "开始";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // button2
        // 
        this.button2.Location = new System.Drawing.Point(295, 51);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(57, 30);
        this.button2.TabIndex = 4;
        this.button2.Text = "停止";
        this.button2.UseVisualStyleBackColor = true;
        this.button2.Click += new System.EventHandler(this.button2_Click);
        // 
        // timer1
        // 
        this.timer1.Interval = 1000;
        this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        // 
        // TimeSliderDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(364, 93);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.m_datePicker);
        this.Controls.Add(this.m_timeSlider);
        this.Name = "TimeSliderDialog";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "时间滑块";
        ((System.ComponentModel.ISupportInitialize)(this.m_timeSlider)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TrackBar m_timeSlider;
    private System.Windows.Forms.DateTimePicker m_datePicker;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Timer timer1;
  }
}