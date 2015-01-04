<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="VideoEncodeList.ascx.cs" Inherits="MediaMgrSystem.VideoEncodeList" %>

<script src="<%=ResolveUrl("Scripts/channelScheduleLogic.js")%>" type="text/javascript"></script>

<script type="text/javascript">

    var currentOperEncoderId;
    var currentOperEncoderName;
    var currentOperEncoderClientIdentify;
    $(document).ready(function () {


        <%   List<MediaMgrSystem.DataModels.VideoEncoderInfo> allEncoders = GetAllEncoders();

             bool isAudio = MediaMgrSystem.GlobalUtils.GetCurrentFunctionType() == MediaMgrSystem.DataModels.BusinessType.AUDITBROADCAST;




             string ids = string.Empty;
             for (int i = 0; i < allEncoders.Count; i++) { ids = ids + "#encoderDiv" + allEncoders[i].EncoderId.ToString() + ","; }; ids = ids.TrimEnd(',');

        %>




        $("#btnOpenEncoder").click(function (e) {


            chat.server.sendVideoEncoderOperation(currentOperEncoderId, "1", '<% =isAudio?"1":"2" %>');

        });

        $("#btnCloseEncoder").click(function (e) {

            chat.server.sendVideoEncoderOperation(currentOperEncoderId, "0", '<% =isAudio?"1":"2" %>');

        });



        $.showEncoderMenu = function () {


            $("<%= ids %>").click(function (e) {


                currentOperEncoderId = e.currentTarget.id.replace("encoderDiv", "");


                is_popup_1st_menu = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;



                $("#encoderListEncoderClickMenuBox").show().css("left", x).css("top", y);





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

                    <div id="encoderDiv<%=allEncoders[j * 3 + k].EncoderId %>" style="height: 60px; line-height: 60px">

                              <% 

                                                
                                      string srcName = GetImageUrl(allEncoders[j * 3 + k].EncoderId);

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

<%--<div style="float: left; height: 160px">
    <% 
        }
        else
        { 
    %>
    <div style="float: right; height: 160px">
        <% }%>
        <div id="encoderDiv<%=allEncoders[i].EncoderId %>" style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

            <img src="Images/ic_image_video_encoder.png" width="50" height="50" />
        </div>

        <div style="clear: both;">
        </div>

        <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
            <%=  allEncoders[i].EncoderName  %>
        </div>--%>

   
    
