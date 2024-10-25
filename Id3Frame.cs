using System.Text;

namespace Mp3TagReader {
	internal abstract class Id3Frame {

		protected Id3Frame( string frameId, BinaryReader binaryReader ) {
			FrameId = frameId;
			BinaryReader = binaryReader;

			var frameSizeRaw = new byte[4];
			var frameFlags = new byte[2];

			binaryReader.Read( frameSizeRaw, 0, frameSizeRaw.Length );
			binaryReader.Read( frameFlags, 0, frameFlags.Length );

			FrameSize = ( ulong )frameSizeRaw[0] << 24 | ( ulong )frameSizeRaw[1] << 16 | ( ulong )( frameSizeRaw[2] << 8 ) | frameSizeRaw[3];

			FrameFlags = frameFlags;
		}
		public string FrameId { get; }
		public ulong FrameSize { get; }
		public byte[] FrameFlags { get; }
		protected BinaryReader BinaryReader { get; private set; }

		public static Id3Frame? GetNextFrame( BinaryReader binaryReader ) {
			var frameIdRaw = new byte[4];

			binaryReader.Read( frameIdRaw, 0, frameIdRaw.Length );

			var frameId = Encoding.ASCII.GetString( frameIdRaw );

			if (frameId == "\0\0\0\0" ) {
				// We're in the padding, there are no more frames
				return null;
			}

			switch ( frameId ) {
				default:
					return new Id3UnimplementedFrame( frameId, binaryReader );
			}
		}
	}
}
