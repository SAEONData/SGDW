using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace sgdw
{
    public class CGDTool
    {
        protected static string accountMail = "sgdw-283@sgdw-197412.iam.gserviceaccount.com";
        protected static string accountFile = "SGDW-673cdca57812.json";

        public DriveService Authenticate(HttpContext context)
        {
            if (context.Session["gdservice"] == null)
                context.Session["gdservice"] = AuthenticateServiceAccount(accountMail, context.Server.MapPath(accountFile));
            return (DriveService)context.Session["gdservice"];
        }

        /// <summary>
        /// Authenticating to Google using a Service account
        /// Documentation: https://developers.google.com/accounts/docs/OAuth2#serviceaccount
        /// </summary>
        /// <param name="serviceAccountEmail">From Google Developer console https://console.developers.google.com</param>
        /// <param name="serviceAccountCredentialFilePath">Location of the .p12 or Json Service account key file downloaded from Google Developer console https://console.developers.google.com</param>
        /// <returns>AnalyticsService used to make requests against the Analytics API</returns>
        protected DriveService AuthenticateServiceAccount(string serviceAccountEmail, string serviceAccountCredentialFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceAccountCredentialFilePath))
                    throw new Exception("Path to the service account credentials file is required.");
                if (!System.IO.File.Exists(serviceAccountCredentialFilePath))
                    throw new Exception("The service account credentials file does not exist at: " + serviceAccountCredentialFilePath);
                if (string.IsNullOrEmpty(serviceAccountEmail))
                    throw new Exception("ServiceAccountEmail is required.");

                // These are the scopes of permissions you need. It is best to request only what you need and not all of them
                // string[] scopes = new string[] { AnalyticsReportingService.Scope.Analytics };             // View your Google Analytics data
                string[] scopes = new string[] {  DriveService.Scope.Drive,                  // view and manage your files and documents
                                         DriveService.Scope.DriveAppdata,                    // view and manage its own configuration data
                                         DriveService.Scope.DriveFile,                       // view and manage files created by this app
                                         DriveService.Scope.DriveScripts };             

//                                         DriveService.Scope.DriveMetadataReadonly,           // view metadata for files
//                                         DriveService.Scope.DriveReadonly,                   // view files and documents on your drive



                // For Json file
                if (Path.GetExtension(serviceAccountCredentialFilePath).ToLower() == ".json")
                {
                    GoogleCredential credential;
                    using (var stream = new FileStream(serviceAccountCredentialFilePath, FileMode.Open, FileAccess.Read))
                    {
                        // credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
                        credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
                    }


                    


                    // Create the  Analytics service.
                    return new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Drive Service account Authentication Sample",
                    });
                }
                else if (Path.GetExtension(serviceAccountCredentialFilePath).ToLower() == ".p12")
                {   // If its a P12 file

                    var certificate = new X509Certificate2(serviceAccountCredentialFilePath, "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                    var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
                    {
                        Scopes = scopes
                    }.FromCertificate(certificate));

                    // Create the  Drive service.
                    return new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Drive Authentication Sample",
                    });
                }
                else
                {
                    throw new Exception("Unsupported Service accounts credentials.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Create service account DriveService failed" + ex.Message);
                throw new Exception("CreateServiceAccountDriveFailed", ex);
            }
        }

        public DriveFile[] GetFiles(DriveService service, String parentID)
        {
            List<DriveFile> fileList = new List<DriveFile>();


            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name, parents, mimeType, description, webViewLink, webContentLink, isAppAuthorized)";
            listRequest.Q = "trashed=false";
            if (parentID != "")
                listRequest.Q += " AND '" + parentID + "' in parents";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    DriveFile df = new DriveFile();
                    df.id = file.Id;
                    df.parentID = file.Parents == null ? "!!! " : file.Parents.Count > 0 ? file.Parents[0] : "??";
                    df.type = file.MimeType;
                    df.name = file.Name;
                    df.description = file.Description;
                    df.webViewLink = file.IsAppAuthorized.ToString();



                    if (df.type.Contains("apps.folder"))
                        df.simpleType = "folder";
                    else if (df.type.Contains("apps.document"))
                        df.simpleType = "doc";
                    else if (df.type.Contains("application/vnd.google-apps.form"))
                        df.simpleType = "form";
                    else if (df.type.Contains("image/"))
                        df.simpleType = "image";
                    else if (df.type.Contains("application/json"))
                        df.simpleType = "json";
                    else if (df.type.Contains("sdk.758379822725"))
                        df.simpleType = "mindmup";
                    else
                        df.simpleType = df.type;

                    fileList.Add(df);
                }
            }

            return fileList.ToArray();

        }
    }

    public class DriveFile
    {
        public string id;
        public string parentID;
        public string type;
        public string simpleType;
        public string name;
        public string webViewLink;
        public string description;
    }

}

