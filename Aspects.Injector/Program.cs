using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;

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
                var assemblyFilePath = args[0];

                // 対象アセンブリを含むディレクトリをカレントディレクトリに変更します。
                Environment.CurrentDirectory = Path.GetDirectoryName(assemblyFilePath);

                // 対象アセンブリにアスペクト(カスタムコード)を注入します。
                Console.Out.WriteLine($" Injecting assembly {assemblyFilePath}...");
                using (var assembly = AssemblyDefinition.ReadAssembly(assemblyFilePath, new ReaderParameters() { ReadSymbols = true, ReadWrite = true }))
                {
                    assembly.Inject();
                    assembly.Write(new WriterParameters() { WriteSymbols = true });
                }
                Console.Out.WriteLine($" Assembly injection for {assemblyFilePath} complete.");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace.ToString());

                return -1;
            }
        }
    }
}
