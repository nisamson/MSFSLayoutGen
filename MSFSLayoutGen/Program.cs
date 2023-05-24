#region license

// MSFSLayoutGen - MSFSLayoutGen - Program.cs
// Copyright (C) 2023 Nicholas Samson
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

namespace MSFSLayoutGen;

public static class Program {
    public static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: MSFSLayoutGen.exe <path> [<path> ...]\nMSFSLayoutGen.exe is licensed under the GNU GPLv3.");
            return;
        }
        
        foreach (var path in args) {
            Console.WriteLine($"Generating layout.json for {path}");
            var realPath = GetPackageDir(path);

            Environment.CurrentDirectory = realPath;
            if (File.Exists("layout.json")) {
                File.Copy("layout.json", "layout.json.bak", true);
            }
            
            var layout = Layout.FromCurrentDir();
            using (var stream = File.OpenWrite("layout.json")) {
                Json.Write(stream, layout);
            }
            Console.WriteLine("Generated layout.json");

            if (!File.Exists("manifest.json")) {
                continue;
            }

            Console.WriteLine("Updating manifest.json if necessary");
            var updated = layout.UpdateManifestTotalSize();
            if (updated) {
                Console.WriteLine($"Updated manifest.json: total_package_size = {layout.TotalSize}");
            } else {
                Console.WriteLine("manifest.json was not changed because no total_package_size was specified");
            }
        }
        
        Console.WriteLine($"Processed {args.Length} packages");
    }

    private static string GetPackageDir(string path) {
        string realPath;

        var finfo = new FileInfo(path);
        if (finfo.Exists && path.EndsWith("layout.json", StringComparison.OrdinalIgnoreCase)) {
            realPath = finfo.Directory!.FullName;
        } else if (new DirectoryInfo(path).Exists) {
            realPath = path;
        } else {
            throw new FileNotFoundException("Non-existent path", path);
        }

        return realPath;
    }
}
