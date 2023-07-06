using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Mappings
{
    public class BoardsAutoMapperProfile : Profile
    {
        public BoardsAutoMapperProfile()
        {
            CreateMap<Board, BoardDto>()
                .ForMember(x => x.ProjectId, opt => opt.Ignore());
            CreateMap<CreateBoardDto, Board>()
                .ForMember(x => x.Project, opt => opt.Ignore());
        }
    }
}