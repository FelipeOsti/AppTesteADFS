using AppTesteADFS.Login;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppTesteADFS
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            if (LoginPageModel.ExisteToken())
            {
                MainPage = new MainPage();
            }
            else
            {
                MainPage = new LoginPage(); //new MainPage();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
