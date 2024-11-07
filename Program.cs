using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Mp3TagReader {
	internal class Program {
		private const int AppReturnValueOk = 0;
		private const int AppReturnValueFail = 1;

		private static int Main( string[] args ) {

			var app = new CommandLineApplication {
				Name = "Mp3TagReader",
				Description = "Read all ID3 tags in an MP3 file."
			};

			app.HelpOption( "-?|-h|--help" );

			var fileSpecOption = app.Option( "-fs|--fileSpec <FileSpec>",
				"The MP3 file(s) specification. Wildcards may be used.",
				CommandOptionType.SingleValue );

			var outputFolderOption = app.Option( "-of|--outputFolder <Folder>",
				"Optional destination folder to write output JSON file(s). If omitted, the MP3 source folder will be used.",
				CommandOptionType.SingleOrNoValue );

			app.OnExecute( () => {
				if ( fileSpecOption.HasValue() ) {
					var fileSpecValue = fileSpecOption.Value() ?? string.Empty;

					var mp3Directory = Path.GetDirectoryName( fileSpecValue );

					if ( !Directory.Exists( mp3Directory ) ) {
						Console.WriteLine( $"MP3 directory does not exist: '{mp3Directory}'" );
						return AppReturnValueFail;
					}

					var searchPattern = Path.GetFileName( fileSpecValue );

					var mp3Files = Directory.GetFiles( mp3Directory, searchPattern );

					if ( !mp3Files.Any() ) {
						Console.WriteLine( "No MP3 files were found." );
						return AppReturnValueFail;
					}

					foreach ( var file in mp3Files ) {
						if ( Path.GetExtension( file ) != ".mp3" ) {
							Console.WriteLine( $"A non-MP3 file was found: '{file}'" );
							return AppReturnValueFail;
						}
					}

					var outputFolder = string.Empty;
					if ( outputFolderOption.HasValue() ) {
						outputFolder = outputFolderOption.Value();

						if ( string.IsNullOrWhiteSpace( outputFolder ) ) {
							outputFolder = mp3Directory;
						}

						if ( !Directory.Exists( outputFolder ) ) {
							Console.WriteLine( $"Output folder does not exist: '{outputFolder}'" );
							return AppReturnValueFail;
						}
					}

					return Process( mp3Files, outputFolder );
				}

				Console.WriteLine( "One or more required arguments were not provided." );
				app.ShowHint();
				return AppReturnValueFail;
			} );

			try {
				return app.Execute( args );
			}
			catch ( Exception ex ) {
				Console.WriteLine( $"Unexpected error: {ex.Message}" );
			}

			return AppReturnValueFail;
		}

		private static int Process( string[] mp3Files, string outputFolder ) {
			foreach ( var mp3File in mp3Files ) {
				try {
					var tag = new Id3Tag( mp3File );

					var json = JsonConvert.SerializeObject( tag, Formatting.Indented );

					if ( string.IsNullOrEmpty( outputFolder ) ) {
						Console.WriteLine( json );
					}
					else {
						var jsonFile = Path.Combine( outputFolder, Path.GetFileNameWithoutExtension( mp3File ) + ".json" );

						Console.WriteLine( $"Writing tag info to '{jsonFile}'." );

						File.WriteAllText( jsonFile, json );
					}
				}
				catch ( Exception e ) {
					Console.WriteLine( e.Message );
					return AppReturnValueFail;
				}
			}

			return AppReturnValueOk;
		}
	}
}

