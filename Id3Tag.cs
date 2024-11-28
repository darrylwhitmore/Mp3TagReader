using Mp3TagReader.Frames;

namespace Mp3TagReader {
	// ID3 tag version 2.3.0
	// https://id3.org/id3v2.3.0#ID3_tag_version_2.3.0
	internal class Id3Tag {
		private readonly IResourceManager resourceManager;

		public Id3Tag( string mp3File, IResourceManager resourceManager, bool sortFrames = false ) {
			Mp3File = mp3File;
			this.resourceManager = resourceManager;

			using var fs = File.Open( mp3File, FileMode.Open, FileAccess.Read, FileShare.Read );

			using var br = new BinaryReader( fs );

			Header = new Id3Header( br, resourceManager );

			ReadFrames( br, sortFrames );
		}
		public string Mp3File { get; }

		public int TagSize => Header.HeaderSize + Header.FramesSize;

		public Id3Header Header { get; }

		public List<IFrame> Frames { get; } = [];

		private void ReadFrames( BinaryReader binaryReader, bool sortFrames ) {
			var workFrames = new List<IFrame>();
			var leftToRead = Header.FramesSize;

			while ( leftToRead > 0 && Id3Frame.GetNextFrame( binaryReader, resourceManager ) is { } frame ) {
				workFrames.Add( frame );

				leftToRead -= frame.Size;
			}

			if ( leftToRead > 0 ) {
				workFrames.Add( new PaddingPlaceholderFrame( leftToRead ) );
			}

			if ( sortFrames ) {
				Frames.AddRange( workFrames.OrderBy( f => f.Id ).ThenBy( f => f.Size ) );
			}
			else {
				Frames.AddRange( workFrames );
			}
		}
	}
}
