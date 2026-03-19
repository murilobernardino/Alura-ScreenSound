using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScreenSound.Modelos;
using ScreenSound.Shared.Dados.Modelos;


namespace ScreenSound.Banco;

public class ScreenSoundContext : IdentityDbContext<PessoaComAcesso, PerfilDeAcesso, int>
{
    public DbSet<Artista> Artistas { get; set; } = null!; //DbSet representa uma coleção de todas as entidades no contexto, ou que podem ser consultadas a partir do banco de dados, do tipo especificado. No seu caso, Artista.
    public DbSet<Musica> Musicas { get; set; } = null!;
    public DbSet<Genero> Generos { get; set; } = null!;
    
    private string connectionString = @"Data Source=localhost,1433;Initial Catalog=aluraV0;User Id=sa;Password=hUmV3bJjALTA9tWehwBh;Encrypt=False;TrustServerCertificate=True";
    //private string connectionString = "Server=screensound-server-murilo.database.windows.net;Database=ScreenSoundV0;User Id=screensound;Password=senha@123;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
    public ScreenSoundContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }
        optionsBuilder
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies();
        // UseLazyLoadingProxies permite o carregamento tardio (lazy loading) das
        // entidades relacionadas, ou seja, as entidades relacionadas só são carregadas do
        // banco de dados quando são acessadas pela primeira vez.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Configurações adicionais do modelo
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Musica>()
            .HasMany(c => c.Generos)// Configuração da relação muitos-para-muitos entre Música e Gênero
            .WithMany(c => c.Musicas);// Configuração da relação muitos-para-muitos entre Música e Gênero
    }
}