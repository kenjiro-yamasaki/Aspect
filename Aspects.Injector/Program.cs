using Mono.Cecil;
using SoftCube.Log;
using System;
using System.IO;
using System.Reflection;

namespace SoftCube.Aspects.Injector
{
    /// <summary>
    /// プログラム。
    /// </summary>
    class Program
    {
        /// <summary>
        /// メイン関数。
        /// </summary>
        /// <param name="args">アプリケーション引数。</param>
        static int Main(string[] args)
        {
            try
            {
                // カスタムコードの注入対象アセンブリファイルパスを取得します。
                var assemblyFilePath      = args[0];
                var assemblyDirectoryName = Path.GetDirectoryName(assemblyFilePath);
                var assemblyFileName      = Path.GetFileName(assemblyFilePath);

                //
                var copyDirectoryName    = Path.Combine(Path.GetTempPath(), "Aspects.Injector");
                var copyAssemblyFilePath = Path.Combine(copyDirectoryName, assemblyFileName);
                using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFilePath, new ReaderParameters() { ReadSymbols = true, ReadWrite = true }))
                {
                    if (!Directory.Exists(copyDirectoryName))
                    { 
                        Directory.CreateDirectory(copyDirectoryName);
                    }
                    assemblyDefinition.Write(copyAssemblyFilePath, new WriterParameters() { WriteSymbols = true });
                }

                // 対象アセンブリを含むディレクトリをカレントディレクトリに変更します。
                Environment.CurrentDirectory = assemblyDirectoryName;

                // 対象アセンブリにアスペクト(カスタムコード)を注入します。
                Logger.Info($" Injecting assembly {assemblyFilePath}...");
                using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFilePath, new ReaderParameters() { ReadSymbols = true, ReadWrite = true }))
                {
                    var assembly = Assembly.LoadFrom(copyAssemblyFilePath);

                    assemblyDefinition.Inject(assembly);
                    assemblyDefinition.Write(new WriterParameters() { WriteSymbols = true });
                }

                Logger.Info($" Assembly injection for {assemblyFilePath} complete.");

                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace.ToString());

                return -1;
            }
        }
    }
}
