using System.Text;

namespace Mp3TagReader {
	internal class StringReader {
		private readonly byte[] sourceBytes;
		private readonly Encoding givenEncoding;
		private readonly int maxIndex;

		public StringReader( byte[] sourceBytes, int startIndex, int maxIndex, Encoding encoding ) {
			givenEncoding = encoding;

			//if ( Equals( encoding, Encoding.Unicode ) ) {
			//	givenEncoding = GetUtf16BomEncoding( sourceBytes[startIndex] );
			//}

			//var preamble = givenEncoding.GetPreamble();

			this.sourceBytes = sourceBytes;
			//CurrentIndex = startIndex + preamble.Length;
			CurrentIndex = startIndex;
			this.maxIndex = maxIndex;


			//var terminatorBytes = encoding.GetBytes( $"{'\0'}" );

			//
			//var text = ReadString();
			//

			//var terminatorIndex = FindTerminatorIndex( terminatorBytes, frameBody, stringStartIndex, stringEndIndex );

			//if ( terminatorIndex == -1 ) {
			//	// The string was not terminated
			//	terminatorIndex = stringEndIndex;
			//}

			//var s = encoding.GetString( frameBody.Skip( stringStartIndex ).Take( terminatorIndex - stringStartIndex ).ToArray() );

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

		private int FindTerminatorIndex( byte[] terminatorBytes, byte[] frameBody, int minIndex, int maxIndex ) {
			for ( var i = maxIndex - terminatorBytes.Length + 1; i >= minIndex; i-- ) {
				var found = true;

				for ( var j = 0; j < terminatorBytes.Length; j++ ) {
					if ( frameBody[i + j] != terminatorBytes[j] ) {
						found = false;
						break;
					}
				}

				if ( found ) {
					return i;
				}
			}

			// Not found
			return -1; 
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
