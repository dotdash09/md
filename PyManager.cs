using System.Diagnostics;

namespace MusicDown;

public class PyManager
{
    private string _language;

    public string GetArtist(string channelId)
        => PyProcess("get_artist", channelId);

    public string GetArtistAlbums(string channelId)
        => PyProcess("get_artist_albums", channelId);

    public string GetArtistSingles(string channelId)
        => PyProcess("get_artist_singles", channelId);

    public string GetTracks(string browseId)
        => PyProcess("get_tracks", browseId);

    public string GetAlbum(string browseId)
        => PyProcess("get_album", browseId);

    public string GetAlbumBrowseId(string channelId)
        => PyProcess("get_album_browse_id", channelId);
            
    public PyManager(string language) => _language = language;

    private string PyProcess(string pyName, string args)
    {
        Process process = new Process();
        process.StartInfo.FileName = "python3";
        process.StartInfo.Arguments = @$"./py/{pyName}.py ""{args}"" ""{_language}"" ";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }
}
