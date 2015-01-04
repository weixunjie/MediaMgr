<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DeviceMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.DeviceMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">


    <style>
        .cb td {
            width: 230px;
            text-align: center;
        }

        .cb label {
            float: left;
            display: inline-block;
        }

        .cb input {
            float: left;
        }
    </style>



    <h3 style="clip: rect(auto, auto, 10px, auto)">设备明细管理</h3>

    <asp:Label ID="lbMessage" runat="server" Text="" Visible="false" ForeColor="Red" Font-Size="Larger" Height="30px"></asp:Label>


    <section id="groupMgrSection">

        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>
        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="TbName" CssClass="col-md-2 control-label" Width="107px">设备名称:</asp:Label>

            <asp:TextBox runat="server" ID="TbName" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TbName"
                ForeColor="Red" ErrorMessage=" 名称不能为空" Height="25px" />

            <asp:TextBox runat="server" ID="TbHiddenId" CssClass="form-control" Visible="False" />
              <asp:TextBox runat="server" ID="tbHiddenOldIpAddress" CssClass="form-control" Visible="False" />
        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="TBIPAddress" CssClass="col-md-2 control-label" Width="107px">IP地址:</asp:Label>


            <asp:TextBox runat="server" ID="TBIPAddress" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TBIPAddress"
                ForeColor="Red" ErrorMessage=" IP地址不能为空" Height="25px" />

        </div>

              <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbServerIpAddress" CssClass="col-md-2 control-label" Width="107px">服务器地址:</asp:Label>

            <asp:TextBox runat="server" ID="tbServerIpAddress" CssClass="form-control" />
          

        </div>

                  <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbMacAddress" CssClass="col-md-2 control-label" Width="107px">Mac地址:</asp:Label>

            <asp:TextBox runat="server" ID="tbMacAddress" CssClass="form-control" ReadOnly="true" />
          

        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="cbFunction" CssClass="col-md-2 control-label" Width="107px">功能:</asp:Label>


            <asp:CheckBoxList ID="cbFunction" runat="server" Height="40px" RepeatColumns="8" RepeatDirection="Horizontal" Width="566px" CssClass="cb">
                <asp:ListItem Value="1">音频广播</asp:ListItem>
                <asp:ListItem Value="2">视频直播</asp:ListItem>
                <asp:ListItem Value="3">物联管理</asp:ListItem>                

            </asp:CheckBoxList>


        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="ddGroups" CssClass="col-md-2 control-label" Width="107px">所在组:</asp:Label>

            <asp:DropDownList runat="server" Width="220px" ID="ddGroups" CssClass="form-control" />

        </div>
        

        <div class="form-group">

            <asp:Button runat="server" Text="确认保存" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Add_Click" />
            <asp:Button runat="server" ValidateRequestMode="Disabled" Text="返回" CssClass="btn btn-default" OnClick="Back_Click" />
        </div>
        
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
            </div>
        </div>
    </section>

</asp:Content>
