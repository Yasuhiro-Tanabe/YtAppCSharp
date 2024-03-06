// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Reflection;
using System.Text;

if (args.Length == 0)
{
    Console.WriteLine(Help());
}
else
{
    var path = Environment.GetEnvironmentVariable("PATH");
    var pathExt = Environment.GetEnvironmentVariable("PATHEXT");
    if(string.IsNullOrEmpty(path) || string.IsNullOrEmpty(pathExt))
    {
        // Linux等他の環境にも PATH 環境変数はあるはずだが、
        // Windows 以外の環境での実行を想定していないので
        Console.WriteLine("This program is only for Windows (TM).");
    }
    else
    {
        var pathes = path.Split(';').ToArray();
        var pathExts = pathExt.Split(";").ToArray();

        if (args.Length == 1)
        {
            var arg = args[0];
            switch (arg)
            {
                case "-h":
                case "/?":
                case "/h":
                case "--help":
                    Console.WriteLine(Help());
                    break;
                case "--version":
                    Console.WriteLine(Version());
                    break;
                default:
                    if (arg.StartsWith("-") || arg.StartsWith("/"))
                    {
                        Console.WriteLine($"Invalid command line option, '{arg}'");
                    }
                    else
                    {
                        Which(args[0], pathes, pathExts);
                    }
                    break;
            }
        }
        else
        {
            foreach (var arg in args)
            {
                Console.WriteLine(Which(arg, pathes, pathExts));
            }
        }
    }
}

static string Which(string arg, string[] path, string[] pathExt)
{
    var found = new List<string>();
    foreach (var dir in path)
    {
        var fullPath = Path.Combine(dir, arg);

        if(File.Exists(fullPath))
        {
            // PATHEXTから拡張子を探さなくても該当する実行ファイルが見つかった：そのファイルのみ列挙
            found.Add(fullPath);
        }
        else
        {
            // PATHEXTに登録されている拡張子を全て実行ファイルの候補として列挙
            foreach (var ext in pathExt)
            {
                fullPath = Path.Combine(dir, $"{arg}{ext}");
                if (File.Exists(fullPath))
                {
                    found.Add(fullPath);
                }
            }
        }
    }

    if (found.Count == 0)
    {
        return $"No such command, {arg}.{Environment.NewLine}";
    }
    else
    {
        found.Add(string.Empty);
        return string.Join(Environment.NewLine, found);
    }
}

static string Help()
{
    var cmd = Process.GetCurrentProcess().ProcessName;
    return new StringBuilder()
        .AppendLine(Version())
        .AppendLine()
        .AppendLine("[Usage]")
        .AppendLine($"  {cmd} fileName [fileName ...]")
        .AppendLine("        Find command named 'fileName'.")
        .AppendLine()
        .AppendLine($"  {cmd} [-h|/?|/h|--help]")
        .AppendLine("        Show this help.")
        .AppendLine()
        .AppendLine($"  {cmd} --version")
        .AppendLine("        Show version of this command.")
        .ToString();
}

static string Version()
{
    var asmName = Assembly.GetExecutingAssembly().GetName();
    return $"{asmName.Name} {asmName.Version}";
}