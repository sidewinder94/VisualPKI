﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VisualPKI.Resources.Converter
{
    class PathToBooleanConverter : IValueConverter
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
            var s = value as string;
            if (s != null)
            {
                return (String.IsNullOrEmpty(s) || String.IsNullOrWhiteSpace(s));
            }
            else return true;
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
