//using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.IO;

namespace vrsutils
{
    internal sealed class mod_Image
    {
        public static readonly string FingerPrintImageFilename = "fingerPrintImage.bmp";

        public static readonly string FingerPrintImageToCompare1 = "fingerPrintImageSource1.bmp";

        public static readonly string FingerPrintImageToCompare2 = "fingerPrintImageSource2.bmp";

        public static byte[] getImage(string fname)
        {
            byte[] result;
            try
            {
                byte[] array = null;
                FileInfo fileInfo = new FileInfo(fname);
                long length = fileInfo.Length;
                FileStream fileStream = new FileStream(fname, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                array = binaryReader.ReadBytes(Convert.ToInt32(length));
                fileInfo = null;
                length = 0L;
                fileStream.Close();
                fileStream.Dispose();
                binaryReader.Close();
                result = array;
            }
            catch //(Exception ex)
            {
                // ProjectData.SetProjectError(ex);
                result = null;
                //  ProjectData.ClearProjectError();
            }
            return result;
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream memoryStream = new MemoryStream(byteArrayIn);
            Image result = Image.FromStream(memoryStream);
            memoryStream.Dispose();
            memoryStream.Close();
            return result;
        }
    }
}