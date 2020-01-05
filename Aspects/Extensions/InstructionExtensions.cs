using Mono.Cecil.Cil;

namespace SoftCube.Aspects
{
    /// <summary>
    /// <see cref="Instruction"/> の拡張メソッド。
    /// </summary>
    public static class InstructionExtensions
    {
        #region メソッド

        /// <summary>
        /// <paramref name="target"/> までの距離 (バイト数) を取得します。
        /// </summary>
        /// <param name="instruction">命令。</param>
        /// <param name="target">ターゲット命令。</param>
        /// <returns>ターゲット命令までの距離 (バイト数)。</returns>
        public static int DistanceTo(this Instruction instruction, Instruction target)
        {
            int distance = 0;
            var current = instruction;

            while (current != target)
            {
                distance += current.GetSize();
                if (instruction.Offset < target.Offset)
                {
                    current = current.Next;
                }
                else
                {
                    current = current.Previous;
                }
            }

            return distance;
        }

        #endregion
    }
}
