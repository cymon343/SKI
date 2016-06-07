using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer;
using System.IO;

namespace Business
{
    public static class E02Handler
    {
        private const int KEY_LENGTH = 4;
        private const int NUMBER_OF_STATIONS = 4; //TODO: Make this load dynamically externally.
        public static Order CreateOrder(string e02FileLocation)
        {
            //e02 string is link to file, not actual file data. ^^
            string e02Content = File.ReadAllText(e02FileLocation, System.Text.Encoding.Default);

            // 0) Get OrderID
            string orderID = GetOrderID(e02Content);
            Console.WriteLine("Ordernumber: " + orderID + "\n");

            // 1) - Extract CustomerData
            CustomerData customer = ExtractCustomerData(e02Content);
            Console.WriteLine(customer + "\n"); //TODO: Remove this line after testing is done.

            // 2) - Extract Production Data (Order Meta Data)
            List<ProductionData> prodData = ExtractProductionData(e02Content, orderID);
            Console.WriteLine(prodData + "\n");

            // 3) - Extract Element Data
            List<Element> elements = ExtractElements(e02Content, orderID);
            foreach (Element e in elements)
                Console.WriteLine(e);

            // 4) - Extract & Prepare data for Order Object.
            string[] orderIDInfo = orderID.Split('/');
            int orderNumber = int.Parse(orderIDInfo[0]);
            int orderSubject = int.Parse(orderIDInfo[1]);
            int orderAlternative = int.Parse(orderIDInfo[2]);

            List<DateTime> dates = ExtractDateData(e02Content);

            return Order.CreateOrder(orderID, customer, orderNumber, orderSubject, orderAlternative, dates[0], dates[1], 0, 0, new List<Link>(), "", new List<Order>(), elements, prodData);
        }

        private static string GetOrderID(string e02Content)
        {
            StringReader sr = new StringReader(e02Content);

            string line = sr.ReadLine();
            string orderID = "";

            while (line != null)
            {
                if (line.Substring(0, KEY_LENGTH) == "300;")
                {
                    string[] lineParts = line.Split(';');
                    orderID = lineParts[1];
                    break;
                }
                line = sr.ReadLine();
            }
            return orderID;
        }

        private static CustomerData ExtractCustomerData(string e02Content)
        {
            StringReader sr = new StringReader(e02Content);

            string name = "";
            string address = "";
            string deliveryAddress = "";

            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.Substring(0, KEY_LENGTH) == "213;")
                {
                    string[] lineParts = line.Split(';');
                    name = lineParts[1];
                    address = lineParts[3] + " - " + lineParts[4] + " " + lineParts[5];
                }
                else if (line.Substring(0, KEY_LENGTH) == "305;")
                {
                    string[] lineParts = line.Split(';');
                    deliveryAddress = lineParts[3] + " - " + lineParts[4] + " " + lineParts[5];
                    break;
                }
                line = sr.ReadLine();
            }
            int subStringLength = name.Length > 2 ? 3 : name.Length; //In case name of customer is less than 3 chars.
            string custID = DateTime.Now.ToString() + name.Substring(0, subStringLength);

