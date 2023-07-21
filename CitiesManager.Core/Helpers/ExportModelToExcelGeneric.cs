using OfficeOpenXml;
using OfficeOpenXml.Style;

public class ExportModelToExcelGeneric //CitiesManager.Core.Helpers
{
    /// <summary>
    /// 15.07.2023.
    /// </summary>
    public static void ExportToExcel<T>(IEnumerable<T> data, string? filePath, string? sheetName)
    {
        using (ExcelPackage package = new ExcelPackage())
        {
            //int maxRows = data.Count();

            //sheetName ??= "Sheet1";

            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

            int headerRow = 1;
            int headerColumn = 1;

            worksheet.Cells.AutoFitColumns();


            foreach (var property in typeof(T).GetProperties())
            {
                worksheet.Cells[headerRow, headerColumn].Value = property.Name;

                if (property.PropertyType == typeof(string))
                {
                    //var length = property.GetCustomAttributes(typeof(MaxLengthAttribute), true)
                    //    .OfType<MaxLengthAttribute>()
                    //    .FirstOrDefault()?.Length;

                    //if (length != null && length > 70)
                    //{
                    //    worksheet.Column(headerColumn).Width = 70;
                    //}

                    // bez provere duzine stringa, stavi na 70
                    worksheet.Column(headerColumn).Style.WrapText = true;
                    worksheet.Column(headerColumn).Width = 70;

                }
                if (property.PropertyType == typeof(Guid))
                {
                    worksheet.Column(headerColumn).Width = 40;
                }

                worksheet.Column(headerColumn).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Column(headerColumn).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                headerColumn++;
            }

            worksheet.Cells[1, 1, 1, headerColumn-1].AutoFilter = true;

            worksheet.Cells[1, 1, 1, headerColumn-1].Style.Fill.PatternType = ExcelFillStyle.Solid;

            worksheet.Cells[1, 1, 1, headerColumn-1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Bisque);

            worksheet.Cells[1, 1, 1, headerColumn - 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, headerColumn - 1].Style.Font.Italic = true;



            // Popunjavanje podacima
            int dataRow = 2;
            foreach (var model in data)
            {
                int dataColumn = 1;
                foreach (var property in typeof(T).GetProperties())
                {

                    var value = property.GetValue(model);

                    if (property.PropertyType == typeof(DateTime?))
                    {

                        worksheet.Cells[dataRow, dataColumn].Style.Numberformat.Format = "dd.MM.yyyy";

                        worksheet.Cells[dataRow, dataColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;

                        worksheet.Cells[dataRow, dataColumn].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AliceBlue);

                    }

                    worksheet.Cells[dataRow, dataColumn].Value = value;
                    dataColumn++;

                }

                dataRow++;

            }

            // Sačuvaj Excel datoteku
            package.SaveAs(filePath);
        }
       

    }
}
