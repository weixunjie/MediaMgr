<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RemoveControlLogList.ascx.cs" Inherits="MediaMgrSystem.RemoveControlLogList" %>



<script type="text/javascript">


    $(document).ready(function () {


        <%   List<MediaMgrSystem.DataModels.LogInfo> top3Logs = GetTop3RemoveControlLogsLogs();


          
        %>
    });
</script>


<div id="divLogs" style="width: 700px;" class="pull-left">

    <%
        foreach (var logItem in top3Logs)
        {%>

    <% =logItem.LogName+"->"+logItem.LogDesp%>
    <br />

    <%  }  %>
</div>


<div style="clear: both;">
</div>



