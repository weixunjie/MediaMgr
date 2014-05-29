function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
}
function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
};





//function showTime() {
//    return Math.floor((1 + Math.random()) * 0x10000)
//            .toString(16)
//            .substring(1);
//};

//$('.modal').on('show', function () {
//    $(this).css({

//        'margin-top': function () {

//            return ($(this).height() / 2);
//        }
//    });
//});

//$("#btnTest").click(function (e) {


//    $.each($("#lb_list option:selected"), function (i, own) {
//        //                    var sText = $(own).text();
//        //                    var sValue = $(own).val();
//        //                    $("#" + listto).append("<option value=\"" + sValue + "\">" + sText + "</option>"); 

//        $(own).appendTo($("#lb_list2"));
//        // $(own).remove();

//        $("#lb_list").children("option:first").attr("selected", true);

//    });
//});


//-------------------
//-----------------------------
// $("#dia8log").dialog(
//{
//    autoOpen: false,
//    modal: true,
//    closeOnEscape: false,
//    position: "center",
//});



//$("#menu_ul li a").click(function (e) {


//    $("#wei").modal('show')

//});




function processControlTimeOut(guidId) {

    $.ajax({
        type: "POST",
        url: "Default.aspx/CheckTimeOutForOperation",
        data: "{'GuidId':'" + guidId + "'}",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {


            var arr = eval(msg.d);

            var exstingIndex = -1;
            for (var i = 0; i < opGuidIds.length; i++) {
                if (opGuidIds[i] == arr[0].StrValue) {
                    exstingIndex = i
                }
            }
            if (exstingIndex >= 0) {

                $("#divLogs").append(opDevices[exstingIndex] + "操作失败");

                opDevices.splice(exstingIndex, 1);
                opGuidIds.splice(exstingIndex, 1);

            }
        }
    })



}