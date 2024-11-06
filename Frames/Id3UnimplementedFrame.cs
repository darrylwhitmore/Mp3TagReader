using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	internal sealed class Id3UnimplementedFrame : Id3Frame {
		public Id3UnimplementedFrame( string frameId, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Todo { get; private set; }

		protected override void ProcessFrameBody() {
			Todo = "*Frame not yet implemented*";
		}
	}
}
