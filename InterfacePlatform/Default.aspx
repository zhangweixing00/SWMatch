<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="InterfacePlatform.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Resource/js/jquery/jquery-1.8.0.min.js"></script>
    <style type="text/css">
        #taSrc
        {
            height: 81px;
            width: 287px;
        }
        #btnQurey
        {
            height: 72px;
            width: 71px;
        }
        #taDes
        {
            height: 83px;
            width: 287px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="test" OnClick="Button1_Click" />
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <br />
    </div>
    <input id="tbKey" type="text" /><br />
    <br />
    <textarea id="taSrc"></textarea>
    <input id="btnQurey" type="button" value="tt" />
    <br />
    <br />
    <textarea id="taDes"></textarea>
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {
        $("#btnQurey").click(function () {
            var key = $("#tbKey").val();
            var obj = new Object;
            var tmp = "<xml><ToUserName>123</ToUserName>";
            tmp += "<FromUserName>321</FromUserName>";
            tmp += "<CreateTime>11111</CreateTime>";
            tmp += "<MsgType>text</MsgType>";
            tmp += "<Content>" + key + "</Content></xml>";

//            obj.ToUserName = "123";
//            obj.FromUserName = "321";
//            obj.CreateTime = 12332423;
//            obj.MsgType = "text";
//            obj.Content = key;
//            var postString = window.xmlJSON.stringify(obj);
            $("#taSrc").val(tmp);
            $.ajax({
                async: false,
                type: "Post",
                contentType: "textplain/txt;charset=utf-8",
                dataType: "text",
                url: '/Interface/Entry.ashx', //'/Interface/Entry.ashx',
                data: tmp,
                success: function (data) {
                    //alert(data);
                    $("#taDes").val(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(XMLHttpRequest.status);
                    alert(XMLHttpRequest.readyState);
                    alert(textStatus);
                }
            });
        });

    });

</script>
