namespace Mp3TagReader {
	internal class Id3Tag {
		public Id3Tag( BinaryReader binaryReader ) {
			Header = new Id3Header( binaryReader );

			ReadFrames( binaryReader );
		}

		public Id3Header Header { get; private set; }
		public List<Id3Frame> Frames { get; } = [];

		private void ReadFrames( BinaryReader binaryReader ) {
			var leftToRead = Header.Size;

			while ( Id3Frame.GetNextFrame( binaryReader ) is { } frame ) {
				Frames.Add( frame );

				leftToRead -= frame.FrameSize;
			}
		}
	}
}
