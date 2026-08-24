# SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
#
# SPDX-License-Identifier: AGPL-3.0-or-later
# SPDX-License-Identifier: Apache-2.0

color-cmd := "44m"

lint: dotnet-restore dotnet-format-check reuse-check



reuse-check:
  @printf "\033[{{color-cmd}}"
  reuse lint
  @printf "\033[0m"

dotnet-restore:
  @printf "\033[{{color-cmd}}"
  dotnet restore --locked-mode
  @printf "\033[0m"

dotnet-build: dotnet-restore
  @printf "\033[{{color-cmd}}"
  dotnet build --no-restore
  @printf "\033[0m"

dotnet-format-check:
  @printf "\033[{{color-cmd}}"
  dotnet format --verify-no-changes --verbosity diagnostic --no-restore
  @printf "\033[0m"

dotnet-format:
  @printf "\033[{{color-cmd}}"
  dotnet format --verbosity diagnostic --no-restore
  @printf "\033[0m"

dotnet-test:
  @printf "\033[{{color-cmd}}"
  dotnet test --no-restore
  @printf "\033[0m"

ci-dotnet: dotnet-restore dotnet-format-check dotnet-build dotnet-test

ci-all: reuse-check ci-dotnet
