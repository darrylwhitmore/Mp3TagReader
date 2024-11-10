namespace Mp3TagReader.Frames;

internal interface IFrame {
	string Id { get; }

	ulong Size { get; }
}