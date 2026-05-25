using System.ComponentModel.DataAnnotations;

using Api.Database;

using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Auth.Endpoints;

public static class Register
{

    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("register", Handle);
    }

    private static async Task<Results<NoContent, Conflict>> Handle(
            [FromServices] PGContext db,
            [FromBody] RegisterDTO dto
            )
    {


        return TypedResults.Conflict();
    }


    private record RegisterDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    private class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(r => r.Email).Must(e => new EmailAddressAttribute().IsValid(e)).WithMessage("Email is invalid");
        }
    }
}