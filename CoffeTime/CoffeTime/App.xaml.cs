using System.Windows;
using Supabase;

namespace CoffeTime
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Client SupabaseClient { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 🔑 CONFIGURA TUS CREDENCIALES DE SUPABASE AQUÍ
            string supabaseUrl = "https://db.utushbtzxirwtccdycqm.supabase.co";
            string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // ← TU ANON KEY

            SupabaseClient = new Client(supabaseUrl, supabaseKey);
            await SupabaseClient.InitializeAsync();
        }
    }
}