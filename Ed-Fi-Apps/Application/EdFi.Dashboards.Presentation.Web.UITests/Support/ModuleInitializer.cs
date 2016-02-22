#if POSTSHARP
using PostSharp.Aspects.Serialization;

namespace EdFi.Dashboards.Presentation.Web.UITests.Support
{
    public class ModuleInitializer
    {
        public static void Run()
        {
            // Retargets serialized PostSharp aspects to newly ILMerged assembly
            ((BinaryAspectSerializationBinder)BinaryAspectSerializer.Binder)
                .Retarget(
                    "EdFi.Dashboards.Presentation.Core.UITests",
                    "EdFi.Dashboards.Presentation.Web.UITests");
        }
    }
}
#endif