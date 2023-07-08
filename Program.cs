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
        // var ytUrl = "https://music.youtube.com/playlist?list=OLAK5uy_krxzlaXQ_JHgHeId9ZWvqCQ2kTpkyT2tk";
        // var ytUrl = args[0];

        Console.Write("앨범 아티스트: ");
        string AlbumArtist = Console.ReadLine();

        while (true)
        {
            Console.Write("URL: ");
            var ytUrl = Console.ReadLine();
            if (ytUrl == "q" || ytUrl == "")
                return;

            var tmpDir = "/home/dotdash09/tmp";

            // // 음악 + 음악 정보 json 파일 다운로드
            string cmdArgs = $"""--cookies cookies.txt --skip-download --write-info-json -o "{tmpDir}/%(album)s/%(playlist_index)s. %(title)s.%(ext)s" {ytUrl}""";
            var shellCommand = new ShellCommand();
            shellCommand.Start("yt-dlp", cmdArgs);


            var downDir = Directory.GetDirectories(tmpDir).Where(n => Path.GetFileName(n) != "NA").FirstOrDefault();
            var jsonFilePaths = Directory.GetFiles(downDir).Where(n => n.IndexOf(".json") > -1);

            List<MusicInfo> musicInfoList = new List<MusicInfo>();
            foreach (var jsonFilePath in jsonFilePaths)
            {
                string json = System.IO.File.ReadAllText(jsonFilePath);
                var musicInfo = JsonSerializer.Deserialize<MusicInfo>(json, JsonSerializerOptions.Default);
                musicInfo.AlbumArtists = AlbumArtist.Split(';', StringSplitOptions.TrimEntries);
                musicInfo.JsonFilePath = jsonFilePath;
                musicInfoList.Add(musicInfo);
            }


            var coverManager = new CoverManager();
            Stream coverStream = await coverManager.Download(musicInfoList.First());

            foreach (var musicInfo in musicInfoList)
            {
                var opusPath = musicInfo.JsonFilePath.Replace(".info.json", ".opus");

                if (musicInfo.HasFormatOpus == true)
                {
                    // 작업할 파일명
                    var webmPath = musicInfo.JsonFilePath.Replace(".info.json", ".webm");

                    System.Console.WriteLine("****************************opus****************************");
                    string downCmdArgs = $"""--cookies cookies.txt -f 251 --load-info-json="{musicInfo.JsonFilePath}"  -o "{webmPath}" """;
                    shellCommand.Start("yt-dlp", downCmdArgs);

                    // webm to opus
                    shellCommand.Start("mkvextract", $"tracks \"{webmPath}\" 0:\"{opusPath}\"", false);

                    // webm 파일 삭제
                    File.Delete(webmPath);
                }
                else if (musicInfo.HasM4AFormat == true)
                {
                    System.Console.WriteLine("****************************m4a*****************************");

                    // 작업할 파일명
                    var m4aPath = musicInfo.JsonFilePath.Replace(".info.json", ".m4a");

                    string downCmdArgs = $"""--cookies cookies.txt -f 141 --load-info-json="{musicInfo.JsonFilePath}"  -o "{m4aPath}" """;
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
            }

            Directory.Delete($"{tmpDir}/NA", true);
        }
    }
}