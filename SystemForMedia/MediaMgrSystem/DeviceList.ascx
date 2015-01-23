<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DeviceList.ascx.cs" Inherits="MediaMgrSystem.DeviceList" %>


<script type="text/javascript">


    var currentOperGroup;

    var currentOperDevice;
    var currentOperDeviceIpAddress;
    var currentOperDeviceGroupId;



    $(document).unbind();


    $(document).ready(function () {

        $("input[id$=tbScheduleTime]").timepicker({
            timeFormat: 'hh:mm:ss',

            showSecond: false
        });

        $("input[id$=tbScheduleTurnOffTime]").timepicker({
            timeFormat: 'hh:mm:ss',

            showSecond: false
        });
        
        <% if (CheckIfCanDragGroups())
           {
               %>
        $("#btnSingleGroupChooseChannel").attr("disabled", false);
        $("#btnSingleGroupChooseEncoder").attr("disabled", false);

        $("#btnBatchGroupOperation").attr("disabled", false);

        <% } else {%> 

        $("#btnSingleGroupChooseChannel").attr("disabled", true);
        $("#btnSingleGroupChooseEncoder").attr("disabled", true);
        $("#btnBatchGroupOperation").attr("disabled", true);
        

        <%}

    
        List<MediaMgrSystem.DataModels.GroupInfo> dGroups = GetAllGroupsIncludeDefaultGroup();
        List<MediaMgrSystem.DataModels.ChannelInfo> channels = GetAllChannels();

        List<MediaMgrSystem.DataModels.VideoEncoderInfo> encoders = GetAllEncoders();

        GetAllEncoderRunning();

        string deviceIds = string.Empty;
        string imgGroupShowIds = string.Empty;

        string deviceBatchOperDeviceIds = string.Empty;
        int tmpDeviceIndex = 0;
        for (int i = 0; i < dGroups.Count; i++)
        {
            if (dGroups[i] != null && dGroups[i].Devices != null && dGroups[i].Devices.Count > 0)
            {

                foreach (var di in dGroups[i].Devices)
                {
                    tmpDeviceIndex++;
                    deviceIds = deviceIds + "#deviceMenu" + di.DeviceId + ",";
                }
            }

            if (dGroups[i].GroupId == "-1") { continue; }

            deviceBatchOperDeviceIds = deviceBatchOperDeviceIds + "#btnDeviceBatchOper" + dGroups[i].GroupId + ",";


            imgGroupShowIds = imgGroupShowIds + "#imgGroupShow" + dGroups[i].GroupId + ",";
        };

        deviceIds = deviceIds.TrimEnd(',');
        imgGroupShowIds = imgGroupShowIds.TrimEnd(',');
        deviceBatchOperDeviceIds = deviceBatchOperDeviceIds.TrimEnd(',');
        string btnMenuChannelSelIds = string.Empty; for (int i = 0; i < channels.Count; i++) { btnMenuChannelSelIds = btnMenuChannelSelIds + "#btnMenuChannelSel" + channels[i].ChannelId + ","; }; btnMenuChannelSelIds = btnMenuChannelSelIds.TrimEnd(',');

        string btnMenuEncoderSelIds = string.Empty; for (int i = 0; i < encoders.Count; i++) { btnMenuEncoderSelIds = btnMenuEncoderSelIds + "#btnMenuEncoderSel" + encoders[i].EncoderId + ","; }; btnMenuEncoderSelIds = btnMenuEncoderSelIds.TrimEnd(',');
    
        %>



   $("#btnConfirmedBatchGroupOperation").click(function (e) {


       $("#dialogForBatchGrouplbAvaiableGroups option:selected").each(function () {



           var selectedGroupId = $(this).val();

           var selectedChannelId = $('#ddBatchSelectChannel option:selected').val();

           var selectedEncoderId = $('#ddBatchSelectVideoSorce option:selected').val();


           $.ajax({
               type: "POST",
               async: false,
               url: "BroadcastMain.aspx/SaveGroupChannelAndEncoder",
               data: "{'cid':'" + selectedChannelId + "',gid:'" + selectedGroupId + "','eid':'" + selectedEncoderId + "'}",
               dataType: "json",
               contentType: "application/json; charset=utf-8",
               success: function (msg) {


               }
           });

       })


       $('#dialogForBatchGroup').modal('hide');
   });

        $("#btnBatchGroupOperation").click(function (e) {

            if ($("#btnBatchGroupOperation").attr("disabled") == "disabled") {
                return;
            }


            $('#dialogForBatchGroup').modal('show');
        });



        $("#btnSingleGroupChooseChannel").click(function (e) {

            if ($("#btnSingleGroupChooseChannel").attr("disabled") == "disabled") {
                return;
            }


            $("#deviceListSingleGroupChooseEncoderMenu").hide();

            is_popup_2nd_menu = true;

            var x = $(this).offset().left + $("#deviceListSingleGroupChooseChannelMenu").width() + 6;
            var y = $(this).offset().top;

            $.ajax({
                type: "POST",
                async: false,
                url: "BroadcastMain.aspx/GetChannelByGroupId",
                data: "{'gid':'" + currentOperGroup + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                    $("<% =btnMenuChannelSelIds%>").css("font-weight", "normal");
                $("#btnMenuChannelSel" + msg.d).css("font-weight", "bold");

            }
        });

        $("#deviceListSingleGroupChooseChannelMenu").show().css("left", x).css("top", y);
    });


        $("<% =btnMenuChannelSelIds%>").click(function (e) {


            var currentOperChannel = e.currentTarget.id.replace("btnMenuChannelSel", "");

            $.ajax({
                type: "POST",
                async: true,
                url: "BroadcastMain.aspx/SaveGroupChannel",
                data: "{'cid':'" + currentOperChannel + "',gid:'" + currentOperGroup + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                }
            });


            is_popup_2nd_menu = false;
            is_popup_1st_menu = false;

        });


        $("#btnSingleGroupChooseChannel").mouseout(function (e) {
            is_popup_2nd_menu = false;
        });

        $("#btnSingleGroupChooseEncoder").click(function (e) {


            if ($("#btnSingleGroupChooseEncoder").attr("disabled") == "disabled") {
                return;
            }


            $("#deviceListSingleGroupChooseChannelMenu").hide()


            is_popup_2nd_menu = true;

            var x = $(this).offset().left + $("#deviceListSingleGroupChooseEncoderMenu").width() + 6;
            var y = $(this).offset().top;


            $.ajax({
                type: "POST",
                async: false,
                url: "BroadcastMain.aspx/GetEncoderByGroupId",
                data: "{'gid':'" + currentOperGroup + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                    $("<% =btnMenuEncoderSelIds%>").css("font-weight", "normal");
                $("#btnMenuEncoderSel" + msg.d).css("font-weight", "bold");

            }
        });

        $("#deviceListSingleGroupChooseEncoderMenu").show().css("left", x).css("top", y);

    });



        $("<% =btnMenuEncoderSelIds%>").click(function (e) {

            var currentOperEncoder = e.currentTarget.id.replace("btnMenuEncoderSel", "");
            $.ajax({
                type: "POST",
                async: true,
                url: "BroadcastMain.aspx/SaveGroupEncoder",
                data: "{'cid':'" + currentOperEncoder + "',gid:'" + currentOperGroup + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                }
            });

            is_popup_2nd_menu = false;
            is_popup_1st_menu = false;

        });

        $("#btnSingleGroupChooseEncoder").mouseout(function (e) {
            is_popup_2nd_menu = false;
        });


        $.showGroupClickMenu = function () {

            $("<%= imgGroupShowIds %>").click(function (e) {

            hideAllNenus();

            currentOperGroup = e.currentTarget.id.replace("imgGroupShow", "");

            is_popup_1st_menu = true;

            var x = $(this).offset().left;
            var y = $(this).offset().top + $(this).height() + 2;




            $("#deviceListSingleGroupClickMenuBox").show().css("left", x).css("top", y);



       


        });

        $("<%= imgGroupShowIds %>").mouseout(function (e) {
            is_popup_1st_menu = false;
        });

        $(document).click(function () {

            if (!is_popup_1st_menu && !is_popup_2nd_menu) {

                hideAllNenus();
            }
        });

    }

        function hideAllNenus() {

            $("#deviceListSingleGroupClickMenuBox").hide();
            $("#deviceListSingleGroupChooseChannelMenu").hide()
            $("#encoderListEncoderClickMenuBox").hide();
            $("#channelListChannelClickMenuBox").hide();
            $("#channelListChannelChooseScheduleMenu").hide()
            $("#deviceListSingleGroupChooseEncoderMenu").hide();
            $("#deviceListDeviceMenu").hide();

        }

        $.showGroupClickMenu();


        $("#deviceListSingleDeviceMenuBtnOpenScreen").click(function (e) {

            var cmdStr = "122"

            chat.server.sendDeviceOperCommand(cmdStr, currentOperDeviceGroupId, currentOperDeviceIpAddress, "", "", false, "")


            hideAllNenus();
        });

        $("#deviceListSingleDeviceMenuBtnCloseScreen").click(function (e) {

            var cmdStr = "123"

            chat.server.sendDeviceOperCommand(cmdStr, currentOperDeviceGroupId, currentOperDeviceIpAddress, "", "", false, "")


            hideAllNenus();
        });


        $("#deviceListSingleDeviceMenuBtnRestartDevice").click(function (e) {

            var cmdStr = "124"

            chat.server.sendDeviceOperCommand(cmdStr, currentOperDeviceGroupId, currentOperDeviceIpAddress, "", "", false, "")


            hideAllNenus();
        });

        $("#deviceListSingleDeviceMenuBtnAdjustVol").click(function (e) {


            $.ajax({
                type: "POST",
                async: false,
                url: "BroadcastMain.aspx/GetDeviceVol",
                data: "{'ipAddress':'" + currentOperDeviceIpAddress + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {
                    $("#tbVolValue").val(msg.d);

                    $('#dialogForDeviveAdjustVol').modal('show');
                }
            });


            




         
        });


        $("#btnConfirmedDialogForDeviveAdjustVol").click(function (e) {

            $('#dialogForDeviveAdjustVol').modal('hide');
            // $("#tbVolValue").val("15");

            var tbVolValue = $("#tbVolValue").val();



            if (!(/^(\+|-)?\d+$/.test(tbVolValue)) || tbVolValue < 0 || tbVolValue > 15) {

                alert("请输入正确的音量值");
                return;

            }

            var cmdStr = "129"

            chat.server.sendDeviceOperCommand(cmdStr, currentOperDeviceGroupId, currentOperDeviceIpAddress, "", "", false, tbVolValue)


        });


        $("#deviceListSingleDeviceMenuBtnShutDownDevice").click(function (e) {

            var cmdStr = "125"

            chat.server.sendDeviceOperCommand(cmdStr, currentOperDeviceGroupId, currentOperDeviceIpAddress, "", "", false, "")


            hideAllNenus();
        });

        $("#deviceListSingleDeviceMenuBtnDelDevice").click(function (e) {
       

            chat.server.removeDevice( currentOperDeviceIpAddress)


            hideAllNenus();
        });

        

        $("#deviceListSingleDeviceMenuBtnScheduleDevice").click(function (e) {

            $('#dialogForDeviveSchdeduelGroup').modal('show');
        });

        $("#btnConfirmedDialogForDeviveSchdeduelGroup").click(function (e) {


            var tbScheduleTimeTurnOnValue = $("#tbScheduleTime").val();
            if (tbScheduleTimeTurnOnValue == "") {

                alert("计划开机时间不能为空");
                return
            }

            var tbScheduleShutDownTimeValue = $("#tbScheduleTurnOffTime").val();
            if (tbScheduleShutDownTimeValue == "") {

                alert("计划关机时间不能为空");
                return
            }




            chat.server.sendDeviceOperCommand(127, currentOperDeviceGroupId, currentOperDeviceIpAddress, tbScheduleTimeTurnOnValue, tbScheduleShutDownTimeValue, $("#ckcEnabledDeviceSchedule").is(':checked'), "")


            $('#dialogForDeviveSchdeduelGroup').modal('hide');
        });


        $("<%= deviceBatchOperDeviceIds %>").click(function (e) {


        hideAllNenus();
        currentOperDevice = "";

        currentOperDeviceGroupId = e.currentTarget.id.replace("btnDeviceBatchOper", "");

        currentOperDeviceIpAddress = "";
        is_popup_1st_menu = true;

        var x = $(this).offset().left;
        var y = $(this).offset().top + $(this).height() + 2;


        $("#deviceListDeviceMenu").show().css("left", x).css("top", y);

    });

    $("<%= deviceBatchOperDeviceIds %>").mouseout(function (e) {
            is_popup_1st_menu = false;
        });


        $.showSingleDeviceClickMenu = function () {

            $("<%= deviceIds %>").click(function (e) {

            is_popup_1st_menu = true;


            hideAllNenus();
            currentOperDeviceGroupId = "";
            currentOperDevice = e.currentTarget.id.replace("deviceMenu", "");
            currentOperDeviceIpAddress = e.currentTarget.name;


            var x = $(this).offset().left;
            var y = $(this).offset().top + $(this).height() + 2;


            $("#deviceListDeviceMenu").show().css("left", x).css("top", y);


        });

        $("<%= deviceIds %>").mouseout(function (e) {
            is_popup_1st_menu = false;
        });

        $(document).click(function () {

            if (!is_popup_1st_menu && !is_popup_2nd_menu) {
                hideAllNenus();
            }
        });

    }

        $.showSingleDeviceClickMenu();



        <%
               
     
    string groupDeviceListIds = string.Empty; for (int i = 0; i < dGroups.Count; i++) { groupDeviceListIds = groupDeviceListIds + "#groupDeviceList" + dGroups[i].GroupId + ","; }; groupDeviceListIds = groupDeviceListIds.TrimEnd(','); %>


        <%
    if (CheckIsSupperUser())
    {
        if (CheckIfCanDragGroups())
        {

              
               %>

        $("<%=groupDeviceListIds%>").dragsort({ dragSelector: "div", dragBetween: true, dragEnd: saveOrder, placeHolderTemplate: "<li class='placeHolder'><div></div></li>" });
         <% }
    } %>
        function saveOrder() {



            var a = $(this).data("itemid");


            var groupId = $(this)[0].parentNode.id;

            if (groupId != null) {
                groupId = groupId.replace("groupDeviceList", "");
            }


            $.ajax({
                type: "POST",
                async: false,
                url: "BroadcastMain.aspx/SaveDeviceGroup",
                data: "{'deivceId':'" + a + "','groupId':'" + groupId + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {


                }
            });




        };



    });


