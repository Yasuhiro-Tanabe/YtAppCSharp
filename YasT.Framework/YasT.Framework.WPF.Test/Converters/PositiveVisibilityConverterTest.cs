using System.Globalization;
using System.Windows;

using YasT.Framework.WPF.Converters;

namespace YasT.Framework.WPF.Test.Converters
{
    public class PositiveVisibilityConverterTest
    {
        [Test]
        public void TestConvertTrueToVisible()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.Convert(true, typeof(Visibility), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void TestConvertFalseToHidden()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.Convert(false, typeof(Visibility), "Hidden", CultureInfo.CurrentCulture), 
                Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void TestConvertFalseToCollupsedWithoutParameterString()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.Convert(false, typeof(Visibility), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void TestConvertFalseToCollupsedWithValidParameterString()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.Convert(false, typeof(Visibility), "Collupsed", CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void TestConvertBackVisibleToTrue()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.ConvertBack(Visibility.Visible, typeof(bool), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(true));
        }

        [Test]
        public void TestConvertBackHiddenToFalse()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.ConvertBack(Visibility.Hidden, typeof(bool), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(false));
        }

        [Test]
        public void TestConvertBackCollupsedToFalse()
        {
            var cnv = new PositiveVisibilityConverter();

            Assert.That(cnv.ConvertBack(Visibility.Hidden, typeof(bool), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(false));
        }
    }
}
