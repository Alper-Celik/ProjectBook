namespace Api.Auth.Models;

[Flags]
public enum UserPermissionBits : long
{
    All = ~0
}