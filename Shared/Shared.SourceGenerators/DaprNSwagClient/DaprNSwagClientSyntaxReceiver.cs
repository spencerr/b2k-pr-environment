using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shared.SourceGenerators.DaprNSwagClient
{
    class DaprNSwagClientSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Classes { get; } = new List<INamedTypeSymbol>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is INamedTypeSymbol namedTypeSymbol)
                {
                    if (namedTypeSymbol.Name.EndsWith("Client") && namedTypeSymbol.AllInterfaces.Any(intf => intf.Name == $"I{namedTypeSymbol.Name}"))
                    {
                        Classes.Add(namedTypeSymbol);
                    }
                }
            }
        }
    }
}


