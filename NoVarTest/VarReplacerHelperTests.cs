using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Moq;
using NoVar.src.Helpers;

namespace NoVarTests
{
	[TestFixture]
	public class VarReplacerHelperTests
	{
		private Mock<IWpfTextView> _textViewMock;
		private Mock<ITextSnapshot> _textSnapshotMock;
		private Mock<ITextEdit> _textEditMock;

		[SetUp]
		public void SetUp()
		{
			_textViewMock = new Mock<IWpfTextView>();
			_textSnapshotMock = new Mock<ITextSnapshot>();
			_textEditMock = new Mock<ITextEdit>();

			_textViewMock.Setup(tv => tv.TextSnapshot).Returns(_textSnapshotMock.Object);
			_textViewMock.Setup(tv => tv.TextBuffer.CreateEdit()).Returns(_textEditMock.Object);
		}
	}
}
