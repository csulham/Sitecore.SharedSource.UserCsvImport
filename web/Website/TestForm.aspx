<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestForm.aspx.cs" Inherits="Website.TestForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
			<asp:TextBox ID="txtFilename" runat="server"></asp:TextBox>
			<asp:Button runat="server" ID="btnDownload" OnClick="btnDownload_OnClick" Text="Download"/>
    </div>
    </form>
</body>
</html>
