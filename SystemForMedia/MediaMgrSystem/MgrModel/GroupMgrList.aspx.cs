using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem.MgrModel
{
    public partial class GroupMgrList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindListData();
            }
        }



        protected void Add_Click(object sender, EventArgs e)
        {

           // ListBox1.Items.RemoveAt(1);
            Response.Redirect("~/MgrModel/GroupMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<GroupInfo> groups = new List<GroupInfo>();

            groups.Add(new GroupInfo { GroupId = "1", GroupName = "test" });
            groups.Add(new GroupInfo { GroupId = "2", GroupName = "test" });

            dvGroupList.DataSource = groups;
            dvGroupList.DataBind();







        }
    }
}