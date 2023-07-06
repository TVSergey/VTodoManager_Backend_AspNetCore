using System;
using System.Collections;
using System.Collections.Generic;
using Vtodo.Entities.Models;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Dto
{
    public class BoardDto
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public int PrioritySort { get; set; }
        
        public int ProjectId { get; set; }

        public string? ImageHeaderPath { get; set; }
    }
}