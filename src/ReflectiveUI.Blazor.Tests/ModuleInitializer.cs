using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReflectiveUI.Blazor.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifyBunit.Initialize();
}
