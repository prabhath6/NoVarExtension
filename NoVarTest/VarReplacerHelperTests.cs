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

		[Test]
		public async Task ReplaceVarsAsyncReplacesAllVarsAsync()
		{
			// Arrange
			var filePath = "test.cs";
			_textSnapshotMock.Setup(ts => ts.GetText()).Returns("var x = 10; var y = new MyClass();");

			// Act
			await VarReplacerHelper.ReplaceVarsAsync(filePath, _textViewMock.Object, replaceAll: true);

			// Assert
			_textEditMock.Verify(te => te.Replace(0, It.IsAny<int>(), It.Is<string>(s => s.Contains("int x = 10;"))), Times.Once);
			_textEditMock.Verify(te => te.Replace(0, It.IsAny<int>(), It.Is<string>(s => s.Contains("MyClass y = new MyClass();"))), Times.Once);
			_textEditMock.Verify(te => te.Apply(), Times.Once);
		}

		[Test]
		public async Task ReplaceVarsAsyncReplacesVarsNotInitializedWithNewAsync()
		{
			// Arrange
			var filePath = "test.cs";
			_textSnapshotMock.Setup(ts => ts.GetText()).Returns("var x = 10; var y = new MyClass();");

			// Act
			await VarReplacerHelper.ReplaceVarsAsync(filePath, _textViewMock.Object, replaceAll: false);

			// Assert
			_textEditMock.Verify(te => te.Replace(0, It.IsAny<int>(), It.Is<string>(s => s.Contains("int x = 10;"))), Times.Once);
			_textEditMock.Verify(te => te.Replace(0, It.IsAny<int>(), It.Is<string>(s => s.Contains("MyClass y = new MyClass();"))), Times.Never);
			_textEditMock.Verify(te => te.Apply(), Times.Once);
		}

		[Test]
		public async Task ReplaceVarsAsyncDoesNotReplaceInNonCSharpFileAsync()
		{
			// Arrange
			var filePath = "test.txt";
			_textSnapshotMock.Setup(ts => ts.GetText()).Returns("var x = 10; var y = new MyClass();");

			// Act
			await VarReplacerHelper.ReplaceVarsAsync(filePath, _textViewMock.Object, replaceAll: true);

			// Assert
			_textEditMock.Verify(te => te.Replace(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
			_textEditMock.Verify(te => te.Apply(), Times.Never);
		}
	}
}
