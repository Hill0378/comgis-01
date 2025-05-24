using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.SpatialAnalystTools;
using System.IO;

namespace ecological_alert
{
    public partial class WeightForm : Form
    {
        private ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        public double Weight { get; set; }
        public string OutputPath { get; set; }

        public WeightForm(ESRI.ArcGIS.Controls.AxMapControl mapControl)
        {
            InitializeComponent();
            this.axMapControl1 = mapControl;
            this.Load += WeightForm_Load;
        }

        private void WeightForm_Load(object sender, EventArgs e)
        {
            if (axMapControl1 == null || axMapControl1.LayerCount == 0)
            {
                MessageBox.Show("地图控件未初始化或没有可用图层!", "错误");
                return;
            }

            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                ILayer layer = axMapControl1.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    comboBoxRaster.Items.Add(layer.Name);
                }
            }

            if (comboBoxRaster.Items.Count > 0)
            {
                comboBoxRaster.SelectedIndex = 0;
            }
        }

        private void btnWeight_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtWeight.Text, out double weight))
            {
                MessageBox.Show("请输入有效的权重值!", "错误");
                return;
            }
            Weight = weight;

            if (comboBoxRaster.SelectedItem == null)
            {
                MessageBox.Show("请选择栅格数据!", "错误");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                MessageBox.Show("请选择输出路径!", "错误");
                return;
            }

            try
            {
                // 获取选中的栅格图层
                IRasterLayer rasterLayer = axMapControl1.get_Layer(comboBoxRaster.SelectedIndex) as IRasterLayer;
                if (rasterLayer == null)
                {
                    MessageBox.Show("选中的图层不是有效的栅格图层!", "错误");
                    return;
                }

                // 执行加权计算
                Geoprocessor gp = new Geoprocessor { OverwriteOutput = true };
                Times timesTool = new Times
                {
                    in_raster_or_constant1 = rasterLayer.Raster,
                    in_raster_or_constant2 = Weight,
                    out_raster = txtOutputPath.Text
                };

                // 执行计算
                gp.Execute(timesTool, null);

                // 将结果添加到地图
                AddResultToMap(txtOutputPath.Text);

                MessageBox.Show($"赋权重操作完成!\n权重值: {Weight}\n输出路径: {txtOutputPath.Text}",
                             "操作完成",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作时出错: {ex.Message}", "错误");
            }
        }

        private void AddResultToMap(string rasterPath)
        {
            try
            {
                string directory = Path.GetDirectoryName(rasterPath);
                string fileName = Path.GetFileName(rasterPath);

                // 创建工作空间并打开栅格数据集
                IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
                IRasterWorkspace rasterWorkspace = workspaceFactory.OpenFromFile(directory, 0) as IRasterWorkspace;
                IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);

                // 创建栅格图层
                IRasterLayer rasterLayer = new RasterLayerClass();
                rasterLayer.CreateFromDataset(rasterDataset);
                rasterLayer.Name = $"加权结果_{Path.GetFileNameWithoutExtension(fileName)}";

                // 添加到地图并刷新
                axMapControl1.AddLayer(rasterLayer as ILayer);
                axMapControl1.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加结果到地图时出错: {ex.Message}", "警告");
            }
        }

        private void btnOutputPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "TIFF 文件 (*.tif)|*.tif|IMG 文件 (*.img)|*.img",
                Title = "选择输出栅格位置",
                FileName = "weighted_result.tif"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = saveFileDialog.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}