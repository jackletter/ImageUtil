using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.CompilerServices;
using System.Drawing.Printing;

namespace ImageUtil
{
    #region 水印位置
    /// <summary>
    /// 水印位置
    /// </summary>
    public enum EnumLocation
    {
        /// <summary>
        /// 左上角
        /// </summary>
        LeftTop,
        /// <summary>
        /// 上边中间
        /// </summary>
        Top,
        /// <summary>
        /// 右上角
        /// </summary>
        RightTop,
        /// <summary>
        /// 左侧中间
        /// </summary>
        LeftCenter,
        /// <summary>
        /// 正中
        /// </summary>
        Center,
        /// <summary>
        /// 右侧中间
        /// </summary>
        RightCenter,
        /// <summary>
        /// 左下角
        /// </summary>
        LeftBottom,
        /// <summary>
        /// 底部中间
        /// </summary>
        Bottom,
        /// <summary>
        /// 右下角
        /// </summary>
        RightBottom
    }
    #endregion

    #region 缩放模式
    /// <summary>
    /// 缩放模式
    /// </summary>
    public enum EnumZoomModel
    {
        /// <summary>
        /// 缩放到宽度全部填满
        /// </summary>
        FillWidth,
        /// <summary>
        /// 缩放到高度全部填满
        /// </summary>
        FillHeight,
        /// <summary>
        /// 缩放到高度和宽度全部填满
        /// </summary>
        FillAll
    }
    #endregion

    #region 图片处理类
    /// <summary>
    /// 图片处理类
    /// </summary>
    public class ImageProcess
    {
        private const int ColorTransparent = 120;
        private const int Margin = 20;
        private Bitmap processImage;

        #region 保存图片
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="destPath">保存图片的路径</param>
        /// <param name="format">要保存的图片格式</param>
        public void Save(string destPath, ImageFormat format)
        {
            var dir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(dir))
            {
                try { Directory.CreateDirectory(dir); } catch { }
            }
            processImage.Save(destPath, format);
        }
        #endregion

        #region 使用源图片路径初始化图片处理器
        /// <summary>
        /// 使用源图片路径初始化图片处理器
        /// </summary>
        /// <param name="srcImg">源图片路径</param>
        public ImageProcess(string srcImg)
        {
            this.processImage = Bitmap.FromFile(srcImg) as Bitmap;
        }
        #endregion

