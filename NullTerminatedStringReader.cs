using System.Text;

namespace Mp3TagReader {
	internal class NullTerminatedStringReader {
		private readonly Encoding encoding;

		public NullTerminatedStringReader( byte[] frameBody, int startIndex, Encoding givenEncoding ) {
			encoding = givenEncoding;

			if ( Equals( givenEncoding, Encoding.Unicode ) ) {
				encoding = GetUtf16BomEncoding( frameBody[startIndex] );
			}

			var preamble = encoding.GetPreamble();

			var stringStartIndex = startIndex + preamble.Length;
			var stringEndIndex = frameBody.Length - 1;
			var terminatorBytes = encoding.GetBytes( $"{'\0'}" );

			var terminatorIndex = FindTerminatorIndex( terminatorBytes, frameBody, stringStartIndex, stringEndIndex );

			if ( terminatorIndex == -1 ) {
				// The string was not terminated
				terminatorIndex = stringEndIndex;
			}

			var s = encoding.GetString( frameBody.Skip( stringStartIndex ).Take( terminatorIndex - stringStartIndex ).ToArray() );

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


////
/*
	encoding.GetBytes($"{'\0'}")
   {byte[2]}
	   [0]: 0
	   [1]: 0
   Encoding.Latin1.GetBytes($"{'\0'}")
   {byte[1]}
	   [0]: 0
   Encoding.UTF8.GetBytes($"{'\0'}")
   {byte[1]}
	   [0]: 0
   Encoding.BigEndianUnicode.GetBytes($"{'\0'}")
   {byte[2]}
	   [0]: 0
	   [1]: 0			*/

