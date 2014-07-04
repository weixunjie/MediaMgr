<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ScheduleTaskMgrDetail.aspx.cs" Inherits="MediaMgrSystem.MgrModel.ScheduleTaskMgrDetail" %>




<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>




    <script type="text/javascript">


        Sys.Application.add_load(EndRequestHandler);


        function EndRequestHandler() {

            $("input[id$=tbSelectDate]").datepicker({
                dateFormat: "yy-mm-dd", changeYear: true,
                changeMonth: true,
                numberOfMonths: 1,

            });

        }
    </script>



    <style>
        .cb td {
            width: 100px;
            text-align: center;
        }

        .cb label {
            float: left;
            display: inline-block;
        }

        .cb input {
            float: left;
        }
    </style>




    <asp:UpdatePanel ID="UpdatePanel1"  runat="server">
        <ContentTemplate>





            <h3 style="clip: rect(auto, auto, 10px, auto)">任务明细管理</h3>

            <asp:Label ID="lbMessage" runat="server" Text="" Visible="false" ForeColor="Red" Font-Size="Larger" Height="30px"></asp:Label>


            <section>

                <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="FailureText" />
                    </p>
                </asp:PlaceHolder>
                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="TbName" CssClass="col-md-2 control-label" Width="107px">任务名称:</asp:Label>

                    <asp:TextBox runat="server" ID="TbName" CssClass="form-control" />

                    <asp:RequiredFieldValidator Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="TbName"
                        ForeColor="Red" ErrorMessage=" 名称不能为空" Height="25px" />

                    <asp:TextBox runat="server" ID="TbHiddenId" CssClass="form-control" Visible="False" />

                    <asp:TextBox runat="server" ID="TbHiddenIdSchedule" CssClass="form-control" Visible="False" />

                </div>

                <div class="form-group" style="margin-bottom: 10px">

                    <asp:Label runat="server" AssociatedControlID="CbWeek" CssClass="col-md-2 control-label" Width="107px">星期:</asp:Label>


                    <asp:CheckBoxList ID="CbWeek" runat="server" Height="40px" RepeatColumns="8" RepeatDirection="Horizontal" Width="566px" CssClass="cb">
                        <asp:ListItem Value="1">星期一</asp:ListItem>
                        <asp:ListItem Value="2">星期二</asp:ListItem>
                        <asp:ListItem Value="3">星期三</asp:ListItem>
                        <asp:ListItem Value="4">星期四</asp:ListItem>
                        <asp:ListItem Value="5">星期五</asp:ListItem>
                        <asp:ListItem Value="6">星期六</asp:ListItem>
                        <asp:ListItem Value="7">星期日</asp:ListItem>
                    </asp:CheckBoxList>


                </div>


                <div class="form-group" style="margin-bottom: 10px">



                    <div style="float: left; width: 200px">
                        <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">特殊日期</asp:Label>
                        <asp:ListBox ID="lbSelectedDate" utoPostBack="true" SelectionMode="Multiple" runat="server" Height="75px" Width="187px"></asp:ListBox>
                    </div>

                    <div style="float: left; width: 240px">

                        <asp:Label runat="server" CssClass="col-md-2 control-label" Width="107px">日期选择</asp:Label>

                        <input type="text" runat="server" id="tbSelectDate" />



                        <asp:Button ID="btnDelSelected" Width="115px" CssClass="btn primary" Height="30px" Style="margin-right: 15px" Text="删除选中" runat="server" OnClick="btnDelSelected_Click"></asp:Button>
                        <asp:Button ID="btnAddDate" Width="88px" CssClass="btn primary" Height="30px" Style="margin-bottom: 5px" Text="添加日期" runat="server" OnClick="btnAddDate_Click"></asp:Button>

                    </div>





                    <div class="form-group" style="margin-bottom: 10px">

                        <asp:Label runat="server" AssociatedControlID="btnPreview" CssClass="col-md-2 control-label" Width="107px">节目:</asp:Label>

                        <div style="height: 30px; line-height: 30px; overflow: hidden;">
                            <asp:DropDownList runat="server" Width="220px" Height="30px" ID="ddProgram" CssClass="form-control" style="margin-right:5px" />

                         

                            <asp:Button ID="btnPreview"  class="btn primary" Width="88px" Height="30px" Style="; margin-bottom: 10px;" Text=" 试听" runat="server" CssClass="btn primary" OnClick="btnPreview_Click"></asp:Button>

                        </div>

                    </div>

                    <div class="form-group" style="margin-bottom: 10px">


                        <asp:Label runat="server" AssociatedControlID="tbStartTime" CssClass="col-md-2 control-label" Width="107px">开始时间</asp:Label>

                        <input type="text" style="margin-top: 10px" runat="server" id="tbStartTime" name="tbStartTime" />


                        <asp:RequiredFieldValidator ID="rfStartTime" Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbStartTime"
                            ForeColor="Red" ErrorMessage=" 开始时间不能为空" Height="25px" />

                    </div>


                    <div class="form-group" style="margin-bottom: 10px">

                        <asp:Label runat="server" AssociatedControlID="tbEndTime" CssClass="col-md-2 control-label" Width="107px">结束时间</asp:Label>

                        <input type="text" style="margin-top: 10px" runat="server" name="tbEndTime" id="tbEndTime" />


                        <asp:RequiredFieldValidator ID="rfEndTime" Style="vertical-align: middle" ValidationGroup="inputValidate" runat="server" ControlToValidate="tbEndTime"
                            ForeColor="Red" ErrorMessage=" 结束时间不能为空" Height="25px" />


                    </div>


                    <div class="form-group" style="margin-bottom: 10px">

                        <asp:Label runat="server" AssociatedControlID="cbIsRepeat" CssClass="col-md-2 control-label" Width="107px">顺序播放:</asp:Label>


                        <asp:CheckBox ID="cbIsRepeat" Text="" runat="server" />

                    </div>


                    <div class="form-group" style="margin-bottom: 10px">


                        <asp:Label runat="server" AssociatedControlID="ddPriority" CssClass="col-md-2 control-label" Width="107px">优先级</asp:Label>

                        <asp:DropDownList runat="server" Width="220px" ID="ddPriority" CssClass="form-control" />


                        <div style="clear: both" class="clear"></div>
                    </div>


                    <div class="form-group">

                        <asp:Button runat="server" Text="确认保存" ValidationGroup="inputValidate" CssClass="btn btn-default" OnClick="Add_Click" />
                        <asp:Button runat="server" ValidateRequestMode="Disabled" Text="返回" CssClass="btn btn-default" OnClick="Back_Click" />

                    </div>
            </section>

        </ContentTemplate>

    </asp:UpdatePanel>



</asp:Content>
