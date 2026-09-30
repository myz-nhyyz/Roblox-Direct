# RobloxDirect

Launcher Windows nho de mo link Roblox bang protocol `roblox://` cua ung dung Roblox.

## Tinh nang

- Ho tro link game va link invite/share cua Roblox.
- Tu dong tim cac Roblox client da dang ky trong Windows, khong gioi han ba launcher co dinh.
- Chon executable qua muc Custom neu client khac chua duoc tu nhan dien.
- Khong yeu cau quyen Administrator va khong xu ly thong tin dang nhap Roblox.

RobloxDirect can Roblox duoc cai va da dang ky protocol `roblox://` voi Windows.

## Build file EXE

Can Windows, .NET SDK va .NET Framework 4.8 Developer Pack/Targeting Pack. Mo PowerShell tai thu muc project va chay:

```powershell
dotnet restore .\RobloxDirect.csproj
dotnet build .\RobloxDirect.csproj -c Release -p:Platform=x64
```

EXE duoc tao tai:

```text
bin\x64\Release\net48\RobloxDirect.exe
```

Day la ung dung .NET Framework 4.8, khong phai ban self-contained. May chay can Windows co .NET Framework 4.8. Lenh publish single-file khong duoc .NET Framework ho tro.

## Source tren GitHub

Day la bo source Windows gon de dua len GitHub. Chi commit source, README, LICENSE va tai nguyen can de build; khong upload `bin/`, `obj/`, file `.user`, keystore, payload, hoac file EXE da build.

Trong VS Code, mo `RobloxDirect.csproj` tu thu muc goc cua bo source; hoac chay lenh build o tren trong PowerShell.

## Link duoc ho tro

- Game: `https://www.roblox.com/games/123456789/...`
- Invite/share: `https://www.roblox.com/share?code=...&type=Server`

Link game duoc chuyen thanh `roblox://experiences/start?placeId=...`; link invite duoc chuyen thanh `roblox://navigation/share_links?...`. RobloxDirect khong vuot qua protocol cua Roblox va khong the mo invite het han/bi thu hoi.

## Dong gop va giay phep

Pull request va bug report duoc hoan nghenh. Du an duoc phat hanh theo MIT License; xem [LICENSE](LICENSE).
