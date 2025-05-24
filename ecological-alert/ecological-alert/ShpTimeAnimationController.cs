using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace ecological_alert
{
    /// <summary>
    /// SHP文件时间动画控制器（整合加载功能）
    /// </summary>
    public class ShpTimeAnimationController
    {
        private ITimeExtent _layerTimeExtent;
        private int _count = 0;
        private readonly AxMapControl _mapControl;
        private readonly Timer _animationTimer;
        private readonly AxTOCControl _tocControl;

        public ShpTimeAnimationController(AxMapControl mapControl, AxTOCControl tocControl, Timer animationTimer)
        {
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _tocControl = tocControl ?? throw new ArgumentNullException(nameof(tocControl));
            _animationTimer = animationTimer ?? throw new ArgumentNullException(nameof(animationTimer));
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        /// <summary>
        /// 加载Shapefile并初始化动画
        /// </summary>
        public void LoadShapefileAndInitAnimation()
        {
            try
            {
                // 1. 打开文件对话框选择Shapefile
                using (var openFileDialog = new OpenFileDialog
                {
                    Filter = "shapefile文件(*.shp)|*.shp",
                    Title = "打开矢量数据",
                    Multiselect = false
                })
                {
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    // 2. 加载Shapefile到地图
                    string pPath = openFileDialog.FileName;
                    string pFolder = System.IO.Path.GetDirectoryName(pPath);
                    string pFileName = System.IO.Path.GetFileName(pPath);

                    IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
                    IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pFolder, 0);
                    IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                    IFeatureClass pFC = pFeatureWorkspace.OpenFeatureClass(pFileName);

                    IFeatureLayer pFLayer = new FeatureLayer
                    {
                        FeatureClass = pFC,
                        Name = pFC.AliasName
                    };

                    _mapControl.Map.AddLayer(pFLayer as ILayer);
                    _mapControl.ActiveView.Refresh();
                    _tocControl.SetBuddyControl(_mapControl);

                    // 3. 检查图层并显示动画窗体
                    if (_mapControl.Map.LayerCount == 0)
                    {
                        MessageBox.Show("请先加载图层！");
                        return;
                    }

                    // 4. 显示时间动画设置窗体
                    var timeForm = new frmTime(_mapControl.Map);
                    timeForm.PLayer = _mapControl.get_Layer(0) as IFeatureLayer;
                    timeForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void StartAnimation(ITimeExtent timeExtent)
        {
            _layerTimeExtent = timeExtent ?? throw new ArgumentNullException(nameof(timeExtent));
            _count = 1;
            _animationTimer.Enabled = true;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_layerTimeExtent == null) return;

            IMap pMap = _mapControl.Map;
            IActiveView pActiveView = pMap as IActiveView;
            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            ITimeDisplay pTimeDisplay = pScreenDisplay as ITimeDisplay;

            ITime startTime = _layerTimeExtent.StartTime;
            ITime endTime = (ITime)((IClone)startTime).Clone();

            ((ITimeOffsetOperator)startTime).AddHours(12.0 * (_count - 1));
            ((ITimeOffsetOperator)endTime).AddHours(12.0 * _count);

            ITimeExtent pTimeExt = new TimeExtent();
            pTimeExt.SetExtent(startTime, endTime);
            pTimeExt.Empty = false;

            pTimeDisplay.TimeValue = pTimeExt as ITimeValue;
            pActiveView.Refresh();

            _count += 1;

            if (endTime.Compare(_layerTimeExtent.EndTime) == 1)
            {
                _animationTimer.Enabled = false;
            }
        }
    }
}