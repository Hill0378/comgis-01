using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;

namespace ecological_alert
{
    class SlopeCalculator
    {
        public static void CalculateSlope(List<IRaster> rasters, List<int> years, string outputPath)
        {
            int N = rasters.Count;
            double[] x = years.Select(y => (double)y).ToArray();

            IRasterProps props = (IRasterProps)rasters[0];
            int width = props.Width;   // 列数
            int height = props.Height; // 行数

            double sumX = x.Sum();
            double sumX2 = x.Select(xi => xi * xi).Sum();

            List<float[,]> rasterPixels = new List<float[,]>();

            for (int idx = 0; idx < N; idx++)
            {
                float[,] pixels = ReadRasterPixels(rasters[idx], width, height);
                rasterPixels.Add(pixels);
            }

            float[,] slopePixels = new float[height, width];  // 注意维度 [行, 列]

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    double sumY = 0;
                    double sumXY = 0;
                    bool valid = true;

                    for (int i = 0; i < N; i++)
                    {
                        float val = rasterPixels[i][row, col];
                        if (float.IsNaN(val) || val == -9999)
                        {
                            valid = false;
                            break;
                        }
                        sumY += val;
                        sumXY += val * x[i];
                    }

                    if (!valid)
                    {
                        slopePixels[row, col] = -9999f;
                    }
                    else
                    {
                        double numerator = N * sumXY - sumX * sumY;
                        double denominator = N * sumX2 - sumX * sumX;
                        slopePixels[row, col] = (float)((denominator != 0) ? numerator / denominator : 0);
                    }
                }
            }

