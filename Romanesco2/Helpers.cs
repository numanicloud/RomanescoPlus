using System.Reactive;
using System.Reactive.Linq;

namespace Romanesco2.DataModel;

internal static class Helpers
{
    public static IObservable<T> FilterNull<T>(this IObservable<T?> source)
        where T : class
    {
        return new FilterNullObservable<T>()
        {
            Source = source
        };
    }

    public static IEnumerable<T> FilterNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        foreach (var item in source)
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    public static IObservable<Unit> DiscardValue<T>(this IObservable<T> source)
    {
        return source.Select(_ => Unit.Default);
    }

    private class FilterNullObservable<T> : IObservable<T>
    {
        public required IObservable<T?> Source { private get; init; }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Source.Subscribe(x =>
            {
                if (x is not null)
                {
                    observer.OnNext(x);
                }
            }, observer.OnError, observer.OnCompleted);
        }
    }
}