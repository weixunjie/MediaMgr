<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ChannelMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ChannelMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h3>通道管理</h3>


    <section id="groupMgrSection">
        <div class="form-horizontal">

            <div class="form-group">

                <div class="col-md-10">
                    <asp:GridView ID="dvList" runat="server" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvGroupList_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="ChannelId" Visible="false" HeaderText="编号" />

                                                        <asp:TemplateField HeaderText="编号">
                                <ItemTemplate>
                                    <%# this.dvList.PageIndex * this.dvList.PageSize + Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="ChannelName" HeaderText="名称" />

                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("ChannelId")%>' runat="server" />
                                    <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("ChannelId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                </ItemTemplate>


                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                    </asp:GridView>






                </div>


            </div>


            <div class="form-group" style="margin-top:10px" >
                <asp:Button runat="server" Text="新增通道" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />
            </div>

        </div>
      
    </section>


</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
