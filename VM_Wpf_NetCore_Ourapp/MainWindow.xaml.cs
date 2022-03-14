using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VM_Wpf_NetCore_Ourapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string contextMenu_searchItem_text = "Search";
        private readonly string contextMenu_SettingsItem_text = "Settings";
        private readonly string contextMenu_quitItem_text = "Quit";

        private NotifyIcon trayIcon;

        public MainWindow()
        {
            InitializeComponent();

            InitialzeWindow();
            InitializeTrayIcon();
        }

        //Initialize functions
        private void InitializeTrayIcon()
        {
            Bitmap bmp = new Bitmap(@"C:\Users\vulem\source\repos\VM_Wpf_NetCore_Ourapp\VM_Wpf_NetCore_Ourapp\Resources\folderVoyager.png");
            // Icon iconImg = System.Drawing.Icon.FromHandle(bmp.GetHicon());

            trayIcon = new NotifyIcon();
            trayIcon.Visible = true;
            trayIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());

            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add(contextMenu_searchItem_text, null, TrayIconContextMenuItemClick);
            trayIcon.ContextMenuStrip.Items.Add(contextMenu_SettingsItem_text, null, TrayIconContextMenuItemClick);
            trayIcon.ContextMenuStrip.Items.Add(contextMenu_quitItem_text, null, TrayIconContextMenuItemClick);
        }

        private void InitialzeWindow()
        {
            //Start position of floating window
            var desktopDim = SystemParameters.WorkArea;
            Left = desktopDim.Right - Width * 2;
            Top = desktopDim.Top + Height * 2;

            HideFloat(); //hide on start - maybe from config file 
        }


        //Events 
        private void TrayIconContextMenuItemClick(object sender, EventArgs e)
        {
            var typedSender = sender as ToolStripMenuItem;
            if (typedSender != null)
            {
                if (typedSender.Text == contextMenu_searchItem_text)
                {
                    ShowFloat();
                }
                else if (typedSender.Text == contextMenu_quitItem_text)
                {
                    this.Close();
                }
                else if (typedSender.Text == contextMenu_SettingsItem_text)
                {
                    //implement settings
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void button_minimize_Click(object sender, RoutedEventArgs e)
        {
            HideFloat();

        }

        protected override void OnClosed(EventArgs e)
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }

        #region TrayIcon click event (Not using)
        private void TrayIcon_Click(object sender, EventArgs e)
        {
            if ((e as System.Windows.Forms.MouseEventArgs).Button == MouseButtons.Left)
            {
                this.Visibility = Visibility.Visible;
            }
        }
        #endregion


        //Helper functions
        #region Show float function
        private void ShowFloat()
        {
            this.Visibility = Visibility.Visible;
        }
        #endregion
        #region Hide float function
        private void HideFloat()
        {
            this.Visibility = Visibility.Collapsed;
        }
        #endregion

    }//[Class]
}//[Namespace]
