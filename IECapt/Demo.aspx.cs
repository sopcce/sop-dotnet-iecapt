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
       
      Capt_img_1.ImageUrl = CaptHelper.Execute(url, IECaptOrCutyCapt.IECapt).Data;

      Capt_img_2.ImageUrl = CaptHelper.Execute(url, IECaptOrCutyCapt.CutyCapt).Data;
    }
  }
}