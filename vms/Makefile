SHELL := /bin/bash

deploy:
	dotnet run

delete:
	@read -p "Enter resource group name: " rg; \
	az group delete --name $$rg