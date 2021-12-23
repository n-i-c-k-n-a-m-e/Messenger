using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatClient.ServiceChat;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        int theme = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                try
                {
                    client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                    ID = client.Connect(tbUserName.Text);
                    tbUserName.IsEnabled = false;
                    tbPassword.IsEnabled = false;
                    bConnDicon.Content = "Disconnect";
                    isConnected = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, Title = "Error");
                }
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                try
                {
                    client.Disconnect(ID);
                    client = null;
                    tbUserName.IsEnabled = true;
                    tbPassword.IsEnabled = true;
                    bConnDicon.Content = "Connect";
                    isConnected = false;
                }
                catch (System.ServiceModel.CommunicationObjectFaultedException ex)
                {
                    MessageBox.Show(ex.Message, Title = "Error");
                }
            }

        }
        
        private void bBlack_Click(object sender, RoutedEventArgs e)
        {
            if (theme == 0)
            {
                MainGrid.Background = Brushes.White;
                lbChat.Background = Brushes.White;
                tbUserName.Background = Brushes.White;
                tbPassword.Background = Brushes.White;
                tbMessage.Background = Brushes.White;

                theme = 1;
            }
            else if (theme == 1)
            {
                MainGrid.Background = Brushes.Black;
                lbChat.Background = Brushes.Gray;
                tbUserName.Background = Brushes.Gray;
                tbPassword.Background = Brushes.Gray;
                tbMessage.Background = Brushes.Gray;

                theme = 0;
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> accounts = new Dictionary<string, string>(3);

            string temp;
            StreamReader f = new StreamReader((@"C:\Users\Lolik-PC\Desktop\wcf_chat\namedate.txt"));
            while ((temp = f.ReadLine()) != null)
            {
                accounts.Add(temp.Split(' ')[0], temp.Split(' ')[1]);
            }
            f.Close();

            bool account_found = false;
            foreach (var account in accounts)
            {
                if (tbUserName.Text == account.Key && tbPassword.Text == account.Value)
                {
                    account_found = true;
                    break;
                }
            }

            if (!account_found) { return;  }

            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }

        }
        private void bSendSticker_Click(object sender, RoutedEventArgs e)
        {
/*            Image myImage = new Image();
            myImage.Width = 200;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"D:\programm\wcf_chat\wcf_chat\ChatClient\bin\Debug\stick1.png");
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            myImage.Source = myBitmapImage;

            lbChat.Items.Add(myImage);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);*/
        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count-1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client!=null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;
                }               
            }
        }
    }
}
