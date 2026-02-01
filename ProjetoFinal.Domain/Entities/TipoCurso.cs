namespace ProjetoFinal.Domain.Entities;

public class TipoCurso
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string? Descricao { get; set; }

    // Um tipo pode ter vários cursos
    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}