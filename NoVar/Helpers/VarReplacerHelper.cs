using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;

namespace NoVar.src.Helpers
{
	public static class VarReplacerHelper
	{
		public static async Task ReplaceVarsAsync(string filePath, ITextView textView, bool replaceAll)
		{
			if (filePath != null)
			{
				bool isCSharpFile = IsCSharpFile(filePath);

				if (isCSharpFile)
				{
					// Get the text view and snapshot
					ITextSnapshot textSnapshot = textView.TextSnapshot;

					string documentText = textSnapshot.GetText();

					// Parse the document with Roslyn
					SyntaxTree tree = CSharpSyntaxTree.ParseText(documentText);

					// Get the semantic model
					IEnumerable<MetadataReference> references = AppDomain.CurrentDomain.GetAssemblies()
						.Where(a => !a.IsDynamic)
						.Select(a => MetadataReference.CreateFromFile(a.Location))
						.Cast<MetadataReference>();

					CSharpCompilation compilation = CSharpCompilation.Create("Analysis")
						.AddReferences(references)
						.AddSyntaxTrees(tree);

					SemanticModel semanticModel = compilation.GetSemanticModel(tree);

					// Find all 'var' declarations
					SyntaxNode root = await tree.GetRootAsync().ConfigureAwait(false);
					IEnumerable<VariableDeclarationSyntax> variableDeclarationSyntaxes = from variableDeclaration in root.DescendantNodes().OfType<VariableDeclarationSyntax>()
																						 where variableDeclaration.Type.IsVar &&
																							   (replaceAll || variableDeclaration.Variables.Any(v => !(v.Initializer?.Value is ObjectCreationExpressionSyntax)))
																						 select variableDeclaration;
					string newDocumentText = documentText;

					// Process each 'var' declaration
					foreach (VariableDeclarationSyntax varDecl in variableDeclarationSyntaxes.Reverse())
					{
						foreach (VariableDeclaratorSyntax variable in varDecl.Variables)
						{
							ILocalSymbol variableSymbol = semanticModel.GetDeclaredSymbol(variable) as ILocalSymbol;
							if (variableSymbol != null)
							{
								string variableName = variableSymbol.Name;
								string variableType = variableSymbol.Type.ToString();

								// Find the line and where in the line is the 'var' declaration
								Microsoft.CodeAnalysis.Text.TextSpan varTextSpan = varDecl.Type.Span;
								newDocumentText = newDocumentText.Remove(varTextSpan.Start, varTextSpan.Length).Insert(varTextSpan.Start, variableType);
							}
						}
					}

					// Update the document with the new text
					using (ITextEdit edit = textView.TextBuffer.CreateEdit())
					{
						edit.Replace(0, textSnapshot.Length, newDocumentText);
						edit.Apply();
					}
				}
			}
		}

		private static bool IsCSharpFile(string filePath)
		{
			string fileExtension = System.IO.Path.GetExtension(filePath);

			return fileExtension == ".cs";
		}
	}
 }
