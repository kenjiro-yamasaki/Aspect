using System.Threading;
using System.Threading.Tasks;

namespace SoftCube.AsyncTest
{
    /// <summary>
    /// アスペクト。
    /// </summary>
    public class Aspect
    {
        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public async Task OnInvokeAsync(AspectArgs args)
        {
            if (false)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                });
            }
        }
    }
}
