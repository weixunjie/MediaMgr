$(document).ready(function () {

  
 
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

    });


    $("#btnChooseSchedule").mouseout(function (e) {
        isChooseSchedule = false;
    });

});