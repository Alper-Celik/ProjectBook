using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record struct AuthorDTO(
        Guid Id,
        int RowVersion,
        string? FirstName,
        string? LastName,
        string DisplayName,
        string[] PenNames
        );

[Mapper]
public static partial class AuthorDTOMapper
{
    public static partial IQueryable<AuthorDTO> ProjectToDTO(IQueryable<Author> q);
}