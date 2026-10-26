using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrabajosController : ControllerBase
{
    private readonly VizaDbContext _context;

    public TrabajosController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /api/trabajos
    [HttpGet]
    public async Task<IActionResult> ObtenerTrabajos([FromQuery] int? categoriaId)
    {
        var query = _context.TrabajoGaleria
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Include(t => t.MediaTrabajos)
            .Where(t => t.Publicado == true);

        if (categoriaId.HasValue)
        {
            query = query.Where(t => t.CategoriaId == categoriaId.Value);
        }

        var lista = await query
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

        return Ok(lista);
    }

    // GET: /api/trabajos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerTrabajoPorId(int id)
    {
        var trabajo = await _context.TrabajoGaleria
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Include(t => t.MediaTrabajos)
            .Where(t => t.Id == id)
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
            .FirstOrDefaultAsync();

        if (trabajo == null)
        {
            return NotFound(new { mensaje = $"No se encontró el trabajo con id {id}" });
        }

        return Ok(trabajo);
    }

    // POST: /api/trabajos
    [HttpPost]
    public async Task<IActionResult> CrearTrabajo([FromBody] TrabajoCrearDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
        {
            return BadRequest(new { mensaje = "El título del trabajo es obligatorio." });
        }

        bool existeCategoria = await _context.CategoriaTrabajos.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!existeCategoria)
        {
            return BadRequest(new { mensaje = $"La categoría con id {dto.CategoriaId} no existe." });
        }

        var nuevoTrabajo = new TrabajoGalerium
        {
            CategoriaId = dto.CategoriaId,
            Titulo = dto.Titulo.Trim(),
            Descripcion = dto.Descripcion,
            Ciudad = dto.Ciudad,
            FechaTrabajo = dto.FechaTrabajo,
            Destacado = dto.Destacado ?? false,
            Publicado = dto.Publicado ?? true,
            Orden = dto.Orden ?? 0,
            FechaCreacion = DateTime.UtcNow
        };

        _context.TrabajoGaleria.Add(nuevoTrabajo);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(dto.ImagenPortadaUrl))
        {
            var media = new MediaTrabajo
            {
                TrabajoId = nuevoTrabajo.Id,
                Tipo = "imagen",
                Url = dto.ImagenPortadaUrl.Trim(),
                EsPortada = true,
                Orden = 1
            };
            _context.MediaTrabajos.Add(media);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(ObtenerTrabajoPorId), new { id = nuevoTrabajo.Id }, new
        {
            id = nuevoTrabajo.Id,
            categoriaId = nuevoTrabajo.CategoriaId,
            titulo = nuevoTrabajo.Titulo,
            descripcion = nuevoTrabajo.Descripcion,
            ciudad = nuevoTrabajo.Ciudad,
            fechaTrabajo = nuevoTrabajo.FechaTrabajo,
            destacado = nuevoTrabajo.Destacado,
            publicado = nuevoTrabajo.Publicado,
            orden = nuevoTrabajo.Orden,
            fechaCreacion = nuevoTrabajo.FechaCreacion
        });
    }

    // PUT: /api/trabajos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarTrabajo(int id, [FromBody] TrabajoActualizarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
        {
            return BadRequest(new { mensaje = "El título del trabajo es obligatorio." });
        }

        var trabajo = await _context.TrabajoGaleria.FindAsync(id);
        if (trabajo == null)
        {
            return NotFound(new { mensaje = $"No se encontró el trabajo con id {id}" });
        }

        bool existeCategoria = await _context.CategoriaTrabajos.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!existeCategoria)
        {
            return BadRequest(new { mensaje = $"La categoría con id {dto.CategoriaId} no existe." });
        }

        trabajo.CategoriaId = dto.CategoriaId;
        trabajo.Titulo = dto.Titulo.Trim();
        trabajo.Descripcion = dto.Descripcion;
        trabajo.Ciudad = dto.Ciudad;
        trabajo.FechaTrabajo = dto.FechaTrabajo;
        if (dto.Destacado.HasValue) trabajo.Destacado = dto.Destacado.Value;
        if (dto.Publicado.HasValue) trabajo.Publicado = dto.Publicado.Value;
        if (dto.Orden.HasValue) trabajo.Orden = dto.Orden.Value;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = trabajo.Id,
            categoriaId = trabajo.CategoriaId,
            titulo = trabajo.Titulo,
            descripcion = trabajo.Descripcion,
            ciudad = trabajo.Ciudad,
            fechaTrabajo = trabajo.FechaTrabajo,
            destacado = trabajo.Destacado,
            publicado = trabajo.Publicado,
            orden = trabajo.Orden
        });
    }
}
