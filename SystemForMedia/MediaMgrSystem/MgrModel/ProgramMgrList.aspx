﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ProgramMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ProgramMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>




    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <h3>节目信息管理</h3>


            <section id="groupMgrSection">
                <div class="form-horizontal">

                    <div class="form-group">

                        <div class="col-md-10">
                            <asp:GridView ID="dvProgameList" AllowPaging="True"  runat="server" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvProgameList_RowCommand" OnPageIndexChanging="dvProgameList_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="ProgramId" Visible="false" HeaderText="节目编号" />

                                     <asp:TemplateField HeaderText="编号">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="ProgramName" HeaderText="节目名称" />

                                    <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("ProgramId")%>' runat="server" />
                                            <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("ProgramId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                        </ItemTemplate>


                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle HorizontalAlign="Center" />
                            </asp:GridView>



                        </div>

                    </div>


                </div>



                <div class="form-group" style="margin-top: 10px">

                    <asp:Button runat="server" Text="新增节目" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />

                </div>
            </section>


        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
