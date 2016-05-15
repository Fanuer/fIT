using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels.Abstract
{
    public class ImageViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR

        public ImageViewModel()
        {
            Icon = ImageData.FromRessouce("fIT.App.Resources.Images.icon.png");
            Logout = ImageData.FromFile("ic_action_logout.png");
            Add = ImageData.FromFile("ic_action_add.png");
            Edit = ImageData.FromFile("ic_action_edit.png");
            Remove = ImageData.FromFile("ic_action_remove.png");
        }
        #endregion

        #region METHODS

        private List<string> GetEmbeddedResources()
        {
            return this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames().ToList();
        }
        #endregion

        #region PROPERTIES

        public ImageData Icon { get; private set; }
        public ImageData Logout { get; private set; }
        public ImageData Add { get; private set; }
        public ImageData Edit { get; private set; }
        public ImageData Remove { get; private set; }
        #endregion

        #region NESTED
        public class ImageData
        {
            #region CONST
            #endregion

            #region FIELDS
            #endregion

            #region CTOR

            private ImageData(string resourcePath="", ImageSource source=null)
            {
                ResourcePath = resourcePath;
                Source = source;
            }

            #endregion

            #region METHODS

            public static ImageData FromRessouce(string resourcePath)
            {
                if (String.IsNullOrWhiteSpace(resourcePath))
                {
                    throw new ArgumentNullException(nameof(resourcePath));
                }
                return new ImageData(resourcePath, ImageSource.FromResource(resourcePath));
            }

            public static ImageData FromFile(string resourcePath)
            {
                if (String.IsNullOrWhiteSpace(resourcePath))
                {
                    throw new ArgumentNullException(nameof(resourcePath));
                }
                var fullpath = Device.OnPlatform<string>("Icons/", "", "Assets/") + resourcePath;
                return new ImageData(fullpath, ImageSource.FromFile(fullpath));
            }
            #endregion

            #region PROPERTIES
            public string ResourcePath { get; set; }
            public ImageSource Source { get; set; }
            public bool IsFileResource => this.Source is FileImageSource;

            #endregion

        }
        #endregion
        
    }
}

