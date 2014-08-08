<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ScheduleMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ScheduleMgrDetail" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">


        <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
    <h3 style="clip: rect(auto, auto, 10px, auto)">计划明细管理</h3>


    <section id="groupMgrSection">

        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>
        <div class="form-group" style="margin-bottom: 10px">

            <asp:Label runat="server" AssociatedControlID="TbName" CssClass="col-md-2 control-label" Width="107px">计划名称:</asp:Label>

            <asp:TextBox runat="server" ID="TbName" CssClass="form-control" />

            <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TbName"
                ForeColor="Red" ErrorMessage=" 名称不能为空" Height="25px" />

            <asp:TextBox runat="server" ID="TbHiddenId" CssClass="form-control" Visible="False" />

        </div>

        <div style="clear: both" class="clear"></div>


        <div class="form-group">

            <asp:Button runat="server" Text="确认保存" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Add_Click" />
            <asp:Button runat="server" ValidateRequestMode="Disabled" Text="返回" CssClass="btn btn-default" OnClick="Back_Click" />

            <br />

        </div>

        <div style="clear: both" class="clear"></div>

        <div id="divTask" style="margin-top: 15px" runat="server">

            <div class="form-group" style="margin-bottom: 10px">
                <h4 style="clip: rect(auto, auto, 10px, auto)">任务列表</h4>
            </div>
                       

            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                </div>
            </div>


            <div class="form-group">

                <div class="col-md-10">
                    <asp:GridView ID="dvTaskList"  AllowPaging="True" runat="server" AutoGenerateColumns="False" Width="857px" OnRowCommand="dvTaskList_RowCommand" OnRowDataBound="dvTaskList_RowDataBound"   OnPageIndexChanging="dvTaskList_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="ScheduleTaskId" Visible="false" HeaderText="编号" />

                               <asp:TemplateField HeaderText="编号">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                            <asp:BoundField DataField="ScheduleTaskName" HeaderText="名称" />

                            <asp:BoundField DataField="ScheduleTaskStartTime" HeaderText="开始时间" />

                            <asp:BoundField DataField="ScheduleTaskEndTime" HeaderText="结束时间" />

                            <asp:BoundField DataField="ScheduleTaskPriority" HeaderText="优先级" />

                            <asp:BoundField DataField="ScheduleTaskProgarmId" HeaderText="节目" />

                            <asp:BoundField DataField="StrWeeks" HeaderText="播放星期" />

                            <asp:BoundField DataField="StrDays" HeaderText="特别日期" />


                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="125px">
                                <ItemTemplate>
                                    <asp:Button ID="Button1" Text="编辑" CssClass="btn btn-default" CommandName="Edit" CommandArgument='<%# Eval("ScheduleTaskId")%>' runat="server" />
                                    <asp:Button ID="Button2" CssClass="btn btn-default" Text="删除" CommandArgument='<%# Eval("ScheduleTaskId")%>' CommandName="Del" runat="server" OnClientClick="return confirm('是否删除该记录?');" />
                                </ItemTemplate>


                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </div>

            <div class="form-group" style="margin-top: 10px">
                <asp:Button runat="server" Text="新增任务" CssClass="btn btn-default" OnClick="AddTask_Click" Width="195px" />
            </div>

        </div>
    </section>

            </ContentTemplate>
        </asp:UpdatePanel>

</asp:Content>
