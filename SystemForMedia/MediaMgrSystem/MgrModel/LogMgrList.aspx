<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="LogMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.LogMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">


    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>



    <script type="text/javascript">


        $(document).ready(function () {

            

            $("input[id$=tbStartDate]").datepicker({
                dateFormat: "yy-mm-dd", changeYear: true,
                changeMonth: true,
                numberOfMonths: 1,
          
            });

            $("input[id$=tbEndDate]").datepicker({
                dateFormat: "yy-mm-dd", changeYear: true,
                changeMonth: true,
                numberOfMonths: 1,              
            });


        });


    </script>


    <h3>日志管理</h3>


    <section id="groupMgrSection">

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="TBName" CssClass="col-md-2 control-label" Width="107px">名称:</asp:Label>

            <asp:TextBox runat="server" ID="TBName" CssClass="form-control" />

        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbStartDate" CssClass="col-md-2 control-label" Width="107px">开始日期</asp:Label>

            <input type="text" runat="server" id="tbStartDate" />

        </div>

        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="tbEndDate" CssClass="col-md-2 control-label" Width="107px">结束选择</asp:Label>

            <input type="text" runat="server" id="tbEndDate" />

        </div>


        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <div class="form-group" style="margin-bottom: 30px">

                    <asp:Button runat="server" Text="查询日志" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Search_Click" />

                </div>
                


                <div class="form-horizontal">

                    <div class="form-group">

                        <div class="col-md-10">
                            <asp:GridView ID="dvList" AllowPaging="True" runat="server" AutoGenerateColumns="False" Width="850" OnRowCommand="dvList_RowCommand" OnPageIndexChanging="dvList_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="LogId" ItemStyle-Width="50px" HeaderText="编号"></asp:BoundField>
                                    <asp:BoundField DataField="LogName" ItemStyle-Width="100px" HeaderText="名称"></asp:BoundField>
                                    <asp:BoundField DataField="LogDesp" ItemStyle-Width="380px" HeaderText="描述" />
                                    <asp:BoundField DataField="LogDate" ItemStyle-Width="160px" HeaderText="时间"></asp:BoundField>

                                    <asp:TemplateField HeaderText="操作" ItemStyle-Width="50px">
                                        <ItemTemplate>

                                            <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("LogId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                        </ItemTemplate>


                                        <ItemStyle Width="70px"></ItemStyle>


                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle HorizontalAlign="Center" />
                            </asp:GridView>


                        </div>



                    </div>


                </div>


                <div class="form-group" style="margin-top: 15px">


                    <asp:Label runat="server" AssociatedControlID="ddDateBefore" CssClass="col-md-2 control-label" Width="107px">日志删除:</asp:Label>


                    <div style="height: 30px; line-height: 30px; overflow: hidden;">
                        <asp:DropDownList runat="server" Width="220px" Height="30px" ID="ddDateBefore" style="margin-right:5px" CssClass="form-control">
                            <asp:ListItem Value="1" Text="1天以前"></asp:ListItem>
                            <asp:ListItem Value="5" Text="5天以前"></asp:ListItem>
                            <asp:ListItem Value="10" Text="10天以前"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:Button ID="btnClearLogs" OnClientClick="return confirm('是否删除时段的日志?');"  class="btn primary" Width="88px" Height="30px" Style="margin-bottom: 10px;" Text=" 删除日志" runat="server" CssClass="btn primary" OnClick="btnClearLog_Click"></asp:Button>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </section>



</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
