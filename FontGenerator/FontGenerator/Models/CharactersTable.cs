using System.Text.Json.Serialization;

namespace FontGenerator.Models;

/// <summary>
/// Characters table
/// </summary>
public class CharactersTable
{
    /// <summary>
    /// Characters in table
    /// </summary>
    [JsonPropertyName("characters")]
    public List<Character> Characters { get; set; }
}