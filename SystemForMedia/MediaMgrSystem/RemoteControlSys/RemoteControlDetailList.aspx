<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RemoteControlDetailList.aspx.cs" Inherits="MediaMgrSystem.RemoteControlDetailList" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    
    <h3 style="clip: rect(auto, auto, 10px, auto)">物联设备明细查看</h3>


    <div>



        <%  

    
            List<MediaMgrSystem.DataModels.GroupInfo> dGroups = GetAllGroups();
            GetAllStatus();
            
            

    
        %>



        <% 
        

            for (int l = 0; l < dGroups.Count; l++)
            {
           
        %>




        <table class="table table-bordered table-striped; " border="0" style="overflow: auto; height: auto; margin-top: 0px; margin-bottom: 0px; margin-left: 0px;">
            <%--<div class="jumbotron"  style="overflow: auto; height: auto; margin-top: 5px;">--%>

            <thead>
                <tr>

                    <th style="line-height: 30px">
                        <div class="row" style="margin-left: 0px">

                            <div class="pull-left"><%=dGroups[l].GroupName %></div>

                        </div>

                    </th>

                </tr>
            </thead>

            <tbody>

                <tr>
                    <td>
                        <% if (dGroups[l].Devices != null && dGroups[l].Devices.Count > 0)
                           { %>


                        <table class="table table-bordered table-striped; " style="border: 1px solid #dddddd; overflow: auto; width: 500px; height: auto; margin-top: 0px; margin-bottom: 0px; margin-left: 0px;">


                            <thead>
                                <tr>
                                    <th>教室</th>
                                    <th style="border-left: 1px solid #dddddd">空调</th>
                                    <th style="border-left: 1px solid #dddddd">电脑</th>
                                    <th style="border-left: 1px solid #dddddd">灯</th>
                                    <th style="border-left: 1px solid #dddddd">电视</th>
                                    <th style="border-left: 1px solid #dddddd">投影仪</th>
                                </tr>
                            </thead>

                            <tbody>

                                <% foreach (var di in dGroups[l].Devices)
                                   { 
                                     
                                %>
                                <tr>
                                    <td style="border-top: 1px solid #dddddd"><% =di.DeviceName %></td>
                                    <td style="border-left: 1px solid #dddddd; border-top: 1px solid #dddddd"><% =GetStatusTextByIdentifyAndDeviceType(di.DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.AC) %></td>
                                    <td style="border-left: 1px solid #dddddd; border-top: 1px solid #dddddd"><% =GetStatusTextByIdentifyAndDeviceType(di.DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.COMPUTER) %></td>
                                    <td style="border-left: 1px solid #dddddd; border-top: 1px solid #dddddd"><% =GetStatusTextByIdentifyAndDeviceType(di.DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.LIGHT) %></td>
                                    <td style="border-left: 1px solid #dddddd; border-top: 1px solid #dddddd"><% =GetStatusTextByIdentifyAndDeviceType(di.DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.TV) %></td>
                                    <td style="border-left: 1px solid #dddddd; border-top: 1px solid #dddddd"><% =GetStatusTextByIdentifyAndDeviceType(di.DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.PROJECTOR) %></td>
                                </tr>
                                <% } %>
                            </tbody>


                        </table>

                        <% } %>
                    </td>

                </tr>

                <tr style="height: 1px; padding: 0px">
                    <td style="height: 1px; padding: 0px">
                        <hr style="width: 100%; height: 1px; border: 0; background-color: #4179b6; margin: 0px" />
                    </td>
                </tr>
            </tbody>
        </table>

        <% } %>
    </div>


</asp:Content>
