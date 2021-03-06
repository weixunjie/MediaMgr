﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="EncoderAudioMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.EncoderAudioMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h3>呼叫台管理</h3>


    <section id="groupMgrSection">
        <div class="form-horizontal">

            <div class="form-group">

                <div class="col-md-10">
                    <asp:GridView ID="dvList" runat="server" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvGroupList_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="EncoderId" Visible="false" HeaderText="编号" />

                               <asp:TemplateField HeaderText="编号">
                                <ItemTemplate>
                                  <%# Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="EncoderName" HeaderText="名称" />

                            <asp:BoundField DataField="BaudRate" HeaderText="码率" />

                              <asp:BoundField DataField="ClientIdentify" HeaderText="IP地址" />


                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("EncoderId")%>' runat="server" />
                                    <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("EncoderId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                </ItemTemplate>


                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                    </asp:GridView>


                </div>

            </div>


        </div>

        <div class="form-group" style="margin-top: 10px">

            <asp:Button runat="server" Text="新增呼叫台" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />

        </div>



    </section>


</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
