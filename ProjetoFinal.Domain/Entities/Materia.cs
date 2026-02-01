namespace ProjetoFinal.Domain.Entities;

public class Materia
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;

    // Uma matéria pode ter vários cursos
    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}