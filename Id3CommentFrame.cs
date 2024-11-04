using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader {
	// Comments
	// https://id3.org/id3v2.3.0#Comments
	internal class Id3CommentFrame : Id3Frame {
		public Id3CommentFrame( string frameId, BinaryReader binaryReader ) : base( frameId, "Comments", binaryReader ) {
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

			var headerEncoding = GetEncoding( FrameBody[currentIndex] );
			currentIndex++;

			var languageReader = new StringReader( FrameBody, currentIndex, currentIndex + 2, Encoding.Latin1 );
			Language = languageReader.ReadString();

			var textReader = new StringReader( FrameBody, languageReader.CurrentIndex, FrameBody.Length - 1, headerEncoding );
			ShortContentDesc = textReader.ReadString();

			Text = textReader.ReadString();

			//Language = Encoding.Latin1.GetString( FrameBody.Skip( currentIndex ).Take( 3 ).ToArray() );
			//currentIndex += 3;

			//var encoding = headerEncoding;

			//if ( Equals( headerEncoding, Encoding.Unicode ) ) {
			//	encoding = GetUtf16BomEncoding( FrameBody[currentIndex] );
			//	currentIndex += 2;
			//}

			//ShortContentDesc = encoding.GetString( FrameBody.Skip( currentIndex ).TakeWhile( b => b != 0 ).ToArray() );

			//currentIndex += ShortContentDesc.Length + 2;

			//encoding = headerEncoding;

			//if ( Equals( headerEncoding, Encoding.Unicode ) ) {
			//	encoding = GetUtf16BomEncoding( FrameBody[currentIndex] );
			//	currentIndex += 2;
			//}

			//Text = encoding.GetString( FrameBody.Skip( currentIndex ).TakeWhile( b => b != 0 ).ToArray() );


		}
	}
}
