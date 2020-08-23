#!/bin/bash

echo "Update packages and install utilities..."
sudo apt update
sudo apt upgrade -y
sudo apt install -y vim htop screen git make

echo "Install Docker..."
curl -sSL https://get.docker.com/ | sh
sudo usermod -aG docker $USER

echo "Install Docker Compose..."
sudo curl -L "https://github.com/docker/compose/releases/download/1.26.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

echo "Setup firewall..."
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable

echo "Done! (remember to set up password-less authentication!)"