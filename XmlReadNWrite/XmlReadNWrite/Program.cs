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
                        new Student(){FirstName="Joanna", LastName="Johnson", StudentID=556825},
                        new Student(){FirstName="Frederick", LastName="Lemowitz", StudentID=567864}
                    }
                },
                new Class()
                {
                    Name = "Mathematics",
                    Time = "0800",
                    Students = new List<Student>()
                    {
                        new Student(){FirstName="Bobby", LastName="Villanova", StudentID=568845},
                        new Student(){FirstName="Sandra", LastName="Thomson", StudentID=574568}
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
                    Console.WriteLine($"- Student {s.FirstName} {s.LastName} ID:{s.StudentID}");
                }
            }
        }
    }
}
