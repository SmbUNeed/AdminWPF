using AdminWPF.Shared;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Windows;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace AdminWPF.Shared;

public static class Exporter
{
    public static void Save(string format, TableData table, string title)
    {
        var dialog = new SaveFileDialog { FileName = title, DefaultExt = format, Filter = $"*.{format}|*.{format}" };
        if (dialog.ShowDialog() != true) return;

        try
        {
            switch (format)
            {
                case "xlsx": Xlsx(dialog.FileName, table, title); break;
                case "docx": Docx(dialog.FileName, table, title); break;
                case "pdf": Pdf(dialog.FileName, table, title); break;
            }
            MessageBox.Show("Файл сохранён", "Экспорт");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка экспорта");
        }
    }

    static void Xlsx(string path, TableData t, string title)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet(title.Length > 31 ? title[..31] : title);
        for (var c = 0; c < t.Columns.Length; c++)
            ws.Cell(1, c + 1).Value = t.Columns[c];
        for (var r = 0; r < t.Rows.Count; r++)
            for (var c = 0; c < t.Columns.Length; c++)
                ws.Cell(r + 2, c + 1).Value = t.Rows[r][c];
        ws.Row(1).Style.Font.Bold = true;
        ws.Columns().AdjustToContents();
        wb.SaveAs(path);
    }

    static void Docx(string path, TableData t, string title)
    {
        using var doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
        var main = doc.AddMainDocumentPart();
        main.Document = new W.Document(new W.Body());
        var body = main.Document.Body!;

        body.Append(new W.Paragraph(new W.Run(new W.RunProperties(new W.Bold()), new W.Text(title))));

        var borders = new W.TableBorders(
            new W.TopBorder { Val = W.BorderValues.Single, Size = 4 },
            new W.BottomBorder { Val = W.BorderValues.Single, Size = 4 },
            new W.LeftBorder { Val = W.BorderValues.Single, Size = 4 },
            new W.RightBorder { Val = W.BorderValues.Single, Size = 4 },
            new W.InsideHorizontalBorder { Val = W.BorderValues.Single, Size = 4 },
            new W.InsideVerticalBorder { Val = W.BorderValues.Single, Size = 4 });
        var table = new W.Table(new W.TableProperties(borders));

        table.Append(Row(t.Columns, true));
        foreach (var row in t.Rows) table.Append(Row(row, false));

        body.Append(table);
    }

    static W.TableRow Row(string[] cells, bool bold)
    {
        var row = new W.TableRow();
        foreach (var text in cells)
        {
            var run = new W.Run(new W.Text(text) { Space = SpaceProcessingModeValues.Preserve });
            if (bold) run.PrependChild(new W.RunProperties(new W.Bold()));
            row.Append(new W.TableCell(new W.Paragraph(run)));
        }
        return row;
    }

    static void Pdf(string path, TableData t, string title)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.UseSystemFonts = true;
        Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9));
            page.Header().Text(title).FontSize(14).Bold();
            page.Content().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(cd =>
                {
                    foreach (var _ in t.Columns) cd.RelativeColumn();
                });
                table.Header(h =>
                {
                    foreach (var col in t.Columns)
                        h.Cell().Background("#EEEEEE").Padding(4).Text(col).Bold();
                });
                foreach (var row in t.Rows)
                    foreach (var cell in row)
                        table.Cell().BorderBottom(0.5f).Padding(4).Text(cell);
            });
        })).GeneratePdf(path);
    }
}