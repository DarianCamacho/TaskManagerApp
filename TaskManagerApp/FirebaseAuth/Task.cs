using Firebase.Auth.Providers;
using Firebase.Auth;

namespace WebAppCondominio.FirebaseAuth
{
    public static class FirebaseAuthHelper
    {
        public const string firebaseAppId = "\r\ntask-6503f";
        public const string firebaseApiKey = "AIzaSyD3zc8a9JKT_viZAw-jzvGqWQr4t0QcEOQ";

        public static FirebaseAuthClient setFirebaseAuthClient()
        {
            var response = new FirebaseAuthClient(new FirebaseAuthConfig
            {
                ApiKey = firebaseApiKey,
                AuthDomain = $"{firebaseAppId}.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                    {
                        new EmailProvider()
                    }
            });

            return response;
        }
    }
}