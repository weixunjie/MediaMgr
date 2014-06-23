<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EncoderList.ascx.cs" Inherits="MediaMgrSystem.EncoderList" %>

<script src="<%=ResolveUrl("Scripts/channelScheduleLogic.js")%>" type="text/javascript"></script>

<script type="text/javascript">

    var currentOperEncoderId;
    var currentOperEncoderName;

    $(document).ready(function () {



        <%   List<MediaMgrSystem.DataModels.EncoderInfo> allEncoders = GetAllEncoders();


             string ids = string.Empty;
             for (int i = 0; i < allEncoders.Count; i++) { ids = ids + "#encoderDiv" + allEncoders[i].EncoderId.ToString() + ","; }; ids = ids.TrimEnd(',');

        %>


        var is_popup_menu = false;

        $.showEncoderMenu = function () {


            $("<%= ids %>").click(function (e) {


                currentOperEncoderId = e.currentTarget.id.replace("encoderDiv", "");

                currentOperEncoderName = $(this).data("itemid");

                <%%>
                is_popup_menu = true;

                var x = $(this).offset().left;
                var y = $(this).offset().top + $(this).height() + 2;

                $("#EncoderMenubox").show().css("left", x).css("top", y);


            });

            $("<%= ids %>").mouseout(function (e) {
                is_popup_menu = false;
            });


        }

        $.showEncoderMenu()

        $(document).click(function () {
      
            if (!is_popup_menu ) {
                $("#EncoderMenubox").hide();
           
            }
          
        });


    });




</script>



<ul class="dropdown-menu" role="menu"
    aria-labelledby="dropdownMenu" id="EncoderMenubox">
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
        <div id="encoderDiv<%=allEncoders[i].EncoderId %>" data-itemid="<%=allEncoders[i].EncoderName %>" style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

            <img src="Images/ic_image_encoder.png" width="90" height="99" />
        </div>

        <div style="clear: both;">
        </div>

        <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
            <%=  allEncoders[i].EncoderName  %>
        </div>

    </div>
    <% } %>

    <%--   <% if (channelIndex < allChanels.Count)
       { %>--%>
    <%-- <div " style="float: right; height: 160px">
        <div id="encoderDiv<%=allEncoders[encoderIndex].EncoderId %>" data-itemid="<%=allEncoders[encoderIndex].ChannelName %>"   style="width: 99px; margin: 0px 0px 0px 0px; height: 99px; line-height: 99px; vertical-align: central; text-align: center; float: left">

            <img src="Images/ic_image_channel.png" width="90" height="99" />
            
        </div>

        <div style="clear: both;">
        </div>
        <div style="height: 10px; margin-top: 10px; text-align: center; font-size: 15pt">
            <%=  allEncoders[encoderIndex].EncoderName  %>
             <% encoderIndex++; %>
        </div>

    </div>--%>

    <%--    <% }
        } %>--%>
