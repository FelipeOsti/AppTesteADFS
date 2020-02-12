using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace AppTesteADFS.Login
{
    public class LoginPage : ContentPage
    {
        Label lblInfo, lblErro;
        Entry sdsUsuario, sdsSenha;
        Button btEntrar;
        ActivityIndicator activityLoading;
        WebView WebViewADFS;

        public delegate void LoginSucess();
        public event LoginSucess OnLoginSucess;

        public LoginPage()
        {         
            Content = new StackLayout()
            {
                Children = {
                    new PancakeView()
                    {
                        HeightRequest = 380, HorizontalOptions = LayoutOptions.FillAndExpand, CornerRadius = new CornerRadius(0,0,5,5), BackgroundGradientStartColor = Color.FromHex("#5e060c"),
                        BackgroundGradientEndColor = Color.FromHex("#D91F2D"), BackgroundGradientAngle = 315,
                        Content = new Image()
                        {
                            VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(20), Source = "logoEmpresa.png"
                        }
                    },
                    new PancakeView()
                    {
                        Margin = new Thickness(30,-50,30,0), BackgroundColor = Color.White, CornerRadius = new CornerRadius(15,15,15,15), Elevation = 5,
                        Content = new StackLayout()
                        {
                            Padding = 10,
                            Children =
                            {
                                GetLabelInfo(),
                                GetEntryUsuario(),
                                GetEntrySenha(),
                                GetButton(),
                                GetIndicator(),
                                GetLabelError()
                            }
                        }
                    },
                    getWebView()
                }
            };

            IniciarLogin();
        }

        /*Alterar esses dados conforme necessidade*/
        static public string authority = "https://adfs.toncoso.com.br/adfs";
        static public string clientID = "1cb51288-9tcb-4557-a122-3af6c0156cda";
        static public string resourceURI = "https://url.com.br/";
        static public string clientReturnURI = resourceURI;
        private void IniciarLogin()
        {
            var urlLogin = authority + "/oauth2/authorize?response_type=token&client_id=" + clientID + "&redirect_uri=" + clientReturnURI + "&resource=" + resourceURI;
            WebViewADFS.Navigated += WebViewADFS_Navigated;
            WebViewADFS.Source = urlLogin;
        }

        private async void WebViewADFS_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success)
            {
                if (e.Url.Contains("#access_token="))
                {
                    activityLoading.IsVisible = false;
                    var token = HttpUtility.ParseQueryString(e.Url).Get(0);
                    var validade = HttpUtility.ParseQueryString(e.Url).Get("expires_in");

                    if (token.Contains("#access_token="))
                    {
                        var posIgual = token.IndexOf("=");
                        token = token.Remove(0, posIgual + 1);
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        await LoginPageModel.SetToken(token, validade, sdsUsuario.Text);
                        OnLoginSucess();
                    }
                    else
                    {
                        btEntrar.IsEnabled = true;
                        await DisplayAlert("Login", "Falha ao realizar o login", "Ok");
                    }
                }
                else
                {
                    btEntrar.IsEnabled = true;
                    activityLoading.IsVisible = false;
                }
            }
            else if (e.Result == WebNavigationResult.Failure)
            {
                activityLoading.IsVisible = false;
                btEntrar.IsEnabled = true;
                await DisplayAlert("Login", "Falha ao realizar o login", "Ok");
            }
        }

        private View getWebView()
        {
            WebViewADFS = new WebView()
            {
                IsVisible = false
            };
            return WebViewADFS;
        }
        private View GetLabelError()
        {
            lblErro = new Label()
            {
                FontSize = 9,
                TextColor = Color.FromHex("#D91F2D")
            };
            return lblErro;
        }

        private View GetIndicator()
        {
            activityLoading = new ActivityIndicator()
            {
                IsVisible = false,
                IsRunning = true,
                IsEnabled = true,
                Color = Color.FromHex("#D91F2D"),
                HeightRequest = 30
            };
            return activityLoading;
        }

        private View GetButton()
        {
            btEntrar = new Button()
            {
                Text = "Entrar",
                CornerRadius = 20,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#D91F2D"),
                Margin = new Thickness(20, 10, 20, 0)
            };
            btEntrar.Clicked += BtEntrar_Clicked;
            return btEntrar;
        }

        private async void BtEntrar_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(sdsUsuario.Text))
            {
                //await DisplayAlert("Aviso", "Favor informar o usuário", "OK");
                sdsUsuario.PlaceholderColor = Color.FromHex("#D91F2D");
                Chacoalhacampo(sdsUsuario);
                return;
            }
            if (string.IsNullOrEmpty(sdsSenha.Text))
            {
                //await DisplayAlert("Aviso", "Favor informar a senha", "OK");
                sdsSenha.PlaceholderColor = Color.FromHex("#D91F2D");
                Chacoalhacampo(sdsSenha);
                return;
            }

            lblErro.Text = "";
            activityLoading.IsVisible = true;
            btEntrar.IsEnabled = false;
            await WebViewADFS.EvaluateJavaScriptAsync("document.getElementById('errorText').textContent = ''");
            var u = await WebViewADFS.EvaluateJavaScriptAsync("document.forms['loginForm'].UserName.value = '" + sdsUsuario.Text + "'");
            var p = await WebViewADFS.EvaluateJavaScriptAsync("document.forms['loginForm'].Password.value = '" + sdsSenha.Text + "'");
            var l = await WebViewADFS.EvaluateJavaScriptAsync("Login.submitLoginRequest()");

            await Task.Delay(500);
            var error = await WebViewADFS.EvaluateJavaScriptAsync("document.getElementById('errorText').textContent");
            if (!string.IsNullOrEmpty(error))
            {
                lblErro.Text = error;
                activityLoading.IsVisible = false;
                btEntrar.IsEnabled = true;
            }
        }

        private async void Chacoalhacampo(View campo)
        {
            uint timeout = 30;
            await campo.TranslateTo(-10, 0, timeout);
            await campo.TranslateTo(10, 0, timeout);
            await campo.TranslateTo(-5, 0, timeout);
            await campo.TranslateTo(5, 0, timeout);
            await campo.TranslateTo(-2, 0, timeout);
            await campo.TranslateTo(2, 0, timeout);
            campo.TranslationX = 0;
        }

        private View GetEntrySenha()
        {
            sdsSenha = new Entry()
            {
                Placeholder = "Senha",
                IsPassword = true,
                PlaceholderColor = Color.Gray,
                FontSize = 12,
                Visual = VisualMarker.Material
        };
            return sdsSenha;
        }

        private View GetEntryUsuario()
        {
            sdsUsuario = new Entry()
            {
                Placeholder = "E-mail",
                Keyboard = Keyboard.Email,
                PlaceholderColor = Color.Gray,
                FontSize = 12,
                Visual = VisualMarker.Material
        };
            return sdsUsuario;
        }

        private View GetLabelInfo()
        {
            lblInfo = new Label()
            {
                Text = "Entre com seus dados de acesso do E - mail",
                FontSize = 8,
                TextColor= Color.Gray,
                HorizontalTextAlignment = TextAlignment.Center
            };
            return lblInfo;
        }
    }
}
