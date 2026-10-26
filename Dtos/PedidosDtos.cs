namespace backend.Dtos;

public record PedidoCrearDto(
    int ClienteId,
    int? CategoriaId,
    string? Descripcion,
    DateOnly? FechaEntregaEstimada
);

public record PedidoResumenDto(
    int Id,
    int ClienteId,
    string? ClienteNombre,
    int? CategoriaId,
    string? Categoria,
    string Estado,
    DateOnly? FechaEntregaEstimada,
    DateTime? FechaRegistro
);

public record PropuestaResumenDto(
    int Id,
    string? Etiqueta,
    string ImagenUrl,
    string Estado,
    string? ComentarioCliente,
    DateTime? FechaSubida
);

public record HistorialResumenDto(
    int Id,
    string Estado,
    string? NotaPublica,
    DateTime? Fecha
);

public record PedidoDetalleDto(
    int Id,
    int ClienteId,
    string? ClienteNombre,
    int? CategoriaId,
    string? Categoria,
    string Estado,
    DateTime? FechaRegistro,
    DateOnly? FechaEntregaEstimada,
    DateOnly? FechaEntregaReal,
    List<PropuestaResumenDto> Propuestas,
    List<HistorialResumenDto> Historial
);