using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FiveMExecutor
{
    public class ServerFileParser
    {
        public async Task<string> ParseServerFiles(string serverPath)
        {
            StringBuilder result = new StringBuilder();
            
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(serverPath, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        if (IsResourceFile(file))
                        {
                            string content = File.ReadAllText(file);
                            result.AppendLine($"File: {file}");
                            result.AppendLine(ParseResourceContent(content));
                        }
                    }
                    catch (Exception) { }
                }
            });

            return result.ToString();
        }

        private bool IsResourceFile(string filePath)
        {
            string[] resourceExtensions = { ".lua", ".json", ".xml", ".meta" };
            return Array.Exists(resourceExtensions, ext => filePath.EndsWith(ext));
        }

        private string ParseResourceContent(string content)
        {
            // Advanced parsing logic here
            return content;
        }
    }
}
