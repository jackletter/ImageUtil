using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Cryptography;
using System.Web;
using System.IO;
using System.Threading;
using System.Data;

namespace ImageUtil
{
    #region 随机生成数字或字母
    /// <summary>
    /// 随机生成数字或字母
    /// </summary>
    internal class Rand
    {
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
        private static byte[] randb = new byte[4];

        #region 获得下一个随机数
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        public static int Next(int max)
        {
            rand.GetBytes(randb);
            int value = BitConverter.ToInt32(randb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }
        #endregion

        #region 根据最大值和最小值随机生成一个数
        /// <summary>
        /// 根据最大值和最小值随机生成一个数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int Next(int min, int max)
        {
            var num = Next(max);
            while (min > num)
            {
                num = Next(max);
            }
            return num;
        }
        #endregion

        #region 生成随机(数字)
        /// <summary>
        /// 生成随机(数字)
        /// </summary>
        /// <param name="len">生成长度</param>
        public static string RandNumber(int len)
        {
            return RandNumber(len, false);
        }

        /// <summary>
        /// 生成随机(数字)
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <param name="isSleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string RandNumber(int len, bool isSleep)
        {
            if (isSleep) System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < len; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        #endregion

        #region 生成随机(字母)
        /// <summary>
        /// 生成随机(字母)
        /// </summary>
        /// <param name="len">生成长度</param>
        public static string RandString(int len)
        {
            return RandString(len, false);
        }

        /// <summary>
        /// 生成随机(字母)
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <param name="isSleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string RandString(int len, bool isSleep = false)
        {
            if (isSleep) System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'g', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'v', 'z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < len; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }
        #endregion

        #region 生成随机(字母+数字)
        /// <summary>
        /// 生成随机(字母+数字)
        /// </summary>
        /// <param name="len">生成长度</param>
        public static string RandStringAndNumber(int len)
        {
            return RandStringAndNumber(len, false);
        }

        /// <summary>
        /// 生成随机(字母+数字)
        /// </summary>
        /// <param name="len">生成长度</param>
        /// <param name="isSleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string RandStringAndNumber(int len, bool isSleep)
        {
            if (isSleep) System.Threading.Thread.Sleep(3);
            var list = new List<char>();
            var digits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var uppers = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var lowers = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'g', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'v', 'z' };
            list.AddRange(digits);
            list.AddRange(digits);
            list.AddRange(uppers);
            list.AddRange(digits);
            list.AddRange(digits);
            list.AddRange(lowers);
            list.AddRange(digits);
            list.AddRange(digits);
            string result = "";
            var n = list.Count;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < len; i++)
            {
                int rnd = random.Next(0, n);
                result += list[rnd];
            }
            return result;
        }
        #endregion
    }
    #endregion

    #region 验证码难度枚举
    /// <summary>
    /// 验证码难度
    /// </summary>
    public enum EnumLevel
    {
        /// <summary>
        /// 低
        /// </summary>
        Low,
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 高
        /// </summary>
        High
    }
    #endregion

    #region 生成随机验证码的选项
    /// <summary>
    /// 生成随机验证码的选项
    /// </summary>
    public class ValidateCodeOption
    {
        /// <summary>
        /// 验证码难度
        /// </summary>
        public EnumLevel Level { set; get; }

        /// <summary>
        /// 是否混合字母
        /// </summary>
        public bool MixLetters { set; get; }

        /// <summary>
        /// 指定的随机验证码
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// 验证码位数
        /// </summary>
        public int LetterCount { set; get; }

        /// <summary>
        /// 单个字体的高度范围
        /// </summary>
        public int LetterHeight { set; get; }

        /// <summary>
        /// 字体范围
        /// </summary>
        internal List<Font> Fonts
        {
            get
            {
                return new List<Font>()
                    {
                        new Font(new FontFamily("Times New Roman"),this.LetterHeight + Rand.Next(-2,2), System.Drawing.FontStyle.Regular),
                        new Font(new FontFamily("Georgia"), this.LetterHeight + Rand.Next(-2,2), System.Drawing.FontStyle.Regular),
                        new Font(new FontFamily("Arial"), this.LetterHeight + Rand.Next(-2,2), System.Drawing.FontStyle.Regular),
                        new Font(new FontFamily("Comic Sans MS"), this.LetterHeight + Rand.Next(-2,2), System.Drawing.FontStyle.Regular)
                    };
            }
        }

        /// <summary>
        /// 默认配置
        /// </summary>
        public static ValidateCodeOption Default
        {
            get
            {
                return new ValidateCodeOption()
                {
                    Level = EnumLevel.Normal,
                    MixLetters = false,
                    LetterCount = 4,
                    LetterHeight = 20
                };
            }
        }
    }
    #endregion

    #region 验证图片类对外接口
    /// <summary>
    /// 验证图片类
    /// </summary>
    public class ValidateCodeUtil
    {
        #region 内部实现绘制图片
        /// <summary>
        /// 配置
        /// </summary>
        internal ValidateCodeOption Option { set; get; }

