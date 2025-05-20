using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace ecological_alert
{
    public partial class SlopeForm : Form
    {
        private AxMapControl _mapControl;

        public SlopeForm(AxMapControl mapControl)
        {
            InitializeComponent();
            _mapControl = mapControl;
            LoadRasterLayers();
        }

        private void LoadRasterLayers()
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
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "TIFF 文件 (*.tif)|*.tif";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = dlg.FileName;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (clbRasterLayers.CheckedItems.Count < 2)
            {
                MessageBox.Show("请至少选择两个图层");
                return;
            }

            if (string.IsNullOrEmpty(txtOutputPath.Text))
            {
                MessageBox.Show("请选择输出路径");
                return;
            }

            List<IRaster> rasters = new List<IRaster>();
            List<int> years = new List<int>();
            IMap map = _mapControl.Map;

            foreach (string name in clbRasterLayers.CheckedItems)
            {
                for (int i = 0; i < map.LayerCount; i++)
                {
                    if (map.get_Layer(i).Name == name && map.get_Layer(i) is IRasterLayer rasterLayer)
                    {
                        rasters.Add(rasterLayer.Raster);

                        Match match = Regex.Match(name, @"\d{4}");
                        if (match.Success && int.TryParse(match.Value, out int year))
                        {
                            years.Add(year);
                        }
                        else
                        {
                            MessageBox.Show($"图层 {name} 的名称中未包含有效年份（如 NDVI_2015）");
                            return;
                        }

                        break;
                    }
                }
            }

            if (rasters.Count != years.Count)
            {
                MessageBox.Show("图层命名中必须包含年份，例如 NDVI_2015");
                return;
            }

            try
            {
                SlopeCalculator.CalculateSlope(rasters, years, txtOutputPath.Text);
                MessageBox.Show("Slope计算完成！");
                AddToMap(txtOutputPath.Text);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("计算失败：" + ex.Message);
            }
        }

        public void AddToMap(string path)
        {
            IRasterLayer rasterLayer = new RasterLayer();
            rasterLayer.CreateFromFilePath(path);
            _mapControl.Map.AddLayer(rasterLayer);
            _mapControl.Refresh();
        }
    }
}
