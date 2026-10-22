
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("categorias")]
public class CategoriaController : ControllerBase
{
    private readonly VizaDbContext _context;

    public CategoriaController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /categorias
    [HttpGet]
    public async Task<IActionResult> ObtenerCategorias()
    {
        var categorias = await _context.CategoriaTrabajos
            .AsNoTracking()
            .Where(c => c.Activo == true)
            .OrderBy(c => c.Orden)
            .Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Slug,
                c.Descripcion,
                c.ImagenPortadaUrl,
                c.Orden
            })
            .ToListAsync();

        return Ok(categorias);
    }
}
