using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class PropuestaDiseno
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public string? Etiqueta { get; set; }

    public string ImagenUrl { get; set; } = null!;

    public int? SubidoPor { get; set; }

    public string Estado { get; set; } = "pendiente";

    public string? ComentarioCliente { get; set; }

    public DateTime? FechaSubida { get; set; }

    public DateTime? FechaRespuesta { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Usuario? SubidoPorNavigation { get; set; }
}
