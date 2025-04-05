<div align="center">

<a href="https://github.com/ignYoqzii/StarZInjector/graphs/contributors">![Contributors](https://img.shields.io/github/contributors/ignYoqzii/StarZInjector?style=flat)</a>
<a href="https://github.com/ignYoqzii/StarZInjector/network/members">![Forks](https://img.shields.io/github/forks/ignYoqzii/StarZInjector?style=flat)</a>
<a href="https://github.com/ignYoqzii/StarZInjector/stargazers">![Stargazers](https://img.shields.io/github/stars/ignYoqzii/StarZInjector?style=flat)</a>
<a href="https://github.com/ignYoqzii/StarZInjector/issues">![Issues](https://img.shields.io/github/issues/ignYoqzii/StarZInjector?style=flat)</a>
<a href="https://github.com/ignYoqzii/StarZInjector/blob/main/LICENSE">![License](https://img.shields.io/github/license/ignYoqzii/StarZInjector?style=flat)</a>

</div>

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/ignYoqzii/StarZInjector">
    <img src="/Assets/StarZInjector.png" alt="Logo" width="1386" height="250">
  </a>

  <h3 align="center">StarZ Injector</h3>

  <p align="center">
    StarZ Injector is a .NET 9.0 Windows application designed for DLL injection into target processes. It leverages WPF UI (WinUI 3 for WPF) for the user interface and includes features such as theme customization, Discord Rich Presence, and various injection methods.
    <br />
  </p>
</p>

## Features

- **DLL Injection**: Supports multiple injection methods including `LoadLibraryA`. More methods might be added in the future.
- **Theme Customization**: Switch between Light and Dark themes.
- **Discord Rich Presence**: Display custom status messages on Discord.
- **Configuration Management**: Easily manage settings through a configuration file.
- **Dependency Injection**: Utilizes .NET Generic Host for dependency injection and configuration.

## Requirements

- .NET 9.0 SDK
- Windows 7 or later

## Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Installation

1. Clone the repository:
```
git clone https://github.com/ignYoqzii/StarZInjector.git
```

2. Open the solution in Visual Studio 2022.

3. Restore the NuGet packages:

```
dotnet restore
```

4. Build the solution:

```
dotnet build
```


## Configuration

The application uses a `Config.txt` file located in the `Documents\StarZ Injector` directory. The configuration file includes settings such as:

- `DiscordRPC`: Enable or disable Discord Rich Presence.
- `DiscordRPCStatus`: Custom status message for Discord.
- `Theme`: Application theme (`Light` or `Dark`).
- `ExeName`: Name of the target executable for injection.
- `DllPath`: Path to the DLL to be injected.
- `InjectionMethod`: Method used for injection (`LoadLibraryA`).
- `AutoInject`: Enable or disable automatic injection.
- `InjectionDelay`: Delay before injection in seconds.

## Usage

2. Run the application.
3. Use the UI to start the injection process.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [MicaWPF](https://github.com/Simnico99/MicaWPF) for additional UI effects.
- [WPF-UI](https://github.com/lepoco/wpfui) for the modern UI components.
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/MVVM) for MVVM support.
