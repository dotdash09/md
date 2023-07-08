using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MusicDown.Models;

namespace MusicDown;

public class CoverManager
{
    public async Task<Stream> Download(MusicInfo musicInfo)
    {
        using HttpClient httpClient = new HttpClient();
        var converMemoryStream = new MemoryStream();

        var coverStream = await httpClient.GetStreamAsync(musicInfo.Thumbnail.Url);
        await coverStream.CopyToAsync(converMemoryStream);

        return converMemoryStream;
    }

    public void WriteCover(string songPath, Stream coverStream)
    {
        coverStream.Position = 0;

        var opus = TagLib.File.Create(songPath);
        var picture = new TagLib.Picture();
        picture.Type = TagLib.PictureType.FrontCover;
        picture.Data = TagLib.ByteVector.FromStream(coverStream);

        opus.Tag.Pictures = new TagLib.Picture[] { picture };
        opus.Save();

        System.Console.WriteLine("cover writing complete.");
    }
}
