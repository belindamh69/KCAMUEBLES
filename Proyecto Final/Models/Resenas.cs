using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final.Models
{
    public class Resenas
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        [Required]
        public string Tipo { get; set; }

        public string Producto { get; set; }

        [Required]
        public int Calificacion { get; set; }

        [Required]
        public string Comentario { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now; // FECHA REAL
    }
}
