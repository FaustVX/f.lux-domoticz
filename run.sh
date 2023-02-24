#! /bin/bash

echo stop service
sudo systemctl stop flux
echo build project
FOLDER=$(dotnet publish -c Release --sc -a arm --os linux -p:PublishSingleFile=true | tail -n 1 | awk '{print $3}')
echo copy to /usr/bin
sudo cp $FOLDER./flux /usr/bin
echo start service
sudo systemctl start flux
