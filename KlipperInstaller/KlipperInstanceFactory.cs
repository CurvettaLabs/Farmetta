using LibGit2Sharp;

namespace KlipperInstaller;

public class KlipperInstanceFactory : IDisposable
{
    public string KlipperBaseDirectory;
    public string KlipperVirtualEnvDirectory;

    public string KlipperRepoUrl = Environment.GetEnvironmentVariable("KlipperRepoUrl")?? "https://github.com/Klipper3d/klipper.git";
    
    public string KlipperRepoPath;
    
    public string SystemDirectory;
    
    private Repository repo;

    public KlipperInstanceFactory()
    {
        KlipperBaseDirectory = Environment.SpecialFolder.ApplicationData + "/Farmetta/Klipper";
        KlipperVirtualEnvDirectory = Environment.SpecialFolder.LocalApplicationData + "/Farmetta/Klipper/VirtualEnv";
        KlipperRepoPath = Path.Combine(KlipperBaseDirectory, "klipper");

        SystemDirectory = "/etc/systemd/system";

        if (!Directory.Exists(KlipperBaseDirectory))
            CloneRepo().Wait();
        
        repo = new Repository(KlipperRepoPath);
        
    }

    public void Dispose()
    {
        repo.Dispose();
    }

    public IEnumerable<string> GetKlipperVersions()
    {
        var remote = repo.Network.Remotes["origin"];
        var refSpec = remote.FetchRefSpecs.Select(x => x.Specification);
        Commands.Fetch(repo, remote.Name, refSpec, new FetchOptions(), "");
        
        
        return repo.Tags.OrderByDescending(tag => ((Commit) tag.Target).Committer.When).Select(tag => tag.FriendlyName);
    }

    public async Task CreateKlipperInstance(string? version = null)
    {
        if (version != null)
            await GotoVersion(version);
        
        
    }

    private async Task GotoVersion(string version)
    {
        var tag = repo.Tags.FirstOrDefault(tag => tag.FriendlyName == version);
        if (tag == null)
            throw new Exception($"Could not find tag {version}");

        await Task.Run(() => Commands.Checkout(repo, tag.CanonicalName));
    }

    private async Task CloneRepo()
    {
        Console.WriteLine("Cloning repo...");
        KlipperRepoPath = await Task.Run(() => Repository.Clone(KlipperRepoUrl, KlipperRepoPath )) ;

        await UpdateDeps();
    }

    public async Task UpdateDeps()
    {
        Console.WriteLine("Updating deps...");
        KlipperDepsParser depsParser = new KlipperDepsParser(KlipperRepoPath);
        var listDeps = depsParser.ListDeps();
        
        
        // TODO: These commands may need sudo
        // However I think they don't as they're being started from an elevated process
        string[] depsInstallCmds = [
            "apt-get update", 
            $"apt-get install --yes {String.Join(' ', listDeps)} python3 python3-venv"
        ];
        
        await CmdHelper.RunCmd(depsInstallCmds);
    }

    public async Task CreateVirtualEnv(int klipperId)
    {
        List<string> cmds = new List<string>();
        
        var klipperVEnvPath = GetKlipperVEnvPath(klipperId);
        
        if(!Directory.Exists(klipperVEnvPath))
            cmds.Add($"python3 -m venv {klipperVEnvPath}");
        
        string requirementsFilePath = Path.GetFullPath(Path.Combine(KlipperRepoPath, "scripts/klippy-requirements.txt"));
        cmds.Add($"{klipperVEnvPath}/bin/pip install -r {requirementsFilePath}");
        
        await CmdHelper.RunCmd(cmds.ToArray());
    }

    public async Task CreateKlipperService(int klipperId)
    {
        var klipperVEnvPath = GetKlipperVEnvPath(klipperId);
        
        await using var serviceFile = File.Open(SystemDirectory, FileMode.Create, FileAccess.Write, FileShare.Read);
        
        await using var fileWriter = new StreamWriter(serviceFile);
        
        await fileWriter.WriteAsync($"#Systemd service file for klipper\n" +
                                        $"[Unit]\n" +
                                        $"Description=Starts klipper on startup\n" +
                                        $"After=network.target\n" +
                                        $"\n" +
                                        $"[Install]\n" +
                                        $"WantedBy=multi-user.target\n" +
                                        $"\n" +
                                        $"[Service]\n" +
                                        $"Type=simple\n" +
                                        $"User=$KLIPPER_USER\n" +
                                        $"RemainAfterExit=yes\n" +
                                        $"ExecStart={klipperVEnvPath}/bin/python ${KlipperRepoPath}/klippy/klippy.py ${{HOME}}/printer.cfg -l ${{KLIPPER_LOG}}\n" +
                                        $"Restart=always\n" +
                                        $"RestartSec=10");
        
        
    }

    private string GetKlipperVEnvPath(int klipperId)
    {
        return Path.Combine(KlipperVirtualEnvDirectory, $"klipper-{klipperId.ToString()}");

    }
}