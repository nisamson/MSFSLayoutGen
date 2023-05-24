# MSFSLayoutGen: `layout.json` updater/generator for Microsoft Flight Simulator 2020

## Usage
```
MSFSLayoutGen.exe <path> [<path> ...]
```

### `<path>`:
Path to a directory containing a MSFS package, or a path to a `layout.json` file.

## Description

Updates or generates a `layout.json` file for a Microsoft Flight Simulator 2020 package, updating `manifest.json` if necessary.

Works similarly to other such tools, but is built against .NET Core, and so is not subject to file path length limitations.

## License
MSFSLayoutGen is licensed under the GPLv3 License. See [LICENSE](LICENSE.md) for the full license text.