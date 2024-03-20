# ZazaNotes deployment

## Easy way to deploy on windows with docker

### 1. Install docker and  wget
- **_with chocolatey:_** 
	- Run cmd as administrator 
	- Install chocolatey (if not already installed):
`@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"`
`choco install docker-desktop wget`
- **_or without chocolatey_**:
	- wget: http://downloads.sourceforge.net/gnuwin32/wget-1.11.4-1-setup.exe
	docker: https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe?utm_source=docker&utm_medium=webreferral&utm_campaign=dd-smartbutton&utm_location=module
### 3. Run docker desktop as administrator
### 4. Deploy!
- Run cmd as administrator
- Enter: `wget https://raw.githubusercontent.com/DmitryHudrich/ZazaNotes/main/Zaza.Web/docker-compose.yml && docker compose up -d`
##  Windows without docker
### 1. Download repo:
-  **_With git:_** `git clone https://github.com/DmitryHudrich/ZazaNotes.git`
-  Or just download as zip...
### 2. Install dotnet and mongo:
- **_Mongodb:_** https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-7.0.7-signed.msi
- **_.NET runtime:_** https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.3-windows-x64-binaries
### 3. Run server:
- Run cmd
- Enter: `dotnet ZazaNotes/Zaza.Web/bin/Release/net8.0/publish/Zaza.Web.dll`
- Congrats!

## Linux
### Arch:
- Install docker: 
	- Install package: `sudo pacman -S docker-compose`
	- Enable daemon: `systemctl enable docker.socket && systemctl start docker.socket`
- Deploy: `$ wget https://raw.githubusercontent.com/DmitryHudrich/ZazaNotes/main/Zaza.Web/docker-compose.yml && docker compose up -d`


