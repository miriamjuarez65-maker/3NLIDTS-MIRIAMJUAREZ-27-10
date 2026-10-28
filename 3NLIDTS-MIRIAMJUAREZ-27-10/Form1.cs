using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Mysqlx.Datatypes.Scalar.Types;

namespace _3NLIDTS_MIRIAMJUAREZ_27_10
{
    public partial class Form : System.Windows.Forms.Form
    {
        private string SQLConection =
    "Server=localhost;Port=3306;Database=programacion;Uid=root;Pwd=admin;";
        public Form()
        {
            InitializeComponent();
            ProbarConexion();
        }
       
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = tbNombre.Text.Trim();
            string apellidos = tbApellidos.Text.Trim();
            string edadTexto = tbEdad.Text.Trim();
            string estaturaTexto = tbEstatura.Text.Trim();
            string telefono = tbTelefono.Text.Trim();

            string genero = "";

            if (rbMasculino.Checked)
                genero = "Masculino";
            else if (rbFemenino.Checked)
                genero = "Femenino";

            // Validar los campos
            if (nombre == "" || apellidos == "" ||
                edadTexto == "" || estaturaTexto == "" ||
                telefono == "" || genero == "")
            {
                MessageBox.Show("Completa todos los campos.");
                return;
            }

            if (!EsTextoValido(nombre) ||
                !EsTextoValido(apellidos) ||
                !EsTelefonoValido(telefono))
            {
                MessageBox.Show("Verifica el nombre, los apellidos y el teléfono.");
                return;
            }

            if (!int.TryParse(edadTexto, out int edad) || edad <= 0)
            {
                MessageBox.Show("Ingresa una edad válida.");
                return;
            }

            if (!decimal.TryParse(estaturaTexto, out decimal estatura) ||
                estatura <= 0)
            {
                MessageBox.Show("Ingresa una estatura válida.");
                return;
            }

            try
            {
                // Guardar en MySQL
                using (MySqlConnection conexion =
                    new MySqlConnection(SQLConection))
                {
                    conexion.Open();

                    string consulta =
                        @"INSERT INTO registros
                (nombres, apellidos, telefono, estatura, edad, genero)
                VALUES
                (@nombre, @apellidos, @telefono, @estatura, @edad, @genero)";

                    using (MySqlCommand comando =
                        new MySqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@apellidos", apellidos);
                        comando.Parameters.AddWithValue("@telefono", telefono);
                        comando.Parameters.AddWithValue("@estatura", estatura);
                        comando.Parameters.AddWithValue("@edad", edad);
                        comando.Parameters.AddWithValue("@genero", genero);

                        comando.ExecuteNonQuery();
                    }
                }

                // Guardar también en el archivo TXT
                string rutaArchivo = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments),
                    "Registros.txt");

                string datos =
                    $"Nombre: {nombre}\r\n" +
                    $"Apellidos: {apellidos}\r\n" +
                    $"Edad: {edad}\r\n" +
                    $"Estatura: {estatura}\r\n" +
                    $"Teléfono: {telefono}\r\n" +
                    $"Género: {genero}\r\n" +
                    "-------------------------\r\n";

                File.AppendAllText(rutaArchivo, datos);

                MessageBox.Show(
                    "Los datos se guardaron correctamente en MySQL y en el TXT.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron guardar los datos:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            tbNombre.Clear();
            tbApellidos.Clear();
            tbEdad.Clear();
            tbEstatura.Clear();
            tbTelefono.Clear();

            rbMasculino.Checked = false;
            rbFemenino.Checked = false;

            tbNombre.Focus();

        }
        private bool EsTextoValido(string valor)
        {
            return Regex.IsMatch(valor, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");
        }

        private bool EsEnteroValido(string valor)
        {
            int resultado;
            return int.TryParse(valor, out resultado) && resultado > 0;
        }

        private bool EsEstaturaValida(string valor)
        {
            decimal resultado;
            return decimal.TryParse(valor, out resultado) && resultado > 0;
        }

        private bool EsTelefonoValido(string valor)
        {
            return Regex.IsMatch(valor, @"^\d{10}$");
        }
        private void ProbarConexion()
        {
            try
            {
                using (MySqlConnection conexion =
                    new MySqlConnection(SQLConection))
                {
                    conexion.Open();

                    MessageBox.Show(
                        "Conexión con MySQL exitosa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error de conexión: " + ex.Message);
            }
        }
    }
}
