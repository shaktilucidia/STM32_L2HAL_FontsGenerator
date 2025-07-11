using System.Text.RegularExpressions;

namespace FontGenerator.Models;

/// <summary>
/// XBM file content
/// </summary>
public class XbmFile
{
    /// <summary>
    /// Dataset name
    /// </summary>
    private const string DatasetNameRegexpText = @"(?'dataset'\S*)_bits\[\] = {";
    
    
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
        
        var matches = DatasetNameRegexp.Matches(content);
        if (!matches.Any())
        {
            throw new ArgumentException("Not a valid XBM file!", nameof(path));
        }

        var datasetName = matches
            .First()
            .Groups["dataset"]
            .Captures
            .First()
            .Value;

        return new XbmFile(datasetName, 10, 10, new List<byte>());
    }
}