using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader {
	internal class Id3TextInformationFrame : Id3Frame {
		public Id3TextInformationFrame( string frameId, string frameIdName, BinaryReader binaryReader ) : base( frameId, frameIdName, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Text { get; private set; }

		protected override void ProcessFrameBody() {
			var encoding = GetEncoding( FrameBody[0] );
			var bytesLength = FrameBody.Length - 1;

			var terminatedString = new NullTerminatedStringReader( FrameBody, 1, encoding );


			if ( Equals( encoding, Encoding.Unicode ) ) {
				encoding = GetUtf16BomEncoding( FrameBody[1] );
				bytesLength = FrameBody.Length - 3;
			}

			Text = encoding.GetString( FrameBody.TakeLast( bytesLength ).ToArray() );
		}
	}
}
