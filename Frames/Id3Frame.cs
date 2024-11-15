﻿using System.Text;
using Newtonsoft.Json;

namespace Mp3TagReader.Frames {
	// ID3v2 frame overview
	// https://id3.org/id3v2.3.0#ID3v2_frame_overview
	internal abstract class Id3Frame : IFrame {
		protected Id3Frame( string id, BinaryReader binaryReader ) {
			Id = id;

			var frameName = GetResourceString( "Name" );
			FrameIdDisplay = string.IsNullOrEmpty( frameName ) ? $"{Id}" : $"{Id} ({frameName})";

			var frameSizeRaw = new byte[4];
			var frameFlags = new byte[2];

			binaryReader.Read( frameSizeRaw, 0, frameSizeRaw.Length );
			binaryReader.Read( frameFlags, 0, frameFlags.Length );

			FrameBodySize = BitConverter.IsLittleEndian ? BitConverter.ToInt32( frameSizeRaw.Reverse().ToArray(), 0 ) : BitConverter.ToInt32( frameSizeRaw );

			Size = FrameBodySize + 10;

			FrameBody = new byte[FrameBodySize];
			binaryReader.Read( FrameBody, 0, FrameBody.Length );

			// Frame header flags
			// https://id3.org/id3v2.3.0#Frame_header_flags
			var tagAlterPreservationFlag = ( frameFlags[0]  & 0b1000000 ) != 0;
			var fileAlterPreservationFlag = ( frameFlags[0] & 0b0100000 ) != 0;
			var readOnlyFlag = ( frameFlags[0]              & 0b0010000 ) != 0;
			var compressionFlag = ( frameFlags[1]           & 0b1000000 ) != 0;
			var encryptionFlag = ( frameFlags[1]            & 0b0100000 ) != 0;
			var groupingIdentityFlag = ( frameFlags[1]      & 0b0010000 ) != 0;

			Flags = $"{( tagAlterPreservationFlag ? "T" : "t" )}{( fileAlterPreservationFlag ? "F" : "f" )}{( readOnlyFlag ? "R" : "r" )}..... {( compressionFlag ? "C" : "c" )}{( encryptionFlag ? "E" : "e" )}{( groupingIdentityFlag ? "G" : "g" )}.....";
		}

		[JsonIgnore]
		public string Id { get; }

		[JsonProperty( PropertyName = "Id" )]
		protected string FrameIdDisplay { get; set; }

		protected byte[] FrameBody { get; set; }

		/// <summary>
		/// Frame body size (excluding header size)
		/// </summary>
		protected int FrameBodySize { get; set; }

		/// <summary>
		/// Total frame size, including header
		/// </summary>
		public int Size { get; }

		public string Flags { get; private set; }

		// ID3v2 frame overview
		// https://id3.org/id3v2.4.0-structure
		protected Encoding GetEncoding( byte encodingByte ) {
			switch ( encodingByte ) {
				case 0x0:
					// ISO-8859-1
					return Encoding.Latin1;

				case 0x1:
					// UTF-16 with BOM
					return Encoding.Unicode;

				case 0x2:
					// UTF-16 without BOM
					return Encoding.BigEndianUnicode;

				case 0x3:
					// UTF-8
					return Encoding.UTF8;

				default:
					throw new ArgumentException( $"Unknown encoding byte: {encodingByte:X}" );
			}
		}

		protected string? GetResourceString( string partialKey ) {
			var key = $"{Id}:{partialKey}";

			return Properties.Resources.ResourceManager.GetString( key );
		}

		protected abstract void ProcessFrameBody();

		public static Id3Frame? GetNextFrame( BinaryReader binaryReader ) {
			var frameIdRaw = new byte[4];

			binaryReader.Read( frameIdRaw, 0, frameIdRaw.Length );

			var frameId = Encoding.ASCII.GetString( frameIdRaw );

			if (frameId == "\0\0\0\0" ) {
				// We're in the padding, there are no more frames
				return null;
			}

			// Declared ID3v2 frames
			// https://id3.org/id3v2.3.0#Declared_ID3v2_frames

			if ( frameId == "TXXX" ) {
				return new Id3UserDefinedTextInformationFrame( frameId, binaryReader );
			}

			if ( frameId.StartsWith( "T" ) ) {
				return new Id3TextInformationFrame( frameId, binaryReader );
			}

			switch ( frameId ) {
				case "APIC":
					return new Id3AttachedPictureFrame( frameId, binaryReader );

				case "COMM":
					return new Id3CommentFrame( frameId, binaryReader );

				case "MCDI":
					return new Id3MusicCdIdentifierFrame( frameId, binaryReader );

				case "PRIV":
					return new Id3PrivateFrame( frameId, binaryReader );

				case "WXXX":
					return new Id3UserDefinedUrlLinkFrame( frameId, binaryReader );

				default:
					return new Id3UnimplementedFrame( frameId, binaryReader );
			}
		}
	}
}
