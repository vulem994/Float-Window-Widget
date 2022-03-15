using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string contextMenu_searchItem_text = "Search";
        private readonly string contextMenu_SettingsItem_text = "Settings";
        private readonly string contextMenu_quitItem_text = "Quit";

        private NotifyIcon trayIcon;

        private bool _stop = false;

        private IKeyboardMouseEvents globalMouseHook;

        #region -selectedText- property
        private String _selectedText;
        public String selectedText
        {
            get { return _selectedText; }
            set
            {
                if (_selectedText != value)
                {
                    _selectedText = value;
                    //System.Windows.MessageBox.Show(selectedText);
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion
        #region -bmp- property
        private Bitmap _bmp;
        public Bitmap bmp
        {
            get { return _bmp; }
            set
            {
                if (_bmp != value)
                {
                    _bmp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //Initialize global mouse hook
            globalMouseHook = Hook.GlobalEvents();



            InitialzeWindow();
            InitializeTrayIcon();
        }

        //Initialize functions
        private void InitializeTrayIcon()
        {
            //Bitmap bmp = new Bitmap(@"C:\Users\vulem\source\repos\VM_Wpf_NetCore_Ourapp\VM_Wpf_NetCore_Ourapp\Resources\testIcon.png");
            // Icon iconImg = System.Drawing.Icon.FromHandle(bmp.GetHicon());
            bmp = VM_Wpf_NetCore_Ourapp.Resources.Resources.folderVoyager;

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


            globalMouseHook.MouseDoubleClick += GlobalMouseHook_SelectionFinished;
            globalMouseHook.MouseDragFinished += GlobalMouseHook_SelectionFinished;

            HideFloat(); //hide on start - maybe from config file 
        }

        private void ReleaseAllProcecss()
        {
            _stop = true;
            if (globalMouseHook != null)
            {
                globalMouseHook.MouseDoubleClick -= GlobalMouseHook_SelectionFinished;
                globalMouseHook.MouseDragFinished -= GlobalMouseHook_SelectionFinished;
                globalMouseHook.Dispose();
                globalMouseHook = null;
            }
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }


        //Events 
        #region TrayIcon context menu items click event
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
                    var result = System.Windows.MessageBox.Show("Are you sure you want to quit?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.Close();
                    }
                }
                else if (typedSender.Text == contextMenu_SettingsItem_text)
                {
                    //TODO: implement settings
                }
            }
        }

        #endregion

        #region ButtonMinize click event
        private void button_minimize_Click(object sender, RoutedEventArgs e)
        {
            HideFloat();
        }
        #endregion

        #region Window mouse down event
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion

        #region Window deactivated event
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
        #endregion

        #region Window closed event
        protected override void OnClosed(EventArgs e)
        {
            ReleaseAllProcecss();
        }
        #endregion

        #region Window mouse double click event (Not using)
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Debugger.Launch();
                //var x = getTextAsync();
                //System.Windows.MessageBox.Show(x);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        #endregion

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

        private async void GlobalMouseHook_SelectionFinished(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();

            System.Windows.Clipboard.Clear();
            await Task.Delay(50);

            System.Windows.Forms.SendKeys.SendWait("^c");

            await Task.Delay(50);
            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                selectedText = text;
            }
            else
            {
                // Restore the Clipboard.
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }


        }

        //Notify propertu change
        #region INotifyPropertyChange implementation
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }//[Class]
}//[Namespace]
