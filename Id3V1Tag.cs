using System.Text;

namespace Mp3TagReader {
	// What are ID3 Tags all about?
	// ID3v1 tag
	// https://phoxis.org/2010/05/08/what-are-id3-tags-all-about/#id3v1
	internal class Id3V1Tag : ITag {

		private readonly IResourceManager resourceManager;

		public Id3V1Tag( IResourceManager resourceManager ) {
			this.resourceManager = resourceManager;
		}

		public string Type => "Id3v1";

		public string Title { get; private set; }

		public string Artist { get; private set; }

		public string Album { get; private set; }

		public string Year { get; private set; }

		public string Comment { get; private set; }
		
		public int Track {  get; private set; }
		
		public int Genre {  get; private set; }

		public bool ReadTag( string mp3File ) {
			var tagFound = false;
			
			using var fs = File.Open( mp3File, FileMode.Open, FileAccess.Read, FileShare.Read );
			fs.Seek( -128, SeekOrigin.End );

			using var br = new BinaryReader( fs );

			var buffer = new byte[128];
			br.Read( buffer, 0, buffer.Length );

			var markerReader = new StringReader( buffer, 0, 2, Encoding.Latin1 );
			var marker = markerReader.ReadString();

			if ( marker == "TAG" ) {
				tagFound = true;

				var titleReader = new StringReader( buffer, 3, 32, Encoding.Latin1 );
				Title = titleReader.ReadString();

				var artistReader = new StringReader( buffer, 33, 62, Encoding.Latin1 );
				Artist = artistReader.ReadString();

				var albumReader = new StringReader( buffer, 63, 92, Encoding.Latin1 );
				Album = albumReader.ReadString();

				var yearReader = new StringReader( buffer, 93, 96, Encoding.Latin1 );
				Year = yearReader.ReadString();

				var commentReader = new StringReader( buffer, 97, 124, Encoding.Latin1 );
				Comment = commentReader.ReadString();

				Track = buffer[126];

				Genre = buffer[127];
			}

			return tagFound;
		}
	}
}
