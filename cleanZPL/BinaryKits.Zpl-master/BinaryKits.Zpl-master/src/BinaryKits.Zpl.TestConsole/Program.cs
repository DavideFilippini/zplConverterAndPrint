using BinaryKits.Zpl.Labelary;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using SkiaSharp;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;


namespace BinaryKits.Zpl.TestConsole
{
    class Program
    {
#pragma warning disable CS1998 // In questo metodo asincrono non sono presenti operatori 'await', pertanto verrà eseguito in modo sincrono. Provare a usare l'operatore 'await' per attendere chiamate ad API non di blocco oppure 'await Task.Run(...)' per effettuare elaborazioni basate sulla CPU in un thread in background.
        static async Task Main(string[] args)
#pragma warning restore CS1998 // In questo metodo asincrono non sono presenti operatori 'await', pertanto verrà eseguito in modo sincrono. Provare a usare l'operatore 'await' per attendere chiamate ad API non di blocco oppure 'await Task.Run(...)' per effettuare elaborazioni basate sulla CPU in un thread in background.
        {
            //args[0] = "@\"C:\\Users\\david\\Downloads\\test_base64.txt";
            string uploadText = System.IO.File.ReadAllText(args[0]);
            string savePAth = "";
            if (args.Length > 1) savePAth = args[1] + "//";
            var printerName = "Badgy200";
            if (args.Length > 2) printerName = args[2];
            

            //start "title" BinaryKits.Zpl.TestConsole.exe C:\\Users\\david\\Downloads\\test_img_convertite_noz64.txt C:\\Users\\david\\Downloads\
            //string uploadText = System.IO.File.ReadAllText("C:\\\\Users\\\\david\\\\Downloads\\\\test_img_convertite_noz64_2.txt");
            renderbasic(uploadText, savePAth, printerName);
        }

        static string renderbasic(string uploadText, string savePAth, string printerName)
        {

            var drawOptions = new DrawerOptions()
            {
                FontLoader = fontName => {
                    if (fontName == "0")
                    {
                        return SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                    }
                    else if (fontName == "1")
                    {
                        return SKTypeface.FromFamilyName("SIMSUN");
                    }

                    return SKTypeface.Default;
                }
            };

            var pathImage = @"" + savePAth + "zplToImageResult.png";

            IPrinterStorage printerStorage = new PrinterStorage();
            var drawer = new ZplElementDrawer(printerStorage, drawOptions);
            var analyzer = new ZplAnalyzer(printerStorage);
            var analyzeInfo = analyzer.Analyze(uploadText);
            int i = 0;

            byte[] fileToPrint = { };
            foreach (var labelInfo in analyzeInfo.LabelInfos)
            {
                var imageData = drawer.Draw(labelInfo.ZplElements, 50, 76, 12);
                File.WriteAllBytes(pathImage, imageData);
                fileToPrint = imageData;
                i++;
            }

            //            return SendFileToPrinter("NPI05D626 (HP Color LaserJet MFP M281fdw)", pathImage).ToString();


            // initialize PrintDocument object
            PrintDocument doc = new PrintDocument()
            {
                PrinterSettings = new PrinterSettings()
                {
                    // set the printer to 'Microsoft Print to PDF'
                    PrinterName = printerName,

                    // tell the object this document will print to file
                    PrintToFile = true,

                    // set the filename to whatever you like (full path)
                    PrintFileName = Path.Combine(pathImage),
                }
            };

            doc.Print();


            return "";

        }




        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);



        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.
            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }





        static async Task RenderPreviewAsync(string zplData)
        {
            var client = new LabelaryClient();
            var previewData = await client.GetPreviewAsync(zplData, PrintDensity.PD8dpmm, new LabelSize(6, 8, Measure.Inch));

            File.WriteAllBytes("label22.png", previewData);




            var fileName = "ps.png";
            await File.WriteAllBytesAsync(fileName, previewData);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = string.Empty
            };

            Process.Start(processStartInfo);





            //ZplElementBase[] elements = { new ZplRaw(zplData) };




            //var renderEngine = new ZplEngine(elements);



            //IPrinterStorage printerStorage = new PrinterStorage();
            //var drawer = new ZplElementDrawer(printerStorage);

            //var analyzer = new ZplAnalyzer(printerStorage);
            //var analyzeInfo = analyzer.Analyze(previewData.ToString());


            //foreach (var labelInfo in analyzeInfo.LabelInfos)
            //{
            //var imageData = drawer.Draw(elements);
            // File.WriteAllBytes("label.png", imageData);
            //}

            //var fileName = $"preview-{Guid.NewGuid()}.png";
            //await File.WriteAllBytesAsync(fileName, imageData);

            //var processStartInfo = new ProcessStartInfo
            //{
            //    FileName = fileName,
            //    UseShellExecute = true,
            //    CreateNoWindow = true,
            //    Verb = string.Empty
            //};

            //Process.Start(processStartInfo);



            //if (previewData.Length == 0)
            //{
            //    return;
            //}

            //var fileName = $"preview-{Guid.NewGuid()}.png";
            //await File.WriteAllBytesAsync(fileName, previewData);

            //var processStartInfo = new ProcessStartInfo
            //{
            //    FileName = fileName,
            //    UseShellExecute = true,
            //    CreateNoWindow = true,
            //    Verb = string.Empty
            //};

            //Process.Start(processStartInfo);
        }
    }
}
