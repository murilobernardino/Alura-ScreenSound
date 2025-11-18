using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Modelos;
using ScreenSound.Banco;

namespace ScreenSound.API.Endpoints;

public static class MusicasExtensions
{
    public static void AddEndPointsMusicas(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("musicas").RequireAuthorization().WithTags("Musicas");
        
        groupBuilder.MapGet("", ([FromServices] DAL<Musica> dal) => // Injeção de dependência do DAL<Musica>
        {
            var musicaList = dal.Listar();
            if (musicaList is null)
            {
                return Results.NotFound();
            }
            var musicaResponseList = EntityListToResponseList(musicaList);
            return Results.Ok(musicaResponseList);
        });

        groupBuilder.MapGet("{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
        {
            var musica = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (musica is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(musica);

        });

        groupBuilder.MapPost("", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) => // Recebe a música no corpo da requisição / Injeção de dependência do DAL<Musica>
        {
            var musica = new Musica(musicaRequest.nome)
            {
                ArtistaId = musicaRequest.ArtistaId,
                AnoLancamento = musicaRequest.anoLancamento,
                Generos = musicaRequest.Generos is not null? GeneroRequestConverter(musicaRequest.Generos, dalGenero) : new List<Genero>()
            };
            
            dal.Adicionar(musica);
            return Results.Ok();
        });

        groupBuilder.MapPut("", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaRequestEdit) => // Recebe a música no corpo da requisição / Injeção de dependência do DAL<Musica>
        {
            var musicaAtualizar = dal.RecuperarPor(m => m.Id == musicaRequestEdit.Id);
            if (musicaAtualizar is null)
            {
                return Results.NotFound();
            }
            musicaAtualizar.Nome = musicaRequestEdit.nome;
            musicaAtualizar.AnoLancamento = musicaRequestEdit.anoLancamento;
            dal.Atualizar(musicaAtualizar);
    
            return Results.Ok();
        });

        groupBuilder.MapDelete("{id}", ([FromServices] DAL<Musica> dal, int id) => // Rota com parâmetro id / Injeção de dependência do DAL<Musica>
        {
            var musica = dal.RecuperarPor(m => m.Id == id);
            if (musica is null)
            {
                return Results.NotFound();
            }

            dal.Deletar(musica);
            return Results.NoContent();
        });
    }

    private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
    {
        return musicaList.Select(a => EntityToResponse(a)).ToList();
    }
    private static MusicaResponse EntityToResponse(Musica musica)
    {
        return new MusicaResponse(
            musica.Id,
            musica.Nome!,
            musica.Artista?.Id ?? 0,           // 0 ou outro valor default
            musica.Artista?.Nome ?? "Desconhecido"
        );
    }
    private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
    {
        var listaDeGeneros = new List<Genero>();
        foreach (var item in generos)
        {
            var entity = RequestToEntity(item);
            var genero = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(item.Nome.ToUpper()));
            if (genero is not null)
            {
                listaDeGeneros.Add(genero);
            }
            else
            {
                listaDeGeneros.Add(entity);
            }
        }
        return listaDeGeneros;
    }
    private static Genero RequestToEntity(GeneroRequest genero)
    {
        return new Genero()
        {
            Nome = genero.Nome,
            Descricao = genero.Descricao
        };
    }
}