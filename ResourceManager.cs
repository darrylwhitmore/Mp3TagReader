namespace Mp3TagReader {
	internal class ResourceManager : IResourceManager {
		public string? GetString( string baseKey, string subKey ) {
			var key = $"{baseKey}:{subKey}";

			return GetString( key );
		}

		public string? GetString( string key ) {
			return Properties.Resources.ResourceManager.GetString( key );
		}
	}
}
