using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// Attached picture
	// https://id3.org/id3v2.3.0#Attached_picture
	internal sealed class Id3AttachedPictureFrame : Id3Frame {
		public Id3AttachedPictureFrame( string frameId, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string MimeType { get; private set; }

		[JsonProperty( Order = 2 )]
		public string PictureType { get; private set; }

		[JsonProperty( Order = 3 )]
		public string Description { get; private set; }

		[JsonProperty( Order = 4 )]
		public int PictureDataLength { get; private set; }

		protected override void ProcessFrameBody() {
			var encoding = GetEncoding( FrameBody[0] );

			var stringReader = new StringReader( FrameBody, 1, FrameBody.Length - 1, encoding );

			MimeType = stringReader.ReadString();

			var pictureType = FrameBody[stringReader.CurrentIndex];
			var pictureTypeDesc = GetResourceString( $"{pictureType:X2}" );

			PictureType = $"0x{pictureType:X2} ({pictureTypeDesc})";

			stringReader = new StringReader( FrameBody, stringReader.CurrentIndex + 1, FrameBody.Length - 1, encoding );

			Description = stringReader.ReadString();

			PictureDataLength = FrameBody.Length - stringReader.CurrentIndex;
		}
	}
}
