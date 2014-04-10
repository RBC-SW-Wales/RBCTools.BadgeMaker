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
			
			var topRect = new XRect(rectBadge.X, rectBadge.Y, rectBadge.Width, XUnit.FromCentimeter(0.5));
			this.graphics.DrawRectangle(XBrushes.DarkBlue, topRect);
			
			this.graphics.DrawString("Jehovah's Witnesses Regional Building Team".ToUpper(), this.fontSmall, XBrushes.White, topRect, XStringFormats.Center);
			this.graphics.DrawString(badge.FullName, this.fontNormal, XBrushes.Black, rectBadge, XStringFormats.Center);
			
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
