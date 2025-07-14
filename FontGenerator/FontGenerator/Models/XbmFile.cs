using System.Text.RegularExpressions;

namespace FontGenerator.Models;

/// <summary>
/// XBM file content
/// </summary>
public class XbmFile
{
    /// <summary>
    /// Dataset name regexp
    /// </summary>
    private const string DatasetNameRegexpText = @"(?'dataset'\S*)_bits\[\] = {";

    /// <summary>
    /// Image width regexp template
    /// </summary>
    private const string WidthRegexpTemplate = "#define {0}_width (?'width'\\d+)";
    
    /// <summary>
    /// Image height regexp template
    /// </summary>
    private const string HeightRegexpTemplate = "#define {0}_height (?'height'\\d+)";
    
    /// <summary>
    /// Image data regexp template
    /// </summary>
    private const string DataRegexpTemplate = @"static char {0}_bits\[\] = {(?'data'.+)};";
    
    private static readonly Regex DatasetNameRegexp = new Regex(DatasetNameRegexpText, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// Dataset name, when array name is arrowdblboth_bits, dataset name will be arrowdblboth
    /// </summary>
    public string DatasetName { get; private set; }

    /// <summary>
    /// Image width
    /// </summary>
    public int Width { get; private set; }
    
    /// <summary>
    /// Image height
    /// </summary>
    public int Height { get; private set; }
    
    /// <summary>
    /// Image data
    /// </summary>
    public IReadOnlyCollection<byte> Data { get; private set; }

    public XbmFile
    (
        string datasetName,
        int width,
        int height,
        IReadOnlyCollection<byte> data
    )
    {
        if (string.IsNullOrWhiteSpace(datasetName))
        {
            throw new ArgumentException($"'{nameof(datasetName)}' cannot be null or whitespace", nameof(datasetName));
        }

        if (width <= 0)
        {
            throw new ArgumentException($"'{nameof(width)}' cannot be negative", nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentException($"'{nameof(height)}' cannot be negative", nameof(height));
        }
        
        DatasetName = datasetName;
        Width = width;
        Height = height;
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    /// Parse given file
    /// </summary>
    /// <param name="path">XBM file to parse</param>
    public static XbmFile Parse(string path)
    {
        var content = File.ReadAllText(path);
        
        #region Dataset name
        
        var datasetName = ExtractMatchGroup
        (
            content,
            DatasetNameRegexpText,
            "dataset",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
        
        #endregion
        
        #region Width

        var width = int.Parse
        (
            ExtractMatchGroup
            (
                content,
                String.Format(WidthRegexpTemplate, datasetName),
                "width",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            )
        );
        
        #endregion
        
        #region Height
        
        var height = int.Parse
        (
            ExtractMatchGroup
            (
                content,
                String.Format(HeightRegexpTemplate, datasetName),
                "height",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            )
        );
        
        #endregion
        
        #region Data
        
        var dataRaw = ExtractMatchGroup
        (
            content,
            DataRegexpTemplate.Replace("{0}", datasetName),
            "data",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
        );
        
        var dataStrings = dataRaw
            .Split(", ")
            .Select(s => s.Trim())
            .ToList();

        if (dataStrings.Last() == string.Empty)
        {
            dataStrings.RemoveAt(dataStrings.Count - 1);
        }

        var data = dataStrings
            .Select(s => Convert.ToByte(s, 16))
            .ToList();
        
        #endregion

        return new XbmFile(datasetName, width, height, data);
    }

    private static string ExtractMatchGroup(string content, string regexpTemplate, string groupName, RegexOptions regexpOptions)
    {
        var regexp = new Regex(regexpTemplate, regexpOptions);
        
        var matches = regexp.Matches(content);
        if (!matches.Any())
        {
            throw new ArgumentException($"Cant extract group { groupName}!", nameof(content));
        }
        
        return matches
            .First()
            .Groups[groupName]
            .Captures
            .First()
            .Value;
    }
}