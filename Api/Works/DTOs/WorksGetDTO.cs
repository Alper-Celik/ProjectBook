using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record struct WorksGetDTO(
        AuthorDTO[] ReferencedAuthors,
        WorkSmallDTO[] Works
        );

public record struct WorkSmallDTO(
        Guid Id,
        string Title,
        IEnumerable<Guid> AuthorIds,
        Uri? CoverUrl = null
        );

//TODO: mapper

public static partial class WorkSmallDTOMapper
{
    public static IQueryable<WorkSmallDTO> ProjectToDTO(IQueryable<Work> q) => q.Select(w => new WorkSmallDTO
    {
        Id = w.Id,
        Title = w.Title,
        AuthorIds = w.Authors!.Select(a => a.Id),
        CoverUrl = null
    });

}