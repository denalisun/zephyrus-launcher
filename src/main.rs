use std::fs;

fn main() {
    // Check if FortniteGame folder exists
    if !fs::exists("./FortniteGame").unwrap() {
        println!("This directory must have a FortniteGame folder!");
        return;
    }

    // Now, let's try to download the base mods
    if !fs::exists("./Mods").unwrap() {
        println!("Installing mods...");

        let new_mods_folder = fs::create_dir("./Mods");
        match new_mods_folder {
            Ok(file) => file,
            Err(error) => panic!("Problem creating Mods folder: {error:?}"),
        };
    }
}