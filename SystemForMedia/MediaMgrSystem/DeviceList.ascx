<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DeviceList.ascx.cs" Inherits="MediaMgrSystem.DeviceList" %>



<script type="text/javascript">



    var currentOperGroup;

    $(document).unbind();

    $(document).ready(function () {

        <%  
    List<MediaMgrSystem.DataModels.GroupInfo> dGroups = GetAllGroupsIncludeDefaultGroup();
    List<MediaMgrSystem.DataModels.ChannelInfo> channels = GetAllChannels();
    string deviceIds = string.Empty; for (int i = 1; i <= 1; i++) { deviceIds = deviceIds + "#deviceMenu" + i.ToString() + ","; }; deviceIds = deviceIds.TrimEnd(',');
    string imgGroupShowIds = string.Empty; for (int i = 0; i < dGroups.Count; i++) { if (dGroups[i].GroupId == "-1") { continue; } imgGroupShowIds = imgGroupShowIds + "#imgGroupShow" + dGroups[i].GroupId + ","; }; imgGroupShowIds = imgGroupShowIds.TrimEnd(',');

    string btnMeneChannelSelIds = string.Empty; for (int i = 0; i < channels.Count; i++) { btnMeneChannelSelIds = btnMeneChannelSelIds + "#btnMeneChannelSel" + channels[i].ChannelId + ","; }; btnMeneChannelSelIds = btnMeneChannelSelIds.TrimEnd(',');
    
             %>



        chat.client.sendResultBrowserClient = function (result, error) {

            // alert(result);
            $("#divLogs").append('<br/>' + result);
        }


        //  $.showGroupmenu = function () {

        <%--
            var is_group_menu_in = false;
            $("<%= ids %>").click(function (e) {

                is_group_menu_in = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#GroupMenubox").show().css("left", x).css("top", y);

                currentOperGroupDevice = e.currentTarget.name;


                $(e.currentTarget).mouseout(function (e) {

                    is_group_menu_in = false;
                });


            });



            $(document).click(function () {

                if (!is_group_menu_in) $("#GroupMenubox").hide();
            });

            $("#GroupMenulistPlayVideo").click(function (e) {

                var guidId = guid();
                chat.server.sendVideoControlMessage("PC");

            });

            $("#GroupMenulistPauseVideo").click(function (e) {

                var guidId = guid();
                chat.server.sendVideoControlPauseMessage("PC");

            });
        }


        $.showGroupmenu();

        //---------------------------------------

        $.showDevicemenu = function () {


            var is_in = false;
            $("<%= deviceIds %>").click(function (e) {



                is_in = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#DeviceMenubox").show().css("left", x).css("top", y);


                //  currentSelectedDevice = e.currentTarget.name;;


            });

            $("<%= ids %>").mouseout(function (e) {
                is_in = false;
            });

            $(document).click(function () {
                if (!is_in) $("#DeviceMenubox").hide();
            });
        }

        $.showDevicemenu();
--%>

        $("#btnConfirmedBatchGroupOperation").click(function (e) {


            $("#dialogForBatchGrouplbAvaiableGroups option:selected").each(function () {


                var selectedGroupId = $(this).val();

                var selectedChannelId = $('#ddBatchSelectChannel option:selected').val();


                $.ajax({
                    type: "POST",
                    async: false,
                    url: "Default.aspx/SaveGroupChannel",
                    data: "{'cid':'" + selectedChannelId + "',gid:'" + selectedGroupId + "'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {


                    }
                });

                // $(this).remove();
            })


            $('#dialogForBatchGroup').modal('hide');
        });
        $("#btnBatchGroupOperation").click(function (e) {

            $('#dialogForBatchGroup').modal('show');
        });



        var isChooseChannel = false;
        $("#btnSingleGroupChooseChannel").click(function (e) {


            isChooseChannel = true;


            var x = $(this).offset().left + $("#channelMenuForSignleGroupBox").width() + 6;
            var y = $(this).offset().top;


            $.ajax({
                type: "POST",
                async: false,
                url: "Default.aspx/GetChannelByGroupId",
                data: "{'gid':'" + currentOperGroup + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                    $("<% =btnMeneChannelSelIds%>").css("font-weight", "normal");
                    $("#btnMeneChannelSel" + msg.d).css("font-weight", "bold");

                }
            });



          <%--  <% int a=GetSelectedSchedule(%> e.target.name <% )%>

            alert(<%=i%>);--%>

            $("#channelMenuForSignleGroupBox").show().css("left", x).css("top", y);


        });


        $("<% =btnMeneChannelSelIds%>").click(function (e) {


            var currentOperChannel = e.currentTarget.id.replace("btnMeneChannelSel", "");

            $.ajax({
                type: "POST",
                async: true,
                url: "Default.aspx/SaveGroupChannel",
                data: "{'cid':'" + currentOperChannel + "',gid:'" + currentOperGroup + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                }
            });

        });


        $("#btnSingleGroupChooseChannel").mouseout(function (e) {
            isChooseChannel = false;
        });


        $.showGroupClickMenu = function () {




            $("<%= imgGroupShowIds %>").click(function (e) {


                $("#menuForSingleGroup").hide();
                $("#channelMenuForSignleGroupBox").hide()

                currentOperGroup = e.currentTarget.id.replace("imgGroupShow", "");

                is_in = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#menuForSingleGroup").show().css("left", x).css("top", y);



            });

            $("<%= imgGroupShowIds %>").mouseout(function (e) {
                is_in = false;
            });

            $(document).click(function () {
                // debugger;
                if (!is_in && !isChooseChannel) {
                    $("#menuForSingleGroup").hide();
                    $("#channelMenuForSignleGroupBox").hide()
                }
                if (!is_in && !isChooseSchedule) {
                    $("#ChannelMenubox").hide();
                    $("#SchduleBox").hide()
                }
            });

            //debugger;
            //$(document).click(function () {
            //    debugger;
            //    if (!is_in && !isChooseSchedule) {
            //        $("#ChannelMenubox").hide();
            //        $("#SchduleBox").hide()
            //    }
            //});
        }

        $.showGroupClickMenu();



        //------------------------------------


        //$("#btnGroupAllPause").click(function (e) {
        //    //   $("#divLogs").html("操作中...");
        //    //  opDevices.push(currentSelectedDevice);
        //    var guidId = guid();
        //    chat.server.sendControlMessage("TO", "AOFF");
        //    opGuidIds.push(guidId);

        //    processControlTimeOut(guidId);


        //});
        //$("#btnGroupOper").click(function (e) {
        //    //   $("#divLogs").html("操作中...");
        //    //  opDevices.push(currentSelectedDevice);
        //    //  var guidId = guid();
        //    chat.server.sendControlMessage("TO", "A");
        //    //   opGuidIds.push(guidId);


        //});




        //$("#DeviceMenulistPauseVideo").click(function (e) {
        //    $("#divLogs").html("操作中...");
        //    //  debugger;
        //    opDevices.push(currentSelectedDevice);
        //    var guidId = guid();
        //    chat.server.sendControlMessage("TP", currentSelectedDevice);
        //    opGuidIds.push(guidId);

        //    processControlTimeOut(guidId);
        //    //setTimeout("showTime()", 5000);

        //    ////$.sleep(8, function (arg) {
        //    ////    debugger;
        //    ////    var exstingIndex = -1;
        //    ////    for (var i = 0; i < opGuidIds.length; i++) {
        //    ////        if (opGuidIds[i] == arg) {
        //    ////            exstingIndex = i
        //    ////        }
        //    ////    }
        //    ////    if (exstingIndex >= 0) {



        //    ////        $("#divLogs").html(opDevices[exstingIndex] + "操作失败");

        //    ////        opDevices.splice(exstingIndex, 1);
        //    ////        opGuidIds.splice(exstingIndex, 1);
        //    ////    }
        //    ////}, guidId);
        //});
        //$("#DeviceMenulistPlayVideo").click(function (e) {
        //    $("#divLogs").html("操作中...");
        //    opDevices.push(currentSelectedDevice);
        //    var guidId = guid();
        //    chat.server.sendControlMessage("TO", currentSelectedDevice);
        //    opGuidIds.push(guidId);


        //    //setTimeout("showTime()", 5000);

        //    ////$.sleep(8, function (arg) {
        //    ////    debugger;
        //    ////    var exstingIndex = -1;
        //    ////    for (var i = 0; i < opGuidIds.length; i++) {
        //    ////        if (opGuidIds[i] == arg) {
        //    ////            exstingIndex = i
        //    ////        }
        //    ////    }
        //    ////    if (exstingIndex >= 0) {



        //    ////        $("#divLogs").html(opDevices[exstingIndex] + "操作失败");

        //    ////        opDevices.splice(exstingIndex, 1);
        //    ////        opGuidIds.splice(exstingIndex, 1);
        //    ////    }
        //    ////}, guidId);
        //});





        <%
               
     
    string groupDeviceListIds = string.Empty; for (int i = 0; i < dGroups.Count; i++) { groupDeviceListIds = groupDeviceListIds + "#groupDeviceList" + i.ToString() + ","; }; groupDeviceListIds = groupDeviceListIds.TrimEnd(','); %>


        $("<%=groupDeviceListIds%>").dragsort({ dragSelector: "div", dragBetween: true, dragEnd: saveOrder, placeHolderTemplate: "<li class='placeHolder'><div></div></li>" });

        function saveOrder() {

            // debugger;


            <%  for (int i = 0; i < dGroups.Count; i++)
                {
                     %>

            var stringData = "";
            $("#groupDeviceList<%=i.ToString()%> li").map(function () {
                stringData += $(this).data("itemid") + ",";
            })
            var a = stringData;


            $.ajax({
                type: "POST",
                async: false,
                url: "Default.aspx/SaveDeviceGroup",
                data: "{'deivceId':'" + a + "','groupId':'<% =dGroups[i].GroupId %>'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {


                }
            });


            <% 
                }
           %>


        };

    });




