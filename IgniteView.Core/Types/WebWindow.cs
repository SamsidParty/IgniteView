using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class WebWindow
    {
        public AppManager CurrentAppManager
        {
            get
            {
                return AppManager.Instance;
            }
        }

        #region Window Bounds
        public WindowBounds Bounds
        {
            get
            {
                if (_Bounds.InitialWidth == 0)
                {
                    _Bounds = new WindowBounds(1280, 720);
                }
                return _Bounds;
            }
            set
            {
                _Bounds = value;
            }
        }
        public WindowBounds _Bounds;

        /// <summary>
        /// Sets the window bounds of this WebWindow
        /// </summary>
        public WebWindow WithBounds(WindowBounds bounds)
        {
            Bounds = bounds;
            return this;
        }

        #endregion

        #region Window Title

        /// <summary>
        /// Gets or sets the title of this WebWindow
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Sets the title of this WebWindow
        /// </summary>
        public virtual WebWindow WithTitle(string newTitle)
        {
            Title = newTitle;
            return this;
        }

        #endregion

        #region Window Handle

        /// <summary>
        /// Gets the handle of the window, on supported platforms this will return the native handle of the window
        /// </summary>
        public virtual IntPtr NativeHandle { get; }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Call this after configuring the window to display it on screen
        /// </summary>
        public virtual WebWindow Show() => this;

        #endregion

        #region Constructors

        public static WebWindow Create() => PlatformManager.Instance.CreateWebWindow();
        public static WebWindow Create(WindowBounds bounds) => Create().WithBounds(bounds);

        /// <summary>
        /// Only inherited classes should call this constructor
        /// </summary>
        protected WebWindow() { }

        #endregion
    }
}
