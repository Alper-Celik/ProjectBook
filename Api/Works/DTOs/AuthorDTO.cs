namespace Api.Works.DTOs;

public record AuthorDTO(
        Guid Id,
        int RowVersion,
        string? FirstName,
        string? LastName,
        string DisplayName,
        string[] PenNames
        );