namespace Mp3TagReader {
	internal class Id3Header {
		private readonly BinaryReader binaryReader;

		public Id3Header( BinaryReader binaryReader) {
			this.binaryReader = binaryReader;
		}
	}
}
