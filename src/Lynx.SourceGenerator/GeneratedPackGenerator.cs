using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System;
using System.Text;

namespace Lynx.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class GeneratedPackGenerator : IIncrementalGenerator
{
    private const string OurAttribute = "Lynx.SourceGenerator.GeneratedPackAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
        {
            postInitializationContext.AddSource("GeneratedPackAttribute.g.cs", SourceText.From("""
                using System;

                namespace GeneratedNamespace
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
                """, Encoding.UTF8));
        });

        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "GeneratedNamespace.GeneratedPackAttribute",
            predicate: static (syntaxNode, _) => syntaxNode is VariableDeclaratorSyntax,
            transform: static (context, _) =>
            {
                var classSymbol = context.TargetSymbol.ContainingSymbol;

                return new Model(
                    Namespace: classSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? string.Empty,
                    ClassName: classSymbol.Name,
                    PropertyName: context.TargetSymbol.Name[1..],
                    Value: ExtractPackedValueFromAttribute(context.TargetNode, context.SemanticModel));
            }
        ).Collect();

        /// <summary>
        /// Inspired by https://andrewlock.net/creating-a-source-generator-part-4-customising-generated-code-with-marker-attributes/
        /// </summary>
        static int ExtractPackedValueFromAttribute(SyntaxNode variableDeclarationSyntax, SemanticModel semanticModel)
        {
            int mg = Model.DefaultShortValue;
            int eg = Model.DefaultShortValue;

            if (semanticModel.GetDeclaredSymbol(variableDeclarationSyntax) is not INamedTypeSymbol variableSymbol)
            {
                // something went wrong
                return Model.DefaultValue;
            }

            INamedTypeSymbol? ourAttribute = semanticModel.Compilation.GetTypeByMetadataName(OurAttribute);

            // Loop through all of the attributes on the enum until we find the [EnumExtensions] attribute
            foreach (AttributeData attributeData in variableSymbol.GetAttributes())
            {
                // Verify that this is our attribute
                if (ourAttribute?.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default) != false)
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
                                        ? Model.DefaultShortValue
                                        : (short)typedConstant.Value!;
                                    break;
                                case "eg":
                                    eg = typedConstant.IsNull
                                        ? Model.DefaultShortValue
                                        : (short)typedConstant.Value!;
                                    break;
                            }
                        }
                    }
                }

                break;
            }

            if (mg == Model.DefaultShortValue || eg == Model.DefaultShortValue)
            {
                return Model.DefaultValue;
            }

            return (eg << 16) + mg;
        }

        context.RegisterSourceOutput(pipeline, static (context, models) =>
        {
            foreach (var group in models.GroupBy(m => m.Namespace))
            {
                foreach (var subgroup in group.GroupBy(m => m.ClassName))
                {
                    var sb = new StringBuilder();
                    sb.Append("namespace ").Append(group.Key ?? "GeneratedNamespace").AppendLine(";");
                    sb.AppendLine();
                    sb.Append("static partial class ").AppendLine(subgroup.Key ?? "GeneratedClass");
                    sb.AppendLine("{");

                    foreach (var model in subgroup)
                    {
                        if (model is null)
                        {
                            continue;
                        }

                        sb.AppendLine($$"""
            public const int {{model.PropertyName}} = {{model.Value}};
        """);
                    }

                    sb.AppendLine("}");

                    var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                    context.AddSource($"{subgroup.Key}.g.cs", sourceText);
                }
            }
        });
    }

    private sealed record Model(string Namespace, string ClassName, string PropertyName, int Value)
    {
        public const int DefaultValue = 999_999_999;

        public const short DefaultShortValue = 32_323;
    }
}