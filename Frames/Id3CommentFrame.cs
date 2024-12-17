using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// Comments
	// https://id3.org/id3v2.3.0#Comments
	internal sealed class Id3CommentFrame : Id3Frame {
		public Id3CommentFrame( string id, BinaryReader binaryReader, IResourceManager resourceManager ) : base( id, binaryReader, resourceManager ) {
			Language = string.Empty;
			ShortContentDesc = string.Empty;
			Text = string.Empty;
			
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Language { get; private set; }

		[JsonProperty( Order = 2 )]
		public string ShortContentDesc { get; private set; }

		[JsonProperty( Order = 3 )]
		public string Text { get; private set; }

		protected override void ProcessFrameBody() {
			var currentIndex = 0;

			var textEncoding = GetEncoding( FrameBody[currentIndex] );
			currentIndex++;

			var languageReader = new StringReader( FrameBody, currentIndex, currentIndex + 2, Encoding.Latin1 );
			Language = languageReader.ReadString();

			var textReader = new StringReader( FrameBody, languageReader.CurrentIndex, FrameBody.Length - 1, textEncoding );
			ShortContentDesc = textReader.ReadString();

			Text = textReader.ReadString();
		}
	}
}
