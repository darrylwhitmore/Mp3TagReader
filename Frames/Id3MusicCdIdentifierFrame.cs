using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// Music CD identifier
	// https://id3.org/id3v2.3.0#Music_CD_identifier
	internal sealed class Id3MusicCdIdentifierFrame : Id3Frame {
		public Id3MusicCdIdentifierFrame( string frameId, BinaryReader binaryReader ) : base( frameId, binaryReader ) {
			ProcessFrameBody();
		}

		[JsonProperty( Order = 1 )]
		public int CdTableOfContentsLength { get; private set; }

		protected override void ProcessFrameBody() {
			CdTableOfContentsLength = FrameBody.Length;
		}
	}
}
