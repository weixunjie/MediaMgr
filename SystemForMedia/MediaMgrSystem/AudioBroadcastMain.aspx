<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AudioBroadcastMain.aspx.cs" Inherits="MediaMgrSystem._Default" %>




<%@ Register Src="EncoderList.ascx" TagName="EncoderList" TagPrefix="encoderList" %>
<%@ Register Src="DeviceList.ascx" TagName="DeviceList" TagPrefix="deviceList" %>
<%@ Register Src="ChannelList.ascx" TagName="ChannelList" TagPrefix="channelList" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">



    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />

    <script type="text/javascript">


        var sess_pollInterval = 60000;
        var sess_expirationMinutes = 20;
        var sess_warningMinutes = 19;
        var sess_intervalID;
        var sess_lastActivity;

        var chat;

        var is_popup_2nd_menu = false;

        var is_popup_1st_menu = false;


        // proces session time out


        initSession();

        function initSession() {
      
            sess_lastActivity = new Date();
            sessSetInterval();
            $(document).bind('keypress.session', function (ed, e) {
                sessKeyPressed(ed, e);
            });
        }
        function sessSetInterval() {
            sess_intervalID = setInterval('sessInterval()', sess_pollInterval);
        }
        function sessClearInterval() {
            clearInterval(sess_intervalID);

        }
        function sessKeyPressed(ed, e) {
            sess_lastActivity = new Date();
        }
        function sessLogOut() {
            window.location.href = "<%=ResolveUrl("~/LogOut.aspx") %>";
        }

        function sessInterval() {
            var now = new Date();

            //get milliseconds of differneces
            var diff = now - sess_lastActivity;
            //get minutes between differences
            var diffMins = (diff / 1000 / 60);
            if (diffMins >= sess_warningMinutes) {
                sessClearInterval();
                sessLogOut();
            }

        }


        // end proces session time out


        $(document).ready(function () {

     
            debugger;
            chat = $.connection.MediaMgrHub;
            $.connection.hub.start();

            chat.connection.stateChanged(function (change) {
                if (change.newState === $.signalR.connectionState.disconnected) {
                    chat = $.connection.MediaMgrHub;
                    $.connection.hub.start();

                }
                else if (change.newState === $.signalR.connectionState.connected) {

                }
            });

            chat.client.sendRefreshAudioDeviceMessge = function (result) {
                loadDeviceList();
            }

            chat.client.sendRefreshLogMessge = function (result) {
                loadLogList();
            }

            loadDeviceList();
            loadLogList();



            function loadDeviceList() {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "AudioBroadcastMain.aspx/RangerUserControl",
                    data: "{'controlName':'DeviceList'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {

                        $("#divForDevice").html(msg.d);

                    }
                });

            }

            function loadLogList() {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "AudioBroadcastMain.aspx/RangerUserControl",
                    data: "{'controlName':'LogList'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {

                        $("#divforLogs").html(msg.d);

                    }
                });

            }

        });

    </script>


    <div style="width: 250px; height: 160%; float: left; margin-left: 10px; text-align: center">

        <channelList:ChannelList ID="cList" runat="server" />

        <div style="margin-top: 20px">

           <%-- <encoderList:EncoderList ID="EncoderList" runat="server" />--%>

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
