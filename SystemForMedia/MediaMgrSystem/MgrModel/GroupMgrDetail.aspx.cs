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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lbAvaibleDevice.Items.Add(new ListItem() { Text = "ff", Value = "5" });
                lbAvaibleDevice.Items.Add(new ListItem() { Text = "ff", Value = "56" });
                lbAvaibleDevice.Items.Add(new ListItem() { Text = "ff", Value = "5333" });

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
    }
}