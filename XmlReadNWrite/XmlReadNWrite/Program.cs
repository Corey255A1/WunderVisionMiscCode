using System;
using System.Collections.Generic;
namespace XmlReadNWrite
{
    class Program
    {
        static void Main(string[] args)
        {
            StudentListFile slf = new StudentListFile();
            slf.Classes = new List<Class>() {
                new Class()
                {
                    Name = "History",
                    Time = "0700",
                    Students = new List<Student>()
                    {
                        new Student(){FirstName="Joanna", LastName="Johnson"},
                        new Student(){FirstName="Frederick", LastName="Lemowitz"}
                    }
                },
                new Class()
                {
                    Name = "Mathematics",
                    Time = "0800",
                    Students = new List<Student>()
                    {
                        new Student(){FirstName="Bobby", LastName="Villanova"},
                        new Student(){FirstName="Sandra", LastName="Thomson"}
                    }
                }
            };

            Console.WriteLine("Writing out Class List...");
            XmlUtils.WriteXML(slf, "StudentList.xml");

            Console.WriteLine("Reading in Class List...");
            StudentListFile readFile = XmlUtils.ReadXML<StudentListFile>("StudentList.xml");
            foreach(Class c in readFile.Classes)
            {
                Console.WriteLine($"Class {c.Name} at {c.Time}");
                foreach(Student s in c.Students)
                {
                    Console.WriteLine($"- Student {s.FirstName} {s.LastName}");
                }
            }



        }
    }
}
