using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.SpatialAnalystTools;
using System.IO;

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

        private void 赋权重ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建赋权重窗体
                WeightForm weightForm = new WeightForm();
                weightForm.axMapControl1 = this.axMapControl1; // 传递MapControl引用

                // 显示对话框并等待用户输入
                if (weightForm.ShowDialog() == DialogResult.OK)
                {
                    // 执行加权计算
                    PerformWeightedCalculation(weightForm.RasterDataset, weightForm.Weight, weightForm.OutputPath);

                    MessageBox.Show("赋权重计算完成!", "成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行赋权重操作时出错: {ex.Message}", "错误");
            }
        }

        private void PerformWeightedCalculation(IRasterDataset rasterDataset, double weight, string outputPath)
        {
            // 创建地理处理器
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;

            // 验证输入数据
            if (rasterDataset == null)
            {
                throw new ArgumentException("栅格数据无效");
            }

            try
            {
                // 执行加权计算
                Times timesTool = new Times();
                timesTool.in_raster_or_constant1 = rasterDataset;
                timesTool.in_raster_or_constant2 = weight;
                timesTool.out_raster = outputPath;

                // 执行计算
                IGeoProcessorResult result = gp.Execute(timesTool, null) as IGeoProcessorResult;

                if (result.Status != esriJobStatus.esriJobSucceeded)
                {
                    throw new Exception("加权计算失败: " + result.GetMessages(-1));
                }

                // 将结果加载到地图中
                ILayer resultLayer = CreateLayerFromRaster(outputPath);
                if (resultLayer != null)
                {
                    axMapControl1.AddLayer(resultLayer);
                    axMapControl1.ActiveView.Refresh();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"执行加权计算时出错: {ex.Message}");
            }
        }

        private ILayer CreateLayerFromRaster(string rasterPath)
        {
            try
            {
                string directory = Path.GetDirectoryName(rasterPath);
                string fileName = Path.GetFileName(rasterPath);

                IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
                IRasterWorkspace rasterWorkspace = (IRasterWorkspace)workspaceFactory.OpenFromFile(directory, 0);
                IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);

                IRasterLayer rasterLayer = new RasterLayerClass();
                rasterLayer.CreateFromDataset(rasterDataset);

                return (ILayer)rasterLayer;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建栅格图层失败: {ex.Message}");
                return null;
            }
        }
    }
}

