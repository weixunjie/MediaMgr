<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="MediaMgrSystem._Default" %>




<%@ Register Src="DeviceList.ascx" TagName="DeviceList" TagPrefix="deviceList" %>
<%@ Register Src="ChannelList.ascx" TagName="ChannelList" TagPrefix="channelList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadMain" runat="Server">

    <script type="text/javascript" src="Scripts/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery.timer.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.js"></script>
    <script type="text/javascript" src="Scripts/jquery.dragsort-0.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.10.4.custom.js"></script>
    <script type="text/javascript" src="Scripts/utils.js"></script>



    <script src="Scripts/jquery-1.6.4.min.js"></script>

    <script src="Scripts/jquery.signalR-2.0.3.min.js"></script>


    <script src="signalr/hubs"></script>

    <link rel="stylesheet" href="Content/jquery-ui-1.10.4.custom.min.css" type="text/css" />

    <link rel="stylesheet" href="Content/jquery-ui-1.10.4.custom.css" type="text/css" />

    <link rel="stylesheet" href="Content/DeviceList.css" type="text/css" />
    <link rel="stylesheet" href="Content/ChannelList.css" type="text/css" />


    <link rel="stylesheet" href="Content/bootstrap.css" type="text/css" />

    <link rel="stylesheet" href="Content/bootstrap.min.css" type="text/css" />
    <link rel="stylesheet" href="Content/jquery-ui-1.10.4.custom" type="text/css" />
    <link rel="stylesheet" href="Content/jquery-ui-1.10.4.custom" type="text/css" />


    <script type="text/javascript">

        var currentOperDeviceDevice;
        var currentOperGroupDevice;
        var opDevices = new Array()
        var opGuidIds = new Array()
        var opSleepTimers = new Array();
        var opSleepGuidIds = new Array()
        var strclientIdentify;
        var chat;
        $(document).ready(function () {

            chat = $.connection.Test;
            $.connection.hub.start();

        });

    </script>



</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">



    <div style="width: 250px; height: 160%; float: left; text-align: center">

        <channelList:ChannelList ID="cList" runat="server" />
        <%--  <div style="width: 100px; position: relative; height: 100px; line-height: 100px; vertical-align: central; text-align: center; float: left; padding: 4px 4px 4px 4px"
                class="jumbotron">
                通道<%= (channelIndex).ToString()  %>
                <% channelIndex++; %>
                <div style="width: 20px; height: 20px; line-height: 20px; position: absolute; right: 5px; bottom: 5px;">
                    <img src="Images/ic_image_menu.png" width="20" height="20" />
                </div>
            </div>

            <div style="width: 100px; position: relative; height: 100px; line-height: 100px; vertical-align: central; text-align: center; float: right; padding: 4px 4px 4px 4px"
                class="jumbotron">
                通道<%= (channelIndex).ToString()  %>
                <% channelIndex++; %>
                <div style="width: 20px; height: 20px; line-height: 20px; position: absolute; right: 5px; bottom: 5px;">
                    <img src="Images/ic_image_menu.png" width="20" height="20" />
                </div>
            </div>--%>

        <%-- <% } %>--%>
    </div>

    <div style="margin-left: 390px">

        <deviceList:DeviceList ID="DeviceList1" runat="server" />

    </div>

    <div style="clear: both;">
    </div>

</asp:Content>
