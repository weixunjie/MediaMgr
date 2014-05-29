<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="GroupMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.GroupMgrList" %>



<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <form runat="server">
    <h2>uu.</h2>

    <div class="row">
        <div class="col-md-8">

              <section id="groupMgrSection">
                <div class="form-horizontal">                 
                    <hr />
                     
                    <div class="form-group">
                        
                        <div class="col-md-10">
                           <asp:GridView ID="dvGroupList" runat="server" AutoGenerateColumns="False" Width="557px">
        <Columns>
            <asp:BoundField DataField="GroupId" HeaderText="分组编号" />
            <asp:BoundField DataField="GroupName" HeaderText="分组名称" />

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="Button1" Text="Edit" CssClass="btn btn-default" CommandName="Edit" CommandArgument="<%#((GridViewRow) Container).RowIndex %>" runat="server" />
                    <asp:Button ID="Button2" CssClass="btn btn-default" Text="Delete" CommandArgument="<%#((GridViewRow) Container).RowIndex %>" CommandName="Del" runat="server" OnClientClick="return confirm('Are you sure to delete the Record?');" />
                </ItemTemplate>


            </asp:TemplateField>
        </Columns>
    </asp:GridView>

                            <br />
                            <asp:ListBox ID="ListBox1" runat="server" SelectionMode="Multiple" Width="142px">
                                <asp:ListItem Value="88">8</asp:ListItem>
                                <asp:ListItem>8</asp:ListItem>
                                <asp:ListItem>89</asp:ListItem>
                                <asp:ListItem Value="88">8</asp:ListItem>
                            </asp:ListBox>
                            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                            <br />

                            <br />
                               <asp:Button runat="server"  Text="新增组" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />

                          
                        </div>

                          <br />

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

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
