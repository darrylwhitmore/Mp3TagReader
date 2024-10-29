using System.Text;
using System.Text.Unicode;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mp3TagReader {
	internal class Id3TextInformationFrame : Id3Frame {
		public Id3TextInformationFrame( string frameId, string frameIdName, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
			FrameIdName = frameIdName;

			ReadText( binaryReader );
		}

		public string Text { get; private set; }

		private void ReadText( BinaryReader binaryReader ) {
			var frameBody = new byte[FrameSize];

			binaryReader.Read( frameBody, 0, frameBody.Length );

			var encoding = GetEncoding( frameBody[0] );
			var bytesLength = frameBody.Length - 1;

			if ( Equals( encoding, Encoding.Unicode ) ) {
				encoding = GetUtf16BomEncoding( frameBody[1] );
				bytesLength = frameBody.Length - 3;
			}

			Text = encoding.GetString( frameBody.TakeLast( bytesLength ).ToArray() );
		}

		// 4.   ID3v2 frame overview
		// https://id3.org/id3v2.4.0-structure
		private Encoding GetEncoding( byte encodingByte ) {
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

		// 4.   ID3v2 frame overview
		// https://id3.org/id3v2.4.0-structure
		private Encoding GetUtf16BomEncoding( byte bomByte ) {
			switch ( bomByte ) {
				case 0xFF:
					return Encoding.Unicode;

				case 0xFE:
					return Encoding.BigEndianUnicode;

				default:
					throw new ArgumentException( $"Unknown BOM: {bomByte:X}" );
			}
		}
	}
}
