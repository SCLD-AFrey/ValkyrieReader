using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Reddit;
using ValkyrieReader.Core;
using ValkyrieReader.Core.AuthTokenRetriever;
using ValkyrieReader.Core.AuthTokenRetriever.EventArgs;



namespace ValkyrieRedReader
{
    class Program
    {
        public const string BROWSER_PATH = @"C:\Users\afrey.STEELCLOUD\AppData\Local\Programs\Opera\launcher.exe";
        static void Main(string[] args)
        {
            var authTokenRetrieverLib =
            new AuthTokenRetrieverLib(appId: Constants.Reddit.AppId,
                appSecret: Constants.Reddit.AppSecret);
            authTokenRetrieverLib.AuthSuccess += C_AuthSuccess;
            authTokenRetrieverLib.AwaitCallback(true);
            OpenBrowser(authTokenRetrieverLib.AuthURL());
            Console.WriteLine("Awaiting Reddit callback -OR- press any key to abort....");

            Console.ReadKey();  // Hit any key to exit.  --Kris

            authTokenRetrieverLib.StopListening();

            Console.WriteLine("Token retrieval utility terminated.");
        }
        public static void C_AuthSuccess(object sender, AuthSuccessEventArgs e)
        {
            Console.Clear();

            Console.WriteLine("Token retrieval successful!");

            Console.WriteLine();

            Console.WriteLine("Access Token: " + e.AccessToken);
            Console.WriteLine("Refresh Token: " + e.RefreshToken);

            Console.WriteLine();

            Console.WriteLine("Press any key to exit....");
        }
        
        public static void OpenBrowser(string authUrl = "about:blank")
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(authUrl);
                    Process.Start(processStartInfo);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(BROWSER_PATH)
                    {
                        Arguments = authUrl
                    };
                    Process.Start(processStartInfo);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // For OSX run a separate command to open the web browser as found in https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
                Process.Start("open", authUrl);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Similar to OSX, Linux can (and usually does) use xdg for this task.
                Process.Start("xdg-open", authUrl);
            }
        }
    }
}