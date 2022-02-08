using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    class Program
    {
        // public void CreateEntry(string fileName, string npcName, string lineToAdd) //npcName = "item1"
        // {
        //     var endTag = "this.add.image";
        //
        //     var txtLines = File.ReadAllLines(fileName).ToList();   //Fill a list with the lines from the txt file.
        //     txtLines.Insert(txtLines.IndexOf(endTag), lineToAdd);  //Insert the line you want to add last under the tag 'item1'.
        //     File.WriteAllLines(fileName, txtLines);                //Add the lines including the new one.
        // }
        
        static void Main(string[] args)
        {
            string dirPath = Console.ReadLine();
            string fileContent;
            string jsonFileContent;
            string pathName;
            List<string> filewithimage;
            List<string> jsonFileWithPath;
            string[] files = Directory.EnumerateFiles(dirPath, "*.js", SearchOption.AllDirectories).Where(d => (!d.StartsWith(dirPath+"\\node_modules")) && (!d.StartsWith(dirPath+"\\webpack.config.js"))&& (!d.StartsWith(dirPath+ "\\dist"))).ToArray();
            string[] JSONFiles = Directory.GetFiles(dirPath, "*.json", SearchOption.AllDirectories).Where(d => (!d.StartsWith(dirPath+"\\node_modules"))&&(!d.StartsWith(dirPath+"\\dist"))).ToArray();
            string JSONstring;
            foreach (string JSFile in files)
            {
                using (StreamReader sr = new StreamReader(JSFile))
                {
                    int i = 0;
                    while (sr.Peek() >=0)
                    {
                        i++;
                        fileContent = sr.ReadLine();
                        if (fileContent.Contains("this.add.image"))
                        {
                            
                            filewithimage = File.ReadAllLines(JSFile).ToList();
                            var imgIndex = filewithimage.FindIndex(a => a.Contains("this.add.image"));
                            var usedString = filewithimage[imgIndex];
                            var fIndexOf = usedString.IndexOf("\"");
                            var lIndexOf = usedString.Substring(fIndexOf).IndexOf("\"");
                            JSONstring = usedString.Substring(fIndexOf).Substring(0,lIndexOf-1);
                            filewithimage.Insert(imgIndex,"text.textures.addBase64(\""+ JSONstring + "\"," + JSONstring + "image);");
                            File.WriteAllLines(JSFile,filewithimage);
                            foreach (string JSONFile in JSONFiles)
                            {
                                using (StreamReader srr = new StreamReader(JSONFile))
                                {
                                    int j = 0;
                                    while (srr.Peek() >=0)
                                    {
                                        j++;
                                        jsonFileContent = srr.ReadLine();
                                        if (jsonFileContent.Contains("\"key\": \"" + JSONstring+ "\""))
                                        {
                                            jsonFileWithPath = File.ReadAllLines(JSONFile).ToList();
                                            var keyIndex = filewithimage.FindIndex(a => a.Contains("\"key\": \"" + JSONstring+ "\""));
                                            var path = jsonFileWithPath[keyIndex - 2];
                                            var ffIndexOf = path.IndexOf(":");
                                            ffIndexOf = path.Substring(ffIndexOf).IndexOf("\"");
                                            pathName = path.Substring(ffIndexOf, path.IndexOf(",")-ffIndexOf);
                                            jsonFileWithPath.Insert(0,"import " + JSONstring + "image from \"../../static/" + pathName + "\"");
                                            File.WriteAllLines(JSONFile,jsonFileWithPath);
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
        }
    }
}