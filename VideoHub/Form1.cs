using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using CefSharp;
using CefSharp.WinForms;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security;
using AutoUpdaterDotNET;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using FluentFTP;
using FluentFTP.Helpers;
using System.Drawing.Printing;

namespace VideoHub
{
    public partial class Form1 : System.Windows.Forms.Form
    {

        static System.Windows.Forms.Form splashscreen = new Splash();
      
        const string passwordMessage = "-Password unchanged-";

        static Image FTPScanImage;

        const int NumRetries = 1;

        VideoCapture capture;
        Mat frame;
        Mat flippedFrame;
        Mat transposedFrame;
        Bitmap image;
        private Thread camera;
        bool isCameraRunning = false;
        Bitmap blankimage = new Bitmap(1, 1);

        public ChromiumWebBrowser chromeBrowser;

        public ChromiumWebBrowser popupBrowser;

        bool isLoggedIn = false;

        bool hubConnectedtoSP = false;

        bool isKnocking = false;

        static string LoginURL = ConfigurationManager.AppSettings["LoginURL"];

        static string[] siteUrl = ConfigurationManager.AppSettings["siteURL"].Split(',');
        static string[] userName = ConfigurationManager.AppSettings["userName"].Split(',');
        static string[] relativeURL = ConfigurationManager.AppSettings["relativeURL"].Split(',');

        static int numOrgs = siteUrl.Count();
        static int currentOrg = 0;

        static string[] secpass = ConfigurationManager.AppSettings["password"].Split(',');
        static List<string> password = new List<string>();


        static string HTTPProxyString = ConfigurationManager.AppSettings["HTTPProxyString"];
        static string FTPProxyString = ConfigurationManager.AppSettings["FTPProxyString"];

        static int FTPPort= System.Convert.ToInt32(ConfigurationManager.AppSettings["FTPPort"]);

        static string HubID = ConfigurationManager.AppSettings["HubID"];
        static string hubName = "";
        static string SnapsFolder = ConfigurationManager.AppSettings["SnapsFolder"];
        static string CommandsFolder = ConfigurationManager.AppSettings["CommandsFolder"];
        static string[] MeetingURL = new string[numOrgs]; //ConfigurationManager.AppSettings["MeetingURL"];
        static string[] PublicMeetingURL = new string[numOrgs];

        static int KnockingTimeOut = System.Convert.ToInt32(ConfigurationManager.AppSettings["KnockingTimeOut"]);
        static int KnockingTimer = 0;

        static int cameraindex = System.Convert.ToInt32(ConfigurationManager.AppSettings["cameraindex"]);
        static bool switchmaincamera = System.Convert.ToBoolean(ConfigurationManager.AppSettings["switchmaincamera"]);
        static bool isAdvisor = System.Convert.ToBoolean(ConfigurationManager.AppSettings["isAdvisor"]);
        
        static string autoUpdateXML = ConfigurationManager.AppSettings["AutoUpdateURL"];
        static string autoUpdateUsername = ConfigurationManager.AppSettings["AutoUpdateUsername"];

        static int ColourBusy = System.Convert.ToInt32(ConfigurationManager.AppSettings["ColourBusy"]);
        static int ColourKnocking = System.Convert.ToInt32(ConfigurationManager.AppSettings["ColourKnocking"]);
        static int ColourReady = System.Convert.ToInt32(ConfigurationManager.AppSettings["ColourReady"]);

        int runprocess = 0;


        private static bool SaveRunningConfig()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            List<string> encpass = new List<string>();

            settings["LoginURL"].Value = LoginURL;
            settings["siteURL"].Value = String.Join(",", siteUrl);
            settings["userName"].Value = String.Join(",", userName);
            settings["relativeURL"].Value = String.Join(",", relativeURL);
            settings["HubID"].Value = HubID;
            settings["SnapsFolder"].Value = SnapsFolder;
            settings["CommandsFolder"].Value = CommandsFolder;
            settings["cameraindex"].Value = System.Convert.ToString(cameraindex);
            settings["switchmaincamera"].Value = System.Convert.ToString(switchmaincamera);
            settings["isAdvisor"].Value = System.Convert.ToString(isAdvisor);

            for (int loop = 0; loop < password.Count; loop++)
                encpass.Add(DataProtection.EncryptString(DataProtection.ToSecureString(password[loop])));

            settings["password"].Value = String.Join(",", encpass);

            if (settings["ColourBusy"] == null)
                settings.Add("ColourBusy", System.Convert.ToString(ColourBusy));
            else
                settings["ColourBusy"].Value = System.Convert.ToString(ColourBusy);

            if (settings["ColourKnocking"] == null)
                settings.Add("ColourKnocking", System.Convert.ToString(ColourKnocking));
            else
                settings["ColourKnocking"].Value = System.Convert.ToString(ColourKnocking);

            if (settings["ColourReady"] == null)
                settings.Add("ColourReady", System.Convert.ToString(ColourReady));
            else
                settings["ColourReady"].Value = System.Convert.ToString(ColourReady);

            configFile.Save(ConfigurationSaveMode.Modified);

            return true;
        }

        private void FTPUploadfiles(int org, string filename, string filefolder)
        {
            try
            {
                using (FtpClient client = new FtpClient(siteUrl[org], FTPPort, userName[org], password[org]))
                {
                    if (FTPProxyString.Length > 4)
                    {
                        client.Host = FTPProxyString;
                        client.Credentials = new NetworkCredential(userName[org] + "@" + siteUrl[org], password[org]);
                        client.EncryptionMode = FtpEncryptionMode.None;
                    }
                    else
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.ValidateAnyCertificate = true;
                    }

                    client.Connect();

                    client.UploadFile(filefolder + filename, relativeURL[org] + "/" + HubID + "/" + filename);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem uploading file to FTP: " + ex.Message);
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem uploading file to FTP: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
            }
        }

        private void Uploadfiles(int org, string filename, string filefolder)
        {
            if (password[org].Length < 6)
            {
                System.Console.WriteLine("Problem uploading file to Sharepoint: bad password");
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem uploading file to Sharepoint: bad password";
            }
            else if (!siteUrl[org].ToLower().StartsWith("http"))
                    FTPUploadfiles(org, filename, filefolder);
            else
                try
                {
                    OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                    using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl[org], userName[org], password[org]))
                    {
                        Web web = ctx.Web;
                        ctx.Load(web);
                        ctx.Load(web.Lists);
                        ctx.ExecuteQueryRetry(NumRetries);

                        Folder folder = web.GetFolderByServerRelativeUrl(relativeURL[org]).EnsureFolder(HubID);
                        ctx.Load(folder);
                        ctx.ExecuteQueryRetry(NumRetries);

                        Folder folderToUpload = web.GetFolderByServerRelativeUrl(folder.ServerRelativeUrl);

                        folderToUpload.UploadFile(filename, filefolder + filename, true);

                        folderToUpload.Update();
                        ctx.Load(folder);
                        ctx.ExecuteQueryRetry(NumRetries);
                        folderToUpload.EnsureProperty(f => f.ServerRelativeUrl);

                        authMgr.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Problem uploading file to Sharepoint: " + ex.Message);
                    System.Console.ReadLine();
                    if (txtError.Visible) txtError.Text += "Problem uploading file to Sharepoint: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                }
        }

