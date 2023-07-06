using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.UseCases.Handlers.Accounts.Mappings
{
    public class AccountsAutoMapperProfile : Profile
    {
        public AccountsAutoMapperProfile()
        {
            CreateMap<CreateAccountDto, Account>()
                .ForMember(x => x.Firstname, opt => opt.Ignore())
                .ForMember(x => x.Surname, opt => opt.Ignore())
                .ForMember(x => x.HashedPassword, opt => opt.Ignore());
        }
    }
}