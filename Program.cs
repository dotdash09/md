using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MusicDown.Models;

namespace MusicDown;

public class Program
{
    static async Task Main(string[] args)
    {
        var tmpDir = "/home/dotdash09/tmp";
        // var ytUrl = "https://music.youtube.com/channel/UCvolP1xNN2maB52Tb1PkXzg";
        // var ytUrl = args[0];


        // string AlbumArtist = "";

        while (true)
        {
            Console.Write("앨범 아티스트: ");
            string AlbumArtist = Console.ReadLine();

            if(string.IsNullOrEmpty(AlbumArtist) == true)
            {
                AlbumArtist = "Various Artists";
                Console.WriteLine("앨범 아티스트: Various Artists");
            }

            Console.Write("URL: ");
            var ytUrl = Console.ReadLine();
            if (ytUrl == "q" || ytUrl == "")
                return;

            UrlParser urlParser = new UrlParser();
            var urlType = urlParser.GetUrlType(ytUrl);
            var id = urlParser.GetId(ytUrl);

            var pyManager = new PyManager();

            List<YtMusicApiAlbumResults> allResult = new List<YtMusicApiAlbumResults>();

            // 채널 URL 입력
            if (urlType == UrlType.Channel)
            {
                var artistJson = pyManager.GetArtist(id);
                var ytMusicApiChannel = JsonSerializer.Deserialize<YtMusicApiChannel>(artistJson, JsonSerializerOptions.Default);

                List<YtMusicApiAlbumResults> albumResuls = new List<YtMusicApiAlbumResults>();
                List<YtMusicApiAlbumResults> singleResuls = new List<YtMusicApiAlbumResults>();

                // 앨범
                if (ytMusicApiChannel.Albums.HasMoreAlbums == false)
                    albumResuls = ytMusicApiChannel.Albums.Results;
                else
                {
                    var albumJson = pyManager.GetArtistAlbums(id);
                    albumResuls = JsonSerializer.Deserialize<List<YtMusicApiAlbumResults>>(albumJson, JsonSerializerOptions.Default);
                }

                // 싱글
                if (ytMusicApiChannel.Singles.HasMoreAlbums == false)
                    singleResuls = ytMusicApiChannel.Singles.Results;
                else
                {
                    var singleResulsJson = pyManager.GetArtistSingles(id);
                    singleResuls = JsonSerializer.Deserialize<List<YtMusicApiAlbumResults>>(singleResulsJson, JsonSerializerOptions.Default);
                }

                allResult = albumResuls.Concat(singleResuls).ToList();
            }

            else if (urlType == UrlType.List)
            {
                string browseId = pyManager.GetAlbumBrowseId(id).Replace(Environment.NewLine, "");
                var albumJson = pyManager.GetAlbum(browseId);
                var result = JsonSerializer.Deserialize<YtMusicApiAlbumResults>(albumJson, JsonSerializerOptions.Default);
                result.BrowseId = browseId;
                allResult.Add(result);
            }
            else
            {
                continue;
            }

            // 트랙정보 기록
            foreach (var album in allResult)
            {
                string albumtrackJson = pyManager.GetTracks(album.BrowseId);
                album.AlbumTrack = JsonSerializer.Deserialize<YTMusicApiAlbumTrack>(albumtrackJson, JsonSerializerOptions.Default);                
            }

            // https://music.youtube.com/watch?v=QV8D6P-NR4c&feature=share
            // https://music.youtube.com/browse/MPREb_2XFuPYX5tLU
            // https://music.youtube.com/playlist?list=OLAK5uy_lRU0GHe1Pf9BIC-Js8rthXaFEWF3QasrU&feature=share

            int albumCount = 1;
            foreach (var album in allResult)
            {
                // 앨범 커버
                var coverManager = new CoverManager();
                Stream coverStream = await coverManager.Download(album.Thumbnail.Url);

                var albumTrack = album.AlbumTrack;
                int trackCount = 1;

                foreach (var track in albumTrack.YTMusicApiTrack)
                {
                    System.Console.WriteLine($"*****  Album: {albumCount}/{allResult.Count()} , Track: {trackCount}/{albumTrack.YTMusicApiTrack.Count()}  *****");

                    if (track.IsAvailable == false)
                    {
                        trackCount += 1;
                        continue;
                    }

                    string albumTitleFix = string.Join("_", album.Title.Split(GetInvalidFileNameChars()));
                    string trackTitleFix = string.Join("_", track.Title.Split(GetInvalidFileNameChars()));

                    string jobPath = $"{tmpDir}/{albumTitleFix}_{album.BrowseId}/{trackTitleFix}_{track.VideoId}";

                    string jsonFilePath = $"{jobPath}.info.json";
                    string opusPath = $"{jobPath}.opus";
                    string webmPath = $"{jobPath}.webm";
                    string m4aPath = $"{jobPath}.m4a";

                    // string cmdArgs = $"""--cookies cookies.txt --skip-download --write-info-json -o "{tmpDir}/%(album)s_%(playlist_id)s/%(playlist_index)s. %(title)s.%(ext)s" https://music.youtube.com/watch?v={track.VideoId}""";
                    string cmdArgs = $"""--cookies cookies.txt --skip-download --write-info-json -o "{jobPath}" https://music.youtube.com/watch?v={track.VideoId}""";
                    // 음악 + 음악 정보 json 파일 다운로드
                    var shellCommand = new ShellCommand();
                    shellCommand.Start("yt-dlp", cmdArgs);

                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    var musicInfo = JsonSerializer.Deserialize<MusicInfo>(json, JsonSerializerOptions.Default);
                    musicInfo.AlbumArtists = AlbumArtist.Split(';', StringSplitOptions.TrimEntries);
                    musicInfo.JsonFilePath = jsonFilePath;
                    musicInfo.AlbumId = album.BrowseId;
                    musicInfo.AlbumType = albumTrack.Type;
                    musicInfo.ReleaseYear = Convert.ToUInt32(album.AlbumTrack.Year);
                    musicInfo.Entries = Convert.ToUInt32(albumTrack.YTMusicApiTrack.Count());
                    musicInfo.PlaylistIndex = Convert.ToUInt32(trackCount);
                    musicInfo.Album = album.Title;
                    musicInfo.Title = track.Title;

                    if (musicInfo.HasFormatOpus == true)
                    {
                        System.Console.WriteLine("******  opus  *****");
                        string downCmdArgs = $"""--cookies cookies.txt -f 251 --load-info-json="{jsonFilePath}"  -o "{webmPath}" """;
                        shellCommand.Start("yt-dlp", downCmdArgs);

                        // webm to opus
                        shellCommand.Start("mkvextract", $"tracks \"{webmPath}\" 0:\"{opusPath}\"", false);

                        // webm 파일 삭제
                        File.Delete(webmPath);
                    }
                    else if (musicInfo.HasM4AFormat == true)
                    {
                        System.Console.WriteLine("*****  m4a  *****");

                        string downCmdArgs = $"""--cookies cookies.txt -f 141 --load-info-json="{jsonFilePath}"  -o "{m4aPath}" """;
                        shellCommand.Start("yt-dlp", downCmdArgs);

                        //m4a to opus
                        shellCommand.Start("ffmpeg", $"-i \"{m4aPath}\" -c:a libopus -b:a 128k \"{opusPath}\"", false);

                        // m4a 파일 삭제
                        File.Delete(m4aPath);
                    }

                    try
                    {
                        // 커버 쓰기
                        coverManager.WriteCover(opusPath, coverStream);

                        //태그 쓰기
                        var tagManager = new TagManager();
                        tagManager.WriteTag(opusPath, musicInfo);

                        // 정리
                        File.Delete(musicInfo.JsonFilePath);
                    }
                    catch
                    {
                        System.Console.WriteLine("Tag write failed.");
                    }
                    trackCount += 1;
                }
                albumCount += 1;
            }
        }
    }

    public static char[] GetInvalidFileNameChars() => new char[]
    {
        '\"', '<', '>', '|', '\0',
        (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
        (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
        (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
        (char)31, ':', '*', '?', '\\', '/'
    };
}