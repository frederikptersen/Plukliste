using System;
using System.Collections.Generic;
using System.IO;

public class CSVPlukListeHandler : IPlukListeHandler
{
    public void HandleFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var plukListe = new PlukListe
        {
            Name = Path.GetFileNameWithoutExtension(filePath) ?? string.Empty,
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

public class PlukListe
{
    // Constructor initializes non-nullable properties
    public PlukListe()
    {
        Name = string.Empty;
        Forsendelse = string.Empty;
        Lines = new List<Item>();
    }

    public string Name { get; set; }
    public string Forsendelse { get; set; }
    public List<Item> Lines { get; set; }
}

public class Item
{
    public string? Title { get; set; }
    public string? ProductID { get; set; }
}

