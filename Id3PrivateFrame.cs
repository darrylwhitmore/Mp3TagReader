using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader {
	internal class Id3PrivateFrame : Id3Frame {
		public Id3PrivateFrame( string frameId, string frameIdName, BinaryReader binaryReader ) : base( frameId, frameIdName, binaryReader ) {
			ReadFrame( binaryReader );
		}

		[JsonProperty( Order = 1 )]
		public string OwnerIdentifier { get; private set; }

		[JsonProperty( Order = 2 )]
		public int BinaryDataLength { get; private set; }

		private void ReadFrame( BinaryReader binaryReader ) {
			var frameBody = new byte[FrameSize];

			binaryReader.Read( frameBody, 0, frameBody.Length );

			OwnerIdentifier = Encoding.Latin1.GetString( frameBody.TakeWhile( b => b != 0 ).ToArray() );

			BinaryDataLength = frameBody.Length - OwnerIdentifier.Length - 1;
		}
	}
}
