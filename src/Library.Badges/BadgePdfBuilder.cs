using System;
using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
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
		
		private Document document;
		private List<Badge> allBadges;
		private Section pageSection;
		private Table pageTable;
		
		private int badgeCount = 0;
		
		#endregion
		
		public string CreatePdf()
		{
			// Create document, basic layout.
			document = new Document();
			document.DefaultPageSetup.TopMargin = Unit.FromInch(0.5);
			document.DefaultPageSetup.BottomMargin = Unit.FromInch(0.5);
			
			CreatePage();
			
			foreach(var badge in allBadges)
				this.CreateBadge(badge);
			
			return CreateFileOnSystem();
		}
		
		private void CreateBadge(Badge badge)
		{
			// Create padge part of the pdf
			Cell pageCell = pageTable.Rows[0].Cells[0];
			
			switch(badgeCount)
			{
				case 0:
					pageCell = pageTable.Rows[0].Cells[0];
					break;
				case 1:
					pageCell = pageTable.Rows[0].Cells[1];
					break;
				case 2:
					pageCell = pageTable.Rows[1].Cells[0];
					break;
				case 3:
					pageCell = pageTable.Rows[1].Cells[1];
					break;
				case 4:
					pageCell = pageTable.Rows[2].Cells[0];
					break;
				case 5:
					pageCell = pageTable.Rows[2].Cells[1];
					break;
				case 6:
					pageCell = pageTable.Rows[3].Cells[0];
					break;
				case 7:
					pageCell = pageTable.Rows[3].Cells[1];
					break;
				case 8:
					pageCell = pageTable.Rows[4].Cells[0];
					break;
				case 9:
					pageCell = pageTable.Rows[4].Cells[1];
					break;
			}
			
			var badgeTable = new Table();
			badgeTable.AddColumn().Width = 40;
			badgeTable.AddRow().Height = Unit.FromInch(2);
			pageCell.Elements.Add(badgeTable);
			
			
			var para = badgeTable.Rows[0].Cells[0].AddParagraph();
			para.AddText("JEHOVAH'S WITNESSES REGIONAL BUILDING TEAM");
			
			para = badgeTable.Rows[0].Cells[0].AddParagraph();
			para.AddText(badge.FirstName);
			
			para = badgeTable.Rows[0].Cells[0].AddParagraph();
			para.AddText(badge.LastName);
			
			para = badgeTable.Rows[0].Cells[0].AddParagraph();
			para.AddText(badge.CongregationName);
			
			para = badgeTable.Rows[0].Cells[0].AddParagraph();
			para.AddText(badge.DepartmentName);
			
			// Count badge index (out of 10) and reset if onto next lot of 10.
			badgeCount++;
			if(badgeCount == 10)
			{
				badgeCount = 0;
				CreatePage();
			}
		}
		
		private void CreatePage()
		{
			pageSection = document.AddSection();
			
			pageTable = pageSection.AddTable();
			
			pageTable.TopPadding =
				pageTable.RightPadding =
				pageTable.BottomPadding =
				pageTable.LeftPadding = 0;
			
			pageTable.Borders.Width = 1;
			
			pageTable.AddColumn();
			pageTable.AddColumn();
			
			pageTable.AddRow();
			pageTable.AddRow();
			pageTable.AddRow();
			pageTable.AddRow();
			pageTable.AddRow();
		}
		
		private string CreateFileOnSystem()
		{
			PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always);
			pdfRenderer.Document = document;
			pdfRenderer.RenderDocument();
			
			var pdfDocument = pdfRenderer.PdfDocument;
			var fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RBCTool\LatestBadges.pdf";
			pdfDocument.Save(fileName);
			
			return fileName;
		}
	}
}
