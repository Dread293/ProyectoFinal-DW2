using backend.Data;
using backend.Dtos;
using backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Authorize]
[Route("api/mis-pedidos")]
public class MisPedidosController : ControllerBase
{
    private readonly VizaDbContext _context;

    public MisPedidosController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /api/mis-pedidos
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        var lista = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.FechaRegistro)
            .Select(p => new MiPedidoResumenDto(
                p.Id,
                p.Descripcion,
                p.Categoria != null ? p.Categoria.Nombre : null,
                p.Estado,
                p.FechaEntregaEstimada,
                p.FechaRegistro
            ))
            .ToListAsync();

        return Ok(lista);
    }

    // GET: /api/mis-pedidos/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id)
    {
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        // Se busca por id Y por dueño: si el pedido es de otro cliente, responde 404
        var pedido = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.Id == id && p.ClienteId == clienteId)
            .Select(p => new MiPedidoDetalleDto(
                p.Id,
                p.Descripcion,
                p.Categoria != null ? p.Categoria.Nombre : null,
                p.Estado,
                p.FechaRegistro,
                p.FechaEntregaEstimada,
                p.FechaEntregaReal
            ))
            .FirstOrDefaultAsync();

        if (pedido == null)
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        return Ok(pedido);
    }
}