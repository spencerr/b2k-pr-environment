using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Shared.Common.NSwag.Processors;

public class RemoveVersionParameterProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var versionParameter =
            context.OperationDescription.Operation.Parameters.FirstOrDefault(p => p.Name == "version");

        if (versionParameter != null)
        {
            context.OperationDescription.Operation.Parameters.Remove(versionParameter);
        }

        return true;
    }
}