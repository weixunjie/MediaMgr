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



                    var found = ddGroups.Items.FindByValue(di.GroupId);

                    if (found != null)
                    {
                        found.Selected = true;
                    }


                }



            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/DeviceMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            DeviceInfo di = new DeviceInfo();

            di.DeviceName = TbName.Text;

            di.DeviceIpAddress = this.TBIPAddress.Text;




            di.GroupId = ddGroups.SelectedItem.Value;




            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                di.DeviceId = TbHiddenId.Text;

                deviceBLL.UpdateDevice(di);
            }
            else
            {
                deviceBLL.AddDevice(di);
            }

            Response.Redirect("~/MgrModel/DeviceMgrList.aspx");
        }
    }
}