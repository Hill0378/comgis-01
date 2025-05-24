using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MakeACustomTimeControl2008;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;

namespace ecological_alert
{
    public class RasterBandAnimation
    {
        private IRasterLayer _rasterLayer;
        private AxMapControl _mapControl;
        private Form _animationForm;
        private Timer _animationTimer;
        private int _currentBand = 1;
        private bool _isReversed = false;

        // 私有构造函数，只能通过静态方法创建实例
        public RasterBandAnimation(IRasterLayer rasterLayer, AxMapControl mapControl)
        {
            _rasterLayer = rasterLayer;
            _mapControl = mapControl;
        }

        // 静态方法处理整个动画流程
        public static void ShowRasterAnimation(AxMapControl mapControl)
        {
            try
            {
                // 创建并配置打开文件对话框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "栅格数据(*.tif)|*.tif",
                    Title = "打开多波段TIF数据"
                };

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                // 清除现有图层
                mapControl.Map.ClearLayers();

                // 加载TIF文件
                IRasterLayer rasterLayer = RasterLayerHelper.CreateRasterLayerFromFile(openFileDialog.FileName);
                if (rasterLayer == null) return;

                // 添加到地图并刷新
                mapControl.Map.AddLayer(rasterLayer);
                mapControl.ActiveView.Refresh();

                // 创建并显示动画控制
                new RasterBandAnimation(rasterLayer, mapControl).ShowAnimationControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载栅格动画失败: {ex.Message}");
            }
        }

        public void ShowAnimationControl(int maxBands = 11)
        {
            // 检查波段数量
            IRasterBandCollection bandCollection = (IRasterBandCollection)_rasterLayer.Raster;
            int bandCount = Math.Min(bandCollection.Count, maxBands);

            // 创建动画控制窗体
            _animationForm = new Form();
            _animationForm.Text = "多波段动画控制";
            _animationForm.Size = new Size(500, 200);
            _animationForm.StartPosition = FormStartPosition.CenterScreen;
            _animationForm.FormClosed += (s, e) => CleanUp();

            // 初始化UI控件
            InitializeControls(bandCount);

            // 初始显示第一个波段
            ApplyBandRender(1);
            _mapControl.ActiveView.Refresh();

            _animationForm.Show();
        }

        private void InitializeControls(int bandCount)
        {
            // 主布局面板
            TableLayoutPanel mainPanel = new TableLayoutPanel { Dock = DockStyle.Fill };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // 控制按钮
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // 滑块
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // 信息

            // 控制按钮
            Button btnPlay = new Button { Text = "播放", Width = 80 };
            Button btnPause = new Button { Text = "暂停", Width = 80 };
            Button btnStop = new Button { Text = "停止", Width = 80 };
            Button btnReverse = new Button { Text = "反向", Width = 80 };

            // 播放速度控制
            TrackBar speedBar = new TrackBar { Width = 150, Minimum = 100, Maximum = 2000, Value = 500 };
            Label lblSpeedValue = new Label { Text = "500 ms", Width = 60 };

            // 波段滑块
            TrackBar bandSlider = new TrackBar { Dock = DockStyle.Fill, Minimum = 1, Maximum = bandCount, Value = 1 };

            // 波段信息显示
            Label lblBandInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"当前波段: 1/{bandCount}",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft YaHei", 10, FontStyle.Bold),
                ForeColor = Color.Blue
            };

            // 定时器
            _animationTimer = new Timer { Interval = 500 };

            // 按钮面板布局
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            buttonPanel.Controls.AddRange(new Control[] { btnPlay, btnPause, btnStop, btnReverse,
                new Label { Text = "速度:", AutoSize = true }, speedBar, lblSpeedValue });

            // 添加控件到主面板
            mainPanel.Controls.Add(buttonPanel, 0, 0);
            mainPanel.Controls.Add(bandSlider, 0, 1);
            mainPanel.Controls.Add(lblBandInfo, 0, 2);

            _animationForm.Controls.Add(mainPanel);

            // 事件处理
            btnPlay.Click += (s, e) => _animationTimer.Start();
            btnPause.Click += (s, e) => _animationTimer.Stop();

            btnStop.Click += (s, e) =>
            {
                _animationTimer.Stop();
                bandSlider.Value = _isReversed ? bandCount : 1;
            };

            btnReverse.Click += (s, e) =>
            {
                _isReversed = !_isReversed;
                btnReverse.Text = _isReversed ? "正向" : "反向";
            };

            bandSlider.ValueChanged += (s, e) =>
            {
                _currentBand = bandSlider.Value;
                ApplyBandRender(_currentBand);
                lblBandInfo.Text = $"当前波段: {_currentBand}/{bandCount}";
                _mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            };

            speedBar.ValueChanged += (s, e) =>
            {
                lblSpeedValue.Text = $"{speedBar.Value} ms";
                if (_animationTimer.Enabled)
                {
                    _animationTimer.Interval = speedBar.Value;
                }
            };

            _animationTimer.Tick += (s, e) =>
            {
                if (_isReversed)
                {
                    bandSlider.Value = (_currentBand > 1) ? _currentBand - 1 : bandCount;
                }
                else
                {
                    bandSlider.Value = (_currentBand < bandCount) ? _currentBand + 1 : 1;
                }
            };
        }

        private void ApplyBandRender(int bandIndex)
        {
            try
            {
                // 创建拉伸渲染器
                IRasterStretchColorRampRenderer stretchRenderer = new RasterStretchColorRampRendererClass
                {
                    BandIndex = bandIndex - 1  // 转换为0-based索引
                };

                // 设置颜色渐变
                IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRamp
                {
                    FromColor = CreateColor(0, 0, 0),     // 黑色
                    ToColor = CreateColor(255, 255, 255), // 白色
                    Size = 256
                };

                bool rampOK;
                colorRamp.CreateRamp(out rampOK);
                stretchRenderer.ColorRamp = colorRamp;

                // 应用渲染器
                _rasterLayer.Renderer = (IRasterRenderer)stretchRenderer;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置波段{bandIndex}渲染时出错: {ex.Message}");
            }
        }

        private IRgbColor CreateColor(byte red, byte green, byte blue)
        {
            return new RgbColor { Red = red, Green = green, Blue = blue };
        }

        private void CleanUp()
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Dispose();
            }
        }
    }
}