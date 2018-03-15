using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sgdw
{
    public partial class gdmindmup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String id = Request["id"];
            String type = "text/plain";


            FileDownloader fd = new FileDownloader(Context);

            byte[] data = fd.Download(id, type, true);
            if (data == null)
            {
                Response.Write("ERROR<hr>");
                foreach (String msg in fd.messages)
                    Response.Write(msg + "<br>");
            }
            else
            {
                String find = "style=\"background-color:#ffffff;padding:72pt 72pt 72pt 72pt;max-width:451.4pt\"";
                String text = System.Text.Encoding.UTF8.GetString(data);
                text = text.Replace(find, "");
                Response.Write(text);
            }

        }
    }
}