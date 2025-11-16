using Supabase;
using System.Threading.Tasks;

namespace ProyectoIntegrador_Cafeteria.Datos.Conexion
{
    public static class SupabaseContext
    {
        private static Supabase.Client _client;

        public static Supabase.Client Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new Supabase.Client(
                        "postgresql://postgres.ycuvjpgbqkfvfvxmoffs:coffe123@aws-0-us-west-2.pooler.supabase.com:5432/postgres",     // <-- Aquí tu URL de Supabase
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InljdXZqcGdicWtmdmZ2eG1vZmZzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjMzMTg5MDMsImV4cCI6MjA3ODg5NDkwM30.AFVXbGB4_6MAyTxkS_0hy5wxYw9HCs1_FZslDfwPaCs", // <-- Aquí tu API Key
                        new SupabaseOptions
                        {
                            AutoConnectRealtime = false
                        }
                    );
                }

                return _client;
            }
        }

        public static async Task InitializeAsync()
        {
            if (!_client.Initialized)
                await _client.InitializeAsync();
        }
    }
}
