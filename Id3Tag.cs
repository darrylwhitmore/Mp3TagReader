using Mp3TagReader.Frames;

namespace Mp3TagReader {
	// ID3 tag version 2.3.0
	// https://id3.org/id3v2.3.0#ID3_tag_version_2.3.0
	internal class Id3Tag {
		public Id3Tag( string mp3File ) {
			Mp3File = mp3File;

			var fs = File.Open( mp3File, FileMode.Open );

			using var br = new BinaryReader( fs );

			Header = new Id3Header( br );

			ReadFrames( br );

			br.Close();
		}
		public string Mp3File { get; }

		public ulong TagSize => Header.HeaderSize + Header.FramesSize;

		public int PaddingSize { get; private set; }

		public Id3Header Header { get; }

		public List<Id3Frame> Frames { get; } = [];

		private void ReadFrames( BinaryReader binaryReader ) {
			var leftToRead = Header.FramesSize;

			while ( leftToRead > 0 && Id3Frame.GetNextFrame( binaryReader ) is { } frame ) {
				Frames.Add( frame );

				leftToRead -= frame.Size;
			}

			PaddingSize = (int)leftToRead;
		}
	}
}
