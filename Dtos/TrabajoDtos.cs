namespace backend.Dtos;

public record MediaItemDto(
    int Id,
    string Tipo,
    string Url,
    string? MiniaturaUrl,
    bool? EsPortada,
    int? Orden
);

public record TrabajoRespuestaDto(
    int Id,
    int CategoriaId,
    string? CategoriaNombre,
    string Titulo,
    string? Descripcion,
    string? Ciudad,
    DateOnly? FechaTrabajo,
    bool? Destacado,
    bool? Publicado,
    int? Orden,
    DateTime? FechaCreacion,
    List<MediaItemDto> Media
);

public record TrabajoCrearDto(
    int CategoriaId,
    string Titulo,
    string? Descripcion,
    string? Ciudad,
    DateOnly? FechaTrabajo,
    bool? Destacado,
    bool? Publicado,
    int? Orden,
    string? ImagenPortadaUrl
);

public record TrabajoActualizarDto(
    int CategoriaId,
    string Titulo,
    string? Descripcion,
    string? Ciudad,
    DateOnly? FechaTrabajo,
    bool? Destacado,
    bool? Publicado,
    int? Orden
);
