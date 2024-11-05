using System.Text;

namespace Mp3TagReader {
	internal class StringReader {
		private readonly byte[] sourceBytes;
		private readonly Encoding givenEncoding;
		private readonly int maxIndex;

		public StringReader( byte[] sourceBytes, int startIndex, int maxIndex, Encoding encoding ) {
			this.sourceBytes = sourceBytes;
			CurrentIndex = startIndex;
			this.maxIndex = maxIndex;
			givenEncoding = encoding;
		}

		public int CurrentIndex { get; private set; }

		public string ReadString() {
			var encoding = givenEncoding;

			if ( Equals( encoding, Encoding.Unicode ) ) {
				encoding = GetUtf16BomEncoding( sourceBytes[CurrentIndex] );
			}

			var preamble = encoding.GetPreamble();

			CurrentIndex += preamble.Length;

			var stringBytes = new List<byte>();
			var testString = string.Empty;

			// Sometimes strings are null-terminated, and sometimes they
			// terminate at the end of the range of source bytes. Read 
			// until we hit either.
			//
			// A null terminator byte(s) is difficult to detect because characters 
			// may be multibyte, nulls can be part of the character bytes, and
			// endian-ness can place null character bytes next to null 
			// terminator bytes. We use a brute force method of decoding and
			// building the string character by character until we find a
			// decoded null character.
			for ( var i = CurrentIndex; i <= maxIndex; i++ ) {
				stringBytes.Add( sourceBytes[i] );

				testString = encoding.GetString( stringBytes.ToArray() );

				if ( testString.EndsWith( '\0' ) ) {
					testString = testString.TrimEnd( '\0' );
					break;
				}
			}

			CurrentIndex += stringBytes.Count;

			return testString;
		}

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
