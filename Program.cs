using System.Net;
using ZephyrusLauncher.Utilities;

if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Mods"))) {
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Mods"));
    using (WebClient client = new WebClient()) {
        client.DownloadFile("https://raw.githubusercontent.com/denalisun/FML-Mods/refs/heads/main/dll/CobaltDelayedInjection.dll", Path.Combine(Directory.GetCurrentDirectory(), "Mods/Redirector.dll"));
    }
}

Launcher launcher = new Launcher(Directory.GetCurrentDirectory());
launcher.Launch(Path.Combine(Directory.GetCurrentDirectory(), "Mods"), "");