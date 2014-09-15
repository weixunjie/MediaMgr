<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RemoteDeviceList.ascx.cs" Inherits="MediaMgrSystem.RemoteDeviceList" %>


<style>
    .cb td {
        width: 100px;
        text-align: center;
    }

    .cb label {
        float: left;
        display: inline-block;
    }

    .cb input {
        float: left;
    }
</style>

<script type="text/javascript">

       

    var dialogddACMode = "<%= ddACMode.ClientID %>";

    var dialogddOpenClose = "<%= this.ddOpenClose.ClientID %>";

    var dialogcbScheduleWeeks = "<%= this.cbScheduleWeeks.ClientID %>";
    
    var dialogForSingleDeviceACMode = "<%= this.ddForSingleDeviceACMode.ClientID %>";

    <%  
        
     
    List<MediaMgrSystem.DataModels.GroupInfo> dGroups = GetAllGroups();

    string deviceIds = string.Empty;
    string imgGroupShowIds = string.Empty;
    int tmpDeviceIndex = 0;
    for (int i = 0; i < dGroups.Count; i++)
    {
        if (dGroups[i] != null && dGroups[i].Devices != null && dGroups[i].Devices.Count > 0)
        {

            foreach (var di in dGroups[i].Devices)
            {
                tmpDeviceIndex++;
                deviceIds = deviceIds + "#deviceMenu" + tmpDeviceIndex + ",";
            }
        }

        if (dGroups[i].GroupId == "-1") { continue; } imgGroupShowIds = imgGroupShowIds + "#imgGroupShow" + dGroups[i].GroupId + ",";
    };

    deviceIds = deviceIds.TrimEnd(',');
    imgGroupShowIds = imgGroupShowIds.TrimEnd(',');


        %>



    var currentOperDevice;
    $(document).unbind();



    $(document).ready(function () {


        $("input[id$=tbScheduleTime]").timepicker({
            timeFormat: 'hh:mm:ss',

            showSecond: true
        });

        $("#btnBatchGroupOperation").click(function (e) {

            var x = $(this).offset().left;
            var y = $(this).offset().top + $(this).height() + 10;

            is_popup_1st_menu = true;

            $("#groupListBatchMenu").show().css("left", x).css("top", y);


        });


        $("#btnBatchGroupOperation").mouseout(function (e) {

            is_popup_1st_menu = false;

        });


        var isBatchSchedule = false;

        $("#groupListBacthMenuSchedule").click(function (e) {


            isBatchSchedule = true;
            $("#divPlanTime").css("display", "block");

            $("#divPlanWeeks").css("display", "block");

            $("#batchGroupRightDiv").css("height", "460px");

            $("#dialogForBatchGrouplbAvaiableGroups").css("height", "440px");

            is_popup_1st_menu = false;
            $("#groupListBatchMenu").hide;
            $('#dialogForBatchGroup').modal('show');
        });

        $("#groupListBacthMenuManual").click(function (e) {


            isBatchSchedule = false;
            $("#divPlanTime").css("display", "none");

            $("#divPlanWeeks").css("display", "none");


            $("#batchGroupRightDiv").css("height", "330px");

            $("#dialogForBatchGrouplbAvaiableGroups").css("height", "310px");

            is_popup_1st_menu = false;
            $("#groupListBatchMenu").hide;
            $('#dialogForBatchGroup').modal('show');
        });



        function hideAllMenus() {

            $("#deviceListSingleDeviceMenu").hide();
            $("#deviceListSingleDeviceSubMenu").hide();
            $("#groupListBatchMenu").hide();

        }


        var currentExternalPointId;

        $("#deviceListSingleDeviceMenuBtnType2,#deviceListSingleDeviceMenuBtnType3,#deviceListSingleDeviceMenuBtnType4,#deviceListSingleDeviceMenuBtnType5").click(function (e) {


            is_popup_2nd_menu = true;

            currentExternalPointId = e.currentTarget.id.replace("deviceListSingleDeviceMenuBtnType", "");

            $("#liDeviceListSingleDeviceSubMenuBtnChangeParam").css("display", "none");

            var x = $(this).offset().left + $("#deviceListSingleDeviceMenu").width() + 6;
            var y = $(this).offset().top;

            $("#deviceListSingleDeviceSubMenu").show().css("left", x).css("top", y);


        });

        $("#deviceListSingleDeviceMenuBtnType1").click(function (e) {


            currentExternalPointId = "1";
            is_popup_2nd_menu = true;


            $("#liDeviceListSingleDeviceSubMenuBtnChangeParam").css("display", "block");

            var x = $(this).offset().left + $("#deviceListSingleDeviceMenu").width() + 6;
            var y = $(this).offset().top;

            $("#deviceListSingleDeviceSubMenu").show().css("left", x).css("top", y);


        });

        $("#deviceListSingleDeviceMenuBtnType1,#deviceListSingleDeviceMenuBtnType2,#deviceListSingleDeviceMenuBtnType3,#deviceListSingleDeviceMenuBtnType4,#deviceListSingleDeviceMenuBtnType5").mouseout(function (e) {
            is_popup_1st_menu = false;
        });

        $("#deviceListSingleDeviceSubMenuBtnOpen,#deviceListSingleDeviceSubMenuBtnClose,#deviceListSingleDeviceSubMenuBtnChangeParam").mouseout(function (e) {
            is_popup_2nd_menu = false;
        });

        $("#deviceListSingleDeviceSubMenuBtnOpen").click(function (e) {


            hideAllMenus();
            is_popup_2nd_menu = false;

            hubForRemoteControl.server.sendRemoteControlBySingleDevice(currentExternalPointId,  currentOperDevice,true,"", "");

            

        });


        $("#deviceListSingleDeviceSubMenuBtnClose").click(function (e) {


            hideAllMenus();
            is_popup_2nd_menu = false;
            hubForRemoteControl.server.sendRemoteControlBySingleDevice(currentExternalPointId, currentOperDevice,false,"", "");



        });

        $("#deviceListSingleDeviceSubMenuBtnChangeParam").click(function (e) {
            $('#dialogOperationACForSingleDevice').modal('show');

            hideAllMenus();
            is_popup_2nd_menu = false;

        });

        $.showSingleDeviceClickMenu = function () {

            $("<%= deviceIds %>").click(function (e) {



                hideAllMenus();

               // currentOperDevice = e.currentTarget.id.replace("deviceMenu", "");

                currentOperDevice = $(this).data("itemid");

              
                is_popup_1st_menu = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#deviceListSingleDeviceMenu").show().css("left", x).css("top", y);


            });

            $("<%= deviceIds %>").mouseout(function (e) {
                is_popup_1st_menu = false;
            });

            $(document).click(function () {
                sess_lastActivity = new Date();
                if (!is_popup_1st_menu && !is_popup_2nd_menu) {
                    hideAllMenus();
                }
            });

        }


        $("#btnDeviceStatus").click(function (e) {

            window.open(" <%= ResolveUrl("~/RemoteControlSys/RemoteControlDetailList.aspx")%>");

        });

        $.showSingleDeviceClickMenu();


        $("#btnConfirmedSingleACOperation").click(function (e) {

            var tbForSingleDeviceACTempureValue = $("#tbForSingleDeviceACTempure").val();



            if (!(/^(\+|-)?\d+$/.test(tbForSingleDeviceACTempureValue)) || tbForSingleDeviceACTempureValue < 0 || tbForSingleDeviceACTempureValue > 45) {

                alert("请输入正确的空调温度");
                return;

            }

            var strSingleACMode = $('#' + dialogForSingleDeviceACMode + ' option:selected').val();

            

            $('#dialogOperationACForSingleDevice').modal('hide');

            
            hubForRemoteControl.server.sendRemoteControlBySingleDevice(currentExternalPointId,  currentOperDevice,true, strSingleACMode, tbForSingleDeviceACTempureValue);


        });
        

        $("#btnConfirmedBatchGroupOperation").click(function (e) {


            var strGroupIds = "";
            $("#dialogForBatchGrouplbAvaiableGroups option:selected").each(function () {


                strGroupIds += $(this).val() + ",";


            })


            if (strGroupIds == "") {
                alert('请选择分组');
           
                return
                //alert($("#" + dialogddACMode).val());
            }


            var strExternalPointIds = "";
            $("input[name^='cbScheduleDevice']").each(function () {

                if ($(this)[0].checked) {
                    strExternalPointIds += $(this).val() + ",";
                }
            });

            if (strExternalPointIds == "") {
                alert('请选择设备');
                return
                //alert($("#" + dialogddACMode).val());
            }


            var tbACTempureValue = $("#tbACTempure").val();



            if (!(/^(\+|-)?\d+$/.test(tbACTempureValue)) || tbACTempureValue < 0 || tbACTempureValue > 45) {

                alert("请输入正确的空调温度");
                return;

            }


            var tbScheduleTimeValue = $("#tbScheduleTime").val();
            if (tbScheduleTimeValue == "") {
                if (isBatchSchedule) {
                    alert("计划时间不能为空");
                    return
                }
            }




            var strWeeks = "";

            $("input[name^='dialogcbScheduleWeeks']").each(function () {

                if ($(this)[0].checked) {
                    strWeeks += $(this).val() + ",";
                }
            });

            if (strWeeks == "") {
                if (isBatchSchedule) {
                    alert("请选择计划周期。");
                    return
                }
            }
            var strACMode = $('#' + dialogddACMode + ' option:selected').val();

            var strOpenOrClose = $('#' + dialogddOpenClose + ' option:selected').val();



            hubForRemoteControl.server.sendRemoteControlByGroups(strExternalPointIds, strGroupIds, strOpenOrClose == "1", strACMode, tbACTempureValue, tbScheduleTimeValue, strWeeks, isBatchSchedule);
            $('#dialogForBatchGroup').modal('hide');

        });


    });


