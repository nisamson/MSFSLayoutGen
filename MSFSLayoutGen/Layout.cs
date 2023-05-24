#region license

// MSFSLayoutGen - MSFSLayoutGen - Layout.cs
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

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MSFSLayoutGen;

public class Layout {
    private const string TotalPackageSizeKey = "total_package_size";

    private static readonly IReadOnlyList<string> IgnoredFiles = new[] {
        "layout.json",
        "layout.json.bak",
        "business.json",
        "manifest.json",
    };

    [JsonIgnore]
    public long TotalSize => Contents.Sum(x => x.Size);
    
    [JsonIgnore]
    public string ManifestTotalSizeString => TotalSize.ToString().PadLeft(20, '0');

    [JsonPropertyName("content")] public ISet<LayoutFile> Contents { get; } = new SortedSet<LayoutFile>();

    public static Layout FromCurrentDir() {
        var layout = new Layout();
        var currentDir = Directory.GetCurrentDirectory();
        var files = Directory.EnumerateFiles(currentDir, "*", SearchOption.AllDirectories);
        foreach (var file in files) {
            var relativePath = Path.GetRelativePath(currentDir, file);
            if (ShouldNotBeInLayoutFile(relativePath)) {
                continue;
            }
            layout.Contents.Add(LayoutFile.FromFile(relativePath));
        }

        return layout;
    }

    public static bool ShouldNotBeInLayoutFile(string path) {
        return IgnoredFiles.Contains(path, StringComparer.OrdinalIgnoreCase) || path.StartsWith("_CVT_", StringComparison.OrdinalIgnoreCase);
    }

    public static bool ShouldBeInLayoutFile(string path) {
        return !ShouldNotBeInLayoutFile(path);
    }

    /// <summary>
    /// </summary>
    /// <returns>true if the manifest was updated, false otherwise.</returns>
    /// <exception cref="Exception"></exception>
    public bool UpdateManifestTotalSize() {
        JsonNode manifest;
        using (var stream = File.OpenRead("manifest.json")) {
            manifest = JsonNode.Parse(stream) ?? throw new Exception("manifest.json is not valid JSON");
        }

        if (manifest is not JsonObject) {
            throw new Exception("manifest.json is not a JSON object");
        }

        var root = manifest.AsObject();
        if (!root.ContainsKey(TotalPackageSizeKey)) {
            return false;
        }

        root[TotalPackageSizeKey] = ManifestTotalSizeString;
        using (var stream = File.OpenWrite("manifest.json")) {
            Json.Write(stream, manifest);
        }

        return true;
    }
}

public readonly record struct LayoutFile(
    string Path,
    long Size,
    [property: JsonPropertyName("date")] long Timestamp) : IComparable<LayoutFile>{
    /// <remarks>
    /// Assumes file is in correct location relative to current working directory.
    /// </remarks>
    /// <param name="path"></param>
    /// <returns></returns>
    public static LayoutFile FromFile(string path) {
        var fileInfo = new FileInfo(path);
        return new LayoutFile(path.Replace('\\', '/'), fileInfo.Length, fileInfo.LastWriteTimeUtc.ToFileTimeUtc());
    }

    public int CompareTo(LayoutFile other) {
        var pathComparison = string.Compare(Path, other.Path, StringComparison.OrdinalIgnoreCase);
        if (pathComparison != 0) {
            return pathComparison;
        }

        var sizeComparison = Size.CompareTo(other.Size);
        if (sizeComparison != 0) {
            return sizeComparison;
        }

        return Timestamp.CompareTo(other.Timestamp);
    }
}
