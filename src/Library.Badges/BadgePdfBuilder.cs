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
			
			this.allRects = new List<XRect>();
			
			this.badgeCount = 0;
			
			this.badgeWidth = Unit.FromInch(3.3);
			this.badgeHeight = Unit.FromInch(2.1);
			
			// Each badge area is split into 8 even rows and 13 even columns.
			this.rowHeight = this.badgeHeight / 8;
			this.columnWidth = this.badgeWidth / 13;
			
			this.fontNormal = new XFont("Verdana", 7, XFontStyle.Regular);
		}
		
		#endregion
		
		#region Fields
		
		private List<Badge> allBadges;
		private List<XRect> allRects;
		
		private PdfDocument pdfDocument;
		private PdfPage page;
		private XGraphics graphics;
		
		private int badgeCount;
		private float badgeWidth;
		private float badgeHeight;
		
		private float rowHeight;
		private float columnWidth;
		
		private XFont fontNormal;
		
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
			var y = dept.Bottom;
			var col2X = contentRect.X + (columnWidth * 4);
			var col3X = contentRect.X + (columnWidth * 8);
			
			this.DrawCheckItem(contentRect.X, y, true, "Training 1");
			this.DrawCheckItem(col2X, y, false, "Training 2");
			
			var access = new XRect(col3X, y, columnWidth * 4, rowHeight);
			this.DrawString("ACCESS", access, this.fontNormal);
			
			y += this.rowHeight;
			this.DrawCheckItem(contentRect.X, y, false, "Training 3");
			this.DrawCheckItem(col2X, y, true, "Training 4");
			this.DrawCheckItem(col3X, y, false, "Roof/Scaffold");
			
			y += this.rowHeight;
			this.DrawCheckItem(contentRect.X, y, true, "Training 4");
			this.DrawCheckItem(col2X, y, false, "Training 5");
			this.DrawCheckItem(col3X, y, true, "Site");
			
			y += this.rowHeight;
			this.DrawCheckItem(contentRect.X, y, false, "Training 7");
			this.DrawCheckItem(col2X, y, true, "Training 8");
			
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
		
		#region Drawing Methods
		
		private void DrawCheckItem(double x, double y, bool isChecked, string text)
		{
			var checkBoxWidth = this.columnWidth / 2;
			var textXPoint = x + checkBoxWidth;
			
			var checkRect = new XRect(x, y, checkBoxWidth, rowHeight);
			var textRect = new XRect(textXPoint, y, columnWidth * 3.5, rowHeight);
			
			var fontWingdings = new XFont("Wingdings 2", 9, XFontStyle.Bold);
			
			this.DrawString(isChecked ? "R" : "£", checkRect, fontWingdings);
			this.DrawString(text, textRect);
		}
		
		private void DrawString(string text, XRect rect, XFont font = null)
		{
			if(font == null)
				font = this.fontNormal;
			
			this.graphics.DrawString(text, font, XBrushes.Black, rect, this.CenterLeft);
		}
		
		#endregion
		
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
