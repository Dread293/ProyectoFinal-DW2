using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual CuentaCliente? CuentaCliente { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
