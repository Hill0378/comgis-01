using System;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
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

            IRaster raster = rasterLayer.Raster;
            IRasterProps props = (IRasterProps)raster;
            int width = props.Width;
            int height = props.Height;

            // 读取像素（请确保 SlopeCalculator.ReadRasterPixels 已实现）
            float[,] pixels = SlopeCalculator.ReadRasterPixels(raster, width, height);

            // 复制像素数组
            float[,] newPixels = (float[,])pixels.Clone();

            // 保存新栅格（请确保 SlopeCalculator.SaveRaster2 已实现）
            string outputPath = txtOutputPath.Text;
            SlopeCalculator.SaveRaster2(newPixels, props, outputPath);

            // 加载结果并赋颜色
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

            // 应用颜色渲染
            ApplyPositiveNegativeColorRampRenderer(rasterLayer);

            _mapControl.AddLayer(rasterLayer);
            _mapControl.Refresh();
        }

        private void ApplyPositiveNegativeColorRampRenderer(IRasterLayer rasterLayer)
        {
            IRaster raster = rasterLayer.Raster;
            IRaster2 raster2 = (IRaster2)raster;
            IRasterProps props = (IRasterProps)raster;
            int width = props.Width;
            int height = props.Height;

            float[,] pixels = SlopeCalculator.ReadRasterPixels(raster, width, height);

            float noData = -9999f;
            float minPos = float.MaxValue, maxPos = float.MinValue;
            float minNeg = float.MaxValue, maxNeg = float.MinValue;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float val = pixels[i, j];
                    if (val == noData || float.IsNaN(val)) continue;

                    if (val >= 0)
                    {
                        if (val < minPos) minPos = val;
                        if (val > maxPos) maxPos = val;
                    }
                    else
                    {
                        float absVal = Math.Abs(val);
                        if (absVal < minNeg) minNeg = absVal;
                        if (absVal > maxNeg) maxNeg = absVal;
                    }
                }
            }

            // 防止无数据
            if (minPos == float.MaxValue) { minPos = 0; maxPos = 1; }
            if (minNeg == float.MaxValue) { minNeg = 0; maxNeg = 1; }

            int classCountPerSide = 10;
            int totalClassCount = classCountPerSide * 2;

            IRasterClassifyColorRampRenderer renderer = new RasterClassifyColorRampRendererClass();
            renderer.ClassCount = totalClassCount;

            double[] breaks = new double[totalClassCount + 1];

            // 负值断点（从 -maxNeg 到 -minNeg）
            for (int i = 0; i <= classCountPerSide; i++)
            {
                breaks[i] = -maxNeg + i * ((maxNeg - minNeg) / classCountPerSide);
            }
            // 正值断点（从 minPos 到 maxPos）
            for (int i = 0; i <= classCountPerSide; i++)
            {
                breaks[classCountPerSide + i] = minPos + i * ((maxPos - minPos) / classCountPerSide);
            }

            // 设置断点
            for (int i = 0; i < totalClassCount; i++)
            {
                renderer.set_Break(i, breaks[i + 1]);
            }

            // HSV 渐变色带
            IColor[] negColors = GenerateHSVColors(classCountPerSide, 0, 60);     // 红到黄
            IColor[] posColors = GenerateHSVColors(classCountPerSide, 90, 180); // 绿到深绿

            // 设置符号
            for (int i = 0; i < classCountPerSide; i++)
            {
                renderer.set_Label(i, $"< 0 Class {i + 1}");
                renderer.set_Symbol(i, CreateFillSymbol(negColors[i]));
            }
            for (int i = 0; i < classCountPerSide; i++)
            {
                renderer.set_Label(classCountPerSide + i, $">= 0 Class {i + 1}");
                renderer.set_Symbol(classCountPerSide + i, CreateFillSymbol(posColors[i]));
            }

            IRasterRenderer rasterRenderer = (IRasterRenderer)renderer;
            rasterRenderer.Raster = raster;
            rasterRenderer.Update();
            rasterLayer.Renderer = rasterRenderer;
        }

        private IColor[] GenerateHSVColors(int count, float startHue, float endHue)
        {
            IColor[] colors = new IColor[count];
            float hueStep = (endHue - startHue) / Math.Max(count - 1, 1);

            for (int i = 0; i < count; i++)
            {
                float hue = startHue + i * hueStep;
                Color color = ColorFromHSV(hue, 1.0f, 1.0f); // 饱和度和亮度为 1.0
                colors[i] = ColorToRgbColor(color);
            }

            return colors;
        }

        private Color ColorFromHSV(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return Color.FromArgb(255, v, t, p);
                case 1: return Color.FromArgb(255, q, v, p);
                case 2: return Color.FromArgb(255, p, v, t);
                case 3: return Color.FromArgb(255, p, q, v);
                case 4: return Color.FromArgb(255, t, p, v);
                default: return Color.FromArgb(255, v, p, q);
            }
        }


        private IRgbColor ColorToRgbColor(Color color)
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
