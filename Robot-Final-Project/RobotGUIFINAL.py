from Tkinter import *
import serial
import time


def sel():
   selection = "Speed = " + str(var.get())+ "%"
   label.config(text = selection)
   ser.write(repr(int(var.get())) + "S")

def moveUp():
    label.config(text = "Moving forward")
    ser.write("F")
    

def moveDown():
    label.config(text = "Moving backwards")
    ser.write("B")

def moveRight():
    label.config(text = "Moving right")
    ser.write("R")

def moveLeft():
    label.config(text = "Moving left")
    ser.write("L")

def stop():
    label.config(text = "Stopping")
    ser.write("P")

root = Tk()
root["bg"] = "light cyan"
root.geometry("1250x680")
var = DoubleVar()
scale = Scale( root, variable = var )
scale.place(x = 800, y = 150)
scale.config(length = 500, width = 50, font = 8,
             fg = "steel blue", bd = "1", bg = "powder blue", troughcolor = "pale turquoise"
             )

ser = serial.Serial('dev/ttyUSB1')


button = Button(root, text="Set Speed", command=sel)
button.config(height = 6, width = 15,
              bg = "powder blue", fg = "steel blue")
button.place(x = 950, y = 300)

button2 = Button(root, text=" ^ ", command=moveUp)
button2.place(x = 200, y = 150)
button2.config(height = 6, width = 15,
               bg = "powder blue", fg = "steel blue")


button4 = Button(root, text=" > ", command=moveRight)
button4.place(x = 350, y = 300)
button4.config(height = 6, width = 15,
               bg = "powder blue", fg = "steel blue")


button5 = Button(root, text=" < ", command=moveLeft)
button5.place(x = 50, y = 300)
button5.config(height = 6, width = 15,
               bg = "powder blue", fg = "steel blue")

button3 = Button(root, text=" v ", command=moveDown)
button3.place(x = 200, y = 450)
button3.config(height = 6, width = 15,
               bg = "powder blue", fg = "steel blue")

button6 = Button(root, text=" STOP ", command=stop)
button6.place(x = 200, y = 300)
button6.config(height = 6, width = 15,
               bg = "powder blue", fg = "steel blue")

label = Label(root)
label.pack()
label.place(x = 950, y = 450)
label.config(font = 10, bg = "light cyan", fg = "steel blue")

root.mainloop()
