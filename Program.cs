using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlToHtml
{
   class Program
   {
      const short VERSION = 1;
      static IList<string> myInputs = null;
      static bool myQuiet = false;
      static bool myPause = false;

      static int Main(string[] args)
      {
         try
         {
            if (ParseArgs(ref args))
            {
               string[] outs = TransformXml.ToHtml(args);
               for (int x = 0; x < args.Length; x++)
               {
                  if (outs[x] != null)
                  {
                     WriteLine(string.Format("\"{0}\" => {1}", System.IO.Path.GetFileName(args[x]), outs[x]));
                  }
               }
               return 0;
            }

            return 1;
         }
         catch (Exception e)
         {
            Console.WriteLine(string.Format("Error: {0}", e.Message));
            return e.GetHashCode();
         }
         finally
         {
            if (myPause) { Console.WriteLine("Press any key to exit"); Console.ReadKey(false); };
         }
      }

      static bool ParseArgs(ref string[] args)
      {
         if (args.Length == 0) { ShowUsage(); myPause = false; return false; }

         for (int x = 0; x < args.Length; x++)
         {
            if (args[x].StartsWith("-") || args[x].StartsWith("/"))
            {
               // if argument is a switch
               if (args[x].Length > 1)
                  switch (args[x].Substring(1).ToLowerInvariant())
                  {
                     case "quiet":
                        myQuiet = true;
                        break;
                     case "pause":
                        myPause = true;
                        break;
                     default:
                        ShowUsage();
                        myPause = false;
                        return false;
                  }
            }
            else
            {
               // add to file inputs, initialize the inputs only when we're ready
               if (myInputs == null) myInputs = new List<string>();
               myInputs.Add(args[x]);
            }
         }
         return true;
      }

      static void ShowUsage()
      {
         Console.WriteLine();
         Console.WriteLine(string.Format("XmlToHtml v{0}: Authored by Alexander Miles", VERSION));
         Console.WriteLine();
         Console.WriteLine("This tool is used to convert XSL-styled XML documents to HTML.");
         Console.WriteLine("The XSL stylesheet needs to be created to output the data to the HTML format");
         Console.WriteLine();
         Console.WriteLine("Usage: xmltohtml [OPTIONS] [INPUTS]");
         Console.WriteLine("  Options:");
         Console.WriteLine("     -quiet   Do not output anything to the console or ask for input");
         Console.WriteLine("              Some output may still occur at certain times");
         Console.WriteLine("     -nopause   Do not wait for keypress before exiting the program");
         Console.WriteLine();
         Console.WriteLine("Example: xmltohtml -nopause example.xml");
         Console.WriteLine("Example: xmltohtml -quiet example.xml");
      }

      static void WriteLine(string s)
      {
         if (!myQuiet) Console.WriteLine(s);
      }
      static void WriteLine()
      {
         if (!myQuiet) Console.WriteLine();
      }
   }
}
