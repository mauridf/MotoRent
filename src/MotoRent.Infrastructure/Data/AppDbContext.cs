using Microsoft.EntityFrameworkCore;
using MotoRent.Domain.Entities;

namespace MotoRent.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<FotoDocumento> FotosDocumentos { get; set; }
        public DbSet<Atendente> Atendentes { get; set; }
        public DbSet<HabilitarEntregador> HabilitacoesEntregadores { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Moto> Motos { get; set; }
        public DbSet<FotoMoto> FotosMotos { get; set; }
        public DbSet<Manutencao> Manutencoes { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Filtro global para soft delete
            modelBuilder.Entity<BaseEntity>().HasQueryFilter(e => !e.IsDeleted);

            // Configurações de User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // Relacionamento opcional com Entregador
                entity.HasOne<Entregador>()
                      .WithOne()
                      .HasForeignKey<User>(u => u.EntregadorId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento opcional com Atendente
                entity.HasOne<Atendente>()
                      .WithOne()
                      .HasForeignKey<User>(u => u.AtendenteId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurações de Entregador
            modelBuilder.Entity<Entregador>(entity =>
            {
                entity.HasIndex(e => e.CPF).IsUnique();
                entity.HasIndex(e => e.CNH).IsUnique();

                // Garantir que a categoria da CNH seja A ou AB
                entity.Property(e => e.CategoriaCNH)
                      .HasMaxLength(2)
                      .IsRequired();

                // Check constraint para PostgreSQL
                entity.ToTable(t => t.HasCheckConstraint("CK_Entregador_CategoriaCNH", "\"CategoriaCNH\" IN ('A', 'AB')"));

                // Relacionamento com User (1:1)
                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<User>(u => u.EntregadorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações de FotoDocumento
            modelBuilder.Entity<FotoDocumento>(entity =>
            {
                entity.HasOne(f => f.Entregador)
                      .WithMany(e => e.Documentos)
                      .HasForeignKey(f => f.EntregadorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(f => f.Tipo)
                      .HasMaxLength(50);
            });

            // Configurações de Atendente
            modelBuilder.Entity<Atendente>(entity =>
            {
                entity.HasIndex(a => a.CPF).IsUnique();

                // Relacionamento com User (1:1)
                entity.HasOne(a => a.User)
                      .WithOne()
                      .HasForeignKey<User>(u => u.AtendenteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações de HabilitarEntregador
            modelBuilder.Entity<HabilitarEntregador>(entity =>
            {
                entity.HasOne(h => h.Atendente)
                      .WithMany()
                      .HasForeignKey(h => h.AtendenteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Entregador)
                      .WithMany()
                      .HasForeignKey(h => h.EntregadorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(h => h.Observacao)
                      .HasMaxLength(500);
            });

            // Configurações de Marca
            modelBuilder.Entity<Marca>(entity =>
            {
                entity.HasIndex(m => m.Nome).IsUnique();
                entity.Property(m => m.Nome).HasMaxLength(100);
            });

            // Configurações de Modelo
            modelBuilder.Entity<Modelo>(entity =>
            {
                entity.HasOne(m => m.Marca)
                      .WithMany(m => m.Modelos)
                      .HasForeignKey(m => m.MarcaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.Nome).HasMaxLength(100);

                // Índice composto para evitar modelos duplicados na mesma marca
                entity.HasIndex(m => new { m.MarcaId, m.Nome }).IsUnique();
            });

            // Configurações de Moto
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasIndex(m => m.Placa).IsUnique();
                entity.HasIndex(m => m.Renavam).IsUnique();
                entity.HasIndex(m => m.Chassi).IsUnique();

                entity.HasOne(m => m.Modelo)
                      .WithMany(m => m.Motos)
                      .HasForeignKey(m => m.ModeloId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configurações de enumerações
                entity.Property(m => m.Status)
                      .HasMaxLength(20);

                entity.Property(m => m.TipoMotor)
                      .HasMaxLength(20);

                entity.Property(m => m.TipoCombustivel)
                      .HasMaxLength(20);

                entity.Property(m => m.TipoSeguro)
                      .HasMaxLength(20);

                // Configurações de tamanho para campos string
                entity.Property(m => m.Placa).HasMaxLength(10);
                entity.Property(m => m.Renavam).HasMaxLength(20);
                entity.Property(m => m.Chassi).HasMaxLength(30);
                entity.Property(m => m.Cor).HasMaxLength(50);
                entity.Property(m => m.ObservacoesDocumentacao).HasMaxLength(500);

                // Configurações para campos decimais (especificando precisão para PostgreSQL)
                entity.Property(m => m.CapacidadeTanque).HasPrecision(5, 2);
                entity.Property(m => m.Peso).HasPrecision(7, 2);
                entity.Property(m => m.Quilometragem).HasPrecision(10, 2);
                entity.Property(m => m.ValorAquisicao).HasPrecision(12, 2);
                entity.Property(m => m.ValorLocacaoDiaria).HasPrecision(8, 2);
                entity.Property(m => m.ValorCaucao).HasPrecision(8, 2);
            });

            // Configurações de FotoMoto
            modelBuilder.Entity<FotoMoto>(entity =>
            {
                entity.HasOne(f => f.Moto)
                      .WithMany(m => m.Fotos)
                      .HasForeignKey(f => f.MotoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(f => f.Tipo)
                      .HasMaxLength(50);
            });

            // Configurações de Manutencao
            modelBuilder.Entity<Manutencao>(entity =>
            {
                entity.HasOne(m => m.Moto)
                      .WithMany(m => m.Manutencoes)
                      .HasForeignKey(m => m.MotoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(m => m.Tipo)
                      .HasMaxLength(20);

                entity.Property(m => m.Descricao).HasMaxLength(500);
                entity.Property(m => m.Valor).HasPrecision(10, 2);
                entity.Property(m => m.Km).HasPrecision(10, 2);
            });

            // Configurações de Locacao
            modelBuilder.Entity<Locacao>(entity =>
            {
                entity.HasOne(l => l.Moto)
                      .WithMany(m => m.Locacoes)
                      .HasForeignKey(l => l.MotoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Entregador)
                      .WithMany(e => e.Locacoes)
                      .HasForeignKey(l => l.EntregadorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(l => l.Status)
                      .HasMaxLength(20);

                entity.Property(l => l.ValorTotal).HasPrecision(10, 2);

                // Validação para datas
                entity.ToTable(t => t.HasCheckConstraint("CK_Locacao_Datas",
                    "\"DataFimPrevista\" > \"DataInicio\" AND (\"DataFimReal\" IS NULL OR \"DataFimReal\" >= \"DataInicio\")"));
            });

            // Configurações de Reserva
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasOne(r => r.Moto)
                      .WithMany(m => m.Reservas)
                      .HasForeignKey(r => r.MotoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Entregador)
                      .WithMany(e => e.Reservas)
                      .HasForeignKey(r => r.EntregadorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.Status)
                      .HasMaxLength(20);

                // Validação para datas
                entity.ToTable(t => t.HasCheckConstraint("CK_Reserva_Datas", "\"DataFim\" > \"DataInicio\""));
            });
        }
    }
}