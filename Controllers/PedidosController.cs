using backend.Data;
using backend.Dtos;
using backend.Extensions;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly VizaDbContext _context;

    public PedidosController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /api/pedidos
    [HttpGet]
    public async Task<IActionResult> ObtenerPedidos([FromQuery] int? clienteId, [FromQuery] string? estado)
    {
        var query = _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Categoria)
            .AsQueryable();

        if (clienteId.HasValue)
        {
            query = query.Where(p => p.ClienteId == clienteId.Value);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(p => p.Estado == estado.Trim().ToLower());
        }

        var lista = await query
            .OrderByDescending(p => p.Id)
            .Select(p => new PedidoResumenDto(
                p.Id,
                p.ClienteId,
                p.Cliente.Nombre,
                p.CategoriaId,
                p.Categoria != null ? p.Categoria.Nombre : null,
                p.Estado,
                p.FechaEntregaEstimada,
                p.FechaRegistro
            ))
            .ToListAsync();

        return Ok(lista);
    }

    // GET: /api/pedidos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPedidoPorId(int id)
    {
        var pedido = await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Categoria)
            .Include(p => p.PropuestaDisenos)
            .Include(p => p.HistorialEstados)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound(new { mensaje = $"No se encontró el pedido con id {id}" });
        }

        var detalle = new PedidoDetalleDto(
            pedido.Id,
            pedido.ClienteId,
            pedido.Cliente.Nombre,
            pedido.CategoriaId,
            pedido.Categoria?.Nombre,
            pedido.Estado,
            pedido.FechaRegistro,
            pedido.FechaEntregaEstimada,
            pedido.FechaEntregaReal,
            pedido.PropuestaDisenos
                .OrderBy(pr => pr.Id)
                .Select(pr => new PropuestaResumenDto(
                    pr.Id,
                    pr.Etiqueta,
                    pr.ImagenUrl,
                    pr.Estado,
                    pr.ComentarioCliente,
                    pr.FechaSubida
                ))
                .ToList(),
            pedido.HistorialEstados
                .OrderBy(h => h.Fecha)
                .Select(h => new HistorialResumenDto(
                    h.Id,
                    h.Estado,
                    h.NotaPublica,
                    h.Fecha
                ))
                .ToList()
        );

        return Ok(detalle);
    }

    // POST: /api/pedidos
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearPedido([FromBody] PedidoCrearDto dto)
    {
        // La identidad del cliente se toma del JWT, no del id enviado por el navegador.
        // Así evitamos asociar por error una cotización a otro cliente.
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token. Inicia sesión nuevamente." });
        }

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
        {
            return BadRequest(new { mensaje = "La descripción del pedido es obligatoria." });
        }

        bool existeCliente = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
        if (!existeCliente)
        {
            return BadRequest(new { mensaje = "El cliente asociado a la sesión no existe." });
        }

        if (dto.CategoriaId.HasValue)
        {
            bool existeCategoria = await _context.CategoriaTrabajos.AnyAsync(c => c.Id == dto.CategoriaId.Value);
            if (!existeCategoria)
            {
                return BadRequest(new { mensaje = $"La categoría con id {dto.CategoriaId.Value} no existe." });
            }
        }

        var nuevoPedido = new Pedido
        {
            ClienteId = clienteId,
            CategoriaId = dto.CategoriaId,
            Descripcion = dto.Descripcion,
            Estado = "pendiente",
            FechaRegistro = DateTime.UtcNow,
            FechaEntregaEstimada = dto.FechaEntregaEstimada
        };

        _context.Pedidos.Add(nuevoPedido);
        await _context.SaveChangesAsync();

        // Registrar primer estado en el historial
        var primerHistorial = new HistorialEstado
        {
            PedidoId = nuevoPedido.Id,
            Estado = "pendiente",
            NotaPublica = "Pedido registrado en el sistema",
            Fecha = DateTime.UtcNow
        };
        _context.HistorialEstados.Add(primerHistorial);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerPedidoPorId), new { id = nuevoPedido.Id }, new
        {
            id = nuevoPedido.Id,
            clienteId = nuevoPedido.ClienteId,
            categoriaId = nuevoPedido.CategoriaId,
            descripcion = nuevoPedido.Descripcion,
            estado = nuevoPedido.Estado,
            fechaRegistro = nuevoPedido.FechaRegistro,
            fechaEntregaEstimada = nuevoPedido.FechaEntregaEstimada
        });
    }
}
