using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AppTesteADFS.Login
{
    public static class LoginPageModel
    {
        public static bool IsLogged { get; set; }
        public static string Token { get; set; }
        public static string TokenBearer { get { return "Bearer " + Token; } } //Propriedade utilizada na autenticação
        public static string Email { get; set; }
        public static DateTime? Validade { get; set; }
        public static bool IsValid {
            get
            {
                if (Validade == null) return false;
                if (Validade <= DateTime.Now) return false;
                return true;
            }
        }

        const string Oauth_Email_key = "oauth_email";
        const string Oauth_token_key = "oauth_token";
        const string Validade_token_key = "validade_token";

        public async static Task<bool> SetToken(string token, string segValidade, string sdsEmail)
        {
            try
            {
                var tokenOld = await SecureStorage.GetAsync(Oauth_token_key);
                if (token != tokenOld)
                {
                    var validade = DateTime.Now;
                    var novaValidade = validade.AddSeconds(long.Parse(segValidade));

                    await SecureStorage.SetAsync(Oauth_token_key, token);
                    await SecureStorage.SetAsync(Validade_token_key, novaValidade.ToString());
                    if (!string.IsNullOrEmpty(sdsEmail)) await SecureStorage.SetAsync(Oauth_Email_key, sdsEmail);
                }
                return ExisteToken();
            }
            catch
            {
                return false;
            }
        }

        public static bool ExisteToken()
        {
            try
            {
                GetTokenCofre();
                if (Validade <= DateTime.Now) return false;
                if (string.IsNullOrEmpty(Email)) return false;
                if (!string.IsNullOrEmpty(Token)) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void LogOff()
        {
            LimparToken();
        }

        public async static void LimparToken()
        {
            Xamarin.Forms.DependencyService.Get<IClearCookies>().Clear();

            await SecureStorage.SetAsync(Oauth_token_key, "");
            SecureStorage.Remove(Oauth_token_key);
            Token = null;

            await SecureStorage.SetAsync(Validade_token_key, "");
            SecureStorage.Remove(Validade_token_key);
            Validade = null;

            await SecureStorage.SetAsync(Oauth_Email_key, "");
            SecureStorage.Remove(Oauth_Email_key);
            Email = null;

            IsLogged = false;
        }

        private static async void GetTokenCofre()
        {
            if (!string.IsNullOrEmpty(Token)) return;

            var token = await SecureStorage.GetAsync(Oauth_token_key);
            var validade = await SecureStorage.GetAsync(Validade_token_key);
            var sdsEmail = await SecureStorage.GetAsync(Oauth_Email_key);

            if (!string.IsNullOrEmpty(token))
            {
                Token = token;
                Validade = DateTime.Parse(validade);
                Email = sdsEmail;
                IsLogged = true;
            }
        }
    }
}
