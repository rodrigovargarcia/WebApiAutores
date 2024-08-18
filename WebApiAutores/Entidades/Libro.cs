using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        //[PrimeraLetraMayúscula]
        [Required]
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }  
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }
    }
}
