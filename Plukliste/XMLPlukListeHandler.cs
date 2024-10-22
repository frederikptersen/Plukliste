using System;
using System.IO;
using System.Xml.Serialization;
using Plukliste;

public class XMLPlukListeHandler : IPlukListeHandler
{
    public void HandleFile(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlukListe));
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            var plukListe = serializer.Deserialize(fileStream) as PlukListe;
            if (plukListe != null)
            {
                // Process the plukListe as needed
                Console.WriteLine($"Processed XML file for {plukListe.Name}");
            }
        }
    }
}
