using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.DataManagementTools;
using System.Text;

namespace ecological_alert
{
    public partial class MosaicDialog : Form
    {
        private AxMapControl _mapControl;
        public List<string> SelectedRasters { get; private set; }
        public string OutputPath { get; private set; }
        public string MosaicMethod { get; private set; }

        public MosaicDialog(AxMapControl axMapControl1)
        {
            InitializeComponent();
            _mapControl = axMapControl1;
            LoadRasterLayers();
            InitializeUI();
        }

        private void InitializeUI()
        {
            cboMethod.Items.AddRange(new string[] {
                "LAST - 后值覆盖前值",
                "FIRST - 前值优先",
                "BLEND - 混合",
                "MEAN - 平均值",
                "MINIMUM - 最小值",
                "MAXIMUM - 最大值"
            });
            cboMethod.SelectedIndex = 0;
        }

        private void LoadRasterLayers()
        {
            clbRasterLayers.Items.Clear();
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    clbRasterLayers.Items.Add(layer.Name, false);
                }
            }
        }

        public void ExecuteMosaic(List<string> inputRasters, string outputPath, string mosaicMethod)
        {
            try
            {
                Geoprocessor gp = new Geoprocessor { OverwriteOutput = true };
                MosaicToNewRaster mosaicTool = new MosaicToNewRaster();
                gp.SetEnvironmentValue("NoData", "-9999");


                // 参数配置
                mosaicTool.input_rasters = string.Join(";", inputRasters);
                mosaicTool.output_location = Path.GetDirectoryName(outputPath);
                mosaicTool.raster_dataset_name_with_extension = Path.GetFileName(outputPath);
                mosaicTool.pixel_type = "32_BIT_FLOAT";
                mosaicTool.number_of_bands = 1;
                mosaicTool.mosaic_method = mosaicMethod;


                gp.Execute(mosaicTool, null);

                if (gp.MessageCount > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < gp.MessageCount; i++)
                    {
                        sb.AppendLine(gp.GetMessage(i)); // 显式传递索引参数
                    }
                    string messages = sb.ToString();
                    MessageBox.Show(messages.Contains("ERROR") ?
                        $"错误:\n{messages}" : "镶嵌成功！",
                        messages.Contains("ERROR") ? "错误" : "成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行出错: {ex.Message}", "错误");
            }
        }



        public void AddResultLayerToMap(string layerPath)
        {
            try
            {
                if (File.Exists(layerPath))
                {
                    IRasterLayer rasterLayer = new RasterLayerClass();
                    rasterLayer.CreateFromFilePath(layerPath);
                    _mapControl.AddLayer(rasterLayer);
                    _mapControl.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加图层失败: {ex.Message}");
            }
        }

        private void Btbrowse_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "TIFF文件|*.tif",
                Title = "选择输出位置"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    outputTextBox.Text = dlg.FileName;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<IRaster> rasters = new List<IRaster>();
            List<string> inputPaths = new List<string>();
            IMap map = _mapControl.Map;

            // 输入验证
            if (clbRasterLayers.CheckedItems.Count < 2)
            {
                MessageBox.Show("请至少选择两个栅格图层", "输入错误");
                return;
            }

            if (string.IsNullOrWhiteSpace(outputTextBox.Text))
            {
                MessageBox.Show("请指定输出路径", "输入错误");
                return;
            }

            if (cboMethod.SelectedIndex == -1)
            {
                MessageBox.Show("请选择镶嵌方法", "输入错误");
                return;
            }

            foreach (string name in clbRasterLayers.CheckedItems)
            {
                IRasterLayer rasterLayer = FindRasterLayer(name);
                if (rasterLayer != null)
                {
                    // 直接获取文件路径（适用于文件型栅格）
                    string rasterPath = rasterLayer.FilePath;

                    // 如果FilePath为空，尝试从RasterDataset获取
                    if (string.IsNullOrEmpty(rasterPath))
                    {
                        IRasterDataset rasterDataset = (rasterLayer.Raster as IRasterDataset);
                        if (rasterDataset != null)
                            rasterPath = rasterDataset.CompleteName;
                    }

                    if (!string.IsNullOrEmpty(rasterPath))
                    {
                        Console.WriteLine($"栅格路径: {rasterPath}");
                        inputPaths.Add(rasterPath);
                    }
                    else
                        MessageBox.Show($"无法获取图层'{name}'的路径");
                }
            }

            string outputDir = Path.GetDirectoryName(OutputPath);
            string outputFileName = Path.GetFileName(OutputPath);
            Console.WriteLine($"输出目录: {outputDir}, 输出文件名: {outputFileName}"); // 添加调试输出

            // 执行镶嵌操作
            MosaicMethod = cboMethod.SelectedItem.ToString().Split(' ')[0];
            OutputPath = outputTextBox.Text;
            ExecuteMosaic(inputPaths, OutputPath, MosaicMethod);
            AddResultLayerToMap(OutputPath);
            DialogResult = DialogResult.OK;
            Close();
        }

        private IRasterLayer FindRasterLayer(string layerName)
        {
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer.Name == layerName && layer is IRasterLayer rasterLayer)
                {
                    return rasterLayer;
                }
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}