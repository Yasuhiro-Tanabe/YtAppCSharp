using System.Globalization;
using System.Windows;

using YasT.Framework.WPF.Converters;

namespace YasT.Framework.WPF.Test.Converters
{
    public class NegativeVisivilityConverterTest
    {
        [Test]
        public void TestConvertTrueToHiddenWithValidParameterString()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.Convert(true, typeof(Visibility), "Hidden", CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void TestConvertTrueToCollupsedWithoutParameterString()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.Convert(true, typeof(Visibility), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void TestConvertTrueToCollupsedWithValidParameterString()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.Convert(true, typeof(Visibility), "Collupsed", CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void TestConvertFalseToVisible()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.Convert(false, typeof(Visibility), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void TestConvertBackVisibleToFalse()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.ConvertBack(Visibility.Visible, typeof(bool), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(false));
        }

        [Test]
        public void TestConvertBackCollupsedToTrue()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.ConvertBack(Visibility.Collapsed, typeof(bool), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(true));
        }

        [Test]
        public void TestConvertBackHiddenToTrue()
        {
            var cnv = new NegativeVisivilityConverter();

            Assert.That(cnv.ConvertBack(Visibility.Hidden, typeof(bool), string.Empty, CultureInfo.CurrentCulture),
                Is.EqualTo(true));
        }
    }
}
