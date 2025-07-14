using System.Text.Json;
using FontGenerator.Models;

namespace FontGenerator;

class Program
{
    static void Main(string[] args)
    {
        //var xbmFile = XbmFile.Parse(@"/mnt/storage0/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSansExtracted/yericyrillic.xbm");

        var sourceDirectory = @"/mnt/storage0/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSansExtracted/";
        
        var charactersTableFile = @"/mnt/storage0/shakti/projects/Electronics/STM32_L2HAL_FontsGenerator/Resources/KOI8-R.json";
        
        /*var xbmFiles = Directory.EnumerateFiles(sourceDirectory, "*.xbm", SearchOption.AllDirectories);

        var parsedXbmFiles = xbmFiles
            .Select(f => XbmFile.Parse(f))
            .ToList();*/
        
        var charactersTable = JsonSerializer.Deserialize<CharactersTable>(File.ReadAllText(charactersTableFile));

        var missingFilesCount = 0;

        foreach (var character in charactersTable.Characters)
        {
            var fullPath = Path.Combine(sourceDirectory, character.Name + ".xbm");

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"File {fullPath} not found!");
                missingFilesCount ++;
                continue;
            }
            
            var xbmFile = XbmFile.Parse(fullPath);
        }
        
        Console.WriteLine($"Missing files: {missingFilesCount}");
    }
}