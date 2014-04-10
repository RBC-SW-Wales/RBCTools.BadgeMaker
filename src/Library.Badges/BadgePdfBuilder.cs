using System;
using System.Collections.Generic;
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
		
		private XFont fontSmall = new XFont("Verdana", 5, XFontStyle.Regular);
		private XFont fontNormal = new XFont("Verdana", 7, XFontStyle.Regular);
		private XFont fontBold = new XFont("Verdana", 7, XFontStyle.Bold);
		
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
				this.CreateBadge(badge);
			
			return CreateFileOnSystem();
		}
		
		private void CreateBadge(Badge badge)
		{
			if(badgeCount == 0)
				this.CreatePage();
			
			//Get matching rect for this badge.
			XRect rectBadge = this.allRects[this.allBadges.IndexOf(badge)];
			
			var lineHeight = XUnit.FromCentimeter(0.5);
			
			// Top rect
			var topRect = new XRect(rectBadge.X, rectBadge.Y, rectBadge.Width, lineHeight);
			this.graphics.DrawRectangle(XBrushes.DarkBlue, topRect);
			this.graphics.DrawString("Jehovah's Witnesses Regional Building Team".ToUpper(), this.fontSmall, XBrushes.White, topRect, XStringFormats.Center);
			
			var padding = Unit.FromCentimeter(0.25);
			
			var contentRect = new XRect(rectBadge.X + padding, topRect.Bottom, rectBadge.Width - (padding * 2), rectBadge.Height - topRect.Height - padding);
			// this.graphics.DrawRectangle(XBrushes.LightBlue, contentRect);
			
			var gridWidth = contentRect.Width / 8;
			var column1Width = gridWidth * 1;
			var column2Width = gridWidth * 4;
			var column3Width = gridWidth * 3;
			
			// Name, Congregation and Department labels
			var nameLabel = new XRect(contentRect.X, contentRect.Y, column1Width, lineHeight);
			this.graphics.DrawString("Name", this.fontSmall, XBrushes.Black, nameLabel, this.CenterLeft);
			
			var congLabelRect = new XRect(contentRect.X, nameLabel.Bottom, column1Width, lineHeight);
			this.graphics.DrawString("Cong.", this.fontSmall, XBrushes.Black, congLabelRect, this.CenterLeft);
			
			var deptLabelRect = new XRect(contentRect.X, congLabelRect.Bottom, column1Width, lineHeight);
			this.graphics.DrawString("Dept.", this.fontSmall, XBrushes.Black, deptLabelRect, this.CenterLeft);
			
			var nameRect = new XRect(nameLabel.Right, contentRect.Y, column2Width, lineHeight);
			this.graphics.DrawString(badge.FullName, this.fontBold, XBrushes.Black, nameRect, this.CenterLeft);
			
			var congRect = new XRect(congLabelRect.Right, nameRect.Bottom, column2Width, lineHeight);
			this.graphics.DrawString(badge.CongregationName, this.fontNormal, XBrushes.Black, congRect, this.CenterLeft);
			
			var deptRect = new XRect(deptLabelRect.Right, congRect.Bottom, column2Width, lineHeight);
			this.graphics.DrawString(badge.DepartmentName, this.fontNormal, XBrushes.Black, deptRect, this.CenterLeft);
			
			badgeCount++;
			if(badgeCount == 10) badgeCount = 0;
		}
		
		private void CreatePage()
		{
			this.page = this.pdfDocument.AddPage();
			this.page.Size = PageSize.A4;
			this.graphics = XGraphics.FromPdfPage(this.page);
			
			var outerHeight = (XUnit)(badgeHeight * 5);
			var outerWidth  = (XUnit)(badgeWidth * 2);
			
			var topMargin = (XUnit)((this.page.Height - outerHeight) / 2);
			var leftMargin = (XUnit)((this.page.Width - outerWidth) / 2);
			
			// var outerRect = new XRect(leftMargin, topMargin, outerWidth, outerHeight);
			// this.graphics.DrawRectangle(new XPen(XColors.Aqua), outerRect);
			
			var startx = leftMargin;
			var starty = topMargin;
			
			for (int rectIndex = 0; rectIndex < 5; rectIndex++)
			{
				var rectLeft = new XRect(startx, starty, badgeWidth, badgeHeight);
				this.graphics.DrawRectangle(new XPen(XColors.LightGray), rectLeft);
				
				var rectRight = new XRect(startx + badgeWidth, starty, badgeWidth, badgeHeight);
				this.graphics.DrawRectangle(new XPen(XColors.LightGray), rectRight);
				
				this.allRects.Add(rectLeft);
				this.allRects.Add(rectRight);
				
				starty = starty + badgeHeight;
			}
		}
		
		private string CreateFileOnSystem()
		{
			var fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RBCTool\LatestBadges.pdf";
			this.pdfDocument.Save(fileName);
			return fileName;
		}
	}
}
