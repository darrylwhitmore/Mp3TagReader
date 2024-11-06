﻿namespace Mp3TagReader.Frames {
	internal class Id3UnimplementedFrame : Id3Frame {
		public Id3UnimplementedFrame( string frameId, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
		}

		protected override void ProcessFrameBody() {
		}
	}
}
