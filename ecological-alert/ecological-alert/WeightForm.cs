using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.IO;

namespace ecological_alert
{
    public partial class WeightForm : Form
    {
        public WeightForm()
        {
            InitializeComponent();
        }

        public IRasterDataset RasterDataset { get; set; }
        public double Weight { get; set; }
        public string OutputPath { get; set; }
        public ESRI.ArcGIS.Controls.AxMapControl axMapControl1;

        private void WeightForm_Load(object sender, EventArgs e)
        {
            // 获取地图中的所有栅格图层
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
            try
            {
                // 验证并获取权重值
                if (!double.TryParse(txtWeight.Text, out double weight))
                {
                    MessageBox.Show("请输入有效的权重值!", "错误");
                    txtWeight.Focus();
                    return;
                }
                Weight = weight;

                // 获取选择的栅格数据
                if (comboBoxRaster.SelectedItem == null)
                {
                    MessageBox.Show("请选择栅格数据!", "错误");
                    return;
                }

                // 从地图中获取选中的栅格图层
                IRasterLayer rasterLayer = GetRasterLayerByName(comboBoxRaster.SelectedItem.ToString());
                if (rasterLayer == null)
                {
                    MessageBox.Show("获取栅格数据失败!", "错误");
                    return;
                }

                // 正确获取栅格数据集的方法
                RasterDataset = GetRasterDatasetFromLayer(rasterLayer);
                if (RasterDataset == null)
                {
                    MessageBox.Show("无法获取栅格数据集!", "错误");
                    return;
                }

                // 验证输出路径
                if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
                {
                    MessageBox.Show("请选择输出路径!", "错误");
                    return;
                }
                OutputPath = txtOutputPath.Text;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作时出错: {ex.Message}", "错误");
            }
        }

        // 获取栅格数据集
        private IRasterDataset GetRasterDatasetFromLayer(IRasterLayer rasterLayer)
        {
            try
            {
                // 直接获取栅格数据集
                IDataset dataset = (IDataset)rasterLayer;
                if (dataset == null) return null;

                // 获取工作空间和数据集名称
                string workspacePath = dataset.Workspace.PathName;
                string datasetName = dataset.Name;

                // 通过工作空间打开数据集
                IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactoryClass();
                IRasterWorkspace rasterWorkspace = (IRasterWorkspace)workspaceFactory.OpenFromFile(
                    Path.GetDirectoryName(workspacePath), 0);
                return rasterWorkspace.OpenRasterDataset(datasetName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取栅格数据集时出错: {ex.Message}");
                return null;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOutputPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "TIFF 文件 (*.tif)|*.tif|IMG 文件 (*.img)|*.img|所有文件 (*.*)|*.*",
                Title = "选择输出栅格位置",
                FileName = "weighted_result.tif"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = saveFileDialog.FileName;
            }
        }

        // 根据名称获取栅格图层
        private IRasterLayer GetRasterLayerByName(string layerName)
        {
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                ILayer layer = axMapControl1.get_Layer(i);
                if (layer.Name == layerName && layer is IRasterLayer)
                {
                    return (IRasterLayer)layer;
                }
            }
            return null;
        }
    }
}