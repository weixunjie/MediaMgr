﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChannelList.ascx.cs" Inherits="MediaMgrSystem.ChannelList" %>

<script src="<%=ResolveUrl("Scripts/channelScheduleLogic.js")%>" type="text/javascript"></script>

<style>
    .notactive {
        pointer-events: none;
        cursor: default;
    }
</style>
<script type="text/javascript">


    var currentOperChannelId;
    var currentOperChannelName;
    var currentPlayPIds;

    var boolIsRepeat;

    var currentPlayingFunction;

    var currentFunction;

    var currentSId;
    var timeOutCurrentChannel;
    $(document).ready(function () {

        setButtonStatus("AllDisabled");

        currentFunction = '<% = CheckIfAudio()=="true"? "1":"2" %>';

        <%   List<MediaMgrSystem.DataModels.ChannelInfo> allChanels = GetAllChannels();
             List<MediaMgrSystem.DataModels.ScheduleInfo> schedules = GetAllSchedules();


             List<MediaMgrSystem.DataModels.ProgramInfo> allProgramInfos = new List<MediaMgrSystem.DataModels.ProgramInfo>();

             if (Session["FunctionType"] != null && Session["FunctionType"].ToString() == "A")
             {
                 allProgramInfos = GetAllAuditProgram();
             }
             else
             {
                 allProgramInfos = GetAllVideoProgram();
             }

             string ids = string.Empty;
             for (int i = 0; i < allChanels.Count; i++) { ids = ids + "#channelDiv" + allChanels[i].ChannelId.ToString() + ","; }; ids = ids.TrimEnd(',');

             string schdueleIds = string.Empty;
             for (int i = 0; i < schedules.Count; i++) { schdueleIds = schdueleIds + "#btnSchedule" + schedules[i].ScheduleId.ToString() + ","; }; schdueleIds = schdueleIds.TrimEnd(',');

        %>



        chat.client.sendManualPlayStatus = function (result, error, cid, cname, pids, strPlayingFunction) {

      
            var strNornalChannelBg;


            if (result == "Play") {
                if (currentPlayingFunction == "1") {
                    strNornalChannelBg = '<% =ResolveUrl("~/images/ic_image_channel_playing_bg.png")%>';
                }
                else {
                    strNornalChannelBg = '<% =ResolveUrl("~/images/ic_image_channel_playing_bg_video.png")%>';
                }
            }
            else {
                if (currentPlayingFunction == "1") {
                    strNornalChannelBg = '<% =ResolveUrl("~/images/ic_image_channel_bg.png")%>';
                }
                else {
                    strNornalChannelBg = '<% =ResolveUrl("~/images/ic_image_channel_bg_video.png")%>';
                }
            }

            $("#channelDiv" + cid).css("background-image", "url(" + strNornalChannelBg + ")");


            if (currentOperChannelId == cid) {


                currentPlayingFunction = strPlayingFunction;

                currentOperChannelId = cid;
                currentOperChannelName = cname;

                currentPlayPIds = pids;

                //  debugger;
                if (error != "0") {
                    if (currentPlayingFunction == currentFunction) {
                        $("#divChannelInfo").html(result);
                        isPlaying = false;
                        setButtonStatus("Stop");
                        $("#btnChooseProgram").attr("disabled", false);
                    }

                }
                else {

                    var typeStrPlaying = "视频";
                    if (currentPlayingFunction == "1") {
                        typeStrPlaying = "音频";
                    }

                    if (result == "Play") {
                        if (currentPlayingFunction == currentFunction) {
                            setButtonStatus("Play");
                            $("#btnChooseProgram").attr("disabled", true);
                            $("#divChannelInfo").html(currentOperChannelName + "正在播放中");
                        }
                        else {
                            $("#divChannelInfo").html(currentOperChannelName + "正在播放" + typeStrPlaying);
                        }
                    }
                    else if (result == "Stop") {
                        if (currentPlayingFunction == currentFunction) {
                            setButtonStatus("StopAndDelayStart");

                            $("#btnChooseProgram").attr("disabled", false);
                            $("#divChannelInfo").html(currentOperChannelName + "已停止播放");

                        }
                        else {
                            $("#divChannelInfo").html(currentOperChannelName + "已停止播放" + typeStrPlaying);
                        }


                    }
                }
            }


        }



        function setButtonStatus(type) {


            //All disable
            if (type == "AllDisabled") {
                $("#btnChannelControlPlay").attr("disabled", true);

                $("#btnChannelControlStop").attr("disabled", true);

                $("#btnChannelControlRepeat").attr("disabled", true);
            }

            //Playing disable play, enabld,stop and repeta
            if (type == "Play") {
                $("#btnChannelControlPlay").attr("disabled", true);

                $("#btnChannelControlStop").attr("disabled", false);

                $("#btnChannelControlRepeat").attr("disabled", false);
            }

            //Stop disable stop and repeta, enabled palying
            if (type == "Stop") {


                $("#btnChannelControlPlay").attr("disabled", false);
                $("#btnChannelControlStop").attr("disabled", true);
                $("#btnChannelControlRepeat").attr("disabled", true);



            }

            if (type == "StopAndDelayStart") {


                timeOutCurrentChannel = currentOperChannelId;

                var timeOut = '<% =GetIntervalTimeFromStopToPlay() %>';


                $("#btnChannelControlPlay").attr("disabled", true);

                $("#btnChannelControlStop").attr("disabled", true);

                $("#btnChannelControlRepeat").attr("disabled", true);



                $(function () {
                    setTimeout(function () {

                        if (timeOutCurrentChannel == currentOperChannelId) {
                            setButtonStatus("Stop");
                        }
                    }, timeOut);
                })

            }



            if ($("#btnChannelControlPlay").attr("disabled") == "disabled") {
                $("#btnChannelControlPlay").attr("src", "<%=ResolveUrl("~/Images/ic_image_play_disabled.png") %>");
                $("#btnChannelControlPlay").addClass("notactive");
            }
            else {
                $("#btnChannelControlPlay").attr("src", "<%=ResolveUrl("~/Images/ic_image_play.png") %>");
                $("#btnChannelControlPlay").removeClass("notactive");
            }


            if ($("#btnChannelControlStop").attr("disabled") == "disabled") {
                $("#btnChannelControlStop").attr("src", "<%=ResolveUrl("~/Images/ic_image_stop_disabled.png") %>");
                $("#btnChannelControlStop").addClass("notactive");

            }
            else {

                $("#btnChannelControlStop").removeClass("notactive");
                $("#btnChannelControlStop").attr("src", "<%=ResolveUrl("~/Images/ic_image_stop.png") %>");

            }


            if ($("#btnChannelControlRepeat").attr("disabled") == "disabled") {

                $("#btnChannelControlRepeat").addClass("notactive");
                $("#btnChannelControlRepeat").attr("src", "<%=ResolveUrl("~/Images/ic_image_repeat_disabled.png") %>");

            }
            else {
                $("#btnChannelControlRepeat").removeClass("notactive");
                $("#btnChannelControlRepeat").attr("src", "<%=ResolveUrl("~/Images/ic_image_repeat.png") %>");

            }


        }

        $("#btnChannelControlPlay").mouseout(function (e) {

            if ($(this).attr("disabled") == "disabled") {
                $(this).attr("src", "Images/ic_image_play_disabled.png");

            }
            else {
                $(this).attr("src", "Images/ic_image_play.png");
            }
        });

        $("#btnChannelControlStop").mouseout(function (e) {

            if ($(this).attr("disabled") == "disabled") {
                $(this).attr("src", "Images/ic_image_stop_disabled.png");

            }
            else {
                $(this).attr("src", "Images/ic_image_stop.png");
            }
        });



        $("#btnChannelControlRepeat").mouseout(function (e) {

            if ($(this).attr("disabled") == "disabled") {
                $(this).attr("src", "Images/ic_image_repeat_disabled.png");

            }
            else {
                $(this).attr("src", "Images/ic_image_repeat.png");
            }
        });


        $("#btnChooseProgram").click(function (e) {

   

            if ($("#btnChooseProgram").attr("disabled") == "disabled")
            {
                return;
            }


            $("#lbSelectedProgram option").each(function () {

                $(this).remove();
            })


            $("#lbAvaiableProgram option").each(function () {

                $(this).remove();
            })



              <%  foreach (var pi in allProgramInfos)
                  { %>


            $("#lbAvaiableProgram").append("<option value='<% =pi.ProgramId%>'><%=pi.ProgramName%></option>");
             
                 <%} %>

            $("#channelListChannelClickMenuBox").hide();
            $("#channelListChannelChooseScheduleMenu").hide()
            is_popup_1st_menu = false;
            $('#dialogForChooseProgram').modal('show');
        });



        $.showChannelMenu = function () {


            $("<%= ids %>").click(function (e) {



                $("#channelListChannelClickMenuBox").hide();
                $("#channelListChannelChooseScheduleMenu").hide()


                currentOperChannelId = e.currentTarget.id.replace("channelDiv", "");

                currentOperChannelName = $(this).data("itemid");


                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;


                $.ajax({
                    type: "POST",
                    async: false,
                    url: "BroadcastMain.aspx/GetIsCurrentChannelPlayingAndInfo",
                    data: "{'channelId':'" + currentOperChannelId + "'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {


                        if (msg.d == null || msg.d == "") {
                            $("#btnChooseProgram").attr("disabled", false);
                            setButtonStatus("AllDisabled");

                            $("#divChannelInfo").html(currentOperChannelName + "等待操作");
                        }
                        else {


                            var jsonData = eval('(' + msg.d + ')');
                            currentPlayingFunction = jsonData.PlayingFunction;

                            currentOperChannelId = jsonData.ChannelId;
                            currentOperChannelName = jsonData.ChannelName;


                            boolIsRepeat = jsonData.IsRepeating;


                            currentPlayPIds = jsonData.PlayingPids;



                            if (jsonData.IsPlaying) {
                                if (currentPlayingFunction != currentFunction) {
                                    setButtonStatus("AllDisabled");
                                    $("#btnChooseProgram").attr("disabled", true);

                                    var typeStrPlaying = "视频";
                                    if (currentPlayingFunction == "1") {
                                        typeStrPlaying = "音频";
                                    }
                                    $("#divChannelInfo").html(currentOperChannelName + "正在播放" + typeStrPlaying);
                                }
                                else {
                                    $("#btnChooseProgram").attr("disabled", true);
                                    setButtonStatus("Play");
                                    $("#divChannelInfo").html(currentOperChannelName + "正在播放中");
                                }





                            }
                            else {



                                if (currentPlayingFunction != currentFunction) {
                                    $("#btnChooseProgram").attr("disabled", false);
                                    setButtonStatus("AllDisabled");
                                    $("#divChannelInfo").html(currentOperChannelName + "等待操作");
                                }
                                else {
                                    if (currentPlayPIds != null) {
                                        $("#btnChooseProgram").attr("disabled", false);
                                        setButtonStatus("Stop");
                                        $("#divChannelInfo").html(currentOperChannelName + "准备就绪");

                                    }
                                }
                            }

                        }
                        // debugger;

                        is_popup_1st_menu = true;


                        $("#channelListChannelClickMenuBox").show().css("left", x).css("top", y);

                    }
                });




            });

            $("<%= ids %>").mouseout(function (e) {
                is_popup_1st_menu = false;
            });


        }

        $.showChannelMenu();


        ///-------------------------pgrame 

        $("#btnProgamToRight").click(function () {
            if ($("#lbAvaiableProgram option:selected").length > 0) {
                $("#lbSelectedProgram").append("<option value='" + $("#lbAvaiableProgram option:selected").val() + "'>" + $("#lbAvaiableProgram option:selected").text() + "</option>");
                $("#lbAvaiableProgram option:selected").remove();
            }

        })

        $("#btnProgamAllToRight").click(function () {
            var leftvalue = "";
            $("#lbAvaiableProgram option").each(function () {
                leftvalue += "<option value='" + $(this).val() + "'>" + $(this).text() + "</option>";
                $(this).remove();
            })
            $("#lbSelectedProgram").append(leftvalue);
        })

        $("#btnProgamToLeft").click(function () {
            //  debugger;
            if ($("#lbSelectedProgram option:selected").length > 0) {
                $("#lbAvaiableProgram").append("<option value='" + $("#lbSelectedProgram option:selected").val() + "'>" + $("#lbSelectedProgram option:selected").text() + "</option>");
                $("#lbSelectedProgram option:selected").remove();
            }
        })

        $("#btnProgamAllToLeft").click(function () {
            var rightvalue = "";
            $("#lbSelectedProgram option").each(function () {

                rightvalue += "<option value='" + $(this).val() + "'>" + $(this).text() + "</option>";
                $(this).remove();
            })
            $("#lbAvaiableProgram").append(rightvalue);
        })


        $("#btnConfirmPlayPrograme").click(function () {



            if ($("#lbSelectedProgram option").length <= 0) {
                alert("请选择节目");
            }
            else {
                currentPlayPIds = new Array();

                $("#lbSelectedProgram option").each(function () {

                    currentPlayPIds.push($(this).val());

                })

                $('#dialogForChooseProgram').modal('hide');

                $("#divChannelInfo").html(currentOperChannelName + "准备就绪");

                setButtonStatus("Stop");


            }

        })


        $("#btnChannelControlPlay").click(function () {

            // debugger;
            setButtonStatus("Play");
            if (currentPlayPIds != null && currentPlayPIds.length > 0) {
                chat.server.sendPlayCommand(currentPlayPIds, currentOperChannelId, currentOperChannelName, null, "0", '<% =CheckIfAudio()%>');

                $("#divChannelInfo").html(currentOperChannelName + "正在播放中");
            }

            boolIsRepeat = false;

        })


        $("#btnChannelControlStop").click(function () {

            //string commandType, string channelId, string scheduleGuidId)

            setButtonStatus("StopAndDelayStart");

            //  debugger;

            chat.server.sendStopRoRepeatCommand(currentOperChannelId, currentOperChannelName, true, "", false, '<% =CheckIfAudio()%>');



            $("#divChannelInfo").html(currentOperChannelName + "已停止播放");

            boolIsRepeat = false;


        })


        $("#btnChannelControlRepeat").click(function () {


            chat.server.sendStopRoRepeatCommand(currentOperChannelId, currentOperChannelName, false, "", boolIsRepeat,'<% =CheckIfAudio()%>');
 


            boolIsRepeat = !boolIsRepeat;


            if (boolIsRepeat) {
                $("#divChannelInfo").html(currentOperChannelName + "开启偱环播放");
            }
            else {
                $("#divChannelInfo").html(currentOperChannelName + "关闭偱环播放");
            }



        })


        $("#btnChooseSchedule").click(function (e) {



            is_popup_2nd_menu = true;

            var x = $(this).offset().left + $("#channelListChannelClickMenuBox").width() + 6;
            var y = $(this).offset().top;

            $.ajax({
                type: "POST",
                async: false,
                url: "BroadcastMain.aspx/GetScheduleByChannelId",
                data: "{'cid':'" + currentOperChannelId + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                    $("<% =schdueleIds%>").css("font-weight", "normal");
                    $("#btnSchedule" + msg.d).css("font-weight", "bold");
                    currentSId = msg.d;

                }
            });


            $("#channelListChannelChooseScheduleMenu").show().css("left", x).css("top", y);


        });


        $("<% =schdueleIds%>").click(function (e) {

            var currentOperScheduel = e.currentTarget.id.replace("btnSchedule", "");

            $.ajax({
                type: "POST",
                async: true,
                url: "BroadcastMain.aspx/SaveSchedule",
                data: "{'cid':'" + currentOperChannelId + "',sid:'" + currentOperScheduel + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                }
            });

        
            if (currentSId != currentOperScheduel)
            {
                chat.server.sendChangeChannelSchedule(currentOperChannelId, currentOperChannelName, currentFunction);

            }
            is_popup_1st_menu = false;
            is_popup_2nd_menu = false;

        });


        $("#btnChooseSchedule").mouseout(function (e) {
            is_popup_2nd_menu = false;
        });

    });



