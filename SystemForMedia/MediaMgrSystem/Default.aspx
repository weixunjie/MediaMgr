<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="MediaMgrSystem._Default" %>


<%@ Register Src="DeviceList.ascx" TagName="DeviceList" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadMain" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">   


    <div style="width: 250px; height: 160%; float: left">

        <%  int channelIndex = 1;
            for (int i = 0; i < 3; i++)
            { %>

        <div style="float: left; height: 160px">
            <div style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

                <img src="Images/ic_image_channel.png" width="90" height="99" />

            </div>


            <div style="clear: both;">
            </div>
            <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
                通道<%= (channelIndex).ToString()  %>
                <% channelIndex++; %>
            </div>

        </div>


        <div style="float: right; height: 160px">
            <div style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

                <img src="Images/ic_image_channel.png" width="90" height="99" />

            </div>s

            <div style="clear: both;">
            </div>
            <div id="testdiv" style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">通道<%= (channelIndex).ToString()  %> <% channelIndex++; %></div>

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

        <% } %>
    </div>

    <div style="margin-left: 390px">

        <uc1:DeviceList ID="DeviceList1" runat="server" />

    </div>

    <div style="clear: both;">
    </div>

</asp:Content>
