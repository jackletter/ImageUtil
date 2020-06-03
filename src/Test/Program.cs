using System;
using System.Drawing.Imaging;
using System.IO;

namespace Test.Standard
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestBarcode();
            //TestQRCode();
            //TestValicode();
            //TestImageProcess();
            Console.WriteLine("ok");
            Console.ReadLine();
        }

        private static void TestImageProcess()
        {
            var jpgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImageProcess.jpg");
            var srcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timg.jpg");
            var process = new ImageUtil.ImageProcess(srcPath);
            process.Thumbnail(100).Save(jpgPath, ImageFormat.Jpeg);
        }

        private static void TestValicode()
        {
            var pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestValicode.png");
            Console.WriteLine(ImageUtil.ValidateCodeUtil.Generate(pngPath));
        }

        private static void TestQRCode()
        {
            var pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestQRCode.png");
            ImageUtil.QRCodeUtil.Encode("https://github.com/jackletter/ImageUtil", pngPath);
        }

        private static void TestBarcode()
        {
            var pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestBarcode.png");
            ImageUtil.BarCodeUtil.Encode("https://github.com/jackletter/ImageUtil", pngPath);
        }
    }
}
