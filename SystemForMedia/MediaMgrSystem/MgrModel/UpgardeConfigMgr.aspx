﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UpgardeConfigMgr.aspx.cs" Inherits="MediaMgrSystem.MgrModel.UpgardeConfigMgr" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h3 style="clip: rect(auto, auto, 10px, auto)">更新配置</h3>


            <section id="groupMgrSection">

                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>

                <div class="form-group" style="margin-bottom: 10px">

                    <h4>音/视频终端</h4>
                </div>
                <div class="form-group" style="margin-bottom: 10px;width:300px">

                    <asp:Label runat="server" AssociatedControlID="TbVCVersionId" CssClass="col-md-2 control-label" Width="107px">版本号:</asp:Label>

                    <asp:TextBox runat="server" ID="TbVCVersionId" CssClass="form-control"   Style="width:300px"/>

          

                </div>


                <div class="form-group" style="margin-bottom: 10px; width:300px">

                    <asp:Label runat="server" AssociatedControlID="TbVCUpgardeUrl" CssClass="col-md-2 control-label" Width="107px">更新路径:</asp:Label>

                    <asp:TextBox runat="server" ID="TbVCUpgardeUrl"  Style="width:300px" CssClass="form-control" />


                  

                </div>




                <div class="form-group" style="margin-bottom: 10px; width:300px">

                    <h4>物联终端</h4>
                </div>
                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="tbRmsVersion" CssClass="col-md-2 control-label" Width="107px">版本号:</asp:Label>

                    <asp:TextBox runat="server" ID="tbRmsVersion" CssClass="form-control"  Style="width:300px" />


              

                </div>


                <div class="form-group" style="margin-bottom: 10px; width:300px">

                    <asp:Label runat="server" AssociatedControlID="TbRmsUpgardeUrl" CssClass="col-md-2 control-label" Width="107px">APK更新路径:</asp:Label>

                    <asp:TextBox runat="server" ID="TbRmsUpgardeUrl" CssClass="form-control"  Style="width:300px" />

               

                </div>






                <div class="form-group">

                    <asp:Button runat="server" Text="确认保存" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Add_Click" />


                </div>



                <div class="form-group">
                    <div class="col-md-offset-2 col-md-10">
                    </div>
                </div>
            </section>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>
