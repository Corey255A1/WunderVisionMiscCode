using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace XmlReadNWrite
{
    [XmlRoot("Classes")]
    public class StudentListFile
    {
        [XmlElement("Class")]
        public List<Class> Classes { get; set; }
    }

    public class Class
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("time")]
        public string Time { get; set; }

        [XmlArray("Students"), XmlArrayItem("Student")]
        public List<Student> Students { get; set; }
    }

    public class Student
    {
        [XmlAttribute("firstname")]
        public string FirstName { get; set; }

        [XmlAttribute("lastname")]
        public string LastName { get; set; }
    }
}
