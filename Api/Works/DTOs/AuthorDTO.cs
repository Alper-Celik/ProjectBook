using Api.Works.Models;

using Riok.Mapperly.Abstractions;

namespace Api.Works.DTOs;

public record AuthorGetDTO(
        Guid Id,
        int RowVersion,
        string? FirstName,
        string? LastName,
        string DisplayName,
        string[] PenNames
        );

[Mapper]
public static partial class AuthorGetDTOMapper
{
    public static partial IQueryable<AuthorGetDTO> ProjectToDTO(IQueryable<Author> q);
}