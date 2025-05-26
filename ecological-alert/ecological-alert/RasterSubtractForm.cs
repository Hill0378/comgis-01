using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;

namespace ecological_alert
{
    public partial class RasterSubtractForm : Form
    {
        private AxMapControl _mapControl;

        public RasterSubtractForm(AxMapControl mapControl)
        {
            InitializeComponent();
            _mapControl = mapControl;
        }

        private void btnLoadLayers_Click(object sender, EventArgs e)
        {
            clbRasterLayers.Items.Clear();

            IMap map = _mapControl.Map;
            for (int i = 0; i < map.LayerCount; i++)
            {
                ILayer layer = map.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    clbRasterLayers.Items.Add(layer.Name);
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputFolder.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            // 获取用户勾选的图层名（保持用户勾选顺序）
            List<string> selectedNames = new List<string>();
            foreach (var item in clbRasterLayers.CheckedItems)
            {
                selectedNames.Add(item.ToString());
            }

            if (selectedNames.Count < 2)
            {
                MessageBox.Show("请至少勾选两个图层进行相邻作差。");
                return;
            }

            string outputFolder = txtOutputFolder.Text;
            if (string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("请选择输出文件夹。");
                return;
            }

            // 获取用户勾选的 IRasterLayer
            List<IRasterLayer> rasterLayers = new List<IRasterLayer>();
            IMap map = _mapControl.Map;
            foreach (string name in selectedNames)
            {
                for (int i = 0; i < map.LayerCount; i++)
                {
                    ILayer layer = map.get_Layer(i);
                    if (layer is IRasterLayer rasterLayer && layer.Name == name)
                    {
                        rasterLayers.Add(rasterLayer);
                        break;
                    }
                }
            }

            // 检查所有栅格是否有效
            foreach (var layer in rasterLayers)
            {
                if (layer.Raster == null)
                {
                    MessageBox.Show($"无法访问栅格数据: {layer.Name}");
                    return;
                }
            }

            // 开始相邻作差
            for (int i = 1; i < rasterLayers.Count; i++)
            {
                try
                {
                    IRaster raster1 = rasterLayers[i - 1].Raster;
                    IRaster raster2 = rasterLayers[i].Raster;

                    // 构造表达式并执行作差
                    IMapAlgebraOp algebra = new RasterMapAlgebraOpClass();
                    algebra.BindRaster((IGeoDataset)raster1, "r1");
                    algebra.BindRaster((IGeoDataset)raster2, "r2");
                    IRaster result = (IRaster)algebra.Execute("[r1] - [r2]");

                    string name1 = rasterLayers[i - 1].Name;
                    string name2 = rasterLayers[i].Name;
                    string outputFile = Path.Combine(outputFolder, $"{name1}-{name2}.tif");

                    SaveRaster(result, outputFile);

                    // 添加结果图层到地图中
                    IRasterLayer outputLayer = new RasterLayer();
                    outputLayer.CreateFromFilePath(outputFile);
                    _mapControl.Map.AddLayer(outputLayer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"处理 {rasterLayers[i - 1].Name} 和 {rasterLayers[i].Name} 时出错: {ex.Message}");
                    continue;
                }
            }

            _mapControl.Refresh();
            MessageBox.Show("相邻作差完成！");
        }


        private void SaveRaster(IRaster raster, string outputFullPath)
        {
            string folder = Path.GetDirectoryName(outputFullPath);
            string fileName = Path.GetFileName(outputFullPath);

            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            IWorkspace ws = wsFactory.OpenFromFile(folder, 0);
            ISaveAs saveAs = raster as ISaveAs;

            saveAs.SaveAs(fileName, ws, "TIFF");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
