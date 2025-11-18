using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Modelos;
using ScreenSound.Banco;

namespace ScreenSound.API.Endpoints;

public static class ArtistasExtensions
{
    public static void AddEndPointsArtistas(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("artistas").RequireAuthorization().WithTags("Artistas");
        
        groupBuilder.MapGet("", ([FromServices] DAL<Artista> dal) => // Injeção de dependência do DAL<Artista>
        {
            var listaDeArtistas = dal.Listar();
            if (listaDeArtistas is null)
            {
                return Results.NotFound();
            }
            var artistaResponseList = EntityListToResponseList(listaDeArtistas);
            return Results.Ok(artistaResponseList);
        });

        groupBuilder.MapGet("{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
        {
            var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (artista is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(EntityToResponse(artista));

        });

        groupBuilder.MapPost("", async ([FromServices] IHostEnvironment env, [FromServices] DAL<Artista> dal, [FromBody]ArtistaRequest artistaRequest) => // Recebe o artista no corpo da requisição / Injeção de dependência do DAL<Artista>
        {
            var nome = artistaRequest.nome.Trim();
            var imagemArtista = DateTime.Now.ToString("ddMMyyyyhhss") + "." + nome + ".jpeg";
            
            var path = Path.Combine(env.ContentRootPath, "wwwroot", "FotosPerfil", imagemArtista);
            
            using MemoryStream ms = new MemoryStream(Convert.FromBase64String(artistaRequest.fotoPerfil!));
            using FileStream fs = new FileStream(path, FileMode.Create);
            await ms.CopyToAsync(fs);

            var artista = new Artista(artistaRequest.nome, artistaRequest.bio)
            {
                FotoPerfil = $"/FotosPerfil/{imagemArtista}"
            };
            dal.Adicionar(artista);
            return Results.Ok();
        });

        groupBuilder.MapDelete("{id}", ([FromServices] DAL<Artista> dal, int id) => // Rota com parâmetro id / Injeção de dependência do DAL<Artista>
        {
            var artista = dal.RecuperarPor(a => a.Id == id);
            if (artista is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(artista);
            return Results.NoContent();
        });

        groupBuilder.MapPut("", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaRequestEdit) => // Recebe o artista no corpo da requisição / Injeção de dependência do DAL<Artista>)
        {
            var artistaAtualizar = dal.RecuperarPor(a => a.Id == artistaRequestEdit.Id);
            if (artistaAtualizar is null)
            {
                return Results.NotFound();
            }
            artistaAtualizar.Nome = artistaRequestEdit.nome;
            artistaAtualizar.Bio = artistaRequestEdit.bio;
            dal.Atualizar(artistaAtualizar);
    
            return Results.Ok();
        });
    }

    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }
    
    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
    }
}