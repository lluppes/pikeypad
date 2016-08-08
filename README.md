# pikeypad
##How to use a 4x4 Matrix Keypad with a Raspberry Pi

I wanted to use a 4x4 Matrix Keypad in a Raspberry Pi project running on Windows 10 IoT Core, but I couldn't find any libraries that supported it, so I had to write one myself.  This is my first attempt and seems to work pretty well, but there are a few minor issues with it - mainly that you can't press they keys really fast - there has to be a slight delay between presses.  I'm not sure if that's a problem with this code, or with the keypad.  

###Steps to get your Keypad and Pi running
1. Buy a keypad and a Raspberry Pi 3

  Raspberry Pi ($35-$75, depending on accessories) - https://www.amazon.com/CanaKit-Raspberry-Complete-Starter-Kit/dp/B01C6Q2GSY/  
  
  Matrix Keypad ($7) - https://www.amazon.com/gp/product/B00TNF7Q6Y/
  
  Male/Female Jumper Wires (~$5) -https://www.amazon.com/Foxnovo-Breadboard-Jumper-Wires-Female/dp/B00PBZMN7C/
  
  
2. Wire up your matrix keypad to 8 GPIO pins on your Raspberry Pi and make a list of the 8 pin numbers you used.
   For my project, I hooked them up to Pins 16, 20, 21, 5, 6, 13, 19, 26
   ![Diagram](https://raw.githubusercontent.com/lluppes/pikeypad/master/Pi_Keypad_Wiring.png)
   ![Photo](https://raw.githubusercontent.com/lluppes/pikeypad/master/Pi_Keypad_Picture.jpg)

3. Download the CS_Universal_App folder and open the MatrixKeypad.csproj.

4. Update the MatrixKeypadPage.xaml.cs if you used different pins than what I listed above.

5. Deploy the project to your Pi and test it out!

###Using this in your project
If you want to use it in other projects, all you should need is the MatrixKeypadMonitor.cs file.   Include that file and then put this code in your code somewhere.
```
  // The List on the next line is the GPIO pins that you hooked up
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
  
  // This event gets triggered when a key is pressed
  public void FoundDigit(object sender, string digit)
  {
    Debug.WriteLine(string.Format("{0} was pressed!", digit));
    // Do something here with your keypress
  }
```

Hope this helps!

Lyle
