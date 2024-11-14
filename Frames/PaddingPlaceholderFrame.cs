namespace Mp3TagReader.Frames {
	internal class PaddingPlaceholderFrame : IFrame {

		public PaddingPlaceholderFrame( int size ) {
			Size = size;
		}

		public string Id => "0000 (Padding placeholder)";
		public int Size { get; }
	}
}
