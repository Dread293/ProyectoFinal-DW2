using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class HistorialEstado
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public string Estado { get; set; } = null!;

    public string? NotaPublica { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
