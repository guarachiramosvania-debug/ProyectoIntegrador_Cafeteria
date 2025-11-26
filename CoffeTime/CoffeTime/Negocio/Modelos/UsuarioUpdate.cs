using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;

namespace CoffeTime.Negocio.Modelos
{
    public class UsuarioUpdate : BaseModel
    {
        [Column("online")]
        public bool Online { get; set; }
    }
}