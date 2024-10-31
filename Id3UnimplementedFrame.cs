namespace Mp3TagReader {
	internal class Id3UnimplementedFrame : Id3Frame {
		public Id3UnimplementedFrame( string frameId, BinaryReader binaryReader ) : base( frameId, "*Frame not yet implemented*", binaryReader ) {
		}

		protected override void ProcessFrameBody() {
		}
	}
}
