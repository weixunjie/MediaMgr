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

        var isChooseSchedule = false;

        var is_in = false;

        var isChooseChannel = false;




        $(document).ready(function () {

            chat = $.connection.MediaMgrHub;
            $.connection.hub.start();

            chat.client.sendRefreshMessge = function (result) {

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





            // debugger;
            // $("#wei").load()


        });

    </script>


    <div style="width: 250px; height: 160%; float: left; text-align: center">

        <channelList:ChannelList ID="cList" runat="server" />


        <div style="margin-top:20px">

        <encoderList:EncoderList ID="EncoderList" runat="server" />
            </div>
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

    <div id="divForDevice" style="margin-left: 390px">

        <%--    <deviceList:DeviceList ID="DeviceList1" runat="server" />--%>
    </div>

    <div style="clear: both;">
    </div>

</asp:Content>
