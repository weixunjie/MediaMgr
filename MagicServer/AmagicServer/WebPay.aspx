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

    <h3>HKD$0.1</h3>

      <form action='expresscheck.aspx?DNO=<% = Request["snNumber"].ToString() %>' method='POST'>
        <input type='image' name='submit' src='https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif' border='0' align='top' alt='PayPal' />
    </form>

  
</body>

