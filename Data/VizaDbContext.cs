using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public partial class VizaDbContext : DbContext
{
    public VizaDbContext(DbContextOptions<VizaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoriaTrabajo> CategoriaTrabajos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<CuentaCliente> CuentaClientes { get; set; }

    public virtual DbSet<HistorialEstado> HistorialEstados { get; set; }

    public virtual DbSet<MediaTrabajo> MediaTrabajos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<PropuestaDiseno> PropuestaDisenos { get; set; }

    public virtual DbSet<TrabajoGalerium> TrabajoGaleria { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoriaTrabajo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_trabajo_pkey");

            entity.ToTable("categoria_trabajo");

            entity.HasIndex(e => e.Slug, "categoria_trabajo_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.ImagenPortadaUrl)
                .HasMaxLength(500)
                .HasColumnName("imagen_portada_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cliente_pkey");

            entity.ToTable("cliente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(30)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<CuentaCliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cuenta_cliente_pkey");

            entity.ToTable("cuenta_cliente");

            entity.HasIndex(e => e.ClienteId, "cuenta_cliente_cliente_id_key").IsUnique();

            entity.HasIndex(e => e.Correo, "cuenta_cliente_correo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UltimoAcceso)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ultimo_acceso");

            entity.HasOne(d => d.Cliente).WithOne(p => p.CuentaCliente)
                .HasForeignKey<CuentaCliente>(d => d.ClienteId)
                .HasConstraintName("cuenta_cliente_cliente_id_fkey");
        });

        modelBuilder.Entity<HistorialEstado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("historial_estado_pkey");

            entity.ToTable("historial_estado");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.NotaPublica)
                .HasMaxLength(255)
                .HasColumnName("nota_publica");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Pedido).WithMany(p => p.HistorialEstados)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("historial_estado_pedido_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialEstados)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("historial_estado_usuario_id_fkey");
        });

        modelBuilder.Entity<MediaTrabajo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("media_trabajo_pkey");

            entity.ToTable("media_trabajo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .HasColumnName("tipo");
            entity.Property(e => e.EsPortada)
                .HasDefaultValue(false)
                .HasColumnName("es_portada");
            entity.Property(e => e.MiniaturaUrl)
                .HasMaxLength(500)
                .HasColumnName("miniatura_url");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");
            entity.Property(e => e.TrabajoId).HasColumnName("trabajo_id");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("url");

            entity.HasOne(d => d.Trabajo).WithMany(p => p.MediaTrabajos)
                .HasForeignKey(d => d.TrabajoId)
                .HasConstraintName("media_trabajo_trabajo_id_fkey");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pedido_pkey");

            entity.ToTable("pedido");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEntregaEstimada).HasColumnName("fecha_entrega_estimada");
            entity.Property(e => e.FechaEntregaReal).HasColumnName("fecha_entrega_real");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.PrecioTotal)
                .HasPrecision(12, 2)
                .HasColumnName("precio_total");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("pedido_categoria_id_fkey");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_cliente_id_fkey");
        });

        modelBuilder.Entity<PropuestaDiseno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("propuesta_diseno_pkey");

            entity.ToTable("propuesta_diseno");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComentarioCliente).HasColumnName("comentario_cliente");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.Etiqueta)
                .HasMaxLength(100)
                .HasColumnName("etiqueta");
            entity.Property(e => e.FechaRespuesta)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_respuesta");
            entity.Property(e => e.FechaSubida)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_subida");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .HasColumnName("imagen_url");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.SubidoPor).HasColumnName("subido_por");

            entity.HasOne(d => d.Pedido).WithMany(p => p.PropuestaDisenos)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("propuesta_diseno_pedido_id_fkey");

            entity.HasOne(d => d.SubidoPorNavigation).WithMany(p => p.PropuestaDisenos)
                .HasForeignKey(d => d.SubidoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("propuesta_diseno_subido_por_fkey");
        });

        modelBuilder.Entity<TrabajoGalerium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trabajo_galeria_pkey");

            entity.ToTable("trabajo_galeria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(100)
                .HasColumnName("ciudad");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Destacado)
                .HasDefaultValue(false)
                .HasColumnName("destacado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaTrabajo).HasColumnName("fecha_trabajo");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.Publicado)
                .HasDefaultValue(true)
                .HasColumnName("publicado");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");

            entity.HasOne(d => d.Categoria).WithMany(p => p.TrabajoGaleria)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trabajo_galeria_categoria_id_fkey");

            entity.HasOne(d => d.Pedido).WithMany(p => p.TrabajoGaleria)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("trabajo_galeria_pedido_id_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasColumnName("rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
