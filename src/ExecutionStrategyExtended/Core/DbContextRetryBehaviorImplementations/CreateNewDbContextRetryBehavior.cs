﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.ExecutionStrategyExtended.Core.DbContextRetryBehaviorImplementations;

internal class CreateNewDbContextRetryBehavior<TDbContext> : IDbContextRetryBehavior<TDbContext> where TDbContext : DbContext
{
    private readonly bool _disposePreviousContext;
    private readonly IDbContextFactory<TDbContext> _factory;
    private readonly TDbContext _mainContext;
    private readonly ActualDbContextProvider<TDbContext> _actualDbContextProvider;
    private TDbContext? _previousContext;

    public CreateNewDbContextRetryBehavior(bool disposePreviousContext, IDbContextFactory<TDbContext> factory, TDbContext mainContext, ActualDbContextProvider<TDbContext> actualDbContextProvider)
    {
        _disposePreviousContext = disposePreviousContext;
        _factory = factory;
        _mainContext = mainContext;
        _actualDbContextProvider = actualDbContextProvider;
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _mainContext.Database.CreateExecutionStrategy();
    }

    public async Task<TDbContext> ProvideDbContextForRetry(int attempt)
    {
        await DisposePreviousContext();

        var context = await _factory.CreateDbContextAsync();
        _previousContext = context;

        _actualDbContextProvider.DbContext = context;
        return context;
    }

    private async Task DisposePreviousContext()
    {
        if (_disposePreviousContext)
        {
            if (_previousContext is null)
            {
                return;
            }
            
            await _previousContext.DisposeAsync();
        }

        _previousContext = null;
    }
}