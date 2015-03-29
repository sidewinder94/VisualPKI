using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using VisualPKI.Generation;

namespace VisualPKI.Resources.Converter
{
    public class SignatureAlgorithmsToHashesList : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Convertit une valeur.
        /// </summary>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur Null valide est utilisée.
        /// </returns>
        /// <param name="value">Valeur produite par la source de liaison.</param><param name="targetType">Type de la propriété de cible de liaison.</param><param name="parameter">Paramètre de convertisseur à utiliser.</param><param name="culture">Culture à utiliser dans le convertisseur.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is String)
            {
                return Certificate.SignatureAlgorithmsAndAssociatedHashes[(String)value];
            }
            return new List<String>();
        }

        /// <summary>
        /// Convertit une valeur.
        /// </summary>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur Null valide est utilisée.
        /// </returns>
        /// <param name="value">Valeur produite par la cible de liaison.</param><param name="targetType">Type dans lequel convertir.</param><param name="parameter">Paramètre de convertisseur à utiliser.</param><param name="culture">Culture à utiliser dans le convertisseur.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}