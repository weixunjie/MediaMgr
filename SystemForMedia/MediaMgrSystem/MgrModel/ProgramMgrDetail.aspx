<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ProgramMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ProgramMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">


    <script type="text/javascript">
        window.onload = function () {
            var id = "#" + '<% =this.LoadData.ClientID %>';
            $(id).click();
        }
    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>




    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <h3 style="clip: rect(auto, auto, 10px, auto)">节目明细管理</h3>

            <div runat="server" id="ss">
            <img src="<%=ResolveUrl("~/Images/ic_image_loading.gif") %>" style="width: 25px; height: 25px; margin-right: 5px" />
            正在加载节目文件数据...
    </div>



                <span style="color: #FF6666;">
                    <asp:Button ID="LoadData" runat="server" Text="Button" OnClick="LoadData_Click" Style="display: none" /></span>





            <section id="groupMgrSection" runat="server">

                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>
                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="TbProgrmeName" CssClass="col-md-2 control-label" Width="107px">节目名称:</asp:Label>

                    <asp:TextBox runat="server" ID="TbProgrmeName" CssClass="form-control" />

                    <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TbProgrmeName"
                        ForeColor="Red" ErrorMessage=" 节目名称不能为空" Height="25px" />

                    <asp:TextBox runat="server" ID="TbHiddenId" CssClass="form-control" Visible="False" />

                </div>

                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">文件列表:</asp:Label>
                </div>


                <div class="form-group" style="margin-bottom: 10px">


                    <div style="float: left; width: 200px">
                        <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">可选</asp:Label>
                        <asp:ListBox ID="lbAvaibleFiles" SelectionMode="Multiple" runat="server" Height="226px" Width="187px"></asp:ListBox>
                    </div>
                    <div style="float: left; width: 50px; margin-top: 40px">
                        <asp:Button ID="btnToRight" CssClass="btn primary" Width="40px" Height="30px" Style="margin-bottom: 5px" Text=">" runat="server" OnClick="btnToRight_Click"></asp:Button>
                        <asp:Button ID="btnAllToRight" CssClass="btn primary" Height="30px" Width="40px" Style="margin-bottom: 5px" Text=">>" runat="server" OnClick="btnAllToRight_Click"></asp:Button>
                        <asp:Button ID="btnToLeft" CssClass="btn primary" Width="40px" Height="30px" Style="margin-bottom: 5px" Text="<" runat="server" OnClick="btnToLeft_Click"></asp:Button>
                        <asp:Button ID="btnAllToLeft" CssClass="btn primary" Width="40px" Height="30px" Style="margin-bottom: 5px" Text="<<" runat="server" OnClick="btnAllToLeft_Click"></asp:Button>
                    </div>
                    <div style="float: left; width: 240px">
                        <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">已选</asp:Label>
                        <asp:ListBox ID="lbSelectedFiles" SelectionMode="Multiple" runat="server" Height="222px" Width="186px"></asp:ListBox>
                    </div>
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

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
