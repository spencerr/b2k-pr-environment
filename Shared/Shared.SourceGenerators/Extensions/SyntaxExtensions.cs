﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shared.SourceGenerators.Extensions
{
    public static class SyntaxExtensions
    {
        public static string? ExtractName(this NameSyntax? name)
        {
            while (name != null)
            {
                switch (name)
                {
                    case IdentifierNameSyntax ins:
                        return ins.Identifier.Text;

                    case QualifiedNameSyntax qns:
                        name = qns.Right;
                        break;

                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
