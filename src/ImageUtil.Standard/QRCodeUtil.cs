using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using ThoughtWorks.QRCode.Codec.Data;
using System.IO;

namespace ImageUtil
{
    #region 二维码的尺寸选项枚举
    /// <summary>
    /// 二维码的尺寸选项
    /// </summary>
    public enum EnumSizeMode
    {
        /// <summary>
        /// 将生成的二维码进行缩放以满足目标尺寸
        /// </summary>
        Stretch,
        /// <summary>
        /// 将生成的二维码直接输出(这意味着,生成的二维码的大小将最接近但不大于目标大小)
        /// </summary>
        Pure,
        /// <summary>
        /// 将生成的二维码进行白边填充以满足目标尺寸
        /// </summary>
        WhiteBorderFill
    }
    #endregion

    /// <summary>
    /// 二维码工具类
    /// </summary>
    public class QRCodeUtil
    {
        #region 生成二维码图片
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="str">内容字符串</param>
        /// <param name="maxSize">生成的图片大小,图片宽度以像素为单位(默认100)</param>
        /// <param name="enumSizeMode">二维码大小模式(默认进行缩放已适应maxSize)</param>
        /// <param name="filePath">生成的二维码图片路径</param>
        /// <param name="backImgPath">中心图标路径(如果没有找到图标就忽略)</param>
        /// <param name="zoomBackImg">是否缩放背景图片(默认进行缩放)</param>
        public static void Encode(string str, string filePath, int maxSize = 100, EnumSizeMode enumSizeMode = EnumSizeMode.Stretch, string backImgPath = null, bool zoomBackImg = true)
        {
            Image image = CreateQRCode(str, QRCodeEncoder.ENCODE_MODE.BYTE, QRCodeEncoder.ERROR_CORRECTION.M, 8, 5, maxSize);
            var actualSize = image.Width;
            #region 根据enumSizeMode处理生成的二维码大小
            if (maxSize - image.Width >= 2)
            {
                if (enumSizeMode == EnumSizeMode.WhiteBorderFill)
                {
                    Bitmap bitmap = new Bitmap(maxSize, maxSize);
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        int p_left = (maxSize - image.Width) / 2;
                        int p_top = (maxSize - image.Height) / 2;
                        g.Clear(System.Drawing.Color.White);
                        //将生成的二维码图像粘贴至绘图的中心位置
                        g.DrawImage(image, p_left, p_top, image.Width, image.Height);
                        image = new Bitmap(bitmap);
                        bitmap.Dispose();
                        bitmap = null;
                    }
                }
                else if (enumSizeMode == EnumSizeMode.Stretch)
                {
                    image = ResizeImage(image, maxSize, maxSize);
                }
            }
            #endregion
            if (!string.IsNullOrWhiteSpace(backImgPath) && File.Exists(backImgPath))
            {
                AddCenterImage(image, actualSize, backImgPath, zoomBackImg);
            }
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                try { Directory.CreateDirectory(dir); } catch { }
            }
            image.Save(filePath);
        }
        #endregion

        #region 添加中心图片
        /// <summary>    
        /// 向生成的二维码图片中增加中心图片  
        /// </summary>    
        /// <param name="qrImg">二维码图片</param> 
        /// <param name="actualSize">原二维码图片的实际内容大小</param>   
        /// <param name="backImg">要粘贴进来的图片</param>    
        /// <param name="zoomBackImg">是否缩放背景图片以适应二维码大小</param>    
        internal static void AddCenterImage(Image qrImg, int actualSize, string backImg, bool zoomBackImg)
        {
            Image img = Image.FromFile(backImg);
            if (zoomBackImg)
            {
                var maxSize = actualSize * 0.28;
                var minSize = actualSize * 0.22;
                if (img.Width < minSize || img.Width > maxSize)
                {
                    var destWidth = (int)(actualSize * 0.25);
                    var destHeight = (int)(img.Height / (img.Width + 0.0) * destWidth);
                    img = ResizeImage(img, destWidth, destHeight);
                }
            }
            using (Graphics g = Graphics.FromImage(qrImg))
            {
                g.DrawImage(img, qrImg.Width / 2 - img.Width / 2, qrImg.Width / 2 - img.Width / 2, img.Width, img.Height);
            }
        }
        #endregion

        #region 生成二维码
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="Content">内容文本</param>
        /// <param name="QRCodeEncodeMode">二维码编码方式</param>
        /// <param name="QRCodeErrorCorrect">纠错码等级</param>
        /// <param name="QRCodeVersion">二维码版本号 0-40,一般使用8</param>
        /// <param name="QRCodeScale">每个小方格的预设宽度（像素），正整数</param>
        /// <param name="size">图片尺寸（像素），0表示不设置</param>
        /// <returns></returns>
        internal static Image CreateQRCode(string Content,
            QRCodeEncoder.ENCODE_MODE QRCodeEncodeMode,
            QRCodeEncoder.ERROR_CORRECTION QRCodeErrorCorrect,
            int QRCodeVersion, int QRCodeScale, int size)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncodeMode;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeErrorCorrect;
            qrCodeEncoder.QRCodeScale = QRCodeScale;
            qrCodeEncoder.QRCodeVersion = QRCodeVersion;
            System.Drawing.Image image = qrCodeEncoder.Encode(Content, Encoding.UTF8);
            #region 根据设定的目标图片尺寸调整二维码QRCodeScale设置，并添加边框
            if (size > 0)
            {
                #region 当设定目标图片尺寸大于生成的尺寸时，逐步增大方格尺寸,直至图片尺寸达到接近size的最大值
                while (image.Width < size)
                {
                    qrCodeEncoder.QRCodeScale++;
                    System.Drawing.Image imageNew = qrCodeEncoder.Encode(Content, Encoding.UTF8);
                    if (imageNew.Width < size)
                    {
                        image = new Bitmap(imageNew);
                        imageNew.Dispose();
                        imageNew = null;
                    }
                    else
                    {
                        //新尺寸未采用，恢复最终使用的尺寸
                        qrCodeEncoder.QRCodeScale--;
                        imageNew.Dispose();
                        imageNew = null;
                        break;
                    }
                }
                #endregion

                #region 当设定目标图片尺寸小于生成的尺寸时，逐步减小方格尺寸
                while (image.Width > size && qrCodeEncoder.QRCodeScale > 1)
                {
                    qrCodeEncoder.QRCodeScale--;
                    System.Drawing.Image imageNew = qrCodeEncoder.Encode(Content, Encoding.UTF8);
                    image = new System.Drawing.Bitmap(imageNew);
                    imageNew.Dispose();
                    imageNew = null;
                    if (image.Width < size)
                    {
                        break;
                    }
                }
                #endregion
            }
            #endregion
            return image;
        }
        #endregion

        #region 调整图片的尺寸
        /// <summary>    
        /// Resize图片    
        /// </summary>    
        /// <param name="bmp">原始Bitmap</param>    
        /// <param name="newW">新的宽度</param>    
        /// <param name="newH">新的高度</param>    
        /// <returns>处理以后的图片</returns>    
        internal static Image ResizeImage(Image bmp, int newW, int newH)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                using (Graphics g = Graphics.FromImage(b))
                {
                    // 插值算法的质量    
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                    g.Dispose();
                }
                return b;
            }
            catch
            {
                return bmp;
            }
        }
        #endregion
    }
}
