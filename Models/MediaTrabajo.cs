using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class MediaTrabajo
{
    public int Id { get; set; }

    public int TrabajoId { get; set; }

    public string Tipo { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? MiniaturaUrl { get; set; }

    public bool? EsPortada { get; set; }

    public int? Orden { get; set; }

    public virtual TrabajoGalerium Trabajo { get; set; } = null!;
}
