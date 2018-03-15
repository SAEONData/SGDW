using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sgdw
{
    public partial class jdjson : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String id = Request["id"];
            String type = Request["type"];

            FileDownloader fd = new FileDownloader(Context);
            byte[] data = fd.Download(id, type, false);
            if (data == null)
            {
                Response.Write("ERROR<hr>");
                foreach (String msg in fd.messages)
                    Response.Write(msg + "<br>");
            }
            else
            {
                Response.ContentType = type;
                Response.BinaryWrite(data);
            }

        }
    }
}