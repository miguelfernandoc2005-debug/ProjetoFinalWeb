namespace ProjetoFinal.Application.Services;

/// <summary>
/// Serviço para consultar listas auxiliares do catálogo de cursos
/// (matérias e tipos de curso).
/// Retorna projeções leves (Id, Nome) para uso em combos/dropdowns.
/// </summary>
public interface ICursoLookupService
{
    /// <summary>
    /// Obtém a lista de matérias disponíveis (ex.: Matemática, Física...).
    /// </summary>
    /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
    /// <returns>
    /// Lista somente leitura de tuplas <c>(int Id, string Nome)</c>.
    /// </returns>
    Task<IReadOnlyList<(int Id, string Nome)>> GetMateriasAsync(CancellationToken ct = default);

    /// <summary>
    /// Obtém a lista de tipos de curso (ex.: Técnico, Livre, Profissionalizante...).
    /// </summary>
    /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
    /// <returns>
    /// Lista somente leitura de tuplas <c>(int Id, string Nome)</c>.
    /// </returns>
    Task<IReadOnlyList<(int Id, string Nome)>> GetTiposCursoAsync(CancellationToken ct = default);
}
