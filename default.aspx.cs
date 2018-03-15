using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sgdw
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void WriteFiles()
        {
            String parentID = "1SAvLu-iPgghf7sksRo6LW7BYttXQzkxO";
            if (Request["parent"] != null)
                parentID = Request["parent"];

            CGDTool gdt = new CGDTool();
            DriveService service = gdt.Authenticate(Context);
            DriveFile[] files = gdt.GetFiles(service, parentID);

            foreach (DriveFile file in files)
            {
                Response.Write("<tr>");
//                Response.Write("<td>" + file.id + "</td>");
//                Response.Write("<td>" + file.parentID + "</td>");
                Response.Write("<td>" + file.simpleType + "</td>");
                Response.Write("<td>" + file.name + "</td>");
                Response.Write("<td>" + file.description + "</td>");

                String link = ".";
                switch (file.simpleType)
                {
                    case "folder":
                        link = "./?parent=" + file.id;
                        break;
                    case "image":
                        link = "gdimage.aspx?id=" + file.id + "&type=" + file.type;
                        break;
                    case "doc":
                        link = "gddoc.aspx?id=" + file.id;
                        break;
                    case "json":
                        link = "gdjson.aspx?id=" + file.id;
                        break;
                    case "form":
                        link = "gdform.aspx?id=" + file.id;
                        break;
                    case "mindmup":
                        link = "https://drive.mindmup.com/map/" + file.id;
                        break;
                }

                Response.Write("<td><a href='" + link + "'>Open</a></td>");
                Response.Write("<td>" + file.webViewLink + "</td>");
                Response.Write("</tr>");

            }

        }
    }
}