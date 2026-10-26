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
public class EstadoController : ControllerBase
{
    private readonly VizaDbContext _context;

    public EstadoController(VizaDbContext context)
    {
        _context = context;
    }

    // Texto para mostrar y porcentaje de avance de cada estado
    private static readonly Dictionary<string, (string Etiqueta, int Progreso)> Estados = new()
    {
        ["pendiente"] = ("Pedido recibido", 10),
        ["en_proceso"] = ("En producción", 50),
        ["terminado"] = ("Trabajo terminado", 90),
        ["entregado"] = ("Entregado", 100),
    };

    private static (string Etiqueta, int Progreso) Info(string? estado) =>
        estado is not null && Estados.TryGetValue(estado, out var info) ? info : (estado ?? "", 0);

    // GET: /api/mis-pedidos/{id}/estado
    //viza esta fallando aca
    [HttpGet("{id:int}/estado")]
    public async Task<IActionResult> ObtenerEstado(int id)
    {
        if (User.ClienteId() is not int clienteId)
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        var pedido = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.Id == id && p.ClienteId == clienteId)
            .Select(p => new { p.Estado, p.FechaEntregaEstimada, p.FechaEntregaReal })
            .FirstOrDefaultAsync();

        if (pedido == null)
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        var historial = await _context.HistorialEstados
            .AsNoTracking()
            .Where(h => h.PedidoId == id)
            .OrderBy(h => h.Fecha)
            .Select(h => new { h.Estado, h.NotaPublica, h.Fecha })
            .ToListAsync();

        var actual = Info(pedido.Estado);

        var dto = new EstadoPedidoDto(
            id,
            pedido.Estado,
            actual.Etiqueta,
            actual.Progreso,
            pedido.FechaEntregaEstimada,
            pedido.FechaEntregaReal,
            historial
                .Select(h => new LineaTiempoDto(h.Estado, Info(h.Estado).Etiqueta, h.NotaPublica, h.Fecha))
                .ToList()
        );

        return Ok(dto);
    }
}