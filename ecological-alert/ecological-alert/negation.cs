using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using System.Runtime.InteropServices;

namespace ecological_alert
{
    public partial class negation : Form
    {
        private AxMapControl _mapControl;

        public negation(AxMapControl axMapControl1)
        {
            InitializeComponent();
            _mapControl = axMapControl1;
            LoadVectorLayers1();
            LoadVectorLayers2();
        }

        // 加载多边形矢量图层到复选框列表1
        private void LoadVectorLayers1()
        {
            clbVectorLayers1.Items.Clear();
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer is IFeatureLayer featureLayer &&
                    featureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    clbVectorLayers1.Items.Add(layer.Name, true);
                }
            }
        }

        // 加载多边形矢量图层到复选框列表2
        private void LoadVectorLayers2()
        {
            clbVectorLayers2.Items.Clear();
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer is IFeatureLayer featureLayer &&
                    featureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    clbVectorLayers2.Items.Add(layer.Name, true);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 输入验证
            if (clbVectorLayers1.CheckedItems.Count == 0 || clbVectorLayers2.CheckedItems.Count == 0)
            {
                MessageBox.Show("请至少选择一个矢量图层1和矢量图层2！");
                return;
            }

            // 获取选中的图层
            var vectorLayers1 = GetCheckedLayers(clbVectorLayers1);
            var vectorLayers2 = GetCheckedLayers(clbVectorLayers2);

            // 执行差异操作
            foreach (var layer1 in vectorLayers1)
            {
                foreach (var layer2 in vectorLayers2)
                {
                    try
                    {
                        var geometryCollection = ProcessDifference(layer1, layer2);
                        SaveAndDisplayResult(geometryCollection, $"{layer1.Name}_更新");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"处理图层时出错: {ex.Message}");
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private IGeometryCollection ProcessDifference(IFeatureLayer layer1, IFeatureLayer layer2)
        {
            IGeometryCollection resultCollection = new GeometryBagClass();
            IFeatureCursor cursor1 = null;
            try
            {
                cursor1 = layer1.FeatureClass.Search(null, true);
                IFeature feature1;
                while ((feature1 = cursor1.NextFeature()) != null)
                {
                    var polygon1 = feature1.Shape as IPolygon;
                    if (polygon1 == null) continue;

                    IFeatureCursor cursor2 = null;
                    try
                    {
                        cursor2 = layer2.FeatureClass.Search(null, true);
                        IFeature feature2;
                        while ((feature2 = cursor2.NextFeature()) != null)
                        {
                            var polygon2 = feature2.Shape as IPolygon;
                            if (polygon2 == null) continue;

                            // 转换为拓扑操作接口
                            ITopologicalOperator topoOp = polygon1 as ITopologicalOperator;
                            topoOp?.Simplify();

                          

                            // 执行差异操作
                            var difference = topoOp?.Difference(polygon2);
                            if (difference != null && !difference.IsEmpty)
                                resultCollection.AddGeometry(difference);
                        }
                    }
                    finally
                    {
                        if (cursor2 != null)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursor2);
                    }
                }
            }
            finally
            {
                if (cursor1 != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(cursor1);
            }
            return resultCollection;
        }

        // 修改SaveAndDisplayResult中的FeatureBuffer管理
        //private void SaveAndDisplayResult(IGeometryCollection geometryCollection, string layerName)
        //{
        //    IFeatureClass featureClass = CreateOutputFeatureClass(layerName);
        //    IFeatureBuffer featureBuffer = null;
        //    try
        //    {
        //        featureBuffer = featureClass.CreateFeatureBuffer();
        //        for (int i = 0; i < geometryCollection.GeometryCount; i++)
        //        {
        //            var geometry = geometryCollection.get_Geometry(i);
        //            if (geometry.IsEmpty) continue;

        //            IFeature feature = featureClass.CreateFeature();
        //            feature.Shape = geometry;
        //            feature.Store();
        //        }
        //    }
        //    finally
        //    {
        //        if (featureBuffer != null)
        //            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureBuffer);
        //    }

        //    // 添加到地图
        //    IFeatureLayer featureLayer = new FeatureLayer();
        //    featureLayer.FeatureClass = featureClass;
        //    featureLayer.Name = layerName;
        //    _mapControl.AddLayer(featureLayer, 0);
        //    _mapControl.Refresh();
        //}

        private void SaveAndDisplayResult(IGeometryCollection geometryCollection, string layerName)
        {
            IFeatureClass featureClass = CreateOutputFeatureClass(layerName);

            for (int i = 0; i < geometryCollection.GeometryCount; i++)
            {
                IGeometry geometry = geometryCollection.get_Geometry(i);
                if (geometry == null || geometry.IsEmpty)
                    continue;

                IFeature feature = featureClass.CreateFeature();
                feature.Shape = geometry;
                feature.Store();
            }

            // 显示图层
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = layerName;

            _mapControl.AddLayer(featureLayer);
            _mapControl.Refresh();
        }

        // 创建输出要素类
        //private IFeatureClass CreateOutputFeatureClass(string layerName)
        //{
        //    IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
        //    IWorkspace workspace = workspaceFactory.OpenFromFile(System.IO.Path.GetTempPath(), 0);

        //    // 定义字段
        //    IFields fields = new Fields();
        //    IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

        //    // 添加几何字段
        //    IField shapeField = new Field();
        //    IFieldEdit shapeFieldEdit = (IFieldEdit)shapeField;
        //    shapeFieldEdit.Name_2 = "Shape";
        //    shapeFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

        //    // 几何定义
        //    IGeometryDef geometryDef = new GeometryDef();
        //    IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
        //    geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
        //    geometryDefEdit.SpatialReference_2 = _mapControl.SpatialReference;

        //    shapeFieldEdit.GeometryDef_2 = geometryDef;
        //    fieldsEdit.AddField(shapeField);

        //    // 创建要素类
        //    return ((IFeatureWorkspace)workspace).CreateFeatureClass(
        //        layerName,
        //        fields,
        //        null,
        //        null,
        //        esriFeatureType.esriFTSimple,
        //        "Shape",
        //        "");
        //}

        private IFeatureClass CreateOutputFeatureClass(string layerName)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = workspaceFactory.OpenFromFile(System.IO.Path.GetTempPath(), 0);

            // 定义字段
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            // 添加OID字段
            IField oidField = new FieldClass();
            IFieldEdit oidFieldEdit = (IFieldEdit)oidField;
            oidFieldEdit.Name_2 = "OID";
            oidFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(oidField);

            // 添加Shape字段
            IField shapeField = new FieldClass();
            IFieldEdit shapeFieldEdit = (IFieldEdit)shapeField;
            shapeFieldEdit.Name_2 = "Shape";
            shapeFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            // 几何定义
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            geometryDefEdit.SpatialReference_2 = _mapControl.SpatialReference;
            geometryDefEdit.HasZ_2 = false;
            geometryDefEdit.HasM_2 = false;

            shapeFieldEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(shapeField);

            return ((IFeatureWorkspace)workspace).CreateFeatureClass(
                layerName,
                fields,
                null,
                null,
                esriFeatureType.esriFTSimple,
                "Shape",
                ""
            );
        }


        private List<IFeatureLayer> GetCheckedLayers(CheckedListBox clb)
        {
            var layers = new List<IFeatureLayer>();
            foreach (var item in clb.CheckedItems)
            {
                string name = item.ToString();
                var layer = FindVectorLayer(name);
                if (layer != null) layers.Add(layer);
            }
            return layers;
        }

        private IFeatureLayer FindVectorLayer(string layerName)
        {
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                var layer = _mapControl.Map.get_Layer(i);
                if (layer.Name == layerName && layer is IFeatureLayer featureLayer)
                    return featureLayer;
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}