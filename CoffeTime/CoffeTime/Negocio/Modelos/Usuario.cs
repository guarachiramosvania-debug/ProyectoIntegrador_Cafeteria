// ProyectoIntegrador_Cafeteria.Negocio.Modelos.Usuario.cs
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System; // ← necesario para DateTime

namespace CoffeTime.Negocio.Modelos
{
    [Table("usuarios")]
    public class Usuario : BaseModel
    {
        [PrimaryKey("id_usuario")]
        public long IdUsuario { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("usuario")]
        public string NombreUsuario { get; set; }

        [Column("contrasena")]
        public string Contrasena { get; set; }

        [Column("rol")]
        public string Rol { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }
    }


}