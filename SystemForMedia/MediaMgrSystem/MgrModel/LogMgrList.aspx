<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="LogMgrList.aspx.cs" Inherits="MediaMgrSystem.MgrModel.LogMgrList" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <script type="text/javascript">


        $(document).ready(function () {

            $("input[id$=tbStartDate]").datepicker({ dateFormat: "yy-mm-dd" });

            $("input[id$=tbEndDate]").datepicker({ dateFormat: "yy-mm-dd" });

            //$("#btnClearLog").click(function (e) {


            //    if (confirm('是否将所选日期前的日志删除?')) {

            //        var strDaysBefore = $('#ddLogDate option:selected').val();

            //        $.ajax({
            //            type: "POST",
            //            async: false,
            //            url: "LogMgrList.aspx/RemoveLogsByDaysBefore",
            //            data: "{'dayBefore':'" + strDaysBefore + "'}",
            //            dataType: "json",
            //            contentType: "application/json; charset=utf-8",
            //            success: function (msg) {


            //                if (msg.d != "-1") {
            //                    alert('删除成功');
            //                    location.reload(true);
            //                }
            //                else {
            //                    alert('删除失败');
            //                }
            //            }
            //        });
            //    }
            //});
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

        <div class="form-group" style="margin-bottom: 30px">

            <asp:Button runat="server" Text="查询日志" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Search_Click" />

        </div>



      


        <div class="form-horizontal">

            <div class="form-group">

                <div class="col-md-10">
                    <asp:GridView ID="dvList" AllowPaging="True" runat="server" AutoGenerateColumns="False" Width="850" OnRowCommand="dvList_RowCommand" OnPageIndexChanging="dvList_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="LogId" ItemStyle-Width="50px" HeaderText="编号" >

                            </asp:BoundField>
                            <asp:BoundField DataField="LogName" ItemStyle-Width="100px" HeaderText="名称" >

                            </asp:BoundField>
                            <asp:BoundField DataField="LogDesp" ItemStyle-Width="380px" HeaderText="描述" />
                            <asp:BoundField DataField="LogDate" ItemStyle-Width="160px"  HeaderText="时间" >


                            </asp:BoundField>

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

        <%--  <div class="form-group" style="  margin-top: 15px">

            <asp:Label runat="server"  CssClass="col-md-2 control-label" Width="107px">日志清理:</asp:Label>
            <div style="height: 32px; margin-top:5px; line-height: 32px; overflow: hidden;">

                <select id="ddLogDate" style="width: 220px">

                   
                    <option value="1">1天以前</option>

                    <option value="5">5天以前</option>

                    <option value="10">10天以前</option>


                </select>

                <a id="btnClearLog" style="width: 88px; line-height: 30px; height: 30px; margin-left: 5px; margin-bottom: 10px; margin-top: 0px; padding: 0px" class="btn btn-default">清空日志</a>

            </div>--%>


           


<%--        </div>--%>


        
        <div class="form-group" style="margin-bottom: 10px">


            <asp:Label runat="server" AssociatedControlID="ddDateBefore" CssClass="col-md-2 control-label" Width="107px">日志删除:</asp:Label>

            
            <div style="height: 30px; line-height: 30px; overflow: hidden;">
                <asp:DropDownList runat="server" Width="220px" Height="30px" ID="ddDateBefore" CssClass="form-control" >
                    <asp:ListItem Value="1" Text="1天以前"></asp:ListItem>
                    <asp:ListItem Value="5" Text="5天以前"></asp:ListItem>
                    <asp:ListItem Value="10" Text="10天以前"></asp:ListItem>
                </asp:DropDownList>

                <asp:Button ID="btnClearLogs" class="btn primary" Width="88px" Height="30px" Style="margin-left: 5px; margin-bottom: 10px; margin-top: 0px; padding-top: 0px" Text=" 删除日志" runat="server" CssClass="btn primary" OnClick="btnClearLog_Click"></asp:Button>


            </div>



        </div>


    </section>



</asp:Content>

<%--<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.90890</h2>





    <asp:Button runat="server" Text="新增分组" CssClass="btn btn-default" Width="215px" OnClick="Add_Click" />



    <p></p>




    




</asp:Content>--%>
