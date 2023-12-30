using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfShazam.ChatGPT.Models;

namespace WpfShazam.ChatGPT
{
    public partial class ChatGPTUserControl : UserControl
    {
        private bool _isFirstLoaded;
        private ChatGPTViewModel? _chatgptViewMode;
        private ScrollViewer? _chatListViewScrollViewer;
        private ScrollViewer? _messageListViewScrollViewer;

        public ChatGPTUserControl()
        {
            InitializeComponent();

            Loaded += ChatGPTUserControl_Loaded;            
            PreviewKeyDown += ChatGPTUserControl_PreviewKeyDown;
        }       

        private void ChatGPTUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;

                _chatgptViewMode = (DataContext as ChatGPTViewModel)!;

                SetupChatListViewScrollViewer();
                _messageListViewScrollViewer = GetScrollViewer(MessageListView);
                SetupMessageListViewScrollViewer();

                _chatgptViewMode.UpdateUIAction = UpdateUI;                
                return;
            }

            _chatgptViewMode?.OnChatGPTTabActivated();
        }
        
        private void ChatGPTUserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrl+Enter for input of multiple lines                
                TextBox textBox = ChatInputTextBox;
                int caretIndex = textBox.CaretIndex;
                textBox.Text = textBox.Text.Insert(caretIndex, Environment.NewLine);
                textBox.CaretIndex = caretIndex + Environment.NewLine.Length;
                e.Handled = true;
            }
            else if ((e.Key == Key.Up || e.Key == Key.Down) && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                // Use CTRL+Up/Down to allow Up/Down alone for multiple lines in ChatInputTextBox
                TextBox? inputTextBox = Keyboard.FocusedElement as TextBox;
                if (inputTextBox?.Name == "ChatInputTextBox")
                {
                    _chatgptViewMode?.PrevNextChatInput(isUp: e.Key == Key.Up);
                }
            }
        }

        // Update UI from ChatViewModel
        private void UpdateUI(UpdateUIEnum updateUIEnum)
        {
            switch (updateUIEnum)
            {
                case UpdateUIEnum.SetFocusToChatInput:
                    ChatInputTextBox.Focus();
                    break;
                case UpdateUIEnum.SetupMessageListViewScrollViewer:
                    SetupMessageListViewScrollViewer();
                    break;
                case UpdateUIEnum.MessageListViewScrollToBottom:
                    _messageListViewScrollViewer?.ScrollToBottom();
                    break;
            }
        }

        private void SetupChatListViewScrollViewer()
        {
            // Get the ScrollViewer from the ListView. We'll need that in order to reliably
            // implement "automatically scroll to the bottom when new items are added" functionality.            
            _chatListViewScrollViewer = GetScrollViewer(ChatListView);

            // Based on: https://stackoverflow.com/a/1426312	
            INotifyCollectionChanged? notifyCollectionChanged = ChatListView.ItemsSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += (sender, e) =>
                {
                    _chatListViewScrollViewer?.ScrollToBottom();
                };
            }
        }

        // Needs to re-setup because MessageListView.ItemsSource resets with SelectedChat.MessageList
        // Note: technically there could be a leak without doing 'CollectionChanged -='
        private void SetupMessageListViewScrollViewer()
        {
            INotifyCollectionChanged? notifyCollectionChanged = MessageListView.ItemsSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += (sender, e) =>
                {
                    _messageListViewScrollViewer?.ScrollToBottom();
                };
            }
        }

        // From: https://stackoverflow.com/a/41136328
        // This is part of implementing the "automatically scroll to the bottom" functionality.
        private ScrollViewer? GetScrollViewer(UIElement? element)
        {
            ScrollViewer? scrollViewer = null;
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && scrollViewer == null; i++)
                {
                    if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                    {
                        scrollViewer = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                    }
                    else
                    {
                        scrollViewer = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                    }
                }
            }
            return scrollViewer;
        }
    }
}
