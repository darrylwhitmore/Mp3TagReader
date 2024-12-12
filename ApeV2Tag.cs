using System.Text;

namespace Mp3TagReader {
	// APEv2 specification
	// https://wiki.hydrogenaud.io/index.php?title=APEv2_specification
	internal class ApeV2Tag : ITag {
		private const string ApePreamble = "APETAGEX";
		private const int FooterSize = 32;

		private readonly IResourceManager resourceManager;

		public ApeV2Tag( IResourceManager resourceManager ) {
			this.resourceManager = resourceManager;

			Items = [];
		}

		public string Type => "APEv2";

		public decimal Version { get; private set; }

		public int ItemCount { get; private set; }
		
		public List<KeyValuePair<string,string>> Items { get; }

		public bool ReadTag( string mp3File ) {
			var tagFound = false;

			using var fs = File.Open( mp3File, FileMode.Open, FileAccess.Read, FileShare.Read );

			using var br = new BinaryReader( fs );

			var footerData = new byte[FooterSize];
			var tagSize = 0;

			// Look for the 32 byte Ape footer:
			// - First, at the end of the file (when no ID3V1 tag is present)
			// - Next, ahead of a possibly-present 128 byte ID3V1 tag (located at the end of the file)
			foreach ( var offset in new[] { 0, -128 } ) {
				br.BaseStream.Seek( offset - footerData.Length, SeekOrigin.End );
				br.Read( footerData, 0, footerData.Length );

				var preambleReader = new StringReader( footerData, 0, 7, Encoding.Latin1 );
				var preamble = preambleReader.ReadString();

				if ( preamble == ApePreamble ) {
					// APE Tag Footer
					// https://wiki.hydrogenaud.io/index.php?title=APE_Tags_Header

					Version = ReadApeNumber( footerData, 8 ) / 1000M;

					tagSize = ReadApeNumber( footerData, 12 );

					ItemCount = ReadApeNumber( footerData, 16 );

					// Ape Tag Flags
					// https://wiki.hydrogenaud.io/index.php?title=Ape_Tags_Flags
					var footerFlags = ReadApeNumber( footerData, 20 );
					var hasHeader = ( footerFlags & ( 1 << 31 ) ) != 0;
					var hasFooter = ( footerFlags & ( 1 << 30 ) ) != 0;
					var isHeader = ( footerFlags & ( 1 << 29 ) ) != 0;
					
					tagFound = true;
					break;
				}
			}

			if ( tagFound && ItemCount > 0 ) {
				// Read items above the footer and below the header. The tag size excludes the header but includes the footer.
				// We are now at the end of the footer, so seeking back the tag size will position us on the first item.
				br.BaseStream.Seek( -tagSize, SeekOrigin.Current );

				var itemData = new byte[tagSize - FooterSize];
				br.Read( itemData, 0, itemData.Length );

				var currentIndex = 0;

				// APE Tag Item
				// https://wiki.hydrogenaud.io/index.php?title=APE_Tag_Item
				for ( var i = 0; i < ItemCount; i++ ) {
					var itemLength = ReadApeNumber( itemData, currentIndex );
					currentIndex += 4;

					// Ape Item Flags
					// https://wiki.hydrogenaud.io/index.php?title=Ape_Tags_Flags
					var itemFlags = ReadApeNumber( itemData, currentIndex );
					var valueIsBinary = ( itemFlags & ( 1 << 1 ) ) != 0;
					currentIndex += 4;

					// APE key
					// https://wiki.hydrogenaud.io/index.php?title=APE_key
					var itemKeyReader = new StringReader( itemData, currentIndex, itemData.Length - 1, Encoding.Latin1 );
					var itemKey = itemKeyReader.ReadString();
					currentIndex = itemKeyReader.CurrentIndex;

					// APE Item Value
					// https://wiki.hydrogenaud.io/index.php?title=APE_Item_Value
					string itemValue;
					if ( valueIsBinary ) {
						itemValue = $"{itemLength} bytes of binary data";
						currentIndex += itemLength;
					}
					else {
						var itemValueReader = new StringReader( itemData, currentIndex, currentIndex + itemLength - 1, Encoding.UTF8, false );
						itemValue = itemValueReader.ReadString();
						currentIndex = itemValueReader.CurrentIndex;
					}

					Items.Add( new KeyValuePair<string, string>( itemKey, itemValue ) );
				}
			}

			return tagFound;
		}

		private static int ReadApeNumber( byte[] data, int index ) {
			// Ape numbers are stored in little endian
			return BitConverter.IsLittleEndian ? BitConverter.ToInt32( data, index ) : BitConverter.ToInt32( data.Skip( index ).Take( 4 ).Reverse().ToArray(), 0 );
		}
	}
}
