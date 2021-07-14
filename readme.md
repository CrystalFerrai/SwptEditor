# SwptSaveEditor

This project is an unofficial save game editor for the game "She Will Punish Them" by L2 Games. This product and its creators are not affiliated with L2 Games. I am not responsible for any damage caused by use of this program. See the included license.txt for licensing details. Use of this product constitutes acceptance of the license terms.

## Releases

Releases can be found over [here](https://github.com/CrystalFerrai/SwptEditor/releases).

You may need to install the [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48) if you do not already have it.

Some virus scanners assume that any unsigned software is potentially a virus. I do not have a software signing certificate and do not wish to purchase one. This product does not do anything intentionally malicious, but all I can offer you is my word on that. The only damage that may be caused by the program is to your save game files. I strongly recommend backing them up before editing them with this tool.

## How to Build

This project is built in Visual Studio 2019 using WPF .NET Framework 4.8. `SwptSaveEditor.sln` is the primary solution file. Build the solution from within Visual Studio or via Visual Studio command line build tools.

This project depends on some third party libraries which are not included. These can be easily obtained by opening the solution in Visual Studio, right-clicking the solution (top level node) in the Solution Explorer view, and selecting "Restore NuGet Packages". For more information about NuGet package restore, see [this article](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore).

These are the specific third-party libraries in use:

* [Microsoft-WindowsAPICodePack-Core](https://www.nuget.org/packages/Microsoft-WindowsAPICodePack-Core)
* [Microsoft-WindowsAPICodePack-Shell](https://www.nuget.org/packages/Microsoft-WindowsAPICodePack-Shell)