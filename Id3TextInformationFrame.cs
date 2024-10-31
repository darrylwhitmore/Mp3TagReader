using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader {
	internal class Id3TextInformationFrame : Id3Frame {
		public Id3TextInformationFrame( string frameId, string frameIdName, BinaryReader binaryReader ) : base( frameId, frameIdName, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Text { get; private set; }

		protected override void ProcessFrameBody() {
			var encoding = GetEncoding( FrameBody[0] );
			var bytesLength = FrameBody.Length - 1;

			if ( Equals( encoding, Encoding.Unicode ) ) {
				encoding = GetUtf16BomEncoding( FrameBody[1] );
				bytesLength = FrameBody.Length - 3;
			}

			Text = encoding.GetString( FrameBody.TakeLast( bytesLength ).ToArray() );
		}

		// ID3v2 frame overview
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

		// ID3v2 frame overview
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
