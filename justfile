# SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
#
# SPDX-License-Identifier: AGPL-3.0-or-later
# SPDX-License-Identifier: Apache-2.0

color-cmd := "44m"

lint: dotnet-restore dotnet-format-check reuse-check



reuse-check:
  reuse lint

dotnet-restore:
  dotnet restore --locked-mode

dotnet-build: dotnet-restore
  dotnet build --no-restore

dotnet-format-check:
  dotnet format --verify-no-changes --verbosity diagnostic --no-restore

dotnet-format:
  dotnet format --verbosity diagnostic --no-restore

dotnet-test:
  dotnet test --no-restore

ci-dotnet: dotnet-restore dotnet-format-check dotnet-build dotnet-test

ci-all: reuse-check ci-dotnet
