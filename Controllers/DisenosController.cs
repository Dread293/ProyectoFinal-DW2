using backend.Data;
using backend.Dtos;
using backend.Extensions;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Authorize]
[Route("api/mis-pedidos")]
public class DisenosController : ControllerBase
{
    private readonly VizaDbContext _context;

    public DisenosController(VizaDbContext context)
    {
        _context = context;
    }

    // ¿El pedido pertenece al cliente del token?
    private Task<bool> EsMio(int pedidoId, int clienteId) =>
        _context.Pedidos.AnyAsync(p => p.Id == pedidoId && p.ClienteId == clienteId);

    private static DisenoDto Mapear(PropuestaDiseno d) =>
        new(d.Id, d.Etiqueta, d.ImagenUrl, d.Estado, d.ComentarioCliente, d.FechaSubida, d.FechaRespuesta);

    // GET: /api/mis-pedidos/{pedidoId}/disenos
    [HttpGet("{pedidoId:int}/disenos")]
    public async Task<IActionResult> Listar(int pedidoId)
    {
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        if (!await EsMio(pedidoId, clienteId))
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        var lista = await _context.PropuestaDisenos
            .AsNoTracking()
            .Where(d => d.PedidoId == pedidoId)
            .OrderBy(d => d.FechaSubida)
            .ThenBy(d => d.Id)
            .ToListAsync();

        return Ok(lista.Select(Mapear));
    }

    // POST: /api/mis-pedidos/{pedidoId}/disenos/{disenoId}/aprobar
    [HttpPost("{pedidoId:int}/disenos/{disenoId:int}/aprobar")]
    public async Task<IActionResult> Aprobar(int pedidoId, int disenoId)
    {
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        if (!await EsMio(pedidoId, clienteId))
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        var diseno = await _context.PropuestaDisenos
            .FirstOrDefaultAsync(d => d.Id == disenoId && d.PedidoId == pedidoId);

        if (diseno == null)
        {
            return NotFound(new { mensaje = "Diseño no encontrado." });
        }

        if (diseno.Estado != "pendiente")
        {
            return Conflict(new { mensaje = "Este diseño ya fue respondido." });
        }

        bool yaHayAprobado = await _context.PropuestaDisenos
            .AnyAsync(d => d.PedidoId == pedidoId && d.Estado == "aprobada");

        if (yaHayAprobado)
        {
            return Conflict(new { mensaje = "Este pedido ya tiene un diseño aprobado." });
        }

        diseno.Estado = "aprobada";
        diseno.FechaRespuesta = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(Mapear(diseno));
    }

    // POST: /api/mis-pedidos/{pedidoId}/disenos/{disenoId}/rechazar
    [HttpPost("{pedidoId:int}/disenos/{disenoId:int}/rechazar")]
    public async Task<IActionResult> Rechazar(int pedidoId, int disenoId, [FromBody] RechazarDisenoDto dto)
    {
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        var comentario = (dto.Comentario ?? "").Trim();
        if (comentario.Length == 0)
        {
            return BadRequest(new { mensaje = "El comentario es obligatorio al rechazar un diseño." });
        }
        if (comentario.Length > 1000)
        {
            return BadRequest(new { mensaje = "El comentario no puede pasar de 1000 caracteres." });
        }

        if (!await EsMio(pedidoId, clienteId))
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        var diseno = await _context.PropuestaDisenos
            .FirstOrDefaultAsync(d => d.Id == disenoId && d.PedidoId == pedidoId);

        if (diseno == null)
        {
            return NotFound(new { mensaje = "Diseño no encontrado." });
        }

        if (diseno.Estado != "pendiente")
        {
            return Conflict(new { mensaje = "Este diseño ya fue respondido." });
        }

        diseno.Estado = "rechazada";
        diseno.ComentarioCliente = comentario;
        diseno.FechaRespuesta = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(Mapear(diseno));
    }
}