using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.AnalysisTools;

namespace ecological_alert
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axToolbarControl1.SetBuddyControl(axMapControl1);
            axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void 缓冲区分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 确保MapControl有内容
            if (this.axMapControl1.Object == null)
            {
                MessageBox.Show("地图控件未初始化!", "错误");
                return;
            }

            using (BufferForm bufferForm = new BufferForm(this.axMapControl1.Object))
            {
                if (bufferForm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(bufferForm.strOutputPath))
                {
                    try
                    {
                        // 获取输出文件路径
                        string strBufferPath = bufferForm.strOutputPath;

                        // 验证路径格式
                        if (strBufferPath.Contains("\\"))
                        {
                            int index = strBufferPath.LastIndexOf("\\");
                            string directory = strBufferPath.Substring(0, index);
                            string fileName = strBufferPath.Substring(index + 1);

                            // 缓冲区图层载入到MapControl
                            this.axMapControl1.AddShapeFile(directory, fileName);
                        }
                        else
                        {
                            MessageBox.Show("输出路径格式不正确!", "错误");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"加载缓冲区图层时出错: {ex.Message}", "错误");
                    }
                }
            }
        }
    }
}

