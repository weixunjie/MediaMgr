using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem.MgrModel
{
    public partial class DeviceMgrDetail : System.Web.UI.Page
    {
        private GroupBLL groupBLL = new GroupBLL(GlobalUtils.DbUtilsInstance);
        private DeviceBLL deviceBLL = new DeviceBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!Page.IsPostBack)
            {

                List<GroupInfo> allGroup = groupBLL.GetAllGroups();


                allGroup.Insert(0, new GroupInfo() { GroupId = "-1", GroupName = "默认分组" });

                ddGroups.DataSource = allGroup;

                ddGroups.DataValueField = "GroupID";

                ddGroups.DataTextField = "GroupName";

                ddGroups.DataBind();


                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    DeviceInfo di = deviceBLL.GetADevicesById(id)[0];

                    this.TbName.Text = di.DeviceName;
                    this.TBIPAddress.Text = di.DeviceIpAddress;

                    cbFunction.Items[0].Selected = di.UsedToAudioBroandcast;

                    cbFunction.Items[1].Selected = di.UsedToVideoOnline;

                    cbFunction.Items[2].Selected = di.UsedToRemoteControl;
                    
                    var found = ddGroups.Items.FindByValue(di.GroupId);

                    if (found != null)
                    {
                        found.Selected = true;
                    }


                }

                else
                {
                    cbFunction.Items[0].Selected = true;
                }


            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/DeviceMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            lbMessage.Visible = false;
            if (GlobalUtils.CheckIfPlaying())
            {
                lbMessage.Visible = true;
                lbMessage.Text = "分组正在使用，不能修改";
                return;                    
            }

           
            DeviceInfo di = new DeviceInfo();

            di.DeviceName = TbName.Text;

            di.DeviceIpAddress = this.TBIPAddress.Text;


            di.GroupId = ddGroups.SelectedItem.Value;

            di.UsedToAudioBroandcast = cbFunction.Items[0].Selected;

            di.UsedToVideoOnline = cbFunction.Items[1].Selected;

            di.UsedToRemoteControl = cbFunction.Items[2].Selected;

            
            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                di.DeviceId = TbHiddenId.Text;

                deviceBLL.UpdateDevice(di);
            }
            else
            {
                if (deviceBLL.AddDevice(di) == -1)
                {
                    lbMessage.Text = "不能添加终端，端已经达到最大数量";
                    lbMessage.Visible = true;

                    return;
                }
            }

            Response.Redirect("~/MgrModel/DeviceMgrList.aspx");
        }
    }
}