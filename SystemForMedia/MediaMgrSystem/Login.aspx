<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MediaMgrSystem.Login" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <style>
        * {
            margin: 0;
            padding: 0;
        }

        body {
            font-family: "宋体";
        }

        .loginBox {
            width: 520px;
            height: 270px;
            padding: 0 20px;
            border: 1px solid #fff;
            color: #000;
            margin-top: 40px;
            border-radius: 8px;
            background: white;
            box-shadow: 0 0 15px #222;
            background: -moz-linear-gradient(top, #fff, #efefef 8%);
            background: -webkit-gradient(linear, 0 0, 0 100%, from(#f6f6f6), to(#f4f4f4));
            font: 11px/1.5em 'Microsoft YaHei';
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -210px;
            margin-top: -115px;
        }

            .loginBox h2 {
                height: 45px;
                font-size: 20px;
                font-weight: normal;
            }

            .loginBox .left {
                border-right: 1px solid #ccc;
                height: 100%;
                padding-right: 20px;
            }
    </style>
    <div class="container">
        <section class="loginBox row-fluid">
            <section class="span7 left">
                <h2>登录</h2>



                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="tbUserName" CssClass="col-md-2 control-label" Width="107px">用户名:</asp:Label>

                    <asp:TextBox runat="server" ID="tbUserName" CssClass="form-control" />


                    <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbUserName"
                        ForeColor="Red" ErrorMessage=" *" Height="25px" />




                </div>



                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="tbPassword" CssClass="col-md-2 control-label" Width="107px">密码:</asp:Label>

                    <asp:TextBox runat="server" ID="tbPassword"  TextMode="Password" CssClass="form-control" />


                    <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbPassword"
                        ForeColor="Red" ErrorMessage=" *" Height="25px" />


                </div>


                <section class="row-fluid">
                    <section class="span8 lh30">
                        <asp:Button runat="server" Text="登录" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Login_Click" Width="95px" />
                    </section>
                    <section class="span1">
                    </section>
                </section>
            </section>
            <section class="span5 right">
                <h2>没有帐户？</h2>
                <section>
                    <h2>请联系系统管理员</h2>

                    <p>
                        <asp:Label ID="lbMessage" runat="server" Text="登录失败" Visible="false" ForeColor="Red" Font-Size="Larger" Height="30px"></asp:Label>

                    </p>
                </section>
            </section>
        </section>
        <!-- /loginBox -->
    </div>

</asp:Content>
