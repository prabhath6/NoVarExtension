using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace NoVar.Helpers
{
	class VarTypeRewriter : CSharpSyntaxRewriter
	{
		private readonly SemanticModel _semanticModel;
		private readonly bool _replaceAll;

		public VarTypeRewriter(SemanticModel semanticModel, bool replaceAll)
		{
			_semanticModel = semanticModel;
			_replaceAll = replaceAll;
		}

		public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
		{
			if (node.Type.IsVar &&
				(this._replaceAll || node.Variables.Any(v => v?.Initializer?.Value is not ObjectCreationExpressionSyntax)))
			{
				foreach (VariableDeclaratorSyntax variable in node.Variables)
				{
					if (_semanticModel.GetDeclaredSymbol(variable) is ILocalSymbol variableSymbol)
					{
						// Get the type with proper formatting
						string variableType = variableSymbol.Type.ToMinimalDisplayString(_semanticModel, node.Type.SpanStart);

						// Replace 'var' with the actual type
						TypeSyntax newTypeName = SyntaxFactory.ParseTypeName(variableType)
							.WithTriviaFrom(node.Type);

						// Return the node with the new type
						return node.WithType(newTypeName);
					}
					else
					{
						// Log the issue if the variable symbol is null
						System.Diagnostics.Debug.WriteLine($"Failed to get symbol for variable: {variable.Identifier.Text}");
					}
				}
			}

			return base.VisitVariableDeclaration(node);
		}
	}
}
