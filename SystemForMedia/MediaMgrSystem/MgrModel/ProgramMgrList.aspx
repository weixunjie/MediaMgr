﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ProgramMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ProgramMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <form runat="server">
        <h3>分组信息管理</h3>


        <section id="groupMgrSection">
            <div class="form-horizontal">

                <div class="form-group">

                    <div class="col-md-10">
                        <asp:GridView ID="dvGroupList" runat="server" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvGroupList_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="GroupId" HeaderText="分组编号" />
                                <asp:BoundField DataField="GroupName" HeaderText="分组名称" />

                                <asp:TemplateField HeaderText="操作" ItemStyle-Width="150px">
                                    <ItemTemplate>
                                        <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("GroupId")%>' runat="server" />
                                        <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("GroupId")%>'  CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                    </ItemTemplate>


                                </asp:TemplateField>
                            </Columns>
                            <RowStyle HorizontalAlign="Center" />
                        </asp:GridView>

                        <br />

                        <br />
                        <asp:Button runat="server" Text="新增组" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />


                    </div>

                    <br />

                </div>


            </div>

            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                </div>
            </div>
        </section>


    </form>
</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
