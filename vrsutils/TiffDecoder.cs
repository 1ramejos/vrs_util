#nullable enable

//using ph;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace vrsutils;

[SupportedOSPlatform("windows6.1")]
public static class TiffDecoder
{
    public static Image DecodeTiffByte(Stream stream, string sessionKey, string iv, int keySize)
    {
        ArgumentNullException.ThrowIfNull(stream);
        EnsureWindowsImagingSupported();

        using var encryptedStream = new MemoryStream();
        stream.CopyTo(encryptedStream);

        byte[] cipherBytes = Decryptor.DecryptWithZeroPadding(
            encryptedStream.ToArray(),
            Encoding.UTF8.GetBytes(sessionKey),
            Encoding.UTF8.GetBytes(iv),
            keySize);

        using var imageStream = new MemoryStream(cipherBytes, writable: false);
        using var image = Image.FromStream(imageStream);
        var scaledImage = new Bitmap(480, 180);

        using (Graphics graphics = Graphics.FromImage(scaledImage))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawImage(image, 0, 0, 480, 180);
        }

        return scaledImage;
    }

    public static byte[] Readbytes(byte[] columnName)
    {
        ArgumentNullException.ThrowIfNull(columnName);
        return columnName;
    }

    public static Image BufferedImagetoImage(Bitmap image)
    {
        ArgumentNullException.ThrowIfNull(image);
        EnsureWindowsImagingSupported();
        return new Bitmap(image);
    }

    internal static Stream ConvertDecryptedImageToJpegStream(byte[] imageBytes)
    {
        EnsureWindowsImagingSupported();

        using var imageStream = new MemoryStream(imageBytes, writable: false);
        using var image = Image.FromStream(imageStream);
        var jpegStream = new MemoryStream();
        image.Save(jpegStream, ImageFormat.Jpeg);
        jpegStream.Position = 0;
        return jpegStream;
    }

    private static void EnsureWindowsImagingSupported()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            throw new PlatformNotSupportedException("Image decoding requires Windows 6.1 or later.");
        }
    }
}
