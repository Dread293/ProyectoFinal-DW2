using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaludController : ControllerBase
{
    private readonly VizaDbContext _context;

    public SaludController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /api/salud
    [HttpGet]
    public async Task<IActionResult> ObtenerSalud()
    {
        bool bdConectada = await _context.Database.CanConnectAsync();

        return Ok(new
        {
            estado = "OK",
            mensaje = "Servicio backend operativo",
            baseDeDatos = bdConectada ? "Conectada" : "No disponible",
            fecha = DateTime.UtcNow
        });
    }
}
