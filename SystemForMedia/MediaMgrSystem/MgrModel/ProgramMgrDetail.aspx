﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ProgramMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ProgramMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <form runat="server">
        <h3 style="clip: rect(auto, auto, 10px, auto)">节目明细管理</h3>


        <section id="groupMgrSection">

            <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                <p class="text-danger">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
            </asp:PlaceHolder>
            <div class="form-group" style="margin-bottom: 10px">

                <asp:Label runat="server" AssociatedControlID="TbProgrmeName" CssClass="col-md-2 control-label" Width="107px">节目名称:</asp:Label>

                <asp:TextBox runat="server" ID="TbProgrmeName" CssClass="form-control" />

                <asp:RequiredFieldValidator  style="vertical-align:middle"  ValidationGroup="inputValidate" runat="server" ControlToValidate="TbProgrmeName"
                   ForeColor="Red" ErrorMessage=" 节目名称不能为空" Height="25px" />

                <asp:TextBox runat="server" ID="TbHiddenId" CssClass="form-control" Visible="False" />

            </div>

            <div class="form-group" style="margin-bottom: 10px">

                <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">文件列表:</asp:Label>
            </div>


            <div class="form-group" style="margin-bottom: 10px">


                <div style="float: left; width: 200px">
                    <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">可选</asp:Label>
                    <asp:ListBox ID="lbAvaibleFiles" utoPostBack="true" SelectionMode="Multiple" runat="server" Height="226px" Width="187px"></asp:ListBox>
                </div>
                <div style="float: left; width: 50px; margin-top: 40px">
                    <asp:Button ID="btnToRight"   Width="40px" Height="30px" style="margin-bottom:5px" Text=">" runat="server" OnClick="btnToRight_Click"></asp:Button>
                    <asp:Button ID="btnAllToRight" Height="30px" Width="40px" style="margin-bottom:5px" Text=">>" runat="server" OnClick="btnAllToRight_Click"></asp:Button>
                    <asp:Button ID="btnToLeft" Width="40px" Height="30px" style="margin-bottom:5px" Text="<" runat="server" OnClick="btnToLeft_Click"></asp:Button>
                    <asp:Button ID="btnAllToLeft" Width="40px" Height="30px" style="margin-bottom:5px"  Text="<<" runat="server" OnClick="btnAllToLeft_Click"></asp:Button>
                </div>
                <div style="float: left; width: 240px">
                    <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">已选</asp:Label>
                    <asp:ListBox ID="lbSelectedFiles" AutoPostBack="true" SelectionMode="Multiple" runat="server" Height="222px" Width="186px"></asp:ListBox>
                </div>
                <div style="clear: both" class="clear"></div>
            </div>







            <div class="form-group">

                <asp:Button runat="server" Text="确认保存" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Unnamed6_Click" />
                <asp:Button runat="server" ValidateRequestMode="Disabled" Text="返回" CssClass="btn btn-default" OnClick="Unnamed7_Click" />

            </div>



            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                </div>
            </div>
        </section>


    </form>
</asp:Content>
