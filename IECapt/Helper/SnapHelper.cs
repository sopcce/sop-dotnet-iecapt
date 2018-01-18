using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace IECapt.Helper
{
  /// <summary>
  ///     图片辅助类
  /// </summary>
  public class SnapHelper
  {
    static string url;
    static string savePath;
    static string filePath;
    static string imageName;
    static string imagePath;

    static int imageWidth;
    static int imageMaxHeigth;


    public static string GetWebSiteThumbnail(string link)
    {
      //   ImageHelper thumbnailGenerator = new ImageHelper(url, savePath, imageType);          
      url = (link.IndexOf("http://") > -1 || link.IndexOf("https://") > -1) ? link : "http://" + link;

      filePath = "/ImageFiles/" + DateTime.Now.ToString("yyyyMMdd") + "/";
      var path = System.Web.HttpContext.Current.Server.MapPath("~" + filePath);
      if (!System.IO.Directory.Exists(path))
      {
        System.IO.Directory.CreateDirectory(path);//不存在则创建文件夹 
      }

      imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
      savePath = Path.Combine(path, imageName);

      imageWidth = 1000;
      imageMaxHeigth = 10000;  //最大高度

      GenerateWebSiteThumbnailImage();
      return imagePath;
    }
    public static void GenerateWebSiteThumbnailImage()
    {
      Thread m_thread = new Thread(new ThreadStart(SaveUrlToImage));
      m_thread.SetApartmentState(ApartmentState.STA);
      m_thread.Start();
      m_thread.Join();
    }

    /// <summary>
    ///     将Url地址保存为图片
    /// </summary>
    /// <param name="url">网页路径</param>
    /// <param name="savePath">图片存放路径</param>
    /// <param name="imageType">图片类型</param>
    /// <returns></returns>
    public static void SaveUrlToImage()
    {
      var uri = new Uri(url);

      using (var bit = GetHtmlImage(uri, imageWidth))  //Screen.PrimaryScreen.Bounds.Width
      {
        if (bit == null)
        {
          imagePath = "";
          return;
        }
        Image image = bit;


        bit.Save(savePath, ImageFormat.Jpeg);
        imagePath = filePath + imageName;


      }
    }

    protected static Bitmap GetHtmlImage(Uri urlString, int width)
    {

      using (var control = new WebBrowser { Size = new Size(width, 10), Url = urlString, ScriptErrorsSuppressed = true })
      {
        int count = 0;
        while (control.ReadyState != WebBrowserReadyState.Complete)
        {
          //if (count > 1000 && control.ReadyState == WebBrowserReadyState.Interactive)
          //{//1000次后还是没加载完成，并且该控件已加载足够的文档以允许有限的用户交互，比如单击已显示的超链接。                        
          //    break;  //中断循环，执行截图操作
          //}
          //if (control.ReadyState == WebBrowserReadyState.Interactive)
          //{
          //    count++;
          //}

          Application.DoEvents();
        }
        if (control.Document != null)
        {
          if (control.Document.Body != null)
          {
            // control.Height = control.Document.Body.ScrollRectangle.Height+20;
            //图片太大，超过内存时会报错
            control.Height = control.Document.Body.ScrollRectangle.Height > imageMaxHeigth ? imageMaxHeigth : control.Document.Body.ScrollRectangle.Height;

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


          var bitmap = new Bitmap(bmpRect.Width, bmpRect.Height);  //图片太大，超过内存时会报错

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
            return null;
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