</script>


<div>



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

                <th>
                    <div class="row" style="margin-left: 0px">

                        <div class="pull-left"><%=dGroups[l].GroupName %></div>
                        <% if (dGroups[l].GroupId != "-1")
                           {  %>
                        <div class="pull-right"><a class="btn  btn-success" id="btnDeviceBatchOper<% =l.ToString() %>" name="<%=dGroups[l].GroupId %>" data-content="">操作</a></div>
                        <% }  %>
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

                                            <% string srcName = "Images/ic_image_device.png";

                                               if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                               {
                                                   srcName = "Images/ic_image_device_offline.png";
                                               }
                                               
                                            %>

                                            <img id="deviceMenu<% =deviceIndex.ToString() %>" name="<%=dGroups[l].Devices[k].DeviceIpAddress %>" src="<%=srcName %>" style="width: 50px; height: 50px" />
                                        </p>
                                        <p id="ptext" style="text-align: center">
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


    <table class="table table-bordered table-striped; " style="overflow: auto; height: auto; margin-top: 5px;">

        <thead>
            <tr>

                <th>
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


                                            <img id="imgGroupShow<% =dGroups[k].GroupId %>" src="Images/ic_image_group.png" style="width: 50px; height: 50px" />
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



    <div id="dialogForBatchGroup" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForBatchGroup').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">分组</h3>
        </div>
        <div class="modal-body">
            <div style="float: left; height: 185px; width: 160px; margin-right: 10px">

                <h4 style="text-align: left; margin-top: 0px">可选组</h4>

                <select size="4" multiple="multiple" style="height: 160px; width: 150px;" id="dialogForBatchGrouplbAvaiableGroups">


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
                <h4 style="text-align: left; margin-top: 0px">操作</h4>

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
                        <option value="1">11</option>
                        <option value="2">22</option>
                        <option value="3">33</option>
                        <option value="4">44</option>
                        <option value="5">55</option>
                        <option value="6">66</option>
                    </select>
                </div>

                <div style="vertical-align: middle">


                    <input type="checkbox" id="ckcBatchOpenVideoSource" style="float: left; vertical-align: middle" />
                    <label for="ckcBatchOpenVideoSource" style="line-height: 21px">&nbsp;打开视频源</label>
                    <br />

                </div>

            </div>

        </div>

        <div class="modal-footer">
            <a id="btnConfirmedBatchGroupOperation" class="btn primary">确认</a>
        </div>
    </div>



    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="menuForSingleGroup">
        <li><a class="btn" id="btnSingleGroupChooseChannel" data-backdrop="static" data-dismiss="modal" data-keyboard="false">通通选择</a></li>
        <li><a class="btn" id="btnSingleGroupChooseEncoder" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">视频源选择</a></li>

    </ul>

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="channelMenuForSignleGroupBox">

        <% 
            for (int i = 0; i < channels.Count; i++)
            { 
        %>
        <li><a class="btn" style="margin-bottom: 3px; font-weight: normal" name="<% =channels[i].ChannelName %>" id="btnMeneChannelSel<% =channels[i].ChannelId %>" data-backdrop="static" data-dismiss="modal" data-keyboard="false"><% =channels[i].ChannelName %></a></li>
        <%}%>
    </ul>

    <%--    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="DeviceMenubox">

        <li><a class="btn" id="DeviceMenulistPlayVideo" data-backdrop="static" data-dismiss="modal" data-keyboard="false">播放视频</a></li>
        <li><a class="btn" id="DeviceMenulistPauseVideo" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">暂停播放</a></li>

    </ul>



    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="GroupMenubox">

        <li><a class="btn" id="GroupMenulistPlayVideo" data-backdrop="static" data-dismiss="modal" data-keyboard="false">播放视频</a></li>
        <li><a class="btn" id="GroupMenulistPauseVideo" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">暂停播放</a></li>

    </ul>--%>



    <!-- save sort order here which can be retrieved on server on postback -->

    <div id="divLogs" style="width: 300px; height: 200px" class="pull-left"></div>
    <div style="clear: both;">
    </div>



</div>

