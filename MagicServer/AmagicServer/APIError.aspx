<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="APIError.aspx.cs" Inherits="WebApplication8.APIError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

        <table class="api" id="Table1">
            <tr>
                <td class="field"></td>
                <td><%=Request.QueryString.Get("ErrorCode")%></td>
            </tr>

            <tr>
                <td class="field"></td>
                <td><%=Request.QueryString.Get("Desc")%></td>
            </tr>

            <tr>
                <td class="field"></td>
                <td><%=Request.QueryString.Get("Desc2")%></td>
            </tr>

        </table>
    </form>
</body>
</html>
