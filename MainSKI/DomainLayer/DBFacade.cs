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
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("createOrder", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.ID;
                        //TODO 1) CREATE CUSTOMER 
                        command.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id; //Cannot be null

                        if (o.MainOrder != null)
                            command.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = o.MainOrder.ID;//This guarantees that we never need to handle subOrders on order (DB) creation
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

        private bool CreateCustomer(Customer customer, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateLink(List<string> appendixLinks, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateOPS(ProgressState[] progressInfo, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateProductionData(List<ProductionData> prodData, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateElements(List<Element> elements, SqlConnection connection)
        {
            bool success = false;
            //Foreach element:
            for (int i = 0; i < elements.Count; i++)
            {
                //Create the Element...
                if (CreateEPS(elements[i].ProgressInfo, connection))
                {
                    success = true;
                }
            }

            throw new NotImplementedException();
        }

        private bool CreateEPS(ProgressState[] progressInfo, SqlConnection connection)
        {
            throw new NotImplementedException();
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
                    cmd = new SqlCommand("SELECT * FROM getCustomers", connection);
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
                        customers.Add(new Customer(id, name, address, deliveryAddress, email, phonePrivate, phoneWork, phoneCell, fax);
                    }

                    // 2) Retrieving Orders
                    Console.WriteLine("Retrieving Orders from DB...");
                    SqlCommand cmd = new SqlCommand("SELECT * FROM getOrders", connection);
                    table.Load(cmd.ExecuteReader());

                    List<Order> orders = new List<Order>();
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
                    SqlCommand cmd = new SqlCommand("SELECT * FROM getElementProgressStates", connection);
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
                        string orderID =  = row["OrderID"].ToString();
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
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
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
