using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	internal sealed class Id3UnimplementedFrame : Id3Frame {
		public Id3UnimplementedFrame( string id, BinaryReader binaryReader, IResourceManager resourceManager ) : base( id, binaryReader, resourceManager ) {
			Todo = string.Empty;
			
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public string Todo { get; private set; }

		protected override void ProcessFrameBody() {
			Todo = "*Frame not yet implemented*";
		}
	}
}
