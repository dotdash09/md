using System;
using System.Text.RegularExpressions;
using System.Web;

namespace MusicDown;

public class UrlParser
{
    /// <summary>
    /// ID 값을 가져옴
    /// </summary>
    public string GetId(string url)
    {
        string result = RemoveQueryStringParameter(url);
        return RemoveUrl(result);
    }

    /// <summary>
    /// URL 타입을 가져옴
    /// </summary>
    public UrlType GetUrlType(string url)
    {

        Match match1 = Regex.Match(url, @"channel/(\w+)");
        if (match1.Success)
            return UrlType.Channel;

        Match match2 = Regex.Match(url, @"list=([\w-]+)");
        if (match2.Success)
            return UrlType.List;

        return UrlType.None;
    }

    private string RemoveQueryStringParameter(string url)
    {
        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);
        query.Remove("feature");
        var builder = new UriBuilder(uri) { Query = query.ToString() };
        return builder.Uri.ToString();
    }

    private string RemoveUrl(string url)
    {
        // 채널 ID 추출
        if (url.IndexOf("channel/") > 0)
            return url.Substring(url.IndexOf("channel/") + 8);
        // // 플레이리스트 ID 추출
        if (url.IndexOf("list=") > 0)
            return url.Substring(url.IndexOf("list=") + 5);

        return null;
    }
}

public enum UrlType
{
    None = 0,
    Channel = 1,
    List = 2
}