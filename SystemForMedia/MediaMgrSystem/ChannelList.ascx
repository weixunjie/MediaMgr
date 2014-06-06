<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChannelList.ascx.cs" Inherits="MediaMgrSystem.ChannelList" %>



<script type="text/javascript">

    var currentOperChannel;
    $(document).ready(function () {

        <%   List<MediaMgrSystem.DataModels.ChannelInfo> allChanels = GetAllChanels();
             List<MediaMgrSystem.DataModels.ScheduleInfo> schedules = GetAllSchedules();
             string ids = string.Empty;
             for (int i = 0; i < allChanels.Count; i++) { ids = ids + "#channelDiv" + allChanels[i].ChannelId.ToString() + ","; }; ids = ids.TrimEnd(',');

             string schdueleIds = string.Empty;
             for (int i = 0; i < schedules.Count; i++) { schdueleIds = schdueleIds + "#btnSchedule" + schedules[i].ScheduleId.ToString() + ","; }; schdueleIds = schdueleIds.TrimEnd(',');

        %>


        chat.client.sendAllMessge = function (result) {


        }

        var isChooseSchedule = false;
        $("#btnChooseSchedule").click(function (e) {

            isChooseSchedule = true;


            var x = $(this).offset().left + $("#ChannelMenubox").width() + 6;
            var y = $(this).offset().top;


            $.ajax({
                type: "POST",
                async: false,
                url: "Default.aspx/GetScheduleByChannelId",
                data: "{'cid':'" + currentOperChannel + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                    $("<% =schdueleIds%>").css("font-weight", "normal");
                    $("#btnSchedule" + msg.d).css("font-weight", "bold");

                }
            });



          <%--  <% int a=GetSelectedSchedule(%> e.target.name <% )%>

            alert(<%=i%>);--%>

            $("#SchduleBox").show().css("left", x).css("top", y);


        });


        $("<% =schdueleIds%>").click(function (e) {



            var currentOperScheduel = e.currentTarget.id.replace("btnSchedule", "");

            $.ajax({
                type: "POST",
                async: true,
                url: "Default.aspx/SaveSchedule",
                data: "{'cid':'" + currentOperChannel + "',sid:'" + currentOperScheduel + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (msg) {

                }
            });


            //alert(e.target.name);
            // debugger;

        });


        $("#btnChooseSchedule").mouseout(function (e) {
            isChooseSchedule = false;
        });

        $("#btnChooseProgram").click(function (e) {
            $('#dialogForChooseProgram').modal('show');
        });



        $.showChannelMenu = function () {

            var is_in = false;
            $("<%= ids %>").click(function (e) {


                currentOperChannel = e.currentTarget.id.replace("channelDiv", "");

                is_in = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#ChannelMenubox").show().css("left", x).css("top", y);

                //  currentSelectedDevice = e.currentTarget.name;;


            });

            $("<%= ids %>").mouseout(function (e) {
                is_in = false;
            });

            $(document).click(function () {
                if (!is_in && !isChooseSchedule) {
                    $("#ChannelMenubox").hide();
                    $("#SchduleBox").hide()
                }
            });
        }

        $.showChannelMenu();


    });



</script>





    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="ChannelMenubox">
        <li><a class="btn" id="btnChooseSchedule" data-backdrop="static" data-dismiss="modal" data-keyboard="false">播放视频</a></li>
        <li><a class="btn" id="btnChooseProgram" style="margin-top: 3px" id="GroupMenulistPauseVideo" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">暂停播放</a></li>

    </ul>

    <ul class="dropdown-menu" role="menu"
        aria-labelledby="dropdownMenu" id="SchduleBox">

        <% 
            for (int i = 0; i < schedules.Count; i++)
            { 
        %>
        <li><a class="btn" style="margin-bottom: 3px; font-weight: normal" name="<% =schedules[i].ScheduleId %>" id="btnSchedule<% =schedules[i].ScheduleId %>" data-backdrop="static" data-dismiss="modal" data-keyboard="false">播8放视555555555555555555555555555555频</a></li>
        <%}%>
    </ul>

    <%  int channelIndex = 0;
        int cRows = allChanels.Count / 2 + allChanels.Count % 2;
        for (int i = 0; i < cRows; i++)
        { %>

    <div  style="float: left; height: 160px">
        <div id="channelDiv<%=allChanels[channelIndex].ChannelId %>"style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

            <img src="Images/ic_image_channel.png" width="90" height="99" />
        </div>

        <div style="clear: both;">
        </div>


        <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
            <%=  allChanels[channelIndex].ChannelName  %>
            <% channelIndex++; %>
        </div>

    </div>

    <div " style="float: right; height: 160px">
        <div id="channelDiv<%=allChanels[channelIndex].ChannelId %>" style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

            <img src="Images/ic_image_channel.png" width="90" height="99" />

            
        </div>

        <div style="clear: both;">
        </div>
        <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
            <%=  allChanels[channelIndex].ChannelName  %>
        </div>

    </div>

    <% } %>

    

   <ul style=" clear:left; float:left; wi text-align:center;list-style-type:none;">

       <li  class="channelControlButtonLI" >           

           <img src="Images/ic_image_play.png" onmousedown='this.src="Images/ic_image_play.png"' class="channelControlButtonImage"  />

       </li>


          <li class="channelControlButtonLI" >           

           <img src="Images/ic_image_stop.png" onmousedown='this.src="Images/ic_image_stop.png"' class="channelControlButtonImage" />

       </li>

       <li class="channelControlButtonLI">
           

           <img src="Images/ic_image_repeat.png" onmousedown='this.src="Images/ic_image_repeat.png"' class="channelControlButtonImage" />

       </li>
         

   </ul>


 <div id="dialogForChooseProgram"   style="width:460px"  class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForChooseProgram').modal('hide');" title="关闭">&times;</a><h3>节目选择</h3>
        </div>
        <div class="modal-body">
                    
            <select size="4" name="lbAvaiableProgram" multiple="multiple" id="lb_list" style=" float:left; height: 200px; width: 150px;">


                <option value="1">后台登录</option>

                <option value="2">密码修改</option>

                <option value="3">新闻添加</option>

                <option value="4">新闻编辑</option>

                <option value="5">新闻删除</option>

                <option value="6">新闻发布</option>


            </select>

            <div style=" float:left;height:200px; margin-top:15px;  margin-left:10px; margin-right:10px">
               
                <div  class="channelProgrameListOperButton" >
                 <a class="btn primary"  >确认</a>
               </div>
                                              
                 <div  class="channelProgrameListOperButton" >
                 <a class="btn primary"  >确认</a>
               </div>


                 <div  class="channelProgrameListOperButton" >
                 <a class="btn primary"  >确认</a>
               </div>


                 <div  class="channelProgrameListOperButton" >
                 <a class="btn primary"  >确认</a>
               </div>

                </div>
           

                

        
            <select size="4" multiple="multiple" id="lbSelected" style=" float:left;height: 200px; width: 150px;">
            </select>

             
         
        </div>
        <div class="modal-footer">
            <a class="btn primary">确认</a>
        </div>
    </div>



  


