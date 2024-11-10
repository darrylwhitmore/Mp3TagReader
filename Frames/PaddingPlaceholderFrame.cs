namespace Mp3TagReader.Frames {
	internal class PaddingPlaceholderFrame : IFrame {

		public PaddingPlaceholderFrame( ulong size ) {
			Size = size;
		}

		public string Id => "0000 (Padding placeholder)";
		public ulong Size { get; }
	}
}
