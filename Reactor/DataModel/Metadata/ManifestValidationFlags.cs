using System;

namespace Reactor.DataModel.Metadata
{
    [Flags]
    internal enum ManifestValidationFlags
    {
        MissingFriendlyName = 1,
        MissingModuleFileName = 2
    }
}
