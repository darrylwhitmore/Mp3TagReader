using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// Private frame
	// https://id3.org/id3v2.3.0#Private_frame
	internal sealed class Id3PrivateFrame : Id3Frame {
		public Id3PrivateFrame( string id, BinaryReader binaryReader ) : base( id, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string OwnerIdentifier { get; private set; }

		[JsonProperty( Order = 2 )]
		public int PrivateDataLength { get; private set; }

		protected override void ProcessFrameBody() {
			var stringReader = new StringReader( FrameBody, 0, FrameBody.Length - 1, Encoding.Latin1 );
			OwnerIdentifier = stringReader.ReadString();

			PrivateDataLength = FrameBody.Length - stringReader.CurrentIndex;
		}
	}
}
