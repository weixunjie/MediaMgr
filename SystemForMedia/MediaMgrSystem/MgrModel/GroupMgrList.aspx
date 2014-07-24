<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="GroupMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.GroupMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h3>分组信息管理</h3>


    <section id="groupMgrSection">
        <div class="form-horizontal">

            <div class="form-group">

                <div class="col-md-10">
                    <asp:GridView ID="dvGroupList" runat="server" AllowPaging="True" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvGroupList_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="GroupId" Visible="false" HeaderText="分组编号" />

                            <asp:TemplateField HeaderText="编号">
                                <ItemTemplate>
                                    <%# this.dvGroupList.PageIndex * this.dvGroupList.PageSize + Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="GroupName" HeaderText="分组名称" />

                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("GroupId")%>' runat="server" />
                                    <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("GroupId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                </ItemTemplate>


                                <ItemStyle Width="135px"></ItemStyle>


                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                    </asp:GridView>

                </div>

            </div>


        </div>

        <div class="form-group" style="margin-top: 10px">
            <asp:Button runat="server" Text="新增组" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />
        </div>
    </section>



</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
