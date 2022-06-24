using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutomationControls
{
    public class Paths
    {
        public static string GeneratePath(CodexData data)
        {
            return Path.Combine(GeneratedFilesPath, data.csNamespaceName, data.className);
        }
        public static string GeneratedFilesPath = @"C:\CodeGen"; 
    }
}
