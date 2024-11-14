use std::{error::Error, fs::File};
use reqwest;
use winapi::um::processthreadsapi::{OpenProcess, SuspendThread, ResumeThread};
use winapi::um::handleapi::CloseHandle;
use winapi::um::winnt::PROCESS_SUSPEND_RESUME;
use std::io::Write;
use std::process::{Command, Child};

pub fn download_file(url: &str, path: &str) -> Result<(), Box<dyn Error>> {
    let res = reqwest::blocking::get(url)?.text()?;
    let mut file = File::create_new(path)?;

    let _ = file.write_all(res.as_bytes());

    Ok(())
}

pub fn launch_process(path: &str, args: &str) -> std::io::Result<Child> {
    let child = Command::new(path)
        .arg(args.clone())
        .current_dir(".\\FortniteGame\\Binaries\\Win64")
        .spawn()?;
    Ok(child)
}

pub fn suspend_process(child: &Child) {
    unsafe {
        let handle = OpenProcess(PROCESS_SUSPEND_RESUME, 0, child.id());
        if !handle.is_null() {
            SuspendThread(handle);
            CloseHandle(handle);
        }
    }
}

fn resume_process(child: &Child) {
    unsafe {
        let handle = OpenProcess(PROCESS_SUSPEND_RESUME, 0, child.id());
        if !handle.is_null() {
            ResumeThread(handle);
            CloseHandle(handle);
        }
    }
}