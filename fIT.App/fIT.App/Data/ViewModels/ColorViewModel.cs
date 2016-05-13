using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class ColorViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR

        public ColorViewModel()
        {
            this.Red = Color.FromRgb(219,17,47);
            this.Blue = Color.FromRgb(11, 163, 193);
            this.DarkGray = Color.FromRgb(112,112,112);
            this.LightGray = Color.FromRgb(224,224,224);
            this.White = Color.White;
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Creates a Color
        /// </summary>
        /// <param name="red">Value for red between 0 and 255</param>
        /// <param name="green">Value for green between 0 and 255</param>
        /// <param name="blue">Value for blue between 0 and 255</param>
        /// <param name="hue">Value for hue between 0 and 360</param>
        /// <param name="saturation">Value for saturation between 0 and 100</param>
        /// <param name="luminosity">value for luminosity between 0 and 100</param>
        /// <param name="alpha">value for alpha between 0 and 100</param>
        /// <returns></returns>
        private Color CreateColor(int red, int green, int blue, int hue, int saturation, int luminosity,int alpha = 100)
        {
            return this.CreateColor(red, green, blue, (double) hue, (double) saturation,(double) luminosity, alpha);
        }

        /// <summary>
        /// Creates a Color
        /// </summary>
        /// <param name="red">Value for red between 0 and 255</param>
        /// <param name="green">Value for green between 0 and 255</param>
        /// <param name="blue">Value for blue between 0 and 255</param>
        /// <param name="hue">Value for hue between 0 and 360</param>
        /// <param name="saturation">Value for saturation between 0 and 100</param>
        /// <param name="luminosity">value for luminosity between 0 and 100</param>
        /// <param name="alpha">value for alpha between 0 and 100</param>
        /// <returns></returns>
        private Color CreateColor(int red, int green, int blue, double hue, double saturation, double luminosity,int alpha = 100)
        {
            if (red < 0 || red > 256){throw new ArgumentException($"The value for '{nameof(red)}' must be between 0 and 255");}
            if (green < 0 || green > 256){throw new ArgumentException($"The value for '{nameof(green)}' must be between 0 and 255");}
            if (blue < 0 || blue > 256){throw new ArgumentException($"The value for '{nameof(blue)}' must be between 0 and 255");}
            if (hue < 0 || hue > 360){throw new ArgumentException($"The value for '{nameof(hue)}' must be between 0 and 360");}
            if (saturation < 0 || saturation > 100){throw new ArgumentException($"The value for '{nameof(saturation)}' must be between 0 and 100");}
            if (luminosity < 0 || luminosity > 100){throw new ArgumentException($"The value for '{nameof(luminosity)}' must be between 0 and 100");}
            if (alpha < 0 || alpha > 100){throw new ArgumentException($"The value for '{nameof(alpha)}' must be between 0 and 100");}

            
            var color = Color.FromRgba(red, green, blue, alpha);
            /*color = color.WithLuminosity(Math.Abs(luminosity) < 0.001 ? 0 : luminosity / 100);
            color = color.WithSaturation(Math.Abs(saturation) < 0.001 ? 0 : saturation / 100);
            color = color.WithHue(Math.Abs(hue) < 0.001 ? 0 : hue / 360);*/
            return color;
        }
        #endregion

        #region PROPERTIES
        public Color Red { get; private set; }
        public Color DarkGray { get; private set; }
        public Color LightGray { get; private set; }
        public Color White { get; private set; }
        public Color Blue { get; private set; }
        #endregion

    }
}