        #region 缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="width">缩略图宽度</param>
        public ImageProcess Thumbnail(int width)
        {
            int towidth = width;
            int toheight = processImage.Height * width / processImage.Width;
            //新建一个bmp图片
            var bitmap = new Bitmap(towidth, toheight);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.Default;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.Default;
                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(processImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(0, 0, processImage.Width, processImage.Height), GraphicsUnit.Pixel);
                processImage = bitmap;
            }
            return this;
        }
        #endregion

        #region 图片水印
        /// <summary>
        /// 图片水印处理方法
        /// </summary>
        /// <param name="waterpath">水印图片（绝对路径）</param>
        /// <param name="location">水印位置（传送正确的代码）</param>
        public ImageProcess ImageWatermark(string waterpath, EnumLocation location)
        {
            Bitmap waterimg = Image.FromFile(waterpath) as Bitmap;
            var waterimg2 = new Bitmap(waterimg.Width, waterimg.Height);
            var wid = waterimg.Width;
            var hei = waterimg.Height;
            for (int i = 0; i < wid; i++)
            {
                for (int j = 0; j < hei; j++)
                {
                    var baseColor = waterimg.GetPixel(i, j);
                    if (baseColor.A != 0)
                    {
                        baseColor = Color.FromArgb(ColorTransparent, baseColor.R, baseColor.G, baseColor.B);
                    }
                    waterimg2.SetPixel(i, j, baseColor);
                }
            }
            ArrayList loca = GetLocation(location, processImage, waterimg2.Width, waterimg2.Height);
            using (var g = Graphics.FromImage(processImage))
            {
                g.DrawImage(waterimg2, new Rectangle(int.Parse(loca[0].ToString()), int.Parse(loca[1].ToString()), waterimg.Width, waterimg.Height));
                waterimg.Dispose();
            };
            return this;
        }

        /// <summary>
        /// 水印位置计算
        /// </summary>
        /// <param name="location">水印位置</param>
        /// <param name="img">需要添加水印的图片</param>
        /// <param name="waterWidth">水印宽度</param>
        /// <param name="waterHeight">水印高度</param>
        private static ArrayList GetLocation(EnumLocation location, Image img, int waterWidth, int waterHeight)
        {
            ArrayList loca = new ArrayList();
            int x = 0;
            int y = 0;

            if (location == EnumLocation.LeftTop)
            {
                x = Margin;
                y = Margin;
            }
            else if (location == EnumLocation.Top)
            {
                x = img.Width / 2 - waterWidth / 2;
                y = Margin;
            }
            else if (location == EnumLocation.RightTop)
            {
                x = img.Width - waterWidth - 10;
                y = Margin;
            }
            else if (location == EnumLocation.LeftCenter)
            {
                x = Margin;
                y = img.Height / 2 - waterHeight / 2;
            }
            else if (location == EnumLocation.Center)
            {
                x = img.Width / 2 - waterWidth / 2;
                y = img.Height / 2 - waterHeight / 2;
            }
            else if (location == EnumLocation.RightCenter)
            {
                x = img.Width - waterWidth - Margin;
                y = img.Height / 2 - waterHeight / 2;
            }
            else if (location == EnumLocation.LeftBottom)
            {
                x = Margin;
                y = img.Height - waterHeight - Margin;
            }
            else if (location == EnumLocation.Bottom)
            {
                x = img.Width / 2 - waterWidth / 2;
                y = img.Height - waterHeight - Margin;
            }
            else if (location == EnumLocation.RightBottom)
            {
                x = img.Width - waterWidth - Margin;
                y = img.Height - waterHeight - Margin;
            }
            loca.Add(x);
            loca.Add(y);
            return loca;
        }
        #endregion

        #region 文字水印
        /// <summary>
        /// 文字水印处理方法
        /// </summary>
        /// <param name="font">字体</param>
        /// <param name="letter">水印文字</param>
        /// <param name="color">颜色</param>
        /// <param name="location">水印位置</param>
        public ImageProcess LetterWatermark(string letter, Font font, Color color, EnumLocation location)
        {
            Brush br = new SolidBrush(Color.FromArgb(ColorTransparent, color.R, color.G, color.B));
            using (var g = Graphics.FromImage(processImage))
            {
                var size = g.MeasureString(letter, font).ToSize();
                ArrayList loca = GetLocation(location, processImage, size.Width, size.Height);
                g.DrawString(letter, font, br, float.Parse(loca[0].ToString()), float.Parse(loca[1].ToString()));
            }
            return this;
        }
        #endregion

        #region 调整颜色明暗度(其实就是对RGB的每个颜色值做相同的加减)
        /// <summary>
        /// 调整颜色明暗度(其实就是对RGB的每个颜色值做相同的加减)
        /// </summary>
        /// <param name="addPercent">增加或减少的光暗值(-1,1)</param>
        public ImageProcess DeepColor(double addPercent)
        {
            int x, y, resultR, resultG, resultB;//x、y是循环次数，后面三个是记录红绿蓝三个值的
            Color pixel;
            var wid = processImage.Width;
            var hei = processImage.Height;
            var bitmap = new Bitmap(wid, hei);
            for (x = 0; x < wid; x++)
            {
                for (y = 0; y < hei; y++)
                {
                    pixel = processImage.GetPixel(x, y);//获取当前像素的值
                    resultR = (int)(pixel.R * (1 + addPercent));
                    resultR = Math.Min(255, Math.Max(0, resultR));
                    resultG = (int)(pixel.G * (1 + addPercent));
                    resultG = Math.Min(255, Math.Max(0, resultG));
                    resultB = (int)(pixel.B * (1 + addPercent));
                    resultB = Math.Min(255, Math.Max(0, resultB));
                    bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, resultR, resultG, resultB));//绘图
                }
            }
            processImage.Dispose();
            processImage = bitmap;
            return this;
        }
        #endregion

        #region 反色处理
        /// <summary>
        /// 反色处理
        /// </summary>
        public ImageProcess ReverseColor()
        {
            int x, y, resultR, resultG, resultB;
            var wid = processImage.Width;
            var hei = processImage.Height;
            Color pixel;
            for (x = 0; x < wid; x++)
            {
                for (y = 0; y < hei; y++)
                {
                    pixel = processImage.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    processImage.SetPixel(x, y, Color.FromArgb(pixel.A, resultR, resultG, resultB));//绘图
                }
            }
            return this;
        }
        #endregion

        #region 浮雕处理
        /// <summary>
        /// 浮雕处理
        /// </summary>
        public ImageProcess Carve()
        {
            var wid = processImage.Width;
            var hei = processImage.Height;
            Bitmap newBitmap = new Bitmap(wid, hei);
            Color color1, color2;
            for (int x = 0; x < wid - 1; x++)
            {
                for (int y = 0; y < hei - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    color1 = processImage.GetPixel(x, y);
                    color2 = processImage.GetPixel(x + 1, y + 1);
                    r = Math.Abs(color1.R - color2.R + 128);
                    g = Math.Abs(color1.G - color2.G + 128);
                    b = Math.Abs(color1.B - color2.B + 128);
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;
                    newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            processImage = newBitmap;
            return this;
        }
        #endregion

        #region 拉伸图片
        /// <summary>
        /// 拉伸图片
        /// </summary>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        public ImageProcess Stretch(int newW, int newH)
        {
            Bitmap bap = new Bitmap(newW, newH);
            using (var g = Graphics.FromImage(bap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(processImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, processImage.Width, processImage.Height), GraphicsUnit.Pixel);
            }
            processImage = bap;
            return this;
        }
        #endregion

        #region 滤色处理
        /// <summary>
        /// 滤色处理
        /// </summary>
        public ImageProcess RemoveColorChanel(bool removeA, bool removeR, bool removeG, bool removeB)
        {
            var width = processImage.Width;
            var height = processImage.Height;
            int x, y;
            Color pixel;
            var bitMap = new Bitmap(width, height);

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = processImage.GetPixel(x, y);//获取当前坐标的像素值
                    bitMap.SetPixel(x, y, Color.FromArgb(removeA ? 255 : pixel.A, removeR ? 0 : pixel.R, removeG ? 0 : pixel.G, removeB ? 0 : pixel.B));
                }
            }
            processImage.Dispose();
            processImage = bitMap;
            return this;
        }
        #endregion

        #region 透明处理

        /// <summary>
        /// 将指定的颜色透明处理
        /// </summary>
        /// <param name="color">要透明化处理的颜色</param>
        /// <returns></returns>
        public ImageProcess MakeTransparent(Color color)
        {
            processImage.MakeTransparent(color);
            return this;
        }

        /// <summary>
        /// 将当前图片进行整体透明处理
        /// </summary>
        /// <param name="percent">相对于当前的变化比例(-1,1)</param>
        /// <returns></returns>
        public ImageProcess MakeOpacity(double percent)
        {
            var wid = processImage.Width;
            var hei = processImage.Height;
            var bitMap = new Bitmap(wid, hei);
            for (int x = 0; x < wid; x++)
            {
                for (int y = 0; y < hei; y++)
                {
                    var pix = processImage.GetPixel(x, y);
                    var a = (int)(pix.A * (1 + percent));
                    a = Math.Min(Math.Max(a, 0), 255);
                    bitMap.SetPixel(x, y, Color.FromArgb(a, pix.R, pix.G, pix.B));
                }
            }
            processImage.Dispose();
            processImage = bitMap;
            return this;
        }

        /// <summary>
        /// 移除图片的透明度
        /// </summary>
        /// <returns></returns>
        public ImageProcess RemoveOpacity()
        {
            var wid = processImage.Width;
            var hei = processImage.Height;
            var bitMap = new Bitmap(wid, hei);
            for (int x = 0; x < wid; x++)
            {
                for (int y = 0; y < hei; y++)
                {
                    var pix = processImage.GetPixel(x, y);
                    bitMap.SetPixel(x, y, Color.FromArgb(255, pix.R, pix.G, pix.B));
                }
            }
            processImage.Dispose();
            processImage = bitMap;
            return this;
        }
        #endregion

        #region 左右翻转
        /// <summary>
        /// 左右翻转
        /// </summary>
        public ImageProcess ReverseLeftRight()
        {
            var width = processImage.Width;
            var height = processImage.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y;
            Color pixel;
            for (y = height - 1; y >= 0; y--)
            {
                for (x = width - 1; x >= 0; x--)
                {
                    pixel = processImage.GetPixel(x, y);
                    bm.SetPixel(width - 1 - x, y, Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                }
            }
            processImage.Dispose();
            processImage = bm;
            return this;
        }
        #endregion

        #region 上下翻转
        /// <summary>
        /// 上下翻转
        /// </summary>
        public ImageProcess ReverseTopBottom()
        {
            var width = processImage.Width;
            var height = processImage.Height;
            Bitmap bm = new Bitmap(width, height);
            int x, y;
            Color pixel;
            for (x = 0; x < width; x++)
            {
                for (y = height - 1; y >= 0; y--)
                {
                    pixel = processImage.GetPixel(x, y);
                    bm.SetPixel(x, height - 1 - y, Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                }
            }
            processImage.Dispose();
            processImage = bm;
            return this;
        }
        #endregion

        #region 图片灰度化
        /// <summary>
        /// 图片灰度化
        /// </summary>
        /// <returns></returns>
        public ImageProcess Gray()
        {
            var wid = processImage.Width;
            var hei = processImage.Height;
            var bitMap = new Bitmap(wid, hei);
            for (int x = 0; x < wid; x++)
            {
                for (int y = 0; y < hei; y++)
                {
                    var pix = processImage.GetPixel(x, y);
                    int gray = (int)(pix.R * 0.3 + pix.G * 0.59 + pix.B * 0.11);
                    bitMap.SetPixel(x, y, Color.FromArgb(255, gray, gray, gray));
                }
            }
            processImage.Dispose();
            processImage = bitMap;
            return this;
        }
        #endregion

        #region 转换为黑白图片
        /// <summary>
        /// 转换为黑白图片
        /// </summary>
        /// <returns></returns>
        public ImageProcess BlackWhite()
        {
            var wid = processImage.Width;
            var hei = processImage.Height;
            var bitmap = new Bitmap(wid, hei);
            int x, y, result;
            Color pixel;
            for (x = 0; x < wid; x++)
            {
                for (y = 0; y < hei; y++)
                {
                    pixel = processImage.GetPixel(x, y);
                    result = (pixel.R + pixel.G + pixel.B) / 3;
                    if (result > 255 / 2)
                    {
                        result = 255;
                    }
                    else
                    {
                        result = 0;
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(255, result, result, result));
                }
            }
            processImage.Dispose();
            processImage = bitmap;
            return this;
        }
        #endregion

        #region 将gif图片按每帧进行切割,并返回切分好的图片
        /// <summary>
        /// 将gif图片按每帧进行切割,并返回切分好的图片
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavedPath">保存路径</param>
        public static List<string> SplitGif(string pPath, string pSavedPath)
        {
            pSavedPath = Path.GetFullPath(pSavedPath);
            var res = new List<string>();
            Image gif = Image.FromFile(pPath);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            int count = gif.GetFrameCount(fd);
            for (int i = 0; i < count; i++)
            {
                //以Jpeg格式保存各帧
                gif.SelectActiveFrame(fd, i);
                string filepath = Path.Combine(pSavedPath, "frame_" + i + ".jpg");
                res.Add(filepath);
                gif.Save(filepath, ImageFormat.Jpeg);
            }
            return res;
        }
        #endregion

        #region 按缩放模式等比例缩放图片
        /// <summary>
        /// 按缩放模式等比例缩放图片
        /// </summary>
        /// <param name="newW">目标宽度</param>
        /// <param name="newH">目标高度</param>
        /// <param name="zoomModel">缩放模式</param>
        /// <returns></returns>
        public ImageProcess Zoom(int newW, int newH, EnumZoomModel zoomModel)
        {
            var wid = processImage.Width;
            var hei = processImage.Height;
            var percent = hei / (wid + 0.0);
            if (zoomModel == EnumZoomModel.FillWidth)
            {
                var newH2 = (int)(newW * percent);
                Stretch(newW, newH2);
                var bitMap = new Bitmap(newW, newH);
                if (newH2 >= newH)
                {
                    var diff = (newH2 - newH) / 2;
                    using (var g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(processImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, diff, newW, newH), GraphicsUnit.Pixel);
                    }
                }
                else if (newH2 < newH)
                {
                    var diff = (newH - newH2) / 2;
                    using (var g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(processImage, new Rectangle(0, diff, newW, newH2), new Rectangle(0, 0, newW, newH2), GraphicsUnit.Pixel);
                    }
                }
                processImage.Dispose();
                processImage = bitMap;
            }
            else if (zoomModel == EnumZoomModel.FillHeight)
            {
                var newW2 = (int)(newH / percent);
                Stretch(newW2, newH);
                var bitMap = new Bitmap(newW, newH);
                if (newW2 >= newW)
                {
                    var diff = (newW2 - newH) / 2;
                    using (var g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(processImage, new Rectangle(0, 0, newW, newH), new Rectangle(diff, 0, newW, newH), GraphicsUnit.Pixel);
                    }
                }
                else if (newW2 < newW)
                {
                    var diff = (newW - newW2) / 2;
                    using (var g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(processImage, new Rectangle(diff, 0, newW2, newH), new Rectangle(0, 0, newW2, newH), GraphicsUnit.Pixel);
                    }
                }
                processImage.Dispose();
                processImage = bitMap;
            }
            else if (zoomModel == EnumZoomModel.FillAll)
            {
                var percent2 = newH / (newW + 0.0);
                var bitMap = new Bitmap(newW, newH);
                if (percent >= percent2)
                {
                    //长度一致,高度裁剪
                    var newH2 = (int)(newW * percent);
                    Stretch(newW, newH2);
                    var diff = (newH2 - newH) / 2;
                    using (var g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(processImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, diff, newW, newH), GraphicsUnit.Pixel);
                    }
                }
                else if (percent < percent2)
                {
                    //高度一致,长度裁剪
                    var newW2 = (int)(newH / percent);
                    Stretch(newW2, newH);
                    var diff = (newW2 - newW) / 2;
                    using (var g = Graphics.FromImage(bitMap))
                    {
                        g.DrawImage(processImage, new Rectangle(0, 0, newW, newH), new Rectangle(diff, 0, newW, newH), GraphicsUnit.Pixel);
                    }
                }
                processImage.Dispose();
                processImage = bitMap;
            }
            return this;
        }
        #endregion
    }
    #endregion
}
