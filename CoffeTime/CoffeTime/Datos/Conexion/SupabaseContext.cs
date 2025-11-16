using Supabase;

namespace ProyectoIntegrador_Cafeteria.Datos.Conexion
{
    public static class SupabaseContext
    {
        private static Client _client;

        public static Client Client
        {
            get
            {
                if (_client == null)
                {
                    var url = "https://ycuvjpgbqkfvfvxmoffs.supabase.co";   // <-- URL correcta
                    var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InljdXZqcGdicWtmdmZ2eG1vZmZzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjMzMTg5MDMsImV4cCI6MjA3ODg5NDkwM30.AFVXbGB4_6MAyTxkS_0hy5wxYw9HCs1_FZslDfwPaCs";                               // <-- Tu anon key

                    _client = new Client(url, key, new SupabaseOptions
                    {
                        AutoConnectRealtime = false
                    });
                }

                return _client;
            }
        }
    }
}
