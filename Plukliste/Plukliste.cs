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

        public void AddItem(Item item)
        {
            if (item != null)
            {
                Lines.Add(item);
            }
        }
    }

    public class Item
    {
        [XmlElement("ProductID")]
        public string ProductID { get; set; } = string.Empty;

        [XmlElement("Title")]
        public string Title { get; set; } = string.Empty;

        [XmlElement("Type")]
        public ItemType Type { get; set; }

        [XmlElement("Amount")]
        public int Amount { get; set; }
    }

    public enum ItemType
    {
        [XmlEnum("Fysisk")]
        Fysisk,

        [XmlEnum("Print")]
        Print
    }
}
