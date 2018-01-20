#region CutyCapt
//Open a command prompt and ask for help:

// % CutyCapt --help
// -----------------------------------------------------------------------------
// Usage: CutyCapt --url=http://www.example.org/ --out=localfile.png            
// -----------------------------------------------------------------------------
//  --help Print this help page and exit                
//  --url=< url > The URL to capture (http:...|file:...|...)   
//  --out=<path>                   The target file (.png|pdf|ps|svg|jpeg|...)   
//  --out-format=<f>               Like extension in --out, overrides heuristic 
//  --min-width=<int>              Minimal width for the image (default: 800)   
//  --min-height=<int>             Minimal height for the image (default: 600)  
//  --max-wait=<ms>                Don't wait more than (default: 90000, inf: 0)
//  --delay=<ms>                   After successful load, wait (default: 0)     
//  --user-style-path=<path>       Location of user style sheet file, if any    
//  --user-style-string=<css>      User style rules specified as text           
//  --header=<name>:<value>        request header; repeatable; some can't be set
//  --method=<get| post | put > Specifies the request method(default: get)
//  --body - string =< string > Unencoded request body(default: none)       
//  --body - base64 =< base64 > Base64 - encoded request body(default: none)  
//  --app - name =< name > appName used in User - Agent;
//   default is none
//  --app - version =< version > appVers used in User - Agent;
//   default is none
//  --user - agent =< string > Override the User-Agent header Qt would set
//  --javascript =< on | off > JavaScript execution(default: on)
//  --java =< on | off > Java execution(default: unknown)
//  --plugins =< on | off > Plugin execution(default: unknown)
//  --private-browsing=<on|off>    Private browsing(default: unknown)
//  --auto-load-images=<on|off>    Automatic image loading(default: on)
//  --js-can-open-windows=<on|off> Script can open windows? (default: unknown)  
//  --js-can-access-clipboard=<on|off> Script clipboard privs(default: unknown)
//  --print-backgrounds=<on|off>   Backgrounds in PDF/PS output(default: off)
//  --zoom-factor=<float>          Page zoom factor(default: no zooming)
//  --zoom-text-only=<on|off>      Whether to zoom only the text(default: off)
//  --http-proxy=<url>             Address for HTTP proxy server(default: none)
// -----------------------------------------------------------------------------
//  <f> is svg,ps,pdf,itext,html,rtree,png,jpeg,mng,tiff,gif,bmp,ppm,xbm,xpm    
// -----------------------------------------------------------------------------
// http://cutycapt.sf.net - (c) 2003-2013 Bjoern Hoehrmann - bjoern@hoehrmann.de 
#endregion
#region IECapt
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





using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Capt.Helper
{

  public class CaptHelper
  {
    /// <summary>
    /// 执行截图操作
    /// </summary>
    /// <param name="url">网页链接，example:"https://www.baidu.com/"</param> 
    /// <returns></returns>
    public static ReturnResult<string> Execute(string url, IECaptOrCutyCapt type = IECaptOrCutyCapt.IECapt)
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
      var data = Execute(new CaptInfo() { Url = url, Out = completePath, CaptType = type });
      data.Data = completePath;
      return data;
    }
    /// <summary>
    /// 执行截图操作
    /// </summary>
    /// <param name="url"></param>
    /// <param name="path">物理路径，</param>
    /// <returns></returns>
    public static ReturnResult<string> Execute(string url, string path,
      IECaptOrCutyCapt type = IECaptOrCutyCapt.IECapt)
    {
      if (string.IsNullOrEmpty(url))
      {
        return new ReturnResult<string>() { Msg = "url 为空" };
      }
      url = (url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) > -1 ||
        url.IndexOf("https://", StringComparison.OrdinalIgnoreCase) > -1) ? url : "http://" + url;

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      string fileName = Guid.NewGuid().ToString("N") + ".png";
      string completePath = Path.Combine(path, fileName);
      var data = Execute(new CaptInfo() { Url = url, Out = completePath, CaptType = type });
      data.Data = completePath;
      return data;
    }
    /// <summary>
    /// 执行输出快照
    /// </summary>
    /// <param name="info">CaptInfo</param>
    /// <returns></returns>
    public static ReturnResult<string> Execute(CaptInfo info)
    {
      string output = string.Empty;
      Stopwatch sw = Stopwatch.StartNew();
      string root = string.Empty;
      if (info.CaptType == IECaptOrCutyCapt.IECapt)
      {
        root = AppDomain.CurrentDomain.BaseDirectory + @"Lib\\IECapt";
        if (!File.Exists(root + "\\IECapt.exe"))
          throw new FileNotFoundException("IECapt.exe file can't be found .");
      }
      else
      {
        root = AppDomain.CurrentDomain.BaseDirectory + @"Lib\\CutyCapt";
        if (!File.Exists(root + "\\CutyCapt.exe"))
          throw new FileNotFoundException("IECapt.exe file can't be found .");
      }

      using (var process = new Process())
      {
        try
        {
          process.StartInfo.WorkingDirectory = root;
          process.StartInfo.FileName = "cmd.exe";
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardInput = true;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.CreateNoWindow = true;

          //process.StartInfo.CreateNoWindow = false;
          process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
          process.Start();
          string value = string.Format(@"{0} --url={1} --out={2} --min-width={3} --max-wait={4} --delay={5} --silent",
            info.CaptType == IECaptOrCutyCapt.IECapt ? "iecapt" : "cutycapt", //输出方式
            info.Url,  //输入路径网站
            info.Out,   //输出
            info.Min_width,
            info.Max_wait,
            info.Delay);
          process.StandardInput.WriteLine(value);
          process.StandardInput.WriteLine("exit");
          process.WaitForExit(120000);  //2分钟失效 
          output = process.StandardOutput.ReadToEnd();

        }
        catch (Exception ex)
        {
          return new ReturnResult<string>()
          {
            Status = CaptStatus.ErrorException,
            Msg = "快照失败:" + ex.Message
          };
        }
        finally
        {
          sw.Stop();
          if (!process.HasExited)
          {
            process.Kill();
          }
          process.Close();
          process.Dispose();


        }
        if (System.IO.File.Exists(info.Out))
        {
          return new ReturnResult<string>()
          {
            Status = CaptStatus.Success,
            QTime = sw.ElapsedMilliseconds,
            Msg = "快照生产成功：" + output
          };
        }
        else
        {
          return new ReturnResult<string>()
          {
            Status = CaptStatus.ErrorNotOutFile,
            Msg = "快照失败,文件不存在"
          };
        }


      }
    }

  }
}