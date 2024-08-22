using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace LangChain.DocumentLoaders;

/// <summary>
/// 
/// </summary>
public static class ExcelToMarkdown
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="firstRowIsHeader"></param>
    /// <returns></returns>
    public static IList<KeyValuePair<string, string>> Convert(
        Stream stream,
        bool firstRowIsHeader = true)
    {
        using var document = SpreadsheetDocument.Open(stream, isEditable: false);
        if (document.WorkbookPart == null)
        {
            return [];
        }

        var sharedStringTable = document.WorkbookPart.SharedStringTablePart?.SharedStringTable;

        var markdowns = new List<KeyValuePair<string, string>>();
        foreach (var sheet in document.WorkbookPart.Workbook.Sheets?.Elements<Sheet>() ?? [])
        {
            if (sheet.Id?.Value is null)
            {
                continue;
            }
            
            var isFirstRow = true;
            var builder = new StringBuilder();
            
            foreach (var row in (document.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart)?.Worksheet
                     .GetFirstChild<SheetData>()?
                     .Descendants<Row>() ?? [])
            {
                //Read the first row as header
                if (isFirstRow)
                {
                    isFirstRow = false;
                    builder.AppendLine("| " + string.Join(" | ", row
                        .Descendants<Cell>()
                        .Select((cell, i) => firstRowIsHeader ? GetCellValue(sharedStringTable, cell) : $"Field{i + 1}")
                        .ToList()) + " |");
                    builder.AppendLine("| " + string.Join(" | ", row.Select(_ => "-----------")) + " |");
                }
                else
                {
                    builder.AppendLine("| " + string.Join(" | ", row
                        .Descendants<Cell>()
                        .Select(cell => GetCellValue(sharedStringTable, cell))
                        .ToList()) + " |");
                }
            }
            
            markdowns.Add(new KeyValuePair<string, string>(
                sheet.Name?.Value  ?? $"Sheet{markdowns.Count}",
                builder.ToString()));
        }
        
        
        return markdowns;
    }
    
    private static string GetCellValue(SharedStringTable? table, Cell cell)
    {
        var value = cell.CellValue?.InnerText ?? string.Empty;
        if (table != null &&
            cell.DataType != null &&
            cell.DataType.Value == CellValues.SharedString &&
            int.TryParse(value, out var index))
        {
            return table.ChildElements[index].InnerText;
        }
        
        return value;
    }
}