using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using ESRI.ArcGIS.SpatialAnalyst;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.GeoAnalyst;

namespace ecological_alert
{
    public partial class Mainform : Form
    {
      

        public Mainform()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axToolbarControl1.SetBuddyControl(axMapControl1);
            axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void 按掩膜提取ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // 获取地图中所有栅格图层
            List<IRasterLayer> rasterLayers = GetRasterLayersFromMap(axMapControl1.Map);
            if (rasterLayers.Count == 0)
            {
                MessageBox.Show("地图中未找到栅格图层", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 获取地图中多边形矢量图层
            IFeatureLayer vectorLayer = GetPolygonVectorLayerFromMap(axMapControl1.Map);
            if (vectorLayer == null)
            {
                MessageBox.Show("地图中未找到多边形矢量图层", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 获取矢量图层形状
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = "";
            IFeatureCursor featureCursor = null;
            try
            {
                featureCursor = vectorLayer.FeatureClass.Search(queryFilter, true);
                IFeature feature = featureCursor.NextFeature();
                if (feature == null)
                {
                    MessageBox.Show("矢量图层中无有效要素", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                IGeometry pGeometry = feature.Shape;
                IPolygon pPolygon = pGeometry as IPolygon;
                if (pPolygon == null)
                {
                    MessageBox.Show("要素形状无法转换为多边形", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 对每个栅格图层进行掩膜提取
                foreach (IRasterLayer rasterLayer in rasterLayers)
                {
                    IRaster raster = rasterLayer.Raster;
                    ClipRaster(raster, pPolygon, rasterLayer.Name + "_掩膜提取");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取矢量图层要素时出错: {ex.Message}", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (featureCursor != null)
                {
                    // 释放COM对象
                    Marshal.ReleaseComObject(featureCursor);
                }
            }
        }

        // 获取地图中所有栅格图层
        private List<IRasterLayer> GetRasterLayersFromMap(IMap map)
        {
            List<IRasterLayer> rasterLayers = new List<IRasterLayer>();
            for (int i = 0; i < map.LayerCount; i++)
            {
                ILayer layer = map.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    rasterLayers.Add(layer as IRasterLayer);
                }
            }
            return rasterLayers;
        }

        // 获取地图中多边形矢量图层
        private IFeatureLayer GetPolygonVectorLayerFromMap(IMap map)
        {
            for (int i = 0; i < map.LayerCount; i++)
            {
                ILayer layer = map.get_Layer(i);
                if (layer is IFeatureLayer)
                {
                    IFeatureClass featureClass = (layer as IFeatureLayer).FeatureClass;
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        return layer as IFeatureLayer;
                    }
                }
            }
            return null;
        }

        // 执行栅格掩膜提取
        public static void ClipRaster(IRaster pRaster, IPolygon clipGeo, string saveFileName)
        {
            if (pRaster == null || clipGeo == null)
            {
                MessageBox.Show("输入的栅格或多边形为空，无法裁剪", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 设置环境参数
                var pProps = pRaster as IRasterProps;
                if (pProps == null)
                {
                    MessageBox.Show("无法获取栅格属性", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 设置处理环境
                object cellSizeProvider = pProps.MeanCellSize().X;
                var pInputDataset = pRaster as IGeoDataset;
                IExtractionOp pExtractionOp = new RasterExtractionOpClass();
                var pRasterAnaEnvir = pExtractionOp as IRasterAnalysisEnvironment;

                // 设置单元格大小和范围
                pRasterAnaEnvir.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ref cellSizeProvider);
                object extentProvider = clipGeo.Envelope;
                object snapRasterData = Type.Missing;
                pRasterAnaEnvir.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, ref extentProvider, ref snapRasterData);

                // 执行掩膜提取
                var pOutputDataset = pExtractionOp.Polygon(pInputDataset, clipGeo, true);

                // 处理输出结果
                IRaster clipRaster = null;
                if (pOutputDataset is IRasterLayer)
                {
                    clipRaster = (pOutputDataset as IRasterLayer).Raster;
                }
                else if (pOutputDataset is IRasterDataset)
                {
                    clipRaster = (pOutputDataset as IRasterDataset).CreateDefaultRaster();
                }
                else if (pOutputDataset is IRaster)
                {
                    clipRaster = pOutputDataset as IRaster;
                }
                else
                {
                    MessageBox.Show("无法处理输出数据集类型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 显示结果
                ShowRasterResult(pOutputDataset, saveFileName);
                MessageBox.Show("掩膜提取成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行掩膜提取时出错: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 显示输出栅格结果
        private static void ShowRasterResult(IGeoDataset geoDataset, string rasterName)
        {
            try
            {
                // 创建新的栅格图层
                IRasterLayer rasterLayer = new RasterLayerClass();
                IRaster raster = geoDataset as IRaster;

                if (raster == null)
                {
                    MessageBox.Show("无法将数据集转换为栅格", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 从栅格创建图层并设置名称
                rasterLayer.CreateFromRaster(raster);
                rasterLayer.Name = rasterName;

                // 获取当前主窗体实例并添加图层
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Mainform)
                    {
                        Mainform mainForm = form as Mainform;
                        mainForm.axMapControl1.AddLayer((ILayer)rasterLayer, 0);
                        mainForm.axMapControl1.ActiveView.Refresh();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("显示结果时出错: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

