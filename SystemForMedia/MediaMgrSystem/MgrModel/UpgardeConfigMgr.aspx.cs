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
    public partial class UpgardeConfigMgr : System.Web.UI.Page
    {
       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!Page.IsPostBack)
            {


                UpgradeInfo ui = GlobalUtils.UpgradeConfigBLLInstance.GetUpgradeConfig("1");

                if (ui != null)
                {

                    TbVCUpgardeUrl.Text = ui.UpgardeUrl;
                    TbVCVersionId.Text = ui.VersionId;
                }

                ui = null;
                ui = GlobalUtils.UpgradeConfigBLLInstance.GetUpgradeConfig("2");

                if (ui != null)
                {

                    this.TbRmsUpgardeUrl.Text = ui.UpgardeUrl;
                    tbRmsVersion.Text = ui.VersionId;
                }
            }
        }



        protected void Add_Click(object sender, EventArgs e)
        {

            UpgradeInfo ui = new UpgradeInfo();
            ui.VersionType = "1";
            ui.UpgardeUrl = TbVCUpgardeUrl.Text;
            ui.VersionId = TbVCVersionId.Text;

            GlobalUtils.UpgradeConfigBLLInstance.UpdateUpgradeConfig(ui);

            ui = new UpgradeInfo();
            ui.VersionType = "2";
            ui.UpgardeUrl = this.TbRmsUpgardeUrl.Text;
            ui.VersionId = this.tbRmsVersion.Text;

            GlobalUtils.UpgradeConfigBLLInstance.UpdateUpgradeConfig(ui);

            ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "updateScript233", "alert('保存成功！')", true);
            


        }
    }
}