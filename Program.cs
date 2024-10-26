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

			app.OnExecute( () => {
				if ( fileSpecOption.HasValue() ) {
					var fileSpecValue = fileSpecOption.Value() ?? string.Empty;

					var directory = Path.GetDirectoryName( fileSpecValue );

					if ( !Directory.Exists( directory ) ) {
						Console.WriteLine( $"Directory does not exist: '{directory}'" );
						return AppReturnValueFail;
					}

					var searchPattern = Path.GetFileName( fileSpecValue );

					var files = Directory.GetFiles( directory, searchPattern );

					if ( !files.Any() ) {
						Console.WriteLine( "No files were found." );
						return AppReturnValueFail;
					}

					foreach ( var file in files ) {
						if ( Path.GetExtension( file ) != ".mp3" ) {
							Console.WriteLine( $"A non-MP3 file was found: '{file}'" );
							return AppReturnValueFail;
						}
					}

					return Process( files );
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

		private static int Process( string[] files ) {
			var tags = ( from file in files select new Id3Tag( file ) ).ToList();

			var json = JsonConvert.SerializeObject( tags, Formatting.Indented );

			Console.WriteLine( json );

			return AppReturnValueOk;
		}
	}
}

