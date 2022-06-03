# Celeste Shader Compiler

A tiny wrapper for XNA's EffectImporter that can be used to compile shaders for Celeste. This compiler uses a file watcher to detect when an .fx file is saved, and will automatically compile it.

# Download
You can find the compiled version of the tool [here](https://github.com/JaThePlayer/CelesteShaderCompiler/releases), though keep in mind it's only tested on Windows, and *might* not work on Linux due to using XNA, but I'm not sure.

```
USAGE: CelesteShaderCompiler.exe [sourceDirectory] [outputDirectory]
Arguments marked with [] are optional
sourceDirectory defaults to the current working directory
outputDirectory defaults to sourceDirectory
The compiler will listen for .fx file changes in the sourceDirectory, then compile them into .cso files in the outputDirectory automatically
```
