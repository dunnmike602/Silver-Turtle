using System;
using System.Windows.Media;

namespace SilverTurtle.Controls
{
    public partial class ImageButton
    {
        public delegate void ClickHandler(object sender, EventArgs e);
        public event ClickHandler Click;

        public void InvokeClick(EventArgs e)
        {
            var handler = Click;
            if (handler != null) handler(this, e);
        }

        public ImageButton()
        {
            InitializeComponent();
        }

        public object ToolTip
        {
            get { return ButtonToolTip.Content; }
            set
            {
                ButtonToolTip.Content = value;
            }
        }

        public string Text
        {
            get { return ButtonText.Text; }
            set
            {
                ButtonText.Text = value;
            }
        }

        public ImageSource Image
        {
            get { return ButtonImage.Source; }
            set
            {
                ButtonImage.Source = value;
            }
        }

        private void MainButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            InvokeClick(e);
        }
    }
}
