using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;



namespace BinaryKits.Zpl.Label.Helpers
{
    /// <summary>
    /// Alternative Data Compression Scheme for ~DG and ~DB Commands
    /// There is an alternative data compression scheme recognized by the Zebra printer. This scheme further
    /// reduces the actual number of data bytes and the amount of time required to download graphic images and
    /// bitmapped fonts with the ~DG and ~DB commands
    /// </summary>
    public static class ZebraHexCompressionHelper
    {
        /// <summary>
        /// MinCompressionBlockCount (CompressionCountMapping -> g)
        /// </summary>
        const int MinCompressionBlockCount = 20;

        /// <summary>
        /// CompressionCountMapping (CompressionCountMapping -> z)
        /// </summary>
        const int MaxCompressionRepeatCount = 400;

        /// <summary>
        /// The mapping table used for compression.
        /// Each character count (the key) is represented by a certain char (the value).
        /// </summary>
        private static readonly Dictionary<int, char> CompressionCountMapping = new Dictionary<int, char>()
        {
            {1, 'G'}, {2, 'H'}, {3, 'I'}, {4, 'J'}, {5, 'K'}, {6, 'L'}, {7, 'M'}, {8, 'N'}, {9, 'O' }, {10, 'P'},
            {11, 'Q'}, {12, 'R'}, {13, 'S'}, {14, 'T'}, {15, 'U'}, {16, 'V'}, {17, 'W'}, {18, 'X'}, {19, 'Y'},
            {20, 'g'}, {40, 'h'}, {60, 'i'}, {80, 'j' }, {100, 'k'}, {120, 'l'}, {140, 'm'}, {160, 'n'}, {180, 'o'}, {200, 'p'},
            {220, 'q'}, {240, 'r'}, {260, 's'}, {280, 't'}, {300, 'u'}, {320, 'v'}, {340, 'w'}, {360, 'x'}, {380, 'y'}, {400, 'z' }
        };

        private static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        /// <summary>
        /// Compress hex
        /// </summary>
        /// <param name="hexData">Clean hex data 000000\nFFFFFF</param>
        /// <param name="bytesPerRow"></param>
        /// <returns></returns>
        public static string Compress(string hexData, int bytesPerRow)
        {
            var chunkSize = bytesPerRow * 2;
            var cleanedHexData = hexData.Replace("\n", string.Empty).Replace("\r", string.Empty);

            var compressedLines = new StringBuilder(hexData.Length);
            var compressedCurrentLine = new StringBuilder(chunkSize);
            var compressedPreviousLine = string.Empty;

            var dataLines = Split(cleanedHexData, chunkSize);
            foreach (var dataLine in dataLines)
            {
                var lastChar = dataLine[0];
                var charRepeatCount = 1;

                for (var i = 1; i < dataLine.Length; i++)
                {
                    if (dataLine[i] == lastChar)
                    {
                        charRepeatCount++;
                        continue;
                    }

                    //char changed within the line
                    compressedCurrentLine.Append(GetZebraCharCount(charRepeatCount));
                    compressedCurrentLine.Append(lastChar);

                    lastChar = dataLine[i];
                    charRepeatCount = 1;
                }

                //process last char in dataLine
                if (lastChar.Equals('0'))
                {
                    //fills the line, to the right, with zeros
                    compressedCurrentLine.Append(',');
                }
                else if (lastChar.Equals('1'))
                {
                    //fills the line, to the right, with ones
                    compressedCurrentLine.Append('!');
                }
                else
                {
                    compressedCurrentLine.Append(GetZebraCharCount(charRepeatCount));
                    compressedCurrentLine.Append(lastChar);
                }

                if (compressedCurrentLine.Equals(compressedPreviousLine))
                {
                    //previous line is repeated
                    compressedLines.Append(':');
                }
                else
                {
                    compressedLines.Append(compressedCurrentLine);
                }

                compressedPreviousLine = compressedCurrentLine.ToString();
                compressedCurrentLine.Clear();
            }

            return compressedLines.ToString();
        }





        public static byte[] Decompress(byte[] data)
        {
            byte[] decompressedArray = null;
            try
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (MemoryStream compressStream = new MemoryStream(data))
                    {
                        using (DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
                        {
                            deflateStream.CopyTo(decompressedStream);
                        }
                    }
                    decompressedArray = decompressedStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                // do something !
            }

            return decompressedArray;
        }

