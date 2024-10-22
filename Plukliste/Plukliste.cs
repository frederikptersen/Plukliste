using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Plukliste
{
    [XmlRoot("Pluklist")]
    public class PlukListe
    {
        [XmlElement("Name")]
        public string? Name { get; set; }
        [XmlElement("Forsendelse")]
        public string? Forsendelse { get; set; }
        [XmlElement("Adresse")]
        public string? Adresse { get; set; }
        [XmlArray("Lines")]
        [XmlArrayItem("Item")]
        public List<Item> Lines { get; set; } = new List<Item>();
    }

    public class Item
    {
        [XmlElement("Title")]
        public string? Title { get; set; }
        [XmlElement("ProductID")]
        public string? ProductID { get; set; }
    }

    public enum ItemType
    {
        [XmlEnum("Fysisk")]
        Fysisk,

        [XmlEnum("Print")]
        Print
    }
}