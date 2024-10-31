using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader {
	// Private frame
	// https://id3.org/id3v2.3.0#Private_frame
	internal class Id3PrivateFrame : Id3Frame {
		public Id3PrivateFrame( string frameId, string frameIdName, BinaryReader binaryReader ) : base( frameId, frameIdName, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string OwnerIdentifier { get; private set; }

		[JsonProperty( Order = 2 )]
		public int PrivateDataLength { get; private set; }

		protected override void ProcessFrameBody() {
			OwnerIdentifier = Encoding.Latin1.GetString( FrameBody.TakeWhile( b => b != 0 ).ToArray() );

			PrivateDataLength = FrameBody.Length - OwnerIdentifier.Length - 1;
		}
	}
}
