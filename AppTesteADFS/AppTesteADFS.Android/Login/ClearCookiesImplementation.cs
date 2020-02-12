using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AppTesteADFS.Login;

[assembly: Xamarin.Forms.Dependency(typeof(AppTesteADFS.Droid.Login.ClearCookiesImplementation))]
namespace AppTesteADFS.Droid.Login
{
    public class ClearCookiesImplementation : IClearCookies
    {
        public void Clear()
        {
            var cookieManager = Android.Webkit.CookieManager.Instance;
            cookieManager.RemoveAllCookie();
            cookieManager.RemoveSessionCookie();
            cookieManager.Flush();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => new Android.Webkit.WebView(Android.App.Application.Context).ClearCache(true));
        }
    }
}