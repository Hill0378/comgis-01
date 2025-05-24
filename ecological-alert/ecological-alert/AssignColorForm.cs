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
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ecological_alert;
using ESRI.ArcGIS.Controls;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Display;






namespace ecological_alert
{
    public partial class AssignColorForm : Form
    {

        private AxMapControl _mapControl;

        public AssignColorForm(AxMapControl mapControl)
        {
            InitializeComponent();
            _mapControl = mapControl;
            LoadRasterLayers();
        }

        private void LoadRasterLayers()
        {
            comboRasterLayers.Items.Clear();
            for (int i = 0; i < _mapControl.LayerCount; i++)
            {
                ILayer layer = _mapControl.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    comboRasterLayers.Items.Add(layer.Name);
                }
            }
            if (comboRasterLayers.Items.Count > 0)
                comboRasterLayers.SelectedIndex = 0;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "TIFF (*.tif)|*.tif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = dlg.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboRasterLayers.SelectedItem == null || string.IsNullOrEmpty(txtOutputPath.Text))
            {
                MessageBox.Show("请选择图层并设置输出路径。");
                return;
            }

            string layerName = comboRasterLayers.SelectedItem.ToString();
            IRasterLayer rasterLayer = null;

            for (int i = 0; i < _mapControl.LayerCount; i++)
            {
                ILayer layer = _mapControl.get_Layer(i);
                if (layer.Name == layerName && layer is IRasterLayer)
                {
                    rasterLayer = layer as IRasterLayer;
                    break;
                }
            }

            if (rasterLayer == null)
            {
                MessageBox.Show("未找到图层。");
                return;
            }

            // 获取 IRaster
            IRaster raster = rasterLayer.Raster;
            IRasterProps props = (IRasterProps)raster;
            int width = props.Width;
            int height = props.Height;

            // 读取像素
            float[,] pixels = SlopeCalculator.ReadRasterPixels(raster, width, height);

            // 生成新像素值（此处不改变原值，仅复制）
            float[,] newPixels = (float[,])pixels.Clone();

            // 保存新栅格
            string outputPath = txtOutputPath.Text;
            SlopeCalculator.SaveRaster2(newPixels, props, outputPath);

            // 加入地图并赋颜色
            AddRasterWithColor(outputPath);

            this.Close();
        }

        private void AddRasterWithColor(string rasterPath)
        {
            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            string folder = System.IO.Path.GetDirectoryName(rasterPath);
            string file = System.IO.Path.GetFileName(rasterPath);


            IRasterWorkspace2 rasterWs = wsFactory.OpenFromFile(folder, 0) as IRasterWorkspace2;
            IRasterDataset rasterDataset = rasterWs.OpenRasterDataset(file);
            IRasterLayer rasterLayer = new RasterLayer();
            rasterLayer.CreateFromDataset(rasterDataset);

            // 设置颜色渲染
            ApplyPositiveNegativeColorRampRenderer(rasterLayer);

            _mapControl.AddLayer(rasterLayer);
            _mapControl.Refresh();
        }

        private void ApplyPositiveNegativeColorRampRenderer(IRasterLayer rasterLayer)
        {
            IRaster raster = rasterLayer.Raster;
            IRaster2 raster2 = (IRaster2)raster;
            IRasterDataset rasterDataset = raster2.RasterDataset;

            IRasterProps props = (IRasterProps)raster;
            int width = props.Width;
            int height = props.Height;

            float[,] pixels = SlopeCalculator.ReadRasterPixels(raster, width, height);

            // 过滤 NoData (-9999f)
            float noDataValue = -9999f;

            // 找出负数和非负数的最大最小值
            float minPos = float.MaxValue;   // ≥0最小值
            float maxPos = float.MinValue;   // ≥0最大值
            float minNeg = float.MaxValue;   // 负数最小（绝对值最大）
            float maxNeg = float.MinValue;   // 负数最大（绝对值最小）

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float val = pixels[i, j];
                    if (val == noDataValue || float.IsNaN(val))
                        continue;
                    if (val >= 0)
                    {
                        if (val < minPos) minPos = val;
                        if (val > maxPos) maxPos = val;
                    }
                    else
                    {
                        float absVal = Math.Abs(val);
                        if (absVal < maxNeg) maxNeg = absVal;
                        if (absVal > minNeg) minNeg = absVal;
                    }
                }
            }

            // 计算断点数量（两部分分别3类）
            int classCountPerSide = 3;
            int totalClassCount = classCountPerSide * 2;

            IRasterClassifyColorRampRenderer classifyRenderer = new RasterClassifyColorRampRenderer();
            classifyRenderer.ClassCount = totalClassCount;

            double[] breaks = new double[totalClassCount + 1];

            // 负数部分断点（注意断点要用原始值，负数是小于0）
            // 因为渲染断点按升序，所以负数部分从 -minNeg 到 -maxNeg 逆序
            // 这里用负数值断点区间从 -minNeg (最接近0) 到 -maxNeg (最小值)
            for (int i = 0; i <= classCountPerSide; i++)
            {
                // 负数区间从 -minNeg 到 -maxNeg
                breaks[i] = -minNeg + i * ((-maxNeg + minNeg) / classCountPerSide);
            }

            // >=0部分断点，从 minPos 到 maxPos
            for (int i = 0; i <= classCountPerSide; i++)
            {
                breaks[classCountPerSide + i] = minPos + i * ((maxPos - minPos) / classCountPerSide);
            }

            // 设置断点，断点数 = ClassCount
            for (int i = 0; i < totalClassCount; i++)
            {
                classifyRenderer.set_Break(i, breaks[i + 1]);
            }

            // 颜色渐变设置
            // 负数用黄->橙->红（绝对值小黄，大红）
            IColor[] negColors = new IColor[]
            {
        ColorToRgbColor(Color.Yellow),
        ColorToRgbColor(Color.Orange),
        ColorToRgbColor(Color.Red)
            };

            // 非负数用浅绿->深绿
            IColor[] posColors = new IColor[]
            {
        ColorToRgbColor(Color.LightGreen),
        ColorToRgbColor(Color.Green),
        ColorToRgbColor(Color.DarkGreen)
            };

            // 设置符号和标签
            for (int i = 0; i < classCountPerSide; i++)
            {
                classifyRenderer.set_Label(i, $"< 0 Class {i + 1}");
                classifyRenderer.set_Symbol(i, CreateFillSymbol(negColors[i]));
            }
            for (int i = 0; i < classCountPerSide; i++)
            {
                classifyRenderer.set_Label(classCountPerSide + i, $">= 0 Class {i + 1}");
                classifyRenderer.set_Symbol(classCountPerSide + i, CreateFillSymbol(posColors[i]));
            }

            // 绑定 Raster，调用 Update()
            IRasterRenderer rasterRenderer = (IRasterRenderer)classifyRenderer;
            rasterRenderer.Raster = raster;
            rasterRenderer.Update();

            rasterLayer.Renderer = rasterRenderer;
        }





        private IRgbColor ColorToRgbColor(System.Drawing.Color color)
        {
            IRgbColor rgbColor = new RgbColor();
            rgbColor.Red = color.R;
            rgbColor.Green = color.G;
            rgbColor.Blue = color.B;
            return rgbColor;
        }

        private ISymbol CreateFillSymbol(IColor color)
        {
            ISimpleFillSymbol fillSymbol = new SimpleFillSymbol();
            fillSymbol.Color = color;
            return (ISymbol)fillSymbol;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
