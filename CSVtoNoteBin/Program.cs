using System;
using System.Collections;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace CSVtoNoteBin
{
    class Program
    {

        static void Main(string[] args)
        {
            // Ask the user for CSV file
            Console.WriteLine("Full Filename (With Path) to CSV:");

            string filename = Console.ReadLine();

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                // Set the Delimiter to comma
                parser.Delimiters = new string[] { "," };
                while (true)
                {
                    int i = 0;
                    string[] parts = parser.ReadFields();
                    // If the parser returns null, leave the while loop, should probably add an error message here huh
                    if (parts == null)
                    {
                        break;
                    }

                    Console.WriteLine("{0} field(s)", parts.Length);

                    // Create our byte pool to parse into
                    byte[] byteArray = new byte[parts.Length];

                    // Convert each csv entry to binary
                    foreach (var part in parts)
                    {
                        // Replace junk entries with a blank
                        if (part.Length != 4)
                        {
                            Console.WriteLine("Value {0} is not 4 characters long, replacing with 0000", i);
                            parts[i] = "0000";
                        }
                        BitArray bits = new BitArray(8);
                        for (int s = 0; s < 4; s++)
                        {
                            string tempString = parts[i][s].ToString();
                            if (tempString.Equals("0"))
                            {
                                // Reverse the index due to CopyTo reversal and set the value, same below
                                bits[7 - s] = false;
                            }
                            else
                            {
                                bits[7 - s] = true;
                            }
                            Console.WriteLine("bit {0} of part {1} is {2}", s, i, parts[i][s]);
                        }
                        bits.CopyTo(byteArray, i);
                        i++;
                    }

                    // Build the new filename
                    int offset = filename.Length - filename.LastIndexOf(@".");
                    string newName = filename.Substring(0, filename.Length - offset);
                    newName += ".bin";

                    Console.WriteLine("value of first byte = {0}", byteArray[0]);

                    // Write the generated binary to [fileName].bin
                    bool eh = Program.ByteArrayToFile(newName, byteArray);
                    if (eh)
                    {
                        Console.WriteLine("wrote .bin file to {0}", newName);
                    }
                    else
                    {
                        Console.WriteLine("couldn't write file to {0}", newName);
                    }
                }
            }
        }

        // Write the generated binary to [fileName].bin
        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
    }
}
