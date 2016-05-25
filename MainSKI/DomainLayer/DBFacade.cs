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
    class DBFacade
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
            else if (UserName.ToUpper() == "BRUGER")
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
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    conn.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (transAction = conn.BeginTransaction())
                    {
                        transAction.Save("BEGIN");

                        //#1 Customer
                        using (SqlCommand custCommand = new SqlCommand("createCustomer", conn) { CommandType = CommandType.StoredProcedure })
                        {
                            custCommand.Transaction = transAction;

                            custCommand.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id;
                            custCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@address", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@deliveryAddress", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@email", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@phonePrivate", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@phoneWork", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@phoneCell", SqlDbType.VarChar).Value = o.Customer.Name;
                            custCommand.Parameters.Add("@fax", SqlDbType.VarChar).Value = o.Customer.Name;

                            transAction.Save("Customer");

                        }

                        //#2 Order
                        using (SqlCommand orderCommand = new SqlCommand("createOrder", conn) { CommandType = CommandType.StoredProcedure })
                        {
                            orderCommand.Transaction = transAction;

                            orderCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                            orderCommand.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id; 

                            if (o.MainOrderID != null)
                                orderCommand.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = o.MainOrderID;
                            else
                                orderCommand.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = null;

                            orderCommand.Parameters.Add("@orderNumber", SqlDbType.Int).Value = o.OrderNumber;
                            orderCommand.Parameters.Add("@deliveryDate", SqlDbType.Date).Value = o.DeliveryDate.Date;
                            orderCommand.Parameters.Add("@productionDate", SqlDbType.Date).Value = o.ProductionDate.Date;
                            orderCommand.Parameters.Add("@cubicMeters", SqlDbType.Float).Value = o.CubicMeters;
                            orderCommand.Parameters.Add("@numberOfElements", SqlDbType.Float).Value = o.NumOfElements;
                            
                            transAction.Save("Order");
                        }

                        //#3 Link
                        for (int i = 0; i < o.AppendixLinks.Count; i++)
                        {
                            using (SqlCommand linkCommand = new SqlCommand("createLink", conn) { CommandType = CommandType.StoredProcedure })
                            {

                                linkCommand.Transaction = transAction;

                                linkCommand.Parameters.Add("@linkID", SqlDbType.VarChar).Value = o.ID + "_LINK_" + i+1;
                                linkCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                linkCommand.Parameters.Add("@theLink", SqlDbType.VarChar).Value = o.AppendixLinks[i].TheLink;

                                transAction.Save("Link_" + i);
                            }
                        }

                        //#4 OPS
                        for (int i = 0; i < o.ProgressInfo.Length; i++) 
                        {
                            using (SqlCommand OPSCommand = new SqlCommand("createOPS", conn) { CommandType = CommandType.StoredProcedure })
                            {
                                OPSCommand.Transaction = transAction;

                                OPSCommand.Parameters.Add("@OPSID", SqlDbType.VarChar).Value = o.ID + "_OPS_" + i+1; 
                                OPSCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                OPSCommand.Parameters.Add("@comment", SqlDbType.VarChar).Value = o.ProgressInfo[i].Comment;
                                OPSCommand.Parameters.Add("@begun", SqlDbType.Bit).Value = o.ProgressInfo[i].Begun;
                                OPSCommand.Parameters.Add("@done", SqlDbType.Bit).Value = o.ProgressInfo[i].Done;                                
                                OPSCommand.Parameters.Add("@stationNumber", SqlDbType.Int).Value = o.ProgressInfo[i].StationNumber; 

                                transAction.Save("OPS_" + i);
                            }
                        }

                        //#5 ProductionData
                        for (int i = 0; i < o.ProdData.Count; i++)
                        {
                            using (SqlCommand ProdDataCommand = new SqlCommand("createProdData", conn) { CommandType = CommandType.StoredProcedure })
                            {
                                ProdDataCommand.Transaction = transAction;

                                ProdDataCommand.Parameters.Add("@productionDataID", SqlDbType.VarChar).Value = o.ID + "_PRODDATA_" + i+1; 
                                ProdDataCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;                                
                                transAction.Save("PRODDATA_" + i+1);
                            }

                            //#5.1 Data
                            for (int j = 0; j < o.ProdData[i].Data.Count; j++)                            
                            {                                
                                using (SqlCommand DataCommand = new SqlCommand("createData", conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    DataCommand.Transaction = transAction;

                                    DataCommand.Parameters.Add("@dataID", SqlDbType.VarChar).Value = o.ID + "_PRODDATA_" + i + 1 + "_DATA_" + j + 1;
                                    DataCommand.Parameters.Add("@productionDataID", SqlDbType.VarChar).Value = o.ID + "_PRODDATA_" + i + 1;
                                    DataCommand.Parameters.Add("@data", SqlDbType.VarChar).Value = o.ProdData[i].Data[j];

                                    transAction.Save(o.ID + "_PRODDATA_" + i + 1 + "_DATA_" + j + 1);
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

                                transAction.Save("Element_" + i);
                            }

                            //#6.1 EPS 
                            for (int j = 0; j < o.Elements[i].ProgressInfo.Length; j++)
                            {
                                using (SqlCommand EPSCommand = new SqlCommand("createEPS", conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    EPSCommand.Transaction = transAction;

                                    EPSCommand.Parameters.Add("@EPSID", SqlDbType.VarChar).Value = o.Elements[i].ProgressInfo[j].Id;
                                    EPSCommand.Parameters.Add("@elementID", SqlDbType.VarChar).Value = o.Elements[i].Id;
                                    EPSCommand.Parameters.Add("@comment", SqlDbType.VarChar).Value = o.Elements[i].ProgressInfo[j].Comment;
                                    EPSCommand.Parameters.Add("@begun", SqlDbType.Bit).Value = o.Elements[i].ProgressInfo[j].Begun;
                                    EPSCommand.Parameters.Add("@done", SqlDbType.Bit).Value = o.Elements[i].ProgressInfo[j].Done;
                                    EPSCommand.Parameters.Add("@stationNumber", SqlDbType.Int).Value = o.Elements[i].ProgressInfo[j].StationNumber; 

                                    transAction.Save("EPS_" + i+1 + "_" + j+1);
                                }
                            }

                        }

                        transAction.Commit();
                        Console.WriteLine("Order created in DB...");
                        conn.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
                        success = true;
                    }
                }
            }
            catch (SqlException e)
            {
                transAction.Rollback("BEGIN");
                success = false;
                Console.WriteLine("Failed to create Order in DB...");
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public List<Order> RetrieveAllOrders()
        {
            /*
            ORDER OF BUSINESS:
            EXTRACT EACH TABLE SEPERATLY.
            LOOP TO COMBINE IN THE ORDER DESCRIBED BELOW.

            "ORDER DESCRIBED BELOW":
            1) ElementProgressState
            2) Element
            3) OrderProgressState
            4) Links
            5) ProductionData
            6) Customer
            7) Order
            */
            List<Order> orders = new List<Order>();
            
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");
                DataTable table = new DataTable();
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");

                    // 1) Retrieving Customer
                    Console.WriteLine("Retrieving Customers from DB...");
                    SqlCommand cmd = new SqlCommand("SELECT * FROM getCustomers", connection);
                    table.Load(cmd.ExecuteReader());

                    List<Customer> customers = new List<Customer>();
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
                        customers.Add(new Customer(id, name, address, deliveryAddress, email, phonePrivate, phoneWork, phoneCell, fax));
                    }

                    // 2) Retrieving ElementProgressState
                    Console.WriteLine("Retrieving Element ProgressStates from DB...");
                    cmd = new SqlCommand("SELECT * FROM getElementProgressStates", connection);
                    table.Load(cmd.ExecuteReader());

                    List<ProgressState> eps = new List<ProgressState>();
                    foreach (DataRow row in table.Rows)
                    {
                        string elementID = row["ElementID"].ToString();
                        string comment = row["Comment"].ToString();
                        bool begun = (bool)row["Begun"];
                        bool done = (bool)row["Done"];
                        int stationNumber = (int)row["StationNumber"];
                        eps.Add(new ProgressState(elementID, comment, begun, done, stationNumber));
                    }

                    // 3) Retrieving Element
                    Console.WriteLine("Retrieving Elements from DB...");
                    cmd = new SqlCommand("SELECT * FROM getElements", connection);
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

                    // 4) Retrieving OrderProgressState
                    Console.WriteLine("Retrieving Order ProgressStates from DB...");
                    cmd = new SqlCommand("SELECT * FROM getOrderProgressState", connection);
                    table.Load(cmd.ExecuteReader());

                    List<ProgressState> ops = new List<ProgressState>();
                    foreach (DataRow row in table.Rows)
                    {
                        string orderID = row["OrderID"].ToString();
                        string comment = row["Comment"].ToString();
                        bool begun = (bool)row["Begun"];
                        bool done = (bool)row["Done"];
                        int stationNumber = (int)row["StationNumber"];
                        ops.Add(new ProgressState(orderID, comment, begun, done, stationNumber));
                    }

                    // 5) Retrieving Links
                    Console.WriteLine("Retrieving Links from DB...");
                    cmd = new SqlCommand("SELECT * FROM getLinks", connection);
                    table.Load(cmd.ExecuteReader());

                    List<Link> links = new List<Link>();
                    foreach (DataRow row in table.Rows)
                    {
                        string orderID = row["OrderID"].ToString();
                        string theLink = row["TheLink"].ToString();

                        links.Add(new Link(orderID, theLink));
                    }

                    // 6) Retrieving Data for Prod. Data.

                    Console.WriteLine("Retrieving Data for Prod. Data from DB...");
                    cmd = new SqlCommand("SELECT * FROM getData", connection);
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
                    // 7) Retrieving Production Data
                    Console.WriteLine("Retrieving Production Data from DB...");
                    cmd = new SqlCommand("SELECT * FROM getProductionData", connection);
                    table.Load(cmd.ExecuteReader());

                    List<ProductionData> prodData = new List<ProductionData>();
                    foreach (DataRow row in table.Rows)
                    {
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
                        prodData.Add(new ProductionData(orderID, data));
                    }

                    // 8) Retrieving Orders
                    Console.WriteLine("Retrieving Orders from DB...");
                    cmd = new SqlCommand("SELECT * FROM getOrders", connection);
                    table.Load(cmd.ExecuteReader());

                    foreach (DataRow row in table.Rows)
                    {
                        string id = row["OrderID"].ToString();
                        string customerID = row["CustomerID"].ToString();
                        string mainOrderID = row["MainOrderID"].ToString();
                        int orderNumber = (int)row["OrderNumber"];
                        //TODO: Fix DateTime
                        DateTime deliveryDate;
                        DateTime.TryParse(row["DeliveryDate"].ToString(), out deliveryDate);
                        DateTime productionDate;
                        DateTime.TryParse(row["ProductionDate"].ToString(), out productionDate);
                        double cubicMeters = (double)row["CubicMeters"];
                        double numberOfElements = (double)row["NumberOfElements"];

                        //8.1 Get Customer
                        Customer tmpCust = null;
                        for (int i = 0; i < customers.Count; i++)
                        {
                            if(customerID == customers[i].Id)
                            {
                                tmpCust = customers[i];
                                customers.RemoveAt(i);
                                break;
                            }
                        }

                        //8.2 Get Elements
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
                        //8.3 Get OPS'
                        List<ProgressState> tmpOPS = new List<ProgressState>();
                        for (int i = 0; i < tmpOPS.Count; i++)
                        {
                            if (id == ops[i].ParentID)
                            {
                                tmpOPS.Add(ops[i]);
                                ops.RemoveAt(i);
                                i--;
                            }
                        }
                        //8.4 Get Links
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
                        //8.5 Get Prod. Data
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
                        //8.6 Create Order.
                        orders.Add(Order.CreateOrder(id, tmpCust, orderNumber, deliveryDate, productionDate, cubicMeters,
                                    numberOfElements, tmpLinks, mainOrderID, new List<Order>(), tmpElements, tmpOPS.ToArray(), tmpProductionData));
                    }
                    connection.Close();
                    Console.WriteLine(UserName + " disconnected from DB...");

                    //8.7 Add suborders to orders.
                    for (int i = 0; i < orders.Count; i++)
                    {
                        for (int j = 0; j < orders.Count; j++)
                        {
                            if (orders[i].ID == orders[j].MainOrderID)
                                orders[i].SubOrders.Add(orders[j]);
                        }
                    }

                    //8.8 Check all main lists are empty. (Ensure we got it all)
                    int count = eps.Count + ops.Count + elements.Count + links.Count + dataList.Count + prodData.Count + customers.Count;
                    if(count != 0)
                    {
                        throw new UnhappyException();
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SqlException)
                    Console.WriteLine(e.StackTrace);
                else if (e is UnhappyException)
                    Console.Write("Unhappy Event: Not all extracted data used! QQ go PEW PEW!");
            }
            return orders;
        }

        public bool UpdateOrderProgressStateBegun(string orderID, int stationNumber, bool begun)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateOrderProgressStateBegun", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = orderID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@begun", SqlDbType.Bit).Value = begun;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("OrderProgressStateBegun has been set in DB...");
                        else
                            Console.WriteLine("Failed to set OrderProgressStateBegun in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public bool UpdateOrderProgressStateDone(string orderID, int stationNumber, bool done)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateOrderProgressStateDone", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = orderID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@done", SqlDbType.Bit).Value = done;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("OrderProgressStateDone has been set in DB...");
                        else
                            Console.WriteLine("Failed to set OrderProgressStateDone in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public bool UpdateOrderProgressStateComment(string orderID, int stationNumber, string comment)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateOrderProgressStateComment", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = orderID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@comment", SqlDbType.VarChar).Value = comment;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("OrderProgressStateComment has been set in DB...");
                        else
                            Console.WriteLine("Failed to set OrderProgressStateComment in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public bool UpdateElementProgressStateBegun(string elementID, int stationNumber, bool begun)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateBegun", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@begun", SqlDbType.Bit).Value = begun;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("ElementProgressStateBegun has been set in DB...");
                        else
                            Console.WriteLine("Failed to set ElementProgressStateBegun in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
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
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateDone", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@done", SqlDbType.Bit).Value = done;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("ElementProgressStateDone has been set in DB...");
                        else
                            Console.WriteLine("Failed to set ElementProgressStateDone in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
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
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateComment", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@comment", SqlDbType.VarChar).Value = comment;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("ElementProgressStateComment has been set in DB...");
                        else
                            Console.WriteLine("Failed to set ElementProgressStateComment in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
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
