<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AudioEncoderList.ascx.cs" Inherits="MediaMgrSystem.AudioEncoderList" %>

<script src="<%=ResolveUrl("Scripts/channelScheduleLogic.js")%>" type="text/javascript"></script>

<script type="text/javascript">

    var currentOperEncoderId;
    var currentOperEncoderName;
   var  currentOperEncoderClientIdentify;
    $(document).ready(function () {


        <%   List<MediaMgrSystem.DataModels.EncoderAudioInfo> allEncoders = GetAllEncoders();

             List<MediaMgrSystem.DataModels.EncoderSyncGroupInfo> allGroups = GetAllGroups();

             string ids = string.Empty;
             for (int i = 0; i < allEncoders.Count; i++) { ids = ids + "#encoderDiv" + allEncoders[i].EncoderId.ToString() + ","; }; ids = ids.TrimEnd(',');

        %>

        



        $("#btnConfirmGroupSel").click(function (e) {


            if ($("#lbAvaiableGroups option:selected").length <= 0) {
                alert("请选择分组");
                return;
            }


            var groupIds = "";
            $("#lbAvaiableGroups option:selected").each(function () {
                groupIds = groupIds + $(this).val() + ",";
            })


            chat.server.sendAudioEncoderOpenCommand(currentOperEncoderClientIdentify,groupIds)
            $('#dialogForChooseProgram').modal('hide');

        });

        $("#btnOpenEncoder").click(function (e) {

            $('#dialogForChooseGroupId').modal('show');

        });
       
        $("#btnCloseEncoder").click(function (e) {

            chat.server.sendAudioEncoderCloseCommand(currentOperEncoderClientIdentify)
  
        });

      

        $.showEncoderMenu = function () {


            $("<%= ids %>").click(function (e) {


                currentOperEncoderId = e.currentTarget.id.replace("encoderDiv", "");

                currentOperEncoderName = $(this).data("itemid");


                currentOperEncoderClientIdentify = $(this).data("itemcid");

        
                is_popup_1st_menu = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;


                $.ajax({
                    type: "POST",
                    async: false,
                    url: "BroadcastMain.aspx/CheckAudiotEncoderIsRunning",
                    data: "{'cid':'" + currentOperEncoderClientIdentify + "'}",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (msg) {

                    
                        if (msg != null && msg.d != null)
                        {
                            if (msg.d == "0") {
                                $("#btnOpenEncoder").attr("disabled", true);
                                $("#btnCloseEncoder").attr("disabled", false);
                            }
                            else {

                                $("#btnOpenEncoder").attr("disabled", false);
                                $("#btnCloseEncoder").attr("disabled", true);
                            }
                        }

                        $("#encoderListEncoderClickMenuBox").show().css("left", x).css("top", y);

                    }
                });


                               
                


            });

            $("<%= ids %>").mouseout(function (e) {
                is_popup_1st_menu = false;
            });


        }

        $.showEncoderMenu()

    });




</script>



<ul class="dropdown-menu" role="menu"
    aria-labelledby="dropdownMenu" id="encoderListEncoderClickMenuBox">
    <li><a class="btn" id="btnOpenEncoder" data-backdrop="static" data-dismiss="modal" data-keyboard="false">打开</a></li>
    <li><a class="btn" id="btnCloseEncoder" style="margin-top: 3px" data-controls-modal="my_modal" data-backdrop="true" data-keyboard="false">关闭</a></li>

</ul>


<% 
    for (int i = 0; i < allEncoders.Count; i++)
    {

        if (i % 2 == 0)
        {
%>


<div style="float: left; height: 160px">
    <% 
        }
        else
        { 
    %>
    <div style="float: right; height: 160px">
        <% }%>
        <div id="encoderDiv<%=allEncoders[i].EncoderId %>" data-itemcid="<%=allEncoders[i].ClientIdentify %>" data-itemid="<%=allEncoders[i].EncoderName %>" style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

            <img src="Images/ic_image_encoder.png" width="90" height="99" />
        </div>

        <div style="clear: both;">
        </div>

        <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
            <%=  allEncoders[i].EncoderName  %>
        </div>

    </div>
    <% } %>


    <div id="dialogForChooseGroupId" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForChooseProgram').modal('hide');" title="关闭">&times;</a><h3>分组选择</h3>
        </div>
        <div class="modal-body">
            <div style="float: left; height: 190px; width: 250px;">

                <select multiple="multiple" style="height: 190px; width: 250px;" id="lbAvaiableGroups">

                    <%  foreach (var g in allGroups)
                        { %>

                    <option value="<% =g.GroupId %>" ><% =g.GroupName %></option>


                    <%} %>
                </select>

            </div>


        </div>
        <div class="modal-footer">
            <a id="btnConfirmGroupSel" class="btn primary">确认</a>
        </div>
    </div>
