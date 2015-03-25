namespace VisualPKI.Resources.Lang
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static readonly Strings LocaleInstance = new Strings();
        public Strings Locale { get { return LocaleInstance; } }

    }
}
