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
            $('#dialogForChooseGroupId').modal('hide');

        });

        $("#btnCallAll").click(function (e) {
                     

            var groupIds = "";
            $("#lbAvaiableGroups option").each(function () {
                groupIds = groupIds + $(this).val() + ",";
            })


            chat.server.sendAudioEncoderOpenCommand(currentOperEncoderClientIdentify, groupIds)
            $('#dialogForChooseGroupId').modal('hide');

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
                                $("#btnOpenEncoder").attr("disabled", false);
                                $("#btnCloseEncoder").attr("disabled", false);
                            }
                            else {

                                $("#btnOpenEncoder").attr("disabled", false);
                                $("#btnCloseEncoder").attr("disabled", false);
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


<div style="margin-bottom: 10px; margin-left: 0px; margin-top: 10px; text-align: left;">
    <table class="table table-bordered table-striped;" cellspacing="0" cellpadding="5" border="0" style="width: 300px; margin-top: 0px; margin-bottom: 0px; margin-left: 0px; padding: 0px;">
        <%  
        
            int totalRow = allEncoders.Count / 3 + allEncoders.Count % 3;

            if (allEncoders.Count < 3)
            {
                totalRow = 1;
            }

            for (int j = 0; j < totalRow; j++)
            {
            
        %>


        <tr>



            <% for (int k = 0; k < 3; k++)
               {

                   if ((j * 3 + k) > allEncoders.Count - 1)
                   {
            %>

            <td style="text-align: left; padding: 0px; padding-right: 10px; padding-bottom: 5px">
                <div style="margin: 0px 0px 0px 0px; height: 105px; line-height: 105px; vertical-align: central; text-align: center;">
                </div>
            </td>


            <%
                       continue;
                   }                 

                                  
               
            %>

            <td style="text-align: left; width: 100px;padding: 0px; padding-right: 10px; padding-bottom: 5px">

                <div style="width: 100px; margin: 0px 0px 0px 0px; height: 105px; line-height: 105px; vertical-align: central; text-align: center;">

                    <div id="encoderDiv<%=allEncoders[j * 3 + k].EncoderId %>" data-itemcid="<%=allEncoders[j * 3 + k].ClientIdentify %>" data-itemid="<%=allEncoders[j * 3 + k].EncoderName %>"  style="height: 60px; line-height: 60px">


                          <%                                                 
                                      string srcName = GetImageUrl(allEncoders[j * 3 + k].ClientIdentify);

                                      srcName = ResolveUrl("~/Images/" + srcName);
                                            %>


                        <img src="<% =srcName %>" style="width: 60px; height: 100%" />

                      
                    </div>


                    <div style="text-align: center; line-height: 30px">
                        <%=  allEncoders[j * 3 + k].EncoderName  %>
                    </div>

                </div>
            </td>
            <% } %>
        </tr>

        <% } %>
    </table>
</div>


    <div id="dialogForChooseGroupId" style="width: auto" class="modal hide">
        <div class="modal-header">
            <a class="close" onclick=" $('#dialogForChooseGroupId').modal('hide');" title="关闭">&times;</a><h3>分组选择</h3>
        </div>
        <div class="modal-body">
            <div style="float: left; height: 190px; width: 250px;">

                <select multiple="multiple" style="height: 190px; width: 250px;" id="lbAvaiableGroups">

                    <%  foreach (var g in allGroups)
                        { %>

                    <option value="<% =g.groupId %>" ><% =g.groupName %></option>


                    <%} %>
                </select>

            </div>


        </div>
        <div class="modal-footer">
            <a id="btnConfirmGroupSel" class="btn primary">确认</a>
             <a id="btnCallAll" class="btn primary">全体呼叫</a>
        </div>
    </div>
