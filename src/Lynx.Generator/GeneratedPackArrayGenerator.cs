using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace Lynx.Generator;

[Generator(LanguageNames.CSharp)]
public class GeneratedPackArrayGenerator : IIncrementalGenerator
{
    private const string OurAttribute = "Lynx.Generator.GeneratedPackArrayAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
            postInitializationContext.AddSource("GeneratedPackArrayAttribute.g.cs", AttributeSource()));

        IncrementalValueProvider<ImmutableArray<Model?>> pipeline = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: OurAttribute,
                predicate: static (syntaxNode, _) => IsGeneratedPackAttribute(syntaxNode),
                transform: static (context, _) => GetModel(context)
        ).Where(static m => m is not null).Collect();

        context.RegisterSourceOutput(pipeline, static (context, models) =>
            Execute(context, models));
    }

    private static SourceText AttributeSource() => SourceText.From("""
        #pragma warning disable IDE1006 // Naming Styles

        using System;

        namespace Lynx.Generator
        {
            [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
            internal sealed class GeneratedPackArrayAttribute : Attribute
            {
                public (short mg, short eg)[] Values { get; init; }

                public GeneratedPackArrayAttribute(params string[] values)
                {
                    Values = values.Select(static s =>
                    {
                        var middle = s.IndexOf(',');

                        return (short.Parse(s[..middle]), short.Parse(s[(middle + 1)..]));
                    }).ToArray();
                }
            }
        }

        #pragma warning restore IDE1006 // Naming Styles
        """, Encoding.UTF8);

    private static bool IsGeneratedPackAttribute(SyntaxNode syntaxNode) =>
        syntaxNode is VariableDeclaratorSyntax;

    private static Model? GetModel(GeneratorAttributeSyntaxContext context)
    {
        var value = ExtractPackedArrayFromAttribute(context.TargetNode, context.SemanticModel);

        if (value.Count != 0)
        {
            var classSymbol = context.TargetSymbol.ContainingSymbol;

            return new Model(
                Namespace: classSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? string.Empty,
                ClassName: classSymbol.Name,
                PropertyName: context.TargetSymbol.Name[1..],
                PackedValue: value);
        }

        return null;

        /// <summary>
        /// Heavily inspired by https://andrewlock.net/creating-a-source-generator-part-4-customising-generated-code-with-marker-attributes/
        /// Code in https://github.com/andrewlock/blog-examples/tree/c35edf1c1f0e1f9adf84c215e2ce7ab644b374f5/NetEscapades.EnumGenerators4
        /// </summary>
        static EquatableArray<UnpackedValues> ExtractPackedArrayFromAttribute(SyntaxNode variableDeclarationSyntax, SemanticModel semanticModel)
        {
            List<UnpackedValues> values = [];

            if (semanticModel.GetDeclaredSymbol(variableDeclarationSyntax) is not IFieldSymbol variableSymbol)
            {
                return [];
            }

            INamedTypeSymbol? ourAttribute = semanticModel.Compilation.GetTypeByMetadataName(OurAttribute);
            if (ourAttribute is null)
            {
                return [];
            }

            // Loop through all of the attributes on the enum until we find the [EnumExtensions] attribute
            foreach (var group in variableSymbol.GetAttributes().GroupBy(static attr => attr.AttributeClass, SymbolEqualityComparer.Default))
            {
                // Verify that this is our attribute
                if (!ourAttribute.Equals(group.Key, SymbolEqualityComparer.Default))
                {
                    continue;
                }

                // More than one of our attributes -> invalid for our use case
                if (group.Count() > 1)
                {
                    return [];
                }

                AttributeData attributeData = group.First();

                // Constructor arguments
                if (!attributeData.ConstructorArguments.IsEmpty)
                {
                    // Make sure we don't have any errors, otherwise don't do generation
                    foreach (TypedConstant arg in attributeData.ConstructorArguments)
                    {
                        if (arg.Kind == TypedConstantKind.Error)
                        {
                            return [];
                        }
                    }

                    // Get the constructor arguments
                    var constructorArguments = attributeData.ConstructorArguments;

                    if (constructorArguments.Length == 1 && constructorArguments[0].Kind == TypedConstantKind.Array)
                    {
                        foreach (var typedConstant in constructorArguments[0].Values)
                        {
                            if (typedConstant.Value is string value)
                            {
                                var middle = value.IndexOf(',');
                                var mg = short.Parse(value[..middle]);
                                var eg = short.Parse(value[(middle + 1)..]);
                                values.Add(new(mg, eg));
                            }
                        }
                    }
                }

                // Named arguments
                if (!attributeData.NamedArguments.IsEmpty)
                {
                    foreach (KeyValuePair<string, TypedConstant> arg in attributeData.NamedArguments)
                    {
                        TypedConstant typedConstant = arg.Value;

                        // Make sure we don't have any errors, otherwise don't do generation
                        if (typedConstant.Kind == TypedConstantKind.Error)
                        {
                            return [];
                        }

                        if (arg.Key == "Values")
                        {
                            foreach (var potentialString in arg.Value.Values)
                            {
                                if (potentialString.Value is string value)
                                {
                                    var middle = value.IndexOf(',');
                                    var mg = short.Parse(value[..middle]);
                                    var eg = short.Parse(value[(middle + 1)..]);
                                    values.Add(new(mg, eg));
                                }
                            }
                        }
                    }
                }

                break;
            }

            return new EquatableArray<UnpackedValues>([.. values]);
        }
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<Model?> models)
    {
        foreach (var group in models.GroupBy(m => m?.Namespace))
        {
            Debug.Assert(group.Key is not null);

            foreach (var subgroup in group.GroupBy(m => m?.ClassName))
            {
                Debug.Assert(subgroup.Key is not null);

                var sb = new StringBuilder();
                sb.Append("namespace ").Append(group.Key).AppendLine(";");
                sb.AppendLine();
                sb.Append("static partial class ").AppendLine(subgroup.Key);
                sb.AppendLine("{");

                foreach (var model in subgroup)
                {
                    Debug.Assert(model is not null);

#pragma warning disable S2589 // Boolean expressions should not be gratuitous - Debug.Assert check only exists on release mode
                    if (model is null)
                    {
                        continue;
                    }
#pragma warning restore S2589 // Boolean expressions should not be gratuitous

                    sb.AppendLine($$"""
                        public static ReadOnlySpan<int> {{model.PropertyName}} =>
                        [
                    """);

                    foreach (var arrayItem in model.PackedValue)
                    {
                        sb.AppendLine($$"""
                                {{string.Format("{0,-12}", $"{arrayItem.Pack()},")}}// Pack({{string.Format("{0,4}", arrayItem.mg)}}, {{string.Format("{0,4}", arrayItem.eg)}})
                        """);
                    }

                    sb.AppendLine($$"""
                        ];
                    """);
                    sb.AppendLine();
                }

                sb.AppendLine("}");

                var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                context.AddSource($"{subgroup.Key}.g.cs", sourceText);
            }
        }
    }

#pragma warning disable IDE1006 // Naming Styles
    private sealed record UnpackedValues(short mg, short eg)
    {
        public int Pack() => Utils.Pack(mg, eg);
    }
#pragma warning restore IDE1006 // Naming Styles

    private sealed record Model(string Namespace, string ClassName, string PropertyName, EquatableArray<UnpackedValues> PackedValue);
}
