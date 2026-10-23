using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string? Descripcion { get; set; }

    public string Estado { get; set; } = "pendiente";

    public DateTime? FechaRegistro { get; set; }

    public DateOnly? FechaEntregaEstimada { get; set; }

    public DateOnly? FechaEntregaReal { get; set; }

    public decimal? PrecioTotal { get; set; }

    public int? CategoriaId { get; set; }

    public virtual CategoriaTrabajo? Categoria { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();

    public virtual ICollection<PropuestaDiseno> PropuestaDisenos { get; set; } = new List<PropuestaDiseno>();

    public virtual ICollection<TrabajoGalerium> TrabajoGaleria { get; set; } = new List<TrabajoGalerium>();
}
