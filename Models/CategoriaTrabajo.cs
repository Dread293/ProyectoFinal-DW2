using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class CategoriaTrabajo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? ImagenPortadaUrl { get; set; }

    public int? Orden { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<TrabajoGalerium> TrabajoGaleria { get; set; } = new List<TrabajoGalerium>();
}
