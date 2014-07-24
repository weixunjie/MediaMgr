<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UserMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.UserMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h3>用户信息管理</h3>


    <section id="groupMgrSection">
        <div class="form-horizontal">

            <div class="form-group">

                <div class="col-md-10">
                    <asp:GridView ID="dvList" runat="server" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvGroupList_RowCommand" OnRowDataBound="dvList_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="UserId" HeaderText="" Visible="false" />

                            <asp:TemplateField HeaderText="编号">
                                <ItemTemplate>
                                    <%# this.dvList.PageIndex * this.dvList.PageSize + Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="UserCode" HeaderText="用户编号" />
                            <asp:BoundField DataField="UserName" HeaderText="用户姓名" />

                            <asp:BoundField DataField="UserLevel" HeaderText="用户级别" />

                            <asp:BoundField DataField="IsActive" HeaderText="是否生效" />

                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("UserId")%>' runat="server" />
                                    <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("UserId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                </ItemTemplate>


                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                    </asp:GridView>

                    <br />

                    <br />
                    <asp:Button runat="server" Text="新增用户" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />


                </div>

                <br />

            </div>


        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
            </div>
        </div>
    </section>



</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