</script>



<ul class="dropdown-menu" role="menu"
    aria-labelledby="dropdownMenu" id="channelListChannelClickMenuBox">
    <li><a class="btn" id="btnChooseSchedule" data-backdrop="static" data-dismiss="modal" data-keyboard="false">计划选择</a></li>
    <li><a class="btn" id="btnChooseProgram" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">播放节目</a></li>

</ul>

<ul class="dropdown-menu" role="menu"
    aria-labelledby="dropdownMenu" id="channelListChannelChooseScheduleMenu">

    <% 
        for (int i = 0; i < schedules.Count; i++)
        { 
    %>
    <li><a class="btn" style="margin-bottom: 3px; font-weight: normal" name="<% =schedules[i].ScheduleId %>" id="btnSchedule<% =schedules[i].ScheduleId %>" data-backdrop="static" data-dismiss="modal" data-keyboard="false"><% =schedules[i].ScheduleName %></a></li>
    <%}%>
</ul>


<div style="margin-bottom: 10px; margin-left: 0px; margin-top: 10px; text-align: left;">
    <table class="table table-bordered table-striped;" cellspacing="0" cellpadding="5" border="0" style="width: 300px; margin-top: 0px; margin-bottom: 0px; margin-left: 0px; padding: 0px;">
        <%  
        
            int totalRow = allChanels.Count / 3 + allChanels.Count % 3;

            if (allChanels.Count < 3)
            {
                totalRow = 1;
            }

            for (int i = 0; i < totalRow; i++)
            {
            
        %>


        <tr>



            <% for (int k = 0; k < 3; k++)
               {

                   if ((i * 3 + k) > allChanels.Count - 1)
                   {
            %>

            <td style="text-align: left; padding: 0px; padding-right: 10px; padding-bottom: 5px">
                <div style="width: 100px; margin: 0px 0px 0px 0px; height: 33px; line-height: 33px; vertical-align: central; text-align: center; float: left">
                </div>
            </td>


            <%
                       continue;
                   }

                   string strNornalChannelBg = "";
                   if (CheckChannelIdPlaying(allChanels[i * 3 + k].ChannelId))
                   {
                       if (CheckIfAudio() == "true")
                       {
                           strNornalChannelBg = "~/images/ic_image_channel_playing_bg.png";
                       }
                       else
                       {
                           strNornalChannelBg = "~/images/ic_image_channel_playing_bg_video.png";
                       }

                   }
                   else
                   {
                       if (CheckIfAudio() == "true")
                       {
                           strNornalChannelBg = "~/images/ic_image_channel_bg.png";
                       }
                       else
                       {
                           strNornalChannelBg = "~/images/ic_image_channel_bg_video.png";
                       }
                   }

               
                   
               
            %>

            <td style="text-align: left; padding: 0px; padding-right: 10px; padding-bottom: 5px">




                <div id="channelDiv<%=allChanels[i*3+k].ChannelId %>" data-itemid="<%=allChanels[i*3+k].ChannelName.Trim() %>" style="background-image: url('<% =ResolveUrl(strNornalChannelBg) %>'); width: 100px; margin: 0px 0px 0px 0px; height: 33px; line-height: 33px; vertical-align: central; text-align: center; font-weight:bold; font-size:14px; float: left">

                    <%=  allChanels[i*3+k].ChannelName  %>
                </div>
            </td>
            <% } %>
        </tr>

        <% } %>
    </table>
