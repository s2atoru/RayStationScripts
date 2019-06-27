using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RoiManager
{
    public class NamedColor
    {
        public string Name { get; }
        public Color Color { get; }

        public NamedColor(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        static public List<NamedColor> GetNamedColors()
        {
            //from https://qiita.com/Kokudori/items/7c4b2ca35592e21af1d5
            Type colorsType = typeof(System.Windows.Media.Colors);
            var namedColors = colorsType.GetProperties(BindingFlags.Static | BindingFlags.Public)
                        .Where(x => x.PropertyType == typeof(System.Windows.Media.Color))
                        .Select(x => new NamedColor(x.Name, (Color)x.GetValue(colorsType, null))).ToList();

            return namedColors;
        }
    }
}
