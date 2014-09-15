<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BroadcastMain.aspx.cs" Inherits="MediaMgrSystem.BroadcastMain" %>




<%@ Register Src="AudioEncoderList.ascx" TagName="EncoderList" TagPrefix="encoderList" %>
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

        }
        function sessSetInterval() {
            sess_intervalID = setInterval('sessInterval()', sess_pollInterval);
        }

        function sessLogOut() {
            window.location.href = "<%=ResolveUrl("~/LogOut.aspx") %>";
        }

        function sessInterval() {


            $.ajax({
                url: "AudioLogic.ashx",
                contentType: "text/plain",
                cache: false,
                data: { sessInterval: "Y" },
                type: "GET",
                success: function (data) {


                }
            });
        }

        $(document).ready(function () {


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
                loadDeviceList(true);
            }

            chat.client.sendRefreshLogMessge = function (result) {

                loadLogList();
            }

            loadDeviceList(false);
            loadLogList();



            function loadDeviceList(loadSync) {

                $.ajax({
                    url: "AudioLogic.ashx",
                    contentType: "text/plain",
                    cache: false,
                    async: loadSync,
                    data: null,
                    type: "GET",
                    success: function (data) {

                        $("#divForDevice").html(data);
                    }
                });

            }

            function loadLogList() {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "BroadcastMain.aspx/RangerUserControl",
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


    <div style="width: 300px; height: 100%; float: left; margin-left: 0px; text-align: center">

        <channelList:ChannelList ID="cList" runat="server" />

        <div style="margin-top: 20px">

           <%-- <encoderList:EncoderList ID="EncoderList" runat="server" />--%>

        </div>
    </div>

    <div style="margin-left: 350px;  border-left:solid; margin-top:0px; border-left-color:#4179b6;border-left-width:1px">

        

        <div id="divForDevice" style="margin-left: 0px; padding-top:10px "></div>

        <div id="divforLogs" style="margin-left: 3px; margin-top:5px"></div>

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>


    <div style="clear: both;">
    </div>




</asp:Content>
