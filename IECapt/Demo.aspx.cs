using Capt.Helper;
using IECapt.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IECapt
{
  public partial class Demo : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {




    }



    protected void Btn_Screenshot_Click(object sender, EventArgs e)
    {
      string url = text_url.Text;

      var result_1 = CaptHelper.Execute(url, IECaptOrCutyCapt.IECapt);
      Capt_img_1.ImageUrl = result_1.Data;
      Capt_img_1.Text = Capt_img_1.Text + "|" + result_1.Data;

      var result_2 = CaptHelper.Execute(url, IECaptOrCutyCapt.CutyCapt);
      Capt_img_2.ImageUrl = result_2.Data;
      Capt_img_2.Text = Capt_img_2.Text + "|" + result_2.Data;
    }
  }
}