<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="GroupMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.GroupMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <form runat="server">
    <h2><%: Title %>.</h2>

    <div class="row">
        <div class="col-md-8">

              <section id="groupMgrSection">
                <div class="form-horizontal">                 
                    <hr />
                      <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="GroupName" CssClass="col-md-2 control-label">组名称</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="GroupName" CssClass="form-control"  />


                            <asp:RequiredFieldValidator runat="server" ControlToValidate="GroupName"
                                CssClass="text-danger" ErrorMessage="组名称不能为空" />

                             <asp:TextBox runat="server" ID="GroupId" Visible="false" CssClass="form-control" />
                        </div>


                    </div>

               
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <asp:Button runat="server"  Text="确认保存" CssClass="btn btn-default" />
                            <asp:Button runat="server" ValidateRequestMode="Disabled"  Text="返回" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
            
                
                  <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                        </div>
                    </div>
            </section>
           
        </div>

    
    </div>
          </form>
</asp:Content>
