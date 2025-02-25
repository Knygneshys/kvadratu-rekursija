using System;
using System.Diagnostics;
using System.IO;

namespace Kvadratu_rekursija
{
    class Program
    {
        static int counter = 0;
        static void Main(string[] args)
        {
            using (FileStream file = new FileStream("sample.bmp", FileMode.Create, FileAccess.Write))
            {
                // Define BMP header (with correct file size)
                int size = 1000;
                int width = size, height = size;


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

                        // juoda ant balto
                        // 0xff, 0xff, 0xff, 0x0,
                        // 0x0, 0x0, 0x0, 0x0
                        

                        // balta ant juodo
                        0x0, 0x0, 0x0, 0x0,
                        0xff, 0xff, 0xff, 0x0
                    }
                 );

                // Calculate the number of bytes in a BMP image row (multiple of 4): 1000 bits / 32 bits = 31.25, 
                // so in this case the row will require 32 bytes of 4
                int l = (size + 31) / 32 * 4;

                // Define the array for the pixels. The color of the first pixel in the array corresponds to the value of the first bit.
                var t = new byte[height * l];
                Stopwatch timer = new Stopwatch();
                timer.Start();
                BeginDrawing(size, l, t, 3);
                timer.Stop();
                
                Console.WriteLine("Veiksmų sk.: " + counter);
                Console.WriteLine("Laikas ms.: " + timer.Elapsed);  
                
                file.Write(t);
                file.Close();
            }
        }

        static void BeginDrawing(int size, int l, byte[] t, int depth)
        {
            counter += 2;
            
            DrawOutline(size, l, t);
            DrawSquare(0, 0, size, l, t, depth); 
        }

        static void DrawOutline(int size, int l, byte[] t)
        {
            counter += 2;
            DrawHorizontalBorders(0, size, l, t);
            DrawVerticalBorders(0, size, l, t);
        }

        static void DrawHorizontalBorders(int i, int size, int l, byte[] t)
        {
            counter++;
            if (i >= l)
            {
                counter++;
                return;
            }

            counter += 3;
            // Top border
            t[i] = 0b11111111;
            // Bottom border
            t[l * (size - 1) + i] = 0b11111111;
    
            DrawHorizontalBorders(i + 1, size, l, t);
        }

        static void DrawVerticalBorders(int i, int size, int l, byte[] t)
        {
            counter++;
            if (i >= size)
            {
                counter++;
                return;
            }

            counter += 3;
            // Left border
            t[l * i] |= 0b10000000;
            // Right border
            t[l * i + ((size - 1) / 8)] |= (byte)(0b10000000 >> ((size - 1) % 8));
            
            DrawVerticalBorders(i + 1, size, l, t);
        }
        
        static void DrawSquare(int startPosX, int startPosY, int size, int l, byte[] t, int depth)
        {
            counter++;
            if (depth == 0)
            {
                counter++;
                return;
            }

            counter++;
            int offset = size / 3;

            // Nupiesia vertikalias linijas

            counter += 3;
            
            DrawVerticalLines(startPosY, startPosY, startPosX, offset, l, size, t);

            // Nupiesia horizontalias linijas
            
            DrawHorizontalLines(startPosX / 8, startPosY, startPosX, offset, l, size, t);
            
            // Uzpildo vidurini kvadrata
            
            
            DrawSquareFirstCycle(startPosY + offset, startPosY, startPosX, offset, l, size, t);

            counter += 8;
            DrawSquare(startPosX, startPosY, size / 3, l, t, depth - 1); // bottom left
            DrawSquare(startPosX + offset, startPosY, offset, l, t, depth - 1); // bottom middle
            DrawSquare(startPosX + offset * 2, startPosY, offset, l, t, depth - 1); // bottom right
            DrawSquare(startPosX, startPosY + offset, offset, l, t, depth - 1); // middle left
            DrawSquare(startPosX + offset * 2, startPosY + offset, offset, l, t, depth - 1); // middle right
            DrawSquare(startPosX, startPosY + offset * 2, offset, l, t, depth - 1); // top left
            DrawSquare(startPosX + offset, startPosY + offset * 2, offset, l, t, depth - 1); // top middle
            DrawSquare(startPosX + offset * 2, startPosY + offset * 2, offset, l, t, depth - 1); // top right
        }

        static void DrawVerticalLines(int i, int startPosY, int startPosX, int offset, int l, int size, byte[] t)
        {
            counter++;
            if (i >= size + startPosY)
            {
                counter++;
                return;
            }

            counter += 3;
            // Ensure continuous vertical lines by using appropriate bit masks
            t[i * l + (offset + startPosX) / 8] |= 0b10000000;
            t[i * l + (offset * 2 + startPosX) / 8] |= 0b10000000;

            DrawVerticalLines(++i, startPosY, startPosX, offset, l, size, t);
        }
        
        static void DrawHorizontalLines(int i, int startPosY, int startPosX, int offset, int l, int size, byte[] t)
        {
            counter++;
            if (i >= (size + startPosX) / 8)
            {
                counter += 3;
                // Draw final pixels to connect the lines
                t[(startPosY + offset) * l + (size + startPosX) / 8 ] = 0b11111111;
                t[(startPosY + offset * 2) * l + (size + startPosX) / 8 ] = 0b11111111;
                return;
            }

            counter += 3;
            // Use OR operation to ensure that existing pixels are not overwritten
            t[(startPosY + offset) * l + i] = 0b11111111;
            t[(startPosY + offset * 2) * l + i] = 0b11111111;

            DrawHorizontalLines(++i, startPosY, startPosX, offset, l, size, t);
        }


        static void DrawSquareFirstCycle(int y, int startPosY, int startPosX, int offset, int l, int size, byte[] t)
        {
            counter++;
            if (y >= startPosY + offset * 2)
            {
                counter++;
                return;
            }

            counter += 2;
            DrawSquareSecondCycle(y, (startPosX + offset) / 8, startPosY, startPosX, offset, l, size, t);

            DrawSquareFirstCycle(++y, startPosY, startPosX, offset, l, size, t);
        }

        static void DrawSquareSecondCycle(int y, int x, int startPosY, int startPosX, int offset, int l, int size, byte[] t)
        {
            counter++;
            if (x >= (startPosX + offset * 2) / 8)
            {
                counter++;
                return;
            }

            counter += 2;
            t[y * l + x] = 0b11111111;

            DrawSquareSecondCycle(y, ++x, startPosY, startPosX, offset, l, size, t);
        }

        // nuo pav dydzio ziureti laika ir ivykdytas kodo eilutes (su globaliu kintamuoju apsiskaiciuoti) taip pat ir su gyliu.
        // liūte neliūdėk
    }
}
