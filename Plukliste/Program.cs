using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Plukliste;
//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag

class Program
{
    const string _exportPath = "export";
    const string _importPath = "import";
    const string _templatePath = "templates";
    const string _printPath = "print";
    static List<string> _files = GetFiles(_exportPath);
    static int _currentFileIndex = 0;

    static List<string> GetFiles(string path)
    {
        Directory.CreateDirectory("export");
        return Directory.EnumerateFiles("export").Where(file => file.EndsWith(".XML")).ToList();
    }

    static void Main(String[] args)
    {
        char menuAction = ' ';
        while (menuAction != 'Q')
        {
            Pluklist? pluklist = PrintPlukseddelToScreen();

            menuAction = PrintMenuAndGetAction(pluklist);
            Console.Clear();
            switch (menuAction)
            {
                case 'G':
                    _files = GetFiles(_exportPath);
                    _currentFileIndex = 0;
                    PrintStatusLine("Pluklister genindlæst");
                    break;
                case 'F':
                    if (_currentFileIndex > 0) _currentFileIndex--;
                    break;
                case 'N':
                    if (_currentFileIndex < _files.Count - 1) _currentFileIndex++;
                    break;
                case 'A':
                    FinishPlukseddel();
                    break;
                case 'P':
                    PrintPapers(pluklist);
                    break;
            }
        }
    }



    private static Pluklist? PrintPlukseddelToScreen()
    {
        if (_files.Count == 0)
        {
            Console.WriteLine("No files found.");
            return null;
        }
        Console.WriteLine($"Plukliste {_currentFileIndex + 1} af {_files.Count}");
        Console.WriteLine($"\nfile: {_files[_currentFileIndex]}");

        Pluklist? plukliste = ReadPlukliste();
        PrintPluklist(plukliste);
        return plukliste;
    }

    private static Pluklist? ReadPlukliste()
    {
        using (var file = File.OpenRead(_files[_currentFileIndex]))
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
            return (Pluklist?)xmlSerializer.Deserialize(file);
        }
    }

    private static void PrintPluklist(Pluklist? plukliste)
    {
        if (plukliste != null && plukliste.Lines != null)
        {
            Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
            Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
            Console.WriteLine("{0, -13}{1}", "Adresse:", plukliste.Adresse);

            Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
            foreach (var item in plukliste.Lines)
            {
                Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
            }
        }
    }

    static char PrintMenuAndGetAction(Pluklist? pluklist)
    {
        Console.WriteLine("\n\nOptions:");

        if (pluklist != null)
        {
            if (pluklist.Lines.Any(x => x.Type == ItemType.Print))
                PrintMenuOption("Print vejledninger");
            PrintMenuOption("Afslut plukseddel\n");
        }
        if (_currentFileIndex > 0)
            PrintMenuOption("Forrige plukseddel");

        if (_currentFileIndex < _files.Count - 1)
            PrintMenuOption("Næste plukseddel");

        Console.WriteLine();
        PrintMenuOption("Genindlæs pluksedler");
        PrintMenuOption("Quit");
        var readKey = Console.ReadKey().KeyChar;
        if (readKey >= 'a') readKey -= (char)('a' - 'A');
        return readKey;
    }

    static private void PrintMenuOption(string text, int highlightPosition = 0)
    {
        if (highlightPosition < 0 || highlightPosition >= text.Length) throw new IndexOutOfRangeException($"highlightPosition {highlightPosition} out of index in {text}");
        var currentColor = Console.ForegroundColor;
        if (highlightPosition > 0) Console.Write(text.Substring(0, highlightPosition));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(text.Substring(highlightPosition, 1));
        Console.ForegroundColor = currentColor;
        Console.WriteLine(text.Substring(highlightPosition + 1));
    }

    static private void PrintStatusLine(string text)
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = currentColor;
    }
    private static void FinishPlukseddel()
    {
        Directory.CreateDirectory("import");
        var filewithoutPath = _files[_currentFileIndex].Substring(_files[_currentFileIndex].LastIndexOf('\\'));
        File.Move(_files[_currentFileIndex], string.Format(@"import\\{0}", filewithoutPath));
        PrintStatusLine($"Plukseddel {_files[_currentFileIndex]} afsluttet.");
        _files.Remove(_files[_currentFileIndex]);
        if (_currentFileIndex == _files.Count) _currentFileIndex--;
    }

    private static void PrintPapers(Pluklist? pluklist)
    {
        if (pluklist == null) return;
        pluklist.Lines.ForEach(x =>
        {
            if (x.Type == ItemType.Print)
                PrintPaper(x.ProductID, pluklist);
        });
    }

    private static void PrintPaper(string template, Pluklist pluklist)
    {
        try
        {
            var text = File.ReadAllText($"{_templatePath}\\{template}.html");

            text = text.Replace("[Adresse]", pluklist.Adresse);
            text = text.Replace("[Name]", pluklist.Name);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<table><tr><th>Antal</th><th>ProduktId</th><th>Beskrivelse</th></tr>");
            pluklist.Lines.ForEach(x =>
            {
                if (x.Type == ItemType.Fysisk)
                    stringBuilder.Append($"<tr><td>{x.Amount}</td><td>{x.ProductID}</td><td>{x.Title}</td></tr>");
            });
            stringBuilder.Append("</table>");
            text = text.Replace("[Plukliste]", stringBuilder.ToString());

            Directory.CreateDirectory(_printPath);
            var filename = Path.GetFileName(_files[_currentFileIndex]);
            filename = $"{filename.Substring(0, filename.IndexOf("_") + 1)}{template}.html";
            File.WriteAllText($"{_printPath}\\{filename}", text);
            PrintStatusLine($"{filename} printet");
        }
        catch (FileNotFoundException)
        {
            PrintStatusLine($"Template {template} not found and is not printet");
            return;
        }
    }
}