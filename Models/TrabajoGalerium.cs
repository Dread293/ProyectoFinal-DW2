using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class TrabajoGalerium
{
    public int Id { get; set; }

    public int CategoriaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Ciudad { get; set; }

    public DateOnly? FechaTrabajo { get; set; }

    public bool? Destacado { get; set; }

    public bool? Publicado { get; set; }

    public int? Orden { get; set; }

    public int? PedidoId { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual CategoriaTrabajo Categoria { get; set; } = null!;

    public virtual ICollection<MediaTrabajo> MediaTrabajos { get; set; } = new List<MediaTrabajo>();

    public virtual Pedido? Pedido { get; set; }
}
