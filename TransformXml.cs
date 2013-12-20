using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XmlToHtml
{
   public static class TransformXml
   {
      public static string ToHtml(string infile, string outfile)
      {
         if (infile == null) throw new ArgumentNullException("infile");
         if (outfile == null) throw new ArgumentNullException("outfile");

         string returner = null;

         // stylesheet is determined as a processing instruction, must parse this from the XML
         try
         {
            XslCompiledTransform xform = new XslCompiledTransform();
            XmlDocument xml = new XmlDocument();
            xml.Load(infile);
            int start, length;
            XmlProcessingInstruction xpi = xml.SelectSingleNode("processing-instruction('xml-stylesheet')") as XmlProcessingInstruction;
            if (xpi != null)
            {
               start = xpi.Value.IndexOf("href='") + 6; // add 6 for the length of the "href='" string
               length = xpi.Value.LastIndexOf('\'') - start;
               xform.Load(Path.Combine(Path.GetDirectoryName(infile), xpi.Value.Substring(start, length))); // only load the XSL if the XML specifies it
            }

            XPathDocument data = new XPathDocument(infile); // read XPath information from the infile
            using (XmlTextWriter xt = new XmlTextWriter(outfile, Encoding.UTF8))
               xform.Transform(data, null, xt, null);
         }
         catch (Exception ex)
         {
            returner = ex.Message; // if there is an error during the transform, the stylesheet warning can be ignored for the time being.
         }

         return returner;
      }

      /// <summary>
      /// Converts several XSL styled XML sheets to HTML, and returns an array where each index is the output path of each path specified in 'xmlInputs', respective to the xmlInputs index. If 'xmlInputs' is null, this function will also return null.
      /// </summary>
      /// <param name="xmlInputs"></param>
      /// <returns></returns>
      public static string[] ToHtml(string[] xmlInputs)
      {
         if (xmlInputs != null)
         {
            string[] outputs = new string[xmlInputs.Length];
            int index = 0;
            foreach (string input in xmlInputs)
            {
               string output = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(input), System.IO.Path.GetFileNameWithoutExtension(input) + ".html");
               if (TransformXml.ToHtml(input, output) == null)
                  outputs[index] = output;
               index++;
            }
            return outputs; // any indices that are null did not successfully convert.
         }

         return null;
      }
   }
}
