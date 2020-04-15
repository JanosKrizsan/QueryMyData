using System.Windows;

namespace QueryMyData.Models
{
    public class Popup
    {
        private string _text = string.Empty;
        private string _cptn = string.Empty;
        private MessageBoxButton _btn = MessageBoxButton.OK;
        private MessageBoxImage _img = MessageBoxImage.Information;
        public Popup()
        {
        }
        public void Show(string text, string cptn, int btn, int img)
        {
            Setup(text, cptn, btn, img);
            MessageBox.Show(_text, _cptn, _btn, _img);
        }

        private void Setup(string text, string cptn, int btn, int img)
        {
            _text = text;
            _cptn = cptn;

            _btn = btn switch
            {
                0 => MessageBoxButton.OK,
                1 => MessageBoxButton.OKCancel,
                3 => MessageBoxButton.YesNoCancel,
                4 => MessageBoxButton.YesNo,
                _ => MessageBoxButton.OK,
            };

            _img = img switch
            {
                0 => MessageBoxImage.Information,
                1 => MessageBoxImage.Warning,
                3 => MessageBoxImage.Error,
                4 => MessageBoxImage.Exclamation,
                _ => MessageBoxImage.Information,
            };
        }


    }
}
