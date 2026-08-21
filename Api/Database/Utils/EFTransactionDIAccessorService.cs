// SPDX-FileCopyrightText: 2026 Alper Çelik <alper@alper-celik.dev>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.EntityFrameworkCore.Storage;
namespace Api.Database.Utils;

public interface IEFTransactionDIAccessorService : IAsyncDisposable, IDisposable
{
    public IDbContextTransaction BeginOrGetTransaction();
    public Task<IDbContextTransaction> BeginOrGetTransactionAsync();
}

public class EFTransactionDIAccessorService(PGContext db) : IEFTransactionDIAccessorService
{
    private IDbContextTransaction? _tx;

    public IDbContextTransaction BeginOrGetTransaction()
    {
        return _tx ?? db.Database.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginOrGetTransactionAsync()
    {
        return _tx ?? await db.Database.BeginTransactionAsync();
    }

    public async ValueTask FinishTransaction()
    {
        if (_tx is not null)
            await _tx.DisposeAsync();

        _tx = null;
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        _tx?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (_tx is not null)
            await _tx.DisposeAsync();
    }
}