<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="DeviceMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.DeviceMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>




    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <h3>设备信息管理</h3>



            <section id="groupMgrSection">
                <div class="form-horizontal">

                    <div class="form-group">

                        <div class="col-md-10">
                            <asp:GridView ID="dvList" AllowPaging="true" runat="server" AutoGenerateColumns="False" Width="557px" OnRowCommand="dvGroupList_RowCommand" OnRowDataBound="dvList_RowDataBound" OnPageIndexChanging="dvList_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="DeviceId" Visible="false" HeaderText="编号" />

                                    <asp:TemplateField HeaderText="编号">
                                        <ItemTemplate>
                                            <%#   Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="DeviceName" HeaderText="名称" />

                                    <asp:BoundField DataField="GroupId" HeaderText="所在分组" />

                                    <asp:BoundField DataField="DeviceIPAddress" HeaderText="Ip地址" />

                                    <asp:BoundField DataField="macAddress" HeaderText="Mac地址" />


                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="True"
                                                OnCheckedChanged="chkAll_CheckedChanged" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkItem" runat="server" />
                                            <asp:Label runat="server" ID="lbId" Visible="false" Text='<%# Eval("DeviceId")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("DeviceId")%>' runat="server" />
                                            <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("DeviceId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                        </ItemTemplate>


                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle HorizontalAlign="Center" />
                            </asp:GridView>


                        </div>

                    </div>


                </div>


                <div class="form-group" style="margin-top: 10px">

                    <asp:Button runat="server" Text="新增设备" CssClass="btn btn-default" OnClick="Add_Click" Width="195px" />
                     &nbsp;
                        <asp:Button ID="Button3" runat="server" CssClass="btn btn-default" OnClick="Button3_Click" Text="批量删除 " Width="195px" />
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
