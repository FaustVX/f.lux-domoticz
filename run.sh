sudo systemctl stop flux
dotnet publish -c Release --sc -a arm --os linux -p:PublishSingleFile=true
sudo cp bin/Release/net7.0/linux-arm/publish/flux /usr/bin
sudo systemctl start flux
