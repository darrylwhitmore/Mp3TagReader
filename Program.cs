using McMaster.Extensions.CommandLineUtils;

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

			var fileOption = app.Option( "-f|--file <Mp3File>",
				"The MP3 file to read.",
				CommandOptionType.SingleValue );

			app.OnExecute( () => {
				if ( fileOption.HasValue() ) {
					var fileValue = fileOption.Value() ?? string.Empty;

					if ( !File.Exists( fileValue ) ) {
						Console.WriteLine( $"Input file(s) not found: '{fileValue}'" );
						return AppReturnValueFail;
					}

					return Process( fileValue );
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

		private static int Process( string fileValue ) {
			return AppReturnValueOk;
		}
	}
}

