namespace KlipperInstaller;

public class KlipperDepsParser
{
    private string klipperInstallFilePath;
    public KlipperDepsParser(string klipperRepoPath)
    {
        klipperInstallFilePath = Path.GetFullPath(Path.Combine(klipperRepoPath, "scripts/install-debian.sh"));
    }

    public IEnumerable<string> ListDeps()
    {
        using var klipperInstallFile = File.Open(klipperInstallFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        StreamReader reader = new StreamReader(klipperInstallFile);

        while (true)
        {
            string? line = reader.ReadLine();
            if (line == null)
                break;
            
            if(!line.Contains("PKGLIST="))
                continue;

            int openingQuote = line.IndexOf('"');
            int closingingQuote = line.IndexOf('"', openingQuote + 1);
            
            string depsLine = line.Substring(openingQuote + 1, closingingQuote - openingQuote - 1);
            IEnumerable<string> deps = depsLine.Split(' ');

            foreach (var dep in deps)
            {
                if(dep != "${PKGLIST}" && dep != "virtualenv" && dep != "python-dev" && dep != string.Empty)
                    yield return dep;
            }
        }
    }
}