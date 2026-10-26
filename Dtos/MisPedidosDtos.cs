namespace backend.Dtos;

// Lo que ve el cliente logueado. No incluye precios ni datos de otros clientes.
public record MiPedidoResumenDto(
    int Id,
    string? Descripcion,
    string? Categoria,
    string Estado,
    DateOnly? FechaEntregaEstimada,
    DateTime? FechaRegistro
);

public record MiPedidoDetalleDto(
    int Id,
    string? Descripcion,
    string? Categoria,
    string Estado,
    DateTime? FechaRegistro,
    DateOnly? FechaEntregaEstimada,
    DateOnly? FechaEntregaReal
);

public record DisenoDto(
    int Id,
    string? Etiqueta,
    string ImagenUrl,
    string Estado,
    string? ComentarioCliente,
    DateTime? FechaSubida,
    DateTime? FechaRespuesta
);

public record RechazarDisenoDto(string? Comentario);