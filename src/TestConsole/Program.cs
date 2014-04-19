
using System;
using System.Collections.Generic;
using System.Diagnostics;

using RbcTools.Library.Badges;
using RbcTools.Library.Database;

namespace TestConsole
{
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				// Program.CreatePdf();
				Program.ListDepartments();
			}
			catch(Exception ex)
			{
				Console.WriteLine("ERROR");
				Console.WriteLine("");
				Console.WriteLine(ex.Message);
				Console.WriteLine("");
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("");
				Console.Write("Press any key to close . . . ");
				Console.ReadKey(true);
			}
		}
		
		public static void CreatePdf()
		{
			Console.WriteLine("Creating a PDF");
			
			var badges = new List<Badge>();
			
			for (int i = 0; i < 4; i++)
			{
				var john = new Badge(){ FirstName = "John", LastName = "Doe", CongregationName = "Cardigan", DepartmentName = "Decorating" };
				john.HasSiteAccess = true;
				john.HasDrillsTraining = true;
				john.HasJigsawsTraining = true;
				badges.Add(john);
				
				var joe = new Badge(){ FirstName = "Joe", LastName = "Bloggs", CongregationName = "Aberaeron", DepartmentName = "Air Handling" };
				joe.HasSiteAccess = true;
				joe.HasRoofAndScaffoldAccess = true;
				joe.HasDrillsTraining = true;
				joe.HasNailersTraining = true;
				badges.Add(joe);
				
				var jane = new Badge(){ FirstName = "Jane", LastName = "Doe", CongregationName = "Cardigan", DepartmentName = "Electricians" };
				jane.HasSiteAccess = true;
				jane.HasDrillsTraining = true;
				jane.HasPlanersTraing = true;
				badges.Add(jane);
				
				var phil = new Badge(){ FirstName = "Phil", LastName = "Baines", CongregationName = "Aberaeron", DepartmentName = "IT Department" };
				phil.HasSiteAccess = true;
				phil.HasRoofAndScaffoldAccess = true;
				phil.HasDrillsTraining = true;
				phil.HasJigsawsTraining = true;
				badges.Add(phil);
				
				var madelyn = new Badge(){ FirstName = "Madelyn", LastName = "Baines", CongregationName = "Aberaeron", DepartmentName = "Volunteers" };
				madelyn.HasSiteAccess = true;
				madelyn.HasRoofAndScaffoldAccess = true;
				madelyn.HasDrillsTraining = true;
				madelyn.HasJigsawsTraining = true;
				badges.Add(madelyn);
			}
			
			var builder = new BadgePdfBuilder(badges);
			//builder.UseLocalVolunteerDesign = true;
			var fileName = builder.CreatePdf();
			var process = Process.Start(fileName);
		}
		
		public static void ListDepartments()
		{
			Console.WriteLine("List Departments");
			
			var departments = RbcTools.Library.Database.Departments.GetDepartments();
			foreach(var department in departments)
			{
				Console.WriteLine(string.Format("{0, 4} : {1}", department.ID, department.Name));
				var volunteers = Volunteers.GetByDepartment(department);
				foreach(var vol in volunteers)
				{
					Console.WriteLine(" ---- " + vol.FirstName);
				}
			}
			
			Console.Write("Press any key to close . . . ");
			Console.ReadKey(true);
		}
	}
}