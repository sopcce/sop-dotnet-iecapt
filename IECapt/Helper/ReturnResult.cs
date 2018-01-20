using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capt.Helper
{
  public class ReturnResult<T>
  {
    /// <summary>
    /// 返回结果
    /// </summary>
    public T Data { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    public string Msg { get; set; }
    /// <summary>
    /// 检索耗时
    /// </summary>
    public long QTime { get; set; }
    /// <summary>
    /// 状态码
    /// </summary>
    public CaptStatus Status { get; set; }
  }
}