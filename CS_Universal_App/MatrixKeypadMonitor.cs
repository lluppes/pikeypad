using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace MatrixKeypad
{
    /// <summary>
    /// Class that handles input from a 4x4 matrix keypad
    /// Usage Example:
    ///    // Send in a list of the GPIO pins that are used from line 1 to line 8
    ///    var matrixPad = new MatrixKeypadMonitor(new List<int> { 16, 20, 21, 5, 6, 13, 19, 26 });
    ///    // Subscribe to an event that is triggered when a keypress happens
    ///    if (matrixPad.SetupSuccessful)
    ///    {
    ///      matrixPad.FoundADigitEvent += FoundDigit;
    ///    }
    ///    // Do something when a key is pressed 
    ///    public void FoundDigit(object sender, string digit)
    ///    {
    ///       Debug.WriteLine(string.Format("Found {0}", digit));
    ///    }
    /// </summary>
    public class MatrixKeypadMonitor
    {
        #region Variables
        public bool SetupSuccessful { get; set; }
        public string SetupMessage { get; set; }
        public bool VerboseMode { get; set; }

        private int _timerIntervalMilliseconds = 20;
        private int _buttonDebounceMilliseconds = 25;

        private static GpioController _gpio = null;
        private List<int> Pins;
        private List<GpioPin> _rows = new List<GpioPin>();
        private List<GpioPin> _cols = new List<GpioPin>();

        private readonly string[,] _keypad = {
            {"1", "2", "3", "A"},
            {"4", "5", "6", "B"},
            {"7", "8", "9", "C"},
            {"*", "0", "#", "D"}
        };

        private string _lastPinValue = string.Empty;
        private DateTime _clearLastPinValueTime = DateTime.UtcNow;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        #endregion

        #region Initialization
        public MatrixKeypadMonitor(List<int> pins)
        {
            SetupSuccessful = false;
            // validate inputs
            if (pins != null && pins.Count == 8)
            {
                Pins = pins;
                // Set up the GPIO controls
                if (InitializeGpio(Pins))
                {
                    // Start Listening
                    StartTimer();
                    SetupSuccessful = true;
                }
                else
                {
                    SetupMessage = "GPIO Initialization failed! " + SetupMessage;
                    if (VerboseMode) Debug.WriteLine(SetupMessage);
                }
            }
            else
            {
                SetupMessage = "Please supply a list of 8 GPIO pin numbers.";
                if (VerboseMode) Debug.WriteLine(SetupMessage);
            }
        }

        private bool InitializeGpio(IReadOnlyList<int> pins)
        {
            try
            {
                // Initialize GPIO controller
                _gpio = GpioController.GetDefault();
                if (_gpio != null)
                {
                    // Initialize Column GPIO Pins
                    _cols.Add(_gpio.OpenPin(pins[0]));
                    _cols.Add(_gpio.OpenPin(pins[1]));
                    _cols.Add(_gpio.OpenPin(pins[2]));
                    _cols.Add(_gpio.OpenPin(pins[3]));

                    // Set the columns up for output
                    foreach (var c in _rows)
                    {
                        c.SetDriveMode(GpioPinDriveMode.Output);
                        c.Write(GpioPinValue.Low);
                    }

                    // Initialize Row GPIO Pins
                    var r1Pin = _gpio.OpenPin(pins[4]);
                    var r2Pin = _gpio.OpenPin(pins[5]);
                    var r3Pin = _gpio.OpenPin(pins[6]);
                    var r4Pin = _gpio.OpenPin(pins[7]);

                    // Add listeners to Row Pins
                    r1Pin.ValueChanged += Pin_ValueChanged;
                    r2Pin.ValueChanged += Pin_ValueChanged;
                    r3Pin.ValueChanged += Pin_ValueChanged;
                    r4Pin.ValueChanged += Pin_ValueChanged;

                    // Add to Row Arrao
                    _rows.Add(r1Pin);
                    _rows.Add(r2Pin);
                    _rows.Add(r3Pin);
                    _rows.Add(r4Pin);

                    // Set the rows up for input
                    foreach (var r in _rows)
                    {
                        r.SetDriveMode(GpioPinDriveMode.InputPullUp);
                    }

                    return true;
                }
                SetupMessage = "GPIO Controller not found!";
                if (VerboseMode) Debug.WriteLine(SetupMessage);
                return false;
            }
            catch (Exception ex)
            {
                SetupMessage = ex.Message;
                if (VerboseMode) Debug.WriteLine(SetupMessage);
                return false;
            }
        }

        private void StartTimer()
        {
            _dispatcherTimer.Tick += TimerTick;
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(_timerIntervalMilliseconds);
            _dispatcherTimer.Start();
        }
        #endregion

        #region Main Processes
        private void TimerTick(object source, object e)
        {
            // if it's been a while, then clear the last value so you enter another one
            if (_clearLastPinValueTime <= DateTime.UtcNow)
            {
                _lastPinValue = string.Empty;
                // reset the next check a ways out in the future
                _clearLastPinValueTime = DateTime.UtcNow.AddSeconds(15);
            }
        }

        private void Pin_Changed(GpioPin sender)
        {
            var colNumber = -1;
            var rowNumber = -1;
            try
            {
                var senderValue = sender.Read();
                if (senderValue.ToString() == _lastPinValue)
                {
                    if (VerboseMode) Debug.WriteLine("{0:hh:mm:ss.ffff} - Skipping duplicate value for pin {1}!", DateTime.Now, sender.PinNumber);
                    return;
                }
                // Key was pressed
                if (senderValue == GpioPinValue.High)
                {
                    // store last pressed value to help debounce value
                    _lastPinValue = senderValue.ToString();
                    _clearLastPinValueTime = DateTime.UtcNow.AddMilliseconds(_buttonDebounceMilliseconds);

                    // Get the corresponding row index for conversion later
                    if (sender.PinNumber == Pins[4])
                    {
                        rowNumber = 0;
                    }
                    else
                    {
                        if (sender.PinNumber == Pins[5])
                        {
                            rowNumber = 1;
                        }
                        else
                        {
                            if (sender.PinNumber == Pins[6])
                            {
                                rowNumber = 2;
                            }
                            else
                            {
                                if (sender.PinNumber == Pins[7])
                                {
                                    rowNumber = 3;
                                }
                            }
                        }
                    }

                    // Set all the columns to low value using output mode
                    foreach (var c in _cols)
                    {
                        c.SetDriveMode(GpioPinDriveMode.Output);
                        c.Write(GpioPinValue.Low);
                    }

                    // Now switch the columns to input mode
                    foreach (var c in _cols)
                    {
                        c.SetDriveMode(GpioPinDriveMode.InputPullDown);
                    }

                    // Scan the columns to see which one is pressed
                    foreach (var c in _cols)
                    {
                        if (c.Read() == GpioPinValue.High)
                        {
                            // Get the corresponding column index for conversion later
                            if (c.PinNumber == Pins[0])
                            {
                                colNumber = 0;
                            }
                            else
                            {
                                if (c.PinNumber == Pins[1])
                                {
                                    colNumber = 1;
                                }
                                else
                                {
                                    if (c.PinNumber == Pins[2])
                                    {
                                        colNumber = 2;
                                    }
                                    else
                                    {
                                        if (c.PinNumber == Pins[3])
                                        {
                                            colNumber = 3;
                                        }
                                    }
                                }
                            }

                        }
                    }

                    // If both a row and column were found, then we have a valid input!
                    if (colNumber >= 0 && rowNumber >= 0)
                    {
                        var ch = _keypad[colNumber, rowNumber];
                        FoundADigit(ch);
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Pin_Changed Error: {0} {1} {2} {3}", sender.PinNumber, rowNumber, colNumber, ex.Message);
                if (VerboseMode) Debug.WriteLine(msg);
            }
            ////finally
            ////{
            ////    // Reset the sending pin back to input mode
            ////    sender.SetDriveMode(GpioPinDriveMode.InputPullUp);
            ////}
        }

        private void Pin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            Pin_Changed(sender);
        }
        #endregion

        #region Digit Found Message Delegate
        /// <summary>
        /// Digit Found Delegate
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="digit">The digit that was pressed</param>
        public delegate void FoundADigitPublisher(object sender, string digit);

        /// <summary>
        /// Event occurs when a digit is found and published
        /// </summary>
        public event FoundADigitPublisher FoundADigitEvent;

        /// <summary>
        /// Sends message to subscribers telling them a digit was pressed
        /// </summary>
        /// <param name="digit">The digit</param>
        public void FoundADigit(string digit)
        {
            if (VerboseMode) Debug.WriteLine("Found character " + digit);
            //// If there are any subscribers then call them
            FoundADigitEvent?.Invoke(this, digit);
        }
        #endregion
    }
}
