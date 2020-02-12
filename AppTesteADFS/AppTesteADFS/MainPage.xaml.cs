using AppTesteADFS.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppTesteADFS
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        static string sdsSite = "appromateste";
        static public string authority = "https://adfs.romagnole.com.br/adfs";
        static public string clientID = "1cb68688-95ee-4e27-acf2-3ab6c0174cdb";
        static public string resourceURI = "https://" + sdsSite + ".romagnole.com.br/";
        static public string clientReturnURI = resourceURI;

        string bearerToken;

        public MainPage()
        {
            InitializeComponent();
            Login();
        }

        public async void Login()
        {
            var token = await SecureStorage.GetAsync("oauth_token");
            if (!string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Login", "Login realizado com sucesso", "Ok");
            }
            else
            {
                var urlLogin = authority + "/oauth2/authorize?response_type=token&client_id=" + clientID + "&redirect_uri=" + clientReturnURI + "&resource=" + resourceURI;
                webView.Navigated += WebView_Navigated;
                webView.Source = urlLogin;
            }
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success)
            {             
                if (e.Url.Contains("#access_token="))
                {
                    activityLoading.IsVisible = false;
                    var token = HttpUtility.ParseQueryString(e.Url).Get(0);
                    var validade = HttpUtility.ParseQueryString(e.Url).Get("expires_in");

                    if (!string.IsNullOrEmpty(token)){
                        camposLogin.IsVisible = false;
                        bearerToken = "bearer " + token;
                        await DisplayAlert("Login", "Login realizado com sucesso", "Ok");

                        await SecureStorage.SetAsync("oauth_token", bearerToken);
                    }
                    else
                    {
                        await DisplayAlert("Login", "Falha ao realizar o login", "Ok");
                        btEntrar.IsEnabled = true;
                    }
                }
                else
                {
                    camposLogin.IsVisible = true;
                    btEntrar.IsEnabled = true;
                    activityLoading.IsVisible = false;
                }
            }
            else if(e.Result == WebNavigationResult.Failure)
            {
                activityLoading.IsVisible = false;
                camposLogin.IsVisible = true;
                btEntrar.IsEnabled = true;
                await DisplayAlert("Login", "Falha ao realizar o login", "Ok");
            }
        }

        private async void btEntrar_Clicked(object sender, EventArgs e)
        {
            activityLoading.IsVisible = true;
            btEntrar.IsEnabled = false;
            var r = await webView.EvaluateJavaScriptAsync("document.forms['loginForm'].UserName.value = '" + sdsUsuario.Text + "'");
            r = await webView.EvaluateJavaScriptAsync("document.forms['loginForm'].Password.value = '" + sdsSenha.Text + "'");
            r = await webView.EvaluateJavaScriptAsync("Login.submitLoginRequest()");
        }
    }
}
