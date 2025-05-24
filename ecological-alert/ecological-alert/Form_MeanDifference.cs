using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;

namespace ecological_alert
{
    public partial class Form_MeanDifference : Form
    {
        private AxMapControl _mapControl;

        public Form_MeanDifference(AxMapControl mapControl)
        {
            InitializeComponent();
            _mapControl = mapControl;
        }

        private void Form_MeanDifference_Load(object sender, EventArgs e)
        {
            // 加载图层
            clbRasterLayers.Items.Clear();
            for (int i = 0; i < _mapControl.LayerCount; i++)
            {
                ILayer layer = _mapControl.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    clbRasterLayers.Items.Add(layer.Name, true);
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = dialog.SelectedPath;
                }
            }
        }
        private IRasterLayer GetRasterLayerByName(AxMapControl mapControl, string layerName)
        {
            for (int i = 0; i < mapControl.LayerCount; i++)
            {
                ILayer layer = mapControl.get_Layer(i);
                if (layer.Name == layerName && layer is IRasterLayer)
                {
                    return layer as IRasterLayer;
                }
            }
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (clbRasterLayers.CheckedItems.Count < 2)
            {
                MessageBox.Show("请至少选择两个图层用于计算平均和差值！");
                return;
            }

            string outFolder = txtOutputPath.Text;
            if (!Directory.Exists(outFolder))
            {
                MessageBox.Show("输出路径无效！");
                return;
            }

            // 1. 读取图层、栅格、年份
            List<IRaster> rasters = new List<IRaster>();
            List<int> years = new List<int>();
            Dictionary<int, string> layerNames = new Dictionary<int, string>();

            Regex yearRegex = new Regex(@"\d{4}");

            foreach (object item in clbRasterLayers.CheckedItems)
            {
                string name = item.ToString();
                Match match = yearRegex.Match(name);
                if (!match.Success)
                {
                    MessageBox.Show($"图层名中未找到年份: {name}");
                    return;
                }
                int year = int.Parse(match.Value);
                IRasterLayer rasterLayer = GetRasterLayerByName(_mapControl, name);
                rasters.Add(rasterLayer.Raster);
                years.Add(year);
                layerNames[year] = name;
            }

            // 2. 获取栅格信息
            IRasterProps templateProps = (IRasterProps)rasters[0];
            int width = templateProps.Width;
            int height = templateProps.Height;
            //float noData = (float)templateProps.NoDataValue;
            float noData = -9999f; // 默认值

            // 3. 读取像素
            List<float[,]> pixelList = new List<float[,]>();
            for (int i = 0; i < rasters.Count; i++)
            {
                float[,] px = SlopeCalculator.ReadRasterPixels(rasters[i], width, height);
                pixelList.Add(px);
            }

            
            float[,] avg = new float[height, width];

            
            float epsilon = 0.00001f;

            // 计算平均值部分
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    float sum = 0;
                    int count = 0;

                    foreach (float[,] px in pixelList)
                    {
                        float val = px[row, col];
                        // 和SlopeCalculator一样处理nodata，去掉IsNoData调用
                        if (Math.Abs(val - noData) > epsilon && !float.IsNaN(val) && !float.IsInfinity(val))
                        {
                            sum += val;
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        float avgVal = sum / count;
                        if (float.IsNaN(avgVal) || float.IsInfinity(avgVal))
                        //if (float.IsNaN(avgVal))
                            avg[row, col] = noData;//100
                        else
                            avg[row, col] = avgVal;
                    }
                    else
                    {
                        avg[row, col] = noData;
                    }
                }
            }

            //计算差值部分
            int maxYear = years.Max();
            int maxIndex = years.IndexOf(maxYear);
            float[,] latest = pixelList[maxIndex];
            float[,] diff = new float[height, width];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    float val = latest[row, col];
                    float avgVal = avg[row, col];

                    bool valIsValid = Math.Abs(val - noData) > epsilon && !float.IsNaN(val) && !float.IsInfinity(val);
                    bool avgIsValid = Math.Abs(avgVal - noData) > epsilon && !float.IsNaN(avgVal) && !float.IsInfinity(avgVal);

                    if (valIsValid && avgIsValid)
                        diff[row, col] = val - avgVal;
                    else
                        diff[row, col] = noData;
                }
            }



            // 6. 保存两个图像
            string avgPath = System.IO.Path.Combine(outFolder, "mean.tif");
            string diffPath = System.IO.Path.Combine(outFolder, $"diff_{maxYear}_mean.tif");
            SlopeCalculator.SaveRaster2(avg, templateProps, avgPath);
            

            SlopeCalculator.SaveRaster2(diff, templateProps, diffPath);
            

            
            AddToMap(avgPath);
            AddToMap(diffPath);
            

            MessageBox.Show("均值和差值图像已生成并添加！");
            this.Close();
        }
        public void AddToMap(string path)
        {
            IRasterLayer rasterLayer = new RasterLayer();
            rasterLayer.CreateFromFilePath(path);
            _mapControl.Map.AddLayer(rasterLayer);
            _mapControl.Refresh();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
