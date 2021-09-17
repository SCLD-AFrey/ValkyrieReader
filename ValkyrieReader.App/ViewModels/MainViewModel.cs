using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using ValkyrieReader.Core;
using Reddit;
using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;
using ValkyrieReader.App.Views;

namespace ValkyrieReader.App.ViewModels
{
    public class NewTab
    {
        public string header { get; set; }
        public UserControl content { get; set; }
    }
    
    [MetadataType(typeof(MetaData))]
    public class MainViewModel
    {
        public class MetaData : IMetadataProvider<MainViewModel>
        {
            void IMetadataProvider<MainViewModel>.BuildMetadata(MetadataBuilder<MainViewModel> p_builder)
                {
                    p_builder.CommandFromMethod(p_x => p_x.OnLoginButton()).CommandName("DoLoginButton");
                }
        }

        #region Constructors

        protected MainViewModel()
        {
            var app = Application.Current as App;
            AccessToken = $"Client ID: {app.m_AppId} - Access Token: {Properties.Settings.Default.AccessToken} - IsAuth: {IsAuth}";
        }

        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual Application app { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual bool IsAuth { get; set; } = false;
        public virtual List<DXTabItem> ListTabs { get; set; } = new List<DXTabItem>();

        #endregion

        #region Methods

        
        
        public async void OnLoginButton()
        {
            var app = Application.Current as App;
            if (string.IsNullOrEmpty(Properties.Settings.Default.AccessToken) || string.IsNullOrEmpty(Properties.Settings.Default.RefreshToken))
            {
                await AuthorizeUser();
                Properties.Settings.Default.AccessToken = app.m_accesstoken;
                Properties.Settings.Default.RefreshToken = app.m_refreshtoken;
            }
            else
            {
                app.m_accesstoken = Properties.Settings.Default.AccessToken;
                app.m_refreshtoken = Properties.Settings.Default.RefreshToken;
            }
            IsAuth = true;
            AccessToken = $"Client ID: {app.m_AppId} - Access Token: {Properties.Settings.Default.AccessToken} - IsTabsEnabled: {IsAuth}";


            while (string.IsNullOrEmpty(app.m_accesstoken))
            {
            }


            if (IsAuth)
            {
                ListTabs.Add(new DXTabItem()
                {
                    Header = "Front Page", 
                    Content = new FrontPage()
                });
            }
        }


        public static async Task AuthorizeUser()
        {
            var app = Application.Current as App;
            var authTokenRetrieverLib =
                new AuthTokenRetrieverLib(
                    appId: app.m_AppId,
                    appSecret: app.m_AppSecret);
            authTokenRetrieverLib.AuthSuccess += C_AuthSuccess;
            authTokenRetrieverLib.AwaitCallback(true);
            await OpenBrowser(authTokenRetrieverLib.AuthURL());
            while (Properties.Settings.Default.AccessToken == null && Properties.Settings.Default.RefreshToken == null) { }
            authTokenRetrieverLib.StopListening();
        }
        
        
        public static async Task OpenBrowser(string authUrl = "about:blank")
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
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(@"C:\Users\afrey.STEELCLOUD\AppData\Local\Programs\Opera\launcher.exe")
                    {
                        Arguments = authUrl
                    };
                    Process.Start(processStartInfo);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", authUrl);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", authUrl);
            }
        }
        
        private static void C_AuthSuccess(object sender, AuthSuccessEventArgs e)
        {
            var app = Application.Current as App;

            Properties.Settings.Default.AccessToken = e.AccessToken;
            Properties.Settings.Default.RefreshToken = e.RefreshToken;
            Properties.Settings.Default.Save();
            
            app.m_client = new RedditClient(app.m_AppId, e.RefreshToken, app.m_AppSecret, e.AccessToken);
            app.m_accesstoken = e.AccessToken;
            app.m_refreshtoken = e.RefreshToken;
            
            Debug.WriteLine("Access Token: " + e.AccessToken);
            Debug.WriteLine("Refresh Token: " + e.RefreshToken);
        }
        
        #endregion
    }
}