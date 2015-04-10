using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234235 上有介绍

namespace SduPackage.Controls
{
    public sealed class HeaderButton : Button
    {
        public static readonly DependencyProperty SelectedImageProperty = DependencyProperty.Register("SelectedImage", typeof(ImageSource), typeof(HeaderButton), null);

        public ImageSource SelectedImage
        {
            get
            {
                return (ImageSource)base.GetValue(SelectedImageProperty);
            }
            set
            {
                base.SetValue(SelectedImageProperty, value);
            }
        }

        public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(HeaderButton), null);

        public ImageSource DefaultImage
        {
            get
            {
                return (ImageSource)base.GetValue(DefaultImageProperty);
            }
            set
            {
                base.SetValue(DefaultImageProperty, value);
            }
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(HeaderButton), new PropertyMetadata(false, new PropertyChangedCallback(IsCheckedChanged)));

        public bool IsChecked
        {
            get
            {
                return (bool)base.GetValue(IsCheckedProperty);
            }
            set
            {
                base.SetValue(IsCheckedProperty, value);
            }
        }

        private static void IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetState((HeaderButton)d);
        }

        private static void SetState(HeaderButton button)
        {
            VisualStateManager.GoToState(button, button.IsChecked ? "Checked" : "Unchecked", true);
        }

        public HeaderButton()
        {
            this.DefaultStyleKey = typeof(HeaderButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetState(this);
        }
    }
}