        private void GetFTPImageFile(int org, string filepath)
        {
            try
            {
                using (FtpClient client = new FtpClient(siteUrl[org], FTPPort, userName[org], password[org]))
                {
                    if (FTPProxyString.Length > 4)
                    {
                        client.Host = FTPProxyString;
                        client.Credentials = new NetworkCredential(userName[org] + "@" + siteUrl[org], password[org]);
                        client.EncryptionMode = FtpEncryptionMode.None;
                    }
                    else
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.ValidateAnyCertificate = true;
                    }
                    client.Connect();

                    long filesize = client.GetFileSize(filepath);

                    if (txtError.Visible)
                    {
                        txtError.Text += "File path: " + filepath + "\r\n";
                        txtError.Text += "File length: " + filesize + "\r\n";
                    }

                    if (filesize > 100 && filesize < 10000000)
                        using (MemoryStream rawFTPfile = new MemoryStream())
                        {
                            if (client.Download(rawFTPfile, filepath))
                                txtError.Text += "rawFTPfile length: " + rawFTPfile.Length + "\r\n";
                            else
                                txtError.Text += "error reading rawFTPfile: " + "\r\n";

                            rawFTPfile.Position = 0;

                            FTPScanImage = Image.FromStream(rawFTPfile);

                            if(FTPScanImage==null)
                                if (txtError.Visible) txtError.Text += "Problem getting image from FTP: - converting image from stream.";

                            client.Dispose();
                        }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem getting image from FTP: " + ex.Message);
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem getting image from FTP: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
            }
        }

        private string ReadFTPFile(int org, string filename, int firstline, int numlines)
        {
            try
            {
                using (FtpClient client = new FtpClient(siteUrl[org], FTPPort, userName[org], password[org]))
                {
                    if (FTPProxyString.Length > 4)
                    {
                        client.Host = FTPProxyString;
                        client.Credentials = new NetworkCredential(userName[org] + "@" + siteUrl[org], password[org]);
                        client.EncryptionMode = FtpEncryptionMode.None;
                    }
                    else
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.ValidateAnyCertificate = true;
                    }
                    client.Connect();

                    string filepath = relativeURL[org] + HubID + "/" + filename + ".txt";
                    string fileline;
                    string filetext="";

                    long filesize = client.GetFileSize(filepath);

                    if (txtError.Visible)
                    {
                        txtError.Text += "File path: " + filepath + "\r\n";
                        txtError.Text += "File length: " + filesize + "\r\n";
                    }

                    if (filesize > 10 && filesize < 4096)
                        using (MemoryStream rawFTPfile = new MemoryStream())
                        {
                            if (client.Download(rawFTPfile, filepath))
                                txtError.Text += "rawFTPfile length: " + rawFTPfile.Length + "\r\n";
                            else
                                txtError.Text += "error reading rawFTPfile: " + "\r\n";

                            rawFTPfile.Position = 0;

                            using (System.IO.StreamReader sr = new System.IO.StreamReader(rawFTPfile))
                            {
                                
                                for (int lineloop = 0; lineloop < firstline; lineloop++)      // discard unwanted lines at start of file.
                                {
                                    fileline = sr.ReadLine();

                                    if (txtError.Visible) txtError.Text += "File Line: " + fileline + "\r\n";
                                }

                                for (int lineloop = firstline; lineloop < firstline + numlines; lineloop++)
                                {
                                    fileline = sr.ReadLine();
                                    if (txtError.Visible) txtError.Text += "File Line: " + fileline + "\r\n";

                                    filetext += fileline;
                                }

                                if (txtError.Visible) txtError.Text += "File Contents: " + filetext + "\r\n";
                            }
                            client.Dispose();
                            return filetext;
                        }
                    else
                        return "";
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem checking FTP: " + ex.Message);
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem checking FTP: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                return "";
            }
        }

        private string ReadSharepointFile(int org, string filename, int firstline = 0, int numlines = 1)
        {
            if (password[org].Length < 6)
            {
                System.Console.WriteLine("Problem checking Sharepoint: bad password");
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: bad password";
                return "";
            }
            else if (!siteUrl[org].ToLower().StartsWith("http"))
                return ReadFTPFile(org, filename, firstline, numlines);
            else
                try
                {
                    OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                    using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl[org], userName[org], password[org]))
                    {
                        Web web = ctx.Web;
                        Microsoft.SharePoint.Client.File file;
                        string filepath = relativeURL[org] + HubID + "/" + filename + ".txt";
                        string fileline;
                        string filetext;

                        file = web.GetFileByServerRelativeUrl(filepath);
                        ctx.Load(file);
                        ctx.ExecuteQuery();             // Triggers exception if file not found

                        FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, filepath);

                        using (System.IO.StreamReader sr = new System.IO.StreamReader(fileInformation.Stream))
                        {
                            filetext = "";

                            for (int lineloop=0; lineloop < firstline; lineloop++)      // discard unwanted lines at start of file.
                                fileline = sr.ReadLine();

                            for (int lineloop = firstline; lineloop < firstline + numlines; lineloop++)
                            {
                                fileline = sr.ReadLine();
                                filetext += fileline;
                            }

                            if (txtError.Visible) txtError.Text += "File Contents: " + filetext;
                        }

