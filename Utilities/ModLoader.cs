using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ZephyrusLauncher.Utilities
{
    public enum ModType
    {
        NONE,
        DLL_MOD,
        PAK_MOD,
        PATCH_MOD,
        DISABLED_MOD
    }

    public class Mod
    {
        public string ModFilePath { get; set; }
        public ModType ModType { get; set; }

        public Mod(string ModFilePath, ModType modType = ModType.DLL_MOD)
        {
            this.ModFilePath = ModFilePath;
            this.ModType = modType;
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(ModFilePath)
                .Replace(".disabled", "")
                .Replace(".dll", "")
                .Replace(".pak", ""); // this is so hacky but it may fix it
        }
    }

    public class ModLoader
    {
        private readonly string ModsPath;
        private readonly string FortniteBuildPath;

        public ModLoader(string modsPath, string fortniteBuildPath)
        {
            FortniteBuildPath = fortniteBuildPath;
            ModsPath = modsPath;
        }

        private void Inject(string dllPath, int processId)
        {
            IntPtr hProcess = Win32.OpenProcess(Win32.PROCESS_CREATE_THREAD | Win32.PROCESS_QUERY_INFORMATION | Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_WRITE | Win32.PROCESS_VM_READ, false, processId);

            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("Failed to open process.");
                return;
            }

            IntPtr addr = Win32.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(dllPath.Length + 1), Win32.MEM_COMMIT | Win32.MEM_RESERVE, Win32.PAGE_READWRITE);

            if (addr == IntPtr.Zero)
            {
                Console.WriteLine("Failed to allocate memory.");
                Win32.CloseHandle(hProcess);
                return;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(dllPath);

            if (!Win32.WriteProcessMemory(hProcess, addr, bytes, (uint)bytes.Length, out UIntPtr bytesWritten))
            {
                Console.WriteLine("Failed to write memory.");
                Win32.CloseHandle(hProcess);
                return;
            }

            IntPtr loadLibraryAddr = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (loadLibraryAddr == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get LoadLibraryA address.");
                Win32.CloseHandle(hProcess);
                return;
            }

            IntPtr hThread = Win32.CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, addr, 0, IntPtr.Zero);

            if (hThread == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create remote thread.");
                Win32.CloseHandle(hProcess);
                return;
            }

            Win32.CloseHandle(hThread);
            Win32.CloseHandle(hProcess);
        }

        public List<Mod> FetchMods()
        {
            List<Mod> mods = new List<Mod>();
            if (Directory.Exists(ModsPath))
            {
                foreach (string ModFile in Directory.GetFiles(ModsPath))
                {
                    Mod newMod = new Mod(ModFile);

                    if (ModFile.EndsWith(".dll"))
                        newMod.ModType = ModType.DLL_MOD;
                    else if (ModFile.EndsWith(".pak"))
                        newMod.ModType = ModType.PAK_MOD;
                    else if (ModFile.EndsWith(".zp"))
                        newMod.ModType = ModType.PATCH_MOD;

                    mods.Add(newMod);
                }
            }

            return mods;
        }

        public void ApplyPakMods(List<Mod> Mods)
        {
            string PakPath = Path.Combine(FortniteBuildPath, "FortniteGame\\Content\\Paks");

            string[] sigFiles = Directory.GetFiles(PakPath, "*.sig", SearchOption.AllDirectories);
            string smallestSig = sigFiles
                .Select(file => new FileInfo(file))
                .OrderBy(fileInfo => fileInfo.Length)
                .First().FullName;

            foreach (Mod mod in Mods)
            {
                if (File.Exists(mod.ModFilePath))
                {
                    if (mod.ModType == ModType.PAK_MOD)
                    {
                        string NewModPath = Path.Combine(PakPath, Path.GetFileName(mod.ModFilePath));
                        if (!File.Exists(NewModPath))
                        {
                            File.Copy(mod.ModFilePath, NewModPath);

                            var SigName = Path.GetFileNameWithoutExtension(mod.ModFilePath) + ".sig";
                            File.Copy(smallestSig, Path.Combine(PakPath, SigName));
                        }
                    }
                }
            }
        }

        public void RemovePakMods(List<Mod> Mods)
        {
            string PakPath = Path.Combine(FortniteBuildPath, "FortniteGame\\Content\\Paks");

            foreach (Mod mod in Mods)
            {
                if (File.Exists(mod.ModFilePath))
                {
                    if (mod.ModType == ModType.PAK_MOD)
                    {
                        string NewModPath = Path.Combine(PakPath, Path.GetFileName(mod.ModFilePath));
                        if (File.Exists(NewModPath))
                        {
                            File.Delete(NewModPath);

                            var sigPath = Path.Combine(PakPath, Path.GetFileNameWithoutExtension(mod.ModFilePath) + ".sig");
                            if (File.Exists(sigPath))
                                File.Delete(sigPath);
                        }
                    }
                }
            }
        }

        public void ApplyDLLMods(List<Mod> Mods, int FortniteProcessID)
        {
            foreach (Mod mod in Mods)
            {
                if (File.Exists(mod.ModFilePath))
                {
                    if (mod.ModType == ModType.DLL_MOD)
                        Inject(mod.ModFilePath, FortniteProcessID);
                }
            }
        }
    }
}