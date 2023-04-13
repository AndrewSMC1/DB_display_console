// See https://aka.ms/new-console-template for more information

//Andrew Kernycky
//Where I found how to use the Datatable[] select to look through a data table
//https://stackoverflow.com/questions/17567830/select-a-datarow-from-a-datarow-collection


using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:5432;User Id=postgres; " +
           "Password=*****;Database=prods;");
        conn.Open();

        // Define a query returning a single row result set
		/*
		 Change the table which the data is pulled from the first one for question 7 
		 Second one for question 20
		 */

		//Question 7
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM product", conn);

		//Question 20
		//NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM customer", conn);


		//NpgsqlCommand quantity_command = new NpgsqlCommand("SELECT prod_id, prod_desc, prod_quantity from product WHERE prod_quantity <=30 and prod_quantity >= 12", conn);
		//NpgsqlCommand repids_command = new NpgsqlCommand("SELECT rep_id, SUM(cust_balance) AS rep_total_bal FROM customer GROUP BY rep_id", conn);

		NpgsqlDataReader reader = command.ExecuteReader();

        
		DataTable dt = new DataTable();
		dt.Load(reader);
		
		//print_results(dt);

		//Question 7
        PrintSeven(dt);

		//Question 20
        //printreps(dt);
	
        conn.Close();
    }

    static void PrintSeven(DataTable data)
        
    {
		DataTable sorted_data = new DataTable();
        foreach (DataColumn col in data.Columns)
        {
			//adds the columns to the new data table and the data type
			if (col.ColumnName.Contains("prod_id") || col.ColumnName.Contains("prod_desc") || col.ColumnName.Contains("prod_quantity"))
            {
                sorted_data.Columns.Add(col.ColumnName, col.DataType);
                
            }

            
		}
        foreach (DataRow row in data.Rows)
        {
			//turns the quanity into ints to compare if it is between 12 or 30
         int rowdata = Convert.ToInt32(row["prod_quantity"]);
         if  (rowdata >= 12 && rowdata<= 30){
            DataRow sorted_row = sorted_data.NewRow();
            foreach (DataColumn col in sorted_data.Columns)
            {
                
                sorted_row[col.ColumnName] = row[col.ColumnName];
            }
			sorted_data.Rows.Add(sorted_row); 
           };
			
		}
        Console.Write(sorted_data.Columns);
        Console.Write(sorted_data.Rows);
		Console.WriteLine();
        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        foreach (DataColumn col in sorted_data.Columns)
        {
            
			
			Console.Write(col.ColumnName);
            var maxLabelSize = data.Rows.OfType<DataRow>()
                    .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                    .OrderByDescending(m => m).FirstOrDefault();

            colWidths.Add(col.ColumnName, maxLabelSize);
            for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
       
            
        }

        Console.WriteLine();

       foreach (DataRow dataRow in sorted_data.Rows)
        {
            for (int j = 0; j < dataRow.ItemArray.Length; j++)
            {
               Console.Write(dataRow.ItemArray[j]);
                for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
            }
            Console.WriteLine();
        }

    }
	static void printreps(DataTable data)
	{
		//creates a new datatable
		DataTable sorted_data = new DataTable();

		//creates the columns for the information the new table holds
		sorted_data.Columns.Add("rep_id", typeof(string));
		sorted_data.Columns.Add("total_bal",typeof(double));

		foreach (DataRow row in data.Rows)
		{
			string repIdValue = row["rep_id"].ToString();
			//does a comparison between the repid values in sorteddata table and the repids from the main table
			//the matching rows are assigned tomaching rows meaning that rows need to be added and not created in the new data table
			DataRow[] matchingRows = sorted_data.Select("rep_id = '" + repIdValue + "'");

			
			if (matchingRows.Length == 0) // if the row in complete data is not found in the sorted data table rows then it would return a variable
				//of lenth zero meaning it needs to be added to the sorted table
			{
				// The current row's rep_id is not present in the sorted_data table
				// Create a new row with the same schema as sorted_data
				DataRow newRow = sorted_data.NewRow();
				newRow["rep_id"] = row["rep_id"];
				//newRow["total_bal"] = row["total_bal"];
				// Repeat for all columns in sorted_data...

				// Add the new row to sorted_data
				sorted_data.Rows.Add(newRow);
				//add the inital balance to the rep row

				newRow["total_bal"] = row["cust_balance"];
			}
			else
			{
				
				double value_to_add = Convert.ToDouble(row["cust_balance"]);
				foreach (DataRow RepRow in sorted_data.Rows)
				{
					
					string mainrow = Convert.ToString(row["rep_id"]);
					string grouprow = Convert.ToString(RepRow["rep_id"]);
					if (mainrow == grouprow) // im not understanding something about the object types of these two tables
					//if (row["rep_id"] == RepRow["rep_id"])
					{
						RepRow["total_bal"] = (double)RepRow["total_bal"] + value_to_add;
					}
				}
			}
		}

		DataRow[] rowstoremove = sorted_data.Select("total_bal < '12000'");// filters the data from the table that fits the argument
		//define the rows to remove outside of a loop otherwise it will raise and exception if data is removed mid loop
		//removes rep ids that do not fit the required balance
		foreach (DataRow row in rowstoremove)
		{
			sorted_data.Rows.Remove(row);
		}
		
		data = sorted_data; // sets the new data set to fit into the display function

		Console.Write(sorted_data.Columns);

		Console.Write(sorted_data) ;
		Console.WriteLine();
		Dictionary<string, int> colWidths = new Dictionary<string, int>();

		foreach (DataColumn col in data.Columns)
		{
			Console.Write(col.ColumnName);
			var maxLabelSize = data.Rows.OfType<DataRow>()
					.Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
					.OrderByDescending(m => m).FirstOrDefault();

			colWidths.Add(col.ColumnName, maxLabelSize);
			for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
		}

		Console.WriteLine();

		foreach (DataRow dataRow in data.Rows)
		{
			for (int j = 0; j < dataRow.ItemArray.Length; j++)
			{
				Console.Write(dataRow.ItemArray[j]);
				for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
			}
			Console.WriteLine();
		}

	}
	static void print_results(DataTable data)
	{
		Console.WriteLine();
		Dictionary<string, int> colWidths = new Dictionary<string, int>();

		foreach (DataColumn col in data.Columns)
		{
			Console.Write(col.ColumnName);
			var maxLabelSize = data.Rows.OfType<DataRow>()
					.Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
					.OrderByDescending(m => m).FirstOrDefault();

			colWidths.Add(col.ColumnName, maxLabelSize);
			for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
		}

		Console.WriteLine();

		foreach (DataRow dataRow in data.Rows)
		{
			for (int j = 0; j < dataRow.ItemArray.Length; j++)
			{
				Console.Write(dataRow.ItemArray[j]);
				for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
			}
			Console.WriteLine();
		}

	}
}

