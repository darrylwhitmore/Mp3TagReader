namespace Mp3TagReader {
	internal class Id3Tag {
		private readonly BinaryReader binaryReader;
		public readonly List<Id3Frame> frames;
		public Id3Tag( BinaryReader binaryReader ) {
			this.binaryReader = binaryReader;

			Header = new Id3Header( binaryReader );

			frames = new List<Id3Frame>();
			while ( Id3Frame.GetNextFrame( binaryReader ) is { } frame ) {
				frames.Add( frame );
			}
		}

		public Id3Header Header { get; private set; }
		public List<Id3Frame> Frames => frames;
	}
}
