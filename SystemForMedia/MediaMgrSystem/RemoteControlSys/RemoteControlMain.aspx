<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RemoteControlMain.aspx.cs" Inherits="MediaMgrSystem.RemoteControlMain" %>




<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">



    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />

    <script type="text/javascript">

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

                }
                else if (change.newState === $.signalR.connectionState.connected) {

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
            

            function loadDeviceList() {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "RemoteControlMain.aspx/RangerUserControl",
                    data: "{'controlName':'RemoteDeviceList'}",
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
                    url: "RemoteControlMain.aspx/RangerUserControl",
                    data: "{'controlName':'RemoveControlLogList'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {

                        $("#divforLogs").html(msg.d);

                    }
                });

            }

        });

    </script>



    <div id="divForDevice" ">

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>

    <div style="clear: both;">
    </div>


    <div id="divforLogs" >

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>

</asp:Content>
