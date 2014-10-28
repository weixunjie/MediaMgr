<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewMP3.aspx.cs" Inherits="MediaMgrSystem.MgrModel.PreviewMP3" %>




<script type="text/javascript" src='<%=ResolveUrl("~/Scripts/jquery-1.10.2.js") %>'></script>

<script type="text/javascript" src='<%=ResolveUrl("~/Scripts/jquery.jmp3.js") %>'></script>



<style type="text/css">

</style>


<script type="text/javascript">
    $(document).ready(function () {
        // default options
        $(".mp3").jmp3({
            backcolor: "ffd700",
            forecolor: "8B4513",
            width: 100,
            showdownload: "<% =Request["FileUrl"].ToString() %>"
        });

    });
</script>





<span class="mp3" ></span>






