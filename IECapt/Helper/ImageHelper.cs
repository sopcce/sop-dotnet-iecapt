using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Web;
using System.Windows.Forms;

namespace IECapt.Helper
{

  /// <summary>
  ///     图片辅助类
  /// </summary>
  public class ImageHelper
  {
    public static ReturnResult<string> Execute(string url)
    {
      if (string.IsNullOrEmpty(url))
      {
        return new ReturnResult<string>() { Msg = "url 为空" };
      }
      url = (url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) > -1 ||
        url.IndexOf("https://", StringComparison.OrdinalIgnoreCase) > -1) ? url : "http://" + url;
      var path = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\Image";
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
      string fileName = Guid.NewGuid().ToString("N") + ".png";
      string completePath = Path.Combine(path, fileName);

      var data = Execute(url, completePath,ImageFormat.Png);
      data.Data = completePath;
      return data;
    }


    /// <summary>
    ///     将Url地址保存为图片
    /// </summary>
    /// <param name="url">网页路径</param>
    /// <param name="savePath">图片存放路径</param>
    /// <param name="imageType">图片类型</param>
    /// <returns></returns>
    public static ReturnResult<string> Execute(string url, string savePath,
      ImageFormat imageType = null)
    {
      if (imageType == null)
        imageType = ImageFormat.Jpeg;
      var uri = new Uri(url);
      using (var bit = GetHtmlImage(uri, Screen.PrimaryScreen.Bounds.Width))
      {
        bit.Save(savePath, imageType);
        return new ReturnResult<string>() { Msg = "url 为空", Data = savePath, Status = IECaptStatus.Success };
      }
    }

    protected static Bitmap GetHtmlImage(Uri urlString, int width)
    {
      using (var control = new WebBrowser
      {
        Size = new Size(width, 10),
        Url = urlString,
        ScriptErrorsSuppressed = true
      })
      {
        while (control.ReadyState != WebBrowserReadyState.Complete)
        {
          Application.DoEvents();
        }
        if (control.Document != null)
        {
          if (control.Document.Body != null)
          {
            control.Height = control.Document.Body.ScrollRectangle.Height + 20;
          }
        }
        control.Url = urlString;
        var snap = new WebControlImage.Snapshot();
        var bitmap = snap.TakeSnapshot(control.ActiveXInstance, new Rectangle(0, 0, control.Width, control.Height));
        control.Dispose();
        return bitmap;
      }
    }

    /// WebBrowser获取图形
    protected class WebControlImage
    {
      internal static class NativeMethods
      {
        [StructLayout(LayoutKind.Sequential)]
        public sealed class TagDvtargetdevice
        {
          [MarshalAs(UnmanagedType.U4)]
          public int tdSize;
          [MarshalAs(UnmanagedType.U2)]
          public short tdDriverNameOffset;
          [MarshalAs(UnmanagedType.U2)]
          public short tdDeviceNameOffset;
          [MarshalAs(UnmanagedType.U2)]
          public short tdPortNameOffset;
          [MarshalAs(UnmanagedType.U2)]
          public short tdExtDevmodeOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Comrect
        {
          public int left;
          public int top;
          public int right;
          public int bottom;

          public Comrect()
          {
          }

          public Comrect(Rectangle r)
          {
            left = r.X;
            top = r.Y;
            right = r.Right;
            bottom = r.Bottom;
          }

          public Comrect(int left, int top, int right, int bottom)
          {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
          }

          public static Comrect FromXywh(int x, int y, int width, int height)
          {
            return new Comrect(x, y, x + width, y + height);
          }

          public override string ToString()
          {
            return string.Concat("Left = ", left, " Top ", top, " Right = ", right, " Bottom = ", bottom);
          }
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class TagLogpalette
        {
          [MarshalAs(UnmanagedType.U2)]
          public short palVersion;
          [MarshalAs(UnmanagedType.U2)]
          public short palNumEntries;
        }
      }

      public class Snapshot
      {
        /// 图象大小
        public Bitmap TakeSnapshot(object pUnknown, Rectangle bmpRect)
        {
          if (pUnknown == null)
            return null;
          //必须为com对象 
          if (!Marshal.IsComObject(pUnknown))
            return null;
          //IViewObject 接口 
          IntPtr viewObject;
          //内存图 
          var bitmap = new Bitmap(bmpRect.Width, bmpRect.Height);
          var image = Graphics.FromImage(bitmap);
          //获取接口 
          object hret = Marshal.QueryInterface(Marshal.GetIUnknownForObject(pUnknown),
              ref UnsafeNativeMethods.IidIViewObject, out viewObject);
          try
          {
            var o =
                Marshal.GetTypedObjectForIUnknown(viewObject, typeof(UnsafeNativeMethods.IViewObject)) as
                    UnsafeNativeMethods.IViewObject;
            //调用Draw方法 
            if (o != null)
              o.Draw((int)DVASPECT.DVASPECT_CONTENT,
                  -1,
                  IntPtr.Zero,
                  null,
                  IntPtr.Zero,
                  image.GetHdc(),
                  new NativeMethods.Comrect(bmpRect),
                  null,
                  IntPtr.Zero,
                  0);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            throw;
          }
          //释放 
          image.Dispose();
          return bitmap;
        }
      }

      [SuppressUnmanagedCodeSecurity]
      internal static class UnsafeNativeMethods
      {
        public static Guid IidIViewObject = new Guid("{0000010d-0000-0000-C000-000000000046}");

        [ComImport, Guid("0000010d-0000-0000-C000-000000000046"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IViewObject
        {
          [PreserveSig]
          int Draw([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect,
              [In] NativeMethods.TagDvtargetdevice ptd, IntPtr hdcTargetDev, IntPtr hdcDraw,
              [In] NativeMethods.Comrect lprcBounds, [In] NativeMethods.Comrect lprcWBounds,
              IntPtr pfnContinue,
              [In] int dwContinue);

          [PreserveSig]
          int GetColorSet([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect,
              [In] NativeMethods.TagDvtargetdevice ptd, IntPtr hicTargetDev,
              [Out] NativeMethods.TagLogpalette ppColorSet);

          [PreserveSig]
          int Freeze([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect,
              [Out] IntPtr pdwFreeze);

          [PreserveSig]
          int Unfreeze([In, MarshalAs(UnmanagedType.U4)] int dwFreeze);

          void SetAdvise([In, MarshalAs(UnmanagedType.U4)] int aspects,
              [In, MarshalAs(UnmanagedType.U4)] int advf,
              [In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink);

          void GetAdvise([In, Out, MarshalAs(UnmanagedType.LPArray)] int[] paspects,
              [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] advf,
              [In, Out, MarshalAs(UnmanagedType.LPArray)] IAdviseSink[] pAdvSink);
        }
      }
    }
  }
}