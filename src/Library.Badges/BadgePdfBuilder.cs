using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

using MigraDoc.DocumentObjectModel;
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
			
			// Each badge area is split into 9 even rows and 13 even columns.
			this.rowHeight = this.badgeHeight / 9;
			this.columnWidth = this.badgeWidth / 13;
			
			this.fontNormal = new XFont("Verdana", 7, XFontStyle.Regular);
			this.fontItalic = new XFont("Verdana", 7, XFontStyle.Italic);
			
			this.centerLeft = new XStringFormat();
			this.centerLeft.LineAlignment = XLineAlignment.Center;
			this.centerLeft.Alignment = XStringAlignment.Near;
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
		private XFont fontItalic;
		
		private XStringFormat centerLeft;
		
		#endregion
		
		#region Properties
		
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
			var valueWidth = columnWidth * 5.5;
			var valueXPoint = contentRect.X + labelWidth;
			
			var nameLabel = new XRect(contentRect.X, contentRect.Y, labelWidth, rowHeight);
			this.graphics.DrawString("Name", this.fontItalic, XBrushes.Black, nameLabel, this.centerLeft);
			var name = new XRect(valueXPoint, contentRect.Y, valueWidth, rowHeight);
			this.graphics.DrawString(badge.FullName, this.fontNormal, XBrushes.Black, name, this.centerLeft);
			this.graphics.DrawLine(XPens.Gray, nameLabel.BottomLeft, name.BottomRight);
			
			var congLabel = new XRect(contentRect.X, nameLabel.Bottom, labelWidth, rowHeight);
			this.graphics.DrawString("Cong.", this.fontItalic, XBrushes.Black, congLabel, this.centerLeft);
			var cong = new XRect(valueXPoint, name.Bottom, valueWidth, rowHeight);
			this.graphics.DrawString(badge.CongregationName, this.fontNormal, XBrushes.Black, cong, this.centerLeft);
			this.graphics.DrawLine(XPens.Gray, congLabel.BottomLeft, cong.BottomRight);
			
			var deptLabel = new XRect(contentRect.X, congLabel.Bottom, labelWidth, rowHeight);
			this.graphics.DrawString("Dept.", this.fontItalic, XBrushes.Black, deptLabel, this.centerLeft);
			var dept = new XRect(valueXPoint, cong.Bottom, valueWidth, rowHeight);
			this.graphics.DrawString(badge.DepartmentName, this.fontNormal, XBrushes.Black, dept, this.centerLeft);
			this.graphics.DrawLine(XPens.Gray, deptLabel.BottomLeft, dept.BottomRight);
			
			// Logo
			var logoXPoint = valueXPoint + (columnWidth * 6);
			var imageRect = new XRect(logoXPoint, contentRect.Y + Unit.FromPoint(1), columnWidth * 4, rowHeight * 3);
			var badgeLogo = GetXImageFromResource("badge-logo");
			this.graphics.DrawImage(badgeLogo, imageRect);
			
			// Training
			var y = dept.Bottom;
			var col2X = contentRect.X + (columnWidth * 4);
			var col3X = contentRect.X + (columnWidth * 8);
			var halfRow = rowHeight / 2;
			
			var training = new XRect(contentRect.X, y + halfRow, columnWidth * 4, halfRow);
			this.DrawString("TRAINING", training, this.fontItalic);
			
			var access = new XRect(col3X, y + halfRow, columnWidth * 4, halfRow);
			this.DrawString("ACCESS", access, this.fontItalic);
			
			y += this.rowHeight;
			this.DrawCheckItem(contentRect.X, y, true, "Training 1");
			this.DrawCheckItem(col2X, y, false, "Training 2");
			this.DrawCheckItem(col3X, y, false, "Roof/Scaffold");
			
			y += this.rowHeight;
			this.DrawCheckItem(contentRect.X, y, false, "Training 3");
			this.DrawCheckItem(col2X, y, true, "Training 4");
			this.DrawCheckItem(col3X, y, true, "Site");
			
			y += this.rowHeight;
			this.DrawCheckItem(contentRect.X, y, true, "Training 4");
			this.DrawCheckItem(col2X, y, false, "Training 5");
			
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
			
			this.graphics.DrawString(text, font, XBrushes.Black, rect, this.centerLeft);
		}
		
		#endregion
		
		private static XImage GetXImageFromResource(string imageName)
		{
			var bitmap = GetBitmapFromResource(imageName);
			return XImage.FromGdiPlusImage(bitmap);
		}
		
		private static Bitmap GetBitmapFromResource(string imageName)
		{
			Bitmap bitmap = null;
			Assembly assem = Assembly.GetExecutingAssembly();
			var resourceName = "RbcTools.Library.Badges.Resources." + imageName + ".jpg";
			Stream stream = assem.GetManifestResourceStream(resourceName);
			bitmap = new Bitmap(stream);
			return bitmap;
		}
		
		private string CreateFileOnSystem()
		{
			var fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RBCTool\LatestBadges.pdf";
			this.pdfDocument.Save(fileName);
			return fileName;
		}
	}
}
