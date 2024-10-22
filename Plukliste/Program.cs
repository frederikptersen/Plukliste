using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Plukliste
{
    class PlukListeProgram
    {
        static void Main()
        {
            char brugerInput = ' ';
            List<string> filer;
            var nuværendeFilIndex = -1;
            var standardFarve = Console.ForegroundColor;
            Directory.CreateDirectory("import");
            Directory.CreateDirectory("print");

            if (!Directory.Exists("export"))
            {
                Console.WriteLine("Mappen \"export\" findes ikke");
                Console.ReadLine();
                return;
            }

            filer = Directory.EnumerateFiles("export").ToList();

            while (brugerInput != 'Q')
            {
                string? printBesked = null;

                if (filer.Count == 0)
                {
                    Console.WriteLine("Ingen filer fundet.");
                }
                else
                {
                    if (nuværendeFilIndex == -1) nuværendeFilIndex = 0;

                    Console.WriteLine($"Plukliste {nuværendeFilIndex + 1} af {filer.Count}");
                    Console.WriteLine($"\nFil: {filer[nuværendeFilIndex]}");

                    var filExtension = Path.GetExtension(filer[nuværendeFilIndex]).ToLower();
                    IPlukListeHandler handler = filExtension switch
                    {
                        ".xml" => new XMLPlukListeHandler(),
                        ".csv" => new CSVPlukListeHandler(),
                        _ => throw new NotSupportedException($"File type {filExtension} is not supported")
                    };

                    handler.HandleFile(filer[nuværendeFilIndex]);

                    VisNavigationsmuligheder();

                    brugerInput = HåndterBrugerInput(filer, ref nuværendeFilIndex);

                    if (printBesked != null)
                    {
                        Console.WriteLine(printBesked);
                    }
                }
            }
        }

        static void VisNavigationsmuligheder()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("F");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("orrige plukseddel");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("N");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("æste plukseddel");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("G");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("enindlæs pluksedler");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("A");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("fslut plukseddel");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("P");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("rint vejledning");
        }

        static char HåndterBrugerInput(List<string> filer, ref int nuværendeFilIndex)
        {
            char brugerInput = Console.ReadKey().KeyChar;
            if (brugerInput >= 'a') brugerInput = (char)(brugerInput - ('a' - 'A'));

            Console.Clear();

            switch (brugerInput)
            {
                case 'G':
                    filer = Directory.EnumerateFiles("export").ToList();
                    nuværendeFilIndex = -1;
                    Console.WriteLine("Pluklister genindlæst.");
                    break;
                case 'F':
                    if (nuværendeFilIndex > 0) nuværendeFilIndex--;
                    break;
                case 'N':
                    if (nuværendeFilIndex < filer.Count - 1) nuværendeFilIndex++;
                    break;
                case 'A':
                    var filUdenSti = filer[nuværendeFilIndex].Substring(filer[nuværendeFilIndex].LastIndexOf('\\') + 1);
                    File.Move(filer[nuværendeFilIndex], $"import\\{filUdenSti}");
                    Console.WriteLine($"Plukseddel {filer[nuværendeFilIndex]} afsluttet.");
                    filer.RemoveAt(nuværendeFilIndex);
                    if (nuværendeFilIndex >= filer.Count) nuværendeFilIndex--;
                    break;
                case 'P':
                    var vejledningFil = filer[nuværendeFilIndex];
                    var vejledningIndhold = File.ReadAllText(vejledningFil);
                    var opdateretIndhold = UdskiftTags(vejledningIndhold);
                    var printFil = $"print\\{Path.GetFileName(vejledningFil)}";
                    File.WriteAllText(printFil, opdateretIndhold);
                    Console.WriteLine($"Vejledning {printFil} klar til udskrivning.");
                    break;
            }

            return brugerInput;
        }

        static string UdskiftTags(string indhold)
        {
            indhold = indhold.Replace("[KUNDENAVN]", "Eksempel Kunde");
            indhold = indhold.Replace("[DATO]", DateTime.Now.ToString("dd-MM-yyyy"));

            return indhold;
        }
    }

    public class CSVPlukListeHandler : IPlukListeHandler
    {
        public void HandleFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var plukListe = new PlukListe
            {
                Name = Path.GetFileNameWithoutExtension(filePath),
                Forsendelse = "pickup",
                Lines = new List<Item>()
            };

            foreach (var line in lines)
            {
                var columns = line.Split(',');
                plukListe.Lines.Add(new Item { Title = columns[0], ProductID = columns[1] });
            }

            // Process the plukListe as needed
            Console.WriteLine($"Processed CSV file for {plukListe.Name}");
        }
    }

    public class XMLPlukListeHandler : IPlukListeHandler
    {
        public void HandleFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(PlukListe));
            using var reader = new StreamReader(filePath);
            var plukListe = serializer.Deserialize(reader) as PlukListe ?? throw new InvalidOperationException("Deserialization failed, resulting in a null PlukListe.");

            // Process the plukListe as needed
            Console.WriteLine($"Processed XML file for {plukListe.Name}");
        }
    }

    public interface IPlukListeHandler
    {
        void HandleFile(string filePath);
    }
}
