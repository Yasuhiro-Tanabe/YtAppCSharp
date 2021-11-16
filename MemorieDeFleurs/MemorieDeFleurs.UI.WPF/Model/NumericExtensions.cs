namespace MemorieDeFleurs.UI.WPF.Model
{
    public static class NumericExtensions
    {
        /// <summary>
        /// インチ→Pixcel変換
        /// </summary>
        /// <param name="inch">変換元[インチ]</param>
        /// <param name="dpi">Dot per Pixcel 値：デフォルトでは 96.0 DPI</param>
        /// <returns>変換した値[px] = <see cref="inch"/> * <see cref="dpi"/></returns>
        public static double InchToPixcel(this double inch, double dpi = 96.0)
        {
            return inch * dpi;
        }

        /// <summary>
        /// ミリ→インチ変換
        /// </summary>
        /// <param name="mm">変換元[mm]</param>
        /// <returns>変換した値[インチ]</returns>
        public static double MmToPixcel(this double mm)
        {
            return (mm/24.5).InchToPixcel();
        }
    }
}