        static string mainSec(string[] args)
        {
            //this is C# 
            var sb = new StringBuilder();
            sb.AppendLine("^XA");
            sb.AppendLine("^UT1,1");
            sb.AppendLine("^Uh0,300,64,4000,2,0,F");
            sb.AppendLine("^Uh1,300,64,4000,2,0,F");
            sb.AppendLine("^UC1,0,1,0,0");
            sb.AppendLine("^LH0,0");
            sb.AppendLine("^LT0");
            sb.AppendLine("^XZ");
            sb.AppendLine("^XA");
            sb.AppendLine("^PW3543");
            sb.AppendLine("^LL512");
            sb.AppendLine("^FO532,66^GFA,885,9272,76,:Z64:eJztWkFywyAMDMOBI0/gKTyNPC1P6RN6zKFTNxiCY0AgJE0nh+zJTc0GrdYES1wu/wWzHfiVIuLR2YZpB4FJpZH38UcouP6oRHddogqPEd/9f0WyLzyTGqpsVuLUj5tvoxv8tv3gqSa3WCSZxrjIoMgUzpAGoxnW2wZMdIFHG9tOEhQtekVyze7VSzYMQ/3DVIMTRt/sFtcUA0epZ2o28GCUfi3CCOjbDWHVhMaElRyOB1nss3+C7no7rAqf4DsTM+vr+A7VUYw4rTixeqAmqdUf2bKjESrz96LGwlQWNxRvPVHNw1N+2p9wJ30UWfkIfbKT5YQY1X/5gxVinMqtXCui53vjWVmMCEcmHS/EGOT1ecnKYoQudlXra3ONIphZ3Oh14H7rCzrKdDxXrijYLV3w5Socmr50HcixCUhfNBeQvkxIQPoiVOA92AkqPdISacwsImnMSklyiVgiu8FyF6+EnUbEXjk8SS4Rq+YUviuXyCOUH6IP14er5npX37/rmiO5rkqu90K/Q5Jcux0Ud0OekCwvuZ+Q3OdI7r9EDJZJJPerkvtoiUQWY0m+d0i+Dwk4/5BJ8P2R++p+enk3XIf5I3uSdYDX93gKTnUEywvypLdmBVlVgwInk1U1iFUcqibCKVo1AjHqhc1Qanm1W2qsq5FouFZqS677diyw2r7LcL0Vnjax7rTgHsEQQC+CUsDvl+8vpMYCOGTQogEwaALBLZo+hk2gpRbYpM1llnLpx4V6fMsQ0TQM6KaAmQqisC1ijWiyahwZ7jZUVxfZIcb0m1F96x12dnzALRx9GHf8x6cFWmzwkQAPH2EA4AE26PMh0lmTagLkMye+HDG5P4+HUCbVsHGZIqqzMLPb/wCr2VTW:718B");
            sb.AppendLine("^PQ1,0,1,Y^XZ");
            var zpl = args[0].ToString();

            //extract the Z64-String
            var z64Data = zpl;
            var bytesPerRow = int.Parse(args[1]);

            foreach (var item in zpl.Split('^'))
            {
                if (item.StartsWith("GFA"))
                {
                    var sp = item.Split(':');
                    z64Data = item.Substring(sp[0].Length);
                    bytesPerRow = Convert.ToInt32(sp[0].Split(',')[3]);
                    break;
                }
            }


            //convert String to Bitmap
            //System.Drawing.Bitmap decodedBitmap = null;
            if (z64Data.StartsWith(":Z64"))
            {
                var imageData = DecompressZb64(z64Data.Substring(5));

                int width = bytesPerRow * 8;
                int height = imageData.Length / bytesPerRow;

                var decodedBitmap = ArrayToBitmap(imageData, width, height, PixelFormat.Format1bppIndexed);

                decodedBitmap.Save("imageASD.png", ImageFormat.Png);

                System.IO.MemoryStream ms = new MemoryStream();
                decodedBitmap.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                var SigBase64 = Convert.ToBase64String(byteImage);
                return SigBase64.ToString();
            }

            else return "";

            //Debug.WriteLine(decodedBitmap.Width + ":" + decodedBitmap.Height);
            var fffff = "";
        }

        public static byte[] DecompressZb64(string compressedString)
        {
            var b64 = Convert.FromBase64String(compressedString.Split(':')[0]).Skip(2).ToArray();
            return Decompress(b64);
        }

        public static Bitmap ArrayToBitmap(byte[] bytes, int width, int height, PixelFormat pixelFormat)
        {
            var image = new Bitmap(width, height, pixelFormat);
            var imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                              ImageLockMode.ReadWrite, pixelFormat);

            try
            {
                Marshal.Copy(bytes, 0, imageData.Scan0, bytes.Length);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return image;
        }


        /// <summary>
        /// Uncompress data
        /// </summary>
        /// <param name="compressedHexData"></param>
        /// <param name="bytesPerRow"></param>
        /// <returns></returns>
        public static string Uncompress(string compressedHexData, int bytesPerRow)

