
using System;
using System.Collections.Generic;
using System.Diagnostics;
using RbcTools.Library.Badges;

namespace TestConsole
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Creating a PDF");
			
			var badges = new List<Badge>();
			
			for (int i = 0; i < 4; i++)
			{
				badges.Add(new Badge(){ FirstName = "John", LastName = "Doe", CongregationName = "Cardigan", DepartmentName = "Decorating" });
				badges.Add(new Badge(){ FirstName = "Joe", LastName = "Bloggs", CongregationName = "Aberaeron", DepartmentName = "Air Handling" });
				badges.Add(new Badge(){ FirstName = "Jane", LastName = "Doe", CongregationName = "Cardigan", DepartmentName = "Electricians" });
				badges.Add(new Badge(){ FirstName = "Phil", LastName = "Baines", CongregationName = "Aberaeron", DepartmentName = "IT Department" });
				badges.Add(new Badge(){ FirstName = "Madelyn", LastName = "Baines", CongregationName = "Aberaeron", DepartmentName = "Volunteers" });
			}
			
			try
			{
				var builder = new BadgePdfBuilder(badges);
				var fileName = builder.CreatePdf();
				var process = Process.Start(fileName);
			}
			catch(Exception ex)
			{
				Console.WriteLine("ERROR");
				Console.WriteLine("");
				Console.WriteLine(ex.Message);
				Console.WriteLine("");
				Console.WriteLine(ex.StackTrace);
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}