﻿using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Entidades
{
    public class Autor 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:120, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres")]
        public string Nombre { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }
    }
}
