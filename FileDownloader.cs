using Google.Apis.Download;
using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;

namespace sgdw
{
    public class FileDownloader
    {
        protected HttpContext context;
        BackgroundWorker worker;
        string id;
        string mimetype;
        bool export;
        bool threadDone = false;
        byte[] data = null;

        public List<String> messages = new List<string>();


        public FileDownloader(HttpContext c)
        {
            context = c;
        }

        public byte[] Download(String id, String mimetype, bool export)
        {
            this.id = id;
            this.mimetype = mimetype;
            this.export = export;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(ProgressChanged);
            worker.RunWorkerAsync();

            int sleepCount = 0;
            while (threadDone == false && sleepCount < 200)
            {
                Thread.Sleep(100);
                sleepCount++;
            }

            return data;
        }

        private void UpateStatus(String status, bool done)
        {
            messages.Add(status);
            if (done == true)
                this.threadDone = true;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            CGDTool gdt = new CGDTool();
            DriveService driveService = gdt.Authenticate(context);

            var fileId = id;
            try
            {
                if (export)
                {
                    var request = driveService.Files.Export(fileId, this.mimetype);
                    var stream = new System.IO.MemoryStream();
                    request.MediaDownloader.ProgressChanged +=
                        (IDownloadProgress progress) =>
                        {
                            switch (progress.Status)
                            {
                                case DownloadStatus.Downloading:
                                    {
                                        UpateStatus("bytes" + progress.BytesDownloaded, false);
                                        break;
                                    }
                                case DownloadStatus.Completed:
                                    {
                                        data = stream.ToArray();
                                        UpateStatus("Download complete", true);
                                        break;
                                    }
                                case DownloadStatus.Failed:
                                    {
                                        UpateStatus("Download failed: " + progress.Exception.Message, true);
                                        break;
                                    }
                            }
                        };
                    request.Download(stream);
                }
                else
                {
                    var stream = new System.IO.MemoryStream();

                // https://docs.google.com/forms/d/171SteVTr-P8HtM5N5p8ftGFlEq_1LVQcTQ2p9yMUmDY/downloadresponses?tz_offset=7200000

                    Google.Apis.Drive.v3.FilesResource.GetRequest request = driveService.Files.Get(fileId);
                    
                    request.MediaDownloader.ProgressChanged +=
                        (IDownloadProgress progress) =>
                        {
                            switch (progress.Status)
                            {
                                case DownloadStatus.Downloading:
                                    {
                                        UpateStatus("bytes" + progress.BytesDownloaded, false);
                                        break;
                                    }
                                case DownloadStatus.Completed:
                                    {
                                        data = stream.ToArray();
                                        UpateStatus("Download complete", true);
                                        break;
                                    }
                                case DownloadStatus.Failed:
                                    {
                                        UpateStatus("Download failed 1: " + progress.Exception.Message, true);
                                        break;
                                    }
                            }
                        };



                    request.Alt = Google.Apis.Drive.v3.DriveBaseServiceRequest<Google.Apis.Drive.v3.Data.File>.AltEnum.Json;
                    request.Download(stream);

                }
            }
            catch (Exception err)
            {
                UpateStatus("Download failed 2: " + err.Message, true);
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FDStatus s = (FDStatus)e.UserState;
            context.Response.Write(s.status + "<br>");
            if (s.done == true)
                threadDone = true;
        }
    }

    public class FDStatus
    {
        public FDStatus(string status, bool done)
        {
            this.status = status;
            this.done = done;
        }
        public string status;
        public bool done;
    }

}