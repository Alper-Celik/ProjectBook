# SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
#
# SPDX-License-Identifier: AGPL-3.0-or-later
# SPDX-License-Identifier: Apache-2.0

{
  inputs = {
    flake-parts.url = "github:hercules-ci/flake-parts";
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    inputs@{ flake-parts, ... }:
    flake-parts.lib.mkFlake { inherit inputs; } {
      imports = [
      ];
      systems = [
        "x86_64-linux"
        "aarch64-linux"
        "aarch64-darwin"
        "x86_64-darwin"
      ];
      perSystem =
        {
          config,
          self',
          inputs',
          pkgs,
          system,
          ...
        }:
        {
          devShells.default =
            let
              dotnet = pkgs.dotnetCorePackages.sdk_11_0;
            in
            pkgs.mkShell {
              packages = with pkgs; [
                dotnet
                nodejs_22
                reuse
              ];

              DOTNET_ROOT = "${dotnet}/share/dotnet";
              DOTNET_PATH = "${dotnet}/bin/dotnet";
            };
          devShells.ci = config.devShells.default;
        };
    };
}
