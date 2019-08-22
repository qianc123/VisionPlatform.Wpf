﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using VisionPlatform.BaseType;
using VisionPlatform.Core;
using VisionPlatform.Wpf;

namespace VisionPlatformDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public SceneManager SceneManager { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //更新视觉框架集合
            VisionFrameFactory.UpdateAssembly();
            VisionFrameFactory.DefaultVisionFrameType = EVisionFrameType.Halcon;

            //更新相机框架集合
            CameraFactory.UpdateAssembly();
            CameraFactory.DefaultCameraSdkType = ECameraSdkType.VirtualCamera;

            //获取场景管理器实例(单例)
            SceneManager = SceneManager.GetInstance();

            //恢复场景
            SceneManager.RecoverScenes();

            ScenesListView.Items.Clear();
            foreach (var item in SceneManager.Scenes.Values)
            {
                ScenesListView.Items.Add(item);
            }

            //更新控件
            VisionFrameComboBox.Items.Clear();
            VisionFrameComboBox.ItemsSource = VisionFrameFactory.VisionFrameAssemblys.Keys;
            VisionFrameComboBox.SelectedItem = VisionFrameFactory.DefaultVisionFrameType;

            CameraSDKComboBox.Items.Clear();
            CameraSDKComboBox.ItemsSource = CameraFactory.CameraAssemblys.Keys;
            CameraSDKComboBox.SelectedItem = CameraFactory.DefaultCameraSdkType;
        }

        private void ConfigFrameButton_Click(object sender, RoutedEventArgs e)
        {
            CameraFactory.DefaultCameraSdkType = (ECameraSdkType)CameraSDKComboBox.SelectedItem;
            VisionFrameFactory.DefaultVisionFrameType = (EVisionFrameType)VisionFrameComboBox.SelectedItem;
        }

        private void VisionFrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if ((EVisionFrameType)e.AddedItems[0] == EVisionFrameType.VisionPro)
                {
                    CameraSdkDockPanel.Visibility = Visibility.Hidden;
                }
                else
                {
                    CameraSdkDockPanel.Visibility = Visibility.Visible;
                }
            }

        }

        /// <summary>
        /// 恢复场景列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecoverScenesButton_Click(object sender, RoutedEventArgs e)
        {
            SceneManager.RecoverScenes();
            ScenesListView.ItemsSource = SceneManager.Scenes.Values;
            ScenesListView.UpdateLayout();
        }

        private Window SceneConfigWindow;

        /// <summary>
        /// 打开创建场景窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigFrameButton_Click(null, null);

            if ((VisionFrameFactory.DefaultVisionFrameType != EVisionFrameType.VisionPro) && (CameraFactory.DefaultCameraSdkType == ECameraSdkType.VirtualCamera))
            {
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images");
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images\alpha1.png");
                CameraFactory.AddCamera(@"C:\Users\Public\Documents\MVTec\HALCON-17.12-Progress\examples\images\autobahn.png");
                CameraFactory.AddCamera(@"E:\测试图像\刹车片");
                CameraFactory.AddCamera(@"E:\测试图像\AGV标定板");
            }

            var view = new SceneView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var viewModel = (view.DataContext as SceneViewModel);

            //注册场景配置完成事件
            viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

            //将控件嵌入窗口之中
            SceneConfigWindow = new Window();
            SceneConfigWindow.MinWidth = 800;
            SceneConfigWindow.MinHeight = 500;
            SceneConfigWindow.Width = 800;
            SceneConfigWindow.Height = 500;
            SceneConfigWindow.Content = view;
            SceneConfigWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SceneConfigWindow.Owner = Window.GetWindow(this);
            SceneConfigWindow.Title = "场景配置窗口";
            SceneConfigWindow.WindowState = WindowState.Maximized;

            SceneConfigWindow.ShowDialog();
        }

        /// <summary>
        /// 场景配置完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_SceneConfigurationCompleted(object sender, SceneConfigurationCompletedEventArgs e)
        {
            if (e.Scene != null)
            {
                //注册场景
                SceneManager.RegisterScene(e.Scene);
                
                ScenesListView.Items.Clear();
                foreach (var item in SceneManager.Scenes.Values)
                {
                    ScenesListView.Items.Add(item);
                }
            }

            SceneConfigWindow.Close();
        }

        private void ModifySceneButton_Click(object sender, RoutedEventArgs e)
        {
            //ScenesListView.SelectedItem
            var view = new SceneView(ScenesListView.SelectedItem as Scene)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var viewModel = (view.DataContext as SceneViewModel);

            //注册场景配置完成事件
            viewModel.SceneConfigurationCompleted += ViewModel_SceneConfigurationCompleted;

            //将控件嵌入窗口之中
            SceneConfigWindow = new Window();
            SceneConfigWindow.MinWidth = 800;
            SceneConfigWindow.MinHeight = 500;
            SceneConfigWindow.Width = 800;
            SceneConfigWindow.Height = 500;
            SceneConfigWindow.Content = view;
            SceneConfigWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SceneConfigWindow.Owner = Window.GetWindow(this);
            SceneConfigWindow.Title = "场景配置窗口";
            SceneConfigWindow.WindowState = WindowState.Maximized;

            SceneConfigWindow.ShowDialog();

        }

        private void DeleteSceneButton_Click(object sender, RoutedEventArgs e)
        {
            var scene = ScenesListView.SelectedItem as Scene;
            SceneManager.DeleteScene(scene.Name);

            ScenesListView.Items.Clear();
            foreach (var item in SceneManager.Scenes.Values)
            {
                ScenesListView.Items.Add(item);
            }
        }

        Thread thread1;
        Thread thread2;

        bool isEsc = false;
        object lockObj = new object();

        /// <summary>
        /// 自动执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AotoExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            lock (lockObj)
            {
                if (isEsc == false)
                {
                    return;
                }
            }

            if (ScenesListView.Items.Count >= 2)
            {
                var scene1 = ScenesListView.Items[0] as Scene;
                var scene2 = ScenesListView.Items[1] as Scene;

                RunningWindow1.Content = scene1.VisionFrame.RunningWindow;
                RunningWindow2.Content = scene2.VisionFrame.RunningWindow;

                lock (lockObj)
                {
                    isEsc = false;
                }

                thread1 = new Thread(() =>
                {
                    string result;
                    while (true)
                    {
                        var random = new Random();
                        scene1.Execute(1000, out result);

                        lock (lockObj)
                        {
                            if (isEsc)
                            {
                                break;
                            }
                        }

                        Thread.Sleep(random.Next(100, 300));
                    }

                });

                thread2 = new Thread(() =>
                {
                    string result;
                    while (true)
                    {
                        var random = new Random();
                        scene2.Execute(1000, out result);

                        lock (lockObj)
                        {
                            if (isEsc)
                            {
                                break;
                            }
                        }

                        Thread.Sleep(random.Next(100, 200));
                    }

                });

                thread1.Start();
                thread2.Start();
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (lockObj)
            {
                isEsc = true;
            }
        }

        private void ExecuteSceneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result;
                var scene1 = ScenesListView.SelectedItem as Scene;

                if (scene1 == null)
                {
                    return;
                }

                if (RunningWindow1.Content != scene1.VisionFrame.RunningWindow)
                {
                    RunningWindow1.Content = scene1.VisionFrame.RunningWindow;

                }
                RunStatus runStatus = scene1.Execute(1000, out result);

                if (runStatus.Result == EResult.Error)
                {
                    MessageBox.Show(runStatus.Message);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void EscThreadButton_Click(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                isEsc = true;
            }
        }
    }
}
