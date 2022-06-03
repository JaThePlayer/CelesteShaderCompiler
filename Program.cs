using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System;
using System.IO;
using System.Threading;
using CelesteShaderCompiler;
using System.Linq;

string CompiledPath;
string SrcPath;

Console.WriteLine("Celeste Shader Compiler");
if (args.Any(arg => arg == "-h"))
{
    Console.WriteLine(@"USAGE: CelesteShaderCompiler.exe [sourceDirectory] [outputDirectory]");
    
    Console.WriteLine(@"Arguments marked with [] are optional");
    Console.WriteLine(@"sourceDirectory defaults to the current working directory");
    Console.WriteLine(@"outputDirectory defaults to sourceDirectory");
    Console.WriteLine(@"The compiler will listen for .fx file changes in the sourceDirectory, then compile them into .cso files in the outputDirectory automatically");
    
    Console.WriteLine();

    return;
}

SrcPath = Path.GetFullPath(args.Length switch
{
    > 0 => args[0],
    _ => Environment.CurrentDirectory,
});

CompiledPath = Path.GetFullPath(args.Length switch
{
    > 1 => args[1],
    _ => SrcPath,
});

Console.WriteLine($"Watching {SrcPath} for .fx file changes");
Console.WriteLine($"Compiled .cso files will be output to {CompiledPath}");

var watcher = new FileSystemWatcher(SrcPath);
watcher.Changed += Watcher_Changed;

while (true)
{
    watcher.WaitForChanged(WatcherChangeTypes.Changed | WatcherChangeTypes.Created);
}

void Watcher_Changed(object sender, FileSystemEventArgs e)
{
    // without this, the code editor could fail to save the file somehow
    Thread.Sleep(1000 / 60);
    try
    {
        if (e.FullPath.EndsWith(".fx"))
        {
            Console.WriteLine($"{e.FullPath} changed, recompiling");
            string txt = null;
            while (string.IsNullOrWhiteSpace(txt))
            {
                try
                {
                    txt = File.ReadAllText(e.FullPath);
                }
                catch
                {
                    Console.WriteLine("file locked. waiting");
                    Thread.Sleep(200);
                }
            }

            EffectProcessor effectProcessor = new();
            var effect = effectProcessor.Process(new EffectContent { EffectCode = txt }, new MyContext());

            File.WriteAllBytes(Path.Combine(CompiledPath, Path.GetFileNameWithoutExtension(e.FullPath) + ".cso"), effect.GetEffectCode());

            Console.WriteLine("compiled");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.GetType());
        Console.WriteLine(ex);
    }
}
