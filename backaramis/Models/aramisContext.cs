using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace backaramis.Models
{
    public partial class aramisContext : DbContext
    {
        public aramisContext()
        {
        }

        public aramisContext(DbContextOptions<aramisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<ClienteGenero> ClienteGeneros { get; set; } = null!;
        public virtual DbSet<ClienteResponsabilidad> ClienteResponsabilidads { get; set; } = null!;
        public virtual DbSet<Documento> Documentos { get; set; } = null!;
        public virtual DbSet<DocumentoDetalle> DocumentoDetalles { get; set; } = null!;
        public virtual DbSet<DocumentoEstado> DocumentoEstados { get; set; } = null!;
        public virtual DbSet<DocumentoTipo> DocumentoTipos { get; set; } = null!;
        public virtual DbSet<Point> Points { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;
        public virtual DbSet<ProductoIva> ProductoIvas { get; set; } = null!;
        public virtual DbSet<ProductoRubro> ProductoRubros { get; set; } = null!;
        public virtual DbSet<ProveedorImputacion> ProveedorImputacions { get; set; } = null!;
        public virtual DbSet<Recibo> Recibos { get; set; } = null!;
        public virtual DbSet<ReciboDetalle> ReciboDetalles { get; set; } = null!;
        public virtual DbSet<SystemOption> SystemOptions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserLog> UserLogs { get; set; } = null!;
        public virtual DbSet<UserModule> UserModules { get; set; } = null!;
        public virtual DbSet<UserOperation> UserOperations { get; set; } = null!;
        public virtual DbSet<UserPerfil> UserPerfils { get; set; } = null!;
        public virtual DbSet<UserPerfilOperation> UserPerfilOperations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=RICARDO\\SERVER;Database=aramis;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");

                entity.HasIndex(e => e.Cuit, "KEY_Cliente_Cuit")
                    .IsUnique();

                entity.Property(e => e.Domicilio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Genero)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Masculino')");

                entity.Property(e => e.Imputacion).HasDefaultValueSql("((1))");

                entity.Property(e => e.LimiteSaldo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NombreFantasia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Responsabilidad)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Consumidor Final')");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('s/d')");

                entity.HasOne(d => d.GeneroNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.Genero)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Genero");

                entity.HasOne(d => d.ImputacionNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.Imputacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Imputacion");

                entity.HasOne(d => d.ResponsabilidadNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.Responsabilidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Responsabilidad");
            });

            modelBuilder.Entity<ClienteGenero>(entity =>
            {
                entity.ToTable("ClienteGenero");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClienteResponsabilidad>(entity =>
            {
                entity.ToTable("ClienteResponsabilidad");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Documento>(entity =>
            {
                entity.ToTable("Documento");

                entity.Property(e => e.Cai)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Creado).HasColumnType("datetime");

                entity.Property(e => e.Estado).HasDefaultValueSql("((1))");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(254)
                    .IsUnicode(false);

                entity.Property(e => e.Operador)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pago).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Razon)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vence).HasColumnType("datetime");

                entity.HasOne(d => d.EstadoNavigation)
                    .WithMany(p => p.Documentos)
                    .HasForeignKey(d => d.Estado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Documento_Estado");

                entity.HasOne(d => d.TipoNavigation)
                    .WithMany(p => p.Documentos)
                    .HasForeignKey(d => d.Tipo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Documento_Tipo");
            });

            modelBuilder.Entity<DocumentoDetalle>(entity =>
            {
                entity.ToTable("DocumentoDetalle");

                entity.Property(e => e.Cantidad).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Internos).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Iva).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Rubro)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Unitario).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<DocumentoEstado>(entity =>
            {
                entity.ToTable("DocumentoEstado");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DocumentoTipo>(entity =>
            {
                entity.ToTable("DocumentoTipo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Letra)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Point>(entity =>
            {
                entity.ToTable("Point");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Token).IsUnicode(false);

                entity.Property(e => e.Ubicacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Producto");

                entity.HasIndex(e => e.Codigo, "KEY_Producto_Codigo")
                    .IsUnique();

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Costo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Internos).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Stock).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IvaNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.Iva)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Producto_Iva");

                entity.HasOne(d => d.RubroNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.Rubro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Producto_Rubro");
            });

            modelBuilder.Entity<ProductoIva>(entity =>
            {
                entity.ToTable("ProductoIva");

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<ProductoRubro>(entity =>
            {
                entity.ToTable("ProductoRubro");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProveedorImputacion>(entity =>
            {
                entity.ToTable("ProveedorImputacion");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Recibo>(entity =>
            {
                entity.ToTable("Recibo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Operador)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ClienteNavigation)
                    .WithMany(p => p.Recibos)
                    .HasForeignKey(d => d.Cliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Recibo_Cliente");
            });

            modelBuilder.Entity<ReciboDetalle>(entity =>
            {
                entity.ToTable("ReciboDetalle");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Sucursal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ReciboNavigation)
                    .WithMany(p => p.ReciboDetalles)
                    .HasForeignKey(d => d.Recibo)
                    .HasConstraintName("FK_ReciboDetalle_Recibo");
            });

            modelBuilder.Entity<SystemOption>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Contacto)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cuit)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Domicilio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lote).HasDefaultValueSql("((1))");

                entity.Property(e => e.Razon)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.EndOfLife)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(dateadd(day,(-1),dateadd(hour,(-3),getdate())))");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Perfil).HasDefaultValueSql("((1))");

                entity.Property(e => e.Username).HasColumnName("Username ");

                entity.HasOne(d => d.PerfilNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Perfil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Perfil");
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.ToTable("UserLog");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Modulo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Operador)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserModule>(entity =>
            {
                entity.ToTable("UserModule");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Modulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("modulo");
            });

            modelBuilder.Entity<UserOperation>(entity =>
            {
                entity.ToTable("UserOperation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Modulo).HasColumnName("modulo");

                entity.Property(e => e.Operacion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("operacion");

                entity.HasOne(d => d.ModuloNavigation)
                    .WithMany(p => p.UserOperations)
                    .HasForeignKey(d => d.Modulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_seguridadOperacion_modulo");
            });

            modelBuilder.Entity<UserPerfil>(entity =>
            {
                entity.ToTable("UserPerfil");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Rol)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rol");
            });

            modelBuilder.Entity<UserPerfilOperation>(entity =>
            {
                entity.ToTable("UserPerfilOperation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdOperacion).HasColumnName("idOperacion");

                entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");

                entity.HasOne(d => d.IdOperacionNavigation)
                    .WithMany(p => p.UserPerfilOperations)
                    .HasForeignKey(d => d.IdOperacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPerfilOperation_idOperacion");

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.UserPerfilOperations)
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_seguridadPerfilOperacion_idPerfil");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
