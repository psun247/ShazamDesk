# WpfShazam
This is a C# WPF app that identifies songs like the popular mobile app Shazam and saves song info in Azure SQL DB via Web API / gRPC service or in SQL Server DB. 

# ChatGPT tab
C# WPF version of ChatGPT using OpenAI API key. This is independent of the rest of the app.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/612c1854-f299-4321-850e-7b78513f3803)

# Shazam tab
Listen to a device (mic or speaker) selected from the dropdown list.  When identified, a list of songs will be displayed, plus lyrics (if found) will be shown on the right side of the screen.  Usually, you could select a song from the list to save it with the lyrics in Azure SQL DB (via Web API or gRPC service) or SQL Server DB.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/3baece02-17a9-44ab-a35f-79e8205333c2)

You can open currently selected YouTube video externally with default web browser.

# Azure (Web API) tab
Saved song info list in Azure SQL DB (via Web API or gRPC service) will be displayed in the left panel. When an item is selected in the list, the matching YouTube video will display on the right side.

You can delete a selected item in the list via Web API or gRPC service.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/b1708d2a-a5a8-467d-9710-294ef766ca0f)

# SQL Server tab
Similar to Azure tab, but data store is a local SQL Server DB.  Note: SQL Server needs to be installed and configured properly, and connection string change in SqlServerContext.cs will be required.

Since SQL Server is assumed not installed, default mode on this tab is Demo that shows a predefined read-only list.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/fc799b1b-06b3-4402-8cb1-a0177419d990)

# Build
Build WpfShazam.sln with Visual Studio Professional 2022 (64-bit) or Community version.  This app is targeted for .NET 6 and 8. If .NET 8 is not installed on your computer, remove net8.0 from WpfShazam.csproj and ShazamCore.csproj and compile the WpfShazam.sln.

# Run
To run WpfShazam app without compiling it,
1. Click WpfShazam_v1.0 under Releases on the right side of this page
2. Download WpfShazam_v1.0_net6.0-windows.zip
3. Unzip the file and run WpfShazam.exe

# Usage
Audio devices will be automatically queried and displayed in the dropdown list.  You will need to select a proper device for 'Listen to'.  Add and Delete buttons are for Azure SQL DB (via Web API or gRPC service) or SQL Server DB. The blue arrow on the right side of the screen will expand or collapse the song info section.

# WinUI3Shazam
This is WinUI 3 version of WfpShazam. For details, run WinUI3Shazam.sln and see source code in WinUI3Shazam folder.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/17a11527-d3b7-4ee6-939a-0d5d1c303e4b)

# Supporting libraries
CommunityToolkit.Mvvm
 
https://www.nuget.org/packages/CommunityToolkit.Mvvm

Microsoft Authentication Library (MSAL) for .NET (formerly named Azure AD)

https://www.nuget.org/packages/Microsoft.Identity.Client/
 
ModernWpfUI
 
https://www.nuget.org/packages/ModernWpfUI/

NAudio

https://www.nuget.org/packages/NAudio

Whetstone.ChatGPT

https://www.nuget.org/packages/Whetstone.ChatGPT
 
RestoreWindowPlace

https://www.nuget.org/packages/RestoreWindowPlace
