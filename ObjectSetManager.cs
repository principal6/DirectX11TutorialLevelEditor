using System;
using System.Collections.Generic;
using System.Xml;

using SSize = System.Drawing.Size;

namespace DirectX11TutorialObjectEditor
{
    public class ObjectSetElementData
    {
        public string ElementName { set; get; }

        public int OffsetU { set; get; }

        public int OffsetV { set; get; }

        public SSize Size { set; get; }
    }

    public class ObjectSetData
    {
        public string ObjectSetName { set; get; }

        public string TextureFileName { set; get; }

        public List<ObjectSetElementData> Elements { set; get; }   
    }

    class ObjectSetManager
    {
        public ObjectSetData ObjectSet { get; set; }

        public void CreateNewObjectSet(string ObjectSetName, string TextureFileName)
        {
            ObjectSet = new ObjectSetData
            {
                ObjectSetName = ObjectSetName,
                TextureFileName = TextureFileName,
                Elements = new List<ObjectSetElementData>()
            };
        }

        public void LoadFromFile(string FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            XmlElement xml_root = (XmlElement)doc.FirstChild;

            XmlElement xml_object_set_info = (XmlElement)xml_root.FirstChild;

            ObjectSet = new ObjectSetData
            {
                ObjectSetName = xml_object_set_info.GetAttribute("Name"),
                TextureFileName = xml_object_set_info.GetAttribute("Texture")
            };

            XmlElement xml_object_set = (XmlElement)xml_object_set_info.NextSibling;

            if (xml_object_set == null) return;

            ObjectSet.Elements = new List<ObjectSetElementData>();
            XmlElement xml_element = (XmlElement)xml_object_set.FirstChild;
            while (xml_element != null)
            {
                ObjectSetElementData new_element = new ObjectSetElementData
                {
                    ElementName = xml_element.GetAttribute("Name"),
                    OffsetU = Convert.ToInt32(xml_element.GetAttribute("OffsetU")),
                    OffsetV = Convert.ToInt32(xml_element.GetAttribute("OffsetV")),
                    Size = new SSize(
                        Convert.ToInt32(xml_element.GetAttribute("Width")),
                        Convert.ToInt32(xml_element.GetAttribute("Height")))
                };

                ObjectSet.Elements.Add(new_element);

                xml_element = (XmlElement)xml_element.NextSibling;
            }
        }

        public void SaveToFile(string FileName)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement xml_root = doc.CreateElement("DirectX11TutorialObjectSet");

            XmlElement xml_object_set_info = doc.CreateElement("ObjectSetInfo");
            xml_object_set_info.SetAttribute("Name", ObjectSet.ObjectSetName);
            xml_object_set_info.SetAttribute("Texture", ObjectSet.TextureFileName);

            XmlElement xml_object_set = doc.CreateElement("ObjectSet");
            foreach (ObjectSetElementData element in ObjectSet.Elements)
            {
                XmlElement xml_element = doc.CreateElement("Element");
                xml_element.SetAttribute("Name", element.ElementName);
                xml_element.SetAttribute("OffsetU", element.OffsetU.ToString());
                xml_element.SetAttribute("OffsetV", element.OffsetV.ToString());
                xml_element.SetAttribute("Width", element.Size.Width.ToString());
                xml_element.SetAttribute("Height", element.Size.Height.ToString());

                xml_object_set.AppendChild(xml_element);
            }

            xml_root.AppendChild(xml_object_set_info);
            xml_root.AppendChild(xml_object_set);

            doc.AppendChild(xml_root);

            doc.Save(FileName);
        }
    }
}
