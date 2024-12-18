using NoVar.src.Helpers;

namespace NoVar
{
	[Command(PackageIds.ReplaceAll)]
	internal sealed class ReplaceAll : BaseCommand<ReplaceAll>
	{
		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			try
			{
				// Get current document
				DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync().ConfigureAwait(false);
				if (docView != null)
				{
					await VarReplacerHelper.ReplaceVarsAsync(docView.FilePath, docView.TextView, replaceAll: true);
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
