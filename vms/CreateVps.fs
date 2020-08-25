(*  Simple script to create the VPS resources in Azure using Farmer. *) 
open Farmer
open Farmer.Builders
open DotNetEnv
open System

[<EntryPoint>]
let main argv =

    Env.Load()
    let vmHostname = Env.GetString("VM_HOSTNAME")
    let vmUsername = Env.GetString("VM_USERNAME")
    let vmPassword = Env.GetString("VM_PASSWORD")
    let vmResourceName = Env.GetString("RESOURCE_NAME")
    let resourceGroup = vmResourceName + "-rg"

    let myVm = vm {
        name vmResourceName
        username vmUsername
        vm_size Vm.Standard_B2s
        operating_system (Vm.makeLinuxVm "0001-com-ubuntu-server-focal" "canonical" "20_04-lts")
        os_disk 64 Vm.Premium_LRS
        diagnostics_support
        domain_name_prefix (Some vmHostname) 

        // script will be executed by root
        // log file will be in /var/lib/waagent/custom-script/download/0/
        custom_script ("bash setup-vm.sh > setup-vm.log 2>&1; sudo usermod -aG docker " + vmUsername)
        custom_script_files [ "https://gist.githubusercontent.com/pviotti/f3aca8f5746add0a0ba1310de8ad7ad7/raw/3ec4eb38bdd9a2a67a561f02d18ec49e58c9c6ac/setup-vm.sh" ]
    }

    let deployment = arm {
        location Location.NorthEurope
        add_resource myVm 
    }

    deployment
    |> Writer.quickWrite vmHostname
    printfn "ARM template file written as %s.json." vmHostname

    let response =
        deployment
        |> Deploy.tryExecute resourceGroup [ "password-for-" + vmResourceName, vmPassword ]

    match response with
    | Ok outputs -> printfn "Success! Outputs: %A" outputs
    | Error error -> printfn "Failed! %s" error

    0
