using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using Reddit;
using ValkyrieReader.Core;

namespace ValkyrieReader.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Task _initializingTask;
        public string m_AppId = null;
        public string m_AppSecret = null;
        public string m_accesstoken = null;
        public string m_refreshtoken = null;
        public string m_browserPath = @"C:\Users\afrey.STEELCLOUD\AppData\Local\Programs\Opera\launcher.exe";
        public RedditClient m_client = null;

        protected override async void OnStartup(StartupEventArgs e)
        {
            m_AppId = "i-7eOqShoomb9k_NO8C7DQ";
            m_AppSecret = "YxmIleu9rjVyZuvrYApdopYFsCyJ7Q";
        }
        
        public App()
        {

        }

    }
    
}
