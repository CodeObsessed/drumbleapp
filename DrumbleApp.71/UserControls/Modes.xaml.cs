using System.Windows;
using System.Windows.Controls;

namespace Drumble.UserControls
{
    public partial class Modes : UserControl
    {
        public Modes()
        {
            InitializeComponent();
        }

        private bool showFooter;
        public bool ShowFooter
        {
            get
            {
                return showFooter;
            }
            set
            {
                showFooter = value;

                if (!value)
                    Footer.Visibility = Visibility.Collapsed;
            }
        }
    }
}
