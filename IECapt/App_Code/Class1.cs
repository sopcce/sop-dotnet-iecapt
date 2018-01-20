using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace IECapt.Helper
{
  public class Class1
  {

    public void Img()
    {

      var dataUrl = "";
      using (System.Drawing.Image s = CreateBitmapImage("Hello"))
      {
        ImageConverter converter = new ImageConverter();
        var imageBytes = (byte[])converter.ConvertTo(s, typeof(byte[]));
        var b64String = Convert.ToBase64String(imageBytes);
        dataUrl = "data:image/jpg;base64," + b64String;
      }
      var htmlContent = "<img src=\"" + dataUrl + "\" />";


    }

    private System.Drawing.Image CreateBitmapImage(string v)
    {
      var image = CreateBitmapImage(v, new Font("宋体", 16, FontStyle.Regular), Rectangle.Empty, Color.Red, Color.White);
      return image;
    }

    /// <summary>
    /// 把文字转换才Bitmap
    /// </summary>
    /// <param name="text"></param>
    /// <param name="font"></param>
    /// <param name="rect">用于输出的矩形，文字在这个矩形内显示，为空时自动计算</param>
    /// <param name="fontcolor">字体颜色</param>
    /// <param name="backColor">背景颜色</param>
    /// <returns></returns>
    private Bitmap CreateBitmapImage(string text, Font font, Rectangle rect, Color fontcolor, Color backColor)
    {
      Graphics g;
      Bitmap bmp;
      StringFormat format = new StringFormat(StringFormatFlags.NoClip);
      if (rect == Rectangle.Empty)
      {
        bmp = new Bitmap(1, 1);
        g = Graphics.FromImage(bmp);
        //计算绘制文字所需的区域大小（根据宽度计算长度），重新创建矩形区域绘图
        SizeF sizef = g.MeasureString(text, font, PointF.Empty, format);

        int width = (int)(sizef.Width + 1);
        int height = (int)(sizef.Height + 1);
        rect = new Rectangle(0, 0, width, height);
        bmp.Dispose();

        bmp = new Bitmap(width, height);
      }
      else
      {
        bmp = new Bitmap(rect.Width, rect.Height);
      }

      g = Graphics.FromImage(bmp);

      //使用ClearType字体功能
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      g.FillRectangle(new SolidBrush(backColor), rect);
      g.DrawString(text, font, Brushes.Black, rect, format);
      return bmp;
    }
  }
}