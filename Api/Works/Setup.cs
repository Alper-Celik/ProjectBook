// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Api.Works.Endpoints;

namespace Api.Works;


public static class Setup
{
    public static void MapEndpoints(IEndpointRouteBuilder route)
    {
        WorkEndpoints.Map(route);
    }
}