namespace Mp3TagReader {
	internal class Mp3TagInfo {
		private readonly IResourceManager resourceManager;
		private readonly bool sortId3V2Frames;

		public Mp3TagInfo( string mp3File, IResourceManager resourceManager, bool sortId3V2Frames ) {
			Mp3File = mp3File;
			this.resourceManager = resourceManager;
			this.sortId3V2Frames = sortId3V2Frames;

			Tags = [];
		}

		public string Mp3File { get; }

		public List<ITag> Tags { get; }
		
		public void LoadTags() {
			// Id3v2
			var id3V2Tag = new Id3V2Tag( resourceManager );
			if ( id3V2Tag.ReadTag( Mp3File, sortId3V2Frames ) ) {
				Tags.Add( id3V2Tag );
			}

			// Id3v1
			var id3V1Tag = new Id3V1Tag( resourceManager );
			if ( id3V1Tag.ReadTag( Mp3File ) ) {
				Tags.Add( id3V1Tag );
			}

			// APEv2
			var apeV2Tag = new ApeV2Tag( resourceManager );
			if ( apeV2Tag.ReadTag( Mp3File ) ) {
				Tags.Add( apeV2Tag );
			}
		}
	}
}
