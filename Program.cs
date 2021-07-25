using System;
using System.Text;
using System.IO;

namespace txb
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
                ParseTxb(args[0], true);
            else if (args.Length == 2)
                if (args[1].ToLower() == "-nodel")
                    ParseTxb(args[0], false);
                else
                    PrintUsage();
            else
                PrintUsage();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Gravity Rush TXB Text Extractor by LinkOFF v.0.2");
            Console.WriteLine("This program supports PS4 and PS Vita versions.");
            Console.WriteLine("Currently version supports extraction only.");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("\tExtraction:\ttxb.exe <txb file> <argument>");
            Console.WriteLine("\tNote: You can use '-nodel' argument to extract text without delimeter '[t][/t]'.");

        }

        static void ParseTxb(string inputFile, bool useReg)
        {
            string filename = Path.GetFileNameWithoutExtension(inputFile);
            using (BinaryReader reader = new BinaryReader(File.OpenRead(inputFile)))
            {
                string magic = GetString(reader, 4);
                if (magic != "txbL")
                    throw new FormatException("Incorrect signature of file!");
                int version = reader.ReadInt32();
                int fullSize = reader.ReadInt32();
                int entries = reader.ReadInt32();

                uint[] unknownValues = new uint[entries];
                uint[] offsets = new uint[entries];
                string[] text = new string[entries];

                //unknown values
                for(int i = 0; i < entries; i++)
                    unknownValues[i] = reader.ReadUInt32();

                //offsets
                for (int i = 0; i < entries; i++)
                    offsets[i] = reader.ReadUInt32();

                long startOffset = reader.BaseStream.Position;

                //info + text
                for(int i = 0; i < entries; i++)
                {
                    reader.BaseStream.Position = offsets[i] + startOffset;
                    short char_number = reader.ReadInt16();
                    short size = reader.ReadInt16();
                    reader.BaseStream.Position += 4;
                    if(useReg)
                        text[i] = "[t]"+GetString(reader, size)+"[/t]";
                    else
                        text[i] = GetString(reader, size);
                }
                File.WriteAllLines(filename + ".txt", text);
            }
        }
        public static string GetString(BinaryReader reader, int length)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
    }
}
