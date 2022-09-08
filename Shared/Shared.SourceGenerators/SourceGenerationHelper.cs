namespace Shared.SourceGenerators
{
    public static class SourceGenerationHelper
    {
        public const string SieveAttributes = @"
namespace Shared.Common.Sieve;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ApplySieveAttribute : System.Attribute
{

}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ApplyFilterAttribute : System.Attribute
{

}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ApplySortAttribute : System.Attribute
{

}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ExcludeSieveAttribute : System.Attribute
{

}";

        public const string ExpandableAttribute = @"
namespace SIS.EntityFramework;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class IncludableAttribute : System.Attribute
{

}

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ExpandAttribute : System.Attribute
{

}
";
    }
}
