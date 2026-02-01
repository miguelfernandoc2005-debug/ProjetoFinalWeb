using ProjetoFinal.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal.Application.Filters
{
    public sealed class CursoFilter : PagedRequest
    {
        public int? IdMateria { get; set; }
        public int? IdTipoCurso { get; set; }
        public string? Q { get; set; }
        public string? SortBy { get; set; }
        public bool Desc { get; set; } = true;
    }

}
