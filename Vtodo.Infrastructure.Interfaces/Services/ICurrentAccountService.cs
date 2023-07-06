using Vtodo.Entities.Models;

namespace Vtodo.Infrastructure.Interfaces.Services
{
    internal interface ICurrentAccountService
    {
        Account Account { get; set; }
    }
}