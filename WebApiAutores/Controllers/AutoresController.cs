using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
         

        [HttpGet(Name = "ObtenerAutores")]
        [AllowAnonymous]
        public async Task<List<AutorDTO>> Get()
        {            
            var autores = await context.Autores.ToListAsync();

            var dtos = mapper.Map<List<AutorDTO>>(autores);
            dtos.ForEach(dto => GenerarEnlaces(dto));
            return dtos;
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var existeAutor = await context.Autores
                .Include(autorDB => autorDB.AutorLibro)
                .ThenInclude(libroDB => libroDB.Libro)
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);

            if (existeAutor == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<AutorDTOConLibros>(existeAutor);

            GenerarEnlaces(dto);

            return dto;
        }

        private void GenerarEnlaces(AutorDTO autorDTO)
        {
            autorDTO.Enlaces.Add(new DatoHateoas(
                enlace: Url.Link("ObtenerAutor", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "GET"
                ));

            autorDTO.Enlaces.Add(new DatoHateoas(
                enlace: Url.Link("ActualizarAutor", new { id = autorDTO.Id }),
                descripcion: "autor-actualizar",
                metodo: "PUT"
                ));

            autorDTO.Enlaces.Add(new DatoHateoas(
                enlace: Url.Link("BorrarAutor", new { id = autorDTO.Id }),
                descripcion: "autor-eliminar",
                metodo: "DELETE"
                ));
        }

        [HttpGet("{nombre}", Name = "ObtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre)
        {
            var autores = await context.Autores.Where(autoDB => autoDB.Nombre.Contains(nombre)).ToListAsync();
            
            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpPost(Name = "CrearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorDTO.Nombre);

            if(existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el mismo nombre {autorDTO.Nombre} ");
            }

            var autor = mapper.Map<Autor>(autorDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorCreacionDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorCreacionDTO);
        }

        [HttpPut("{id:int}", Name = "ActualizarAutor")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorDTO, int id)
        {            
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();    
        }

        [HttpDelete("{id:int}",Name = "BorrarAutor")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
