using ProjetoFinal.Web.Models;     // DTOs/ViewModels do ProjetoFinal
using System.Threading;            // CancellationToken
using System.Collections.Generic;  // IReadOnlyList
using System.Threading.Tasks;      // Task

namespace ProjetoFinal.Web.Services
{
    public interface IApiClient
    {
        Task<IReadOnlyList<(int Id, string Nome)>> GetMateriasAsync(CancellationToken ct = default);
        Task<IReadOnlyList<(int Id, string Nome)>> GetTiposCursoAsync(CancellationToken ct = default);
        Task<PagedResult<CursoDto>> SearchCursosAsync(CursoFilterVm filter, CancellationToken ct = default);
        Task<CursoDto?> GetCursoAsync(int id, CancellationToken ct = default);
    }
}
