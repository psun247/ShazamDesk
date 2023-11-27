# WpfShazam
This is a C# WPF app that identifies songs like the popular mobile app Shazam and saves song info in Azure SQL DB via Web API or in SQL Server DB.

# Shazam tab


# Azure (Web API) tab


# SQL Server tab


# Build
Build CSharpWpfShazam.sln with Visual Studio Professional 2022 (64-bit) or Visual Studio Community 2022 (64-bit).  This app is targeted for .NET 7. To compile for .NET 6, simply modify WpfShazam.csproj.

# Run
To run the app without compiling it,
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