</script>


<div>
    <table class="table table-bordered table-striped; " border="0" style="overflow: auto; height: auto; margin-top: 0px; margin-bottom: 0px; margin-left: 0px;">

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

        <%--<div class="jumbotron"  style="overflow: auto; height: auto; margin-top: 5px;">--%>

        <tbody>

            <tr>
                <td>
                    <div style="height: 90px">
                        <ul id="deviceGroupList" class="deviceULStyle">
                            <%          
         
                                for (int k = 0; k < dGroups.Count; k++) %>
                            <%    {
                                      if (dGroups[k].GroupId == "-1")
                                      {
                                          continue;
                                      }
                            %>
                            <li data-itemid="<%=dGroups[k].GroupId %>">

                                <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">
                                        <p style="text-align: center">

                                            <% string strImageName = GetGroupImageUrl(); %>

                                            <img id="imgGroupShow<% =dGroups[k].GroupId %>" src="Images/<% =strImageName %>" style="width: 50px; height: 50px" />
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
            <tr style="height: 1px; padding: 0px">
                <td style="height: 1px; padding: 0px">
                    <hr style="width: 100%; height: 1px; border: 0; background-color: #CDCDC1; margin: 0px" />
                </td>
            </tr>

        </tbody>
    </table>



    <% 
        int deviceIndex = 0;

        int groupIndex = 0;

        for (int l = 0; l < dGroups.Count; l++)
        {


    %>



    <table class="table table-bordered table-striped; " border="0" style="border-bottom-width: 0px; overflow: auto; height: auto; margin-top: 0px; margin-bottom: 0px">

        <%--<div class="jumbotron"  style="overflow: auto; height: auto; margin-top: 5px;">--%>

        <thead>
            <tr>

                <th style="line-height: 30px">
                    <div class="row" style="margin-left: 0px">

                        <div class="pull-left"><%=dGroups[l].GroupName +CheckDeviceIsGroupPlaying(dGroups[l].GroupId)  %>   </div>


                        <% if (dGroups[l].GroupId != "-1")
                           {  %>
                        <div class="pull-right"><a class="btn  btn-success" id="btnDeviceBatchOper<% =dGroups[l].GroupId %>" name="<%=dGroups[l].GroupId %>" data-content="">批量操作</a></div>
                        <% }  %>
                    </div>

                </th>

            </tr>
        </thead>

        <tbody>

            <tr>
                <td>
                    <div style="height: 90px">
                        <ul id="groupDeviceList<%=dGroups[l].GroupId %>" class="deviceULStyle">
                            <%
                        
                           deviceIndex++;
                           if (dGroups[l].Devices != null && dGroups[l].Devices.Count > 0)
                           {
                               for (int k = 0; k < dGroups[l].Devices.Count; k++) %>
                            <%    {

                                    
                                   
                            %>
                            <li data-itemid="<%=dGroups[l].Devices[k].DeviceId %>">

                                <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">
                                        <p style="text-align: center">

                                            <% 

                                                
                                      string srcName = GetImageUrl(dGroups[l].Devices[k].DeviceIpAddress);

                                      srcName = ResolveUrl("~/Images/" + srcName + ".png");
                                            %>


                                            <img id="deviceMenu<% =dGroups[l].Devices[k].DeviceId %>" name="<%=dGroups[l].Devices[k].DeviceIpAddress %>" src="<%=srcName %>" style="width: 50px; height: 50px" />
                                        </p>
                                        <p style="text-align: center; font-size: 12px">
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

            <tr style="height: 1px; padding: 0px">
                <td style="height: 1px; padding: 0px">

                    <% if (l == dGroups.Count - 1)
                       { %>
                    <hr style="width: 100%; height: 1px; border: 0; background-color: #CDCDC1; margin: 0px" />
                    <% }
                       else
                       { %>
                    <hr style="width: 100%; height: 1px; border: 0; background-color: #CDCDC1; margin-top: 0px; margin-bottom: 0px; margin-left: 5px; margin-right: 5px" />
                    <% } %>
                </td>
            </tr>
        </tbody>
    </table>

    <%
                       groupIndex++;
        } %>





    <div id="dialogForBatchGroup" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForBatchGroup').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">分组</h3>
        </div>
        <div class="modal-body">
            <div style="float: left; height: 185px; width: 160px; margin-right: 10px">

                <h4 style="text-align: left; margin-top: 0px">可选组</h4>

                <select size="4" multiple="multiple" style="height: 136px; width: 150px;" id="dialogForBatchGrouplbAvaiableGroups">

                    <% foreach (var di in dGroups)
                       {
                           if (di.GroupId == "-1") { continue; } %>
                    <option value="<%= di.GroupId %>"><%= di.GroupName %></option>

                    <% 
                       }
                    %>
                </select>
            </div>


            <div style="float: left; height: 220px; width: 220px;">
                <h4 style="text-align: left; margin-top: 0px">批量操作</h4>

                <div>
                    <div class="batchOperationLableStyle">
                        <p>
                            通道选择
                        </p>
                    </div>
                    <select id="ddBatchSelectChannel" style="width: 220px">

                        <% foreach (var ci in channels)
                           { %>
                        <option value="<%= ci.ChannelId %>"><%= ci.ChannelName %></option>

                        <% 
                           }
                        %>
                    </select>
                </div>
                <div>
                    <div class="batchOperationLableStyle">
                        <p>
                            视频源选择:
                        </p>
                    </div>
                    <select id="ddBatchSelectVideoSorce" style="width: 220px" name="selectTest">

                        <%  foreach (var ei in encoders)
                            { %>
                        <option value="<%= ei.EncoderId %>"><% =ei.EncoderName %></option>
                        <% } %>
                    </select>
                </div>



            </div>

        </div>

        <div class="modal-footer">
            <a id="btnConfirmedBatchGroupOperation" class="btn primary">确认</a>
        </div>
    </div>


    <div id="dialogForDeviveAdjustVol" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForDeviveAdjustVol').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">分组</h3>
        </div>
        <div class="modal-body">

            <h4 style="text-align: left; margin-top: 0px">音量调节</h4>


            <div>

                <p>
                    音量值:（0-15)
                </p>

                <div>
                    <input type="text" runat="server" style="width: 220px" id="tbVolValue" />
                </div>
            </div>


        </div>

        <div class="modal-footer">
            <a id="btnConfirmedDialogForDeviveAdjustVol" class="btn primary">确认</a>
        </div>
    </div>


    <div id="dialogForDeviveSchdeduelGroup" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForDeviveSchdeduelGroup').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">分组</h3>
        </div>
        <div class="modal-body">

            <h4 style="text-align: left; margin-top: 0px">计划</h4>


            <div id="divPlanTime">

                <p>
                    计划开机时间:
                </p>


                <div>
                    <input type="text" runat="server" style="width: 220px" id="tbScheduleTime" />
                </div>
            </div>



            <div id="divPlanShutTimeTime">

                <p>
                    计划关机时间:
                </p>


                <div>
                    <input type="text" runat="server" style="width: 220px" id="tbScheduleTurnOffTime" />
                </div>
            </div>

            <div style="vertical-align: middle">

                <input type="checkbox" id="ckcEnabledDeviceSchedule" style="float: left; vertical-align: middle" />
                <label for="ckcBatchOpenVideoSource" style="line-height: 21px">&nbsp;是否有效</label>
                <br />

            </div>



        </div>

        <div class="modal-footer">
            <a id="btnConfirmedDialogForDeviveSchdeduelGroup" class="btn primary">确认</a>
        </div>
    </div>


    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="deviceListSingleGroupClickMenuBox">
        <li><a class="btn" id="btnSingleGroupChooseChannel" data-backdrop="static" data-dismiss="modal" data-keyboard="false">通道选择</a></li>
        <li><a class="btn" id="btnSingleGroupChooseEncoder" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">视频源选择</a></li>

    </ul>

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="deviceListSingleGroupChooseChannelMenu">

        <% 
            for (int i = 0; i < channels.Count; i++)
            { 
        %>
        <li><a class="btn" style="margin-bottom: 3px; font-weight: normal" name="<% =channels[i].ChannelName %>" id="btnMenuChannelSel<% =channels[i].ChannelId %>" data-backdrop="static" data-dismiss="modal" data-keyboard="false"><% =channels[i].ChannelName %></a></li>
        <%}%>
    </ul>

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="deviceListSingleGroupChooseEncoderMenu">

        <% 
            for (int i = 0; i < encoders.Count; i++)
            { 
        %>
        <li><a class="btn  btn-default" style="margin-bottom: 3px; font-weight: normal" name="<% =encoders[i].EncoderName %>" id="btnMenuEncoderSel<% =encoders[i].EncoderId %>" data-backdrop="static" data-dismiss="modal" data-keyboard="false"><% =encoders[i].EncoderName %></a></li>
        <%}%>
    </ul>

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="deviceListDeviceMenu">
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnAdjustVol" data-backdrop="static" data-dismiss="modal" data-keyboard="false">音量调节</a></li>


        <li><a class="btn" id="deviceListSingleDeviceMenuBtnRestartDevice" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">重启设备</a></li>

        <li><a class="btn" id="deviceListSingleDeviceMenuBtnShutDownDevice" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">关闭设备</a></li>

             <%
    if (CheckIsSupperUser())
    {
                 %>
            <li><a class="btn" id="deviceListSingleDeviceMenuBtnDelDevice" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">删除设备</a></li>

        <% } %>
      <%--  <li><a class="btn" id="deviceListSingleDeviceMenuBtnScheduleDevice" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">计划开关机</a></li>--%>



    </ul>


    <!-- save sort order here which can be retrieved on server on postback -->



</div>

