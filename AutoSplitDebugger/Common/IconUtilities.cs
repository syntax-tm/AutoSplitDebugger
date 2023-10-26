using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoSplitDebugger;

internal static class IconUtilities
{
    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteObject(IntPtr hObject);

    public static ImageSource FromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (!File.Exists(fileName)) throw new FileNotFoundException($"Image file '{fileName}' does not exist.",  fileName);

        var image = new Icon(fileName);

        return ToImageSource(image);
    }

    //public static ImageSource ToImageSource(this Icon icon)
    //{            
    //    var bitmap = icon.ToBitmap();
    //    var hBitmap = bitmap.GetHbitmap();

    //    var imgSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
    //                                                          BitmapSizeOptions.FromEmptyOptions());

    //    if (!DeleteObject(hBitmap))
    //    {
    //        throw new Win32Exception();
    //    }

    //    return imgSource;
    //}

    public static ImageSource ToImageSource(this Icon icon)
    {
        var imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty,
                                                              BitmapSizeOptions.FromEmptyOptions());

        return imageSource;
    }
}