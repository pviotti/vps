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
        // the following doesn't seem to work - https://github.com/CompositionalIT/farmer/issues/319
        //custom_script_files [ "https://gist.githubusercontent.com/pviotti/f3aca8f5746add0a0ba1310de8ad7ad7/raw/e22dd35e75716df0ffec2f0d4d1edaa769725ccc/setup-vm.sh" ]
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
