using FontGenerator.Models;

namespace FontGenerator;

class Program
{
    static void Main(string[] args)
    {
        //var xbmFile = XbmFile.Parse(@"/mnt/storage0/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSansExtracted/yericyrillic.xbm");

        var sourceDirectory =
            @"/mnt/storage0/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSansExtracted/";
        
        var xbmFiles = Directory.EnumerateFiles(sourceDirectory, "*.xbm", SearchOption.AllDirectories);

        var parsedXbmFiles = xbmFiles
            .Select(f => XbmFile.Parse(f))
            .ToList();
        
        Console.WriteLine("Hello, World!");
    }
}