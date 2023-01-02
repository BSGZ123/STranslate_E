﻿using STranslate.Helper;
using STranslate.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;


namespace STranslate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM vm;
        public MainWindow()
        {
            InitializeComponent();

            vm = (MainVM)DataContext;
            
            InitView();

            InitialTray();

            //if (HotKeys.InputTranslate.Conflict || HotKeys.CrosswordTranslate.Conflict || HotKeys.ScreenShotTranslate.Conflict)
            //{
            //    MessageBox.Show("全局快捷键有冲突，请您到设置中重新设置");
            //}
        }

        private void InitialTray()
        {
            notifyIcon.Text = "STranslate";
            //notifyIcon.BalloonTipText = "STranslate 已启动";
            notifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("Images/translate.ico", UriKind.Relative)).Stream);
            notifyIcon.Visible = true;
            //notifyIcon.ShowBalloonTip(1000);

            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            System.Windows.Forms.ToolStripMenuItem CrossWordTranslateMenuItemBTN = new System.Windows.Forms.ToolStripMenuItem("划词翻译");
            CrossWordTranslateMenuItemBTN.Click += new EventHandler(CrossWordTranslateMenuItem_Click);
            //当失去焦点后无法从托盘处获取选中文本
            CrossWordTranslateMenuItemBTN.Enabled = true;

            System.Windows.Forms.ToolStripMenuItem ScreenshotTranslateMenuItemBTN = new System.Windows.Forms.ToolStripMenuItem("截图翻译");
            ScreenshotTranslateMenuItemBTN.Click += new EventHandler(ScreenshotTranslateMenuItem_Click);
            ScreenshotTranslateMenuItemBTN.Enabled = false;

            System.Windows.Forms.ToolStripMenuItem InputTranslateMenuItemBTN = new System.Windows.Forms.ToolStripMenuItem("输入翻译");
            InputTranslateMenuItemBTN.Click += new EventHandler(InputTranslateMenuItem_Click);

            System.Windows.Forms.ToolStripMenuItem OpenMainWinBTN = new System.Windows.Forms.ToolStripMenuItem("显示主界面");
            OpenMainWinBTN.Click += new EventHandler(OpenMainWin_Click);
            
            System.Windows.Forms.ToolStripMenuItem AutoStartBTN = new System.Windows.Forms.ToolStripMenuItem("开机自启");
            AutoStartBTN.Click += new EventHandler(AutoStart_Click);

            AutoStartBTN.Checked = StartupHelper.IsStartup();

            System.Windows.Forms.ToolStripMenuItem ExitBTN = new System.Windows.Forms.ToolStripMenuItem("退出");
            ExitBTN.Click += new EventHandler(Exit_Click);

            //MenuContainer menuContainer = new MenuContainer();
            //menuContainer.Add(CrossWordTranslateMenuItemBTN);
            //menuContainer.Add(ScreenshotTranslateMenuItemBTN);
            //menuContainer.Add(InputTranslateMenuItemBTN);
            //menuContainer.Add(OpenMainWinBTN);
            //menuContainer.Add(AutoStartBTN);
            //menuContainer.Add(ExitBTN);

            //System.Windows.Forms.ToolStripMenuItem[] childen = new System.Windows.Forms.ToolStripMenuItem[] {
            //    CrossWordTranslateMenuItemBTN,
            //    ScreenshotTranslateMenuItemBTN,
            //    InputTranslateMenuItemBTN,
            //    OpenMainWinBTN,
            //    AutoStartBTN,
            //    ExitBTN,
            //};
           
            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(CrossWordTranslateMenuItemBTN);
            notifyIcon.ContextMenuStrip.Items.Add(ScreenshotTranslateMenuItemBTN);
            notifyIcon.ContextMenuStrip.Items.Add(InputTranslateMenuItemBTN);
            notifyIcon.ContextMenuStrip.Items.Add(OpenMainWinBTN);
            notifyIcon.ContextMenuStrip.Items.Add(AutoStartBTN);
            notifyIcon.ContextMenuStrip.Items.Add(ExitBTN);
        }

        private void AutoStart_Click(object sender, EventArgs e)
        {
            if (StartupHelper.IsStartup()) StartupHelper.UnSetStartup();
            else StartupHelper.SetStartup();
            (sender as System.Windows.Forms.ToolStripMenuItem).Checked = StartupHelper.IsStartup();
        }


        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// 软件运行时快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //最小化 Esc
            if (e.Key == Key.Escape)
            {
                this.Hide();

                //取消置顶
                TopmostBtn.SetResourceReference(TemplateProperty, _UnTopmostTemplateName);
            }
            //置顶 Ctrl+Shift+T
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)
                && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)
                && e.Key == Key.T)
            {
                Top_Click(null, null);
            }
            //退出 Ctrl+Shift+Q
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)
                && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)
                && e.Key == Key.Q)
            {
                Exit_Click(null, null);
            }

        }

        /// <summary>
        /// 监听全局快捷键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            //base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HotkeysHelper.RegisterHotKey(handle);

            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(WndProc);
        }

        /// <summary>
        /// 热键的功能
        /// </summary>
        /// <param name="m"></param>
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            switch (msg)
            {
                case 0x0312: //这个是window消息定义的 注册的热键消息
                    //Console.WriteLine(wParam.ToString());
                    if (wParam.ToString().Equals(HotkeysHelper.InputTranslateId + ""))
                    {
                        this.InputTranslateMenuItem_Click(null, null);
                    }
                    else if (wParam.ToString().Equals(HotkeysHelper.CrosswordTranslateId + ""))
                    {
                        this.CrossWordTranslateMenuItem_Click(null, null);
                    }
#if false
                    else if (wParam.ToString().Equals(HotkeysHelper.ScreenShotTranslateId + ""))
                    {
                        this.ScreenshotTranslateMenuItem_Click(null, null);
                    }
#endif
                    else if (wParam.ToString().Equals(HotkeysHelper.OpenMainWindowId + ""))
                    {
                        this.OpenMainWin_Click(null, null);
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 非激活窗口则隐藏起来
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!_IsTopmost)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// 清空输入输出框
        /// </summary>
        private void ClearTextBox()
        {
            vm.InputTxt = string.Empty;
            vm.OutputTxt = string.Empty;
            vm.SnakeRet = string.Empty;
            vm.SmallHumpRet = string.Empty;
            vm.LargeHumpRet = string.Empty;
            vm.IdentifyLanguage = string.Empty;
        }

        /// <summary>
        /// 显示主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMainWin_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }


        /// <summary>
        /// 左键双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            InputTranslateMenuItem_Click(null, null);
        }

        /// <summary>
        /// 输入翻译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTranslateMenuItem_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            OpenMainWin_Click(null, null);
            TextBoxInput.Focus();
        }

        /// <summary>
        /// 划词翻译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CrossWordTranslateMenuItem_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            var sentence = GetWordsHelper.Get();
            this.Show();
            this.Activate();
            this.TextBoxInput.Text = sentence.Trim();
            _ = vm.Translate();
        }

        /// <summary>
        /// 截图翻译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenshotTranslateMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开发中");
        }

        /// <summary>
        /// 是否置顶
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Top_Click(object sender, RoutedEventArgs e)
        {
            if (_IsTopmost)
            {
                TopmostBtn.SetResourceReference(TemplateProperty, _UnTopmostTemplateName);
            }
            else
            {
                TopmostBtn.SetResourceReference(TemplateProperty, _TopmostTemplateName);
            }
            _IsTopmost = !_IsTopmost;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Environment.Exit(0);
        }
        private void InitView()
        {
            this.Activate();
            this.TextBoxInput.Focus();
        }
        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        private bool _IsTopmost { get; set; }
        private readonly string _TopmostTemplateName = "ButtonTemplateTopmost";
        private readonly string _UnTopmostTemplateName = "ButtonTemplateUnTopmost";
    }
}