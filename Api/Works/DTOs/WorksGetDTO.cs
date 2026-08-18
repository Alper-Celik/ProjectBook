namespace Api.Works.DTOs;

public record WorksGetDto(
        AuthorDTO[] ReferencedAuthors,
        WorkSmallDTO[] Works
        );

public record WorkSmallDTO(
        Guid Id,
        string Title,
        Uri? CoverUrl,
        Guid[] AuthorIds
        );