            SaveRaster2(slopePixels, props, outputPath);
        }

        public static float[,] ReadRasterPixels(IRaster raster, int width, int height)
        {
            IRaster2 raster2 = (IRaster2)raster;
            IPnt blockSize = new Pnt();
            blockSize.SetCoords(width, height);
            IRasterCursor cursor = raster2.CreateCursorEx(blockSize);

            IPixelBlock pixelBlock = cursor.PixelBlock;
            Array arr = (Array)pixelBlock.get_SafeArray(0);

            float[,] result = new float[height, width];  // [行, 列]

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // SafeArray索引顺序是 [列, 行]
                    object val = arr.GetValue(col, row);
                    result[row, col] = Convert.ToSingle(val);
                }
            }

            return result;
        }

        public static void SaveRaster(float[,] pixels, IRasterProps templateProps, string outputPath)
        {
            int width = templateProps.Width;
            int height = templateProps.Height;

            string folderPath = System.IO.Path.GetDirectoryName(outputPath);
            string fileName = System.IO.Path.GetFileName(outputPath);

            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            IRasterWorkspace2 rasterWorkspace = wsFactory.OpenFromFile(folderPath, 0) as IRasterWorkspace2;

            IPoint origin = new Point();
            // 注意Y坐标用最小值（左下角），保证空间参考正确
            origin.PutCoords(templateProps.Extent.XMin, templateProps.Extent.YMin);

            IRasterDataset rasterDataset = rasterWorkspace.CreateRasterDataset(
                fileName,
                "TIFF",
                origin,
                width,
                height,
                templateProps.MeanCellSize().X,
                templateProps.MeanCellSize().Y,
                1,
                rstPixelType.PT_FLOAT,
                templateProps.SpatialReference,
                true);

            IRaster raster = rasterDataset.CreateDefaultRaster();
            IRasterEdit rasterEdit = raster as IRasterEdit;

            IRasterProps rasterProps = (IRasterProps)raster;
            rasterProps.NoDataValue = -9999f;

            IPnt blockSize = new Pnt();
            blockSize.SetCoords(width, height);

            IPnt topLeft = new Pnt();
            topLeft.SetCoords(0, 0);

            IPixelBlock pixelBlock = raster.CreatePixelBlock(blockSize);
            Array arr = (Array)pixelBlock.get_SafeArray(0);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // 写入时，SafeArray索引是[col, row]
                    arr.SetValue(pixels[row, col], col, row);
                }
            }

            pixelBlock.set_SafeArray(0, arr);
            rasterEdit.Write(topLeft, pixelBlock);
            rasterEdit.Refresh();
            // === 计算并写入统计信息，避免ArcMap显示异常 ===
            /*IRaster2 raster2 = raster as IRaster2;
            IRasterBandCollection bandCol = raster2 as IRasterBandCollection;
            IRasterBand band = bandCol.Item(0);

            // 设置 NoData
            IRasterProps bandProps = (IRasterProps)band;
            bandProps.NoDataValue = -9999f;

            // 计算统计信息，跳过 NoData
            IRasterBandEdit2 bandEdit = band as IRasterBandEdit2;
            if (bandEdit != null)
            {
                int skipFactorX = 1;
                int skipFactorY = 1;
                object ignoreValues = Type.Missing;
                bool requireHistogram = true;

                bandEdit.ComputeStatisticsHistogram(skipFactorX, skipFactorY, ignoreValues, requireHistogram);

            }

            // 自动跳过 NoData 值计算 min/max

            //band.HasStatistics = true;   // 标记 Band 有统计信息*/


            Marshal.ReleaseComObject(rasterEdit);
            Marshal.ReleaseComObject(raster);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(wsFactory);
        }
        public static void SaveRaster2(float[,] pixels, IRasterProps templateProps, string outputPath)
        {
            int width = templateProps.Width;
            int height = templateProps.Height;

            string folderPath = System.IO.Path.GetDirectoryName(outputPath);
            string fileName = System.IO.Path.GetFileName(outputPath);

            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            IRasterWorkspace2 rasterWorkspace = wsFactory.OpenFromFile(folderPath, 0) as IRasterWorkspace2;

            IPoint origin = new Point();
            origin.PutCoords(templateProps.Extent.XMin, templateProps.Extent.YMin);

            IRasterDataset rasterDataset = rasterWorkspace.CreateRasterDataset(
                fileName,
                "TIFF",
                origin,
                width,
                height,
                templateProps.MeanCellSize().X,
                templateProps.MeanCellSize().Y,
                1,
                rstPixelType.PT_FLOAT,
                templateProps.SpatialReference,
                true);

            IRaster raster = rasterDataset.CreateDefaultRaster();
            IRasterEdit rasterEdit = raster as IRasterEdit;

            IRasterProps rasterProps = (IRasterProps)raster;
            rasterProps.NoDataValue = -9999f;

            IPnt blockSize = new Pnt();
            blockSize.SetCoords(width, height);

            IPnt topLeft = new Pnt();
            topLeft.SetCoords(0, 0);

            IPixelBlock pixelBlock = raster.CreatePixelBlock(blockSize);
            Array arr = (Array)pixelBlock.get_SafeArray(0);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    arr.SetValue(pixels[row, col], col, row);
                }
            }

            pixelBlock.set_SafeArray(0, arr);
            rasterEdit.Write(topLeft, pixelBlock);
            rasterEdit.Refresh();

            // === 清理资源 ===
            Marshal.ReleaseComObject(rasterEdit);
            Marshal.ReleaseComObject(raster);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(wsFactory);

            // === 重新打开保存的 raster 并计算统计信息 ===
            ComputeRasterStatistics(outputPath);
        }
        public static void ComputeRasterStatistics(string rasterPath)
        {
            string folderPath = System.IO.Path.GetDirectoryName(rasterPath);
            string fileName = System.IO.Path.GetFileName(rasterPath);

            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            IRasterWorkspace2 rasterWorkspace = wsFactory.OpenFromFile(folderPath, 0) as IRasterWorkspace2;

            IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);
            IRasterBandCollection bandCol = rasterDataset as IRasterBandCollection;
            IRasterBand band = bandCol.Item(0);

            IRasterProps bandProps = band as IRasterProps;
            bandProps.NoDataValue = -9999f;

            IRasterBandEdit2 bandEdit = band as IRasterBandEdit2;
            if (bandEdit != null)
            {
                int skipFactorX = 1;
                int skipFactorY = 1;
                object ignoreValues = Type.Missing;
                bool requireHistogram = true;

                try
                {
                    bandEdit.ComputeStatisticsHistogram(skipFactorX, skipFactorY, ignoreValues, requireHistogram);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ComputeStatisticsHistogram 异常：" + ex.Message);
                }
                }

            Marshal.ReleaseComObject(band);
            Marshal.ReleaseComObject(bandCol);
            Marshal.ReleaseComObject(rasterDataset);
            Marshal.ReleaseComObject(rasterWorkspace);
            Marshal.ReleaseComObject(wsFactory);
        }



    }
}
