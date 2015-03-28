using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using VisualPKI.Resources.Lang;

namespace VisualPKI.Resources.Validators
{
    public class EmailRule : ValidationRule
    {
        private static readonly Regex EmailRegex = new Regex(@"(\w+|-|\.|_)+@\w*\.?\w{2,}", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);


        public EmailRule()
        { }

        #region Overrides of ValidationRule

        /// <summary>
        /// En cas de substitution dans une classe dérivée, exécute des contrôles de validation sur une valeur.
        /// </summary>
        /// <returns>
        /// Objet <see cref="T:System.Windows.Controls.ValidationResult"/>.
        /// </returns>
        /// <param name="value">Valeur de la cible de liaison à vérifier.</param><param name="cultureInfo">Culture à utiliser dans cette règle.</param>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is String && ((String)value).Length > 0)
            {
                return EmailRegex.IsMatch((String)value)
                    ? new ValidationResult(true, null)
                    : new ValidationResult(false, Strings.InvalidMail);
            }
            return new ValidationResult(true, null);

        }

        #endregion
    }
}