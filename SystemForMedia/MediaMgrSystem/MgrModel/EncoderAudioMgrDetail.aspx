﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="EncoderAudioMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.EncoderAudioMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h3 style="clip: rect(auto, auto, 10px, auto)">呼叫台明细管理</h3>


    <section id="groupMgrSection">

        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>
        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="TbName" CssClass="col-md-2 control-label" Width="107px">名称:</asp:Label>

            <asp:TextBox runat="server" ID="TbName" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TbName"
                ForeColor="Red" ErrorMessage=" 名称不能为空" Height="25px" />

            <asp:TextBox runat="server" ID="TbHiddenId" CssClass="form-control" Visible="False" />

        </div>

         <div class="form-group" style="margin-bottom: 10px">


            <asp:Label runat="server" AssociatedControlID="tbIpAddress" CssClass="col-md-2 control-label" Width="107px">IP地址:</asp:Label>

         
            <asp:TextBox runat="server" ID="tbIpAddress" CssClass="form-control"  ReadOnly="true" />




        </div>


        <div class="form-group" style="margin-bottom: 10px">


            <asp:Label runat="server" AssociatedControlID="ddPriority" CssClass="col-md-2 control-label" Width="107px">优先级:</asp:Label>

            <asp:DropDownList runat="server" Width="220px" ID="ddPriority" CssClass="form-control" />



        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbBoundRate" CssClass="col-md-2 control-label" Width="107px">码率:</asp:Label>

            <asp:TextBox runat="server" ID="tbBoundRate" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TbName"
                ForeColor="Red" ErrorMessage=" 码率不能为空" Height="25px" />


            <div style="clear: both" class="clear"></div>
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
