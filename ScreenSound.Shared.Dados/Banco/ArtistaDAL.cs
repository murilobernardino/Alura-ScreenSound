using ScreenSound.Modelos;

namespace ScreenSound.Banco;

public class ArtistaDAL : DAL<Artista>
//DAL significa Data Access Layer ou seja Camada de Acesso a Dados
/*
Exemplos de responsabilidades da DAL:
- Abrir e fechar conexões com o banco.
- Executar comandos SQL ou chamadas a procedures.
- Mapear resultados do banco para objetos do sistema (como Artista no seu caso).
- Tratar erros relacionados ao banco de dados.
*/

{
    public ArtistaDAL(ScreenSoundContext context) : base(context){ }
}