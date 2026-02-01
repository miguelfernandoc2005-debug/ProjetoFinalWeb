using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace ProjetoFinal.Infrastructure.Entities;

/// <summary>
/// Usuário Identity com campos de domínio essenciais. Relaciona a entidade TipoUsuario com o seu status, data de criação e de edição, através da propriedade IdTipoUsuario, que faz o papel de uma chave estrangeira
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; } = true;      // soft delete
    public int IdTipoUsuario { get; set; }          // FK para TipoUsuario
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
