using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace analyzer
{
    class Program
    {
        const int width = 3032;
        const int height = 2016;
        const int numElements = width * height;

        static void Main(string[] args)
        {
            //readRawFile("D:\\NotBackedUp\\astro\\test images\\colors\\red.raw");
            //readCookedFile("D:\\NotBackedUp\\astro\\test images\\colors\\red.cooked");
            //readRawFile("D:\\NotBackedUp\\astro\\test images\\colors\\green.raw");
            //readCookedFile("D:\\NotBackedUp\\astro\\test images\\colors\\green.cooked");
            readRawFile("D:\\NotBackedUp\\astro\\test images\\colors\\blue.raw");
            readCookedFile("D:\\NotBackedUp\\astro\\test images\\colors\\blue.cooked");

            Console.ReadLine();

        }

        static void stats(UInt32[,] imageData)
        {
            UInt64[,] sums = new UInt64[2, 2];
            UInt64 sum = 0;
            UInt64 zeros = 0;
            int x, y;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    sums[x % 2, y % 2] += imageData[x, y];
                    sum += imageData[x ,y];
                    if (imageData[x, y] == 0)
                    {
                        zeros++;
                    }
                }
            }

            Console.WriteLine("imaageRawData.Length = {0}", imageData.Length);
            Console.WriteLine("sum={0:N}, average={1}", sum, sum / numElements);
            Console.WriteLine("sums:\n\t{0,15:N0}\n\t{1,15:N0}\n\t{2,15:N0}\n\t{3,15:N0}", sums[0, 0], sums[0, 1], sums[1, 0], sums[1, 1]);
            Console.WriteLine("zeros = {0}", zeros);

            Console.WriteLine();
        }

        static void readRawFile(string path)
        {
            Console.WriteLine("processing raw file " + path);
            
            Type elementType = typeof(UInt16);

            Array imageRawData;
            imageRawData = System.Array.CreateInstance(elementType, numElements);

            int x, y, srcIdx=0;

            using (BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        imageRawData.SetValue(binReader.ReadUInt16(), srcIdx++);
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

            UInt32[,] imageData = new UInt32[width, height];

            srcIdx = 0;
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    imageData[x, y] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                }
            }

            Console.WriteLine("srcIdx = {0}", srcIdx);
            stats(imageData);

            imageData = new UInt32[width, height];
            srcIdx = 0;
            for (y = 0; y < height; y += 2)
            {
                for (x = 0; x < width; x += 2)
                {
                    imageData[x, y] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                    imageData[x, y+1] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                    imageData[x + 1, y + 1] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                    imageData[x+1, y] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                }
            }
#if false
            using (BinaryWriter binWriter = new BinaryWriter(File.Open("c:\\temp\\sx-ascom\\image.test", FileMode.Create)))
            {
                srcIdx = 0;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {        
                        binWriter.Write(imageData[x, y]);
                    }
                }
            }
#endif
            Console.WriteLine("srcIdx = {0}", srcIdx);
            stats(imageData);
        }

        static void readCookedFile(string path)
        {
            UInt32 [,] imageData = new UInt32[width, height];
            int x, y;

            Console.WriteLine("processing cooked file " + path);
            using (BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                for (x = 0; x < width; x++)
                {
                    for (y = 0; y < height; y++)
                    {
                        imageData[x, y] = binReader.ReadUInt32();
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

            stats(imageData);
        }
    }

}
