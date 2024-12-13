using System.Text;

namespace Mp3TagReader {
	// APEv2 specification
	// https://wiki.hydrogenaud.io/index.php?title=APEv2_specification
	internal class ApeV2Tag : ITag {
		private class ApeHeaderFooter {
			public const string ApePreamble = "APETAGEX";
			public const int HeaderFooterSize = 32;

			public int OffsetFromEnd { get; set; }
			public decimal Version { get; set; }
			public int TagSize { get; set; }
			public int ItemCount { get; set; }
			public bool HasHeader { get; set; }
			public bool HasFooter { get; set; }
			public bool IsHeader { get; set; }
		}
		
		private readonly IResourceManager resourceManager;

		public ApeV2Tag( IResourceManager resourceManager ) {
			this.resourceManager = resourceManager;

			Items = [];
		}

		public string Type => "APEv2";

		public decimal Version { get; private set; }

		public int ItemCount { get; private set; }
		
		public List<KeyValuePair<string,string>> Items { get; }

		private ApeHeaderFooter? LocateFooter( BinaryReader br ) {
			var footerData = new byte[ApeHeaderFooter.HeaderFooterSize];

			// Look for the 32 byte Ape footer:
			// - First, at the end of the file (when no ID3V1 tag is present)
			// - Next, ahead of a possibly-present 128 byte ID3V1 tag (located at the end of the file)
			foreach ( var tryOffset in new[] { 0, -128 } ) {
				var offset = tryOffset - footerData.Length;
				
				br.BaseStream.Seek( offset, SeekOrigin.End );
				br.Read( footerData, 0, footerData.Length );

				var preambleReader = new StringReader( footerData, 0, 7, Encoding.Latin1 );
				var preamble = preambleReader.ReadString();

				if ( preamble == ApeHeaderFooter.ApePreamble ) {
					// APE Tag Footer
					// https://wiki.hydrogenaud.io/index.php?title=APE_Tags_Header

					var footer = new ApeHeaderFooter {
						OffsetFromEnd = offset,
						Version = ReadApeNumber( footerData, 8 ) / 1000M,
						TagSize = ReadApeNumber( footerData, 12 ),
						ItemCount = ReadApeNumber( footerData, 16 )
					};

					// Ape Tag Flags
					// https://wiki.hydrogenaud.io/index.php?title=Ape_Tags_Flags
					var footerFlags = ReadApeNumber( footerData, 20 );
					footer.HasHeader = ( footerFlags & ( 1 << 31 ) ) != 0;
					footer.HasFooter = ( footerFlags & ( 1 << 30 ) ) != 0;
					footer.IsHeader = ( footerFlags & ( 1 << 29 ) ) != 0;

					return footer;
				}
			}

			return null;
		}

		private static int ReadApeNumber( byte[] data, int index ) {
			// Ape numbers are stored in little endian
			return BitConverter.IsLittleEndian ? BitConverter.ToInt32( data, index ) : BitConverter.ToInt32( data.Skip( index ).Take( 4 ).Reverse().ToArray(), 0 );
		}
		private List<KeyValuePair<string, string>> ReadItems(ApeHeaderFooter footer, BinaryReader br) {
			var items = new List<KeyValuePair<string, string>>();

			// The tag size excludes the header but includes the footer so subtract the footer size to get the items size
			var itemsSize = footer.TagSize - ApeHeaderFooter.HeaderFooterSize;

			// Read items above the footer and below the header. 
			br.BaseStream.Seek( footer.OffsetFromEnd - itemsSize, SeekOrigin.End );

			var itemsData = new byte[itemsSize];
			br.Read( itemsData, 0, itemsData.Length );

			var currentIndex = 0;

			// APE Tag Item
			// https://wiki.hydrogenaud.io/index.php?title=APE_Tag_Item
			for ( var i = 0; i < ItemCount; i++ ) {
				var itemLength = ReadApeNumber( itemsData, currentIndex );
				currentIndex += 4;

				// Ape Item Flags
				// https://wiki.hydrogenaud.io/index.php?title=Ape_Tags_Flags
				var itemFlags = ReadApeNumber( itemsData, currentIndex );
				var valueIsBinary = ( itemFlags & ( 1 << 1 ) ) != 0;
				currentIndex += 4;

				// APE key
				// https://wiki.hydrogenaud.io/index.php?title=APE_key
				var itemKeyReader = new StringReader( itemsData, currentIndex, itemsData.Length - 1, Encoding.Latin1 );
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
					var itemValueReader = new StringReader( itemsData, currentIndex, currentIndex + itemLength - 1, Encoding.UTF8, false );
					itemValue = itemValueReader.ReadString();
					currentIndex = itemValueReader.CurrentIndex;
				}

				items.Add( new KeyValuePair<string, string>( itemKey, itemValue ) );
			}

			return items;
		}
		public bool ReadTag( string mp3File ) {
			using var fs = File.Open( mp3File, FileMode.Open, FileAccess.Read, FileShare.Read );

			using var br = new BinaryReader( fs );

			var footer = LocateFooter( br );

			if ( footer != null ) {
				Version = footer.Version;
				ItemCount = footer.ItemCount;

				Items.AddRange( ReadItems( footer, br ) );

				return true;
			}

			return false;
		}
	}
}
