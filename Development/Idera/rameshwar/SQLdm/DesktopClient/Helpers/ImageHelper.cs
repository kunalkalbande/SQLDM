using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

using BBS.TracerX;
using Svg;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    internal class ImageHelper
    {
        private static readonly Logger Log = Logger.GetLogger("ImageHelper");

        internal static byte[] ConvertImageToByteArray(Image imageToConvert)
        {
            return ConvertImageToByteArray(imageToConvert, ImageFormat.Png);
        }

        internal static byte[] ConvertImageToByteArray(Image imageToConvert, ImageFormat formatOfImage)
        {
            byte[] Ret;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageToConvert.Save(ms, formatOfImage);
                    Ret = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to convert image to byte array", ex);
                throw;
            }
            return Ret;
        }

        internal static Image ConvertByteArrayToImage(Byte[] bytes)
        {
            Image image = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to convert byte array to image", ex);
                throw;
            }
            return image;
        }

        internal static Bitmap GetBitmapFromSvgByteArray(byte[] svg)
        {
            using (var stream = new MemoryStream(svg))
            {
                return SvgDocument.Open<SvgDocument>(stream).Draw();
            }
        }
    }
}