        /// <summary>
        /// 图片
        /// </summary>
        internal Bitmap Bitmap { set; get; }

        /// <summary>
        /// 字符串
        /// </summary>
        internal string Text { set; get; }
        internal ValidateCodeUtil()
        {
            Option = ValidateCodeOption.Default;
            Text = Rand.RandNumber(4);
        }
        internal ValidateCodeUtil(ValidateCodeOption option)
        {
            Option = option;
            if (!string.IsNullOrWhiteSpace(option.Text))
            {
                Text = option.Text;
            }
            else
            {
                if (option.MixLetters)
                {
                    Text = Rand.RandStringAndNumber(option.LetterCount);
                }
                else
                {
                    Text = Rand.RandNumber(option.LetterCount);
                }
            }
        }

        /// <summary>
        /// 绘制验证码
        /// </summary>
        internal void CreateImage()
        {
            int int_ImageWidth = this.Text.Length * (Option.LetterHeight / 5 * 4);
            Bitmap image = new Bitmap(int_ImageWidth, Option.LetterHeight + 20);
            Graphics g = Graphics.FromImage(image);
            //整个将画布用白色填充
            g.Clear(Color.White);
            //根据Level值计算需要画的随机线的条数
            int lineCount = 0;
            int pointCount = 0;
            if (Option.Level == EnumLevel.Low)
            {
                lineCount = 5;
                pointCount = 10;
            }
            else if (Option.Level == EnumLevel.Normal)
            {
                lineCount = 9;
                pointCount = 25;
            }
            else if (Option.Level == EnumLevel.High)
            {
                lineCount = 14;
                pointCount = 35;
            }
            for (int i = 0; i < lineCount; i++)
            {
                int x1 = Rand.Next(image.Width - 1);
                int x2 = Rand.Next(image.Width - 1);
                int y1 = Rand.Next(image.Height - 1);
                int y2 = Rand.Next(image.Height - 1);
                g.DrawLine(new Pen(GetLineRandomColor()), x1, y1, x2, y2);
            }
            //将生成的随机数随机画在画布上
            int _x = -12, _y = 0;
            for (int int_index = 0; int_index < this.Text.Length; int_index++)
            {
                _x += Rand.Next(12, Option.LetterHeight / 5 * 4);
                _y = Rand.Next(-2, 2);
                string str_char = this.Text.Substring(int_index, 1);
                Brush newBrush = new SolidBrush(GetFontRandomColor());
                Point thePos = new Point(_x, _y);
                g.DrawString(str_char, Option.Fonts[Rand.Next(Option.Fonts.Count - 1)], newBrush, thePos);
            }
            //根据Level值计算添加的像素个数
            for (int i = 0; i < pointCount; i++)
            {
                int x = Rand.Next(image.Width - 1);
                int y = Rand.Next(image.Height - 1);
                image.SetPixel(x, y, Color.FromArgb(Rand.Next(0, 255), Rand.Next(0, 255), Rand.Next(0, 255)));
            }
            image = TwistImage(image, true, Rand.Next(1, 3), Rand.Next(4, 6));
            g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, int_ImageWidth - 1, (Option.LetterHeight - 1));
            this.Bitmap = image;
        }

        private static List<Color> fontColors = new List<Color>()
        {
            Color.Black,Color.Red,
            Color.Blue,Color.DeepSkyBlue
        };
        private static List<Color> lineColors = new List<Color>()
        {
            Color.Silver,Color.Pink,
            Color.Blue,Color.DeepSkyBlue,
            Color.Aqua,Color.Cornsilk
        };

        /// <summary>
        /// 字体随机颜色
        /// </summary>
        internal Color GetFontRandomColor()
        {
            int index = Rand.Next(fontColors.Count - 1);
            return fontColors[index];
        }

        /// <summary>
        /// 字体随机颜色
        /// </summary>
        internal Color GetLineRandomColor()
        {
            int index = Rand.Next(fontColors.Count - 1);
            return fontColors[index];
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        /// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        internal System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            double PI = 6.283185307179586476925286766559;
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            srcBmp.Dispose();
            return destBmp;
        }
        #endregion

        #region 对外接口:生成验证码
        /// <summary>
        /// 快捷生成验证码
        /// </summary>
        /// <param name="filePath">生成的图片的存放路径</param>
        /// <returns>生成的验证字符串</returns>
        public static string Generate(string filePath)
        {
            ValidateCodeUtil helper = new ValidateCodeUtil();
            helper.CreateImage();
            helper.Bitmap.Save(filePath);
            return helper.Text;
        }

        /// <summary>
        /// 快捷生成验证码
        /// </summary>
        /// <param name="filePath">生成的图片的存放路径</param>
        /// <param name="option">配置</param>
        /// <returns>生成的验证字符串</returns>
        public static string Generate(string filePath, ValidateCodeOption option)
        {
            ValidateCodeUtil helper = new ValidateCodeUtil(option);
            helper.CreateImage();
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                lock (typeof(ValidateCodeUtil))
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
            }
            helper.Bitmap.Save(filePath);
            return helper.Text;
        }
        #endregion
    }
    #endregion
}