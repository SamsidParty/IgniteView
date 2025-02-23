using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Desktop
{
    public class MacWebWindow : DesktopWebWindow
    {

        #region Native Imports

        [DllImport(InteropHelper.DLLName)]
        protected static extern bool MacIsDark();

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the system is in dark mode
        /// </summary>
        public bool IsDarkMode
        {
            get
            {
                return MacIsDark();
            }
        }

        #endregion

        /// <summary>
        /// Checks the system dark mode state and applies it to the window
        /// </summary>
        void UpdateDarkModeState() => SetWebWindowDark(WindowIndex, IsDarkMode);

        /// <summary>
        /// Constantly checks if the dark mode state has changed and updates accordingly.
        /// TODO: Find a better method to detect when dark mode state changes
        /// </summary>
        async Task DarkModeCheckLoop()
        {
            var lastDarkValue = IsDarkMode;
            while (true)
            {
                await Task.Delay(1000);

                if (lastDarkValue != IsDarkMode) { 
                    lastDarkValue = IsDarkMode;
                    UpdateDarkModeState();
                }
            }
        }

        public override WebWindow Show()
        {
            UpdateDarkModeState();
            Task.Run(DarkModeCheckLoop);
            base.Show();
            return this;
        }
    }
}
