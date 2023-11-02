using System.Runtime.CompilerServices;

namespace LangChain.Databases;

/// <summary>
/// Async lazy
/// <see cref="https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/"/>
/// </summary>
internal sealed class AsyncLazy<T>(Func<Task<T>> taskFactory) :
    Lazy<Task<T>>(
        () => Task.Factory.StartNew(
            () => taskFactory(),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default).Unwrap())
{
    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}