namespace Mp3TagReader;

public interface IResourceManager {
	string? GetString( string baseKey, string subKey );
	string? GetString( string key );
}