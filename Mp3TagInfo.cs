namespace Mp3TagReader {
	internal class Mp3TagInfo {
		private readonly IResourceManager resourceManager;
		private readonly bool sortId3V2Frames;

		public Mp3TagInfo( string mp3File, IResourceManager resourceManager, bool sortId3V2Frames ) {
			Mp3File = mp3File;
			this.resourceManager = resourceManager;
			this.sortId3V2Frames = sortId3V2Frames;

			Tags = new List<ITag>();
		}

		public string Mp3File { get; }

		public List<ITag> Tags { get; private set; }
		
		public void LoadTags() {
			// Id3v2
			Tags.Add( new Id3V2Tag( Mp3File, resourceManager, sortId3V2Frames ) );
		}
	}
}
