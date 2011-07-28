// tabs=4
// Copyright 2010-2010 by Dad Dog Development, Ltd
//
// This work is licensed under the Creative Commons Attribution-No Derivative 
// Works 3.0 License. 
//
// A copy of the license should have been included with this software. If
// not, you can also view a copy of this license, at:
//
// http://creativecommons.org/licenses/by-nd/3.0/ or 
// send a letter to:
//
// Creative Commons
// 171 Second Street
// Suite 300
// San Francisco, California, 94105, USA.
// 
// If this license is not suitable for your purposes, it is possible to 
// obtain it under a different license. 
//
// For more information please contact bretm@daddog.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using nom.tam.util;
using nom.tam.fits;

namespace FitsDataExtractor
{
    class Program
    {
        private static void xreadImage(string fileName, out Int16[][] img, out Header hdr)
        {
            Fits f = new Fits(fileName);

            ImageHDU h = (ImageHDU)f.ReadHDU();
            hdr = h.Header;

            Int32 height = h.Axes[0];
            Int32 width = h.Axes[1];

            object[] rows = (System.Array[])h.Kernel;
            Int16[] inImg = new Int16[height*width];

            int x, y;
            int idx = 0;

            for (y = 0; y < height; y++)
            {
                Int16[] row = (Int16[])rows[y];

                for (x = 0; x < width; x++)
                {
                    inImg[idx++] = row[x];
                }
            }

            Int16 [] outImg = new Int16[height*width];

            int srcIdx, destIdx;

            for (srcIdx = 0, destIdx = 0, y = 0; y < height / 2; y++)
            {
                for (x = 0; x < width / 2; x++)
                {
                    outImg[destIdx] = inImg[srcIdx++];
                    outImg[width + destIdx++] = inImg[srcIdx++];
                    outImg[width + destIdx] = inImg[srcIdx++];
                    outImg[destIdx++] = inImg[srcIdx++];
                }
                destIdx += width;
            }

            img = new Int16[height][];
            for (srcIdx=0, y = 0; y < height; y++)
            {
                img[y] = new Int16[width];

                for (x = 0; x < width; x++)
                {
                    img[y][x] = outImg[srcIdx++];
                }
            }
        }

        private static void readImage(string fileName, out Int16[][] img, out Header hdr)
        {
            Fits f = new Fits(fileName);
            
            ImageHDU h = (ImageHDU)f.ReadHDU();
            hdr = h.Header;

            Int32 height = h.Axes[0];
            Int32 width = h.Axes[1];
            Int16 [][] inImg = new Int16[height][];
            int x, y;
            
            img = new Int16[height][];

            object[] rows = (System.Array[])h.Kernel;

            for (y = 0; y < height / 2; y++)
            {
                inImg[y] = new Int16[2 * width];
                for (x = 0; x < width; x++)
                {
                    inImg[y][x] = (Int16)((Int16[])rows[2 * y])[x];
                    inImg[y][x + width] = (Int16)((Int16[])rows[2 * y + 1])[x];
                }
            }

            for (y = 0; y < height; y++)
            {
                img[y] = new Int16[width];
                for (x = 0; x < width; x++)
                {
                    img[y][x] = (Int16)((Int16[])rows[y])[x];
                }
            }
            
#if false
            for (y = 0; y < height; y += 2)
            {
                int inX = 0;
                int inY = y / 2;

                img[y] = new Int16[width];
                img[y + 1] = new Int16[width];

                for(x = 0; x < width; x += 2)
                {
                    img[y][x] = inImg[inY][inX++];
                    img[y + 1][x] = inImg[inY][inX++];
                    img[y + 1][x + 1] = inImg[inY][inX++];
                    img[y][x + 1] = inImg[inY][inX++];
                }
            }
#endif
        }

        static void writeImage(string fileName, Int16[][] img, Header header)
        {
            Fits f = new Fits();
            BufferedDataStream s = new BufferedDataStream(new FileStream(fileName, FileMode.Create));
            //BufferedFile s = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite);
            BasicHDU h = FitsFactory.HDUFactory(img);
            Header hdr = h.Header;

            f.AddHDU(h);
            
            f.Write(s);
        }

        static void Main(string[] args)
        {
            string fileNamePrefix = "blue.fits";
            string fileName = "c:\\temp\\sx-ascom\\" + fileNamePrefix;
            string outfileName = "c:\\temp\\sx-ascom\\" + "out_" + fileNamePrefix;

            const int width = 3032;
            const int height = 2016;

            Int16[][] img;
            Header hdr;
            readImage(fileName, out img, out hdr);

            using (BinaryReader binReader = new BinaryReader(File.Open("D:\\NotBackedUp\\astro\\test images\\colors\\blue.raw", FileMode.Open, FileAccess.Read)))
            {
                int x, y;

                Int16 [] rawImage = new Int16[width*height];
                int srcIdx = 0;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        rawImage[srcIdx++] = binReader.ReadInt16();
                    }
                }

                try
                {
                    byte b;

                    b = binReader.ReadByte();

                    Console.WriteLine("Did not read all data");
                }
                catch (EndOfStreamException)
                {
                }

            }
            writeImage(outfileName, img, hdr);

        }
    }
}
