#define INCLUDE_WEB_FUNCTIONS

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace ExportToExcel
{
    //
    //  February 2015
    //  http://www.mikesknowledgebase.com
    //
    //  Note: if you plan to use this in an ASP.Net application, remember to add a reference to "System.Web", and to uncomment
    //  the "INCLUDE_WEB_FUNCTIONS" definition at the top of this file.
    //
    //  Release history
    //  -  Feb 2015: 
    //        Needed to replace "Response.End();" with some other code, to make sure the Excel was fully written to the HTTP Response
    //        New ReplaceHexadecimalSymbols() function to prevent hex characters from crashing the export. 
    //        Changed GetExcelColumnName() to cope with more than 702 columns (!)
    //   - Jan 2015: 
    //        Throwing an exception when trying to export a DateTime containing null.
    //        Was missing the function declaration for "CreateExcelDocument(DataSet ds, string filename, System.Web.HttpResponse Response)"
    //        Removed the "Response.End();" from the web version, as recommended in: https://support.microsoft.com/kb/312629/EN-US/?wa=wsignin1.0
    //   - Mar 2014: 
    //        Now writes the Excel data using the OpenXmlWriter classes, which are much more memory efficient.
    //   - Nov 2013: 
    //        Changed "CreateExcelDocument(DataTable dt, string xlsxFilePath)" to remove the DataTable from the DataSet after creating the Excel file.
    //        You can now create an Excel file via a Stream (making it more ASP.Net friendly)
    //   - Jan 2013: Fix: Couldn't open .xlsx files using OLEDB  (was missing "WorkbookStylesPart" part)
    //   - Nov 2012: 
    //        List<>s with Nullable columns weren't be handled properly.
    //        If a value in a numeric column doesn't have any data, don't write anything to the Excel file (previously, it'd write a '0')
    //   - Jul 2012: Fix: Some worksheets weren't exporting their numeric data properly, causing "Excel found unreadable content in '___.xslx'" errors.
    //   - Mar 2012: Fixed issue, where Microsoft.ACE.OLEDB.12.0 wasn't able to connect to the Excel files created using this class.
    //
    //
    //   (c) www.mikesknowledgebase.com 2014 
    //   
    //   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
    //   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
    //   publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
    //   subject to the following conditions:
    //   
    //   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    //   
    //   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    //   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    //   FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    //   WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    //   
    public class CreateExcelFile
    {
        const int DATE_FORMAT_ID = 1;

        private readonly IHttpContextAccessor _accessor;

        public CreateExcelFile(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public static bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(ListToDataTable(list));

            return CreateExcelDocument(ds, xlsxFilePath);
        }
        #region HELPER_FUNCTIONS
        //  This function is adapated from: http://www.codeguru.com/forum/showthread.php?t=450171
        //  My thanks to Carl Quirion, for making it "nullable-friendly".
        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                DisplayNameAttribute attr = (DisplayNameAttribute)Attribute.GetCustomAttribute(info, typeof(DisplayNameAttribute));
                string nome = "";

                if (attr != null)
                    nome = attr.DisplayName;
                else
                    nome = info.Name;

                dt.Columns.Add(new DataColumn(nome, GetNullableType(info.PropertyType)));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {

                    DisplayNameAttribute attr = (DisplayNameAttribute)Attribute.GetCustomAttribute(info, typeof(DisplayNameAttribute));
                    string nome = "";

                    if (attr != null)
                        nome = attr.DisplayName;
                    else
                        nome = info.Name;

                    if (!IsNullableType(info.PropertyType))
                        row[nome] = info.GetValue(t, null);
                    else
                        row[nome] = (info.GetValue(t, null) ?? DBNull.Value);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }


        private static Type GetNullableType(Type t)
        {
            Type returnType = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                returnType = Nullable.GetUnderlyingType(t);
            }
            return returnType;
        }


        private static bool IsNullableType(Type type)
        {
            return (type == typeof(string) ||
                    type.IsArray ||
                    (type.IsGenericType &&
                     type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }

        public static bool CreateExcelDocument(DataTable dt, string xlsxFilePath)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            bool result = CreateExcelDocument(ds, xlsxFilePath);
            ds.Tables.Remove(dt);
            return result;
        }
        #endregion

#if INCLUDE_WEB_FUNCTIONS
        /// <summary>
        /// Create an Excel file, and write it out to a MemoryStream (rather than directly to a file)
        /// </summary>
        /// <param name="dt">DataTable containing the data to be written to the Excel.</param>
        /// <param name="filename">The filename (without a path) to call the new Excel file.</param>
        /// <param name="Response">HttpResponse of the current page.</param>
        /// <returns>True if it was created succesfully, otherwise false.</returns>
        public static bool CreateExcelDocument(DataSet ds, string filename, HttpResponse Response)
        {
            try
            {
                CreateExcelDocumentAsStream(ds, filename, Response);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        public static bool CreateExcelDocument(DataTable dt, string filename, HttpResponse Response)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                CreateExcelDocument(ds, filename, Response);
                ds.Tables.Remove(dt);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        public static bool CreateExcelDocument<T>(List<T> list, string filename, HttpResponse Response)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(ListToDataTable(list));
                CreateExcelDocumentAsStream(ds, filename, Response);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Create an Excel file, and write it out to a MemoryStream (rather than directly to a file)
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="filename">The filename (without a path) to call the new Excel file.</param>
        /// <param name="Response">HttpResponse of the current page.</param>
        /// <returns>Either a MemoryStream, or NULL if something goes wrong.</returns>
        public static bool CreateExcelDocumentAsStream(DataSet ds, string filename, HttpResponse Response)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
                {
                    WriteExcelFile(ds, document);
                }
                stream.Flush();
                stream.Position = 0;

                //Response.ClearContent();
                Response.Clear();
                //Response.Buffer = true;
                //Response.Charset = "";

                //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
                //  manually added System.Web to this project's References.

                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Headers.Add("attachment; filename=", filename;       //Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //byte[] data1 = new byte[stream.Length];
                //stream.Read(data1, 0, data1.Length);
                //stream.Close();
                //Response.BinaryWrite(data1);
                //Response.Body.Flush();

                ////  Feb2015: Needed to replace "Response.End();" with the following 3 lines, to make sure the Excel was fully written to the Response
                //Response.Body.Flush();
                //Response.Body.SuppressContent = true;
                //Response.Headers.ApplicationInstance.CompleteRequest();

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }
#endif      //  End of "INCLUDE_WEB_FUNCTIONS" section

        /// <summary>
        /// Create an Excel file, and write it to a file.
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="excelFilename">Name of file to be written.</param>
        /// <returns>True if successful, false if something went wrong.</returns>
        public static bool CreateExcelDocument(DataSet ds, string excelFilename)
        {
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
                {
                    WriteExcelFile(ds, document);
                }
                Trace.WriteLine("Successfully created: " + excelFilename);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }


        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet)
        {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

            //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            //WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

            AddStyleSheet(spreadsheet);


            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            Sheets sheets = spreadsheet.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            foreach (DataTable dt in ds.Tables)
            {

                //  For each worksheet you want to create
                string worksheetName = dt.TableName;

                //  Create worksheet part, and add it to the sheets collection in workbook
                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();

                Sheet sheet = new Sheet() { Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart), SheetId = worksheetNumber, Name = worksheetName };
                sheets.Append(sheet);

                // If you want to define the Column Widths for a Worksheet, you need to do this *before* appending the SheetData
                // http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/1d93eca8-2949-4d12-8dd9-15cc24128b10/

               

                //  Append this worksheet's data to our Workbook, using OpenXmlWriter, to prevent memory problems
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart);

                worksheetNumber++;
            }

            spreadsheet.WorkbookPart.Workbook.Save();
        }


        private static WorkbookStylesPart AddStyleSheet(SpreadsheetDocument spreadsheet)
        {
            WorkbookStylesPart stylesheet = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();



            // Finalize
            stylesheet.Stylesheet = CreateStylesheet();
            stylesheet.Stylesheet.Save();

            return stylesheet;
        }


        private static Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet()
            {
                MCAttributes =
                    new MarkupCompatibilityAttributes() { Ignorable = "x14ac" }
            };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts()
            {
                Count = (UInt32Value)1U,
                KnownFonts
                    = true
            };
            //Normal Font
            DocumentFormat.OpenXml.Spreadsheet.Font font1 =
            new DocumentFormat.OpenXml.Spreadsheet.Font();
            DocumentFormat.OpenXml.Spreadsheet.FontSize fontSize1 =
            new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 9D };
            DocumentFormat.OpenXml.Spreadsheet.Color color1 =
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering1 =
            new FontFamilyNumbering() { Val = 1 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);
            fonts1.Append(font1);

            //Bold Font
            DocumentFormat.OpenXml.Spreadsheet.Font bFont =
            new DocumentFormat.OpenXml.Spreadsheet.Font();
            DocumentFormat.OpenXml.Spreadsheet.FontSize bfontSize =
            new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 9D };
            DocumentFormat.OpenXml.Spreadsheet.Color bcolor =
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Theme = (UInt32Value)0U };
            FontName bfontName = new FontName() { Val = "Arial" };
            FontFamilyNumbering bfontFamilyNumbering =
            new FontFamilyNumbering() { Val = 2 };
            //FontScheme bfontScheme = new FontScheme() { Val = FontSchemeValues.Minor };
            Bold bFontBold = new Bold();

            bFont.Append(bfontSize);
            bFont.Append(bcolor);
            bFont.Append(bfontName);
            bFont.Append(bfontFamilyNumbering);
            //bFont.Append(bfontScheme);
            bFont.Append(bFontBold);

            fonts1.Append(bFont);

            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);
            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor0 = new ForegroundColor() { Rgb = "FF0000" };
            BackgroundColor backgroundColor0 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill2.Append(foregroundColor0);
            patternFill2.Append(backgroundColor0);
            fill2.Append(patternFill2);

            // FillId = 2           
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FF0000" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3          
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "90EE90" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4            
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);

            // FillId = 5           
            Fill fill6 = new Fill();
            PatternFill patternFill6 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor4 = new ForegroundColor() { Rgb = "1A6600" };
            BackgroundColor backgroundColor4 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            Bold bold1 = new Bold();
            patternFill6.Append(foregroundColor4);
            patternFill6.Append(backgroundColor4);
            fill6.Append(patternFill6);


            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);
            fills1.Append(fill6);

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };

            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color11 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color11);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color2 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color2);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color3);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color4);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat()
            {
                NumberFormatId = (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)0U
            };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats()
            {
                Count =
                    (UInt32Value)4U
            };

            CellFormat cellFormat2 = new CellFormat() //0
            {
                NumberFormatId =
                    (UInt32Value)0U,
                FontId = (UInt32Value)1U,
                FillId =
                    (UInt32Value)0U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U
            };
            CellFormat cellFormat3 = new CellFormat() //1
            {
                NumberFormatId =
                    (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId =
                    (UInt32Value)2U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U,
                ApplyFill = true
            };
            CellFormat cellFormat4 = new CellFormat()//2
            {
                NumberFormatId =
                    (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId =
                    (UInt32Value)3U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U,
                ApplyFill = true
            };
            CellFormat cellFormat5 = new CellFormat()//3
            {
                NumberFormatId =
                    (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId =
                    (UInt32Value)4U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U,
                ApplyFill = true
            };
            CellFormat cellFormat6 = new CellFormat()//4
            {
                NumberFormatId =
                    (UInt32Value)0U,
                FontId = (UInt32Value)1U,
                FillId =
                    (UInt32Value)2U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U,
                ApplyFill = true
            };
            CellFormat cellFormat7 = new CellFormat()//5
            {
                NumberFormatId =
                                   (UInt32Value)0U,
                FontId = (UInt32Value)1U,
                FillId =
                    (UInt32Value)5U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U
            };
            CellFormat cellFormat8 = new CellFormat()//6
            {
                NumberFormatId =
                    (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId =
                    (UInt32Value)0U,
                BorderId = (UInt32Value)0U,
                FormatId =
                    (UInt32Value)0U,
                ApplyFill = true
            };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);
            cellFormats1.Append(cellFormat6);
            cellFormats1.Append(cellFormat7);
            cellFormats1.Append(cellFormat8);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle()
            {
                Name = "Normal",
                FormatId = (UInt32Value)0U,
                BuiltinId = (UInt32Value)0U
            };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 =
            new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles()
            {
                Count = (UInt32Value)0U,
                DefaultTableStyle =
                    "TableStyleMedium2",
                DefaultPivotStyle = "PivotStyleMedium9"
            };

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);

            return stylesheet1;
        }





        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart)
        {
            int numberOfColumns = dt.Columns.Count;


            //List<int> maximumLengthForColumns =
            //  Enumerable.Range(0, dt.Columns.Count)
            //  .Select(col => dt.AsEnumerable()
            //                          .Select(row => row[col]).OfType<string>()
            //                          .Max(val => val.Length)).ToList();

            OpenXmlWriter writer = OpenXmlWriter.Create(worksheetPart, Encoding.ASCII);
            writer.WriteStartElement(new Worksheet());

            writer.WriteStartElement(new Columns());

            for (int c = 0; c < numberOfColumns; c++)
            {
                List<OpenXmlAttribute> oxa;
                oxa = new List<OpenXmlAttribute>();
                // min and max are required attributes
                // This means from columns 2 to 4, both inclusive
                oxa.Add(new OpenXmlAttribute("min", null, (c+1).ToString()));
                oxa.Add(new OpenXmlAttribute("max", null, (c+1).ToString()));
                oxa.Add(new OpenXmlAttribute("width", null, "35")); //maximumLengthForColumns[c] < 20 ? "20" : (maximumLengthForColumns[c] + 5).ToString()));
                writer.WriteStartElement(new Column(), oxa);
                writer.WriteEndElement();
            }


            writer.WriteEndElement();

            writer.WriteStartElement(new SheetData());

            string cellValue = "";

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            
            bool[] IsNumericColumn = new bool[numberOfColumns];
            bool[] IsDateColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            writer.WriteStartElement(new Row { RowIndex = rowIndex });
            for (int colInx = 0; colInx < numberOfColumns; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1", col.ColumnName, ref writer, true);
                IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32") || (col.DataType.FullName == "System.Double") || (col.DataType.FullName == "System.Single");
                IsDateColumn[colInx] = (col.DataType.FullName == "System.DateTime");
            }
            writer.WriteEndElement();   //  End of header "Row"

            //
            //  Now, step through each row of data in our DataTable...
            //
            decimal cellNumericValue = 0;
            foreach (DataRow dr in dt.Rows)
            {
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;

                writer.WriteStartElement(new Row { RowIndex = rowIndex });

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    cellValue = dr.ItemArray[colInx].ToString();
                    cellValue = ReplaceHexadecimalSymbols(cellValue);

                    // Create cell with data
                    if (IsNumericColumn[colInx])
                    {
                        //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                        //  If this numeric value is NULL, then don't write anything to the Excel file.
                        cellNumericValue = 0;
                        if (Decimal.TryParse(cellValue, out cellNumericValue))
                        {
                            cellValue = cellNumericValue.ToString().Replace(",", ".");
                            AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, ref writer);
                        }
                    }
                    else if (IsDateColumn[colInx])
                    {
                        //  This is a date value.
                        DateTime dtValue;
                        string strValue = "";
                        if (DateTime.TryParse(cellValue, out dtValue))
                            strValue = dtValue.ToShortDateString();
                        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), strValue, ref writer);
                    }
                    else
                    {
                        //  For text cells, just write the input data straight out to the Excel file.
                        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, ref writer);
                    }
                }
                writer.WriteEndElement(); //  End of Row
            }
            writer.WriteEndElement(); //  End of SheetData
            writer.WriteEndElement(); //  End of worksheet

            writer.Close();
        }

        private static void AppendTextCell(string cellReference, string cellStringValue, ref OpenXmlWriter writer, bool header = false)
        {
            var colors = new Dictionary<string, int>() { { "red", 1 }, { "yellow", 3 }, { "green", 2 } };
            int color = 0;
            if(colors.ContainsKey(cellStringValue))
                color = colors[cellStringValue];

            var cell = new Cell();
            cell.CellValue = color > 0 ? new CellValue("") : new CellValue(cellStringValue);
            cell.CellReference = cellReference;
            cell.DataType = CellValues.String;
            cell.StyleIndex = header ? (UInt32Value)5U : (UInt32Value)6U;
            if(color > 0)
                cell.StyleIndex = Convert.ToUInt32(color);
            

            writer.WriteElement(cell);

        }

        private static void AppendNumericCell(string cellReference, string cellStringValue, ref OpenXmlWriter writer)
        {
            //  Add a new Excel Cell to our Row 
            writer.WriteElement(new Cell
            {
                CellValue = new CellValue(cellStringValue),
                CellReference = cellReference,
                DataType = CellValues.Number
            });
        }

        private static string ReplaceHexadecimalSymbols(string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }

        //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
        public static string GetExcelColumnName(int columnIndex)
        {
            //  eg  (0) should return "A"
            //      (1) should return "B"
            //      (25) should return "Z"
            //      (26) should return "AA"
            //      (27) should return "AB"
            //      ..etc..
            char firstChar;
            char secondChar;
            char thirdChar;

            if (columnIndex < 26)
            {
                return ((char)('A' + columnIndex)).ToString();
            }

            if (columnIndex < 702)
            {
                firstChar = (char)('A' + (columnIndex / 26) - 1);
                secondChar = (char)('A' + (columnIndex % 26));

                return string.Format("{0}{1}", firstChar, secondChar);
            }

            int firstInt = columnIndex / 26 / 26;
            int secondInt = (columnIndex - firstInt * 26 * 26) / 26;
            if (secondInt == 0)
            {
                secondInt = 26;
                firstInt = firstInt - 1;
            }
            int thirdInt = (columnIndex - firstInt * 26 * 26 - secondInt * 26);

            firstChar = (char)('A' + firstInt - 1);
            secondChar = (char)('A' + secondInt - 1);
            thirdChar = (char)('A' + thirdInt);

            return string.Format("{0}{1}{2}", firstChar, secondChar, thirdChar);
        }

    }
}
