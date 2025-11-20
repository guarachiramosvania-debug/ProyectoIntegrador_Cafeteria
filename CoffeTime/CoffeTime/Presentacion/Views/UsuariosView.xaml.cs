using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ProyectoIntegrador_Cafeteria.Negocio.Modelos;
using CoffeTime.Negocio.Servicios;
using CoffeTime.Datos.Repositorios;

namespace CoffeTime.Presentacion.Views
{
    public partial class UsuariosView : Window
    {
        public UsuariosView()
        {
            InitializeComponent();
            Loaded += UsuariosView_Loaded; // 👈 Conectar evento
        }

        private async void UsuariosView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // ✅ Ahora App.SupabaseClient existe
                var repo = new UsuarioRepository(App.SupabaseClient);
                var service = new UsuarioService(repo);

                var usuarios = await service.ObtenerTodosAsync();

                var lista = usuarios.Select(u => new
                {
                    NombreUsuario = u.NombreUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Rol = u.Rol,
                    EstadoTexto = u.Estado ? "Activo" : "Inactivo"
                }).ToList();

                DataGridUsuarios.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Botones
        private void BtnNuevo_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Nuevo");
        private void BtnEditar_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Editar");
        private void BtnEliminar_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Eliminar");
    }
}