using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;

namespace Romanesco.DataModel;

public static class Helpers
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
        where T : struct
    {
        foreach (var item in source)
        {
            if (item is not null)
            {
                yield return item.Value;
            }
        }
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

    public static void IfNotNull<T>(this T? subject, Action<T> onTrue)
        where T : class
    {
        if (subject is not null)
        {
            onTrue(subject);
        }
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