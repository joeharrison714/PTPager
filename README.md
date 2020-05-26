# PTPager

## Setup AWS Access
1. Create a new IAM user called polly-tts
1. Allow programatic access
1. Give it `AmazonPollyReadOnlyAccess` permissions
1. Save access key and secret access key for use later

## Setup of Raspberry Pi Machine
1. On a windows box, download and install the "Windows 10 IoT Core Dashboard"
1. Image SD Card
    1. Open IoT Dashboard and click `Setup a new Device`
    1. Select Raspberry Pi, Windows 10 IoT Core
    1. For device name, use ptpager
    1. Set an admin password
    1. Click Download and Install
1. Boot the pi with the new SD Card and go through welcome screens
1. Install .NET Core:
    1. From windows box, open `\\ptpager\c$` and login with administrator and the password you specified
    1. Create c$/dotnet
    1. Download SDK for ARM32 and unzip to that folder from `https://dotnet.microsoft.com/download/dotnet/current`
       - `https://download.visualstudio.microsoft.com/download/pr/048302c4-d583-4a31-acba-fdf85d0ebad7/8d33b36319286e27463e9e3fe1d46597/dotnet-sdk-3.1.300-win-arm.zip`
    1. Download asp.net runtime for ARM32 and unzip to that folder
       - `https://download.visualstudio.microsoft.com/download/pr/3bb0854c-541c-46c1-9efa-f26e9dfc701b/b53fb590dca38a967f1b1e12a5c10165/aspnetcore-runtime-3.1.4-win-arm.zip`
1. Back in IoT Dashboard and click `My Devices`. Right Click and Click `Launch Powershell`
1. Test dotnet by running `C:\dotnet\dotnet.exe --version`
1. Create \\ptpager\c$\credentials.txt with credentials from the aws setup. Format is:
   ```
   [pollyaccess]
   aws_access_key_id = <value>
   aws_secret_access_key = <value>
   ```
1. Deploy app:
   1. On windows machine, download this repo
   1. Edit appsettings.Production.json
      - Fill out `bindingIp` to ip of Pi
      - Fill out ClientSecret from smartthings app
   1. From PTPagerServer\PTPager.Web3 folder, execute:
      - `dotnet publish PTPager.Web3.csproj /p:PublishProfile=PiProfile`