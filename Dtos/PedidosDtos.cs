namespace backend.Dtos;

public record PedidoResumenDto(
    int Id, string? Descripcion, string? Categoria, string Estado,
    DateOnly? FechaEntregaEstimada, DateTime? FechaRegistro);

public record PedidoDetalleDto(
    int Id, string? Descripcion, string? Categoria, string Estado,
    DateTime? FechaRegistro, DateOnly? FechaEntregaEstimada, DateOnly? FechaEntregaReal);