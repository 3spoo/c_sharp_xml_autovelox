using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AConsoleAutovelox
{
    internal class Program
    {
        const string FILE_RILEVAZIONI = "RILEVAZIONI.xml";
        const string FILE_AUTOMEZZI = "AUTOMEZZI.xml";
        const string FILE_REGIONI = "REGIONI.xml";
        const string FILE_AUTOMEZZI_PROPIETARI = "AUTOMEZZI_PROPRIETARI.xml";
        const string FILE_CITTADINI = "CITTADINI.xml";

        static void Main(string[] args)
        {
            XmlDocument xd = new XmlDocument();

            // E. 1
            Dictionary<string, string> oput1 = getMaxSpeed(xd, FILE_RILEVAZIONI);
            Console.WriteLine(ToString(oput1));

            // E. 2
            Dictionary<string, string> oput2 = getCar(
                xd, 
                FILE_AUTOMEZZI,
                oput1
            );
            Console.WriteLine(ToString(oput2));

            // E. 3
            Dictionary<string, string> oput3 = getZone(
                xd, 
                FILE_REGIONI,
                oput2
            );
            Console.WriteLine(ToString(oput3));

            // E. 4
            Dictionary<string, string> oput4 = getPerson(
                xd, FILE_CITTADINI,
                FILE_AUTOMEZZI_PROPIETARI,
                oput3
            );
            Console.WriteLine(ToString(oput4));

            // E. 5
            List<Dictionary<string, string>> oput5 = getHighSpeed(xd, FILE_RILEVAZIONI);

            int i = 0;
            foreach(Dictionary<string, string> tmp in oput5)
            {
                i++;
                Console.WriteLine("AMMONIZIONE URBANA N. " + i + "\n" +
                    ToString(
                        getPerson(
                            xd, FILE_CITTADINI,
                            FILE_AUTOMEZZI_PROPIETARI,
                            getZone(
                                xd,
                                FILE_REGIONI,
                                getCar(
                                    xd,
                                    FILE_AUTOMEZZI,
                                    tmp
                                )
                            )
                        )
                    )
                );
            }

            Console.ReadKey();
        }

        public static Dictionary<string, string> getMaxSpeed(XmlDocument xd, string file)
        {
            xd.Load(file);
            int maxVelocita = -1;
            Dictionary<string, string> dati = new Dictionary<string, string>();

            if (xd.HasChildNodes)
            {
                foreach (XmlNode node in xd.ChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        int velocita = int.Parse(child.ChildNodes[6].InnerText);
                        if (velocita > maxVelocita)
                        {
                            maxVelocita = velocita;

                            string data_en = child.ChildNodes[0].InnerText.ToString();
                            DateTime data = DateTime.Parse(data_en);

                            dati = new Dictionary<string, string> {
                                {child.ChildNodes[0].Name, data.ToString("dd-MM-yyyy")}, // DATA
                                {child.ChildNodes[1].Name, child.ChildNodes[1].InnerText.ToString()}, // ORA
                                {child.ChildNodes[2].Name, child.ChildNodes[2].InnerText.ToString()}, // LUOGO
                                {child.ChildNodes[3].Name, child.ChildNodes[3].InnerText.ToString()}, // S.P.
                                {child.ChildNodes[5].Name, child.ChildNodes[5].InnerText.ToString()}, // TARGA
                                {child.ChildNodes[6].Name, child.ChildNodes[6].InnerText.ToString()}, // VELOCITA
                                {child.ChildNodes[4].Name, child.ChildNodes[4].InnerText.ToString()} // ID
                            };
                        }
                    }
                }
            }

            return dati;
        }

        public static Dictionary<string, string> getCar(XmlDocument xd, string file, Dictionary<string, string> dati)
        {
            xd.Load(file);

            if (xd.HasChildNodes)
            {
                foreach (XmlNode node in xd.ChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        string tmp = child.ChildNodes[0].InnerText;
                        if (dati["TARGA"].Equals(tmp))
                        {
                            dati[child.ChildNodes[1].Name] = child.ChildNodes[1].InnerText.ToString();
                        }
                    }
                }
            }

            return dati;
        }

        public static Dictionary<string, string> getZone(XmlDocument xd, string file, Dictionary<string, string> dati)
        {
            xd.Load(file);

            if (xd.HasChildNodes)
            {
                foreach (XmlNode node in xd.ChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        string tmp = child.ChildNodes[0].InnerText;
                        if (dati["ID_REGIONE"].Equals(tmp))
                        {
                            dati[child.ChildNodes[1].Name] = child.ChildNodes[1].InnerText.ToString(); // PAESE
                        }
                    }
                }
            }

            return dati;
        }

        public static Dictionary<string, string> getPerson(XmlDocument xd, string filep, string filea, Dictionary<string, string> dati)
        {
            xd.Load(filea);
            string idp = "";

            if (xd.HasChildNodes)
            {
                foreach (XmlNode node in xd.ChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        string tmp = child.ChildNodes[0].InnerText;
                        if (tmp.Equals(dati["TARGA"]))
                        {
                            idp = child.ChildNodes[1].InnerText.ToString(); // ID_PERSONA
                        }
                    }
                }
            }

            xd.Load(filep);

            if (xd.HasChildNodes)
            {
                foreach (XmlNode node in xd.ChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        string tmp = child.ChildNodes[0].InnerText;
                        if (tmp.Equals(idp))
                        {
                            // PERSONA
                            dati[child.ChildNodes[1].Name] = child.ChildNodes[2].InnerText.ToString() + child.ChildNodes[1].InnerText.ToString();
                            dati["ETA"] = getEta(child.ChildNodes[4].InnerText.ToString());

                        }
                    }
                }
            }

            return dati;
        }

        public static string getEta(string data)
        {
            int dtn = DateTime.Now.Year;
            int dt = int.Parse(data);

            return (dtn - dt).ToString();
        }

        public static string ToString(Dictionary<string, string> dati)
        {
            string oput = "";

            foreach (KeyValuePair<string, string> dato in dati)
            {
                oput += dato.Key + ": " + dato.Value + "\n";
            }

            return oput;
        }

        // E. 5
        public static List<Dictionary<string, string>> getHighSpeed(XmlDocument xd, string file)
        {
            xd.Load(file);
            int maxVelocita = -1;
            List<Dictionary<string, string>> dati = new List<Dictionary<string, string>>();

            if (xd.HasChildNodes)
            {
                foreach (XmlNode node in xd.ChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        int velocita = int.Parse(child.ChildNodes[6].InnerText);
                        if (velocita > 55 && velocita < 70)
                        {
                            maxVelocita = velocita;

                            string data_en = child.ChildNodes[0].InnerText.ToString();
                            DateTime data = DateTime.Parse(data_en);

                            dati.Add( 
                                new Dictionary<string, string> 
                                    {
                                        {child.ChildNodes[0].Name, data.ToString("dd-MM-yyyy")}, // DATA
                                        {child.ChildNodes[1].Name, child.ChildNodes[1].InnerText.ToString()}, // ORA
                                        {child.ChildNodes[2].Name, child.ChildNodes[2].InnerText.ToString()}, // LUOGO
                                        {child.ChildNodes[3].Name, child.ChildNodes[3].InnerText.ToString()}, // S.P.
                                        {child.ChildNodes[5].Name, child.ChildNodes[5].InnerText.ToString()}, // TARGA
                                        {child.ChildNodes[6].Name, child.ChildNodes[6].InnerText.ToString()}, // VELOCITA
                                        {child.ChildNodes[4].Name, child.ChildNodes[4].InnerText.ToString()} // ID
                                    }
                            );
                        }
                    }
                }
            }

            return dati;
        }

    }
}
