# WpfShazam
This is a C# WPF app that identifies songs like the popular mobile app Shazam and saves song info in Azure SQL DB via Web API or in SQL Server DB.

# WinUI3Shazam
This is WinUI 3 version. For details, run WinUI3Shazam.sln and see source code in WinUI3Shazam folder.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/b9b03b42-5027-4647-99b7-2de63d29a124)

# Shazam tab (WpfShazam UI)
Listen to a device (mic or speaker) selected from the dropdown list.  When identified, a list of songs will be displayed, plus lyrics (if found) will be shown on the right side of the screen.  Usually, you could select a song from the list to save it with the lyrics in Azure SQL DB (via Web API) or SQL Server DB.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/999bdeba-905e-446f-ad74-ab226ea1351a)

You can open currently selected YouTube video externally with default web browser.

# Azure (Web API) tab
Saved song info list in Azure SQL DB (via Web API) will be displayed in the left panel. When an item is selected in the list, the matching YouTube video will display on the right side.

You can delete a selected item in the list via Web API.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/07d53b2e-c85a-47c1-836a-40780d98e5b2)

# SQL Server tab
Similar to Azure tab, but data store is a local SQL Server DB.  Note: SQL Server needs to be installed and configured properly, and connection string change in SqlServerContext.cs will be required.

Since SQL Server is assumed not installed, default mode on this tab is Demo that shows a predefined read-only list.

![image](https://github.com/psun247/ShazamDesk/assets/31531761/a7d714a0-6141-417a-9707-01143952bab9)

# Build
Build WpfShazam.sln with Visual Studio Professional 2022 (64-bit) or Community version.  This app is targeted for .NET 7. To compile for .NET 6, simply modify WpfShazam.csproj.

# Run
To run WpfShazam app without compiling it,
1. Click WpfShazam_v1.0 under Releases on the right side of this page
2. Download WpfShazam_v1.0_net6.0-windows.zip
3. Unzip the file and run WpfShazam.exe

# Usage
Audio devices will be automatically queried and displayed in the dropdown list.  You will need to select a proper device for 'Listen to'.  Add and Delete buttons are for Azure SQL DB (via Web API) or SQL Server DB. The blue arrow on the right side of the screen will expand or collapse the song info section.

# Supporting libraries
CommunityToolkit.Mvvm
 
https://www.nuget.org/packages/CommunityToolkit.Mvvm

Microsoft Authentication Library (MSAL) for .NET (formerly named Azure AD)

https://www.nuget.org/packages/Microsoft.Identity.Client/
 
ModernWpfUI
 
https://www.nuget.org/packages/ModernWpfUI/

NAudio

https://www.nuget.org/packages/NAudio
 
RestoreWindowPlace

https://www.nuget.org/packages/RestoreWindowPlace
