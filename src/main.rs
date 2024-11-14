mod utils;

use std::{error::Error, fs, path::Path};
use utils::{download_file, launch_process, suspend_process};

fn main() {
    // Check if FortniteGame and Engine folder exists
    if !fs::exists("./FortniteGame").unwrap() {
        println!("This directory must have a FortniteGame folder!");
        return;
    }
    if !fs::exists("./Engine").unwrap() {
        println!("This directory must have an Engine folder!");
    }

    // Now, let's try to download the base mods
    if !fs::exists("./Mods").unwrap() {
        println!("Installing mods...");

        let new_mods_folder = fs::create_dir("./Mods");
        match new_mods_folder {
            Ok(file) => file,
            Err(error) => panic!("Problem creating Mods folder: {error:?}"),
        };

        // Now, I need to install mods
        let _ = match download_file("https://raw.githubusercontent.com/denalisun/FML-Mods/refs/heads/main/dll/CobaltDelayedInjection.dll", "./Mods/Redirector.dll") {
            Ok(()) => (),
            Err(err) => panic!("Could not download Redirector! Err: {}", err),
        };

        /*let _ = match download_file("url", "./Mods/zephyrus.pak") {
            Ok(()) => (),
            Err(err) => panic!("Could not download Zephyrus files! Err: {}", err),
        };*/
    }

    // Now I need to start actually launching the game which I cannot be asked to do rn
    let fn_eac_path = "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC.exe";
    let fn_launcher_path = "FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe";
    let fn_shipping_path = "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";

    let launch_args = "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -NOSSLPINNING -nobe -fromfl=eac -fltoken=7a848a93a74ba68876c36C1c -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ";

    if fs::exists(fn_launcher_path).unwrap() {
        let h = launch_process(fn_launcher_path, &launch_args).unwrap();
        suspend_process(&h);
    }
    /*if fs::exists(fn_eac_path).unwrap() {
        let h = launch_process(&fn_eac_path, &launch_args).unwrap();
        suspend_process(&h);
    }*/

    let mut h = launch_process(fn_shipping_path, &launch_args).unwrap();
    //suspend_process(&h);

    h.wait();
}