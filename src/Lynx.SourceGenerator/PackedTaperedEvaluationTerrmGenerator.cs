using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                    internal sealed class GeneratePackedConstantAttribute : Attribute
                    {
                    }
                }
                """, Encoding.UTF8));
        });

        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "GeneratedNamespace.GeneratePackedConstantAttribute",
            predicate: static (syntaxNode, _) => syntaxNode is VariableDeclaratorSyntax,
            transform: static (context, _) =>
            {
                var classSymbol = context.TargetSymbol.ContainingSymbol;

                return new Model(
                    Namespace: classSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? string.Empty,
                    ClassName: classSymbol.Name,
                    PropertyName: $"Packed{context.TargetSymbol.Name}",
                    Value: ExtractValueFromIntField(context.TargetNode));
            }
        ).Collect();

        static int ExtractValueFromIntField(SyntaxNode variableDeclarationSyntax)
        {
            var varableDeclaration = (VariableDeclaratorSyntax)variableDeclarationSyntax;

            // Simple, constant initialized
            if (varableDeclaration.Initializer?.Value is LiteralExpressionSyntax literal)
            {
                return int.Parse(literal.Token.ValueText);
            }
            else if (varableDeclaration.Initializer?.Value is InvocationExpressionSyntax invocation)
            {
                var methodName = invocation.Expression.ToString();

                if ((methodName.Equals("Pack", StringComparison.OrdinalIgnoreCase) || methodName.Equals("Utils.Pack", StringComparison.OrdinalIgnoreCase))
                    && invocation.ArgumentList.Arguments.Count == 2)
                {
                    short mg = ParseArgument(invocation.ArgumentList.Arguments[0].Expression);
                    short eg = ParseArgument(invocation.ArgumentList.Arguments[1].Expression);

                    if (mg != Model.DefaultShortValue && eg != Model.DefaultShortValue)
                    {
                        return (eg << 16) + mg;
                    }

                    return 777_777_777;
                }

                return 888_888_888;
            }

            return Model.DefaultValue;
        }

        context.RegisterSourceOutput(pipeline, static (context, models) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("namespace GeneratedNamespace");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PackedTaperedEvaluationTerm");
            sb.AppendLine("    {");

            foreach (var model in models)
            {
                sb.AppendLine($$"""
                public const TaperedEvaluationTerm {{model.PropertyName}} = {{model.Value}};
        """);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
            context.AddSource("PackedTaperedEvaluationTerm.g.cs", sourceText);
        });

        static short ParseArgument(ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax literal)
            {
                return short.Parse(literal.Token.ValueText);
            }
            else if (expression is PrefixUnaryExpressionSyntax unary && unary.Operand is LiteralExpressionSyntax operand)
            {
                var value = short.Parse(operand.Token.ValueText);

                return unary.OperatorToken.IsKind(SyntaxKind.MinusToken) ? (short)-value : value;
            }

            return Model.DefaultShortValue;
        }
    }

    private sealed record Model(string Namespace, string ClassName, string PropertyName, int Value)
    {
        public const int DefaultValue = 999_999_999;

        public const short DefaultShortValue = 32_323;
    }
}