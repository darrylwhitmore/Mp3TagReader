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
			var supportedTags = new List<ITag> {
				new Id3V2Tag( resourceManager, sortId3V2Frames ),
				new Id3V1Tag( resourceManager ),
				new ApeV2Tag( resourceManager )
			};

			foreach ( var tag in supportedTags.Where( t => t.ReadTag( Mp3File ) ) ) {
				Tags.Add( tag );
			}
		}
	}
}
