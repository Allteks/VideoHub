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

namespace VideoHub
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        const string passwordMessage = "-Password unchanged-";

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

        static string LoginURL = ConfigurationManager.AppSettings["LoginURL"];
        static string siteUrl = ConfigurationManager.AppSettings["siteURL"];
        static string userName = ConfigurationManager.AppSettings["userName"];
        static string relativeURL = ConfigurationManager.AppSettings["relativeURL"];
        static string HubID = ConfigurationManager.AppSettings["HubID"];
        static string SnapsFolder = ConfigurationManager.AppSettings["SnapsFolder"];
        static string CommandsFolder = ConfigurationManager.AppSettings["CommandsFolder"];
        static string MeetingURL = ConfigurationManager.AppSettings["MeetingURL"];
        static int cameraindex = System.Convert.ToInt32(ConfigurationManager.AppSettings["cameraindex"]);
        static bool switchmaincamera = System.Convert.ToBoolean(ConfigurationManager.AppSettings["switchmaincamera"]);
        static bool isAdvisor = System.Convert.ToBoolean(ConfigurationManager.AppSettings["isAdvisor"]);
        static string password = DataProtection.ToInsecureString(DataProtection.DecryptString(ConfigurationManager.AppSettings["password"]));
        //static string docLibrary = ConfigurationManager.AppSettings["docLibrary"];


        int runprocess = 0;

        private static bool SaveRunningConfig()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings["LoginURL"].Value = LoginURL;
            settings["siteURL"].Value = siteUrl;
            settings["userName"].Value = userName;
            settings["relativeURL"].Value = relativeURL;
            settings["HubID"].Value = HubID;
            settings["SnapsFolder"].Value = SnapsFolder;
            settings["CommandsFolder"].Value = CommandsFolder;
            settings["MeetingURL"].Value = MeetingURL;
            settings["cameraindex"].Value = System.Convert.ToString(cameraindex);
            settings["switchmaincamera"].Value = System.Convert.ToString(switchmaincamera);
            settings["isAdvisor"].Value = System.Convert.ToString(isAdvisor);
            settings["password"].Value = DataProtection.EncryptString(DataProtection.ToSecureString(password));

            configFile.Save(ConfigurationSaveMode.Modified);

            return true;
        }

        private static void Uploadfiles(string filename, string filefolder)
        {
            try
            {
                OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))
                {
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.Load(web.Lists);
                    ctx.ExecuteQueryRetry();

                    Folder folder = web.GetFolderByServerRelativeUrl(relativeURL).EnsureFolder(HubID);
                    ctx.Load(folder);
                    ctx.ExecuteQueryRetry();

                    Folder folderToUpload = web.GetFolderByServerRelativeUrl(folder.ServerRelativeUrl);

                    folderToUpload.UploadFile(filename, filefolder + filename, true);

                    folderToUpload.Update();
                    ctx.Load(folder);
                    ctx.ExecuteQueryRetry();
                    folderToUpload.EnsureProperty(f => f.ServerRelativeUrl);

                    authMgr.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem uploading file to Sharepoint: " + ex.Message);
                System.Console.ReadLine();
            }
        }

        private static bool CheckSharepointRequest(string reqType, bool deleteReq = true)
        {
            try
            {
                OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))
                {
                    Web web = ctx.Web;
                    Microsoft.SharePoint.Client.File file;

                    file = web.GetFileByServerRelativeUrl(relativeURL + HubID + "/" + reqType + ".txt");
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
                return false;
            }
        }

        private static bool DeleteSharepointFile (string delFile)
        {
            try
            {
                OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();

                using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))
                {
                    Web web = ctx.Web;
                    Microsoft.SharePoint.Client.File file;

                    file = web.GetFileByServerRelativeUrl(relativeURL + HubID + "/" + delFile);
                    ctx.Load(file);
                    ctx.ExecuteQuery();             // Triggers exception if file not found

                    file.DeleteObject();        
                    ctx.ExecuteQuery();

                    authMgr.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem deleting Sharepoint file: " + ex.Message);
                System.Console.ReadLine();
                return false;
            }
        }

        private bool GetSPImageList()
        {
            try
            {
                OfficeDevPnP.Core.AuthenticationManager authMgr = new OfficeDevPnP.Core.AuthenticationManager();
                

                using (var ctx = authMgr.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, userName, password))
                {
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.Load(web.Lists);
                    ctx.ExecuteQueryRetry();

                    Microsoft.SharePoint.Client.Folder folder;

                    folder = web.GetFolderByServerRelativeUrl(relativeURL + HubID);
                    ctx.Load(folder.Files);
                    ctx.ExecuteQuery();
                    FileCollection filecol = folder.Files;

                    foreach (Microsoft.SharePoint.Client.File f in filecol)
                    {
                        if(f.Name.EndsWith(".png"))
                        {
                            if(!lstSPImages.Items.Contains(f.Name))
                                lstSPImages.Items.Insert(0, f.Name);
                        }
                              
                    }
                    if (lstSPImages.Items.Count > 0)
                    {
                        lstSPImages.Left = this.Width - 245;
                        lstSPImages.Top = this.Height - 350;
                        lstSPImages.Visible = true;
                    }
                    else
                        lstSPImages.Visible = false;

                    authMgr.Dispose();
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Problem checking Sharepoint: " + ex.Message);
                System.Console.ReadLine();
                return false;
            }

        }

        private void LoginWebsite()
        {
            if (switchmaincamera)
            {
                if (runprocess == 0)
                {
                    runprocess = 1;     //trigger async process to change the camera
                }
            }
            else
            {
                NativeMethods.SetCursorPos(100, 725);
                NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                isLoggedIn = true;

            }

        }


        private void LogOutWebsite()
        {
            chromeBrowser.Load(MeetingURL);
            System.Threading.Thread.Sleep(1000);    //wait 1 second and send Return
            SendKeys.Send("{ENTER}");
            isLoggedIn = false;
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
            capture = new VideoCapture(cameraindex);
            capture.Open(cameraindex);
            capture.FrameHeight = 1080;
            capture.FrameWidth = 1920;

 //           if (capture.IsOpened())
 //           {
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
//            }
        }

        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            if (isAdvisor)                                  //Adjust menus for Advisor vs default Hub layout
            {
                loginHubToolStripMenuItem.Enabled = true;
                resetHubToolStripMenuItem.Enabled = true;
                advisorToolStripMenuItem.Checked = true;
                hubToolStripMenuItem.Checked = false;
            }

            ScannerPreview.Visible = false;

            SettingsPanel.Visible = false;

            InitializeChromium();

            if(isAdvisor)
            {
                Uploadfiles("SPTest.txt", CommandsFolder);
                if (CheckSharepointRequest("SPTest"))
                    SPStatus.Visible = false;
                else
                    SPStatus.Visible = true;
            }
        }


        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            settings.CefCommandLineArgs.Add("enable-media-stream");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");

            // Initialize cef with the provided settings
            Cef.Initialize(settings);

            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser(MeetingURL);


            // Add it to the form and fill it to the form window.
            this.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;

            popupBrowser = new ChromiumWebBrowser("");
            this.Controls.Add(popupBrowser);
            popupBrowser.Dock = DockStyle.None;

            popupBrowser.SendToBack();

            if (isAdvisor)
                GetSPImageList();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            // Move scanner window to bottom-right edge of screen in case of resized Window
            ScannerPreview.Left = this.Width - ScannerPreview.Width - 20;
            ScannerPreview.Top = this.Height - ScannerPreview.Height - 20;

            

            if (runprocess > 0)    // Used to run tasks asynchronously off main thread
            {
                switch (runprocess)
                {
                    case 1:

                        NativeMethods.SetCursorPos(190, 688);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        runprocess++;
                        break;

                    case 2:
                        //NativeMethods.SetCursorPos(190, 616);
                        NativeMethods.SetCursorPos(190, 638);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        runprocess++;
                        break;

                    case 3:
                        //NativeMethods.SetCursorPos(190, 690);
                        NativeMethods.SetCursorPos(190, 710);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        runprocess++;
                        break;

                    case 4:
                        //NativeMethods.SetCursorPos(190, 862);
                        NativeMethods.SetCursorPos(190, 885);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        switchmaincamera = false;
                        runprocess = 0;
                        isLoggedIn = true;
                        break;

                    case 5:                     // Do scan from document camera
                        runprocess = 0;
                        try
                        {
                            capture.Release();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error pausing document camera : " + ex.Message);
                            if (txtError.Visible) txtError.Text += "Error pausing document camera : " + ex.Message;
                            System.Console.ReadLine();
                        }

                        Bitmap snapshot = new Bitmap(ScannerPreview.Image);
                        String folder = SnapsFolder;
                        String filename = string.Format(@"{0}-{1}.png", HubID, DateTime.Now.ToString("yyyy-MM-dd---HH-mm-ss-fff"));
                        try
                        {
                            snapshot.Save(folder + filename, ImageFormat.Png);
                            snapshot.Dispose();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Issue with creating document snapshot file: " + ex.Message);
                            System.Console.ReadLine();
                        }

                        Uploadfiles(filename, folder);

                        try
                        {
                            System.IO.File.Delete(folder + filename);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Issue with deleting document snapshot file: " + ex.Message);
                            System.Console.ReadLine();
                        }

                        CaptureCamera();
                        isCameraRunning = true;

                        break;

                    case 6:
                        runprocess = 0;
                        GetSPImageList();
                        break;

                    case 7:
                        runprocess = 6;
                        GetSPImageList();
                        break;

                    case 8:
                        runprocess = 7;
                        GetSPImageList();
                        break;

                    case 9:
                        runprocess = 8;
                        GetSPImageList();
                        break;

                    case 10:
                        runprocess = 9;
                        GetSPImageList();
                        break;

                    default:
                        break;
                }

            }
            else if (!hubConnectedtoSP && !isAdvisor)
            {
                Uploadfiles("HubOnline.txt", CommandsFolder);
                hubConnectedtoSP = true;
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

                if (lstSPImages.Visible)
                {
                    lstSPImages.Left = this.Width - 245;
                    lstSPImages.Top = this.Height - 350;
                }
            }
            else if (!isAdvisor)     // This codepath is for the hub to be remotely controlled via Sharepoint by the Advisor
            {
                if (CheckSharepointRequest("Logout"))
                {

                    if (isCameraRunning)
                    {
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
                        isCameraRunning = false;
                    }
                    ScannerPreview.Visible = false;

                    LogOutWebsite();
                    Uploadfiles("HubOnline.txt", CommandsFolder);
                }

                if (isLoggedIn)
                {

                    if (isCameraRunning)
                    {
                        if (ScannerPreview.Image != null)
                        {
                            if (CheckSharepointRequest("Scan"))
                            {
                                isCameraRunning = false;        //pause camera updates
                                runprocess = 5;                 //trigger scan through run process

                            }
                        }
                    }
                    else
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
                                    if (txtError.Visible) txtError.Text+= "Issue blanking camera : " + ex.Message;
                                    System.Console.ReadLine();
                                }
                            }
                        }
                        System.GC.Collect();
                    }

                    if (!isCameraRunning)
                    {
                        if (CheckSharepointRequest("EnableScan"))
                        {
                            ScannerPreview.Visible = true;
                            CaptureCamera();
                            isCameraRunning = true;

                        }
                    }
                    else if (CheckSharepointRequest("DisableScan"))
                    {
                        try
                        {
                            capture.Release();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error stopping document camera : " + ex.Message);
                            if(txtError.Visible) txtError.Text += "Error stopping document camera : " + ex.Message;
                            System.Console.ReadLine();
                        }
                        
                        isCameraRunning = false;

                        ScannerPreview.Visible = false;
                    }



                }
                else if (CheckSharepointRequest("Login"))
                {
                    LoginWebsite();
                }


            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {

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

                Uploadfiles("SPTest.txt", CommandsFolder);
                if (CheckSharepointRequest("SPTest"))
                    SPStatus.Visible = false;
                else
                    SPStatus.Visible = true;
                GetSPImageList();
            }

            chromeBrowser.Load(MeetingURL);
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
            chromeBrowser.Load(MeetingURL);
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

                Uploadfiles("SPTest.txt", CommandsFolder);
                if (CheckSharepointRequest("SPTest"))
                    SPStatus.Visible = false;
                else
                    SPStatus.Visible = true;
                GetSPImageList();
            }

            chromeBrowser.Load(LoginURL);
        }

        private void loginHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor && CheckSharepointRequest("HubOnline") )
            {
                Uploadfiles("Login.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = false;
                logoutHubToolStripMenuItem.Enabled = true;
                startHubDocumentCameraToolStripMenuItem.Enabled = true;
            }
        }

        private void logoutHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles("Logout.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = true;
                logoutHubToolStripMenuItem.Enabled = false;
                startHubDocumentCameraToolStripMenuItem.Enabled = false;
                stopHubDocumentCameraToolStripMenuItem.Enabled = false;
                scanHubDocumentCameraToolStripMenuItem.Enabled = false;
            }
        }

        private void startHubDocumentCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles("EnableScan.txt", CommandsFolder);
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
                Uploadfiles("DisableScan.txt", CommandsFolder);
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
                Uploadfiles("Scan.txt", CommandsFolder);
                runprocess = 10;
            }
        }


        private void defaultCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load("chrome://settings/content/camera");
        }

        private void documentCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cameraindex += 1;
            if (cameraindex > 2)
            {
                cameraindex = 0;
            }
            documentCameraToolStripMenuItem.Text = "Document Camera = " + cameraindex.ToString();
        }

        private void resetHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isAdvisor)
            {
                Uploadfiles("Logout.txt", CommandsFolder);
                loginHubToolStripMenuItem.Enabled = true;
                logoutHubToolStripMenuItem.Enabled = false;
                startHubDocumentCameraToolStripMenuItem.Enabled = false;
                stopHubDocumentCameraToolStripMenuItem.Enabled = false;
                scanHubDocumentCameraToolStripMenuItem.Enabled = false;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            textBoxMeetingURL.Text = MeetingURL;
            textBoxDashboardURL.Text = LoginURL;
            textBoxHubID.Text = HubID;
            textBoxScansFolder.Text = SnapsFolder;
            textBoxCommandsFolder.Text = CommandsFolder;
            numericDocumentCameraIndex.Value = cameraindex;
            checkBoxSwapMainCamera.Checked = switchmaincamera;
            textBoxSharepointURL.Text = siteUrl;
            textBoxSharepointRelativeURL.Text = relativeURL;
            textBoxSharepointUsername.Text = userName;
            textBoxSharepointPassword.PasswordChar = '\0';
            textBoxSharepointPassword.Text = passwordMessage;
            SettingsPanel.Visible = true;

        }

        private void btnSettingsSave_Click(object sender, EventArgs e)
        {
            MeetingURL = textBoxMeetingURL.Text;
            LoginURL = textBoxDashboardURL.Text;
            HubID = textBoxHubID.Text;
            SnapsFolder = textBoxScansFolder.Text;
            CommandsFolder = textBoxCommandsFolder.Text;
            cameraindex = (int)numericDocumentCameraIndex.Value;
            switchmaincamera = checkBoxSwapMainCamera.Checked;
            siteUrl = textBoxSharepointURL.Text;
            relativeURL = textBoxSharepointRelativeURL.Text;
            userName = textBoxSharepointUsername.Text;
            if (!textBoxSharepointPassword.Text.Equals(passwordMessage)
               && textBoxSharepointPassword.Text.Length > 6)
            {                                                    //only update password if password has been modified
                password = textBoxSharepointPassword.Text;
            }
            SettingsPanel.Visible = false;

            SaveRunningConfig();
            if(isAdvisor)
            {
                Uploadfiles("SPTest.txt", CommandsFolder);
                if (CheckSharepointRequest("SPTest"))
                    SPStatus.Visible = false;
                else
                    SPStatus.Visible = true;
                GetSPImageList();
            }

        }

        private void btnSettingsCancel_Click(object sender, EventArgs e)
        {
            SettingsPanel.Visible = false;
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
            //popupBrowser.Load(siteUrl+"/"+docLibrary+"/" + HubID + "/" + lstSPImages.SelectedItem.ToString());
            string spAddress;
            spAddress = siteUrl.Remove(siteUrl.IndexOf("/sites/")) + relativeURL;
            popupBrowser.Load(spAddress + "/"+HubID + "/" + lstSPImages.SelectedItem.ToString());
            popupBrowser.BringToFront();
            closeImageToolStripMenuItem.Enabled = true;
        }

        private void popupBrowser_DoubleClick(object sender, EventArgs e)
        {
            popupBrowser.Load("");
            popupBrowser.SendToBack();

        }

        private void closeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            popupBrowser.Load("");
            popupBrowser.SendToBack();
            closeImageToolStripMenuItem.Enabled = false;
        }

        private void lstSPImages_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && lstSPImages.SelectedIndex!=-1)
            {
                DeleteSharepointFile(lstSPImages.SelectedItem.ToString());
                lstSPImages.Items.RemoveAt(lstSPImages.SelectedIndex);
                if (lstSPImages.Items.Count == 0)
                    lstSPImages.Visible = false;
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtError.Visible = !txtError.Visible;
            txtError.Text = "";
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
