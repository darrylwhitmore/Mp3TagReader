using System.Text;

namespace Mp3TagReader {
	// ID3v2 frame overview
	// https://id3.org/id3v2.3.0#ID3v2_frame_overview
	internal abstract class Id3Frame {
		protected Id3Frame( string frameId, BinaryReader binaryReader ) {
			FrameId = frameId;
			BinaryReader = binaryReader;

			var frameSizeRaw = new byte[4];
			var frameFlags = new byte[2];

			binaryReader.Read( frameSizeRaw, 0, frameSizeRaw.Length );
			binaryReader.Read( frameFlags, 0, frameFlags.Length );

			FrameSize = ( ulong )frameSizeRaw[0] << 24 | ( ulong )frameSizeRaw[1] << 16 | ( ulong )( frameSizeRaw[2] << 8 ) | frameSizeRaw[3];

			// Frame header flags
			// https://id3.org/id3v2.3.0#Frame_header_flags
			TagAlterPreservation = ( frameFlags[0]  & 0b1000000 ) == 0;
			FileAlterPreservation = ( frameFlags[0] & 0b0100000 ) == 0;
			ReadOnly = ( frameFlags[0]              & 0b0010000 ) != 0;
			Compression = ( frameFlags[1]           & 0b1000000 ) != 0;
			Encryption = ( frameFlags[1]            & 0b0100000 ) != 0;
			GroupingIdentity = ( frameFlags[1]      & 0b0010000 ) != 0;
		}
		public string FrameId { get; }
		public string FrameIdName { get; protected set; }
		public ulong FrameSize { get; }
		public bool TagAlterPreservation { get; }
		public bool FileAlterPreservation { get; }
		public bool ReadOnly { get; }
		public bool Compression { get; }
		public bool Encryption { get; }
		public bool GroupingIdentity { get; }

		protected BinaryReader BinaryReader { get; private set; }

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
				case "TALB":
					return new Id3TextInformationFrame( frameId, "Album/Movie/Show title", binaryReader );

				case "TCON":
					return new Id3TextInformationFrame( frameId, "Content type", binaryReader );

				case "TIT2":
					return new Id3TextInformationFrame( frameId, "Title/songname/content description", binaryReader );

				case "TLEN":
					return new Id3TextInformationFrame( frameId, "Length", binaryReader );

				case "TPE1":
					return new Id3TextInformationFrame( frameId, "Lead performer(s)/Soloist(s)", binaryReader );

				case "TPE2":
					return new Id3TextInformationFrame( frameId, "Band/orchestra/accompaniment", binaryReader );

				case "TRCK":
					return new Id3TextInformationFrame( frameId, "Track number/Position in set", binaryReader );

				case "TYER":
					return new Id3TextInformationFrame( frameId, "Year", binaryReader );

				default:
					return new Id3UnimplementedFrame( frameId, binaryReader );
			}
		}
	}
}
