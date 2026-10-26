
using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/categorias")]
[Route("categorias")]
public class CategoriaController : ControllerBase
{
    private readonly VizaDbContext _context;

    public CategoriaController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /api/categorias
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

    // GET: /api/categorias/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var categoria = await _context.CategoriaTrabajos
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Slug,
                c.Descripcion,
                c.ImagenPortadaUrl,
                c.Orden,
                c.Activo
            })
            .FirstOrDefaultAsync();

        if (categoria == null)
        {
            return NotFound(new { mensaje = $"No se encontró la categoría con id {id}" });
        }

        return Ok(categoria);
    }

    // POST: /api/categorias
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CategoriaCrearDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest(new { mensaje = "El nombre de la categoría es requerido." });
        }

        string slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? dto.Nombre.ToLower().Trim().Replace(" ", "-")
            : dto.Slug.ToLower().Trim();

        bool existeSlug = await _context.CategoriaTrabajos.AnyAsync(c => c.Slug == slug);
        if (existeSlug)
        {
            slug = $"{slug}-{DateTime.UtcNow.Ticks % 10000}";
        }

        var nuevaCategoria = new CategoriaTrabajo
        {
            Nombre = dto.Nombre.Trim(),
            Slug = slug,
            Descripcion = dto.Descripcion,
            ImagenPortadaUrl = dto.ImagenPortadaUrl,
            Orden = dto.Orden ?? 0,
            Activo = dto.Activo ?? true
        };

        _context.CategoriaTrabajos.Add(nuevaCategoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaCategoria.Id }, nuevaCategoria);
    }

    // PUT: /api/categorias/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CategoriaActualizarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest(new { mensaje = "El nombre de la categoría es requerido." });
        }

        var categoria = await _context.CategoriaTrabajos.FindAsync(id);
        if (categoria == null)
        {
            return NotFound(new { mensaje = $"No se encontró la categoría con id {id}" });
        }

        categoria.Nombre = dto.Nombre.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Slug))
        {
            categoria.Slug = dto.Slug.ToLower().Trim();
        }
        categoria.Descripcion = dto.Descripcion;
        categoria.ImagenPortadaUrl = dto.ImagenPortadaUrl;
        if (dto.Orden.HasValue) categoria.Orden = dto.Orden.Value;
        if (dto.Activo.HasValue) categoria.Activo = dto.Activo.Value;

        await _context.SaveChangesAsync();

        return Ok(categoria);
    }
        // GET: /api/categorias/{slug}/trabajos
    [HttpGet("{slug}/trabajos")]
    public async Task<IActionResult> ObtenerTrabajosPorCategoria(string slug)
    {
        var slugNormalizado = slug.Trim().ToLower();

        var categoria = await _context.CategoriaTrabajos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slugNormalizado && c.Activo == true);

        if (categoria == null)
        {
            return NotFound(new { mensaje = $"No se encontró la categoría '{slug}'" });
        }

        var trabajos = await _context.TrabajoGaleria
            .AsNoTracking()
            .Where(t => t.CategoriaId == categoria.Id && t.Publicado == true)
            .OrderBy(t => t.Orden)
            .ThenByDescending(t => t.Id)
            .Select(t => new TrabajoRespuestaDto(
                t.Id,
                t.CategoriaId,
                t.Categoria.Nombre,
                t.Titulo,
                t.Descripcion,
                t.Ciudad,
                t.FechaTrabajo,
                t.Destacado,
                t.Publicado,
                t.Orden,
                t.FechaCreacion,
                t.MediaTrabajos
                    .OrderBy(m => m.Orden)
                    .Select(m => new MediaItemDto(
                        m.Id,
                        m.Tipo,
                        m.Url,
                        m.MiniaturaUrl,
                        m.EsPortada,
                        m.Orden
                    ))
                    .ToList()
            ))
            .ToListAsync();

        return Ok(trabajos);
    }
}
