using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace IECapt.Helper
{

  public class IECaptHelper
  {
    /// <summary>
    /// 执行截图操作
    /// </summary>
    /// <param name="url">网页链接，example:"https://www.baidu.com/"</param> 
    /// <returns></returns>
    public static ReturnResult<string> Execute(string url)
    {
      if (string.IsNullOrEmpty(url))
      {
        return new ReturnResult<string>() { Msg = "url 为空" };
      }
      url = (url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) > -1 ||
        url.IndexOf("https://", StringComparison.OrdinalIgnoreCase) > -1) ? url : "http://" + url;
      var path = AppDomain.CurrentDomain.BaseDirectory + "TempFiles/Image";
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
      string fileName = Guid.NewGuid().ToString("N") + ".png";
      string completePath = Path.Combine(path, fileName);
      var data = Execute(new IECaptParameter() { Url = url, Out = completePath });
      data.Data = completePath; 
      return data;
    }
    /// <summary>
    /// 执行截图操作
    /// </summary>
    /// <param name="url"></param>
    /// <param name="path">物理路径，</param>
    /// <returns></returns>
    public static ReturnResult<string> Execute(string url,string path)
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
      var data = Execute(new IECaptParameter() { Url = url, Out = completePath });
      data.Data = completePath;
      return data;
    }
    /// <summary>
    /// 执行输出快照
    /// </summary>
    /// <param name="ieCapt">IECaptParameter</param>
    /// <returns></returns>
    public static ReturnResult<string> Execute(IECaptParameter ieCapt)
    {
      string output = string.Empty;
      Stopwatch sw = Stopwatch.StartNew();
      var root = AppDomain.CurrentDomain.BaseDirectory + @"Lib";
      if (!File.Exists(root + "//IECapt.exe"))
      {
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
          string value = string.Format(@" iecapt --url={0} --out={1} --min-width={2} --max-wait={3} --delay={4} --silent", ieCapt.Url, ieCapt.Out, ieCapt.Min_width, ieCapt.Max_wait, ieCapt.Delay);
          process.StandardInput.WriteLine(value);
          process.StandardInput.WriteLine("exit");
          process.WaitForExit(120000);  //2分钟失效 
          output = process.StandardOutput.ReadToEnd();

        }
        catch (Exception ex)
        {
          return new ReturnResult<string>()
          {
            Status = IECaptStatus.ErrorException,
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
        if (System.IO.File.Exists(ieCapt.Out))
        {
          return new ReturnResult<string>()
          {
            Status = IECaptStatus.Success,
            QTime = sw.ElapsedMilliseconds,
            Msg = "快照生产成功：" + output
          };
        }
        else
        {
          return new ReturnResult<string>()
          {
            Status = IECaptStatus.ErrorNotOutFile,
            Msg = "快照失败,文件不存在"
          };
        }


      }
    }

  }
}