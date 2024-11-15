using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Lynx.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class PackedTaperedEvaluationTermGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
        {
            postInitializationContext.AddSource("GeneratePackedConstantAttribute.g.cs", SourceText.From("""
                using System;

                namespace GeneratedNamespace
                {
                    internal sealed class GeneratePackedConstant : Attribute
                    {
                    }
                }
                """, Encoding.UTF8));
        });

        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "GeneratedNamespace.GeneratePackedConstant",
            predicate: static (syntaxNode, _) => syntaxNode is FieldDeclarationSyntax,
            transform: static (context, _) =>
            {
                var classSymbol = context.TargetSymbol.ContainingSymbol;
                 // Debugging output
                System.Diagnostics.Debug.WriteLine($"Processing field: {context.TargetSymbol.Name} in class: {classSymbol.Name}");
                return new Model(
                    Namespace: classSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? string.Empty,
                    ClassName: classSymbol.Name,
                    PropertyName: $"Packed{context.TargetSymbol.Name}",
                    Value: ExtractValueFromIntField(context.SemanticModel, context.TargetNode));
            }
        ).Collect();

        static int ExtractValueFromIntField(SemanticModel semanticModel, SyntaxNode fieldDeclarationSyntax)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)fieldDeclarationSyntax;
            var variableDeclaration = fieldDeclaration.Declaration;
            var variable = variableDeclaration.Variables.FirstOrDefault();

            if (variable != null && variable.Initializer != null)
            {
                var initializer = variable.Initializer.Value;
                var constantValue = semanticModel.GetConstantValue(initializer);
                if (constantValue.HasValue && constantValue.Value is int intValue)
                {
                    return intValue;
                }

                // Handle cases where the initializer is an object creation expression
                if (initializer is ObjectCreationExpressionSyntax objectCreation)
                {
                    var argument = objectCreation?.ArgumentList?.Arguments.FirstOrDefault();
                    if (argument != null)
                    {
                        constantValue = semanticModel.GetConstantValue(argument.Expression);
                        if (constantValue.HasValue && constantValue.Value is int intArgValue)
                        {
                            return intArgValue;
                        }
                    }
                }
            }

            return Model.DefaultValue;
        }

        context.RegisterSourceOutput(pipeline, static (context, models) =>
        {
            foreach (var model in models)
            {
                var sourceText = SourceText.From($$"""
                    namespace {{model.Namespace}};
                    partial class {{model.ClassName}}
                    {
                        public const TaperedEvaluationTerm {{model.PropertyName}} = {{model.Value}};
                    }
                    """, Encoding.UTF8);

                context.AddSource($"{model.ClassName}_{model.PropertyName}.g.cs", sourceText);
            }
        });
    }

    private sealed record Model(string Namespace, string ClassName, string PropertyName, int Value)
    {
        public const int DefaultValue = 999_999_999;
    }
}