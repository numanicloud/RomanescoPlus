using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Romanesco.Host2.ViewModels;

internal static class Helpers
{
    public static void Forget(this Task task)
    {
        var ignore = task;
    }
}