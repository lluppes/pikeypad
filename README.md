# pikeypad
##How to use a 4x4 Matrix Keypad with a Raspberry Pi

I wanted to use a 4x4 Matrix Keypad in a Raspberry Pi project running on Windows 10 IoT Core, but I couldn't find any libraries that supported it, so I had to write one myself.  This is my first attempt and seems to work pretty well, but there are a few minor issues with it - mainly that you can't press they keys really fast - there has to be a slight delay between presses.  I'm not sure if that's a problem with this code, or with the keypad.  

###Steps to get your Pi and Keypad running
1. Buy a keypad and a Raspberry Pi 3

  https://www.amazon.com/CanaKit-Raspberry-Complete-Starter-Kit/dp/B01C6Q2GSY/  ($35-$75, depending on accessories)
  
  https://www.amazon.com/gp/product/B00TNF7Q6Y/  (~$7)
  
  https://www.amazon.com/Foxnovo-Breadboard-Jumper-Wires-Female/dp/B00PBZMN7C/ (~$5)
  <a href="https://www.amazon.com/Foxnovo-Breadboard-Jumper-Wires-Female/dp/B00PBZMN7C/" target="_blank">Male/Femail Jumper Wires</a>
  
2. Wire up your matrix keypad to 8 GPIO pins on your Raspberry Pi and make a list of the 8 pin numbers you used.
   For my project, I hooked them up to Pins 16, 20, 21, 5, 6, 13, 19, 26
   ![Diagram](https://raw.githubusercontent.com/lluppes/pikeypad/master/Pi_Keypad_Wiring.png)
   ![Photo](https://raw.githubusercontent.com/lluppes/pikeypad/master/Pi_Keypad_Picture.jpg)

3. Download the CS_Universal_App project
4. Update the MatrixKeypadPage.xaml.cs if you used different pins.
5. Run that project and test it out

Let me know if you have problems with these scripts!

Lyle
