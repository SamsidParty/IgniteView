using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    /// <summary>
    /// Inherit This Class To Make A Custom Color That Can Be Changed At Any Point
    /// </summary>
    public class DynamicColor
    {
        public List<Action<DynamicColor>> Changed = new List<Action<DynamicColor>>();
        public Color Value
        {
            get
            {
                OnBeforeValueRequested();
                return Internal;
            }
        }
        internal Color Internal;

        public string HexValue
        {
            get
            {
                var c = Value;
                return $"{c.R:X2}{c.G:X2}{c.B:X2}";
            }
        }

        public DynamicColor() { }

        public DynamicColor(Color c)
        {
            UpdateValue(c);
        }

        internal void UpdateValue(Color c)
        {
            Internal = c;
            foreach (var chan in Changed)
            {
                chan.Invoke(this);
            }
        }

        public virtual void OnBeforeValueRequested()
        {

        }

        public static implicit operator Color(DynamicColor c) => c.Internal;
        public static implicit operator DynamicColor(Color c) => new DynamicColor(c);
    }
}
