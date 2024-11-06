using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	internal sealed class Id3UserDefinedUrlLinkFrame : Id3Frame {
		public Id3UserDefinedUrlLinkFrame( string frameId, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Description { get; private set; }

		[JsonProperty( Order = 2 )]
		public string Url { get; private set; }

		protected override void ProcessFrameBody() {
			var encoding = GetEncoding( FrameBody[0] );

			var stringReader = new StringReader( FrameBody, 1, FrameBody.Length - 1, encoding );

			Description = stringReader.ReadString();

			stringReader = new StringReader( FrameBody, stringReader.CurrentIndex, FrameBody.Length - 1, Encoding.Latin1 );

			Url = stringReader.ReadString();
		}
	}
}
