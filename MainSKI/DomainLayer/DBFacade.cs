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

        //Original CreateOrder (incomplete) is kept in case I destroy it trying to create a batch opposed to several DB-calls
        /*
        public bool CreateOrder(Order o)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("createOrder", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.Id;
                        //TODO 1) CREATE CUSTOMER 
                        command.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id; //Cannot be null
                                                
                        if (o.MainOrder != null)
                            command.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = o.MainOrder.Id;//This guarantees that we never need to handle subOrders on order (DB) creation
                        else
                            command.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = null;

                        command.Parameters.Add("@orderNumber", SqlDbType.Int).Value = o.OrderNumber;
                        command.Parameters.Add("@deliveryDate", SqlDbType.Date).Value = o.DeliveryDate.Date;
                        command.Parameters.Add("@productionDate", SqlDbType.Date).Value = o.ProductionDate.Date;
                        command.Parameters.Add("@cubicMeters", SqlDbType.Float).Value = o.CubicMeters;
                        command.Parameters.Add("@numberOfElements", SqlDbType.Float).Value = o.NumOfElements;

                        if (CreateCustomer(o.Customer, connection))
                        {
                            if (command.ExecuteNonQuery() > 0) //IF order is created
                            {
                                if (CreateLink(o.AppendixLinks, connection))
                                {
                                    if (CreateOPS(o.ProgressInfo, connection))
                                    {
                                        if (CreateProductionData(o.ProdData, connection))
                                        {
                                            if (CreateElements(o.Elements, connection))
                                            {
                                                success = true;                                               
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (success)
                            Console.WriteLine("Order created in DB...");
                        else
                            Console.WriteLine("Failed to create Order in DB...");

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
        */
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
                            orderCommand.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id; //Cannot be null

                            if (o.MainOrder != null)
                                orderCommand.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = o.MainOrder.ID;//This guarantees that we never need to handle subOrders on order (DB) creation
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

                                linkCommand.Parameters.Add("@linkID", SqlDbType.VarChar).Value = o.ID + "_LINK_" + i; //TODO: DESIGN HOW THESE IDs ARE SUPPOSED TO WORK
                                linkCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                linkCommand.Parameters.Add("@theLink", SqlDbType.VarChar).Value = o.AppendixLinks[i];

                                transAction.Save("Link_" + i);
                            }
                        }

                        //#4 OPS
                        for (int i = 0; i < o.ProgressInfo.Length; i++) //
                        {
                            using (SqlCommand OPSCommand = new SqlCommand("createOPS", conn) { CommandType = CommandType.StoredProcedure })
                            {
                                OPSCommand.Transaction = transAction;

                                OPSCommand.Parameters.Add("@OPSID", SqlDbType.VarChar).Value = o.ID + "_OPS_" + i; //TODO: DESIGN HOW THESE IDs ARE SUPPOSED TO WORK (No ID Field in ProgressState)
                                OPSCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                OPSCommand.Parameters.Add("@comment", SqlDbType.VarChar).Value = o.ProgressInfo[i].Comment;
                                OPSCommand.Parameters.Add("@begun", SqlDbType.Bit).Value = o.ProgressInfo[i].Begun;
                                OPSCommand.Parameters.Add("@done", SqlDbType.Bit).Value = o.ProgressInfo[i].Done;
                                OPSCommand.Parameters.Add("@stationNumber", SqlDbType.Int).Value = i + 4; //TODO: Make sure we handle this in the right spot. Either here or in the controller Layer. ++ How do we ensure that the ProgressState[] on Order are of size 4?

                                transAction.Save("OPS_" + i);
                            }
                        }

                        //#5 ProductionData  ********** - TODO FIND OUT HOW TO STORE THIS PROPERLY - **********
                        for (int i = 0; i < o.ProdData.Count; i++)
                        {
                            for (int j = 0; j < o.ProdData[i].Data.Count; j++)                            
                            {
                                using (SqlCommand ProdDataCommand = new SqlCommand("createProdData", conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    ProdDataCommand.Transaction = transAction;

                                    ProdDataCommand.Parameters.Add("@productionDataID", SqlDbType.VarChar).Value = o.ID + "_PRODDATA_" + i + "_" + j; //TODO: DESIGN HOW THESE IDs ARE SUPPOSED TO WORK (No ID Field in ProductionData)
                                    ProdDataCommand.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                                    ProdDataCommand.Parameters.Add("@data", SqlDbType.VarChar).Value = o.ProdData[i].Data[j];

                                    transAction.Save("PRODDATA_" + i + "_" + j);
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

                            //#6.1 FOREACH ELEMENT --> Create EPS !!!!
                            for (int j = 0; j < o.Elements[i].ProgressInfo.Length; j++)
                            {
                                using (SqlCommand EPSCommand = new SqlCommand("createEPS", conn) { CommandType = CommandType.StoredProcedure })
                                {
                                    EPSCommand.Transaction = transAction;

                                    EPSCommand.Parameters.Add("@EPSID", SqlDbType.VarChar).Value = o.Elements[i].Id + "_EPS_" + i; //TODO: DESIGN HOW THESE IDs ARE SUPPOSED TO WORK (No ID Field in ProgressState)
                                    EPSCommand.Parameters.Add("@elementID", SqlDbType.VarChar).Value = o.Elements[i].Id;
                                    EPSCommand.Parameters.Add("@comment", SqlDbType.VarChar).Value = o.Elements[i].ProgressInfo[j].Comment;
                                    EPSCommand.Parameters.Add("@begun", SqlDbType.Bit).Value = o.Elements[i].ProgressInfo[j].Begun;
                                    EPSCommand.Parameters.Add("@done", SqlDbType.Bit).Value = o.Elements[i].ProgressInfo[j].Done;
                                    EPSCommand.Parameters.Add("@stationNumber", SqlDbType.Int).Value = j + 4; //TODO: Make sure we handle this in the right spot. Either here or in the controller Layer. ++ How do we ensure that the ProgressState[] on Element are of size 4?

                                    transAction.Save("EPS_" + i + "_" + j);
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

                    // 2) Retrieving Orders
                    Console.WriteLine("Retrieving Orders from DB...");
                    cmd = new SqlCommand("SELECT * FROM getOrders", connection);
                    table.Load(cmd.ExecuteReader());


                    foreach (DataRow row in table.Rows)
                    {
                        string id = row["OrderID"].ToString();
                        string customerID = row["CustomerID"].ToString();
                        string mainOrderID = row["MainOrderID"].ToString();
                        string OrderNumber = row["OrderNumber"].ToString();
                        DateTime deliveryDate = (DateTime)row["DeliveryDate"];
                        DateTime productionDate = (DateTime)row["ProductionDate"];
                        double cubicMeters = (double)row["CubicMeters"];
                        double numberOfElements = (double)row["NumberOfElements"];

                        // 2.1) Adding customer to Order.
                        for (int i = 0; i < customers.Count; i++)
                        {
                            if (customers[i].Id == customerID)
                            {
                                orders.Add(Order.CreateOrder(id, customers[i], OrderNumber, deliveryDate, productionDate, cubicMeters, numberOfElements));
                                customers.RemoveAt(i);
                                i--;
                                break;
                            }
                        }

                    }

                    // 3) Retrieving ElementProgressState
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
                        eps.Add(new ProgressState(elementID, comment, begun, done));
                    }

                    // 4) Retrieving Element
                    Console.WriteLine("Retrieving Elements from DB...");
                    cmd = new SqlCommand("SELECT * FROM getElements", connection);
                    table.Load(cmd.ExecuteReader());

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

                        // 2.1) Adding ElementProgressStates to Elements
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
                        foreach (Order o in orders)
                        {
                            if (o.ID == orderID)
                                o.Elements.Add(new Element(id, position, text, hinge, fin, amount, unit, heading, tmpEps.ToArray()));
                        }
                    }

                    // 5) Retrieving OrderProgressState
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
                        ops.Add(new ProgressState(orderID, comment, begun, done));
                    }


                    // 6) Retrieving Links
                    Console.WriteLine("Retrieving Links from DB...");
                    cmd = new SqlCommand("SELECT * FROM getLinks", connection);
                    table.Load(cmd.ExecuteReader());

                    foreach (DataRow row in table.Rows)
                    {
                        string orderID = row["OrderID"].ToString();
                        string theLink = row["TheLink"].ToString();

                        foreach (Order o in orders)
                        {
                            if (orderID == o.ID)
                            {
                                o.AppendixLinks.Add(theLink);
                                break;
                            }
                        }
                    }

                    // 7) Retrieving Production Data
                    Console.WriteLine("Retrieving Production Data from DB...");
                    cmd = new SqlCommand("SELECT * FROM getProductionData", connection);
                    table.Load(cmd.ExecuteReader());

                    foreach (DataRow row in table.Rows)
                    {
                        string orderID = row["OrderID"].ToString();
                        string data = row["Data"].ToString();

                        foreach (Order o in orders)
                        {
                            if (orderID == o.ID)
                            {
                                //LOOK INTO THIS - As Simon said - Something something with the ProductionData Object.
                                //o.AppendixLinks.Add(theLink);
                                break;
                            }
                        }
                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");

                    }

                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
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
}
