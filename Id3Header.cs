using System.Collections;
using Mp3TagReader.Frames;

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

		public int HeaderSize => 10;

		public int FramesSize { get; private set; }

		public string Version { get; private set; }

		public List<string> Flags { get; } = [];

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

			FramesSize = DecodeFramesSize( id3SizeRaw );

			var unsynchronisationFlag = ( id3Flags[0] & 0b1000000 ) != 0;
			var extendedHeaderFlag = ( id3Flags[0]    & 0b0100000 ) != 0;
			var experimentalFlag = ( id3Flags[0]      & 0b0010000 ) != 0;

			if ( unsynchronisationFlag ) {
				AddFlag( "HeaderFlag:Unsynchronisation" );
			}
			
			if ( extendedHeaderFlag ) {
				AddFlag( "HeaderFlag:ExtendedHeader" );
			}
			
			if ( experimentalFlag ) {
				AddFlag( "HeaderFlag:Experimental" );
			}

			if ( extendedHeaderFlag ) {
				// ID3v2 extended header
				// https://id3.org/id3v2.3.0#ID3v2_extended_header
				throw new NotImplementedException( "Extended headers are not currently implemented" );
			}
		}

		private void AddFlag( string flagKey ) {
			var flag = Properties.Resources.ResourceManager.GetString( flagKey );

			Flags.Add( flag ?? $"Resource missing for key '{flagKey}'" );
		}

		private int DecodeFramesSize( byte[] bytes ) {
			var bitArray = new BitArray( 32 );
			var bitIndex = 4;

			// The total frame size bytes in the MP3 file are in big endian format:
			//
			// ID3v2 header
			// https://id3.org/id3v2.3.0#ID3v2_header
			//
			// In each byte, bit 7 will be unused. Conceptually, for each byte, we need to take 
			// out bit 7, push bits 6 through 0 to the right to butt up against the next byte's
			// bits, and then evaluate the value.
			foreach ( var b in bytes ) {
				// Populate a bit array to represent the decoded value. By setting
				// the appropriate bit in the array, the bits are "moved" to their
				// new destination. 
				for ( var mask = 64; mask >= 1; mask /= 2 ) {
					if ( ( b & mask ) == mask ) {
						bitArray.Set( bitIndex ^ 7, true );
					}

					bitIndex++;
				}
			}

			var newBytes = new byte[4];
			bitArray.CopyTo( newBytes, 0 );

			return BitConverter.IsLittleEndian ? BitConverter.ToInt32( newBytes.Reverse().ToArray(), 0 ) : BitConverter.ToInt32( newBytes, 0 );
		}
	}
}
