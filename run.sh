sudo systemctl stop flux
dotnet publish -c Release --sc -a arm --os linux -p:PublishSingleFile=true
sudo systemctl start flux
