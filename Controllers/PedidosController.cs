using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> CrearPedido([FromBody] PedidoCrearDto dto)
    {
        if (dto.ClienteId <= 0)
        {
            return BadRequest(new { mensaje = "Debe indicar un cliente válido (clienteId)." });
        }

        bool existeCliente = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
        if (!existeCliente)
        {
            return BadRequest(new { mensaje = $"El cliente con id {dto.ClienteId} no existe." });
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
            ClienteId = dto.ClienteId,
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
