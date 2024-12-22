using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using NoVar.Helpers;

namespace NoVar.src.Helpers
{
	public static class VarReplacerHelper
	{
		public static async Task ReplaceVarsAsync(string solutionPath, string filePath, ITextView textView, bool replaceAll)
		{
			if (filePath != null)
			{
				if (IsCSharpFile(filePath))
				{
					// Create an MSBuild workspace and load the solution
					using MSBuildWorkspace workspace = MSBuildWorkspace.Create();
					Microsoft.CodeAnalysis.Solution solution = await workspace.OpenSolutionAsync(solutionPath);

					// Find the project containing the file
					Microsoft.CodeAnalysis.Project project = solution
						.Projects
						.FirstOrDefault(p => p.Documents.Any(d => d.FilePath == filePath));

					if (project != null)
					{
						Document document = project.Documents.FirstOrDefault(d => d.FilePath == filePath);

						if (document != null)
						{
							// Build the compilation options
							CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary);

							// Collect references including DLLs from bin folders
							List<MetadataReference> references = [];

							// Add references from the project's bin directory
							string outputPath = project.OutputFilePath;
							if (!string.IsNullOrEmpty(outputPath) && File.Exists(outputPath))
							{
								references.Add(MetadataReference.CreateFromFile(outputPath));
							}

							// Add references to necessary system assemblies
							IEnumerable<PortableExecutableReference> assemblies = AppDomain.CurrentDomain.GetAssemblies()
								.Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
								.Select(a => MetadataReference.CreateFromFile(a.Location));
							references.AddRange(assemblies);

							// Create the compilation
							SyntaxTree syntaxTree = await document.GetSyntaxTreeAsync();
							Compilation compilation = CSharpCompilation.Create(
								"Analysis",
								syntaxTrees: [syntaxTree],
								references: references,
								options: compilationOptions);

							// Get the semantic model from the compilation
							SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

							// Get the syntax root
							SyntaxNode root = await syntaxTree.GetRootAsync();

							// Use a rewriter to replace 'var' with the actual type
							VarTypeRewriter rewriter = new(semanticModel, replaceAll);
							SyntaxNode newRoot = rewriter.Visit(root);

							// Get the updated document text
							string newDocumentText = newRoot.GetText().ToString();

							// Get the text view and snapshot
							ITextSnapshot textSnapshot = textView.TextSnapshot;
							string documentText = textSnapshot.GetText();

							// Update the document with the new text
							using ITextEdit edit = textView.TextBuffer.CreateEdit();
							edit.Replace(0, textSnapshot.Length, newDocumentText);
							edit.Apply();
						}
						else
						{
							System.Diagnostics.Debug.WriteLine($"Document {filePath} not found in the project.");
						}
					}
					else
					{
						System.Diagnostics.Debug.WriteLine($"Project containing the file {filePath} not found in the solution.");
					}
				}
			}
		}

		private static bool IsCSharpFile(string filePath)
		{
			return Path.GetExtension(filePath).Equals(".cs", StringComparison.OrdinalIgnoreCase);
		}
	}
}
