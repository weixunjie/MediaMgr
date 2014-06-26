<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UserMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.UserMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h3 style="clip: rect(auto, auto, 10px, auto)">用户明细管理</h3>


    <asp:Label ID="lbMessage" runat="server" Text="" Visible="false" ForeColor="Red" Font-Size="Larger" Height="30px"></asp:Label>


    <section>

        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>


        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbUserCode" CssClass="col-md-2 control-label" Width="107px">用户编号:</asp:Label>

            <asp:TextBox runat="server" ID="tbUserCode" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbUserCode"
                ForeColor="Red" ErrorMessage=" 编号不能为空" Height="25px" />

            <asp:TextBox runat="server" ID="tbHiddenId" CssClass="form-control" Visible="False" />

        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbName" CssClass="col-md-2 control-label" Width="107px">用户姓名:</asp:Label>

            <asp:TextBox runat="server" ID="tbName" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbName"
                ForeColor="Red" ErrorMessage=" 名称不能为空" Height="25px" />


        </div>



        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbPassword" CssClass="col-md-2 control-label" Width="107px">密码:</asp:Label>

            <asp:TextBox runat="server" ID="tbPassword" TextMode="Password" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbPassword"
                ForeColor="Red" ErrorMessage=" 密码不能为空" Height="25px" />


        </div>




        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbConfimedPass" CssClass="col-md-2 control-label" Width="107px">确认密码:</asp:Label>

            <asp:TextBox runat="server" ID="tbConfimedPass" TextMode="Password" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbConfimedPass"
                ForeColor="Red" ErrorMessage=" 确认密码不能为空" Height="25px" />


        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="ddUserLevel" CssClass="col-md-2 control-label" Width="107px">用户级别:</asp:Label>

            <asp:DropDownList runat="server"  Width="220px" Height="30px" ID="ddUserLevel" CssClass="form-control" >
                <asp:ListItem Value="1">超级用户</asp:ListItem>
                <asp:ListItem Value="2">普通用户</asp:ListItem>
            </asp:DropDownList>



        </div>


        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="chIsAcitve" CssClass="col-md-2 control-label" Width="107px">有效:</asp:Label>


            <asp:CheckBox ID="chIsAcitve" Text="" runat="server" />

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
