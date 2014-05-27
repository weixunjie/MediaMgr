<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication8.WebForm1" %>

<!DOCTYPE html>



<body>
    <h3>Buy Andy Magic Full Version</h3>
    <h3>购买安迪魔术完整版本</h3>

    <h3>RMB: 10 </h3>
<form action="https://www.sandbox.paypal.com/cgi-bin/webscr" method="post"> 
<input type="hidden" name="cmd" value="_xclick"> 
<input type="hidden" name="business" value="gm1tran2l@gmail.com">
<input type="hidden" name="item_name" value="001">
<input type="hidden" name="item_number" value="990"> 
<input type="hidden" name="currency_code" value="HKD">
<input type="hidden" name="amount" value="1"> 

    
 <input type="hidden" name="custom" value="<% = Request["snNumber"].ToString() %>"> 
<input type="hidden" name="notify_url" value="http://2lltdapp1.azurewebsites.net/processresult.aspx" />

 <img  src="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif" onclick="submit();" align="left" style="margin-right:7px;">

</form>
    </body>