            return new CustomerData(custID, name, address, deliveryAddress, "", "", "", "", "");
        }

        private static List<ProductionData> ExtractProductionData(string e02Content, string orderID)
        {
            List<string> productData = new List<string>();

            StringReader sr = new StringReader(e02Content);

            string line = sr.ReadLine();

            while (line != null)
            {
                if (line.Substring(0, KEY_LENGTH) == "423;")
                {
                    string[] lineParts = line.Split(';');
                    if (lineParts[4] == "1")
                    {
                        //Notice no check is made for 424;, in the outer scope, as it will always be
                        //preceded by 423;, thus ending up inside "this" scope.
                        line = sr.ReadLine(); //This will always be "424;"
                        string[] subLineParts = line.Split(';');
                        productData.Add(lineParts[2] + ": " + subLineParts[2]);
                    }
                }
                else if (line.Substring(0, KEY_LENGTH) == "425;")
                {
                    string[] lines = line.Split(';');
                    productData.Add(lines[2] + ": " + lines[4]);
                }

                line = sr.ReadLine();
            }

            List<string> tmpData = new List<string>();
            List<ProductionData> resultData = new List<ProductionData>();
            tmpData.Add(productData[0]);
            int i;
            for (i = 1; i < productData.Count; i++)
            {
                if (productData[i].Substring(0, 6) == "Køkken") 
                {
                    resultData.Add(new ProductionData("PD_" + orderID + "_" + DateTime.Now.ToString() + "_" + i, orderID, tmpData));
                    tmpData = new List<string>();
                }
                tmpData.Add(productData[i]);
            }
            resultData.Add(new ProductionData("PD_" + orderID + "_" + DateTime.Now.ToString() + "_" + i, orderID, tmpData));
            return resultData;
        }

        private static List<Element> ExtractElements(string e02Content, string orderID)
        {
            List<Element> elements = new List<Element>();
            Dictionary<string, string> headings = new Dictionary<string, string>();

            StringReader sr = new StringReader(e02Content);

            string line = sr.ReadLine();
            int count = 1;
            while (line != null)
            {
                if (line.Substring(0, KEY_LENGTH) == "430;")
                {
                    string[] lineparts = line.Split(';');
                    headings.Add(lineparts[1], lineparts[2]);
                }
                else if (line.Substring(0, KEY_LENGTH) == "500;")
                {
                    string[] lineParts = line.Split(';');
                    string pos = lineParts[1];
                    if (lineParts[2] != "0")
                        pos += "." + lineParts[2];
                    string text = lineParts[3] + "\n" + lineParts[8];
                    double amount = double.Parse(lineParts[9].Replace('.', ','));
                    string unit = lineParts[10];
                    string key = lineParts[12];

                    string heading;
                    headings.TryGetValue(key, out heading);

                    string hinge = "";
                    string fin = "";

                    line = sr.ReadLine();
                    if (line.Substring(0, KEY_LENGTH) == "501;")
                    {
                        lineParts = line.Split(';');

                        if (lineParts[1] == "1")
                            hinge = "V";
                        else if (lineParts[1] == "2")
                            hinge = "H";

                        if (lineParts[2] == "2")
                        {
                            fin = "V";
                            text += "\nFinering: " + "Venstre";
                        }
                        else if (lineParts[2] == "1")
                        {
                            if (lineParts[6] == "515") 
                            {
                                fin = "F";
                                text += "\nFinering: " + "Forkant";
                            }
                            else
                            {
                                fin = "H";
                                text += "\nFinering: " + "Højre";
                            }
                        }
                        else if (lineParts[2] == "3")
                        {
                            fin = "B";
                            text += "\nFinering: " + "Begge";
                        }
                        else if (lineParts[2] == "10")
                        {
                            fin = "T";
                            text += "\nFinering: " + "Top"; //The data provided does not support "Bottom" Finish
                        }
                    }
                    string id = "ELE_" + orderID + "_" + DateTime.Now.ToString() + count;
                    ProgressState[] eps = CreateProgressStateArray(id);
                    elements.Add(new Element(id, orderID, pos, text, hinge, fin, amount, unit, heading, eps));
                }
                line = sr.ReadLine();
                count++;
            }

            return elements;

        }

        private static List<DateTime> ExtractDateData(string e02Content)
        {
            StringReader sr = new StringReader(e02Content);
            List<DateTime> dates = new List<DateTime>();
            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.Substring(0, KEY_LENGTH) == "300;")
                {
                    string[] lineParts = line.Split(';');

                    //Extract Delivery Date
                    string[] dateParts = lineParts[4].Split('/');
                    dates.Add(new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0])));

                    //Extract Production Date
                    dateParts = lineParts[6].Split('/');
                    dates.Add(new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0])));
                    break;
                }
                line = sr.ReadLine();
            }
            return dates;
        }

        private static ProgressState[] CreateProgressStateArray(string parentID)
        {
            ProgressState[] eps = new ProgressState[NUMBER_OF_STATIONS];

            for (int i = 0; i < NUMBER_OF_STATIONS; i++)
                eps[i] = new ProgressState(parentID + i, parentID, "", false, false, i + NUMBER_OF_STATIONS);
            return eps;
        }
    }
}
