namespace Mp3TagReader {
	internal interface ITag {
		public string Type { get; }

		bool ReadTag( string mp3File );
	}
}
