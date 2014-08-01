﻿<%@ Page Title="校园播放系统管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
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



        <table class="table table-bordered table-striped; " style="overflow: auto; height: auto; margin-top: 5px;">

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
                        <div>
                            <ul style="margin: 0px; list-style-type: none">
                                <%
                         
                if (dGroups[l].Devices != null && dGroups[l].Devices.Count > 0)
                {
                    for (int k = 0; k < dGroups[l].Devices.Count; k++)
                    { %>
                                <li data-itemid="<%=dGroups[l].Devices[k].DeviceId %>" style="float: left; display: block; width: 260px; height: 150px; margin-right: 10px; margin-top: 5px">

                                    <table class="table table-bordered table-striped; " style="overflow: auto; height: auto;">


                                        <thead style="border-radius: 0px">
                                            <tr>

                                                <th colspan="3" style="border-bottom-left-radius: 0px">

                                                    <div style="text-align: left;"><%=dGroups[l].Devices[k].DeviceName %></div>


                                                </th>

                                            </tr>
                                        </thead>



                                        <tbody>
                                            <tr>
                                                <td>

                                                    <% 
                        string colorText;
                        string statusText;
                        statusText = GetStatusTextByIdentifyAndDeviceType(dGroups[l].Devices[k].DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.AC, out colorText);
                       
                                                    %>

                                                    <div class="col-md-4" style="color: <%=colorText %>">
                                                        空调:  <b><%=statusText %></b>

                                                    </div>

                                                </td>
                                                <td>

                                                    <% 
                     
                        statusText = GetStatusTextByIdentifyAndDeviceType(dGroups[l].Devices[k].DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.COMPUTER, out colorText);
                       
                                                    %>

                                                    <div class="col-md-4" style="color: <%=colorText %>">
                                                        电脑: <b><%=statusText %></b>

                                                    </div>
                                                </td>

                                                <td>

                                                    <%statusText = GetStatusTextByIdentifyAndDeviceType(dGroups[l].Devices[k].DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.LIGHT, out colorText);
                       
                                                    %>

                                                    <div class="col-md-4" style="color: <%=colorText %>">
                                                        灯: <b><%=statusText %></b>

                                                    </div>
                                                </td>

                                            </tr>



                                            <tr>

                                                <td>
                                                    <%statusText = GetStatusTextByIdentifyAndDeviceType(dGroups[l].Devices[k].DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.TV, out colorText);
                       
                                                    %>

                                                    <div class="col-md-4" style="color: <%=colorText %>">
                                                        电视: <b><%=statusText %></b>

                                                    </div>
                                                </td>

                                                <td colspan="2">
                                                    <%statusText = GetStatusTextByIdentifyAndDeviceType(dGroups[l].Devices[k].DeviceIpAddress, MediaMgrSystem.DataModels.RemoveControlDeviceType.PROJECTOR, out colorText);
                       
                                                    %>

                                                    <div class="col-md-4" style="  color: <%=colorText %>">
                                                        投影: <b><%=statusText %></b>

                                                    </div>
                                                </td>
                                            </tr>





                                        </tbody>
                                    </table>

                                </li>


                                <%  }
                }
                                %>
                            </ul>

                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

        <% } %>
    </div>


</asp:Content>
