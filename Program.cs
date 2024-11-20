using System.Net;
using ZephyrusLauncher.Utilities;

// Ik i should use the HttpClient but
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Mods"))) {
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Mods"));
    using (WebClient client = new WebClient()) { // TODO: Remove WebClient and use HttpClient
        client.DownloadFile("https://raw.githubusercontent.com/denalisun/FML-Mods/refs/heads/main/dll/CobaltDelayedInjection.dll", Path.Combine(Directory.GetCurrentDirectory(), "Mods/Redirector.dll"));

        //TODO: Implement Zephyrus pak
        //client.DownloadFile("rizz", Path.Combine(Directory.GetCurrentDirectory(), "Mods/Zephyrus_P.pak"));
    }
}

Launcher launcher = new Launcher(Directory.GetCurrentDirectory());
launcher.Launch(Path.Combine(Directory.GetCurrentDirectory(), "Mods"), "-AUTH_LOGIN=Zephyr@lol.rizz -AUTH_PASSWORD=12345zephy -AUTH_TYPE=epic");