using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public static class PermisosService
    {
        public static bool EsAdmin()
        {
            return App.Current.Properties["RolUsuario"]?.ToString() == "admin";
        }
    }
}
