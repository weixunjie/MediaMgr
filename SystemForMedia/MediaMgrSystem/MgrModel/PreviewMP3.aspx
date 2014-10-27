<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewMP3.aspx.cs" Inherits="MediaMgrSystem.MgrModel.PreviewMP3" %>

<style type="text/css">

</style>





<div >


        <object classid="clsid27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0" width="580" height="110">
<param name="_cx" value="10372">
<param name="_cy" value="2355">
<param name="FlashVars" value="-1">
<param name="Movie" value="http://localhost/MediaMgrDemo/FileSource/singlemp3player.swf?showDownload=true&file=<% =Request["FileUrl"].ToString() %>&autoStart=true">

<embed wmode="transparent" src="http://localhost/MediaMgrDemo/FileSource/singlemp3player.swf?showDownload=true&file=<% =Request["FileUrl"].ToString() %>&autoStart=true" width="580" height="110" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash" ></embed>
</object>



   <%-- <object classid="CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95" codebase="http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701"
        type="application/x-oleobject" standby="Loading Microsoft Windows Media Player components..." width="200" height="100">
        <param name="AudioStream" value="-1">
        <param name="AutoSize" value="0">
        <param name="AutoStart" value="1">
        <param name="AnimationAtStart" value="0">
        <param name="AllowScan" value="-1">
        <param name="AllowChangeDisplaySize" value="-1">
        <param name="AutoRewind" value="0">
        <param name="Balance" value="0">
        <param name="BaseURL" value="">
        <param name="BufferingTime" value="5">
        <param name="CaptioningID" value="">
        <param name="ClickToPlay" value="-1">
        <param name="CursorType" value="0">
        <param name="CurrentPosition" value="-1">
        <param name="CurrentMarker" value="0">
        <param name="DefaultFrame" value="">
        <param name="DisplayBackColor" value="0">
        <param name="DisplayForeColor" value="16777215">
        <param name="DisplayMode" value="0">
        <param name="DisplaySize" value="4">
        <param name="Enabled" value="-1">
        <param name="EnableContextMenu" value="-1">
        <param name="EnablePositionControls" value="-1">
        <param name="EnableFullScreenControls" value="0">
        <param name="EnableTracker" value="-1">
        <param name="Filename" value="<% =Request["FileUrl"].ToString() %>">
        <param name="InvokeURLs" value="-1">
        <param name="Language" value="-1">
        <param name="Mute" value="0">
        <param name="PlayCount" value="1">
        <param name="PreviewMode" value="0">
        <param name="Rate" value="1">
        <param name="SAMILang" value="">
        <param name="SAMIStyle" value="">
        <param name="SAMIFileName" value="">
        <param name="SelectionStart" value="-1">
        <param name="SelectionEnd" value="-1">
        <param name="SendOpenStateChangeEvents" value="-1">
        <param name="SendWarningEvents" value="-1">
        <param name="SendErrorEvents" value="-1">
        <param name="SendKeyboardEvents" value="0">
        <param name="SendMouseClickEvents" value="0">
        <param name="SendMouseMoveEvents" value="0">
        <param name="SendPlayStateChangeEvents" value="-1">
        <param name="ShowCaptioning" value="0">
        <param name="ShowControls" value="-1">
        <param name="ShowAudioControls" value="-1">
        <param name="ShowDisplay" value="0">
        <param name="ShowGotoBar" value="0">
        <param name="ShowPositionControls" value="">
        <param name="ShowStatusBar" value="-1">
        <param name="ShowTracker" value="0">
        <param name="TransparentAtStart" value="0">
        <param name="VideoBorderWidth" value="0">
        <param name="VideoBorderColor" value="0">
        <param name="VideoBorder3D" value="0">
        <param name="Volume" value="-600">
        <param name="WindowlessVideo" value="0">
        <embed type="application/x-mplayer2" 
            pluginspage="http://www.microsoft.com/Windows/Downloads/Contents/Products/MediaPlayer/"
             name="MediaPlayer" src="<% =Request["FileUrl"].ToString() %>"
             height="100" width="200"   showaudiocontrols="1" showtracker="0" showdisplay="1" showstatusbar="0" showgotobar="0" showcaptioning="0" autostart="1" autorewind="0" animationatstart="0" transparentatstart="0" allowchangedisplaysize="0" allowscan="0" enablecontextmenu="1" clicktoplay="1"></embed>
    </object>--%>
    
    <%--<embed type="video/x-ms-wmv" align="middle" autostart="0" height="45" width="600" loop="false" src="<% =Request["FileUrl"].ToString() %>"/><br/>--%>
</div>
<%-- <object id="MediaPlayer" height="64" width="260" classid="CLSID:6BF52A52-394A-11d3-B153-00C04F79FAA6">
        <param name="AutoStart" value="1">
        <!--是否自动播放-->
        <param name="Balance" value="0">
        <!--调整左右声道平衡,同上面旧播放器代码-->
        <param name="enabled" value="-1">
        <!--播放器是否可人为控制-->
        <param name="EnableContextMenu" value="-1">
        <!--是否启用上下文菜单-->
        <param name="url" value="<% =Request["FileUrl"].ToString() %>">
        <!--播放的文件地址-->
        <param name="PlayCount" value="1">
        <!--播放次数控制,为整数-->
        <param name="rate" value="1">
        <!--播放速率控制,1为正常,允许小数,1.0-2.0-->
        <param name="currentPosition" value="0">
        <!--控件设置:当前位置-->
        <param name="currentMarker" value="0">
        <!--控件设置:当前标记-->
        <param name="defaultFrame" value="">
        <!--显示默认框架-->
        <param name="invokeURLs" value="0">
        <!--脚本命令设置:是否调用URL-->
        <param name="baseURL" value="">
        <!--脚本命令设置:被调用的URL-->
        <param name="stretchToFit" value="0">
        <!--是否按比例伸展-->
        <param name="volume" value="50">
        <!--默认声音大小0%-100%,50则为50%-->
        <param name="mute" value="0">
        <!--是否静音-->
        <param name="uiMode" value="mini">
        <!--播放器显示模式:Full显示全部;mini最简化;None不显示播放控制,只显示视频窗口;invisible全部不显示-->
        <param name="windowlessVideo" value="0">
        <!--如果是0可以允许全屏,否则只能在窗口中查看-->
        <param name="fullScreen" value="0">
        <!--开始播放是否自动全屏-->
        <param name="enableErrorDialogs" value="-1">
        <!--是否启用错误提示报告-->
        <param name="SAMIStyle" value="">
        <!--SAMI样式-->
        <param name="SAMILang" value="">
        <!--SAMI语言-->
        <param name="SAMIFilename" value="">
        <!--字幕ID-->
    </object>--%>