                        authMgr.Dispose();
                        return filetext;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Problem checking Sharepoint: " + ex.Message);
                    System.Console.ReadLine();
                    if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: " + ex.Message + "-" + userName[org]+ "@" + siteUrl[org];
                    return "";
                }
        }

        private bool CheckFTPRequest(int org, string reqType, bool deleteReq, string hub)
        {
            try
            {
                using (FtpClient client = new FtpClient(siteUrl[org], FTPPort, userName[org], password[org]))
                {
                    if (FTPProxyString.Length > 4)
                    {
                        client.Host = FTPProxyString;
                        client.Credentials = new NetworkCredential(userName[org] + "@" + siteUrl[org], password[org]);
                        client.EncryptionMode = FtpEncryptionMode.None;
                    }
                    else
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.ValidateAnyCertificate = true;
                    }
                    client.Connect();
 
                    string filepath = relativeURL[org] + hub + "/" + reqType + ".txt";
                    bool exists;

                    exists = client.FileExists(filepath);
                    if (exists && deleteReq)
                        client.DeleteFile(filepath);

                    client.Dispose();
                    return exists;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem checking FTP: " + ex.Message);
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem checking FTP: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                return false;
            }

        }

        private bool CheckSharepointRequest(int org, string reqType, bool deleteReq = true, string hub = null)
        {
            if (hub == null)
                hub = HubID;
            if (password[org].Length < 6)
            {
                System.Console.WriteLine("Problem checking Sharepoint: bad password");
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: bad password";
                return false;
            }
            else if (!siteUrl[org].ToLower().StartsWith("http"))
                return CheckFTPRequest(org, reqType, deleteReq, hub);
            else
                try
                {
                    OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                    using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl[org], userName[org], password[org]))
                    {
                        Web web = ctx.Web;
                        Microsoft.SharePoint.Client.File file;

                        file = web.GetFileByServerRelativeUrl(relativeURL[org] + hub + "/" + reqType + ".txt");
                        ctx.Load(file);
                        ctx.ExecuteQuery();             // Triggers exception if file not found

                        if (deleteReq)
                        {
                            file.DeleteObject();        // File has been found - delete the file if requested.
                            ctx.ExecuteQuery();
                        }

                        authMgr.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Problem checking Sharepoint: " + ex.Message);
                    System.Console.ReadLine();
                    if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                    return false;
                }
        }

        private void DeleteFTPFile(int org, string delFile)
        {
            try
            {
                using (FtpClient client = new FtpClient(siteUrl[org], FTPPort, userName[org], password[org]))
                {
                    if (FTPProxyString.Length > 4)
                    {
                        client.Host = FTPProxyString;
                        client.Credentials = new NetworkCredential(userName[org] + "@" + siteUrl[org], password[org]);
                        client.EncryptionMode = FtpEncryptionMode.None;
                    }
                    else
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.ValidateAnyCertificate = true;
                    }

                    client.Connect();

                    string filepath = relativeURL[org] + HubID + "/" + delFile;

                    client.DeleteFile(filepath);

                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem deleting from FTP: " + ex.Message);
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem deleting from FTP: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
            }
        }


        private void DeleteSharepointFile(int org, string delFile)
        {
            if (password[org].Length < 6)
            {
                System.Console.WriteLine("Problem checking Sharepoint: bad password");
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: bad password";
                //return false;
            }
            else if (!siteUrl[org].ToLower().StartsWith("http"))
                DeleteFTPFile(org, delFile);
            else
                try
                {
                    OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                    using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl[org], userName[org], password[org]))
                    {
                        Web web = ctx.Web;
                        Microsoft.SharePoint.Client.File file;

                        file = web.GetFileByServerRelativeUrl(relativeURL[org] + HubID + "/" + delFile);
                        ctx.Load(file);
                        ctx.ExecuteQuery();             // Triggers exception if file not found

                        file.DeleteObject();
                        ctx.ExecuteQuery();

                        authMgr.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Problem deleting Sharepoint file: " + ex.Message);
                    System.Console.ReadLine();
                    if (txtError.Visible) txtError.Text += "Problem deleting Sharepoint file: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                }
        }

        private bool GetFTPImageList(int org)
        {
            try
            {
                using (FtpClient client = new FtpClient(siteUrl[org], FTPPort, userName[org], password[org]))
                {
                    if (FTPProxyString.Length > 4)
                    {
                        client.Host = FTPProxyString;
                        client.Credentials = new NetworkCredential(userName[org] + "@" + siteUrl[org], password[org]);
                        client.EncryptionMode = FtpEncryptionMode.None;
                    }
                    else
                    {
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        client.ValidateAnyCertificate = true;
                    }

                    client.Connect();

                    string filepath= relativeURL[org] + HubID;

                    FtpListItem[] filelist;

                    filelist = client.GetListing(filepath);

                    if (txtError.Visible) txtError.Text += filelist.Length;

                    foreach (FtpListItem f in filelist)
                        if (f.Name.ToLower().EndsWith(".png"))
                            lstSPImages.Items.Insert(0, f.Name);

                    if (lstSPImages.Items.Count > 0)
                    {
                        lstSPImages.Left = this.Width - 245;
                        lstSPImages.Top = this.Height - 350;
                        lstSPImages.Visible = true;
                    }

                    client.Dispose();

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem uploading file to FTP: " + ex.Message);
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem uploading file to FTP: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                return false;
            }
        }

        private bool GetSPImageList(int org)
        {
            lstSPImages.Items.Clear();
            lstSPImages.Visible = false;
            if (password[org].Length < 6)
            {
                System.Console.WriteLine("Problem checking Sharepoint: bad password");
                System.Console.ReadLine();
                if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: bad password";
                return false;
            }
            else if (!siteUrl[org].ToLower().StartsWith("http"))
                return GetFTPImageList(org);
            else
                try
                {
                    OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                    using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl[org], userName[org], password[org]))
                    {
                        Web web = ctx.Web;
                        ctx.Load(web);
                        ctx.Load(web.Lists);
                        ctx.ExecuteQueryRetry(NumRetries);

                        Microsoft.SharePoint.Client.Folder folder;

                        folder = web.GetFolderByServerRelativeUrl(relativeURL[org] + HubID);
                        ctx.Load(folder.Files);
                        ctx.ExecuteQuery();
                        FileCollection filecol = folder.Files;

                        foreach (Microsoft.SharePoint.Client.File f in filecol)
                            if (f.Name.EndsWith(".png"))
                                lstSPImages.Items.Insert(0, f.Name);


                        if (lstSPImages.Items.Count > 0)
                        {
                            lstSPImages.Left = this.Width - 245;
                            lstSPImages.Top = this.Height - 350;
                            lstSPImages.Visible = true;
                        }

                        authMgr.Dispose();
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Problem checking Sharepoint: " + ex.Message);
                    System.Console.ReadLine();
                    if (txtError.Visible) txtError.Text += "Problem checking Sharepoint: " + ex.Message + "-" + userName[org] + "@" + siteUrl[org];
                    return false;
                }
        }

        private void LoginWebsite(int org, bool isPublic = false)
        {
            pictureBoxGreeting.Hide();

            if(txtError.Visible)
            {
                txtError.Text += "Public URL: " + PublicMeetingURL[org]+"\r\n";
                txtError.Text += "Private URL: " + MeetingURL[org]+"\r\n";

                if (isPublic)
                    txtError.Text += "Public meeting initiated.\r\n";
                else
                    txtError.Text += "Private meeting initiated.\r\n";
            }

            if (isPublic)
                chromeBrowser.Load(PublicMeetingURL[org]);
            else
                chromeBrowser.Load(MeetingURL[org]);

            chromeBrowser.Show();

            for(int orgloop=0;orgloop<numOrgs;orgloop++)
                if(!isKnocking || orgloop!=org)
                    CheckSharepointRequest(orgloop,"HubOnline");

            
            if (switchmaincamera)
            {
                if (runprocess == 0)
                    runprocess = 100;     //trigger async process to change the camera
            }
            else
            {
                if (runprocess == 0)
                    runprocess = 110;     //trigger async process to delay slightly then login
            }
        }


        private void LogOutWebsite(int org)
        {
            chromeBrowser.Load(MeetingURL[org]);
            runprocess = 14;                        //delay before sending enter in case a pop-up question is displayed

            SendKeys.Send("{ENTER}");               //try clearing the pop-up anyway - probably fails
        }

        private void CaptureCamera()
        {
            try
            {
                camera = new Thread(new ThreadStart(CaptureCameraCallback));
                camera.Start();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem capturing document camera: " + ex.Message);
                if (txtError.Visible) txtError.Text += "Problem capturing document camera : " + ex.Message;
                System.Console.ReadLine();
            }
        }

        private void CaptureCameraCallback()
        {
            frame = new Mat();
            try
            {
                capture = new VideoCapture(cameraindex);
                capture.Open(cameraindex);
                capture.FrameHeight = 1080;
                capture.FrameWidth = 1920;
            

                while (isCameraRunning && capture.IsOpened())
                {
                    try
                    {
                        capture.Read(frame);
                        transposedFrame = frame.Transpose();
                        flippedFrame = transposedFrame.Flip(FlipMode.Y);

                        image = BitmapConverter.ToBitmap(flippedFrame);
                        if (ScannerPreview.Image != null)
                        {
                            ScannerPreview.Image.Dispose();
                        }
                        ScannerPreview.Image = image;
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Problem setting image to camera feed: " + ex.Message);
                        if (txtError.Visible) txtError.Text += "Problem setting image to camera feed : " + ex.Message;
                        System.Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem starting camera: " + ex.Message);
                if (txtError.Visible) txtError.Text += "Problem starting camera: " + ex.Message;
                System.Console.ReadLine();
            }
        }

        public Form1()
        {
            splashscreen.Show();

            InitializeComponent();

            if (FTPProxyString == null)
                FTPProxyString = "";

            if (FTPPort == 0)
                FTPPort = 21;

            this.WindowState = FormWindowState.Maximized;

            //Uncomment these lines to create FTP logs in c:\temp\ - folder must exist.
            //FtpTrace.LogToFile = @"c:\temp\ftplog.txt";
            //FtpTrace.LogUserName = true;   // show FTP user names
            //FtpTrace.LogPassword = false;   // hide FTP passwords
            //FtpTrace.LogIP = false; 	// hide FTP server IP addresses

            if (isAdvisor)                                  //Adjust menus for Advisor vs default Hub layout
            {
                resetHubToolStripMenuItem.Enabled = true;
                advisorToolStripMenuItem.Checked = true;
                hubToolStripMenuItem.Checked = false;
            }

            if(ColourBusy==0)
                ColourBusy= SystemColors.ControlDark.ToArgb();
            if (ColourKnocking == 0)
                ColourKnocking = System.Drawing.Color.FromName("LightSalmon").ToArgb(); 
            if (ColourReady == 0)
                ColourReady = System.Drawing.Color.FromName("LightBlue").ToArgb();


            ScannerPreview.Visible = false;

            panelFTPImage.Visible = false;

            SettingsPanel.Visible = false;

            SPStatus.Visible = false;

            // Populate passwords array with valid secure passwords, or "blank" if password doesn't decrypt
            for (int loop = 0; loop < secpass.Length; loop++)
            {
                if (secpass[loop].Length > 20)
                {
                    string decpass = DataProtection.ToInsecureString(DataProtection.DecryptString(secpass[loop]));
                    if (decpass.Length > 5)
                        password.Add(decpass);
                    else
                        password.Add("blank");
                }
                else
                    password.Add("blank");
            }

            // Pad passwords array with "blank" passwords so password count matches Org count.
            for (int loop = numOrgs-password.Count; loop > 0; loop--)
                password.Add("blank");

            for (int org = 0; org <numOrgs; org++)
            {
                if(password[org].Length>5)          // Test Sharepoint credentials for all Orgs with valid-looking passwords
                {
                    Uploadfiles(org, "SPTest.txt", CommandsFolder);
                    if (CheckSharepointRequest(org, "SPTest", false))
                    {
                        MeetingURL[org] = ReadSharepointFile(org, HubID + "Vectera");               
                        PublicMeetingURL[org] = ReadSharepointFile(org, HubID + "Vectera", 1);
                    }
                    else
                        SPStatus.Visible = true;
                }
                else
                    SPStatus.Visible = true;
            }

            currentOrg = 0;
            populateHubChooserMenu();

            if (isAdvisor)
            {
                hubToolStripMenuItem.Enabled = false;

                if (CheckSharepointRequest(currentOrg, "HubOnline", false) || CheckSharepointRequest(currentOrg, "HubKnocking", false))
                {
                    loginHubToolStripMenuItem.Enabled = true;
                }
                else
                {
                    loginHubToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                advisorLoginToolStripMenuItem.Enabled = false;
                advisorToolStripMenuItem.Enabled = false;
            }

            InitializeChromium(currentOrg);

            splashscreen.Hide();
        }


        private void InitializeChromium(int org)
        {
            CefSettings settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            settings.CefCommandLineArgs.Add("enable-media-stream");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            if (HTTPProxyString != null)
            {
                if (HTTPProxyString.ToLower().StartsWith("auto"))
                    settings.CefCommandLineArgs.Add("proxy-auto-detect");
                else if (HTTPProxyString.Length > 8)
                    settings.CefCommandLineArgs.Add("proxy-server", HTTPProxyString);
            }

            // Initialize cef with the provided settings
            Cef.Initialize(settings);

            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser(MeetingURL[org]);
            chromeBrowser.DownloadHandler = new CefSharp.Example.Handlers.DownloadHandler();

            // Add it to the form and fill it to the form window.
            this.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;

            popupBrowser = new ChromiumWebBrowser("");
            this.Controls.Add(popupBrowser);
            popupBrowser.Dock = DockStyle.None;

            popupBrowser.SendToBack();

            if (isAdvisor)
                GetSPImageList(org);
            else
            {
                chromeBrowser.Hide();
                popupBrowser.Hide();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            // Pause timer
            timer1.Stop();

            // Move scanner window to bottom-right edge of screen in case of resized Window
            if (!isAdvisor)
            {
                ScannerPreview.Left = this.Width - ScannerPreview.Width - 20;
                ScannerPreview.Top = this.Height - ScannerPreview.Height - 20;

                if (isKnocking && KnockingTimer > 0)
                    if (--KnockingTimer < 1 && runprocess == 0)
                        LogOutWebsite(currentOrg);
            }
            else
            {
                if (lstSPImages.Visible)
                {
                    lstSPImages.Left = this.Width - 245;
                    lstSPImages.Top = this.Height - 350;
                }

                menuStrip1.Left = this.Width / 2 - menuStrip1.Width / 2;
            }

            if (runprocess > 0)    // Used to run tasks asynchronously off main thread
            {
                switch (runprocess)
                {
                    case 100:

                        runprocess++;
                        break;
                    case 101:
                        runprocess++;
                        break;
                    case 102:

                        NativeMethods.SetCursorPos(190, 688);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        runprocess++;
                        break;

                    case 103:
                        //NativeMethods.SetCursorPos(190, 616);
                        NativeMethods.SetCursorPos(190, 638);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        runprocess++;
                        break;

                    case 104:
                        //NativeMethods.SetCursorPos(190, 690);
                        NativeMethods.SetCursorPos(190, 710);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        runprocess++;
                        break;

                    case 105:
                        //NativeMethods.SetCursorPos(190, 862);
                        NativeMethods.SetCursorPos(190, 885);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        switchmaincamera = false;
                        runprocess = 0;
                        if (!isKnocking)
                            isLoggedIn = true;
                        break;

                    case 110:

                        runprocess++;
                        break;
                    case 111:
                        runprocess++;
                        break;
                    case 112:
                        NativeMethods.SetCursorPos(100, 725);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                        runprocess = 0;
                        if (!isKnocking)
                            isLoggedIn = true;
                        break;

                    case 5:                     // Do scan from document camera

                        runprocess = 0;
                        try
                        {
                            capture.Release();
                            runprocess = 16;
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error pausing document camera : " + ex.Message);
                            if (txtError.Visible) txtError.Text += "Error pausing document camera : " + ex.Message;
                            System.Console.ReadLine();
                        }

                        break;

                    case 6:
                        runprocess = 0;
                        GetSPImageList(currentOrg);
                        break;

                    case 7:
                        runprocess = 6;
                        GetSPImageList(currentOrg);
                        break;

                    case 8:
                        runprocess = 7;
                        GetSPImageList(currentOrg);
                        break;

                    case 9:
                        runprocess = 8;
                        GetSPImageList(currentOrg);
                        break;

                    case 10:
                        runprocess = 9;
                        GetSPImageList(currentOrg);
                        break;

                    case 11:

                        isCameraRunning = false;
                        runprocess = 12;

                        break;

                    case 12:
                        runprocess = 0;

                        try
                        {
                            capture.Release();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error stopping document camera : " + ex.Message);
                            if (txtError.Visible) txtError.Text += "Error stopping document camera : " + ex.Message;
                            System.Console.ReadLine();
                        }

                        ScannerPreview.Visible = false;
                        break;

                    case 14:
                        if (isCameraRunning)
                            runprocess = 11;
                        else
                            runprocess = 0;

                        SendKeys.Send("{ENTER}");
                        isLoggedIn = false;
                        isKnocking = false;
                        chromeBrowser.Hide();
                        pictureBoxGreeting.Show();
                        pictureBoxGreeting.Focus();

                        for (int org = 0; org < numOrgs; org++)
                            Uploadfiles(org, "HubOnline.txt", CommandsFolder);

                        break;

                    case 16:
                        runprocess = 0;

                        String folder = SnapsFolder;
                        String filename = string.Format(@"{0}-{1}.png", HubID, DateTime.Now.ToString("yyyy-MM-dd---HH-mm-ss-fff"));

                        try
                        {
                            Bitmap snapshot = new Bitmap(ScannerPreview.Image);
                            snapshot.Save(folder + filename, ImageFormat.Png);
                            snapshot.Dispose();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Issue with creating document snapshot file: " + ex.Message);
                            System.Console.ReadLine();
                        }

                        Uploadfiles(currentOrg, filename, folder);

                        try
                        {
                            System.IO.File.Delete(folder + filename);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Issue with deleting document snapshot file: " + ex.Message);
                            System.Console.ReadLine();
                        }

                        try
                        {
                            CaptureCamera();
                            isCameraRunning = true;
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Issue restarting camera: " + ex.Message);
                            System.Console.ReadLine();
                        }

                        break;

                    case 20:
                        runprocess = 0;

                        try
                        {
                            PrintDocument printDoc = new PrintDocument();
                            printDoc.DocumentName = "VideoHub Scan";
                            printDoc.PrintPage += FTPScanImage_Print;
                            printDialog1.Document = printDoc;

                            if (printDialog1.ShowDialog() == DialogResult.OK)
                                printDoc.Print();
                        }
                        catch(Exception ex)
                        {
                            if (txtError.Visible) txtError.Text += "Error printing : " + ex.Message;
                        }

                        closeImageToolStripMenuItem.Enabled = true;

                        break;

                    default:
                        runprocess = 0;     //In case of invalid runprocess value
                        break;
                }

            }
            else if (!hubConnectedtoSP && !isAdvisor)
            {
                for (int org = 0; org< numOrgs; org++)
                {
                    Uploadfiles(org, "HubOnline.txt", CommandsFolder);
                    hubConnectedtoSP = true;
                }
            }
            else if(isAdvisor)
            {

                if (popupBrowser.Visible)
                {
                    popupBrowser.Left = 100;
                    popupBrowser.Top = 100;
                    if (this.Width > 200)
                        popupBrowser.Width = this.Width - 200;
                    else
                        popupBrowser.Width = 100;

                    if (this.Height > 200)
                        popupBrowser.Height = this.Height - 200;
                    else
                        popupBrowser.Height = 100;
                }


            }
            else if (!isAdvisor)     // This codepath is for the hub to be remotely controlled via Sharepoint by the Advisor
            {
                if (CheckSharepointRequest(currentOrg,"Logout"))
                {
                    LogOutWebsite(currentOrg);
                }

                if (isLoggedIn && !isKnocking)      // Fully controlled, looking for scanner commands
                {
                    if (isCameraRunning)
                    {
                        try
                        {
                            if (ScannerPreview.Image != null)
                            {
                                if (CheckSharepointRequest(currentOrg, "Scan"))
                                {
                                    isCameraRunning = false;        //pause camera updates
                                    runprocess = 5;                 //trigger scan through run process
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Issue testing for scan command : " + ex.Message);
                            if (txtError.Visible) txtError.Text += "Issue testing for scan command : " + ex.Message;
                            System.Console.ReadLine();
                        }
                    }
/*                    else
                    {
                        if (camera != null)
                        {
                            if (!camera.IsAlive)
                            {
                                try
                                {
                                    ScannerPreview.Image = new Bitmap(1, 1);
                                }
                                catch (Exception ex)
                                {
                                    System.Console.WriteLine("Issue blanking camera : " + ex.Message);
                                    if (txtError.Visible) txtError.Text += "Issue blanking camera : " + ex.Message;
                                    System.Console.ReadLine();
                                }
                            }
                        }
                        System.GC.Collect();
                    }
*/
                    if (!isCameraRunning)
                    {
                        if (CheckSharepointRequest(currentOrg, "EnableScan"))
                        {
                            try
                            {
                                ScannerPreview.Image = new Bitmap(1, 1);
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Issue blanking camera : " + ex.Message);
                                if (txtError.Visible) txtError.Text += "Issue blanking camera : " + ex.Message;
                                System.Console.ReadLine();
                            }
                            ScannerPreview.Visible = true;
                            CaptureCamera();
                            isCameraRunning = true;
                        }
                    }
                    else if (CheckSharepointRequest(currentOrg, "DisableScan"))
                    { 
                         runprocess = 11;
                    }
                }
                else if(isKnocking && !isLoggedIn)               // Hub in meeting initated at Hub
                {
                    if (CheckSharepointRequest(currentOrg, "Login"))
                    {
                        isKnocking = false;
                        isLoggedIn = true;
                        CheckSharepointRequest(currentOrg, "HubOnline");
                        CheckSharepointRequest(currentOrg, "HubKnocking");
                    }
                }
                else                                            // Check all orgs for login command
                {
                    for (int org = 0; org < numOrgs; org++)
                    {
                        if (CheckSharepointRequest(org, "Login"))
                        {
                            currentOrg = org;

                            LoginWebsite(currentOrg);
                            break;
                        }
                    }
                }
            }
            // re-enable the timer.
            timer1.Start();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text="VideoHub - v."+System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            if(!isAdvisor)
            {
                pictureBoxGreeting.Image = Image.FromFile(@".\\defaultgreeting.png");
                pictureBoxGreeting.Top = (this.Height - pictureBoxGreeting.Height) / 2 + 10;
                pictureBoxGreeting.Left = (this.Width - pictureBoxGreeting.Width) / 2;
                pictureBoxGreeting.Show();
                pictureBoxGreeting.Focus();
            }
            else
            {
                pictureBoxGreeting.Hide();

                //Autoupdate for advisor only
                UpdateVideoHub();
            }
            timer1.Enabled = true;
        }

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!isLoggedIn && !isKnocking && !isAdvisor)
            {
                for (int orgloop = 0; orgloop < numOrgs; orgloop++)
                {
                    if (e.KeyCode == Keys.D0 + orgloop)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        currentOrg = orgloop;
                        isKnocking = true;
                        KnockingTimer = KnockingTimeOut*12;
                        Uploadfiles(currentOrg, "HubKnocking.txt", CommandsFolder);
                        LoginWebsite(currentOrg, true);
                        break;
                    }
                }
            }
        }


        private void advisorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isAdvisor)
            {
                loginHubToolStripMenuItem.Enabled = true;
                resetHubToolStripMenuItem.Enabled = true;
                isAdvisor = true;
                advisorToolStripMenuItem.Checked = true;
                hubToolStripMenuItem.Checked = false;
                // CheckSharepointRequest("HubOnline");            // Mark hub as offline
                hubConnectedtoSP = false;

                Uploadfiles(currentOrg,"SPTest.txt", CommandsFolder);
                if (CheckSharepointRequest(currentOrg, "SPTest",false))
                    SPStatus.Visible = false;
                else
                    SPStatus.Visible = true;
            }
            GetSPImageList(currentOrg);
            chromeBrowser.Load(MeetingURL[currentOrg]);
        }

        private void hubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAdvisor = false;
            advisorToolStripMenuItem.Checked = false;
            hubToolStripMenuItem.Checked = true;
            loginHubToolStripMenuItem.Enabled = false;
            logoutHubToolStripMenuItem.Enabled = false;
            resetHubToolStripMenuItem.Enabled = false;
            startHubDocumentCameraToolStripMenuItem.Enabled = false;
            stopHubDocumentCameraToolStripMenuItem.Enabled = false;
            scanHubDocumentCameraToolStripMenuItem.Enabled = false;
            chromeBrowser.Load(MeetingURL[currentOrg]);
        }

        private void advisorLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isAdvisor)
            {
                loginHubToolStripMenuItem.Enabled = true;
                resetHubToolStripMenuItem.Enabled = true;
                isAdvisor = true;
                switchmaincamera = false;
                advisorToolStripMenuItem.Checked = true;
                hubToolStripMenuItem.Checked = false;
                // CheckSharepointRequest("HubOnline");            // Mark hub as offline
                hubConnectedtoSP = false;

                Uploadfiles(currentOrg,"SPTest.txt", CommandsFolder);
                if (CheckSharepointRequest(currentOrg,"SPTest",false))
                    SPStatus.Visible = false;
                else
                    SPStatus.Visible = true;
                GetSPImageList(currentOrg);
            }
            chromeBrowser.Load(LoginURL);
        }

        private void loginHubToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (isAdvisor)
            {
                if (CheckSharepointRequest(currentOrg, "HubOnline", false))
                {
                    Uploadfiles(currentOrg, "Login.txt", CommandsFolder);
                    loginHubToolStripMenuItem.Enabled = false;
                    logoutHubToolStripMenuItem.Enabled = true;
                    startHubDocumentCameraToolStripMenuItem.Enabled = true;
                    chooseHubToolStripMenuItem.Enabled = false;
                }
                else
                {
                    chooseHubToolStripMenuItem.Enabled = true;
                    loginHubToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void logoutHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles(currentOrg,"Logout.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = true;
                logoutHubToolStripMenuItem.Enabled = false;
                startHubDocumentCameraToolStripMenuItem.Enabled = false;
                stopHubDocumentCameraToolStripMenuItem.Enabled = false;
                scanHubDocumentCameraToolStripMenuItem.Enabled = false;
                chooseHubToolStripMenuItem.Enabled = true;
            }
        }

        private void startHubDocumentCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles(currentOrg,"EnableScan.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = false;
                logoutHubToolStripMenuItem.Enabled = true;
                startHubDocumentCameraToolStripMenuItem.Enabled = false;
                stopHubDocumentCameraToolStripMenuItem.Enabled = true;
                scanHubDocumentCameraToolStripMenuItem.Enabled = true;
            }
        }

        private void stopHubDocumentCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles(currentOrg,"DisableScan.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = false;
                logoutHubToolStripMenuItem.Enabled = true;
                startHubDocumentCameraToolStripMenuItem.Enabled = true;
                stopHubDocumentCameraToolStripMenuItem.Enabled = false;
                scanHubDocumentCameraToolStripMenuItem.Enabled = false;
            }
        }

        private void scanHubDocumentCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles(currentOrg,"Scan.txt", CommandsFolder);
                runprocess = 10;
            }
        }

        private void UpdateVideoHub()
        {
            if (autoUpdateXML != "")                    //Autoupdate for advisor only
            {
                AutoUpdater.Synchronous = true;
                AutoUpdater.ReportErrors = false;
                AutoUpdater.ShowSkipButton = false;
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.ShowRemindLaterButton = false;
                AutoUpdater.UpdateFormSize = new System.Drawing.Size(400, 300);
          
                if(autoUpdateUsername!=null)
                {
                    AutoUpdater.Start(autoUpdateXML, new NetworkCredential(autoUpdateUsername,""));
                }
                else
                {
                    AutoUpdater.Start(autoUpdateXML);
                }
            }
            // checkUpdateToolStripMenuItem.Enabled = false;
        }

        private void checkUpdatetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateVideoHub();
        }

        private void resetHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles(currentOrg,"Logout.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = true;
                logoutHubToolStripMenuItem.Enabled = false;
                startHubDocumentCameraToolStripMenuItem.Enabled = false;
                stopHubDocumentCameraToolStripMenuItem.Enabled = false;
                scanHubDocumentCameraToolStripMenuItem.Enabled = false;
                loginHubToolStripMenuItem.Enabled = true;
                chromeBrowser.Show();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            populateSettings();
            numericUDOrg.Minimum = 1;
            if (numOrgs > 1)
            {
                numericUDOrg.Maximum = numOrgs;
                numericUDOrg.Enabled = true;
            }
            else
                numericUDOrg.Enabled = false;

            SettingsPanel.Visible = true;
            SettingsPanel.BringToFront();
            settingsToolStripMenuItem.Enabled = false;  
        }

        private void populateSettings()
        {
            textBoxMeetingURL.Text = PublicMeetingURL[currentOrg];
            textBoxDashboardURL.Text = LoginURL;
            textBoxHubID.Text = HubID;
            textBoxScansFolder.Text = SnapsFolder;
            textBoxCommandsFolder.Text = CommandsFolder;
            numericDocumentCameraIndex.Value = cameraindex;
            checkBoxSwapMainCamera.Checked = switchmaincamera;
            textBoxSharepointURL.Text = siteUrl[currentOrg];
            textBoxSharepointRelativeURL.Text = relativeURL[currentOrg];
            textBoxSharepointUsername.Text = userName[currentOrg];
            textBoxSharepointPassword.PasswordChar = '\0';
            textBoxSharepointPassword.Text = passwordMessage;
            btnBusyColour.BackColor= System.Drawing.Color.FromArgb(ColourBusy);
            btnKnockingColour.BackColor= System.Drawing.Color.FromArgb(ColourKnocking);
            btnReadyColour.BackColor = System.Drawing.Color.FromArgb(ColourReady);
        }

        private void btnSettingsSave_Click(object sender, EventArgs e)
        {

            btnSettingsSave.Enabled = false;
            btnSettingsCancel.Enabled = false;
            HubID = textBoxHubID.Text;
            cameraindex = (int)numericDocumentCameraIndex.Value;
            switchmaincamera = checkBoxSwapMainCamera.Checked;
            userName[currentOrg] = textBoxSharepointUsername.Text;
            if (!textBoxSharepointPassword.Text.Equals(passwordMessage)
               && textBoxSharepointPassword.Text.Length > 6)
            {                                                    //only update password if password has been modified
                if (currentOrg > password.Count - 1)
                    password.Add(textBoxSharepointPassword.Text);
                else
                    password[currentOrg] = textBoxSharepointPassword.Text;
            }

            SaveRunningConfig();

            Uploadfiles(currentOrg,"SPTest.txt", CommandsFolder);
            if (CheckSharepointRequest(currentOrg, "SPTest", false))
            {
                SPStatus.Visible = false;
                MeetingURL[currentOrg] = ReadSharepointFile(currentOrg, HubID + "Vectera");
                PublicMeetingURL[currentOrg] = ReadSharepointFile(currentOrg, HubID + "Vectera",1);
            }
            else
                SPStatus.Visible = true;

            if(isAdvisor)
            {
                GetSPImageList(currentOrg);
            }

            SettingsPanel.Visible = false;
            btnSettingsSave.Enabled = true;
            btnSettingsCancel.Enabled = true;
            settingsToolStripMenuItem.Enabled = true;
            populateHubChooserMenu();
        }

        private void btnSettingsCancel_Click(object sender, EventArgs e)
        {
            SettingsPanel.Visible = false;
            settingsToolStripMenuItem.Enabled = true;
            //populateHubChooserMenu();
        }

        private void textBoxSharepointPassword_Enter(object sender, EventArgs e)
        {
            textBoxSharepointPassword.PasswordChar = '*';
            textBoxSharepointPassword.Text = "";
        }

        private void textBoxSharepointPassword_Leave(object sender, EventArgs e)
        {
            if (textBoxSharepointPassword.Text.Length < 7)     //only update password if password has been modified
            {
                textBoxSharepointPassword.PasswordChar = '\0';
                textBoxSharepointPassword.Text = passwordMessage;
            }
        }

        private void lstSPImages_DoubleClick(object sender, EventArgs e)
        {
            string imageAddress;

            if (!siteUrl[currentOrg].ToLower().StartsWith("http"))
            {
                imageAddress = relativeURL[currentOrg] + HubID + "/" + lstSPImages.SelectedItem.ToString();
                GetFTPImageFile(currentOrg, imageAddress);
                //picFTPImage.SizeMode = PictureBoxSizeMode.AutoSize;
                picFTPImage.Image = FTPScanImage;
                picFTPImage.Width = FTPScanImage.Width/2;
                picFTPImage.Height = FTPScanImage.Height/2;

                panelFTPImage.Width = picFTPImage.Width+20;
                if (panelFTPImage.Width > this.Width - 100)
                    panelFTPImage.Width = this.Width - 100;

                panelFTPImage.Height = picFTPImage.Height+20;
                if (panelFTPImage.Height > this.Height - 50)
                    panelFTPImage.Height = this.Height - 50;

                panelFTPImage.Left = this.Left + this.Width / 2 - panelFTPImage.Width / 2;
                panelFTPImage.Top = this.Top + this.Height / 2 - panelFTPImage.Height / 2;
                panelFTPImage.Visible = true;
                closeImageToolStripMenuItem.Enabled = true;
            }
            else
            { 
                imageAddress = siteUrl[currentOrg].Remove(siteUrl[currentOrg].IndexOf("/sites/")) + relativeURL[currentOrg];
                popupBrowser.Load(imageAddress + "/" + HubID + "/" + lstSPImages.SelectedItem.ToString());
                popupBrowser.BringToFront();
                closeImageToolStripMenuItem.Enabled = true;
            }
        }

        private void picFTPImage_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && runprocess!=20)
            {
                if (picFTPImage.Width < FTPScanImage.Width)
                {
                    picFTPImage.Width = FTPScanImage.Width;
                    picFTPImage.Height = FTPScanImage.Height;
                }
                else if (picFTPImage.Width > FTPScanImage.Width)
                {
                    picFTPImage.Width = FTPScanImage.Width / 2;
                    picFTPImage.Height = FTPScanImage.Height / 2;
                }
                else
                {
                    picFTPImage.Width = FTPScanImage.Width * 2;
                    picFTPImage.Height = FTPScanImage.Height * 2;
                }

                panelFTPImage.Width = picFTPImage.Width+20;
                if (panelFTPImage.Width > this.Width - 100)
                    panelFTPImage.Width = this.Width - 100;

                panelFTPImage.Height = picFTPImage.Height+20;
                if (panelFTPImage.Height > this.Height - 50)
                    panelFTPImage.Height = this.Height - 50;

                panelFTPImage.Left = this.Left + this.Width / 2 - panelFTPImage.Width / 2;
                panelFTPImage.Top = this.Top + this.Height / 2 - panelFTPImage.Height / 2;
            }
            else if (e.Button == MouseButtons.Right && runprocess!=20)
            {
                closeImageToolStripMenuItem.Enabled = false;

                runprocess = 20;
            }
        }

        private void FTPScanImage_Print(object o, PrintPageEventArgs e)
        {
            float top, left, width, height;
            if (e.PageSettings.PrintableArea.Width / e.PageSettings.PrintableArea.Height > FTPScanImage.Width/FTPScanImage.Height)
            {
                height = e.PageSettings.PrintableArea.Height;
                width = FTPScanImage.Width * height / FTPScanImage.Height;
                top = 0;
                left = (e.PageSettings.PrintableArea.Width - width) / 2;
            }
            else
            {
                width = e.PageSettings.PrintableArea.Width;
                height = FTPScanImage.Height * width / FTPScanImage.Width;
                left = 0;
                top = (e.PageSettings.PrintableArea.Height - height) / 2;
            }

            e.Graphics.DrawImage(FTPScanImage, left, top, width, height);
        }

        private void closeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(panelFTPImage.Visible)
            {
                panelFTPImage.Visible = false;
            }
            else
            {
                popupBrowser.Load("");
                popupBrowser.SendToBack();
                closeImageToolStripMenuItem.Enabled = false;
            }
        }

        private void lstSPImages_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && lstSPImages.SelectedIndex!=-1)
            {
                DeleteSharepointFile(currentOrg,lstSPImages.SelectedItem.ToString());
                lstSPImages.Items.RemoveAt(lstSPImages.SelectedIndex);
                if (lstSPImages.Items.Count == 0)
                    lstSPImages.Visible = false;
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtError.Visible = !txtError.Visible;
            txtError.Text = "";
            txtError.Text += "isknocking:" + isKnocking;
            txtError.Text += " isLoggedIn:" + isLoggedIn;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            populateHubChooserMenu();
        }

        private void populateHubChooserMenu()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@".\HubList.txt");
            string hubid;
            string hubname;
            int menuitem;

            // clear menu and rebuild
            for (menuitem = chooseHubToolStripMenuItem.DropDownItems.Count - 1; menuitem > 0; menuitem--)
            {
                chooseHubToolStripMenuItem.DropDownItems[menuitem].Click -= new System.EventHandler(this.hubChoose);
                chooseHubToolStripMenuItem.DropDownItems[menuitem].Dispose();
            }
            chooseHubToolStripMenuItem.Text = "Choose Hub";

            while ((hubid = file.ReadLine()) != null)
            {
                if ((hubname = file.ReadLine()) == null)
                    hubname = "";
                if (!isAdvisor)
                {
                    if (hubid == HubID)
                        chooseHubToolStripMenuItem.Text = "** " + hubname + " - " + hubid;
                }
                else
                {
                    ToolStripItem subItem = new ToolStripMenuItem(hubname + " - " + hubid);
                    menuitem = chooseHubToolStripMenuItem.DropDownItems.Add(subItem);

                    chooseHubToolStripMenuItem.DropDownItems[menuitem].Name = hubid;
                    chooseHubToolStripMenuItem.DropDownItems[menuitem].Click += new System.EventHandler(this.hubChoose);

                    if (CheckSharepointRequest(currentOrg, "HubKnocking", false, hubid))
                    {
                        chooseHubToolStripMenuItem.DropDownItems[menuitem].Enabled = true;
                        chooseHubToolStripMenuItem.DropDownItems[menuitem].BackColor = System.Drawing.Color.FromArgb(ColourKnocking);

                        if (hubid == HubID)
                        {
                            chooseHubToolStripMenuItem.Text = "** " + hubname + " - " + hubid;
                            chooseHubToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(ColourKnocking);
                            loginHubToolStripMenuItem.Enabled = true;
                        }
                    }
                    else if (CheckSharepointRequest(currentOrg, "HubOnline", false, hubid))
                    {
                        chooseHubToolStripMenuItem.DropDownItems[menuitem].Enabled = true;
                        chooseHubToolStripMenuItem.DropDownItems[menuitem].BackColor = System.Drawing.Color.FromArgb(ColourReady);

                        if (hubid == HubID)
                        {
                            chooseHubToolStripMenuItem.Text = "** " + hubname + " - " + hubid;
                            chooseHubToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(ColourReady);
                            loginHubToolStripMenuItem.Enabled = true;
                        }
                    }
                    else
                    {
                        chooseHubToolStripMenuItem.DropDownItems[menuitem].Enabled = true;
                        chooseHubToolStripMenuItem.DropDownItems[menuitem].BackColor = System.Drawing.Color.FromArgb(ColourBusy);

                        if (hubid == HubID)
                        {
                            chooseHubToolStripMenuItem.Text = "** " + hubname + " - " + hubid;
                            chooseHubToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(ColourBusy);
                            loginHubToolStripMenuItem.Enabled = false;
                        }
                    }
                }
            }

            file.Close();
        }

        private void hubChoose(object sender, EventArgs e)
        {
            var menuclicked = sender as ToolStripMenuItem;
            if (menuclicked != null)
            {
                HubID = menuclicked.Name;
                hubName = menuclicked.Text;

                chooseHubToolStripMenuItem.Text = "** " + hubName;

                for (int org = 0; org < numOrgs; org++)
                {
                    MeetingURL[org] = ReadSharepointFile(org, HubID + "Vectera");
                    PublicMeetingURL[org] = ReadSharepointFile(org, HubID + "Vectera",1);
                }

                GetSPImageList(currentOrg);

                chromeBrowser.Load(MeetingURL[currentOrg]);

                if (CheckSharepointRequest(currentOrg, "HubKnocking", false))
                {
                    chooseHubToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(ColourKnocking);
                    loginHubToolStripMenuItem.Enabled = true;           //enable login to hub
                    chromeBrowser.Show();
                }
                else if (CheckSharepointRequest(currentOrg, "HubOnline", false))
                {
                    chooseHubToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(ColourReady);
                    loginHubToolStripMenuItem.Enabled = true;           //enable login to hub
                    chromeBrowser.Show();
                }
                else
                {
                    chooseHubToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(ColourBusy);
                    loginHubToolStripMenuItem.Enabled = false;           //disable login to hub
                    chromeBrowser.Hide();
                }
            }
        }

        private void numericUDOrg_ValueChanged(object sender, EventArgs e)
        {
            currentOrg = ((int)numericUDOrg.Value)-1;
            populateSettings();
            if (txtError.Visible)
                txtError.Text += password[currentOrg];
        }

        private void btnBusyColour_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnBusyColour.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnBusyColour.BackColor = colorDialog1.Color;
                ColourBusy = colorDialog1.Color.ToArgb();
            }
        }

        private void btnKnockingColour_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnKnockingColour.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnKnockingColour.BackColor = colorDialog1.Color;
                ColourKnocking = colorDialog1.Color.ToArgb();
            }
        }

        private void btnReadyColour_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnReadyColour.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnReadyColour.BackColor = colorDialog1.Color;
                ColourReady = colorDialog1.Color.ToArgb();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (lstSPImages.Visible)
            {
                lstSPImages.Left = this.Width - 245;
                lstSPImages.Top = this.Height - 350;
            }

            menuStrip1.Left = this.Width / 2 - menuStrip1.Width / 2;
        }
    }

    internal static class NativeMethods
    {
        // To Process Mouse Events
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        public const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        public const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
    }

    public class DataProtection
    {
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("upkHpBsACBX9AZUferekUu5gGUiyBMXxhTzjoKYJ");

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

    }
}
