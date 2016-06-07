using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer;


namespace Persistence
{
    public class DBFacade
    {
        #region Fields
        private string _userName;
        private string _userConnectionString;
        private static DBFacade instance;
        #endregion

        #region Properties
        public static DBFacade Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBFacade();
                return instance;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            private set
            {
                _userName = value;
            }
        }
        #endregion

        #region Constructor
        private DBFacade()
        {
            //Setting the correct connectionString:
            UserName = Environment.UserName;
            if (UserName.ToUpper() == "CYMON343")
                _userConnectionString = "SimonDesktop";
            else if (UserName.ToUpper() == "SIMON")
                _userConnectionString = "SimonLaptop";
            else if (UserName.ToUpper() == "TM_MA")
                _userConnectionString = "MortenLaptop";
            else if (UserName.ToUpper() == "MORTEN")
                _userConnectionString = "MortenDesktop";
        }
        #endregion

        #region Methods
                
        public bool CreateOrder(Order o)
        {
            bool success = false;
            SqlTransaction transAction = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))               

                {
                    conn.Open();                   
                    using (transAction = conn.BeginTransaction())
                    { 
                        //#1 Customer
                        using (SqlCommand custCommand = new SqlCommand("createCustomer", conn) { CommandType = CommandType.StoredProcedure })
                        {
                            custCommand.Transaction = transAction;                            

                            custCommand.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id;
                            custCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@address", SqlDbType.VarChar).Value = o.Customer.Address;
                            custCommand.Parameters.Add("@deliveryAddress", SqlDbType.VarChar).Value = o.Customer.DeliveryAddress;
                            custCommand.Parameters.Add("@email", SqlDbType.VarChar).Value = o.Customer.Email;
                            custCommand.Parameters.Add("@phonePrivate", SqlDbType.VarChar).Value = o.Customer.PhonePrivate;
                            custCommand.Parameters.Add("@phoneWork", SqlDbType.VarChar).Value = o.Customer.PhoneWork;
                            custCommand.Parameters.Add("@phoneCell", SqlDbType.VarChar).Value = o.Customer.PhoneCell;
                            custCommand.Parameters.Add("@fax", SqlDbType.VarChar).Value = o.Customer.Fax;

                            if (custCommand.ExecuteNonQuery() < 1)
                                throw new UnhappyException("Failed to create Customer in DB");
                        }

                        //#2 Order
                        using (SqlCommand orderCommand = new SqlCommand("createOrder", conn) { CommandType = CommandType.StoredProcedure })
                        {
                            orderCommand.Transaction = transAction;

                            string deliveryDateString = o.DeliveryDate.Date.ToString("yyyy-MM-dd");
                            string productionDateString = o.ProductionDate.Date.ToString("yyyy-MM-dd");


                            orderCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                            orderCommand.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id;

                            if (o.MainOrderID != null)
                                orderCommand.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = o.MainOrderID;
                            else
                                orderCommand.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = null;

                            orderCommand.Parameters.Add("@orderNumber", SqlDbType.Int).Value = o.OrderNumber;
                            orderCommand.Parameters.Add("@orderSubject", SqlDbType.Int).Value = o.OrderSubject;
                            orderCommand.Parameters.Add("@orderAlternative", SqlDbType.Int).Value = o.OrderAlternative;
                            orderCommand.Parameters.Add("@deliveryDate", SqlDbType.Date).Value = deliveryDateString;
                            orderCommand.Parameters.Add("@productionDate", SqlDbType.Date).Value = productionDateString;
                            orderCommand.Parameters.Add("@cubicMeters", SqlDbType.Float).Value = o.CubicMeters;
                            orderCommand.Parameters.Add("@numberOfElements", SqlDbType.Float).Value = o.NumOfElements;

                            if (orderCommand.ExecuteNonQuery() < 1)
                                throw new UnhappyException("Failed to create Order in DB");                            
                        }

                        //#3 Link
                        for (int i = 0; i < o.AppendixLinks.Count; i++)
                        {
                            using (SqlCommand linkCommand = new SqlCommand("createLink", conn) { CommandType = CommandType.StoredProcedure })
                            {

                                linkCommand.Transaction = transAction;

                                linkCommand.Parameters.Add("@linkID", SqlDbType.VarChar).Value = o.AppendixLinks[i].ID;
                                linkCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                linkCommand.Parameters.Add("@theLink", SqlDbType.VarChar).Value = o.AppendixLinks[i].TheLink;

                                if (linkCommand.ExecuteNonQuery() < 1)
                                    throw new UnhappyException("Failed to create Link in DB");
                            }
                        }                   

                        //#4 ProductionData
                        for (int i = 0; i < o.ProdData.Count; i++)
                        {
                            using (SqlCommand ProdDataCommand = new SqlCommand("createProdData", conn) { CommandType = CommandType.StoredProcedure })
                            {
                                ProdDataCommand.Transaction = transAction;

                                ProdDataCommand.Parameters.Add("@productionDataID", SqlDbType.VarChar).Value = o.ProdData[i].ID;
                                ProdDataCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ProdData[i].OrderID;

                                if (ProdDataCommand.ExecuteNonQuery() < 1)
                                    throw new UnhappyException("Failed to create Production Data in DB");
                            }

                            //#4.1 Data
                            for (int j = 0; j < o.ProdData[i].Data.Count; j++)
                            {
                                using (SqlCommand DataCommand = new SqlCommand("createData", conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    DataCommand.Transaction = transAction;

                                    DataCommand.Parameters.Add("@dataID", SqlDbType.VarChar).Value = o.ProdData[i].ID + "_DATA_" + (j + 1);
                                    DataCommand.Parameters.Add("@productionDataID", SqlDbType.VarChar).Value = o.ProdData[i].ID;
                                    DataCommand.Parameters.Add("@data", SqlDbType.VarChar).Value = o.ProdData[i].Data[j];

                                    if (DataCommand.ExecuteNonQuery() < 1)
                                        throw new UnhappyException("Failed to create Data Link in DB");
                                }
                            }
                        }

                        //#6 Elements
                        for (int i = 0; i < o.Elements.Count; i++)
                        {
                            //create Element
                            using (SqlCommand ElementCommand = new SqlCommand("createElement", conn) { CommandType = CommandType.StoredProcedure })
                            {
                                ElementCommand.Transaction = transAction;

                                ElementCommand.Parameters.Add("@elementID", SqlDbType.VarChar).Value = o.Elements[i].Id;
                                ElementCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                ElementCommand.Parameters.Add("@position", SqlDbType.VarChar).Value = o.Elements[i].Position;
                                ElementCommand.Parameters.Add("@text", SqlDbType.VarChar).Value = o.Elements[i].Text;
                                ElementCommand.Parameters.Add("@hinge", SqlDbType.VarChar).Value = o.Elements[i].Hinge;
                                ElementCommand.Parameters.Add("@fin", SqlDbType.VarChar).Value = o.Elements[i].Fin;
                                ElementCommand.Parameters.Add("@amount", SqlDbType.Float).Value = o.Elements[i].Amount;
                                ElementCommand.Parameters.Add("@unit", SqlDbType.VarChar).Value = o.Elements[i].Unit;
                                ElementCommand.Parameters.Add("@heading", SqlDbType.VarChar).Value = o.Elements[i].Heading;

                                if (ElementCommand.ExecuteNonQuery() < 1)
                                    throw new UnhappyException("Failed to create Element Link in DB");
                            }

                            //#6.1 EPS 
                            for (int j = 0; j < o.Elements[i].ProgressInfo.Length; j++)
                            {
                                using (SqlCommand EPSCommand = new SqlCommand("createEPS", conn) { CommandType = CommandType.StoredProcedure })
                                {                                    
                                    EPSCommand.Transaction = transAction;

                                    EPSCommand.Parameters.Add("@EPSID", SqlDbType.VarChar).Value = o.Elements[i].ProgressInfo[j].ID;
                                    EPSCommand.Parameters.Add("@elementID", SqlDbType.VarChar).Value = o.Elements[i].Id;
                                    EPSCommand.Parameters.Add("@comment", SqlDbType.VarChar).Value = o.Elements[i].ProgressInfo[j].Comment;
                                    EPSCommand.Parameters.Add("@begun", SqlDbType.Bit).Value = o.Elements[i].ProgressInfo[j].Begun;
                                    EPSCommand.Parameters.Add("@done", SqlDbType.Bit).Value = o.Elements[i].ProgressInfo[j].Done;
                                    EPSCommand.Parameters.Add("@stationNumber", SqlDbType.Int).Value = o.Elements[i].ProgressInfo[j].StationNumber;

                                    if (EPSCommand.ExecuteNonQuery() < 1)
                                        throw new UnhappyException("Failed to create EPS in DB");
                                }
                            }

                        }

                        transAction.Commit(); 
                                               
                        conn.Close();
                        success = true;
                    }
                }
            }
            catch (Exception e)
            {                
                if (e is UnhappyException)
                    Console.Write("[Unhappy Event]: {0}", e.Message);

                try
                {
                    transAction.Rollback();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Rollback failed. {0}", ex.StackTrace.ToString());
                }
                
                success = false;
                Console.WriteLine("Failed to create Order in DB...");
                Console.WriteLine("[Exception Stacktrace]: " + e.StackTrace);
                Console.WriteLine("[Exception Message]: " + e.Message);
            }
            return success;
        }

        public List<Order> RetrieveAllOrders()
        {
            List<Order> orders = new List<Order>();            
            try
            {
                DataTable table = new DataTable();
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                   
                    // 1) Retrieving Customer
                    SqlCommand cmd = new SqlCommand("SELECT * FROM getCustomers", connection);
                    table.Load(cmd.ExecuteReader());

                    List<CustomerData> customers = new List<CustomerData>();
                    foreach (DataRow row in table.Rows)
                    {
                        string id = row["CustomerID"].ToString();
                        string name = row["Name"].ToString();
                        string address = row["Address"].ToString();
                        string deliveryAddress = row["DeliveryAddress"].ToString();
                        string email = row["Email"].ToString();
                        string phonePrivate = row["PhonePrivate"].ToString();
                        string phoneWork = row["PhoneWork"].ToString();
                        string phoneCell = row["PhoneCell"].ToString();
                        string fax = row["Fax"].ToString();
                        customers.Add(new CustomerData(id, name, address, deliveryAddress, email, phonePrivate, phoneWork, phoneCell, fax));
                    }
                    
                    // 2) Retrieving ElementProgressState
                   cmd = new SqlCommand("SELECT * FROM getElementProgressStates", connection);                   

                    table = new DataTable();
                    table.Load(cmd.ExecuteReader());

                    List<ProgressState> eps = new List<ProgressState>();
                    foreach (DataRow row in table.Rows)
                    {
                        string EPSID = row["EPSID"].ToString();
                        string elementID = row["ElementID"].ToString();
                        string comment = row["Comment"].ToString();
                        bool begun = (bool)row["Begun"];
                        bool done = (bool)row["Done"];
                        int stationNumber = (int)row["StationNumber"];
                        eps.Add(new ProgressState(EPSID, elementID, comment, begun, done, stationNumber));
                    }
                    
                    // 3) Retrieving Element
                   cmd = new SqlCommand("SELECT * FROM getElements", connection);

                    table = new DataTable();
                    table.Load(cmd.ExecuteReader());

                    List<Element> elements = new List<Element>();
                    foreach (DataRow row in table.Rows)
                    {
                        string id = row["ElementID"].ToString();
                        string orderID = row["OrderID"].ToString();
                        string position = row["Position"].ToString();
                        string text = row["Text"].ToString();
                        string hinge = row["Hinge"].ToString();
                        string fin = row["Fin"].ToString();
                        double amount = (double)row["Amount"];
                        string unit = row["Unit"].ToString();
                        string heading = row["Heading"].ToString();

                        // 3.1) Adding ElementProgressStates to Elements
                        List<ProgressState> tmpEps = new List<ProgressState>();
                        for (int i = 0; i < eps.Count; i++)
                        {
                            if (eps[i].ParentID == id)
                            {
                                tmpEps.Add(eps[i]);
                                eps.RemoveAt(i);
                                i--;
                            }

                        }
                        elements.Add(new Element(id, orderID, position, text, hinge, fin, amount, unit, heading, tmpEps.ToArray()));
                    }
                    
                    // 4) Retrieving Links
                   cmd = new SqlCommand("SELECT * FROM getLinks", connection);

                    table = new DataTable();
                    table.Load(cmd.ExecuteReader());

                    List<Link> links = new List<Link>();
                    foreach (DataRow row in table.Rows)
                    {
                        string linkID = row["LinkID"].ToString();
                        string orderID = row["OrderID"].ToString();
                        string theLink = row["TheLink"].ToString();

                        links.Add(new Link(linkID, orderID, theLink));
                    }
                    
                    // 5) Retrieving Data for Prod. Data.
                    cmd = new SqlCommand("SELECT * FROM getData", connection);

                    table = new DataTable();
                    table.Load(cmd.ExecuteReader());

                    //Workaround Solution - Two lists..
                    List<string> dataList = new List<string>();
                    List<string> dataOID = new List<string>();
                    foreach (DataRow row in table.Rows)
                    {
                        string productionDataID = row["ProductionDataID"].ToString();
                        string data = row["Data"].ToString();

                        dataList.Add(data);
                        dataOID.Add(productionDataID);
                    }
                   
                    // 6) Retrieving Production Data
                    cmd = new SqlCommand("SELECT * FROM getProductionData", connection);

                    table = new DataTable();
                    table.Load(cmd.ExecuteReader());

                    List<ProductionData> prodData = new List<ProductionData>();
                    foreach (DataRow row in table.Rows)
                    {
                        string prodDataID = row["ProductionDataID"].ToString();
                        string id = row["ProductionDataID"].ToString();
                        string orderID = row["OrderID"].ToString();

                        List<string> data = new List<string>();
                        for(int i = 0; i < dataList.Count; i++)
                        {
                            if(dataOID[i] == id)
                            {
                                data.Add(dataList[i]);
                                dataList.RemoveAt(i);
                                dataOID.RemoveAt(i);
                                i--;
                            }
                        }
                        prodData.Add(new ProductionData(prodDataID, orderID, data));
                    }
                    
                    // 7) Retrieving Orders
                    cmd = new SqlCommand("SELECT * FROM getOrders", connection);

                    table = new DataTable();
                    table.Load(cmd.ExecuteReader());

                    foreach (DataRow row in table.Rows)
                    {
                        string id = row["OrderID"].ToString();
                        string customerID = row["CustomerID"].ToString();
                        string mainOrderID = row["MainOrderID"].ToString();
                        int orderNumber = (int)row["OrderNumber"];
                        int orderSubject = (int)row["OrderSubject"];
                        int orderAlternative = (int)row["OrderAlternative"];
                        DateTime deliveryDate;
                        DateTime.TryParse(row["DeliveryDate"].ToString(), out deliveryDate);
                        DateTime productionDate;
                        DateTime.TryParse(row["ProductionDate"].ToString(), out productionDate);
                        double cubicMeters = (double)row["CubicMeters"];
                        double numberOfElements = (double)row["NumberOfElements"];
                        

                        //7.1 Get Customer
                        CustomerData tmpCust = null;
                        for (int i = 0; i < customers.Count; i++)
                        {
                            if(customerID == customers[i].Id)
                            {
                                tmpCust = customers[i];
                                customers.RemoveAt(i);
                                break;
                            }
                        }

                        //7.2 Get Elements
                        List<Element> tmpElements = new List<Element>();
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if(id == elements[i].OrderID)
                            {
                                tmpElements.Add(elements[i]);
                                elements.RemoveAt(i);
                                i--;
                            }
                        }                     
                        //7.3 Get Links
                        List<Link> tmpLinks = new List<Link>();
                        for (int i = 0; i < links.Count; i++)
                        {
                            if (id == links[i].OrderID)
                            {
                                tmpLinks.Add(links[i]);
                                links.RemoveAt(i);
                                i--;
                            }
                        }
                        //7.4 Get Prod. Data
                        List<ProductionData> tmpProductionData = new List<ProductionData>();
                        for (int i = 0; i < prodData.Count; i++)
                        {
                            if (id == prodData[i].OrderID)
                            {
                                tmpProductionData.Add(prodData[i]);
                                prodData.RemoveAt(i);
                                i--;
                            }
                        }
                        //7.5 Create Order.
                        orders.Add(Order.CreateOrder(id, tmpCust, orderNumber, orderSubject, orderAlternative, deliveryDate, productionDate, cubicMeters,
                                    numberOfElements, tmpLinks, mainOrderID, new List<Order>(), tmpElements, tmpProductionData));
                    }
                    connection.Close();
                   
                    //7.6 Add suborders to orders.
                    for (int i = 0; i < orders.Count; i++)
                    {
                        for (int j = 0; j < orders.Count; j++)
                        {
                            if (orders[i].ID == orders[j].MainOrderID)
                                orders[i].SubOrders.Add(orders[j]);
                        }
                    }

                    //7.7 Check all main lists are empty. (Ensure we got it all)
                    int count = eps.Count + elements.Count + links.Count + dataList.Count + prodData.Count + customers.Count;
                    if(count != 0)
                    {
                        throw new UnhappyException("Failed to use all data from DB.");
                    }
                }                
            }
            catch (Exception e)
            {
                if (e is UnhappyException)
                    Console.Write("Unhappy Event: Not all extracted data used! QQ go PEW PEW!");
                else
                {
                    Console.WriteLine("[StackTrace]: " + e.StackTrace);
                    Console.WriteLine("[Message]:" + e.Message);
                }
            }
            return orders;
        } 

        public bool UpdateElementProgressStateBegun(string elementID, int stationNumber, bool begun)
        {
            bool success = false;
            try
            {
               using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateBegun", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@begun", SqlDbType.Bit).Value = begun;

                        success = (command.ExecuteNonQuery() > 0);
                       
                        connection.Close();
                        }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public bool UpdateElementProgressStateDone(string elementID, int stationNumber, bool done)
        {
            bool success = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                   using (SqlCommand command = new SqlCommand("updateElementProgressStateDone", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@done", SqlDbType.Bit).Value = done;

                        success = (command.ExecuteNonQuery() > 0);

                        connection.Close();
                        }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public bool UpdateElementProgressStateComment(string elementID, int stationNumber, string comment)
        {
            bool success = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateComment", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@comment", SqlDbType.VarChar).Value = comment;

                        success = (command.ExecuteNonQuery() > 0);

                        connection.Close();
                        }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        #endregion
    }

    public class UnhappyException : Exception
    {
        public UnhappyException() { }

        public UnhappyException(string message) : base(message)
        {

        }

        public UnhappyException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
