using System;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;

namespace ecological_alert
{
    public partial class BufferForm : Form
    {
        private ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        public string strOutputPath;

        public BufferForm(ESRI.ArcGIS.Controls.AxMapControl mapControl)
        {
            InitializeComponent();
            this.axMapControl1 = mapControl;
            this.Load += BufferForm_Load;
        }

        private void BufferForm_Load(object sender, EventArgs e)
        {
            if (axMapControl1.LayerCount == 0)
            {
                MessageBox.Show("地图中没有可用的图层!", "错误");
                this.Close();
                return;
            }

            for (int i = 0; i < this.axMapControl1.LayerCount; i++)
            {
                ILayer pLayer = this.axMapControl1.get_Layer(i);
                cboLayers.Items.Add(pLayer.Name);
            }

            if (cboLayers.Items.Count > 0)
                cboLayers.SelectedIndex = 0;
        }

        private void btnOutputLayer_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                Filter = "Shapefile (*.shp)|*.shp",
                Title = "选择缓冲区输出位置"
            };

            if (cboLayers.SelectedItem != null)
            {
                saveDlg.FileName = $"{cboLayers.SelectedItem}_buffer.shp";
            }

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = saveDlg.FileName;
            }
        }

        private void btnBuffer_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtBufferDistance.Text, out double bufferDistance) || bufferDistance <= 0)
            {
                MessageBox.Show("请输入有效的缓冲半径(必须为正数)!", "输入错误");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                MessageBox.Show("请先选择输出路径!", "路径错误");
                return;
            }

            if (cboLayers.SelectedItem == null)
            {
                MessageBox.Show("请选择要分析的图层!", "选择错误");
                return;
            }

            try
            {
                // 创建地理处理器
                Geoprocessor gp = new Geoprocessor { OverwriteOutput = true };

                // 获取选中的要素图层
                IFeatureLayer pFeatureLayer = axMapControl1.get_Layer(cboLayers.SelectedIndex) as IFeatureLayer;

                // 创建缓冲区工具
                var buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(pFeatureLayer, txtOutputPath.Text, bufferDistance.ToString());

                // 执行缓冲区分析
                IGeoProcessorResult results = (IGeoProcessorResult)gp.Execute(buffer, null);

                if (results.Status == esriJobStatus.esriJobSucceeded)
                {
                    this.strOutputPath = txtOutputPath.Text;

                    // 将生成的缓冲区图层添加到地图中
                    AddBufferLayerToMap(txtOutputPath.Text);

                    MessageBox.Show($"缓冲区生成完成！\n缓冲半径: {bufferDistance}\n输出路径: {txtOutputPath.Text}",
                                 "操作成功",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"缓冲区生成失败! 错误信息: {results.GetMessages(-1)}", "错误");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行缓冲区分析时出错: {ex.Message}", "错误");
            }
        }

        // 将缓冲区图层添加到地图中
        private void AddBufferLayerToMap(string shapefilePath)
        {
            try
            {
                string directory = Path.GetDirectoryName(shapefilePath);
                string fileName = Path.GetFileNameWithoutExtension(shapefilePath);

                // 修改为使用接口而不是具体类
                IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory() as IWorkspaceFactory;
                IFeatureWorkspace featureWorkspace = workspaceFactory.OpenFromFile(directory, 0) as IFeatureWorkspace;

                // 打开要素类
                IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(fileName);

                // 创建要素图层
                IFeatureLayer featureLayer = new FeatureLayerClass();
                featureLayer.FeatureClass = featureClass;
                featureLayer.Name = fileName + "_缓冲区";

                // 添加到地图并刷新
                axMapControl1.AddLayer(featureLayer);
                axMapControl1.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加缓冲区图层到地图时出错: {ex.Message}", "警告");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}