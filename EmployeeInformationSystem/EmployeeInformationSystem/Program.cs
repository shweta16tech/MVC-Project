using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using EmployeeInformationSystem.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;
using FastReport.Export.PdfSimple;

namespace EmployeeInformationSystem
{
    class Program
    {
        static void Main()
        {
            // Sample data
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "Alice", Department = "HR", Salary = 50000 },
                new Employee { Id = 2, Name = "Bob", Department = "IT", Salary = 60000 },
                new Employee { Id = 3, Name = "Charlie", Department = "Finance", Salary = 55000 },
            };

            // Export reports
            ExportToPDF(employees, "EmployeeReport.pdf");
            ExportToExcel(employees, "EmployeeReport.xlsx");
            ExportToWord(employees, "EmployeeReport.docx");

            Console.WriteLine(" All reports exported successfully!");
        }

        // PDF export using FastReport
        static void ExportToPDF(List<Employee> employees, string pdfPath)
        {
            var report = new Report();
            string frxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "EmployeeReport.frx");

            if (!File.Exists(frxPath))
            {
                Console.WriteLine(" Error: EmployeeReport.frx not found at " + frxPath);
                return;
            }

            report.Load(frxPath);

            // Convert List<Employee> to DataTable
            DataTable dt = new DataTable("Employee");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Salary", typeof(double));

            foreach (var emp in employees)
                dt.Rows.Add(emp.Id, emp.Name, emp.Department, emp.Salary);

            // Register DataTable directly
            report.RegisterData(dt, "Employee");
            report.GetDataSource("Employee").Enabled = true;

            report.Prepare();
            report.Export(new PDFSimpleExport(), pdfPath);

            Console.WriteLine("PDF exported successfully!");
        }

        // Excel export using ClosedXML
        static void ExportToExcel(List<Employee> employees, string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Department";
            worksheet.Cell(1, 4).Value = "Salary";

            worksheet.Range(1, 1, 1, 4).Style.Font.Bold = true;

            for (int i = 0; i < employees.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = employees[i].Id;
                worksheet.Cell(i + 2, 2).Value = employees[i].Name;
                worksheet.Cell(i + 2, 3).Value = employees[i].Department;
                worksheet.Cell(i + 2, 4).Value = employees[i].Salary;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);

            Console.WriteLine("Excel exported successfully!");
        }

        // Word export using DocX
        static void ExportToWord(List<Employee> employees, string filePath)
        {
            // Create a Word document
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Add main document part
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Add title
                Paragraph title = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    ),
                    new Run(
                        new Text("Employee Report")
                    )
                );
                title.ParagraphProperties!.SpacingBetweenLines = new SpacingBetweenLines() { After = "200" };
                body.AppendChild(title);

                // Create table
                Table table = new Table();

                // Table properties (borders)
                TableProperties tblProps = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                    )
                );
                table.AppendChild(tblProps);

                // Add header row
                TableRow headerRow = new TableRow();
                string[] headers = { "Id", "Name", "Department", "Salary" };
                foreach (var h in headers)
                {
                    TableCell cell = new TableCell();
                    cell.Append(new Paragraph(new Run(new Text(h))) { ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center }) });
                    // Bold header
                    cell.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Fill = "D3D3D3" })); // light gray
                    headerRow.Append(cell);
                }
                table.Append(headerRow);

                // Add data rows
                foreach (var emp in employees)
                {
                    TableRow row = new TableRow();

                    TableCell cellId = new TableCell(new Paragraph(new Run(new Text(emp.Id.ToString()))));
                    TableCell cellName = new TableCell(new Paragraph(new Run(new Text(emp.Name))));
                    TableCell cellDept = new TableCell(new Paragraph(new Run(new Text(emp.Department))));
                    TableCell cellSalary = new TableCell(new Paragraph(new Run(new Text(emp.Salary.ToString()))));

                    row.Append(cellId, cellName, cellDept, cellSalary);
                    table.Append(row);
                }

                body.Append(table);
            }

            Console.WriteLine("Word exported successfully!");
        }
    }
}