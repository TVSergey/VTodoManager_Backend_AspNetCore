using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Dto
{
    public class ProjectDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public long CreationDate { get; set; }
    }
}