using NoVar.src.Helpers;

namespace NoVar.Commands
{
	[Command(PackageIds.ReplaceNotNewVars)]
	internal sealed class ReplaceNotNewVars : BaseCommand<ReplaceNotNewVars>
	{
		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			try
			{
				// Get current document
				DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync().ConfigureAwait(false);
				if (docView != null)
				{
					await VarReplacerHelper.ReplaceVarsAsync(docView.FilePath, docView.TextView, replaceAll: false);
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
