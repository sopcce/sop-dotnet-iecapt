<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Demo.aspx.cs" Inherits="IECapt.Demo" %>

<!DOCTYPE html>


<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
 

    <title>输出网页快照</title>

    <!-- Bootstrap core CSS -->
    <link href="https://cdn.bootcss.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet">

 
    <!--[if lt IE 9]>
      <script src="https://cdn.bootcss.com/html5shiv/3.7.3/html5shiv.min.js"></script>
      <script src="https://cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>

    <form class="form-inline container" runat="server">
        <div class="page-header">
            <h1>网页快照</h1>
            <p class="lead">案例</p>
        </div>
        <div class="form-group">
            <label for="text_url">网址：</label>
            <asp:TextBox ID="text_url" class="form-control"
                runat="server" Width="400px" placeholder="请输入网址"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:HyperLink ID="Capt_img" runat="server" Target="_parent">快照地址</asp:HyperLink>
        </div>
        <asp:Button ID="Btn_Screenshot" runat="server" Text="网页快照" class="btn btn-default" OnClick="Btn_Screenshot_Click" />
    </form>


</body>
</html>

