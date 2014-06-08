<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DeviceList.ascx.cs" Inherits="MediaMgrSystem.DeviceList" %>

<script type="text/javascript" src="Scripts/jquery-1.10.2.js"></script>
<script type="text/javascript" src="Scripts/jquery.timer.js"></script>
<script type="text/javascript" src="Scripts/bootstrap.js"></script>
<script type="text/javascript" src="Scripts/jquery.dragsort-0.5.1.min.js"></script>
<script type="text/javascript" src="Scripts/jquery-ui-1.10.4.custom.js"></script>
<script type="text/javascript" src="Scripts/utils.js"></script>



<script src="Scripts/jquery-1.6.4.min.js"></script>

<script src="Scripts/jquery.signalR-2.0.3.min.js"></script>


<script src="signalr/hubs"></script>

<link rel="stylesheet" href="Content/jquery-ui-1.10.4.custom.min.css" type="text/css" />

<link rel="stylesheet" href="Content/jquery-ui-1.10.4.custom.css" type="text/css" />

<link rel="stylesheet" href="Content/DeviceList.css" type="text/css" />

<webopt:BundleReference runat="server" Path="~/Content/css" />

<script type="text/javascript">

    var currentOperDeviceDevice;
    var currentOperGroupDevice;
    var opDevices = new Array()
    var opGuidIds = new Array()
    var opSleepTimers = new Array();
    var opSleepGuidIds = new Array()


    $(document).ready(function () {

        <%   List<MediaMgrSystem.DataModels.GroupInfo> dGroups = GetAllGroups();%>

        var parameters = new Array();

        var parameterClientIdentify = new Array();

        var parameterClientType = new Array();
        //-------------------------------Singalr 

        strclientIdentify = guid();

        parameterClientIdentify.push("clientIdentify");
        parameterClientIdentify.push(strclientIdentify);

        parameterClientType.push("clientType");
        parameterClientType.push("PC");

        parameters.push(parameterClientIdentify);
        parameters.push(parameterClientType);


        //chat.client.sendAllMessge = function (result) {

        //    $("#divLogs").append("God" + result);
        //}





        ///-------------------------------------------



        //---------------------------------group menu
        $.showGroupmenu = function () {

            <% string ids = string.Empty; for (int i = 0; i < dGroups.Count; i++) { ids = ids + "#btnGroupOper" + i.ToString() + ","; }; ids = ids.TrimEnd(','); %>

            var is_in = false;
            $("<%= ids %>").click(function (e) {

                is_in = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#GroupMenubox").show().css("left", x).css("top", y);

                currentOperGroupDevice = e.currentTarget.name;

            });

            $("<%= ids %>").mouseout(function (e) {
                is_in = false;
            });


            $(document).click(function () {
                if (!is_in) $("#GroupMenubox").hide();
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

            <% string deviceIds = string.Empty; for (int i = 1; i <= 9; i++) { ids = ids + "#deviceMenu" + i.ToString() + ","; }; ids = ids.TrimEnd(','); %>

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

        $("#btnBatchGroupOper").click(function (e) {
            $('#dialogForBatchOperGroup').modal('show');
        });


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
               
     
    string deviceLists = string.Empty; for (int i = 0; i < 2; i++) { deviceLists = deviceLists + "#deviceList" + i.ToString() + ","; }; deviceLists = deviceLists.TrimEnd(','); %>



        $("<%=deviceLists%>").dragsort({ dragSelector: "div", dragBetween: true, dragEnd: saveOrder, placeHolderTemplate: "<li class='placeHolder'><div></div></li>" });

        function saveOrder() {

            var data = $("#deviceList1 li").map(function () { return $(this).data("itemid") }).get();
            $("input[name=list1SortOrder]").val(data.join("|"));
        };

    });





</script>






<div>



    <% 
        int deviceIndex = 0;

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
                        <div class="pull-right"><a class="btn  btn-success" id="btnGroupOper<% =l.ToString() %>" name="<%=dGroups[l].GroupId %>" data-content="">操作</a></div>

                    </div>

                </th>

            </tr>
        </thead>

        <tbody>

            <tr>
                <td>
                    <div style="height: 90px">
                        <ul id="deviceList<%=deviceIndex %>" class="deviceULStyle">
                            <%
            deviceIndex++;
            if (dGroups[l].Devices != null && dGroups[l].Devices.Count > 0)
            {
                for (int k = 0; k < dGroups[l].Devices.Count; k++) %>
                            <%    {
                            %>
                            <li data-itemid="<%=dGroups[l].Devices[k].DeviceName %>">

                                <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">
                                        <p style="text-align: center">
                                            <img id="deviceMenu<% =deviceIndex.ToString() %>" name="<%=dGroups[l].Devices[k].DeviceIpAddress %>" src="Images/ic_image_device.png" style="width: 50px; height: 50px" />
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

    <% } %>





    <table class="table table-bordered table-striped; " style="overflow: auto; height: auto; margin-top: 5px;">

        <thead>
            <tr>

                <th>
                    <div class="row" style="margin-left: 0px">

                        <div class="pull-left">分组信息</div>
                        <div class="pull-right"><a class="btn  btn-success" id="btnBatchGroupOper" data-content="">批量操作</a></div>

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
                            %>
                            <li data-itemid="<%=dGroups[k].GroupId %>">

                                <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">
                                        <p style="text-align: center">
                                            <img id="deviceGroup<% =k.ToString() %>" src="Images/ic_image_device.png" style="width: 50px; height: 50px" />
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


    <div id="wei" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#wei').modal('hide');" title="关闭小窗口">&times;</a><h3>Modal标题</h3>
        </div>
        <div class="modal-body">
            <select size="4" name="lb_list" multiple="multiple" id="lb_list" style="height: 200px; width: 150px;">

                <option value="1">后台登录</option>

                <option value="2">密码修改</option>

                <option value="3">新闻添加</option>

                <option value="4">新闻编辑</option>

                <option value="5">新闻删除</option>

                <option value="6">新闻发布</option>


            </select>
            <select size="4" multiple="multiple" id="lb_list2" style="height: 200px; width: 150px;">
            </select>

            <input type="button" id="btnTest" value="asfd" />
        </div>
        <div class="modal-footer">
            <a class="btn primary">按钮一个</a>
        </div>
    </div>

    <%--    <div id="dia8log" title="Basic dialog" style="background-color: red">

     
    </div>--%>



    <div id="dialogForBatchOperGroup" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForBatchOperGroup').modal('hide');" title="关闭">&times;</a><h3 style="text-align: center">分组</h3>
        </div>
        <div class="modal-body">
            <div style="float: left; height: 200px; width: 160px; margin-right: 10px">

                <h4 style="text-align: left; margin-top: 0px">可选节目</h4>

                <select size="4" multiple="multiple" style="height: 160px; width: 150px;" id="lbAvaiableGroups">
                    
                    <option value="1">后台登录</option>

                    <option value="2">密码修改</option>

                    <option value="3">新闻添加</option>

                    <option value="4">新闻编辑</option>

                    <option value="5">新闻删除</option>

                    <option value="6">新闻发布</option>

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
                    <select id="ddBatchSelectChannel">
                        <option value="1">11</option>
                        <option value="2">22</option>
                        <option value="3">33</option>
                        <option value="4">44</option>
                        <option value="5">55</option>
                        <option value="6">66</option>
                    </select>
                </div>
                <div>
                    <div class="batchOperationLableStyle">
                        <p>
                            视频源选择:
                        </p>
                    </div>
                    <select id="ddBatchSelectVideoSorce" name="selectTest">
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
                    <label for="ckcBatchOpenVideoSource">打开视频源</label>
                    <br />

                </div>

            </div>

        </div>

        <div class="modal-footer">
            <a class="btn primary">确认</a>
        </div>
    </div>

    

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="DeviceMenubox">

        <li><a class="btn" id="DeviceMenulistPlayVideo" data-backdrop="static" data-dismiss="modal" data-keyboard="false">播放视频</a></li>
        <li><a class="btn" id="DeviceMenulistPauseVideo" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">暂停播放</a></li>

    </ul>



    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="GroupMenubox">

        <li><a class="btn" id="GroupMenulistPlayVideo" data-backdrop="static" data-dismiss="modal" data-keyboard="false">播放视频</a></li>
        <li><a class="btn" id="GroupMenulistPauseVideo" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">暂停播放</a></li>

    </ul>



    <!-- save sort order here which can be retrieved on server on postback -->

    <div id="divLogs" style="width: 300px; height: 200px" class="pull-left">asdf</div>
    <div style="clear: both;">
    </div>



</div>
</div>
