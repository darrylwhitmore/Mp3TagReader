using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Mp3TagReader {
	internal class Program {
		private const int AppReturnValueOk = 0;
		private const int AppReturnValueFail = 1;

		private static int Main( string[] args ) {

			var app = new CommandLineApplication {
				Name = "Mp3TagReader",
				Description = "Read all metadata tags in an MP3 file."
			};

			app.HelpOption( "-?|-h|--help" );

			var fileSpecOption = app.Option( "-fs|--fileSpec <FileSpec>",
				"The location of the MP3 file(s). Wildcards may be used. If a folder is provided, all MP3 files in the folder will be selected.",
				CommandOptionType.SingleValue );

			var sortId3V2FramesOption = app.Option( "-sf|--sortFrames",
				"If provided, Id3v2 frames will be sorted by Id; otherwise they will appear in physical order. Helpful if you are diffing the JSON output.",
				CommandOptionType.NoValue );

			var outputFolderOption = app.Option( "-of|--outputFolder <Folder>",
				"Optional destination folder to write output JSON file(s). If omitted, the MP3 source folder will be used.",
				CommandOptionType.SingleOrNoValue );

			app.OnExecute( () => {
				if ( fileSpecOption.HasValue() ) {
					var fileSpecValue = fileSpecOption.Value() ?? string.Empty;

					// The command line parser seems to have an issue when a directory is specified with an ending backslash
					// and is enclosed in double quotes. It seems to think the ending backslash is escaping the ending
					// double quote. Look for this and trim off the trailing double quote if present.
					//
					// -fs "\\foo\bar\" -> \\foo\bar"
					//
					if ( fileSpecValue.EndsWith( '\"' ) ) {
						fileSpecValue = fileSpecValue.TrimEnd( '\"' );
					}

					fileSpecValue = Path.GetFullPath( fileSpecValue );

					string? mp3Directory;
					string[] mp3Files;

					if ( Directory.Exists( fileSpecValue ) ) {
						mp3Directory = fileSpecValue;
						mp3Files = Directory.GetFiles( fileSpecValue, "*.mp3" );
					}
					else {
						mp3Directory = Path.GetDirectoryName( fileSpecValue );

						if ( !Directory.Exists( mp3Directory ) ) {
							Console.WriteLine( $"MP3 directory does not exist: '{mp3Directory}'" );
							return AppReturnValueFail;
						}

						var searchPattern = Path.GetFileName( fileSpecValue );

						mp3Files = Directory.GetFiles( mp3Directory, searchPattern );
					}

					if ( !mp3Files.Any() ) {
						Console.WriteLine( $"No MP3 files were found in '{mp3Directory}'." );
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

					return Process( mp3Files, outputFolder, sortId3V2FramesOption.HasValue() );
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

		private static int Process( string[] mp3Files, string outputFolder, bool sortId3V2Frames ) {
			var resourceManager = new ResourceManager();
			
			foreach ( var mp3File in mp3Files ) {
				try {
					var tagInfo = new Mp3TagInfo( mp3File, resourceManager, sortId3V2Frames );

					tagInfo.LoadTags();

					var json = JsonConvert.SerializeObject( tagInfo, Formatting.Indented );

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