</script>

<div>

    <h3 class="pull-left" style="clip: rect(auto, auto, 10px, auto)">物联设备控制</h3>


    <div class="pull-right" style="margin-right: 10px"><a id="btnDeviceStatus" class="btn  btn-success" data-content="">设备状态</a></div>


    <% 
        int deviceIndex = 0;

        int groupIndex = 0;

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

                    <div style="height: 90px">
                        <ul id="groupDeviceList<%=groupIndex %>" class="deviceULStyle">
                            <%
        
            if (dGroups[l].Devices != null && dGroups[l].Devices.Count > 0)
            {
                for (int k = 0; k < dGroups[l].Devices.Count; k++)
                {
                            %>

                            <li data-itemid="<%=dGroups[l].Devices[k].DeviceId %>">

                                <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">
                                        <p style="text-align: center">

                                            <% 
                                                
                    string srcName = ResolveUrl("~/Images/ic_image_device.png");

                    if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                    {
                        srcName = ResolveUrl("~/Images/ic_image_device_offline.png");
                    }

                    deviceIndex++;
                     
                                            %>

                                            

                                            <img id="deviceMenu<% =deviceIndex.ToString() %>" data-itemid="<% =dGroups[l].Devices[k].DeviceIpAddress %>" src="<%=srcName %>" style="width: 50px; height: 50px" />
                                        </p>
                                        <p style="text-align: center">
                                            <% =dGroups[l].Devices[k].DeviceName %>
                                        </p>
                                    </div>
                                </div>

                            </li>

                            <%  }
            } %>
                        </ul>


                    </div>

                </td>
            </tr>
        </tbody>
    </table>

    <%
            groupIndex++;
        } %>


    <table class="table table-bordered table-striped; " style="overflow: auto; height: auto; margin-top: 0px;">

        <thead>
            <tr>

                <th style="line-height: 30px">
                    <div class="row" style="margin-left: 0px">

                        <div class="pull-left">分组信息</div>
                        <div class="pull-right"><a class="btn  btn-success" id="btnBatchGroupOperation" data-content="">批量操作</a></div>

                    </div>

                </th>

            </tr>
        </thead>


        <tbody>

            <tr>
                <td>
                    <div style="height: 90px">
                        <ul id="deviceGroupList" class="deviceULStyle">
                            <%          
        
                                for (int k = 0; k < dGroups.Count; k++)
                                {                                     
                            
                            %>

                            <li data-itemid="<%=dGroups[k].GroupId %>">

                                <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">
                                        <p style="text-align: center">

                                            <img id="imgGroupShow<% =dGroups[k].GroupId %>" src="<%= ResolveUrl("~/Images/ic_image_group.png") %>" style="width: 50px; height: 50px" />
                                        </p>
                                        <p id="ptext" style="text-align: center">
                                            <% =dGroups[k].GroupName %>
                                        </p>
                                    </div>
                                </div>

                            </li>

                            <%  }
                            %>
                        </ul>


                    </div>
                </td>
            </tr>
        </tbody>
    </table>


    <div id="dialogOperationACForSingleDevice" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogOperationACForSingleDevice').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">空调操作</h3>
        </div>
        <div class="modal-body" style="max-height: 500px">

            <div style="height: 160px; width: 240px;">
                <h4 style="text-align: left; margin-top: 0px">空调参数</h4>

                <div>

                    <p>
                        空调温度
                    </p>

                    <div style="height: 40px">
                        <input type="text" style="width: 220px" id="tbForSingleDeviceACTempure" />
                    </div>

                </div>

                <div>

                    <p>
                        空调模式:
                    </p>


                    <div style="height: 40px">
                        <asp:DropDownList runat="server" Width="235px" ID="ddForSingleDeviceACMode">
                            <asp:ListItem Value="1">制冷</asp:ListItem>
                            <asp:ListItem Value="2">制热</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>


            </div>

        </div>

        <div class="modal-footer">
            <a id="btnConfirmedSingleACOperation" class="btn primary">确认</a>
        </div>
    </div>

    <div id="dialogForBatchGroup" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForBatchGroup').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">设备操作</h3>
        </div>
        <div class="modal-body" style="max-height: 500px">
            <div style="float: left; height: auto; width: 160px; margin-right: 10px">

                <h4 style="text-align: left; margin-top: 0px">可选组</h4>

                <select size="4" multiple="multiple" style="height: 450px; width: 150px;" id="dialogForBatchGrouplbAvaiableGroups">

                    <% foreach (var di in dGroups)
                       {
                    %>
                    <option value="<%= di.GroupId %>"><%= di.GroupName %></option>

                    <% 
                       }
                    %>
                </select>
            </div>

            <div id="batchGroupRightDiv" style="float: left; height: 460px; width: 300px;">
                <h4 style="text-align: left; margin-top: 0px">批量操作</h4>

                <div>
                    <p>
                        设备
                    </p>
                    <div style="height: 80px">

                        <asp:CheckBoxList ID="cbScheduleDevice" runat="server" Height="40px" RepeatColumns="3" RepeatDirection="Horizontal" Width="300px" CssClass="cb">
                            <asp:ListItem Value="1">空调</asp:ListItem>
                            <asp:ListItem Value="2">电视</asp:ListItem>
                            <asp:ListItem Value="3">投影仪</asp:ListItem>
                            <asp:ListItem Value="4">电脑</asp:ListItem>
                            <asp:ListItem Value="5">灯</asp:ListItem>
                        </asp:CheckBoxList>
                    </div>

                </div>

                <div>

                    <p>
                        空调温度
                    </p>


                    <div style="height: 40px">
                        <input type="text" style="width: 220px" id="tbACTempure" />
                    </div>

                </div>

                <div>
                    <p>
                        空调模式:
                    </p>


                    <div style="height: 40px">
                        <asp:DropDownList runat="server" Width="235px" ID="ddACMode">
                            <asp:ListItem Value="1">制冷</asp:ListItem>
                            <asp:ListItem Value="2">制热</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>

                <div>

                    <p>
                        打开/关闭:
                    </p>

                    <div style="height: 40px">

                        <asp:DropDownList runat="server" Width="235px" Height="30px" ID="ddOpenClose">
                            <asp:ListItem Value="0">打开</asp:ListItem>
                            <asp:ListItem Value="1">关闭</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>


                <div id="divPlanTime">

                    <p>
                        计划时间:
                    </p>


                    <div>
                        <input type="text" runat="server" style="width: 220px" id="tbScheduleTime" />
                    </div>
                </div>

                <div id="divPlanWeeks">

                    <p>
                        计划星期:
                    </p>

                    <div>
                        <asp:CheckBoxList ID="cbScheduleWeeks" runat="server" Height="40px" RepeatColumns="7" RepeatDirection="Horizontal" Width="300px" CssClass="cb">
                            <asp:ListItem Value="1">一</asp:ListItem>
                            <asp:ListItem Value="2">二</asp:ListItem>
                            <asp:ListItem Value="3">三</asp:ListItem>
                            <asp:ListItem Value="4">四</asp:ListItem>
                            <asp:ListItem Value="5">五</asp:ListItem>
                            <asp:ListItem Value="6">六</asp:ListItem>
                            <asp:ListItem Value="7">日</asp:ListItem>
                        </asp:CheckBoxList>
                    </div>

                </div>




            </div>

        </div>

        <div class="modal-footer">
            <a id="btnConfirmedBatchGroupOperation" class="btn primary">确认</a>
        </div>
    </div>



    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="deviceListSingleDeviceSubMenu">
        <li><a class="btn" id="deviceListSingleDeviceSubMenuBtnOpen" data-backdrop="static" data-dismiss="modal" data-keyboard="false">打开</a></li>
        <li><a class="btn" id="deviceListSingleDeviceSubMenuBtnClose" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">关闭</a></li>

        <li id="liDeviceListSingleDeviceSubMenuBtnChangeParam"><a class="btn" id="deviceListSingleDeviceSubMenuBtnChangeParam" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">更改参数</a></li>

    </ul>

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="deviceListSingleDeviceMenu">
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnType1" data-itemid="1" data-backdrop="static" data-dismiss="modal" data-keyboard="false">空调</a></li>
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnType2" data-itemid="2" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">电视</a></li>
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnType3" data-itemid="3" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">投影仪</a></li>
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnType4" data-itemid="4" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">电脑</a></li>
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnType5" data-itemid="5" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">灯</a></li>

    </ul>


    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="groupListBatchMenu">
        <li><a class="btn" id="groupListBacthMenuManual" data-backdrop="static" data-dismiss="modal" data-keyboard="false">手工操作</a></li>
        <li><a class="btn" id="groupListBacthMenuSchedule" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">计划调度</a></li>

    </ul>




</div>

