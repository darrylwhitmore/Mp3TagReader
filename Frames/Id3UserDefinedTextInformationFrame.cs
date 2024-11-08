using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// User defined text information
	// https://id3.org/id3v2.3.0#User_defined_text_information_frame
	internal sealed class Id3UserDefinedTextInformationFrame : Id3Frame {
		public Id3UserDefinedTextInformationFrame( string id, BinaryReader binaryReader ) : base( id, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Description { get; private set; }

		[JsonProperty( Order = 2 )]
		public string Value { get; private set; }

		protected override void ProcessFrameBody() {
			var encoding = GetEncoding( FrameBody[0] );

			var stringReader = new StringReader( FrameBody, 1, FrameBody.Length - 1, encoding );

			Description = stringReader.ReadString();

			Value = stringReader.ReadString();
		}
	}
}
