using System;

namespace Reactor.DataModel
{
    [Flags]
    internal enum ManifestValidationFlags
    {
        MissingFriendlyName = 1,
        MissingModuleFileName = 2
    }
}
