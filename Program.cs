using System;
using System.IO;

namespace Kvadratu_rekursija
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream file = new FileStream("sample.bmp", FileMode.Create, FileAccess.Write))
            {

                // Define BMP header (with correct file size)
                int width = 5000, height = 5000;


                int rowSize = (width + 31) / 32 * 4; // Ensure row size is multiple of 4 bytes

                int fileSize = 62 + (height * rowSize);

                // A monochrome 1000x1000 pixel BMP image is created
                file.Write(
                    new byte[62]
                    {
                        // Header
                        0x42, 0x4d,     // Signature 'BM'
                        (byte)(fileSize), (byte)(fileSize >> 8), (byte)(fileSize >> 16), (byte)(fileSize >> 24),  // File size
                        0x0, 0x0, 0x0, 0x0,
                        0x3e, 0x0, 0x0, 0x0,
                        // Header information
                        0x28, 0x0, 0x0, 0x0,

                        (byte)(width), (byte)(width >> 8), 0x00, 0x00,  // Image width
                        (byte)(height), (byte)(height >> 8), 0x00, 0x00, // Image height
                        
                        0x01, 0x00,                 // Number of color planes (1)
                        0x01, 0x00,                 // Bits per pixel (1-bit monochrome)
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0xf4, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x02, 0x00, 0x00, 0x00,     // Number of colors in palette (2 for monochrome)
                        0x0, 0x0, 0x0, 0x0,


                        // Color table (can be changed to black and white)
                        0x0, 0x0, 0x0, 0x0,
                        0xff, 0xff, 0xff, 0x0
                    }
                 );

                // Calculate the number of bytes in a BMP image row (multiple of 4): 1000 bits / 32 bits = 31.25, 
                // so in this case the row will require 32 bytes of 4
                int l = (width + 31) / 32 * 4;
                //Console.WriteLine(l);

                // Define the array for the pixels. The color of the first pixel in the array corresponds to the value of the first bit.
                var t = new byte[height * l];

                DrawSquare(0, 0, width, l, t, 3);

                file.Write(t);
                file.Close();
            }
        }

        // TODO paklausti ar image size visada bus kvadratas ar gali būti ir stačiakampis
        // TODO perdaryti ciklus i rekursija
        static void DrawSquare(int startPosX, int startPosY, int size, int l, byte[] t, int depth)
        {
            if (depth == 0)
                return;

            int offset = size / 3;

            // Nupiesia vertikalias linijas
            for (int i = startPosY; i < size + startPosY; i++)
            {
                t[i * l + (offset + startPosX) / 8] = 0b10000000;
                t[i * l + (offset * 2 + startPosX) / 8] = 0b10000000;
            }

            // Nupiesia horizontalias linijas
            for (int i = startPosX / 8; i < (size + startPosX) / 8; i++)
            {
                t[(startPosY + offset) * l + i] = 0b11111111;
                t[(startPosY + offset * 2) * l + i] = 0b11111111;
            }

            // Uzpildo vidurini kvadrata
            for(int y = startPosY + offset; y < startPosY + offset * 2; y++)
                for(int x = (startPosX + offset) / 8; x < (startPosX + offset * 2)/ 8; x++)
                    t[y * l + x] = 0b11111111;

            
            DrawSquare(startPosX, startPosY, size / 3, l, t, depth - 1); // bottom left
            DrawSquare(startPosX + offset, startPosY, offset, l, t, depth - 1); // bottom middle
            DrawSquare(startPosX + offset * 2, startPosY, offset, l, t, depth - 1); // bottom right
            DrawSquare(startPosX, startPosY + offset, offset, l, t, depth - 1); // middle left
            DrawSquare(startPosX + offset * 2, startPosY + offset, offset, l, t, depth - 1); // middle right
            DrawSquare(startPosX, startPosY + offset * 2, offset, l, t, depth - 1); // top left
            DrawSquare(startPosX + offset, startPosY + offset * 2, offset, l, t, depth - 1); // top middle
            DrawSquare(startPosX + offset * 2, startPosY + offset * 2, offset, l, t, depth - 1); // top right
        }
    }
}
