using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Plukliste
{
    class PlukListeProgram
    {
        enum Command
        {
            Quit = 'Q',
            Forrige = 'F',
            Næste = 'N',
            Genindlæs = 'G',
            Afslut = 'A'
        }

        static void Main()
        {
            char brugerInput = ' ';
            List<string> filer = new();
            int filIndex = -1;

            Directory.CreateDirectory("import");
            if (!Directory.Exists("export"))
            {
                Console.WriteLine("Mappen 'export' findes ikke.");
                return;
            }

            filer = Directory.EnumerateFiles("export").ToList();

            // Hovedløkken
            while (brugerInput != (char)Command.Quit)
            {
                if (filer.Count == 0)
                {
                    Console.WriteLine("Ingen filer fundet.");
                }
                else
                {
                    if (filIndex == -1) filIndex = 0;

                    Console.WriteLine($"Plukliste {filIndex + 1} af {filer.Count}");
                    Console.WriteLine($"Fil: {filer[filIndex]}");

                    VisPlukliste(filer[filIndex]);

                    VisNavigationsmuligheder(filIndex, filer.Count);

                    brugerInput = HåndterBrugerInput(filer, ref filIndex);
                }
            }
        }

        static void VisPlukliste(string fil)
        {
            using (FileStream fileStream = File.OpenRead(fil))
            {
                XmlSerializer xmlSerializer = new(typeof(PlukListe));
                var plukliste = (PlukListe?)xmlSerializer.Deserialize(fileStream);

                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine($"Name: {plukliste.Name}");
                    Console.WriteLine($"Forsendelse: {plukliste.Forsendelse}");
                    Console.WriteLine();
                    Console.WriteLine("{0,-6} {1,-8} {2,-18} {3,-30}", "Antal", "Type", "Produktnr.", "Navn");

                    Console.WriteLine($"Antal varer fundet: {plukliste.Lines.Count}");

                    foreach (var vare in plukliste.Lines)
                    {
                        if (vare != null)
                        {
                            Console.WriteLine($"{vare.Amount,-6} {vare.Type,-8} {vare.ProductID,-18} {vare.Title,-30}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Pluklisten kunne ikke indlæses korrekt eller Lines er tom.");
                }
            }
        }

        static void VisNavigationsmuligheder(int filIndex, int totalFiler)
        {
            Console.WriteLine();
            if (filIndex > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("F");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("orrige plukseddel");
            }

            if (filIndex < totalFiler - 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("N");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("æste plukseddel");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("G");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("enindlæs pluksedler");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("A");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("fslut plukseddel");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Q");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("uit");
        }

        static char HåndterBrugerInput(List<string> filer, ref int filIndex)
        {
            char brugerInput = Console.ReadKey().KeyChar;
            Console.Clear();

            switch (char.ToUpper(brugerInput))
            {
                case 'G':
                    filer = Directory.EnumerateFiles("export").ToList();
                    filIndex = -1;
                    Console.WriteLine("Pluklister genindlæst.");
                    break;
                case 'F':
                    if (filIndex > 0) filIndex--;
                    break;
                case 'N':
                    if (filIndex < filer.Count - 1) filIndex++;
                    break;
                case 'A':
                    FlytFilTilImport(filer[filIndex]);
                    filer.RemoveAt(filIndex);
                    if (filIndex >= filer.Count) filIndex--;
                    break;
                case 'Q':
                    return 'Q';
                default:
                    Console.WriteLine("Ugyldigt valg. Prøv igen.");
                    break;
            }

            return brugerInput;
        }

        static void FlytFilTilImport(string fil)
        {
            string filUdenSti = Path.GetFileName(fil);
            File.Move(fil, $"import\\{filUdenSti}");
            Console.WriteLine($"Plukseddel {filUdenSti} afsluttet og flyttet til import-mappen.");
        }
    }
}