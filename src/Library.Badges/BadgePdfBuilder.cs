using System;
using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Reflection;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace RbcTools.Library.Badges
{
	public class BadgePdfBuilder
	{
		#region Constructor
		
		public BadgePdfBuilder(List<Badge> badges)
		{
			this.allBadges = badges;
		}
		
		#endregion
		
		#region Fields
		
		private List<Badge> allBadges;
		private List<XRect> allRects = new List<XRect>();
		
		private PdfDocument pdfDocument;
		private PdfPage page;
		private XGraphics graphics;
		
		private int badgeCount = 0;
		private float badgeWidth = Unit.FromInch(3.3);
		private float badgeHeight = Unit.FromInch(2.1);
		
		private XFont fontNormal = new XFont("Verdana", 7, XFontStyle.Regular);
		
		#endregion
		
		#region Properties
		
		private XStringFormat CenterLeft
		{
			get
			{
				XStringFormat centerLeft = new XStringFormat();
				centerLeft.LineAlignment = XLineAlignment.Center;
				centerLeft.Alignment = XStringAlignment.Near;
				return centerLeft;
			}
		}
		
		#endregion
		
		public string CreatePdf()
		{
			this.pdfDocument = new PdfDocument();
			this.pdfDocument.Info.Title = "RBC Badges";
			
			foreach(var badge in allBadges)
			{
				if(badgeCount == 0)
					this.CreatePage();
				
				this.CreateBadge(badge);
				
				badgeCount++;
				if(badgeCount == 10) badgeCount = 0;
			}
			
			return CreateFileOnSystem();
		}
		
		private void CreateBadge(Badge badge)
		{
			// Get matching rectangle for this badge.
			XRect rectBadge = this.allRects[this.allBadges.IndexOf(badge)];
			this.graphics.DrawRectangle(XPens.LightGray, rectBadge);
			
			// Split badge area into 8 even rows and 13 even columns.
			var rowHeight = rectBadge.Height / 8;
			var columnWidth = rectBadge.Width / 13;
			
			// Top row (full width)
			var topRow = new XRect(rectBadge.X, rectBadge.Y, rectBadge.Width, rowHeight);
			this.graphics.DrawRectangle(XBrushes.DarkBlue, topRow);
			this.graphics.DrawString("Jehovah's Witnesses Regional Building Team".ToUpper(), this.fontNormal, XBrushes.White, topRow, XStringFormats.Center);
			
			// Define a 'content' rectangle that has half a column either side (leaving 12 columns to use).
			var contentRect = new XRect(rectBadge.X + (columnWidth / 2), topRow.Bottom, rectBadge.Width - columnWidth, rectBadge.Height - topRow.Height);
			
			// Name, Congregation and Department labels
			var labelWidth = columnWidth * 2;
			var valueWidth = columnWidth * 5;
			var valueXPoint = contentRect.X + labelWidth;
			
			var nameLabel = new XRect(contentRect.X, contentRect.Y, labelWidth, rowHeight);
			this.graphics.DrawString("Name", this.fontNormal, XBrushes.Black, nameLabel, this.CenterLeft);
			var name = new XRect(valueXPoint, contentRect.Y, valueWidth, rowHeight);
			this.graphics.DrawString(badge.FullName, this.fontNormal, XBrushes.Black, name, this.CenterLeft);
			this.graphics.DrawLine(XPens.Gray, nameLabel.BottomLeft, name.BottomRight);
			
			var congLabel = new XRect(contentRect.X, nameLabel.Bottom, labelWidth, rowHeight);
			this.graphics.DrawString("Cong.", this.fontNormal, XBrushes.Black, congLabel, this.CenterLeft);
			var cong = new XRect(valueXPoint, name.Bottom, valueWidth, rowHeight);
			this.graphics.DrawString(badge.CongregationName, this.fontNormal, XBrushes.Black, cong, this.CenterLeft);
			this.graphics.DrawLine(XPens.Gray, congLabel.BottomLeft, cong.BottomRight);
			
			var deptLabel = new XRect(contentRect.X, congLabel.Bottom, labelWidth, rowHeight);
			this.graphics.DrawString("Dept.", this.fontNormal, XBrushes.Black, deptLabel, this.CenterLeft);
			var dept = new XRect(valueXPoint, cong.Bottom, valueWidth, rowHeight);
			this.graphics.DrawString(badge.DepartmentName, this.fontNormal, XBrushes.Black, dept, this.CenterLeft);
			this.graphics.DrawLine(XPens.Gray, deptLabel.BottomLeft, dept.BottomRight);
			
			// Logo
			var logoXPoint = valueXPoint + valueWidth + columnWidth;
			var imageRect = new XRect(logoXPoint, contentRect.Y, columnWidth * 4, rowHeight * 3);
			this.graphics.DrawRectangle(XBrushes.Gray, imageRect);
			// TODO Replace with RBC logo
			//this.graphics.DrawImage(GetImageFromResource("badge-logo"), imageRect);
			
			// Training
			var training1 = new XRect(contentRect.X, deptLabel.Bottom, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 1", this.fontNormal, XBrushes.Black, training1, this.CenterLeft);
			
			var training2 = new XRect(contentRect.X, training1.Bottom, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 2", this.fontNormal, XBrushes.Black, training2, this.CenterLeft);
			
			var training3 = new XRect(contentRect.X, training2.Bottom, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 3", this.fontNormal, XBrushes.Black, training3, this.CenterLeft);
			
			var training4 = new XRect(contentRect.X, training3.Bottom, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 4", this.fontNormal, XBrushes.Black, training4, this.CenterLeft);
			
			var trainingCol2X = contentRect.X + training1.Width;
			
			var training5 = new XRect(trainingCol2X, training1.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 5", this.fontNormal, XBrushes.Black, training5, this.CenterLeft);
			
			var training6 = new XRect(trainingCol2X, training2.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 6", this.fontNormal, XBrushes.Black, training6, this.CenterLeft);
			
			var training7 = new XRect(trainingCol2X, training3.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 7", this.fontNormal, XBrushes.Black, training7, this.CenterLeft);
			
			var training8 = new XRect(trainingCol2X, training4.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Training 8", this.fontNormal, XBrushes.Black, training8, this.CenterLeft);
			
			var trainingCol3X = trainingCol2X + training5.Width;
			
			var access1 = new XRect(trainingCol3X, training1.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Access", this.fontNormal, XBrushes.Black, access1, this.CenterLeft);
			
			var access2 = new XRect(trainingCol3X, training2.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Roof/Scaffold", this.fontNormal, XBrushes.Black, access2, this.CenterLeft);
			
			var access3 = new XRect(trainingCol3X, training3.Top, columnWidth * 4, rowHeight);
			this.graphics.DrawString("Site", this.fontNormal, XBrushes.Black, access3, this.CenterLeft);
		}
		
		private void CreatePage()
		{
			// Create new page object and get graphics object for drawing on to.
			
			this.page = this.pdfDocument.AddPage();
			this.page.Size = PageSize.A4;
			this.graphics = XGraphics.FromPdfPage(this.page);
			
			// Create all the badge rectangles to draw into later
			
			var badgesAreaHeight = (XUnit)(this.badgeHeight * 5);
			var badgesAreaWidth  = (XUnit)(this.badgeWidth * 2);
			
			var distanceFromTop = (XUnit)((this.page.Height - badgesAreaHeight) / 2);
			var distanceFromLeft = (XUnit)((this.page.Width - badgesAreaWidth) / 2);
			
			for (int rectIndex = 0; rectIndex < 5; rectIndex++)
			{
				var badgeRectLeft = new XRect(distanceFromLeft, distanceFromTop, badgeWidth, badgeHeight);
				var badgeRectRight = new XRect(distanceFromLeft + this.badgeWidth, distanceFromTop, badgeWidth, badgeHeight);
				this.allRects.Add(badgeRectLeft);
				this.allRects.Add(badgeRectRight);
				distanceFromTop = distanceFromTop + badgeHeight;
			}
		}
		
//		private static XImage GetImageFromResource(string imageName)
//		{
//			Bitmap bitmap = null;
//			Assembly assem = Assembly.GetExecutingAssembly();
//			var resourceName = "RbcTools.Library.Badges.Resources." + imageName + ".jpg";
//			using(Stream stream = assem.GetManifestResourceStream(resourceName))
//			{
//				bitmap = new Bitmap(stream);
//			}
//			return XImage.FromGdiPlusImage(bitmap);
//		}
		
		private string CreateFileOnSystem()
		{
			var fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RBCTool\LatestBadges.pdf";
			this.pdfDocument.Save(fileName);
			return fileName;
		}
	}
}
