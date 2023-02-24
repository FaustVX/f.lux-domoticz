#! /bin/bash

sudo systemctl stop flux
FOLDER=$(dotnet publish -c Release --sc -a arm --os linux -p:PublishSingleFile=true | tail -n 1 | awk '{print $3}')
echo $FOLDER
sudo cp $FOLDER./flux /usr/bin
sudo systemctl start flux
