<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="GroupMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.GroupMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <form runat="server">
        <h2><%: Title %>.</h2>


        <section id="groupMgrSection">
            <div class="form-horizontal">

                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>
                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="GroupName" CssClass="col-md-2 control-label" Width="107px">组名称:</asp:Label>

                    <asp:TextBox runat="server" ID="GroupName" CssClass="form-control" />

                    <asp:RequiredFieldValidator runat="server" ControlToValidate="GroupName"
                        CssClass="text-danger" ErrorMessage="组名称不能为空" />

                </div>


                    <div class="form-group" style="margin-bottom: 10px">


                          <div style="FLOAT: left; WIDTH: 200px" >  
                 <asp:ListBox ID="lbAvaibleDevice"  utoPostBack="true" SelectionMode="Multiple" runat="server" Height="226px" Width="187px"></asp:ListBox>
        </div>  
        <div style="FLOAT: left; WIDTH: 50px; margin-top:50px" >  
            <asp:button id="btnToRight" ValidateRequestMode="Disabled"  Width="40px"  Height="30px" text=">" runat="server" OnClick="btnToRight_Click"></asp:button>  
            <asp:button id="btnAllToRight"   ValidateRequestMode="Disabled"   Width="40px"  Height="30px" text=">>" runat="server"></asp:button>  
              <asp:button id="btnToLeft"    ValidateRequestMode="Disabled"  Width="40px"  Height="30px" text="<" runat="server"></asp:button>  
            <asp:button id="btnAllToLeft"   ValidateRequestMode="Disabled"   Width="40px"  Height="30px" text="<<" runat="server"></asp:button>  
        </div>  
        <div style="FLOAT: left; WIDTH: 240px" >  
               <asp:ListBox ID="lbSelectedDevice"  AutoPostBack="true" SelectionMode="Multiple" runat="server" Height="222px" Width="186px"></asp:ListBox>
        </div>  
        <div style="CLEAR: both" class="clear"></div>  
    </div>  

               
                </div>





                <div class="form-group">
                    <div class="col-md-offset-2 col-md-10">
                        <asp:Button runat="server" Text="确认保存" CssClass="btn btn-default" />
                        <asp:Button runat="server" ValidateRequestMode="Disabled" Text="返回" CssClass="btn btn-default" />
                    </div>
                </div>
            </div>


            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                </div>
            </div>
        </section>


    </form>
</asp:Content>
