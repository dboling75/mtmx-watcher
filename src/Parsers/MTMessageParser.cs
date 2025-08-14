using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MTMessageParser
{
    /// <summary>
    /// Parses a raw MT message string into a dictionary of tag -> value.
    /// </summary>
    public static Dictionary<string, string> Parse(string mtMessage)
    {
        var result = new Dictionary<string, string>();
        var tagPattern = new Regex(@"(?<=\r?\n|^):([0-9A-Z]{2,3}[A-Z]?):");

        var matches = tagPattern.Matches(mtMessage);
        for (int i = 0; i < matches.Count; i++)
        {
            string currentTag = matches[i].Groups[1].Value;
            int startIndex = matches[i].Index + matches[i].Length + 1;

            int endIndex = (i + 1 < matches.Count)
                ? matches[i + 1].Index
                : mtMessage.Length;

            string content = mtMessage.Substring(startIndex, endIndex - startIndex).Trim();
            result[currentTag] = content;
        }

        return result;
    }
}
