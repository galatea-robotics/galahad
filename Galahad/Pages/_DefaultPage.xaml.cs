using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Galahad
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DefaultPage : Page
    {
        private int red_state = 1;
        private bool green_state = false;
        private const int RED = 5;
        private const int GREEN = 18;
        private GpioPin redPin;
        private GpioPin greenPin;

        public DefaultPage()
        {
            InitGPIO();
            this.InitializeComponent();

            Unloaded += MainPage_Unloaded;
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Validate
            if (gpio == null) return;

            // Activate pins, if RPi3 is connected
            redPin = gpio.OpenPin(RED);
            greenPin = gpio.OpenPin(GREEN);
            redPin.Write(GpioPinValue.Low);
            greenPin.Write(GpioPinValue.Low);
            redPin.SetDriveMode(GpioPinDriveMode.Output);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            redPin.Dispose();
            greenPin.Dispose();
        }

        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            if (red_state == 0)
            {
                redPin.Write(GpioPinValue.Low);
                red_state = 1;
            }
            else if (red_state == 1)
            {
                redPin.Write(GpioPinValue.High);
                red_state = 0;
            }
        }

        private async void greenButton_Click(object sender, RoutedEventArgs e)
        {
            green_state = !green_state;
            while (green_state == true)
            {
                greenPin.Write(GpioPinValue.High);
                await Task.Delay(500);
                greenPin.Write(GpioPinValue.Low); ;
                await Task.Delay(500);
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            App.StartEngine();

            stopButton.IsEnabled = true;
            startButton.IsEnabled = false;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            App.ShutdownEngine();

            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
        }
    }
}
