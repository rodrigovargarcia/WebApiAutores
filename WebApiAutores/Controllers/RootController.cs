﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHateoas>>> Get()
        {
            var datosHateoas = new List<DatoHateoas>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatoHateoas(enlace: Url.Link("obtenerRoot", new { }),descripcion: "self", metodo: "GET"));            
            
            datosHateoas.Add(new DatoHateoas(enlace: Url.Link("ObtenerAutores", new { }), descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHateoas(enlace: Url.Link("CrearAutor", new { }), descripcion: "autor-crear", metodo: "POST"));
                datosHateoas.Add(new DatoHateoas(enlace: Url.Link("ActualizarAutor", new { }), descripcion: "autor-actualizar", metodo: "PUT"));
                datosHateoas.Add(new DatoHateoas(enlace: Url.Link("CrearLibro", new { }), descripcion: "libro-crear", metodo: "POST"));
            }

            return datosHateoas;
        }
    }
}
