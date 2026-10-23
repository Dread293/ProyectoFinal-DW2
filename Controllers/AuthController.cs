using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly VizaDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(VizaDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { mensaje = "Debe proporcionar correo y contraseña." });
        }

        var correoNormalizado = dto.Correo.Trim().ToLower();

        var cuenta = await _context.CuentaClientes
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.Correo.ToLower() == correoNormalizado);

        if (cuenta == null || cuenta.Activo == false)
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas o cuenta inactiva." });
        }

        // Verificar contraseña con BCrypt
        bool passwordValida = false;
        try
        {
            passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, cuenta.PasswordHash);
        }
        catch
        {
            passwordValida = false;
        }

        if (!passwordValida)
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas." });
        }

        // Actualizar último acceso
        cuenta.UltimoAcceso = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generar token JWT
        var token = GenerarTokenJwt(cuenta);

        var respuesta = new LoginRespuestaDto(
            token,
            new ClientePerfilDto(
                cuenta.Cliente.Id,
                cuenta.Cliente.Nombre,
                cuenta.Cliente.Email,
                cuenta.Cliente.Telefono,
                cuenta.Cliente.FechaRegistro
            )
        );

        return Ok(respuesta);
    }

    // GET: /api/auth/yo
    [HttpGet("yo")]
    [Authorize]
    public async Task<IActionResult> PerfilActual()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int clienteId))
        {
            return Unauthorized(new { mensaje = "No se pudo identificar al cliente en el token." });
        }

        var cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == clienteId);

        if (cliente == null)
        {
            return NotFound(new { mensaje = "No se encontró el registro del cliente." });
        }

        var perfil = new ClientePerfilDto(
            cliente.Id,
            cliente.Nombre,
            cliente.Email,
            cliente.Telefono,
            cliente.FechaRegistro
        );

        return Ok(perfil);
    }

    private string GenerarTokenJwt(Models.CuentaCliente cuenta)
    {
        var jwtKey = _config["Jwt:Key"] ?? "ClaveSuperSecretaDeVizaDesign2026ParaEstudiantesDeIngenieria";
        var jwtIssuer = _config["Jwt:Issuer"] ?? "VizaDesignBackend";
        var jwtAudience = _config["Jwt:Audience"] ?? "VizaDesignFrontend";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, cuenta.ClienteId.ToString()),
            new Claim(ClaimTypes.Email, cuenta.Correo),
            new Claim(ClaimTypes.Name, cuenta.Cliente.Nombre),
            new Claim(ClaimTypes.Role, "cliente")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
