<%@ Page Title="杰讯智能播控系统" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BroadcastMain.aspx.cs" Inherits="MediaMgrSystem.BroadcastMain" %>




<%@ Register Src="AudioEncoderList.ascx" TagName="AudioEncoderList" TagPrefix="audioEncoderList" %>


<%@ Register Src="VideoEncodeList.ascx" TagName="VideoEncodeList" TagPrefix="videoEncodeList" %>
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

        var isUnLoad = false;
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


            var now = new Date();
            //get milliseconds of differences
            var diff = now - sess_lastActivity;
            //get minutes between differences

            var diffMins = (diff / 1000 / 60);
            if (diffMins >= sess_warningMinutes) {
                //warn before expiring

                //stop the timer
                // sessClearInterval();
                //prompt for attention
                sessLogOut();
            }

            //$.ajax({
            //    url: "AudioLogic.ashx",
            //    contentType: "text/plain",
            //    cache: false,
            //    data: { sessInterval: "Y" },
            //    type: "GET",
            //    success: function (data) {


            //    }
            //});
        }

        $(window).unload(function () {
            isUnLoad = true;
            //  alert("Goodbye!");
        });

        $(document).ready(function () {

            isUnLoad = false;

            chat = $.connection.MediaMgrHub;
            $.connection.hub.start();


            chat.connection.stateChanged(function (change) {
                if (change.newState === $.signalR.connectionState.disconnected) {

                    if (isUnLoad==false) {
                        //alert("disconnect");
                        window.location.href = "<%=ResolveUrl("~/LogOut.aspx") %>";
                }
            }
            else if (change.newState === $.signalR.connectionState.connected) {

            }
            });

            chat.connection.error(function (error) {

                window.location.href = "<%=ResolveUrl("~/LogOut.aspx") %>";
            });

            chat.client.sendRefreshAudioDeviceMessge = function (result) {
                loadDeviceList(true);
            }


            chat.client.sendRefreshVideoEncoderDeviceMessge = function (result) {
                loadVideoEncderDeviceList(true);
            }

            chat.client.sendRefreshCallerEncoderDeviceMessge = function (result) {
                loadCallerEncderDeviceList(true);
            }

            

            chat.client.sendRefreshLogMessge = function (result) {

                loadLogList();
            }

            loadDeviceList(false);
            loadLogList();
            loadVideoEncderDeviceList(false);
            loadCallerEncderDeviceList(false)

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

            function loadVideoEncderDeviceList(loadSync) {

                $.ajax({
                    url: "VideoLogic.ashx",
                    contentType: "text/plain",
                    cache: false,
                    async: loadSync,
                    data: null,
                    type: "GET",
                    success: function (data) {

                 
                        $("#divVideoEncodeList").html(data);
                    }
                });

            }
           
            function loadCallerEncderDeviceList(loadSync) {

                $.ajax({
                    url: "CallerLogic.ashx",
                    contentType: "text/plain",
                    cache: false,
                    async: loadSync,
                    data: null,
                    type: "GET",
                    success: function (data) {
                   
                        $("#divCallerEncodeList").html(data);
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


    <div style="width: 326px; height: 100%; float: left; margin-left: 0px; text-align: center">

        <channelList:ChannelList ID="cList" runat="server" />

        <div style="margin-top: 20px; margin: 0px">


            <%   if (Request["FType"] != null && Request["FType"].ToString() == "V")
                 {        
                 
                     
                
            %>

            <hr style="width: 100%; height: 1px; border: 0; background-color: #CDCDC1; margin-right: 5px" />

            <div id="divVideoEncodeList" style="margin-left: 0px; padding-top: 10px"></div>
            <%--<videoEncodeList:VideoEncodeList ID="EncoderList" runat="server" />--%>

            <% 
                 }
                 else
                 { 
            %>

            <hr style="width: 100%; height: 1px; border: 0; background-color: #CDCDC1; margin-right: 5px" />
            <%--<audioEncoderList:AudioEncoderList ID="audioEncoderList1" runat="server" />--%>

               <div id="divCallerEncodeList" style="margin-left: 0px; padding-top: 10px"></div>

            <%  } %>
        </div>
    </div>

    <div style="margin-left: 336px; border-left: solid; margin-top: 0px; border-left-color: #CDCDC1; border-left-width: 1px; padding-right: 5px; padding-bottom: 15px">


        <div id="divForDevice" style="margin-left: 0px; padding-top: 10px"></div>

        <div id="divforLogs" style="margin-left: 3px; margin-top: 15px;"></div>

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>


    <div style="clear: both;">
    </div>




</asp:Content>
