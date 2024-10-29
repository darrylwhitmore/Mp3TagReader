namespace Mp3TagReader {
	internal class Id3UnimplementedFrame : Id3Frame {
		public Id3UnimplementedFrame( string frameId, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
			FrameIdName = "Unimplemented Frame";

			var body = new byte[FrameSize];

			binaryReader.Read( body, 0, body.Length );
		}
	}
}
