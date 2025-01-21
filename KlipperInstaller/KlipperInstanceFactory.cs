using LibGit2Sharp;

namespace KlipperInstaller;

public class KlipperInstanceFactory : IDisposable
{
    public string KlipperBaseDirectory;

    public string KlipperRepoUrl = Environment.GetEnvironmentVariable("KlipperRepoUrl")?? "https://github.com/Klipper3d/klipper.git";
    
    public string KlipperRepoPath;
    
    private Repository repo;

    public KlipperInstanceFactory()
    {
        KlipperBaseDirectory = Environment.SpecialFolder.ApplicationData + "\\Farmetta\\Klipper";
        KlipperRepoPath = Path.Combine(KlipperBaseDirectory, "klipper");

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

    public async Task CloneRepo()
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
            $"apt-get install --yes {String.Join(' ', listDeps)}"
        ];
        
        await CmdHelper.RunCmd(depsInstallCmds);
    }
}