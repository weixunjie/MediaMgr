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
    public partial class ProgramMgrDetail : System.Web.UI.Page
    {

        protected void LoadData_Click(object sender, EventArgs e)
        {
                       
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }


            if (Request["pid"] != null)
            {
                string pId = Request["pid"].ToString();

                TbHiddenId.Text = pId;

                ProgramInfo pi = GlobalUtils.ProgramBLLInstance.GetProgramById(pId, true)[0];

                this.TbProgrmeName.Text = pi.ProgramName;


                if (pi.MappingFiles != null && pi.MappingFiles.Count > 0)
                {
                    foreach (var fileInfo in pi.MappingFiles)
                    {
                        this.lbSelectedFiles.Items.Add(new ListItem() { Text = fileInfo.FileName, Value = fileInfo.FileName });
                    }
                }


            }



            string fileBasePath = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["filePath"].ToString();

            string mpgExePath = Server.MapPath(@"~\Dlls");


            List<FileAttribute> faAvaibles = GlobalUtils.FileInfoBLLInstance.GetAllDiskFiles(fileBasePath, mpgExePath);


            if (faAvaibles.Count > 0)
            {
                foreach (var fa in faAvaibles)
                {
                    bool isFound = false;
                    foreach (ListItem sel in lbSelectedFiles.Items)
                    {
                        if (sel.Value == fa.FileName)
                        {
                            isFound = true;
                            break;
                        }

                    }

                    if (!isFound)
                    {
                        this.lbAvaibleFiles.Items.Add(new ListItem() { Text = fa.FileName, Value = fa.FileName });
                    }

                }
            }

            groupMgrSection.Visible = true;


            ss.Visible = false;


        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {

                ss.Visible = true;
                groupMgrSection.Visible = false;
            }

            //UpdatePanel1.TemplateControl.set = false;

        }

        protected void btnToRight_Click(object sender, EventArgs e)
        {
            if (this.lbAvaibleFiles.SelectedItem != null)
            {
                List<ListItem> itemToRemoved = new List<ListItem>();

                foreach (ListItem item in lbAvaibleFiles.Items)
                {
                    if (item.Selected)
                    {

                        this.lbSelectedFiles.Items.Add(item);
                        itemToRemoved.Add(item);
                    }
                }


                if (itemToRemoved != null)
                {
                    foreach (ListItem item in itemToRemoved)
                    {
                        lbAvaibleFiles.Items.Remove(item);

                    }
                }

            }


        }

        protected void btnAllToRight_Click(object sender, EventArgs e)
        {
            if (lbAvaibleFiles.Items != null)
            {

                foreach (ListItem item in this.lbAvaibleFiles.Items)
                {

                    lbSelectedFiles.Items.Add(item);


                }

                lbAvaibleFiles.Items.Clear();


            }

        }

        protected void btnToLeft_Click(object sender, EventArgs e)
        {
            if (this.lbSelectedFiles.SelectedItem != null)
            {
                List<ListItem> itemToRemoved = new List<ListItem>();

                foreach (ListItem item in lbSelectedFiles.Items)
                {
                    if (item.Selected)
                    {

                        lbAvaibleFiles.Items.Add(item);
                        itemToRemoved.Add(item);
                    }
                }


                if (itemToRemoved != null)
                {
                    foreach (ListItem item in itemToRemoved)
                    {
                        lbSelectedFiles.Items.Remove(item);


                    }
                }

            }
        }

        protected void btnAllToLeft_Click(object sender, EventArgs e)
        {
            if (lbSelectedFiles.Items != null)
            {

                foreach (ListItem item in lbSelectedFiles.Items)
                {

                    lbAvaibleFiles.Items.Add(item);


                }

                lbSelectedFiles.Items.Clear();

            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ProgramMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            ProgramInfo pi = new ProgramInfo();

            pi.ProgramName = this.TbProgrmeName.Text;




            if (this.lbSelectedFiles.Items.Count <= 0)
            {
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alert", "alert('请至少选择一个文件');", true);

                return;
            }

            if (this.lbSelectedFiles.Items.Count > 0)
            {
                pi.MappingFiles = new List<FileAttribute>();
                foreach (ListItem item in lbSelectedFiles.Items)
                {
                    pi.MappingFiles.Add(new FileAttribute() { FileName = item.Value });
                }
            }




            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                pi.ProgramId = TbHiddenId.Text;
                GlobalUtils.ProgramBLLInstance.UpdateProgram(pi);
            }
            else
            {
                GlobalUtils.ProgramBLLInstance.AddPrograme(pi);

            }


            Response.Redirect("~/MgrModel/ProgramMgrList.aspx");
        }
    }
}