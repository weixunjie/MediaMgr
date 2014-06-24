<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LogList.ascx.cs" Inherits="MediaMgrSystem.LogList" %>

<script src="<%=ResolveUrl("Scripts/channelScheduleLogic.js")%>" type="text/javascript"></script>

<script type="text/javascript">
    

    $(document).ready(function () {


        <%   List<MediaMgrSystem.DataModels.LogInfo> top3Logs = GetTop3Logs();


          
        %>
    });
</script>


<div id="divLogs" style="width: 300px; height: 200px" class="pull-left">

    <%
        foreach (var logItem in top3Logs)
        {%>

    <% =logItem.LogName+"  "+logItem.LogDesp+" "+logItem.LogDate%>

    <%  }  %>
</div>


<div style="clear: both;">
</div>



