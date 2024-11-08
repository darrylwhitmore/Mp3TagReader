namespace Mp3TagReader {
	// ID3v2 header
	// https://id3.org/id3v2.3.0#ID3v2_header
	//
	// Hat tip to this very old question on StackOverflow for inspiration and
	// a starting point:
	//
	// How to read Id3v2 tag
	// https://stackoverflow.com/questions/16399604/how-to-read-id3v2-tag
	internal class Id3Header {
		public Id3Header( BinaryReader binaryReader) {
			ReadHeader( binaryReader );
		}

		public ulong HeaderSize => 10;

		public ulong FramesSize { get; private set; }

		public string Version { get; private set; }

		public string Flags { get; private set; }

		private void ReadHeader( BinaryReader binaryReader ) {
			var id3Id = new byte[3];
			var id3Version = new byte[2];
			var id3Flags = new byte[1];
			var id3SizeRaw = new byte[4];

			binaryReader.Read( id3Id, 0, id3Id.Length );
			binaryReader.Read( id3Version, 0, id3Version.Length );
			binaryReader.Read( id3Flags, 0, id3Flags.Length );
			binaryReader.Read( id3SizeRaw, 0, id3SizeRaw.Length );

			Version = $"ID3v2.{id3Version[0]}.{id3Version[1]}";

			FramesSize = ConvertRawSize( id3SizeRaw );

			var unsynchronisationFlag = ( id3Flags[0] & 0b1000000 ) != 0;
			var extendedHeaderFlag = ( id3Flags[0]    & 0b0100000 ) != 0;
			var experimentalFlag = ( id3Flags[0]      & 0b0010000 ) != 0;
			Flags = $"{( unsynchronisationFlag ? "U" : "u" )}{( extendedHeaderFlag ? "E" : "e" )}{( experimentalFlag ? "X" : "x" )}.....";

			if ( extendedHeaderFlag ) {
				// ID3v2 extended header
				// https://id3.org/id3v2.3.0#ID3v2_extended_header
				throw new NotImplementedException( "Extended headers are not currently implemented" );
			}
		}
		private ulong ConvertRawSize( byte[] rawSize ) {
			return ( ulong )rawSize[0] << 21 | ( ulong )rawSize[1] << 14 | ( ulong )rawSize[2] << 7 | rawSize[3];
		}
	}
}
