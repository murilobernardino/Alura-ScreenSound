namespace ScreenSound.Modelos;

public class Musica
{
    public Musica()
    {
        
    }
    public Musica(string nome)
    {
        Nome = nome;
    }
    public string Nome { get; set; }
    public int Id { get; set; }
    public int? AnoLancamento { get; set; }
    
    public int? ArtistaId { get; set; }
    public virtual Artista? Artista { get; set; } // Relação muitos-para-um com Artista - Virtual para lazy loading

    public virtual ICollection<Genero> Generos { get; set; } // Relação muitos-para-muitos com Gênero - Virtual para lazy loading

    public void ExibirFichaTecnica()
    {
        Console.WriteLine($"Nome: {Nome} - Ano de Lançamento: {AnoLancamento}");
    }

    public override string ToString()
    {
        return @$"Id: {Id}
        Nome: {Nome}";
    }
}