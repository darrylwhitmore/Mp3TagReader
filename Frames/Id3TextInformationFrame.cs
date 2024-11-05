using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	internal sealed class Id3TextInformationFrame : Id3Frame {
		public Id3TextInformationFrame( string frameId, string frameIdName, BinaryReader binaryReader ) : base( frameId, frameIdName, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Text { get; private set; }

		protected override void ProcessFrameBody() {
			var encoding = GetEncoding( FrameBody[0] );

			var stringReader = new StringReader( FrameBody, 1, FrameBody.Length - 1, encoding );

			Text = stringReader.ReadString();
		}
	}
}
