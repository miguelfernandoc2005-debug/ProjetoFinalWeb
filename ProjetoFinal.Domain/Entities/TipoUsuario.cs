namespace ProjetoFinal.Domain.Entities;

public class TipoUsuario
{
    public int Id { get; set; } // 1=Administrador, 2=Docente, 3=Aluno
    public string Nome { get; set; } = default!;
}