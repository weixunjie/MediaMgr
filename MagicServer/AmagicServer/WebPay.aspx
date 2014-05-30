<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebPay.aspx.cs" Inherits="AmagicServer.WebPay" %>

<!DOCTYPE html>

<meta http-equiv="Content-Type" content="application/vnd.wap.xhtml+xml;charset=utf-8" />
<meta http-equiv="Cache-Control" content="no-cache" />
<meta name="viewport" content="width=device-width; initial-scale=1; minimum-scale=1.0; maximum-scale=2.0" />
<meta name="MobileOptimized" content="240" />
<head>
    <title>安迪魔术(Andy Magic)</title>
</head>
<body>
    <h3>Buy Andy Magic Full Version</h3>
    <h3>购买安迪魔术完整版本</h3>

    <h3>HKD$15.00</h3>
   <%-- <form action="https://www.sandbox.paypal.com/cgi-bin/webscr" id="ww" method="post">--%>
    <form action="https://www.paypal.com/cgi-bin/webscr" id="ww" method="post">
        <input type="hidden" name="cmd" value="_xclick">
         <input type="hidden" name="business" value="tran2l@2lltd.com">
      <%--  <input type="hidden" name="business" value="gm1tran2l@gmail.com">--%>
        <input type="hidden" name="item_name" value="Andy Magic Full Version">
        <input type="hidden" name="item_number" value="001">
        <input type="hidden" name="currency_code" value="HKD">
        <input type="hidden" name="amount" value="15">

        <input type="hidden" name="custom" value="<% = Request["snNumber"].ToString() %>">
        <input type="hidden" name="notify_url" value="http://2lltdapp1.azurewebsites.net/processresult.aspx" />

        <img src="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif" onclick="submit();" align="left" style="margin-right: 7px;">
    </form>
</body>

