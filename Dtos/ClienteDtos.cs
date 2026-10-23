namespace backend.Dtos;

public record ClienteCrearDto(
    string Nombre,
    string? Telefono,
    string? Email,
    string? Password
);

public record ClienteRespuestaDto(
    int Id,
    string Nombre,
    string? Telefono,
    string? Email,
    DateTime? FechaRegistro,
    bool TieneCuenta
);
