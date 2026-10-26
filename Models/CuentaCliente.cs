using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class CuentaCliente
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
}
