# MP3 Tag Reader

Yes, yes, there is no shortage of apps that read the metadata [tags](https://id3.org/Home) inside MP3 files, but it seemed like a fun exercise.

I have long used, and highly recommend, [MP3TAG](https://www.mp3tag.de/en/) to view and edit the metadata tags in my MP3 files, but got to wondering how it was doing the magic that it does.

I loaded an MP3 into the nifty online hex file viewer [HexEd.it](https://hexed.it/), and presto! there were the tags. I searched around on Stack Overflow and got inspiration and a leg up from [How to read Id3v2 tag](https://stackoverflow.com/questions/16399604/how-to-read-id3v2-tag). I found a wealth of further details on the [ID3v2 Developer Information page](https://id3.org/Developer%20Information), and was off to the races.

But then...I noticed that my MP3 files also had old school [ID3v1 tags](https://id3.org/ID3v1), so I added support for those.

And then...while normalizing the volume level on my MP3 files using [MP3Gain](https://mp3gain.sourceforge.net/), I found that it added old school [APEv2 tags](https://wiki.hydrogenaud.io/index.php?title=APEv2_specification) to the files, so I added support for these as well. 

## Application Command Line Options
The command line options are:
```
Usage: Mp3TagReader [options]

Options:
  -?|-h|--help                   Show help information.
  -fs|--fileSpec <FileSpec>      The location of the MP3 file(s). Wildcards may be used. If a folder is provided, all
                                 MP3 files in the folder will be selected.
  -sf|--sortFrames               If provided, Id3v2 frames will be sorted by Id; otherwise they will appear in physical
                                 order. Helpful if you are diffing the JSON output.
  -of|--outputFolder[:<Folder>]  Optional destination folder to write output JSON file(s). If omitted, the MP3 source
                                 folder will be used.                                 
```
### Command Line Examples
This command does not specify an output folder, so the JSON output is written to the console (see below for an example of the output):
```
> Mp3TagReader -fs "c:\Music\U2\War\01 Sunday Bloody Sunday.mp3"
```
This command does not specify an output folder, so the JSON output is written to the console with the ID3v2 frames in sorted order:
```
> Mp3TagReader -fs "c:\Music\U2\War\01 Sunday Bloody Sunday.mp3" -sf
```
This command produces ***c:\Music\U2\War\01 Sunday Bloody Sunday.json***:
```
> Mp3TagReader -fs "c:\Music\U2\War\01 Sunday Bloody Sunday.mp3" -of
```
This command produces ***c:\tags\01 Sunday Bloody Sunday.json***:
```
> Mp3TagReader -fs "c:\Music\U2\War\01 Sunday Bloody Sunday.mp3" -of:c:\tags
```
This command outputs JSON for each MP3 in the folder to the console:
```
> Mp3TagReader -fs "c:\Music\U2\War\*.mp3"
```
## Example JSON Output
Since the output is so verbose, here is just one example. It corresponds to the first command line example above:

```
{
  "Mp3File": "c:\\Music\\U2\\War\\01 Sunday Bloody Sunday.mp3",
  "Tags": [
    {
      "Type": "Id3v2",
      "TagSize": 310894,
      "Header": {
        "HeaderSize": 10,
        "FramesSize": 310884,
        "Version": "ID3v2.3.0",
        "Flags": []
      },
      "Frames": [
        {
          "Id": "TALB (Album/Movie/Show title)",
          "Size": 21,
          "Flags": [],
          "Text": "War"
        },
        {
          "Id": "TPE1 (Lead performer(s)/Soloist(s))",
          "Size": 19,
          "Flags": [],
          "Text": "U2"
        },
        {
          "Id": "TCOM (Composer)",
          "Size": 77,
          "Flags": [],
          "Text": "Adam Clayton ???·????? ????????"
        },
        {
          "Id": "TCON (Content type)",
          "Size": 53,
          "Flags": [],
          "Text": "Indie / Alternative"
        },
        {
          "Id": "TLEN (Length)",
          "Size": 18,
          "Flags": [],
          "Text": "279426"
        },
        {
          "Id": "TIT2 (Title/songname/content description)",
          "Size": 55,
          "Flags": [],
          "Text": "Sunday Bloody Sunday"
        },
        {
          "Id": "TRCK (Track number/Position in set)",
          "Size": 13,
          "Flags": [],
          "Text": "1"
        },
        {
          "Id": "TYER (Year)",
          "Size": 16,
          "Flags": [],
          "Text": "1983"
        },
        {
          "Id": "APIC (Attached picture)",
          "Size": 308379,
          "Flags": [],
          "MimeType": "image/jpeg",
          "PictureType": "0x03 (Cover (front))",
          "Description": "",
          "PictureDataLength": 308355
        },
        {
          "Id": "MCDI (Music CD identifier)",
          "Size": 134,
          "Flags": [],
          "CdTableOfContentsLength": 124
        },
        {
          "Id": "PRIV (Private)",
          "Size": 24,
          "Flags": [],
          "OwnerIdentifier": "PeakValue",
          "PrivateDataLength": 4
        },
        {
          "Id": "PRIV (Private)",
          "Size": 27,
          "Flags": [],
          "OwnerIdentifier": "AverageLevel",
          "PrivateDataLength": 4
        },
        {
          "Id": "0000 (Padding placeholder)",
          "Size": 2048
        }
      ]
    },
    {
      "Type": "Id3v1",
      "Title": "Sunday Bloody Sunday",
      "Artist": "U2",
      "Album": "War",
      "Year": "1983",
      "Comment": "",
      "Track": 1,
      "Genre": 255
    }
  ]
}
```