        {
            // var retString = "";
            // var compressedString = "eJztkDFOxDAQRb81hRsULmBtruECyRwpZYpFGLmg5AhwFKMUuYal9CtL26QwHsbe3RMguv3lz9P85wD3/CWaiZ+56OjqWA44cwKIAyfeXXL1sQ7YWqd54czltTge+VOdOQsXFp8TrLUw9KEW3+6pLU4Zk3mC0ataonSEzU8JMywGCiFcue+c8YLGvYcLF5a+68WFhbvtRs5jdmVkWolj96vgXe/it7eucT+0+gxV5N5RrdTveQpevhnxO+BEfRe0xIzc/EbUzkn3lhLSIH6DdFeu+c39Hb7c7vksfrJryB8vu6A4cxE/NjpK1/6LkJZ3+nL1gaLt3D33/Ed+AehfkrY=";
            // var b64 = Convert.FromBase64String(compressedString);
            // var string4 = b64.ToString();
            // var encoding = new ASCIIEncoding();

            // var result = "";

            // string[] tempArray = { compressedHexData, bytesPerRow.ToString() };

            //var stringBitmap = mainSec(tempArray);


            ////var outBytes = new byte[b64.Length - 2];
            //// try
            //// {
            ////     using (var memoryStream = new MemoryStream(b64))
            ////     {
            ////         memoryStream.ReadByte();
            ////         memoryStream.ReadByte();
            ////         using (var decompressionStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            ////         {
            ////             decompressionStream.Read(outBytes, 0, b64.Length - 2);
            ////         }


            ////         retString = encoding.GetString(outBytes);
            ////         result = System.Text.Encoding.UTF8.GetString(outBytes);


            ////         var sss = "";

            ////     }



            //// }
            //// catch (Exception e)
            //// {
            ////     // TODO: DOcument exception
            ////     Console.WriteLine(e.Message);
            //// }

            //// int NumberChars = result.Length;



            // return stringBitmap;


            var hexData = new StringBuilder();
            var chunkSize = bytesPerRow * 2;
            var lineIndex = 0;
            var totalCount = 0;

            var reverseMapping = CompressionCountMapping.ToDictionary(o => o.Value, o => o.Key);

            foreach (var c in compressedHexData)
            {
                if (c.Equals(':'))
                {

                    var appendRepeat = hexData.ToString().Substring(hexData.Length - (chunkSize + 1));
                    hexData.Append(appendRepeat);
                    continue;
                }

                if (c.Equals(','))
                {
                    var appendZero = new string('0', chunkSize - lineIndex);
                    hexData.Append(appendZero);
                    hexData.Append("\n");
                    lineIndex = 0;
                    continue;
                }

                if (c.Equals('!'))
                {
                    var appendOne = new string('1', chunkSize - lineIndex);
                    hexData.Append(appendOne);
                    hexData.Append("\n");
                    lineIndex = 0;
                    continue;
                }

                if (reverseMapping.TryGetValue(c, out var count))
                {
                    totalCount += count;
                    continue;
                }

                if (totalCount == 0)
                {
                    totalCount = 1;
                }

                var append = new string(c, totalCount);
                hexData.Append(append);
                lineIndex += append.Length;
                totalCount = 0;

                if (lineIndex == chunkSize)
                {
                    hexData.Append("\n");
                    lineIndex = 0;
                }
            }

            return hexData.ToString();
        }

        public static string GetZebraCharCount(int charRepeatCount)
        {
            if (CompressionCountMapping.TryGetValue(charRepeatCount, out var compressionKey))
            {
                return $"{compressionKey}";
            }

            var compressionKeys = new StringBuilder(5);

            var multi20 = charRepeatCount / MinCompressionBlockCount * MinCompressionBlockCount;
            var remainder = charRepeatCount % multi20;

            while (remainder > 0 || multi20 > 0)
            {
                if (multi20 > MaxCompressionRepeatCount)
                {
                    remainder += multi20 - MaxCompressionRepeatCount;
                    multi20 = MaxCompressionRepeatCount;
                }

                if (!CompressionCountMapping.TryGetValue(multi20, out compressionKey))
                {
                    throw new Exception("Compression failure");
                }

                compressionKeys.Append(compressionKey);

                if (remainder == 0)
                {
                    break;
                }

                if (remainder <= 20)
                {
                    CompressionCountMapping.TryGetValue(remainder, out compressionKey);
                    compressionKeys.Append(compressionKey);
                    break;
                }

                multi20 = remainder / MinCompressionBlockCount * MinCompressionBlockCount;
                remainder %= multi20;
            }

            return compressionKeys.ToString();
        }
    }
}
