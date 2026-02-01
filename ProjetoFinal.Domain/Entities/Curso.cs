namespace ProjetoFinal.Domain.Entities;

public class Curso
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string? Descricao { get; set; }
    public string? ImagemCapaUrl { get; set; }
    public DateTime DataDeCriacao { get; set; } = DateTime.UtcNow;
    public int CargaHoraria { get; set; }

    // Relação com Matéria (opcional)
    public int? IdMateria { get; set; }
    public Materia? Materia { get; set; }

    // Relação com TipoCurso (opcional)
    public int? IdTipoCurso { get; set; }
    public TipoCurso? TipoCurso { get; set; }

    //Por que opcional? Porque se eu apagar um registro de Tipo de Curso ou Matéria, eu quebro a minha aplicação, pois há uma relação complexa entre entidades... Se eu tivesse uma propriedade Ativo/Inativo
}