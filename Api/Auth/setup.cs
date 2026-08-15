// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Api.Auth;

public static class Setup
{
    public static void MapEndpoints(IEndpointRouteBuilder route)
    {
        Endpoints.RegisterEndpoints.Map(route);
    }
}