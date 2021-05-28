# VideoHub
Provides a wrapper and interface to enable remotely controlled video calling, with no computer interaction required by the remote user. Also enables the remote user to upload documents without needing to physically interact with the computer hardware.

Use cases include, providing a remote drop-in facility for assisting clients who would like an experience as close to a face-to-face meeting as possible.

Uses Sharepoint to communicate between employee (Advisor) and client (Hub) software endpoints. Uses Sharepoint to store and display uploaded documents. Uses Vectera for video calling and screen sharing.

Built and tested with Visual Studio 2017.

Requires the following packages to be installed from NuGet OpenCvSharp3-AnyCPU CefSharp.Common CefSharp.WinForms cef.redist.x86 cef.redist.x64

Once built it can just be run from the build folder - there are two important subfolders .\commands - contains empty textfiles that are used to send messages between endpoints .\scans - used to temporarily hold scanned documents before they are uploaded to sharepoint

VideoHub.exe.config stores the configuration in the appSettings section. Some of these can be changed within the app, some are displayed, but not changeable.

To-do Currently it's quite involved to change between hubs (as need to select correct meeting) - planned is for hubs to advertise their statuses, and Advisor can choose to connect to them, which will configure the required meeting.
