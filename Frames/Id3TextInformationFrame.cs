using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	internal sealed class Id3TextInformationFrame : Id3Frame {
		public Id3TextInformationFrame( string id, BinaryReader binaryReader, IResourceManager resourceManager ) : base( id, binaryReader, resourceManager ) {
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
