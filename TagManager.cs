using System;
using System.IO;
using MusicDown.Models;
using TagLib;

namespace MusicDown;

public class TagManager
{
    public void WriteTag(string songPath, MusicInfo musicInfo)
    {
        if (Path.GetExtension(songPath) == ".opus")
            WriteTagOpus(songPath, musicInfo);
        else if (Path.GetExtension(songPath) == ".m4a")
            WriteTagM4a(songPath, musicInfo);
        else
            throw new NotSupportedException();
    }

    private void WriteTagOpus(string songPath, MusicInfo musicInfo)
    {
        var opus = TagLib.File.Create(songPath);
        opus.Tag.Album = musicInfo.Album;
        opus.Tag.Title = musicInfo.Title;
        opus.Tag.Performers = musicInfo.Artists;
        opus.Tag.AlbumArtists = musicInfo.AlbumArtists;

        if (musicInfo.ReleaseYear.HasValue)
            opus.Tag.Year = (uint)musicInfo.ReleaseYear;

        var customTag = (TagLib.Ogg.XiphComment)opus.GetTag(TagLib.TagTypes.Xiph);
        if (musicInfo.Entries.HasValue)
            customTag.SetField("TRACKTOTAL", musicInfo.Entries.ToString());
        if (musicInfo.PlaylistIndex.HasValue)
            customTag.SetField("TRACKNUMBER", musicInfo.PlaylistIndex.ToString());

        if(musicInfo.SongId != null)
            customTag.SetField("TRACK_ID", musicInfo.SongId);
        if(musicInfo.AlbumId != null)
            customTag.SetField("ALBUM_ID", musicInfo.AlbumId);
        if(musicInfo.ChanneId != null)
            customTag.SetField("CHANNEL_ID", musicInfo.ChanneId);
        if(musicInfo.AlbumType != null)
            customTag.SetField("ALBUM_TYPE", musicInfo.AlbumType);

        // 기타 태그 삭제 (ENCODER 태그는 삭제 불가)
        customTag.SetField("ENCODER", "unknown encoder");

        opus.Save();
        System.Console.WriteLine("Tag writing complete.");
    }

    private void WriteTagM4a(string songPath, MusicInfo musicInfo)
    {
        var m4a = TagLib.File.Create(songPath);
        m4a.Tag.Album = musicInfo.Album;
        m4a.Tag.Title = musicInfo.Title;
        m4a.Tag.Performers = musicInfo.Artists;

        if (musicInfo.ReleaseYear.HasValue)
            m4a.Tag.Year = (uint)musicInfo.ReleaseYear;

        var customTag = (TagLib.Mpeg4.AppleTag)m4a.GetTag(TagLib.TagTypes.Apple);
        if (musicInfo.Entries.HasValue)
            customTag.TrackCount = (uint)musicInfo.Entries;
        if (musicInfo.PlaylistIndex.HasValue)
            customTag.Track = (uint)musicInfo.PlaylistIndex;

        customTag.SetDashBox("YT", "TRACK_ID", musicInfo.SongId);
        customTag.SetDashBox("YT", "ALBUM_ID", musicInfo.AlbumId);
        customTag.SetDashBox("YT", "CHANNEL_ID", musicInfo.ChanneId);

        m4a.Save();
    }
}
