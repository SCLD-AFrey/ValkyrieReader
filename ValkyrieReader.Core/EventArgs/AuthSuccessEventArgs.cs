﻿namespace ValkyrieReader.Core.AuthTokenRetriever.EventArgs
{
    public class AuthSuccessEventArgs
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
