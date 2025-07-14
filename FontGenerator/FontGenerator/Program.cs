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
        
        // FMGL Font structure
        // [9 bytes]: 'FMGLFONT1'
        // [4 bytes]: characters count
        // []: characters, each character is:
        // -- [4 bytes]: code
        // -- [2 bytes]: width
        // -- [2 bytes]: height
        // -- [width * height bytes]: character data

        using (var outputStream = new FileStream(resultFile, FileMode.Create))
        {
            // Signature
            outputStream.WriteByte(0x46);
            outputStream.WriteByte(0x4D);
            outputStream.WriteByte(0x47);
            outputStream.WriteByte(0x4C);
            outputStream.WriteByte(0x46);
            outputStream.WriteByte(0x4F);
            outputStream.WriteByte(0x4E);
            outputStream.WriteByte(0x54);
            outputStream.WriteByte(0x31);
            
            // Characters count
            outputStream.Write(BitConverter.GetBytes(charactersDictionary.Count));
            
            foreach (var character in charactersDictionary)
            {
                // Code
                outputStream.Write(BitConverter.GetBytes(character.Key));
                
                // Width
                outputStream.Write(BitConverter.GetBytes(character.Value.Width));
                
                // Height
                outputStream.Write(BitConverter.GetBytes(character.Value.Height));
                
                // Data
                outputStream.Write(character.Value.Data.ToArray());
            }
        }

        Console.WriteLine($"Missing files: {missingFilesCount}");
    }
}