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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionPlatform.BaseType;
using VisionPlatform.Core;


namespace VisionPlatformDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new SceneCreateWindow().ShowDialog();
        }

        private void ConfigFrameButton_Click(object sender, RoutedEventArgs e)
        {
            string cameraSDK = CameraSDKComboBox.SelectedItem as string;
            ECameraSDK eCameraSDK;
            switch (CameraSDKComboBox.SelectedItem)
            {
                case "Pylon":
                    eCameraSDK = ECameraSDK.Pylon;
                    break;
                case "VirtualCamera":
                    eCameraSDK = ECameraSDK.VirtualCamera;
                    break;
                default:
                    eCameraSDK = ECameraSDK.Unknown;
                    break;
            }

            CameraFactory.ECameraSDK = eCameraSDK;
        }
    }
}
