﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;

namespace Awoo
{
    /// <summary>
    /// MainWin.xaml 的交互逻辑
    /// </summary>
    public partial class MainWin : Window
    {
        public string id;
        public string avatar;
        public string intro;
        public string status;
        public string username;
        public string email;
        public string lastlogin;
        public string token;
        public List<string> friends_username = new List<string>();
        public MainWin(string un, string tk)
        {
            username = un;
            token = tk;

            InitializeComponent();

            initMain();
            initFriends();
        }
        public void initMain()
        {
            ReplyUserFetch res = 
                Shared.sendrecvjson<FormUserFetchByUsername, ReplyUserFetch>
                (Shared.HOST, "/api/user/fetch/username", new FormUserFetchByUsername(username));
            if (res.reply != "succeed") { MessageBox.Show(res.reply, "Error"); this.Close(); }

            id = res.id;
            avatar = res.avatar;
            intro = res.intro;
            status = res.status;
            email = res.email;
            lastlogin = res.lastlogin;

            Avatar.Source = Shared.Base64ToImage(avatar);
            Username.Content = username;
            Intro.Content = "Introduction:" + intro;
        }
        public void initFriends()
        {
            ReplyUserFetchFriends res = 
                Shared.sendrecvjson<FormUserFetchFriends, ReplyUserFetchFriends>
                (Shared.HOST, "/api/user/fetch/friends", new FormUserFetchFriends(username, token));
            if (res.reply != "succeed") { MessageBox.Show(res.reply, "Error"); this.Close(); }

            List.Items.Clear();
            friends_username.Clear();
            foreach (var friend in res.friends)
            {
                /*
                <Grid Height="57" Width="268">
                    <Image Margin="9,9,0,9" HorizontalAlignment="Left" Width="39"/>
                    <Label Margin="55,0,10,30" Content="asdf" FontSize="13"/>
                </Grid>
                */

                ReplyUserFetch friendres = 
                    Shared.sendrecvjson<FormUserFetchByUsername, ReplyUserFetch>
                    (Shared.HOST, "/api/user/fetch/username", new FormUserFetchByUsername(friend));

                Grid grid = new Grid();
                List.Items.Add(grid);
                grid.Height = 57; grid.Width = this.Width-10;
                Image image = new Image();
                image.Source = Shared.Base64ToImage(friendres.avatar);
                image.Width = image.Height = 39; image.Margin = new Thickness(0, 9, 9, 9); image.HorizontalAlignment = HorizontalAlignment.Left;
                Label label = new Label();
                label.Content = friendres.username;
                label.Margin = new Thickness(55,0,10,30); label.HorizontalAlignment = HorizontalAlignment.Left;label.FontSize = 13;
                grid.Children.Add(image);
                grid.Children.Add(label);

                friends_username.Add(friendres.username);
            }


        }

        private void move_window(object sender, MouseButtonEventArgs e)
        {
            Shared.move_window(this);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            Application.Current.Shutdown();
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(List.SelectedIndex.ToString());
            ChatWin chatwin = new ChatWin(username, token, friends_username[List.SelectedIndex]);
            chatwin.Show();            
        }
    }
}