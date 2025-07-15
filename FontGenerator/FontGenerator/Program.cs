using System.Text.Json;
using FontGenerator.Models;

namespace FontGenerator;

class Program
{
    static void Main(string[] args)
    {
        // Search for XBM images here
        var sourceDirectory = @"/mnt/storage0/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSansExtracted/";
        
        // Use this characters table
        var charactersTableFile = @"/mnt/storage0/shakti/projects/Electronics/STM32_L2HAL_FontsGenerator/Resources/KOI8-R.json";
        
        // Save result here
        var resultFile = @"/mnt/storage0/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSans.fmglfont";
        
        var charactersTable = JsonSerializer.Deserialize<CharactersTable>(File.ReadAllText(charactersTableFile));

        var missingFilesCount = 0;

        var charactersDictionary = new Dictionary<int, XbmFile>();

        foreach (var character in charactersTable.Characters)
        {
            var fullPath = Path.Combine(sourceDirectory, character.Name + ".xbm");

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"File {fullPath} not found!");
                missingFilesCount ++;
                continue;
            }
            
            charactersDictionary.Add(character.Code, XbmFile.Parse(fullPath));
        }

        using (var outputStream = new FileStream(resultFile, FileMode.Create))
        {
            #region Header
            
            // Size: 16 bytes
            
            // Signature
            outputStream.WriteByte(0x46);
            outputStream.WriteByte(0x4D);
            outputStream.WriteByte(0x47);
            outputStream.WriteByte(0x4C);
            outputStream.WriteByte(0x46);
            outputStream.WriteByte(0x4F);
            outputStream.WriteByte(0x4E);
            outputStream.WriteByte(0x54);
            
            // Version
            outputStream.Write(BitConverter.GetBytes(1));
            
            // Characters count
            outputStream.Write(BitConverter.GetBytes(charactersDictionary.Count));
            
            #endregion
            
            #region Characters table
            
            // Size: 8 bytes x characters count
            
            var offsetToNextCharacterData = 16 + 8 * charactersDictionary.Count;

            foreach (var character in charactersDictionary)
            {
                // Code
                outputStream.Write(BitConverter.GetBytes(character.Key));
                
                // Offset
                outputStream.Write(BitConverter.GetBytes(offsetToNextCharacterData));
                
                offsetToNextCharacterData += character.Value.Data.Count + 12;
            }
            
            #endregion
            
            #region Characters data
            
            foreach (var character in charactersDictionary)
            {
                // Width
                outputStream.Write(BitConverter.GetBytes(character.Value.Width));
                
                // Height
                outputStream.Write(BitConverter.GetBytes(character.Value.Height));
                
                // Raster size
                outputStream.Write(BitConverter.GetBytes(character.Value.Data.Count));;
                
                // Data
                outputStream.Write(character.Value.Data.ToArray());
            }
            
            #endregion
        }

        Console.WriteLine($"Missing files: {missingFilesCount}");
    }
}