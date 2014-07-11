<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RemoteDeviceList.ascx.cs" Inherits="MediaMgrSystem.RemoteDeviceList" %>


<script type="text/javascript">


    var currentOperGroup;

    $(document).unbind();

    $(document).ready(function () {

        <%  

    
    List<MediaMgrSystem.DataModels.GroupInfo> dGroups = GetAllGroupsIncludeDefaultGroup();
    List<MediaMgrSystem.DataModels.ChannelInfo> channels = GetAllChannels();

    List<MediaMgrSystem.DataModels.EncoderInfo> encoders = GetAllEncoders();




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
                    url: "AudioBroadcastMain.aspx/SaveGroupChannelAndEncoder",
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

            $('#dialogForBatchGroup').modal('show');
        });



        $("#btnSingleGroupChooseChannel").click(function (e) {

            $("#deviceListSingleGroupChooseEncoderMenu").hide();

            is_popup_2nd_menu = true;

            var x = $(this).offset().left + $("#deviceListSingleGroupChooseChannelMenu").width() + 6;
            var y = $(this).offset().top;

            $.ajax({
                type: "POST",
                async: false,
                url: "AudioBroadcastMain.aspx/GetChannelByGroupId",
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
                url: "AudioBroadcastMain.aspx/SaveGroupChannel",
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



            $("#deviceListSingleGroupChooseChannelMenu").hide()


            is_popup_2nd_menu = true;

            var x = $(this).offset().left + $("#deviceListSingleGroupChooseEncoderMenu").width() + 6;
            var y = $(this).offset().top;


            $.ajax({
                type: "POST",
                async: false,
                url: "AudioBroadcastMain.aspx/GetEncoderByGroupId",
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
                url: "AudioBroadcastMain.aspx/SaveGroupEncoder",
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
            $("#deviceListSingleDeviceMenu").hide();

        }

        $.showGroupClickMenu();


        $.showSingleDeviceClickMenu = function () {

            $("<%= deviceIds %>").click(function (e) {


                hideAllNenus();

                currentOperGroup = e.currentTarget.id.replace("deviceMenu", "");

                is_popup_1st_menu = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;


                $("#deviceListSingleDeviceMenu").show().css("left", x).css("top", y);


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
               
     
    string groupDeviceListIds = string.Empty; for (int i = 0; i < dGroups.Count; i++) { groupDeviceListIds = groupDeviceListIds + "#groupDeviceList" + i.ToString() + ","; }; groupDeviceListIds = groupDeviceListIds.TrimEnd(','); %>


        $("<%=groupDeviceListIds%>").dragsort({ dragSelector: "div", dragBetween: true, dragEnd: saveOrder, placeHolderTemplate: "<li class='remoteControlDevicPplaceHolder'><div></div></li>" });

        function saveOrder() {


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
                url: "AudioBroadcastMain.aspx/SaveDeviceGroup",
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

                <th style="line-height: 30px">
                    <div class="row" style="margin-left: 0px">

                        <div class="pull-left"><%=dGroups[l].GroupName %></div>
                        <% if (dGroups[l].GroupId != "-1")
                           {  %>
                        <div class="pull-right"><a class="btn  btn-success" id="btnDeviceBatchOper<% =l.ToString() %>" name="<%=dGroups[l].GroupId %>" data-content="">批量操作</a></div>
                        <% }  %>
                    </div>

                </th>

            </tr>
        </thead>

        <tbody>

            <tr>
                <td>
                    <div style="height: 380px">
                        <ul id="groupDeviceList<%=groupIndex %>" class="remoteControlDeviceListULStyle">
                            <%
                           deviceIndex++;
                           if (dGroups[l].Devices != null && dGroups[l].Devices.Count > 0)
                           {
                               for (int k = 0; k < dGroups[l].Devices.Count; k++) %>
                            <%    {
                            %>
                            <li data-itemid="<%=dGroups[l].Devices[k].DeviceId %>">

                                <table class="table table-bordered table-striped; " style="overflow: auto; height: auto;">



                                    <thead style="border-radius:0px" >
                                        <tr >

                                            <th colspan="3" style="border-bottom-left-radius:0px" >


                                                <div style="text-align:center" >教室1</div>


                                            </th>

                                        </tr>
                                    </thead>



                                    <tbody>
                                        <tr>
                                            <td>
                                                <div class="col-md-4">
                                                    <p style="text-align: center">

                                                        <% string src8Name = "../Images/ic_image_device.png";

                                                           if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                           {
                                                               src8Name = "../Images/ic_image_device_offline.png";
                                                           }                                  
                                               
                                                        %>


                                                        <img id="devic99889eMenu<% =deviceIndex.ToString() %>" src="<%=src8Name %>" style="width: 50px; height: 50px" />
                                                    </p>
                                                    <p style="text-align: center">
                                                        <% =dGroups[l].Devices[k].DeviceName %>
                                                    </p>
                                                </div>
                                            </td>

                                            <td>
                                                <div class="col-md-4">
                                                    <p style="text-align: center">

                                                        <% string src88Name = "../Images/ic_image_device.png";

                                                           if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                           {
                                                               src88Name = "../Images/ic_image_device_offline.png";
                                                           }                                  
                                               
                                                        %>



                                                        <img id="devi88c999eMenu<% =deviceIndex.ToString() %>" src="<%=src88Name %>" style="width: 50px; height: 50px" />
                                                    </p>
                                                    <p style="text-align: center">
                                                        <% =dGroups[l].Devices[k].DeviceName %>
                                                    </p>
                                                </div>
                                            </td>

                                            <td>
                                                <div class="col-md-4">
                                                    <p style="text-align: center">

                                                        <% string src888Name = "../Images/ic_image_device.png";

                                                           if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                           {
                                                               src888Name = "../Images/ic_image_device_offline.png";
                                                           }                                  
                                               
                                                        %>



                                                        <img id="devic99999eMenu<% =deviceIndex.ToString() %>" src="<%=src88Name %>" style="width: 50px; height: 50px" />
                                                    </p>
                                                    <p style="text-align: center">
                                                        <% =dGroups[l].Devices[k].DeviceName %>
                                                    </p>
                                                </div>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td>
                                                <div class="col-md-4">
                                                    <p style="text-align: center">

                                                        <% string sr99c89Name = "../Images/ic_image_device.png";

                                                           if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                           {
                                                               sr99c89Name = "../Images/ic_image_device_offline.png";
                                                           }                                  
                                               
                                                        %>


                                                        <img id="dev99ic99889eMenu<% =deviceIndex.ToString() %>" src="<%=src8Name %>" style="width: 50px; height: 50px" />
                                                    </p>
                                                    <p style="text-align: center">
                                                        <% =dGroups[l].Devices[k].DeviceName %>
                                                    </p>
                                                </div>
                                            </td>

                                            <td>
                                                <div class="col-md-4">
                                                    <p style="text-align: center">

                                                        <% string dsaf = "../Images/ic_image_device.png";

                                                           if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                           {
                                                               src88Name = "../Images/ic_image_device_offline.png";
                                                           }                                  
                                               
                                                        %>



                                                        <img id="dev88i88c999eMenu<% =deviceIndex.ToString() %>" src="<%=dsaf %>" style="width: 50px; height: 50px" />
                                                    </p>
                                                    <p style="text-align: center">
                                                        <% =dGroups[l].Devices[k].DeviceName %>
                                                    </p>
                                                </div>
                                            </td>

                                            <td>
                                                <div class="col-md-4">
                                                    <p style="text-align: center">

                                                        <% string src88999998Name = "../Images/ic_image_device.png";

                                                           if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                           {
                                                               src888Name = "../Images/ic_image_device_offline.png";
                                                           }                                  
                                               
                                                        %>



                                                        <img id="devic988889999eMenu<% =deviceIndex.ToString() %>" src="<%=src88999998Name %>" style="width: 50px; height: 50px" />
                                                    </p>
                                                    <p style="text-align: center">
                                                        <% =dGroups[l].Devices[k].DeviceName %>
                                                    </p>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>

                                <%--    <div class="row" style="margin-left: 0px">
                                    <div class="col-md-4">

                                        <div style="float: left; height: 160px">


                                            <p style="text-align: center">

                                                <% string srcName = "../Images/ic_image_device.png";

                                                   if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                                   {
                                                       srcName = "../Images/ic_image_device_offline.png";
                                                   }                                  
                                               
                                                %>



                                                <img id="device7Menu<% =deviceIndex.ToString() %>" name="<%=dGroups[l].Devices[k].DeviceIpAddress %>" src="<%=srcName %>" style="width: 50px; height: 50px" />
                                            </p>
                                        </div>
                                        <p style="text-align: center">
                                            <% =dGroups[l].Devices[k].DeviceName %>
                                        </p>

                                        <p style="text-align: center">

                                            <% string src77Name = "../Images/ic_image_device.png";

                                               if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                               {
                                                    src77Name = "../Images/ic_image_device_offline.png";
                                               }                                  
                                               
                                            %>



                                            <img id="device9Menu<% =deviceIndex.ToString() %>" name="<%=dGroups[l].Devices[k].DeviceIpAddress %>" src="<%=srcName %>" style="width: 50px; height: 50px" />
                                        </p>
                                        <p style="text-align: center">
                                            <% =dGroups[l].Devices[k].DeviceName %>
                                        </p>

                                    </div>



                                    <div class="col-md-4">
                                        <p style="text-align: center">

                                            <% string src8Name = "../Images/ic_image_device.png";

                                               if (!CheckDeviceIsOnline(dGroups[l].Devices[k].DeviceIpAddress))
                                               {
                                                   src8Name = "../Images/ic_image_device_offline.png";
                                               }                                  
                                               
                                            %>



                                            <img id="devic999eMenu<% =deviceIndex.ToString() %>" name="<%=dGroups[l].Devices[k].DeviceIpAddress %>" src="<%=srcName %>" style="width: 50px; height: 50px" />
                                        </p>
                                        <p style="text-align: center">
                                            <% =dGroups[l].Devices[k].DeviceName %>
                                        </p>
                                    </div>

                                </div>--%>

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
        aria-labelledby="dropdownMenu" id="deviceListSingleGroupClickMenuBox">
        <li><a class="btn" id="btnSingleGroupChooseChannel" data-backdrop="static" data-dismiss="modal" data-keyboard="false">通通选择</a></li>
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
        aria-labelledby="dropdownMenu" id="deviceListSingleDeviceMenu">
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnOpenDevice" data-backdrop="static" data-dismiss="modal" data-keyboard="false">打开设备</a></li>
        <li><a class="btn" id="deviceListSingleDeviceMenuBtnCloseDevice" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">关闭设备</a></li>

    </ul>


    <!-- save sort order here which can be retrieved on server on postback -->



</div>

