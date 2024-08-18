using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include(libroDB => libroDB.AutorLibro)
                .ThenInclude(autorDB => autorDB.Autor)
                .FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            libro.AutorLibro = libro.AutorLibro.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name = "CrearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroDTO)
        {
            if (libroDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }
            
            var autoresIds = await context.Autores.Where(autorDB => libroDTO.AutoresIds.Contains(autorDB.Id)).Select(x => x.Id).ToListAsync();

                
            if (libroDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroDTO);

            if(libro.AutorLibro != null)
            {
                for (int i = 0; i < libro.AutorLibro.Count; i++)
                {
                    libro.AutorLibro[i].Orden = i; 
                }
            }

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroCreacionDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibro", new { Id = libro.Id }, libroCreacionDTO);
        }
    }
}
