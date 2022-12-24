global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Romanesco2.DataModel.Test")]

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