namespace Shared.Common.NSwag.Processors;

public static class SchemaHelpers
{
    public static string GetFeatureNamespace(string typeNamespace, string featurePrefix, bool generateDeepNamespace = false, string separator = ".")
    {
        string[] featureNamespace = typeNamespace[featurePrefix.Length..].Split(".");

        if (!generateDeepNamespace)
        {
            featureNamespace = featureNamespace
                .TakeWhile(directory => !string.Equals(directory, "Queries", StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(directory, "Commands", StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(directory, "Shared", StringComparison.InvariantCultureIgnoreCase))
                .ToArray();
        }

        return string.Join(separator, featureNamespace);
    }
}