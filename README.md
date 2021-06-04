# VPS

This repo holds scripts and configuration files to [self-host] some web services
such as [Bitwarden] and [Nextcloud] on a private server.
The goal is to have a *simple* (as in: concise, programmatic and declarative), 
cheap and secure setup to handle file synchronization
and credential management for a few users (e.g. <10).

> *There exist several tutorials about self-hosting those apps, 
but none of them matches my requirements, so I'm publishing this repo 
in case it's useful to anyone.*

## VPS on Azure

> Prerequisites: [Azure CLI][azure-cli] (select the right Azure subscription: 
`az login; az account set --subscription "NameOfSubscription"`);
[.NET core 3.1][dotnet-core].
  
In the `vms` folder is a [Farmer] script that creates a virtual machine 
on Azure with this specs:
 - [SKU][azure-vm-sku]: Standard B2s 2vCPUs, 4GB RAM, 60GB SSD (~20€/mo as of 8/2020)
 - [region][azure-regions]: North Europe
 - OS: Ubuntu 20.04

To create the virtual machine, change directory to `vms` and:
 1. copy `env.example` to `.env` and edit it as suitable for 
 username, password, host and resource name
 2. issue: `make deploy`. The script will deploy the VM and 
 generate the related ARM template json file. 
 A setup script similar to `setup-vm.sh` will be executed upon deployment 
 to install required tools (e.g. Docker, etc)
 3. setup passwordless authentication
    - copy your public key to the VM: `ssh-copy-id -i ~/.ssh/mypub.key user@server`
    - editing the following settings in `/etc/ssh/sshd_config` on the VM: `PasswordAuthentication no`;
    `ChallengeResponseAuthentication no`; `UsePAM no`.
    Then restart sshd: `sudo systemctl restart ssh`.
 4. set up start and stop VM automation during off hours as described [here][vm-automation], and make the VM IP static (*TODO: automate*)

## Applications

> Prerequisites:
this setup assumes you own a DNS domain, and your made its
`A Record`s for naked domain (`@`) and subdomains (`*`) 
point to the VM's public IP. 
Failing that, you'll still be able to run the applications, 
but Caddy will have issues creating the certificates to use 
for the HTTPS connections. 
Notice that while Azure virtual machine have a public DNS 
name (e.g. `<name>.<region>.cloudapp.azure.net`), their DNS setting 
does not allow using subdomains, so it won't work.

`apps` directory contains the Docker Compose file
to run Bitwarden and Nextcloud (with its MariaDB database) behind [Caddy] reverse proxy.  
At the end of the instructions 
 - Nextcloud will be reachable at `https://nc.<your domain>` and `https://<your domain>`
 - Bitwarden will be reachable at `https://bw.<your domain>`

To deploy the applications:
  1. copy the app directory to your server (or clone this repo)
  2. change to `apps` folder, copy `env.example` to `.env` and edit it as suitable 
  3. run `make up`. You can follow the progress of the setup by issuing `make log`.

## Maintenance

### Applications upgrade

To upgrade the applications just issue:

    docker-compose pull
    docker-compose down
    docker-compose up -d
  
Or, more cautiously, issue the same commands but for one application at a time, 
e.g.`docker-compose pull nextcloud`.  
Beware that some applications require additional steps when upgrading 
between major versions, so make sure to read their upgrade documentation too.

## :construction_worker: To do

 - add instructions for adding Prometheus and Graphana to monitor
 host VM, Docker and applications
 - add instructions for backup
 - automate the remaining manual steps of VM creation

## References

 - [best practices for Docker Compose][docker-compose]

 [azure-vm-sku]: https://docs.microsoft.com/en-us/azure/virtual-machines/sizes
 [azure-regions]: https://azure.microsoft.com/en-us/global-infrastructure/geographies/#overview
 [vm-automation]: https://docs.microsoft.com/en-us/azure/automation/automation-solution-vm-management-enable
 [bitwarden]: https://bitwarden.com/
 [nextcloud]: https://nextcloud.com/
 [self-host]: https://en.wikipedia.org/wiki/Self-hosting_(web_services)
 [azure-cli]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
 [dotnet-core]: https://dotnet.microsoft.com/download/dotnet-core/3.1
 [farmer]: https://compositionalit.github.io/farmer/
 [caddy]: https://caddyserver.com/
 [docker-compose]: https://nickjanetakis.com/blog/best-practices-around-production-ready-web-apps-with-docker-compose
