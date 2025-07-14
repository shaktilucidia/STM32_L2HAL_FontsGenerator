using System.Text.Json.Serialization;

namespace FontGenerator.Models;

/// <summary>
/// One character from characters table
/// </summary>
public class Character
{
    /// <summary>
    /// Character code
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Character image filename
    /// </summary>
    [JsonPropertyName("filename")]
    public string Name { get; set; }
}