</div>


<div id="divChannelInfo" style="margin-left: 2px; clear: both; margin-bottom: 10px; margin-top: 10px; text-align: left; font-size: 15pt">
    等待操作
</div>
<ul style="clear: both; margin-left: 0px; text-align: center; list-style-type: none;">

    <li class="channelControlButtonLI">

        <img src="Images/ic_image_play.png" id="btnChannelControlPlay" onmouseover='this.src="Images/ic_image_play_hover.png"' onmousedown='this.src="Images/ic_image_play_hover.png"' class="channelControlButtonImage" />

    </li>
    

    <li class="channelControlButtonLI">

        <img src="Images/ic_image_stop.png" id="btnChannelControlStop" onmousedown='this.src="Images/ic_image_stop_hover.png"' onmouseover='this.src="Images/ic_image_stop_hover.png"' class="channelControlButtonImage" />

    </li>

    <li class="channelControlButtonLI">


        <img src="Images/ic_image_repeat.png" id="btnChannelControlRepeat" onmouseover='this.src="Images/ic_image_repeat_hover.png"' onmousedown='this.src="Images/ic_image_repeat_hover.png"' class="channelControlButtonImage" />

    </li>



</ul>

<div id="dialogForChooseProgram" style="width: auto" class="modal hide">
    <div class="modal-header">
        <a class="close" onclick=" $('#dialogForChooseProgram').modal('hide');" title="关闭">&times;</a><h3>节目选择</h3>
    </div>
    <div class="modal-body">
        <div style="float: left; height: 200px; width: 150px;">

            <h4 style="text-align: left; margin-top: 0px">可选节目</h4>

            <select multiple="multiple" style="height: 190px; width: 150px;" id="lbAvaiableProgram">

                <%--      <%  foreach (var pi in allProgramInfos)
                    { %>

                     <option value="<% =pi.ProgramId %>"><% =pi.ProgramName %>></option>

             
                 <%} %>--%>
            </select>

        </div>
        <div style="float: left; height: 200px; width: 52px; margin-top: 52px; margin-left: 10px; margin-right: 10px">

            <div class="channelProgrameListOperButton">
                <a class="btn primary" id="btnProgamToRight" style="width: 30px">></a>
            </div>

            <div class="channelProgrameListOperButton">
                <a class="btn primary" id="btnProgamAllToRight" style="width: 30px">>></a>
            </div>


            <div class="channelProgrameListOperButton">
                <a class="btn primary" id="btnProgamToLeft" style="width: 30px"><</a>
            </div>

            <div class="channelProgrameListOperButton">
                <a class="btn primary" id="btnProgamAllToLeft" style="width: 30px"><<</a>
            </div>

        </div>



        <div style="float: left; height: 200px; width: 150px;">
            <h4 style="text-align: left; margin-top: 0px">已选节目</h4>
            <select size="4" style="height: 190px; width: 150px;" multiple="multiple" id="lbSelectedProgram">
            </select>
        </div>

    </div>
    <div class="modal-footer">
        <a id="btnConfirmPlayPrograme" class="btn primary">确认</a>
    </div>
</div>
