namespace ScreenSound.Modelos;

public class Genero
{
    public int Id { get; set; }
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public virtual ICollection<Musica> Musicas { get; set; } // Relação muitos-para-muitos com Música - Virtual para lazy loading

    public override string ToString()
    {
        return $"Nome: {Nome}, Descrição: {Descricao}";
    }
}