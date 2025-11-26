using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;

namespace CoffeTime.Negocio.Modelos
{
    public class UsuarioPerfilUpdate : BaseModel
    {
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("usuario")]
        public string NombreUsuario { get; set; }

        [Column("rol")]
        public string Rol { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }

        [Column("ultimo_login")]
        public DateTime? UltimoLogin { get; set; }   // ← ACEPTA NULL


    }
}
