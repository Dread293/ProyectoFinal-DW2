namespace backend.Dtos;

public record CategoriaCrearDto(
    string Nombre,
    string? Slug,
    string? Descripcion,
    string? ImagenPortadaUrl,
    int? Orden,
    bool? Activo
);

public record CategoriaActualizarDto(
    string Nombre,
    string? Slug,
    string? Descripcion,
    string? ImagenPortadaUrl,
    int? Orden,
    bool? Activo
);
