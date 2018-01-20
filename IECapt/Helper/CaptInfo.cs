using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capt.Helper
{


  /// <summary>
  /// IECapt  
  /// </summary>
  public class CaptInfo
  {
    public CaptInfo()
    {
      Min_width = 800;
      Max_wait = 0;
      Delay = 0;
    }
    public IECaptOrCutyCapt CaptType { get; set; }
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
  /// <summary>
  /// 
  /// </summary>
  public enum CaptStatus
  {
    /// <summary>
    /// 错误
    /// </summary>
    Error = -1,
    ErrorNotOutFile = -2,
    ErrorException = -2,
    /// <summary>
    /// 成功
    /// </summary>
    Success = 1,

  }
  public enum IECaptOrCutyCapt
  {
    IECapt = 0,
    CutyCapt = 1,
  }
}