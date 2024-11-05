using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// ID3v2 frame overview
	// https://id3.org/id3v2.3.0#ID3v2_frame_overview
	internal abstract class Id3Frame {
		protected Id3Frame( string frameId, string frameIdName, BinaryReader binaryReader ) {
			FrameId = frameId;
			FrameIdDisplay = $"{FrameId} ({frameIdName})";

			var frameSizeRaw = new byte[4];
			var frameFlags = new byte[2];

			binaryReader.Read( frameSizeRaw, 0, frameSizeRaw.Length );
			binaryReader.Read( frameFlags, 0, frameFlags.Length );

			FrameSize = ( ulong )frameSizeRaw[0] << 24 | ( ulong )frameSizeRaw[1] << 16 | ( ulong )( frameSizeRaw[2] << 8 ) | frameSizeRaw[3];

			FrameBody = new byte[FrameSize];
			binaryReader.Read( FrameBody, 0, FrameBody.Length );

			// Frame header flags
			// https://id3.org/id3v2.3.0#Frame_header_flags
			var tagAlterPreservationFlag = ( frameFlags[0]  & 0b1000000 ) != 0;
			var fileAlterPreservationFlag = ( frameFlags[0] & 0b0100000 ) != 0;
			var readOnlyFlag = ( frameFlags[0]              & 0b0010000 ) != 0;
			var compressionFlag = ( frameFlags[1]           & 0b1000000 ) != 0;
			var encryptionFlag = ( frameFlags[1]            & 0b0100000 ) != 0;
			var groupingIdentityFlag = ( frameFlags[1]      & 0b0010000 ) != 0;

			Flags = $"{( tagAlterPreservationFlag ? "T" : "t" )}{( fileAlterPreservationFlag ? "F" : "f" )}{( readOnlyFlag ? "R" : "r" )}..... {( compressionFlag ? "C" : "c" )}{( encryptionFlag ? "E" : "e" )}{( groupingIdentityFlag ? "G" : "g" )}.....";
		}

		[JsonIgnore]
		public string FrameId { get; }

		[JsonProperty( PropertyName = "FrameId" )]
		protected string FrameIdDisplay { get; set; }

		protected byte[] FrameBody { get; set; }

		public ulong FrameSize { get; }

		public string Flags { get; private set; }

		// ID3v2 frame overview
		// https://id3.org/id3v2.4.0-structure
		protected Encoding GetEncoding( byte encodingByte ) {
			switch ( encodingByte ) {
				case 0x0:
					// ISO-8859-1
					return Encoding.Latin1;

				case 0x1:
					// UTF-16 with BOM
					return Encoding.Unicode;

				case 0x2:
					// UTF-16 without BOM
					return Encoding.BigEndianUnicode;

				case 0x3:
					// UTF-8
					return Encoding.UTF8;

				default:
					throw new ArgumentException( $"Unknown encoding byte: {encodingByte:X}" );
			}
		}

		protected abstract void ProcessFrameBody();

		public static Id3Frame? GetNextFrame( BinaryReader binaryReader ) {
			var frameIdRaw = new byte[4];

			binaryReader.Read( frameIdRaw, 0, frameIdRaw.Length );

			var frameId = Encoding.ASCII.GetString( frameIdRaw );

			if (frameId == "\0\0\0\0" ) {
				// We're in the padding, there are no more frames
				return null;
			}

			// Declared ID3v2 frames
			// https://id3.org/id3v2.3.0#Declared_ID3v2_frames
			switch ( frameId ) {
				case "COMM":
					return new Id3CommentFrame( frameId, binaryReader );

				case "PRIV":
					return new Id3PrivateFrame( frameId, binaryReader );

				case "TALB":
					return new Id3TextInformationFrame( frameId, "Album/Movie/Show title", binaryReader );

				case "TCOM":
					return new Id3TextInformationFrame( frameId, "Composer", binaryReader );

				case "TCON":
					return new Id3TextInformationFrame( frameId, "Content type", binaryReader );

				case "TCOP":
					return new Id3TextInformationFrame( frameId, "Copyright message", binaryReader );

				case "TIT2":
					return new Id3TextInformationFrame( frameId, "Title/songname/content description", binaryReader );

				case "TLEN":
					return new Id3TextInformationFrame( frameId, "Length", binaryReader );

				case "TMED":
					return new Id3TextInformationFrame( frameId, "Media type", binaryReader );

				case "TPE1":
					return new Id3TextInformationFrame( frameId, "Lead performer(s)/Soloist(s)", binaryReader );

				case "TPE2":
					return new Id3TextInformationFrame( frameId, "Band/orchestra/accompaniment", binaryReader );

				case "TPE3":
					return new Id3TextInformationFrame( frameId, "Conductor/performer refinement", binaryReader );

				case "TPOS":
					return new Id3TextInformationFrame( frameId, "Part of a set", binaryReader );

				case "TPUB":
					return new Id3TextInformationFrame( frameId, "Publisher", binaryReader );

				case "TRCK":
					return new Id3TextInformationFrame( frameId, "Track number/Position in set", binaryReader );

				case "TSO2":
					// Off-Spec Frames
					// https://mutagen-specs.readthedocs.io/en/latest/id3/id3v2-other-frames.html
					return new Id3TextInformationFrame( frameId, "iTunes Album Artist Sort", binaryReader );

				case "TYER":
					return new Id3TextInformationFrame( frameId, "Year", binaryReader );

				case "TXXX":
					return new Id3UserDefinedTextInformationFrame( frameId, binaryReader );

				default:
					return new Id3UnimplementedFrame( frameId, binaryReader );
			}
		}
	}
}
