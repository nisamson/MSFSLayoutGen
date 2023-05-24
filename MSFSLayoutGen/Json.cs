#region license

// MSFSLayoutGen - MSFSLayoutGen - Json.cs
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

using System.Text;
using System.Text.Json;

namespace MSFSLayoutGen;

public static class Json {
    public static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public static void Write<T>(Stream dest, T obj) {
        var jsonText = JsonSerializer.Serialize(obj, Options);
        jsonText.ReplaceLineEndings("\n");
        var bytes = Encoding.UTF8.GetBytes(jsonText);
        dest.Write(bytes);
    }
}
