sudo cp ./flux.service /etc/systemd/system/flux.service
sudo chmod 644 /etc/systemd/system/flux.service
sudo systemctl enable flux.service
sudo systemctl start flux.service
