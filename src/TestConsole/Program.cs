
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
				Program.CreatePdf();
				//Program.ListDepartments();
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
			
			var department = Departments.GetById(13); // IT Department = 13
			
			// Get ALL badges for a deparment.
			var badges = Volunteers.GetBadgesByDepartment(department);
			
			// Use BadgePdfBuilder to create badges PDF
			var builder = new BadgePdfBuilder(badges);
			// Create the file and return filename
			var fileName = builder.CreatePdf();
			
			// Open the file.
			var process = Process.Start(fileName);
		}
		
		public static void ListDepartments()
		{
			Console.WriteLine("List Departments");
			
			var departments = RbcTools.Library.Database.Departments.GetDepartments();
			foreach(var department in departments)
			{
				Console.WriteLine(string.Format("{0, 4} : {1}", department.ID, department.Name));
			}
			
			Console.Write("Press any key to close . . . ");
			Console.ReadKey(true);
		}
	}
}