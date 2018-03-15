using Google.Apis.Drive.v3;
using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sgdw
{
    public partial class gdform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String id = Request["id"];
            String url = "https://docs.google.com/forms/d/" + id + "/downloadresponses?tz_offset=7200000";
            url = "https://www.googleapis.com/drive/v3/files/171SteVTr-P8HtM5N5p8ftGFlEq_1LVQcTQ2p9yMUmDY";
            




            CGDTool gdt = new CGDTool();
            DriveService service = gdt.Authenticate(Context);
            
            service.HttpClientInitializer.Initialize(service.HttpClient);

            Task<byte[]> t = service.HttpClient.GetByteArrayAsync(url);


                string result = System.Text.Encoding.UTF8.GetString(t.Result);
                Response.Write(result);


            

        }
    }
}
/*
https://www.googleapis.com/drive/v3/files/1daO__89-wdATiNruePQLix1ypUjk0xsn


*/