using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppPartes.Data.Models
{
    public partial class AldakinDbContext : DbContext
    {
        public AldakinDbContext()
        {
        }

        public AldakinDbContext(DbContextOptions<AldakinDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Acceso> Acceso { get; set; }
        public virtual DbSet<Administracion> Administracion { get; set; }
        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Correo> Correo { get; set; }
        public virtual DbSet<Diasfestivos> Diasfestivos { get; set; }
        public virtual DbSet<Entidad> Entidad { get; set; }
        public virtual DbSet<Estadodias> Estadodias { get; set; }
        public virtual DbSet<Gastos> Gastos { get; set; }
        public virtual DbSet<Grupopiloto> Grupopiloto { get; set; }
        public virtual DbSet<Ids> Ids { get; set; }
        public virtual DbSet<Lineas> Lineas { get; set; }
        public virtual DbSet<Lineasoriginales> Lineasoriginales { get; set; }
        public virtual DbSet<Logcambios> Logcambios { get; set; }
        public virtual DbSet<Logdesvalidacion> Logdesvalidacion { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Mensajes> Mensajes { get; set; }
        public virtual DbSet<Name> Name { get; set; }
        public virtual DbSet<Ots> Ots { get; set; }
        public virtual DbSet<Pernoctaciones> Pernoctaciones { get; set; }
        public virtual DbSet<Preslin> Preslin { get; set; }
        public virtual DbSet<Presupuestos> Presupuestos { get; set; }
        public virtual DbSet<Pruebadiasfestivos> Pruebadiasfestivos { get; set; }
        public virtual DbSet<Responsables> Responsables { get; set; }
        public virtual DbSet<Servicios> Servicios { get; set; }
        public virtual DbSet<Tipoexcel> Tipoexcel { get; set; }
        public virtual DbSet<Tipogastos> Tipogastos { get; set; }
        public virtual DbSet<Tipohora> Tipohora { get; set; }
        public virtual DbSet<Udobrapresu> Udobrapresu { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Vacaciones> Vacaciones { get; set; }
        public virtual DbSet<Vacacionespendientes> Vacacionespendientes { get; set; }
        public virtual DbSet<Versiones> Versiones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acceso>(entity =>
            {
                entity.HasKey(e => e.Nivel)
                    .HasName("PRIMARY");

                entity.ToTable("acceso");

                entity.Property(e => e.Nivel)
                    .HasColumnName("nivel")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Administracion>(entity =>
            {
                entity.HasKey(e => e.Idusuario)
                    .HasName("PRIMARY");

                entity.ToTable("administracion");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Clientes>(entity =>
            {
                entity.HasKey(e => e.Idclientes)
                    .HasName("PRIMARY");

                entity.ToTable("clientes");

                entity.Property(e => e.Idclientes)
                    .HasColumnName("idclientes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Codigo)
                    .HasColumnName("codigo")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Correo>(entity =>
            {
                entity.HasKey(e => new { e.Idusuario, e.Semana, e.Mes, e.Año })
                    .HasName("PRIMARY");

                entity.ToTable("correo");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Semana)
                    .HasColumnName("semana")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mes)
                    .HasColumnName("mes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Año)
                    .HasColumnName("año")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Enviado).HasColumnName("enviado");

                entity.Property(e => e.Fecha).HasColumnName("fecha");

                entity.Property(e => e.Validado).HasColumnName("validado");
            });

            modelBuilder.Entity<Diasfestivos>(entity =>
            {
                entity.HasKey(e => e.Idfestivos)
                    .HasName("PRIMARY");

                entity.ToTable("diasfestivos");

                entity.Property(e => e.Idfestivos)
                    .HasColumnName("idfestivos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Calendario)
                    .HasColumnName("calendario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dia)
                    .HasColumnName("dia")
                    .HasColumnType("date");

                entity.Property(e => e.Jornadareducida).HasColumnName("jornadareducida");
            });

            modelBuilder.Entity<Entidad>(entity =>
            {
                entity.HasKey(e => e.CodEnt)
                    .HasName("PRIMARY");

                entity.ToTable("entidad");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Estadodias>(entity =>
            {
                entity.ToTable("estadodias");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dia)
                    .HasColumnName("dia")
                    .HasColumnType("date");

                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Horas).HasColumnName("horas");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Gastos>(entity =>
            {
                entity.HasKey(e => e.Idgastos)
                    .HasName("PRIMARY");

                entity.ToTable("gastos");

                entity.HasIndex(e => e.Tipo)
                    .HasName("Tipo_idx");

                entity.Property(e => e.Idgastos)
                    .HasColumnName("idgastos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Cantidad).HasColumnName("cantidad");

                entity.Property(e => e.Idlinea)
                    .HasColumnName("idlinea")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Observacion)
                    .HasColumnName("observacion")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Pagador)
                    .HasColumnName("pagador")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.TipoNavigation)
                    .WithMany(p => p.Gastos)
                    .HasForeignKey(d => d.Tipo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Tipo");
            });

            modelBuilder.Entity<Grupopiloto>(entity =>
            {
                entity.HasKey(e => e.Idusuario)
                    .HasName("PRIMARY");

                entity.ToTable("grupopiloto");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Ids>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ids");

                entity.Property(e => e.Ids1)
                    .HasColumnName("ids")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Lineas>(entity =>
            {
                entity.HasKey(e => e.Idlinea)
                    .HasName("PRIMARY");

                entity.ToTable("lineas");

                entity.HasIndex(e => e.Idot)
                    .HasName("OT_idx");

                entity.HasIndex(e => e.Idpreslin)
                    .HasName("preslin_idx");

                entity.HasIndex(e => e.Idusuario)
                    .HasName("idUsuario_idx");

                entity.Property(e => e.Idlinea)
                    .HasColumnName("idlinea")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dietas).HasColumnName("dietas");

                entity.Property(e => e.Facturable)
                    .HasColumnName("facturable")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Fin).HasColumnName("fin");

                entity.Property(e => e.Horas).HasColumnName("horas");

                entity.Property(e => e.Horasviaje)
                    .HasColumnName("horasviaje")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Idoriginal)
                    .HasColumnName("idoriginal")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Idot)
                    .HasColumnName("idot")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idpreslin)
                    .HasColumnName("idpreslin")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inicio).HasColumnName("inicio");

                entity.Property(e => e.Km)
                    .HasColumnName("km")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Npartefirmado)
                    .HasColumnName("npartefirmado")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Observaciones)
                    .IsRequired()
                    .HasColumnName("observaciones")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Registrado)
                    .HasColumnName("registrado")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Validado)
                    .HasColumnName("validado")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Validador)
                    .IsRequired()
                    .HasColumnName("validador")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.IdotNavigation)
                    .WithMany(p => p.Lineas)
                    .HasForeignKey(d => d.Idot)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ot");

                entity.HasOne(d => d.IdpreslinNavigation)
                    .WithMany(p => p.Lineas)
                    .HasForeignKey(d => d.Idpreslin)
                    .HasConstraintName("preslin");

                entity.HasOne(d => d.IdusuarioNavigation)
                    .WithMany(p => p.Lineas)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idUsuario");
            });

            modelBuilder.Entity<Lineasoriginales>(entity =>
            {
                entity.HasKey(e => e.Idlinea)
                    .HasName("PRIMARY");

                entity.ToTable("lineasoriginales");

                entity.Property(e => e.Idlinea)
                    .HasColumnName("idlinea")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dietas).HasColumnName("dietas");

                entity.Property(e => e.Facturable)
                    .HasColumnName("facturable")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Fin).HasColumnName("fin");

                entity.Property(e => e.Horas).HasColumnName("horas");

                entity.Property(e => e.Horasviaje)
                    .HasColumnName("horasviaje")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Idot)
                    .HasColumnName("idot")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idpreslin)
                    .HasColumnName("idpreslin")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inicio).HasColumnName("inicio");

                entity.Property(e => e.Km)
                    .HasColumnName("km")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Npartefirmado)
                    .HasColumnName("npartefirmado")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Observaciones)
                    .IsRequired()
                    .HasColumnName("observaciones")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Logcambios>(entity =>
            {
                entity.HasKey(e => e.Idlogpermisos)
                    .HasName("PRIMARY");

                entity.ToTable("logcambios");

                entity.Property(e => e.Idlogpermisos)
                    .HasColumnName("idlogpermisos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Autor)
                    .IsRequired()
                    .HasColumnName("autor")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Momento).HasColumnName("momento");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Logdesvalidacion>(entity =>
            {
                entity.HasKey(e => e.Idlogdesvalidacion)
                    .HasName("PRIMARY");

                entity.ToTable("logdesvalidacion");

                entity.Property(e => e.Idlogdesvalidacion)
                    .HasColumnName("idlogdesvalidacion")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Autor)
                    .HasColumnName("autor")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Fecha).HasColumnName("fecha");

                entity.Property(e => e.Idlinea)
                    .HasColumnName("idlinea")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.HasKey(e => e.Idlog)
                    .HasName("PRIMARY");

                entity.ToTable("logs");

                entity.Property(e => e.Idlog)
                    .HasColumnName("idlog")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Clase)
                    .IsRequired()
                    .HasColumnName("clase")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Custommessage)
                    .IsRequired()
                    .HasColumnName("custommessage")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Device)
                    .IsRequired()
                    .HasColumnName("device")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DeviceUser)
                    .IsRequired()
                    .HasColumnName("device_user")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Domain)
                    .IsRequired()
                    .HasColumnName("domain")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Metodo)
                    .IsRequired()
                    .HasColumnName("metodo")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Mensajes>(entity =>
            {
                entity.HasKey(e => e.Idmensajes)
                    .HasName("PRIMARY");

                entity.ToTable("mensajes");

                entity.Property(e => e.Idmensajes)
                    .HasColumnName("idmensajes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.A)
                    .HasColumnName("a")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Asunto)
                    .IsRequired()
                    .HasColumnName("asunto")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.De)
                    .HasColumnName("de")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.Fecha).HasColumnName("fecha");

                entity.Property(e => e.Idlinea)
                    .HasColumnName("idlinea")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inicial)
                    .HasColumnName("inicial")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mensaje)
                    .IsRequired()
                    .HasColumnName("mensaje")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Vistodestino).HasColumnName("vistodestino");

                entity.Property(e => e.Vistoremite).HasColumnName("vistoremite");
            });

            modelBuilder.Entity<Name>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("name");

                entity.Property(e => e.Name1)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Ots>(entity =>
            {
                entity.HasKey(e => e.Idots)
                    .HasName("PRIMARY");

                entity.ToTable("ots");

                entity.HasIndex(e => e.Cliente)
                    .HasName("Cliente_idx");

                entity.HasIndex(e => e.CodEnt)
                    .HasName("ENTIDAD_idx");

                entity.Property(e => e.Idots)
                    .HasColumnName("idots")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Apertura)
                    .HasColumnName("apertura")
                    .HasColumnType("date");

                entity.Property(e => e.Cierre)
                    .HasColumnName("cierre")
                    .HasColumnType("date");

                entity.Property(e => e.CkCapitulo)
                    .IsRequired()
                    .HasColumnName("ck_capitulo")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Cliente)
                    .HasColumnName("cliente")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEntD)
                    .HasColumnName("cod_ent_d")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Codigorefot)
                    .IsRequired()
                    .HasColumnName("codigorefot")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Creador)
                    .HasColumnName("creador")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FacDietas).HasColumnName("fac_dietas");

                entity.Property(e => e.FacKm).HasColumnName("fac_km");

                entity.Property(e => e.FacViajes).HasColumnName("fac_viajes");

                entity.Property(e => e.MaxDietas).HasColumnName("max_dietas");

                entity.Property(e => e.MaxKm).HasColumnName("max_km");

                entity.Property(e => e.MaxViajes).HasColumnName("max_viajes");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Numero)
                    .HasColumnName("numero")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Responsable)
                    .HasColumnName("responsable")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Responsable1)
                    .HasColumnName("responsable1")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Responsable2)
                    .HasColumnName("responsable2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tipoot)
                    .HasColumnName("tipoot")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ClienteNavigation)
                    .WithMany(p => p.Ots)
                    .HasForeignKey(d => d.Cliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Cliente");

                entity.HasOne(d => d.CodEntNavigation)
                    .WithMany(p => p.Ots)
                    .HasForeignKey(d => d.CodEnt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ENTIDAD");
            });

            modelBuilder.Entity<Pernoctaciones>(entity =>
            {
                entity.HasKey(e => new { e.CodEnt, e.Tipo })
                    .HasName("PRIMARY");

                entity.ToTable("pernoctaciones");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Articulo)
                    .IsRequired()
                    .HasColumnName("articulo")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Coste).HasColumnName("coste");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Preslin>(entity =>
            {
                entity.HasKey(e => e.Idpreslin)
                    .HasName("PRIMARY");

                entity.ToTable("preslin");

                entity.HasIndex(e => e.Idpresupuesto)
                    .HasName("Linea-Presupuesto_idx");

                entity.Property(e => e.Idpreslin)
                    .HasColumnName("idpreslin")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Anexo)
                    .HasColumnName("anexo")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CodhPes)
                    .HasColumnName("codh_pes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodpPes)
                    .HasColumnName("codp_pes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Horas)
                    .HasColumnName("horas")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Idpresupuesto)
                    .HasColumnName("idpresupuesto")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nivel)
                    .HasColumnName("nivel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Numero)
                    .HasColumnName("numero")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RefiUobra)
                    .HasColumnName("refi_uobra")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tipohora)
                    .HasColumnName("tipohora")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdpresupuestoNavigation)
                    .WithMany(p => p.Preslin)
                    .HasForeignKey(d => d.Idpresupuesto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Linea-Presupuesto");
            });

            modelBuilder.Entity<Presupuestos>(entity =>
            {
                entity.HasKey(e => e.Idpresupuestos)
                    .HasName("PRIMARY");

                entity.ToTable("presupuestos");

                entity.HasIndex(e => e.Idot)
                    .HasName("idOT_idx");

                entity.Property(e => e.Idpresupuestos)
                    .HasColumnName("idpresupuestos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idot)
                    .HasColumnName("idot")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Numero)
                    .HasColumnName("numero")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdotNavigation)
                    .WithMany(p => p.Presupuestos)
                    .HasForeignKey(d => d.Idot)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("idOT");
            });

            modelBuilder.Entity<Pruebadiasfestivos>(entity =>
            {
                entity.HasKey(e => e.Idfestivos)
                    .HasName("PRIMARY");

                entity.ToTable("pruebadiasfestivos");

                entity.Property(e => e.Idfestivos)
                    .HasColumnName("idfestivos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Calendario)
                    .HasColumnName("calendario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dia)
                    .HasColumnName("dia")
                    .HasColumnType("date");

                entity.Property(e => e.Jornadareducida).HasColumnName("jornadareducida");
            });

            modelBuilder.Entity<Responsables>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("responsables");

                entity.Property(e => e.Autor)
                    .IsRequired()
                    .HasColumnName("autor")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Ots)
                    .IsRequired()
                    .HasColumnName("ots")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PersonasName)
                    .IsRequired()
                    .HasColumnName("personas_name")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Servicios>(entity =>
            {
                entity.ToTable("servicios");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Condicion)
                    .IsRequired()
                    .HasColumnName("condicion")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Ejecutar)
                    .HasColumnName("ejecutar")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Tipoexcel>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PRIMARY");

                entity.ToTable("tipoexcel");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Tipogastos>(entity =>
            {
                entity.HasKey(e => e.Idtipogastos)
                    .HasName("PRIMARY");

                entity.ToTable("tipogastos");

                entity.Property(e => e.Idtipogastos)
                    .HasColumnName("idtipogastos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodArt)
                    .IsRequired()
                    .HasColumnName("cod_art")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Pagador)
                    .HasColumnName("pagador")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasColumnName("tipo")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Tipohora>(entity =>
            {
                entity.HasKey(e => new { e.CodEnt, e.RefOt })
                    .HasName("PRIMARY");

                entity.ToTable("tipohora");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RefOt)
                    .HasColumnName("ref_ot")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("text")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Festivo)
                    .HasColumnName("festivo")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Noche)
                    .HasColumnName("noche")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Normal)
                    .HasColumnName("normal")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sabado)
                    .HasColumnName("sabado")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Udobrapresu>(entity =>
            {
                entity.HasKey(e => e.IdudObraPresu)
                    .HasName("PRIMARY");

                entity.ToTable("udobrapresu");

                entity.Property(e => e.IdudObraPresu)
                    .HasColumnName("idudObraPresu")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar(126)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.RefiPes)
                    .HasColumnName("refi_pes")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("user");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(e => e.Idusuario)
                    .HasName("PRIMARY");

                entity.ToTable("usuarios");

                entity.HasIndex(e => e.Autorizacion)
                    .HasName("ACCESO_idx");

                entity.HasIndex(e => e.CodEnt)
                    .HasName("DELEGACION_idx");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Autorizacion)
                    .HasColumnName("autorizacion")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Baja)
                    .HasColumnName("baja")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Calendario)
                    .HasColumnName("calendario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEmpl)
                    .HasColumnName("cod_empl")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEntO)
                    .HasColumnName("cod_ent_o")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Idcategoria)
                    .HasColumnName("idcategoria")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Incorporacion)
                    .HasColumnName("incorporacion")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Nombrecompleto)
                    .IsRequired()
                    .HasColumnName("nombrecompleto")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.CodEntNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.CodEnt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DELEGACION");
            });

            modelBuilder.Entity<Vacaciones>(entity =>
            {
                entity.HasKey(e => e.Idvacaciones)
                    .HasName("PRIMARY");

                entity.ToTable("vacaciones");

                entity.Property(e => e.Idvacaciones)
                    .HasColumnName("idvacaciones")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Año)
                    .HasColumnName("año")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CodEnt)
                    .HasColumnName("cod_ent")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Horas).HasColumnName("horas");
            });

            modelBuilder.Entity<Vacacionespendientes>(entity =>
            {
                entity.HasKey(e => e.Idusuario)
                    .HasName("PRIMARY");

                entity.ToTable("vacacionespendientes");

                entity.Property(e => e.Idusuario)
                    .HasColumnName("idusuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Año)
                    .HasColumnName("año")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Horas).HasColumnName("horas");
            });

            modelBuilder.Entity<Versiones>(entity =>
            {
                entity.HasKey(e => e.Idfichero)
                    .HasName("PRIMARY");

                entity.ToTable("versiones");

                entity.Property(e => e.Idfichero)
                    .HasColumnName("idfichero")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasColumnName("version")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
