using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoSplitDebugger;

public static class Extensions
{
    public static ImageSource ToImageSource(this Image image)
    {
        using var ms = new MemoryStream();

        image.Save(ms, ImageFormat.Bmp);
        ms.Seek(0, SeekOrigin.Begin);

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = ms;
        bitmapImage.EndInit();

        return bitmapImage;
    }

    public static T FindChild<T>(this DependencyObject control)
        where T : DependencyObject
    {
        if (control == null) throw new ArgumentNullException(nameof(control));
 
        T foundChild = null;
 
        var childrenCount = VisualTreeHelper.GetChildrenCount(control);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(control, i);
            if (child is not T childType)
            {
                // recursively search for each child control
                foundChild = FindChild<T>(child);
 
                if (foundChild != null) break;
            }
            else
            {
                foundChild = childType;
                break;
            }
        }
 
        return foundChild;
    }

    public static T FindChild<T>(this DependencyObject control, string childName)
        where T : DependencyObject
    {
        if (control == null) throw new ArgumentNullException(nameof(control));
        if (string.IsNullOrEmpty(childName)) throw new ArgumentNullException(nameof(childName));
 
        T foundChild = null;
 
        var childrenCount = VisualTreeHelper.GetChildrenCount(control);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(control, i);

            if (child is not T childType)
            {
                // recursively search for each child control
                foundChild = FindChild<T>(child, childName);
 
                if (foundChild != null) break;
            }
            else
            {
                if (childType is not FrameworkElement frameworkElement) continue;
                if (frameworkElement.Name != childName) continue;

                foundChild = childType;
                break;
            }
        }
 
        return foundChild;
    }

    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null) yield return (T) Enumerable.Empty<T>();

        Debug.Assert(depObj != null);

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var ithChild = VisualTreeHelper.GetChild(depObj, i);
            if (ithChild is T t) yield return t;

            foreach (var childOfChild in FindVisualChildren<T>(ithChild))
            {
                yield return childOfChild;
            }
        }
    }
}
