﻿// ------------------------------------------------------------------------
//    YATSE 2 - A touch screen remote controller for XBMC (.NET 3.5)
//    Copyright (C) 2010  Tolriq (http://yatse.leetzone.org)
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Setup;
using Yatse2.Libs;

namespace Yatse2
{
    public class VisibilityConverter : IValueConverter
    {
        private static readonly VisibilityConverter TheInstance = new VisibilityConverter();
        private VisibilityConverter() { }
        public static VisibilityConverter Instance
        {
            get { return TheInstance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return false;

            var param = (string)parameter;
            
            if (param == "bool")
                return ((bool)value) ? Visibility.Visible : Visibility.Hidden;

            if (param == "boolinvert")
                return ((bool)value) ? Visibility.Hidden : Visibility.Visible;

            if (param == "long")
                return ((long)value > 0) ? Visibility.Visible : Visibility.Hidden;

            if (param == "longinvert")
                return ((long)value > 0) ? Visibility.Hidden : Visibility.Visible;

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class LongDurationConverter : IValueConverter
    {
        private static readonly LongDurationConverter TheInstance = new LongDurationConverter();
        private LongDurationConverter() { }
        public static LongDurationConverter Instance
        {
            get { return TheInstance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return false;
            var val = (long) value;
            return (val > 0) ? string.Format(CultureInfo.InvariantCulture,"{0:00}:{1:00}", val / 60, val % 60) : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class SkinImagePathConverter : IValueConverter
    {
        private static readonly SkinImagePathConverter TheInstance = new SkinImagePathConverter();
        private SkinImagePathConverter() { }
        public static SkinImagePathConverter Instance
        {
            get { return TheInstance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return false;
            var path = Helper.SkinPath + (string)value + @"\Interface\" + (string)parameter + ".png";
            if (!File.Exists(path))
            {
                Logger.Instance().Log("C_SkinImgPath","Missing skin image : " + path);
                return "";
            }
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class SkinBrushConverter : IValueConverter
    {
        private static readonly SkinBrushConverter TheInstance = new SkinBrushConverter();
        private SkinBrushConverter() { }
        public static SkinBrushConverter Instance
        {
            get { return TheInstance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return false;

            var param = (string) parameter;
            var temp = param.Split('/');
            if (temp.Length > 1)
            {
                param = temp[0];
            }

            var path = Helper.SkinPath + (string)value + @"\Interface\" + param + ".png";
            if (!File.Exists(path))
            {
                Logger.Instance().Log("C_SkinBrush", "Missing skin image : " + path);
                return new ImageBrush();
            }

            if (temp.Length > 1)
            {
                if (temp[1] == "Small")
                    return new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(path)),
                        Stretch = Stretch.Uniform,
                        TileMode = TileMode.None,
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        Viewport = new Rect(10, 10, 34, 34),
                        ViewportUnits = BrushMappingMode.Absolute,
                    };
                if (temp[1] == "Fill")
                    return new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(path)),
                        Stretch = Stretch.UniformToFill,
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center
                    };
            }

            return new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(path)),
                Stretch = Stretch.Uniform,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class CacheImageConverter : IValueConverter
    {
        private static readonly CacheImageConverter TheInstance = new CacheImageConverter();
        private CacheImageConverter() { }
        public static CacheImageConverter Instance
        {
            get { return TheInstance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return false;

            var param = (string)parameter;
            var path = Helper.CachePath + param + @"\" + ApiHelper.Instance().GetPluginHashFromFileName((string)value, Helper.Instance.CurrentApi) + ".jpg";
            if (File.Exists(path))
            {
                try
                {
                    return new BitmapImage(new Uri(path));
                }
                catch (Exception)
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Skin/Internal/Images/Empty.png"));
                }
            }
                
            param = @"\Interface\Default_" + param.Replace("\\", "-") + ".png";
            path = Helper.SkinPath + Helper.Instance.CurrentSkin + param;
            if (File.Exists(path))
                return new BitmapImage(new Uri(path));

            Logger.Instance().Log("C_CacheImage", "Missing default Image : " + path);
            return new BitmapImage(new Uri("pack://application:,,,/Skin/Internal/Images/Empty.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
 
  
    public class SkinImageConverter : IValueConverter
    {
        private static readonly SkinImageConverter TheInstance = new SkinImageConverter();
        private SkinImageConverter() { }
        public static SkinImageConverter Instance
        {
            get { return TheInstance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return false;

            var img = (string)value;
            var param = (string) parameter;

            if (String.IsNullOrEmpty(param))
                return false;

            var path = Helper.SkinPath + Helper.Instance.CurrentSkin + @"\" + param + @"\" + img + ".png";

            if (!String.IsNullOrEmpty(img))
            {
                if (param == @"Interface\Icon_")
                    path = Helper.SkinPath + Helper.Instance.CurrentSkin + @"\" + param + img + ".png";

                if (File.Exists(path))
                    return new BitmapImage(new Uri(path));

                Logger.Instance().Log("C_SkinImage", "Missing skin Image : " + path);
            }

            path = Helper.SkinPath + Helper.Instance.CurrentSkin + @"\" + param + @"\Default.png";
            if (File.Exists(path))
                return new BitmapImage(new Uri(path));

            Logger.Instance().Log("C_SkinImage", "Default not found : " + path);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
