namespace backend.Dtos;

public record LineaTiempoDto(string Estado, string Etiqueta, string? Nota, DateTime? Fecha);

public record EstadoPedidoDto(
    int PedidoId, string Estado, string Etiqueta, int Progreso,
    DateOnly? FechaEntregaEstimada, DateOnly? FechaEntregaReal,
    List<LineaTiempoDto> LineaTiempo);