namespace Mp3TagReader {
	internal class Id3Tag {
		private readonly BinaryReader binaryReader;
		private Id3Header header;
		private readonly List<Id3Frame> frames = [];
		public Id3Tag( BinaryReader binaryReader ) {
			this.binaryReader = binaryReader;

			header = new Id3Header( binaryReader );

			while ( Id3Frame.GetNextFrame( binaryReader ) is { } frame ) {
				frames.Add( frame );
			}
		}
	}
}
