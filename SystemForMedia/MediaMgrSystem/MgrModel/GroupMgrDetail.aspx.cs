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
    public partial class GroupMgrDetail : System.Web.UI.Page
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

                if (Request["gid"]!=null)
                {
                    string groupId=Request["gid"].ToString();

                    TbHiddenId.Text = groupId;

                    GroupInfo gi = groupBLL.GetGroupById(groupId)[0];

                    TbGroupName.Text = gi.GroupName;

                    List<DeviceInfo> diSelecteds = deviceBLL.GetAllDevicesByGroup(groupId);


                    if (diSelecteds.Count > 0)
                    {
                        foreach (var di in diSelecteds)
                        {
                            lbSelectedDevice.Items.Add(new ListItem() { Text = di.DeviceName, Value = di.DeviceId });
                        }
                    }


                }
               

          

               List<DeviceInfo> diAvaibles= deviceBLL.GetAllDevicesByGroup("-1");


               if (diAvaibles.Count > 0)
               {
                   foreach (var di in diAvaibles)
                   {
                       lbAvaibleDevice.Items.Add(new ListItem() { Text = di.DeviceName, Value = di.DeviceId }); 
                   }
               }

             
            }
        }

        protected void btnToRight_Click(object sender, EventArgs e)
        {
            if (lbAvaibleDevice.SelectedItem != null)
            {
                List<ListItem> itemToRemoved = new List<ListItem>();

                foreach (ListItem item in lbAvaibleDevice.Items)
                {
                    if (item.Selected)
                    {

                        lbSelectedDevice.Items.Add(item);
                        itemToRemoved.Add(item);
                    }
                }


                if (itemToRemoved != null)
                {
                    foreach (ListItem item in itemToRemoved)
                    {
                        lbAvaibleDevice.Items.Remove(item);

                    }
                }

            }


        }

        protected void btnAllToRight_Click(object sender, EventArgs e)
        {
            if (lbAvaibleDevice.Items != null)
            {

                foreach (ListItem item in lbAvaibleDevice.Items)
                {

                    lbSelectedDevice.Items.Add(item);


                }

                lbAvaibleDevice.Items.Clear();


            }

        }

        protected void btnToLeft_Click(object sender, EventArgs e)
        {
            if (lbSelectedDevice.SelectedItem != null)
            {
                List<ListItem> itemToRemoved = new List<ListItem>();

                foreach (ListItem item in lbSelectedDevice.Items)
                {
                    if (item.Selected)
                    {

                        lbAvaibleDevice.Items.Add(item);
                        itemToRemoved.Add(item);
                    }
                }


                if (itemToRemoved != null)
                {
                    foreach (ListItem item in itemToRemoved)
                    {
                        lbSelectedDevice.Items.Remove(item);


                    }
                }

            }
        }

        protected void btnAllToLeft_Click(object sender, EventArgs e)
        {
            if (lbSelectedDevice.Items != null)
            {

                foreach (ListItem item in lbSelectedDevice.Items)
                {

                    lbAvaibleDevice.Items.Add(item);


                }

                lbSelectedDevice.Items.Clear();


            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/GroupMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {

            if (GlobalUtils.CheckIfPlaying())
            {
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alertForGroupDetail", "alert('分组正在使用，不能修改');", true);
                return;
            }

            GroupInfo gi = new GroupInfo();

            gi.GroupName = TbGroupName.Text;


            if (lbSelectedDevice.Items.Count > 0)
            {
                gi.Devices = new List<DeviceInfo>();
                foreach (ListItem item in lbSelectedDevice.Items)
                {
                    gi.Devices.Add(new DeviceInfo() { DeviceId = item.Value });
                }
            }

            if (this.lbAvaibleDevice.Items.Count > 0)
            {
                GroupInfo giUpdateNotAssinged = new GroupInfo();
                giUpdateNotAssinged.Devices = new List<DeviceInfo>();
                foreach (ListItem item in lbAvaibleDevice.Items)
                {
                    giUpdateNotAssinged.Devices.Add(new DeviceInfo() { DeviceId = item.Value });
                }

                groupBLL.UpdateDeviceEmptyGroup(giUpdateNotAssinged);
            }



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                gi.GroupId = TbHiddenId.Text;
                groupBLL.UpdateGroup(gi);
            }
            else
            {
                groupBLL.AddGroup(gi);

            }

            Response.Redirect("~/MgrModel/GroupMgrList.aspx");
        }
    }
}