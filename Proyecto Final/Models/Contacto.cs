using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Final.Models
{
    public class Contacto
    {
        [Required]
        public string NombreCompleto { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
        [Required]
        public string Asunto { get; set; }
        [Required]
        public string Mensaje { get; set; }
    }
}
