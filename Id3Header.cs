namespace Mp3TagReader {
	internal class Id3Header {
		public Id3Header( BinaryReader binaryReader) {
			ReadHeader( binaryReader );
		}

		public ulong Size { get; private set; }

		public string Version { get; private set; }
		public bool Unsynchronisation { get; private set; }
		public bool ExtendedHeader { get; private set; }
		public bool Experimental { get; private set; }

		// ID3v2 header
		// https://id3.org/id3v2.3.0#ID3v2_header
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

			Size = ConvertRawSize( id3SizeRaw );

			Unsynchronisation = ( id3Flags[0] & 0b1000000 ) != 0;
			ExtendedHeader = ( id3Flags[0] & 0b100000 ) != 0;
			Experimental = ( id3Flags[0] & 0b10000 ) != 0;

			if ( ExtendedHeader ) {
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
