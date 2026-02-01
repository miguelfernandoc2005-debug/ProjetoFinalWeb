using ProjetoFinal.Web.Models; // DTOs/ViewModels do ProjetoFinal
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System;
using System.Linq;

namespace ProjetoFinal.Web.Services
{
    public sealed class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private readonly Uri[] _baseUris;

        public ApiClient(HttpClient http, IConfiguration config)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));

            var primary = config["ApiBaseUrl"];
            var fallback = config["ApiBaseUrlFallback"];

            var bases = new List<Uri>();
            if (!string.IsNullOrWhiteSpace(primary))
            {
                if (Uri.TryCreate(primary, UriKind.Absolute, out var p)) bases.Add(p);
            }

            if (!string.IsNullOrWhiteSpace(fallback))
            {
                if (Uri.TryCreate(fallback, UriKind.Absolute, out var f)) bases.Add(f);
            }

            // If no explicit fallback provided and primary is HTTPS, add an HTTP fallback on common port
            if (bases.Count == 1 && bases[0].Scheme == "https")
            {
                try
                {
                    var host = bases[0].Host;
                    // default http port mapping: try5083 if primary used7028, else use same host with http
                    var fb = new UriBuilder(bases[0]) { Scheme = "http", Port = 5083 };
                    bases.Add(fb.Uri);
                }
                catch
                {
                    // ignore
                }
            }

            // Ensure at least one base (HttpClient.BaseAddress as last resort)
            if (!bases.Any())
            {
                if (_http.BaseAddress != null)
                    bases.Add(_http.BaseAddress);
            }

            _baseUris = bases.ToArray();
        }

        public async Task<IReadOnlyList<(int Id, string Nome)>> GetMateriasAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await GetJsonAsync<List<Item>>("api/materias", ct)
                           ?? new List<Item>();

                return data.Select(x => (x.Id, x.Nome)).ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WARN] ApiClient.GetMateriasAsync failed: " + ex.Message);
                return Array.Empty<(int, string)>();
            }
        }

        public async Task<IReadOnlyList<(int Id, string Nome)>> GetTiposCursoAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await GetJsonAsync<List<Item>>("api/tipoDeCursos", ct)
                           ?? new List<Item>();

                return data.Select(x => (x.Id, x.Nome)).ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WARN] ApiClient.GetTiposCursoAsync failed: " + ex.Message);
                return Array.Empty<(int, string)>();
            }
        }

        public async Task<PagedResult<CursoDto>> SearchCursosAsync(CursoFilterVm f, CancellationToken ct = default)
        {
            try
            {
                var qs = BuildQuery(f);
                var res = await GetJsonAsync<PagedResult<CursoDto>>($"api/cursos{qs}", ct)
                          ?? new PagedResult<CursoDto>(1, f.PageSize, 0, Array.Empty<CursoDto>());
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WARN] ApiClient.SearchCursosAsync failed: " + ex.Message);
                return new PagedResult<CursoDto>(1, f.PageSize, 0, Array.Empty<CursoDto>());
            }
        }

        public async Task<CursoDto?> GetCursoAsync(int id, CancellationToken ct = default)
        {
            try
            {
                return await GetJsonAsync<CursoDto>($"api/cursos/{id}", ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WARN] ApiClient.GetCursoAsync failed: " + ex.Message);
                return null;
            }
        }

        private static string BuildQuery(CursoFilterVm f)
        {
            var sb = new StringBuilder($"?page={f.Page}&pageSize={f.PageSize}");

            if (f.IdMateria is int mid) sb.Append($"&idMateria={mid}");
            if (f.IdTipoCurso is int tid) sb.Append($"&idTipoCurso={tid}");
            if (!string.IsNullOrWhiteSpace(f.Q)) sb.Append($"&q={Uri.EscapeDataString(f.Q)}");
            if (!string.IsNullOrWhiteSpace(f.SortBy)) sb.Append($"&sortBy={f.SortBy}");
            if (!f.Desc) sb.Append("&desc=false");

            return sb.ToString();
        }

        private async Task<T?> GetJsonAsync<T>(string relativeOrAbsolutePath, CancellationToken ct = default)
        {
            foreach (var baseUri in _baseUris)
            {
                try
                {
                    var requestUri = new Uri(baseUri, relativeOrAbsolutePath);
                    return await _http.GetFromJsonAsync<T>(requestUri, ct);
                }
                catch (HttpRequestException hre) when (hre.InnerException is SocketException)
                {
                    // connection refused - try next base
                    continue;
                }
                catch (SocketException)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    // For any other error, log and try next base
                    Console.WriteLine("[WARN] ApiClient request failed for base " + baseUri + ": " + ex.Message);
                    continue;
                }
            }

            // If all bases failed, return default instead of throwing so Web can still run
            Console.WriteLine("[WARN] ApiClient: all configured API bases failed: " + string.Join(", ", _baseUris.Select(u => u.ToString())));
            return default;
        }

        private record Item(int Id, string Nome);
    }
}
