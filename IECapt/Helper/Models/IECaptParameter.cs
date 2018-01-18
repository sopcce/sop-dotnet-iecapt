using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IECapt.Model
{
  #region Open a command prompt and ask for help
  // Open a command prompt and ask for help:
  //C:\> IECapt --help
  // -----------------------------------------------------------------------------
  // Usage: IECapt --url=http://www.example.org/ --out=localfile.png
  // -----------------------------------------------------------------------------
  //  --help                      Print this help page and exit
  //  --url=<url>                 The URL to capture (http:...|file:...|...)
  //  --out=<path>                The target file (.png|bmp|jpeg|emf|...)
  //  --min-width=<int>           Minimal width for the image (default: 800)
  //  --max-wait=<ms>             Don't wait more than (default: 90000, inf: 0)
  //  --delay=<ms>                Wait after loading (e.g. for Flash; default: 0)
  //  --silent                    Whether to surpress some dialogs
  // -----------------------------------------------------------------------------
  // http://iecapt.sf.net - (c) 2003-2008 Bjoern Hoehrmann - <bjoern@hoehrmann.de>"/> 
  #endregion

  /// <summary>
  /// IECapt  
  /// </summary>
  public class IECaptParameter
  {
    public IECaptParameter()
    {
      Min_width = 800;
      Max_wait = 0;
      Delay = 0;
    }

    /// <summary>
    /// The URL to capture (http:...|file:...|...)
    /// 要捕获的URL
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    ///   The target file (.png|bmp|jpeg|emf|...)
    /// 目标文件
    /// </summary>
    public string Out { get; set; }
    /// <summary>
    ///  Minimal width for the image (default: 800)
    ///  图像的最小宽度
    /// </summary>
    public int Min_width { get; set; }
    /// <summary>
    ///  Wait after loading (e.g. for Flash; default: 0)
    /// </summary>
    public int Max_wait { get; set; }
    /// <summary>
    ///  Wait after loading (e.g. for Flash; default: 0)
    /// 等待加载后  
    /// </summary>
    public int Delay { get; set; }
  }
}