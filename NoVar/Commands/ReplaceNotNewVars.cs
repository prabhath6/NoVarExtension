using EnvDTE;
using Microsoft.VisualStudio.Threading;
using NoVar.src.Helpers;
using System.Threading.Tasks;

namespace NoVar.Commands
{
	[Command(PackageIds.ReplaceNotNewVars)]
	internal sealed class ReplaceNotNewVars : BaseCommand<ReplaceNotNewVars>
	{
		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			try
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				// Get the DTE object
				DTE dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
				string solutionPath = dte?.Solution?.FullName;

				if (string.IsNullOrEmpty(solutionPath))
				{
					System.Diagnostics.Debug.WriteLine("No solution is currently open.");
					return;
				}

				// Get current document
				DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
				if (docView != null)
				{
					await VarReplacerHelper.ReplaceVarsAsync(solutionPath, docView.FilePath, docView.TextView, replaceAll: false);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex.Message}");
				throw;
			}
		}
	}
}
