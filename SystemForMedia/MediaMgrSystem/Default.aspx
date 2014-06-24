<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="MediaMgrSystem._Default" %>




<%@ Register Src="EncoderList.ascx" TagName="EncoderList" TagPrefix="encoderList" %>
<%@ Register Src="DeviceList.ascx" TagName="DeviceList" TagPrefix="deviceList" %>
<%@ Register Src="ChannelList.ascx" TagName="ChannelList" TagPrefix="channelList" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">



    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />

    <script type="text/javascript">


        var currentOperDeviceDevice;
        var currentOperGroupDevice;
        var opDevices = new Array()
        var opGuidIds = new Array()
        var opSleepTimers = new Array();
        var opSleepGuidIds = new Array()
        var strclientIdentify;
        var chat;

        var is_popup_2nd_menu = false;

        var is_popup_1st_menu = false;
        

        $(document).ready(function () {

            chat = $.connection.MediaMgrHub;
            $.connection.hub.start();

            chat.connection.stateChanged(function (change) {
                if (change.newState === $.signalR.connectionState.disconnected) {                    
                    chat = $.connection.MediaMgrHub;
                    $.connection.hub.start();
                    console.log('reconnected now!');
                    
                }
                else if (change.newState === $.signalR.connectionState.connected) {
                    console.log('connection started!');
                }
            });
            chat.client.sendRefreshDeviceMessge = function (result) {
                 
                loadDeviceList();

            }

            chat.client.sendRefreshLogMessge = function (result) {

                loadLogList();

            }

            loadDeviceList();
            loadLogList();

     
     
            function loadDeviceList()
            {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "Default.aspx/RangerUserControl",
                    data: "{'controlName':'LogList'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {

                        $("#divforLogs").html(msg.d);

                    }
                });

            }

            function loadLogList() {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "Default.aspx/RangerUserControl",
                    data: "{'controlName':'DeviceList'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {

                        $("#divForDevice").html(msg.d);

                    }
                });

            }

        });

    </script>


    <div style="width: 250px; height: 160%; float: left; text-align: center">

        <channelList:ChannelList ID="cList" runat="server" />

        <div style="margin-top: 20px">

            <encoderList:EncoderList ID="EncoderList" runat="server" />

        </div>
    </div>

    <div id="divForDevice" style="margin-left: 390px">

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>

    <div style="clear: both;">
    </div>

    
    <div id="divforLogs" style="margin-left: 390px">

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>

</asp:Content>
