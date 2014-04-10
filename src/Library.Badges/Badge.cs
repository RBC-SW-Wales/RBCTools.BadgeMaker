
using System;

namespace RbcTools.Library.Badges
{
	public class Badge
	{
		public Badge(){}
		
		public string FirstName { get; set; }
		
		public string LastName { get; set; }
		
		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
		
		public string CongregationName { get; set; }
		
		public string DepartmentName { get; set; }
	}
}
