using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
#if NETSTANDARD2_0
using ZXing.Windows.Compatibility;
#endif

namespace ImageUtil
{
    /// <summary>
    /// 条形码帮助类
    /// </summary>
    public class BarCodeUtil
    {
        #region 生成条形码
        /// <summary>
        /// 生成条形码
        /// </summary>
        /// <param name="content">编码的内容字符串</param>
        /// <param name="filePath">生成的图片保存路径</param>
        /// <param name="margin">生成图片白色边框宽度,默认无</param>
        /// <param name="pureBarcode">是否是纯条形码,false表示图片底部显示content,默认true</param>
        /// <param name="height">生成图片的高度,默认50</param>
        /// <param name="width">生成图片的宽度</param>
        public static void Encode(string content, string filePath, int margin = 0, bool pureBarcode = true, int height = 50, int? width = null)
        {
            var options = new EncodingOptions
            {
                Height = height,
                Margin = margin,
                PureBarcode = pureBarcode
            };
            if (width != null)
            {
                options.Width = (int)width;
            }
            _encode(content, filePath, options);
        }

        private static void _encode(string content, string filePath, EncodingOptions options)
        {
            var writer = new ZXing.BarcodeWriter<Bitmap>();
            writer.Format = BarcodeFormat.CODE_128;
            writer.Options = options;
            writer.Renderer = new BitmapRenderer();
            var bitmap = writer.Write(content);
            bitmap.Save(filePath, ImageFormat.Png);
        }
        #endregion
    }
}

