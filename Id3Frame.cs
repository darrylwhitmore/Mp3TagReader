namespace Mp3TagReader {
	internal abstract class Id3Frame {
		private Id3Frame( BinaryReader binaryReader ) {
			BinaryReader = binaryReader;
		}

		public static Id3Frame GetNextFrame( BinaryReader binaryReader ) {
			return null; //new Id3Frame(binaryReader);
		}
		protected BinaryReader BinaryReader { get; private set; }

	}
}
