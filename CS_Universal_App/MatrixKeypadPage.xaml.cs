using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MatrixKeypad
{
    /// <summary>
    /// Example app for getting input from Matrix Keypad
    /// </summary>
    public sealed partial class MatrixKeypadPage : Page
    {
        readonly SolidColorBrush _normalBlueBrush = new SolidColorBrush(Colors.CadetBlue);
        readonly SolidColorBrush _activeBlueBrush = new SolidColorBrush(Colors.Blue);
        readonly SolidColorBrush _normalRedBrush = new SolidColorBrush(Colors.Red);
        readonly SolidColorBrush _activeRedBrush = new SolidColorBrush(Colors.Orange);

        private string _currentDigits = string.Empty;

        public MatrixKeypadPage()
        {
            this.InitializeComponent();

            // List is the GPIO pins that are used from line 1 to line 8
            var matrixPad = new MatrixKeypadMonitor(new List<int> { 16, 20, 21, 5, 6, 13, 19, 26 });
            // Subscribe to an event that is triggered when a keypress happens
            if (matrixPad.SetupSuccessful)
            {
                matrixPad.FoundADigitEvent += FoundDigit;
            }
            else
            {
                Debug.WriteLine(matrixPad.SetupMessage);
            }
        }

        public void FoundDigit(object sender, string digit)
        {
            Debug.WriteLine(string.Format("Found {0}", digit));
            ShowDigits(digit);
            LightUpCharacter(digit);
        }

        private async void ShowDigits(string ch)
        {
            if (_currentDigits.Length == 4) _currentDigits = string.Empty;
            if (ch != string.Empty)
            {
                _currentDigits += ch;
            }
            var dLen = _currentDigits.Length;
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                digit1.Content = (dLen > 0) ? _currentDigits.Substring(0, 1) : "-";
                digit2.Content = (dLen > 1) ? _currentDigits.Substring(1, 1) : "-";
                digit3.Content = (dLen > 2) ? _currentDigits.Substring(2, 1) : "-";
                digit4.Content = (dLen > 3) ? _currentDigits.Substring(3, 1) : "-";
            });
        }

        private void LightUpCharacter(string key)
        {
            key = key.Trim().ToUpper();
            switch (key)
            {
                case "1":
                    LightUpScreenButton(btn1, _activeBlueBrush);
                    break;
                case "2":
                    LightUpScreenButton(btn2, _activeBlueBrush);
                    break;
                case "3":
                    LightUpScreenButton(btn3, _activeBlueBrush);
                    break;
                case "4":
                    LightUpScreenButton(btn4, _activeBlueBrush);
                    break;
                case "5":
                    LightUpScreenButton(btn5, _activeBlueBrush);
                    break;
                case "6":
                    LightUpScreenButton(btn6, _activeBlueBrush);
                    break;
                case "7":
                    LightUpScreenButton(btn7, _activeBlueBrush);
                    break;
                case "8":
                    LightUpScreenButton(btn8, _activeBlueBrush);
                    break;
                case "9":
                    LightUpScreenButton(btn9, _activeBlueBrush);
                    break;
                case "0":
                    LightUpScreenButton(btn0, _activeBlueBrush);
                    break;
                case "A":
                    LightUpScreenButton(btnA, _activeRedBrush);
                    break;
                case "B":
                    LightUpScreenButton(btnB, _activeRedBrush);
                    break;
                case "C":
                    LightUpScreenButton(btnC, _activeRedBrush);
                    break;
                case "D":
                    LightUpScreenButton(btnD, _activeRedBrush);
                    break;
                case "S":
                case "*":
                    LightUpScreenButton(btnS, _activeRedBrush);
                    break;
                case "P":
                case "#":
                    LightUpScreenButton(btnP, _activeRedBrush);
                    break;
            }
        }

        private async void LightUpScreenButton(Button btn, SolidColorBrush color)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (btn1.Background != _normalBlueBrush) btn1.Background = _normalBlueBrush;
                if (btn2.Background != _normalBlueBrush) btn2.Background = _normalBlueBrush;
                if (btn3.Background != _normalBlueBrush) btn3.Background = _normalBlueBrush;
                if (btn4.Background != _normalBlueBrush) btn4.Background = _normalBlueBrush;
                if (btn5.Background != _normalBlueBrush) btn5.Background = _normalBlueBrush;
                if (btn6.Background != _normalBlueBrush) btn6.Background = _normalBlueBrush;
                if (btn7.Background != _normalBlueBrush) btn7.Background = _normalBlueBrush;
                if (btn8.Background != _normalBlueBrush) btn8.Background = _normalBlueBrush;
                if (btn9.Background != _normalBlueBrush) btn9.Background = _normalBlueBrush;
                if (btn0.Background != _normalBlueBrush) btn0.Background = _normalBlueBrush;
                if (btnA.Background != _normalBlueBrush) btnA.Background = _normalRedBrush;
                if (btnB.Background != _normalBlueBrush) btnB.Background = _normalRedBrush;
                if (btnC.Background != _normalBlueBrush) btnC.Background = _normalRedBrush;
                if (btnD.Background != _normalBlueBrush) btnD.Background = _normalRedBrush;
                if (btnS.Background != _normalBlueBrush) btnS.Background = _normalRedBrush;
                if (btnP.Background != _normalBlueBrush) btnP.Background = _normalRedBrush;

                btn.Background = color;
            });
        }
    }
}
