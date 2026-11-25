namespace Singulink.CodeAnalysis;

/// <summary>
/// Marks a struct that must not be copied.
/// Declared here instead of referencing the Singulink.CodeAnalysis package to avoid a runtime dependency.
/// See https://www.singulink.com/Docs/Singulink.CodeAnalysis/
/// </summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.GenericParameter, Inherited = false)]
internal sealed class NonCopyableAttribute : Attribute;
