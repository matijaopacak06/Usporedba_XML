using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Usporedba_XML
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string oldXML = @"<Books>
     <book id='1' image='C01' name='C# in Depth'/>
     <book id='2' image='C02' name='ASP.NET'/>
     <book id='3' image='C03' name='LINQ in Action '/>
     <book id='4' image='C04' name='Architecting Applications'/>

    </Books>";

            string newXML = @"<Books>
     <book id='1' image='C011' name='C# in Depth'/>
     <book id='2' image='C02' name='ASP.NET 2.0'/>
     <book id='3' image='XXXC03' name='XXXLINQ in Action '/>
     <book id='4' image='C04' name='Architecting Applications'/>
    <book id='5' image='C05' name='PowerShell in Action'/>

    </Books>";

            XDocument xmlOld = XDocument.Parse(oldXML);
            XDocument xmlNew = XDocument.Parse(newXML);

            var res = (from b1 in xmlOld.Descendants("book")
                       from b2 in xmlNew.Descendants("book")
                       let issues = from a1 in b1.Attributes()
                                    join a2 in b2.Attributes()
                                      on a1.Name equals a2.Name
                                    select new
                                    {
                                        Id = a1.Parent.FirstAttribute.Value,
                                        Name = a1.Name,
                                        Value1 = a1.Value,
                                        Value2 = a2.Value
                                    }
                       where issues.Any(i => i.Value1 == i.Value2)
                       from issue in issues
                       where issue.Value1 != issue.Value2
                       select issue);
            var reportXmlItems = (from rx in res select new XElement("book", new XAttribute("id", rx.Id))).Distinct(new MyComparer());

            // This isn't excluding the ids that exist in theold book set because they are different elements I guess and I need to exclude based on the element Id
            var res2 = (from b2 in xmlNew.Descendants("book") select new XElement("book", new XAttribute("id", b2.Attribute("id").Value))).Except(xmlOld.Descendants("book"));

            var res3 = reportXmlItems.Union(res2);

            var reportXml = new XElement("books", res3);
            reportXml.Save(@"c:\test\result.xml");
        }

        public class MyComparer : IEqualityComparer<XElement>
        {
            public bool Equals(XElement x, XElement y)
            {
                return x.Attribute("id").Value == y.Attribute("id").Value;
            }

            public int GetHashCode(XElement obj)
            {
                return obj.Attribute("id").Value.GetHashCode();
            }
        }
    }
    }

