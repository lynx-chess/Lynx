using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace Lynx.Generator;

[Generator(LanguageNames.CSharp)]
public class GeneratedPackGenerator : IIncrementalGenerator
{
    private const string OurAttribute = "Lynx.Generator.GeneratedPackAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
            postInitializationContext.AddSource("GeneratedPackAttribute.g.cs", AttributeSource()));

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
        using System;

        namespace Lynx.Generator
        {
            [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
            internal sealed class GeneratedPackAttribute : Attribute
            {
                public short MG { get; }
                public short EG { get; }

                public GeneratedPackAttribute(short mg, short eg)
                {
                    MG = mg;
                    EG = eg;
                }
            }
        }
        """, Encoding.UTF8);

    private static bool IsGeneratedPackAttribute(SyntaxNode syntaxNode) =>
        syntaxNode is VariableDeclaratorSyntax;

    private static Model? GetModel(GeneratorAttributeSyntaxContext context)
    {
        var value = ExtractPackedValueFromAttribute(context.TargetNode, context.SemanticModel);

        if (value != Model.DefaultValue)
        {
            var classSymbol = context.TargetSymbol.ContainingSymbol;

            return new Model(
                Namespace: classSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? string.Empty,
                ClassName: classSymbol.Name,
                PropertyName: context.TargetSymbol.Name[1..],
                Value: value);
        }

        return null;

        /// <summary>
        /// Heavily inspired by https://andrewlock.net/creating-a-source-generator-part-4-customising-generated-code-with-marker-attributes/
        /// Code in https://github.com/andrewlock/blog-examples/tree/c35edf1c1f0e1f9adf84c215e2ce7ab644b374f5/NetEscapades.EnumGenerators4
        /// </summary>
        static int ExtractPackedValueFromAttribute(SyntaxNode variableDeclarationSyntax, SemanticModel semanticModel)
        {
            const short DefaultShortValue = 32_323;

            int mg = DefaultShortValue;
            int eg = DefaultShortValue;

            if (semanticModel.GetDeclaredSymbol(variableDeclarationSyntax) is not IFieldSymbol variableSymbol)
            {
                return Model.DefaultValue;
            }

            INamedTypeSymbol? ourAttribute = semanticModel.Compilation.GetTypeByMetadataName(OurAttribute);
            if (ourAttribute is null)
            {
                return Model.DefaultValue;
            }

            // Loop through all of the attributes on the enum until we find the [EnumExtensions] attribute
            foreach (AttributeData attributeData in variableSymbol.GetAttributes())
            {
                // Verify that this is our attribute
                if (!ourAttribute!.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                {
                    continue;
                }

                // Constructor arguments
                if (!attributeData.ConstructorArguments.IsEmpty)
                {
                    ImmutableArray<TypedConstant> args = attributeData.ConstructorArguments;

                    // Make sure we don't have any errors, otherwise don't do generation
                    foreach (TypedConstant arg in args)
                    {
                        if (arg.Kind == TypedConstantKind.Error)
                        {
                            return Model.DefaultValue;
                        }
                    }

                    // Make sure both arguments are present
                    if (args.Length != 2 || args[0].IsNull || args[1].IsNull)
                    {
                        return Model.DefaultValue;
                    }

                    mg = (short)args[0].Value!;
                    eg = (short)args[1].Value!;
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
                            return Model.DefaultValue;
                        }
                        else
                        {
                            // Use the constructor argument or property name to infer which value is set
                            switch (arg.Key)
                            {
                                case "mg":
                                    mg = typedConstant.IsNull
                                        ? DefaultShortValue
                                        : (short)typedConstant.Value!;
                                    break;
                                case "eg":
                                    eg = typedConstant.IsNull
                                        ? DefaultShortValue
                                        : (short)typedConstant.Value!;
                                    break;
                            }
                        }
                    }
                }

                break;
            }

            if (mg == DefaultShortValue || eg == DefaultShortValue)
            {
                return Model.DefaultValue;
            }

            // Utils.Pack(mg, eg)
            return (eg << 16) + mg;
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
            public const int {{model.PropertyName}} = {{model.Value}};
        """);
                }

                sb.AppendLine("}");

                var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                context.AddSource($"{subgroup.Key}.g.cs", sourceText);
            }
        }
    }

    private sealed record Model(string Namespace, string ClassName, string PropertyName, int Value)
    {
        public const int DefaultValue = 999_999_999;
    }
}