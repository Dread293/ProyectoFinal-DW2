namespace backend.Dtos;

public record LoginDto(
    string Correo,
    string Password
);

public record LoginRespuestaDto(
    string Token,
    ClientePerfilDto Cliente
);

public record ClientePerfilDto(
    int Id,
    string Nombre,
    string? Email,
    string? Telefono,
    DateTime? FechaRegistro
);
