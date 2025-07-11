using FontGenerator.Models;

namespace FontGenerator;

class Program
{
    static void Main(string[] args)
    {
        var xbmFile = XbmFile.Parse(@"/mnt/storage/shakti/projects/Electronics/FMGL Fonts/Extractor/fontforge/FreeSansExtracted/yericyrillic.xbm");
        
        Console.WriteLine("Hello, World!");
    }
}