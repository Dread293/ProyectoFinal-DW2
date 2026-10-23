using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly VizaDbContext _context;

    public ClientesController(VizaDbContext context)
    {
        _context = context;
    }

    // GET: /api/clientes
    [HttpGet]
    public async Task<IActionResult> ObtenerClientes()
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .Include(c => c.CuentaCliente)
            .OrderBy(c => c.Id)
            .Select(c => new ClienteRespuestaDto(
                c.Id,
                c.Nombre,
                c.Telefono,
                c.Email,
                c.FechaRegistro,
                c.CuentaCliente != null
            ))
            .ToListAsync();

        return Ok(clientes);
    }

    // POST: /api/clientes
    [HttpPost]
    public async Task<IActionResult> CrearCliente([FromBody] ClienteCrearDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest(new { mensaje = "El nombre del cliente es obligatorio." });
        }

        string? emailNormalizado = dto.Email?.Trim().ToLower();

        if (!string.IsNullOrEmpty(emailNormalizado))
        {
            bool existeEmail = await _context.Clientes.AnyAsync(c => c.Email != null && c.Email.ToLower() == emailNormalizado);
            if (existeEmail)
            {
                return BadRequest(new { mensaje = "Ya existe un cliente registrado con ese correo electrónico." });
            }
        }

        var nuevoCliente = new Cliente
        {
            Nombre = dto.Nombre.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Email = emailNormalizado,
            FechaRegistro = DateTime.UtcNow
        };

        _context.Clientes.Add(nuevoCliente);
        await _context.SaveChangesAsync();

        // Si se envió contraseña y tiene email, creamos su cuenta de acceso
        if (!string.IsNullOrWhiteSpace(dto.Password) && !string.IsNullOrWhiteSpace(emailNormalizado))
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var nuevaCuenta = new CuentaCliente
            {
                ClienteId = nuevoCliente.Id,
                Correo = emailNormalizado,
                PasswordHash = hash,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };
            _context.CuentaClientes.Add(nuevaCuenta);
            await _context.SaveChangesAsync();
        }

        var respuesta = new ClienteRespuestaDto(
            nuevoCliente.Id,
            nuevoCliente.Nombre,
            nuevoCliente.Telefono,
            nuevoCliente.Email,
            nuevoCliente.FechaRegistro,
            !string.IsNullOrWhiteSpace(dto.Password)
        );

        return Created($"/api/clientes/{nuevoCliente.Id}", respuesta);
    }
}
