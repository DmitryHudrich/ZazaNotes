
# ZazaNotes deployment

## Easy way to deploy on windows with docker

- Install docker: https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe?utm_source=docker&utm_medium=webreferral&utm_campaign=dd-smartbutton&utm_location=module
- Run docker as an administrator
- Run cmd as administrator: `curl -o docker-compose.yml https://raw.githubusercontent.com/DmitryHudrich/ZazaNotes/main/Zaza.Web/docker-compose.yml && docker compose up -d --build`
##  Windows without docker
- Install dotnet and mongo:
	- **_Mongodb:_** https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-7.0.7-signed.msi
	- **_.NET:_** https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.203-windows-x64-installer
-  Download repo:
	-  **_With git:_** `git clone https://github.com/DmitryHudrich/ZazaNotes.git`
	-  Or just download as zip...
-  Run server:
	-  `cd ZazaNotes/Zaza.Web/`
	-  `dotnet run --no-launch-profile --mongo default --swagger `
	- Congrats!

## Linux
### Arch:
- Install docker: 
	- Install package: `sudo pacman -S docker-compose`
	- Enable daemon: `systemctl enable docker.socket && systemctl start docker.socket`
- Deploy: `$ wget https://raw.githubusercontent.com/DmitryHudrich/ZazaNotes/main/Zaza.Web/docker-compose.yml -O docker-compose.yml && docker compose up -d --build`

add --h flag for help
