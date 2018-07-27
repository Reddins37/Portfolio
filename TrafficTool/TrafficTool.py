import tkinter as tk
import psutil
import pyshark
import sys
import glob, os
import queue
import threading
import asyncio
import random
import traceback
import socket
from netdisco.discovery import NetworkDiscovery
from PIL import ImageTk



class Device:
	# class for iot devices
	def __init__(self, type, name, mac, host, model, color):
		self.type = type
		self.name = name
		self.mac = mac
		self.host = host
		self.model = model
		self.row = 0
		self.col = 0
		self.dest = [] #this needs to be a server object 
		self.color = color
		
	def addServer(self, server):
		if server not in self.dest:
			self.dest.append(server)
			
class Server:
	# class for external servers
	def __init__(self, name, row, col, ip, protocol, time, dns, color):
		self.name = name
		self.row = row
		self.col = col
		self.ip = ip
		self.protocol = protocol
		self.color = color
		self.time = time
		self.dns = dns
		
class ColorRandomizer:
	def __init__(self):
		self.usedList = []
		
	def giveColor(self):
		r = lambda: random.randint(0,255)
		color = ('#%02X%02X%02X' % (r(),r(),r()));
		if color in self.usedList:
			return giveColor()
		else:
			self.usedList.append(color)
			return color

class InterfaceSelect:
	def __init__(self, master):
		self.master = master
		master.title("Interface Selection")
		self.app = None
		self.frame = tk.Frame(self.master, height = 30, width = 270)
		
		self.label = tk.Label(master, text="Please select an interface to capture on.")
		
		iList = getInterfaces()
		
		len_max = getListBoxWidth(iList)
		
		self.lb = tk.Listbox(master, width=len_max)
		
		for item in iList:
			self.lb.insert(tk.END, item)
		
		self.button = tk.Button(master, text="Confirm", command = self._choose)
		
		self.label.pack()
		self.lb.pack()
		self.button.pack(pady=10)
		self.frame.pack()
		
	def _choose(self, event = None):
		try:
			selection = self.lb.get(self.lb.curselection())
			self.newWindow = tk.Toplevel(self.master)
			self.app = MonitorTraffic(self.newWindow, selection)
		except:
			pass
			
class MonitorTraffic(tk.Frame):
	def __init__(self, master, selection):
		tk.Frame.__init__(self, master)
		self.master = master
		master.title("Network Monitor")
		
		self.sniff_control = True
		self.colorPicker = ColorRandomizer()
		# create the frames that will go into the window and place in grid
		self.dFrame = tk.Frame(self.master, borderwidth = 5, bg = "#4e4f51")
		self.mFrame = tk.Frame(self.master, borderwidth = 5, bg = "#414244")
		
		self.dFrame.grid(column = 0, row = 0, sticky = "nsew")
		self.mFrame.grid(column = 1, row = 0, sticky = "nsew")

		
		# the smallest that the grid should be is putting the router at (4,4)
		for i in range(9):
			self.mFrame.grid_rowconfigure(i, minsize = 50)
			self.mFrame.grid_columnconfigure(i, minsize = 50)
		

		self.label = tk.Label(self.dFrame, text="NETWORK DEVICES", bg = "#4e4f51", fg = "#cecece")
		
		# get all the iot devices with netdisco
		devInfoList = self.getDevices()
		self.serverList = []
		self.serverIPs = []
		
		
		# find the longest entry to size the listbox width to fit
		len_max = 0
		for m in devInfoList:
			if len(m.name) > len_max:
				len_max = len(m.name)
		
		self.devListBox = tk.Listbox(self.dFrame, width=len_max, bg = "#4e4f51", fg = "#cecece", borderwidth = 0, highlightthickness = 0)
		
		hosts = list()
		
		# begin interface
		# initialize starting column and row values
		dRow = 5
		dCol = 1
		self.sRow = 0
		self.sCol = 0
		
		i = 0
		
		# make arrays to hold buttons, icons, and canvases
		self.dButtons = []
		self.dButtonIcons = {}
		self.sButtons = []
		self.maxRow, self.maxCol = 7, 9
		self.midRow, self.midCol = 3, 4
		self.canvases = [[0]*self.maxCol for i in range(self.maxRow)] # 9 columns, 8 rows 
		
		
		# directory to find the icons 
		iconFolder = os.path.abspath(os.path.join(os.path.dirname(__file__),'Resources\Icons'))
				
		for f in os.listdir(iconFolder):
			k3y = os.path.basename(f).split('.')[0]
			fullName = "%s\%s"%(iconFolder,f)
			self.dButtonIcons[k3y] = ImageTk.PhotoImage(file=fullName)

		
		self.defaultImg = self.dButtonIcons['device']
		self.serverImgGreen = self.dButtonIcons['green-server']
		self.serverImgRed = self.dButtonIcons['red-server']
		
		
		for dev in devInfoList:
			self.devListBox.insert(tk.END, dev.name)
			hosts.append("ip.addr == " + dev.host)
			
			imgKey = "device"
			for word in dev.model.lower().split(" "):
				if word in self.dButtonIcons:
					imgKey = word
			
			# make device buttons
			dButton = tk.Button(self.mFrame)
			dButton.config(image=self.dButtonIcons[imgKey], bg = "#414244", borderwidth = 0, command = lambda dev=dev: self.showMoreInfo(dev))
			dButton.grid(column = dCol, row = dRow, sticky = "nsew")
			self.dButtons.append(dButton)	
			
			dev.row = dRow
			dev.col = dCol
			
			# increment columns and rows
			dCol += 2
			
			if dCol > 8:
				dRow += 1
				if dRow%2 == 0:
					dCol = 0
				else:
					dCol = 1
			
			i += 1
			
		
				
		captureFilter = "||".join(hosts)
		#Fill grid with canvas in open/blank spots. Assuming 2 rows with every other spot a grid
		self.fillGrid(rows=self.maxRow)
		self.beginCapture(selection, captureFilter)
		
		self.image = ImageTk.PhotoImage(file="Resources\\Icons\\router.png")
		self.router = tk.Button(self.mFrame, highlightthickness = 0)
		self.router.config(image=self.image, bg = "#414244", borderwidth = 0)

		self.label.grid(column=0, row = 0, padx = 10)
		self.devListBox.grid(column = 0, row = 1, padx=10)
		self.router.grid(column = 4, row = 3, padx = 10)
		self.drawAllRoutes(devInfoList)
		
		self.devListBox.bind('<Double-1>', self.showMoreInfoList)
		#self.devListBox.bind('<Double-1>', self.stopReadPackets)
		
	def showMoreInfo(self, dev=None):
		try:
			self.newWindow = tk.Toplevel(self.master)
			self.app = MoreDetailsDev(self.newWindow, dev)
		except:
			pass
			
	def showMoreServerInfo(self, serv=None):
		try:
			self.newWindow = tk.Toplevel(self.master)
			self.app = MoreDetailsServer(self.newWindow, serv)
		except:
			pass

	def showMoreInfoList(self, event=None):
		dev = self.getSelectedDev()
		self.showMoreInfo(dev)
	
	def getSelectedDev(self):
		currentDev = self.deviceList[0]
		for dev in self.deviceList:
			if self.devListBox.get(self.devListBox.curselection()) == dev.name:
				currentDev = dev
		return currentDev
		
	def drawAllRoutes(self, deviceInfoList):
		
		for dev in deviceInfoList:
			self.drawToRouter(dev.row, dev.col)
			
		for serv in self.serverList:
			self.drawToRouter(serv.row, serv.col)
	
	def fillGrid(self, rows):
		# add canvases to empty grid spaces
		fullRows = [self.midRow + 1, self.midRow -1]
		for i in range(rows):
			colStart = 0
			if(i % 2 == 0):
				colStart = 1
			if i in fullRows:
				self.fillFullRow(i, self.maxCol)
			elif i == self.midRow:
				continue
			else:
				self.fillAltRow(i, colStart, self.maxCol)
		
	def generateCanvas(self):
		canvas = tk.Canvas(self.mFrame)
		canvas.config(width = 50, height = 50, bg = "#414244", bd = 0, relief = 'ridge', highlightthickness = 0)
		return canvas
		
	def fillFullRow(self, row, cols = 7):
		for i in range(cols):
			canvas = self.generateCanvas()
			canvas.grid(column = i, row=row)
			self.canvases[row][i] = canvas
			
	def fillAltRow(self, row, col, maxCol = 7):
		while col < maxCol:
			canvas = self.generateCanvas()
			canvas.grid(column = col, row=row)
			self.canvases[row][col] = canvas
			col += 2
			
	def drawToRouter(self, row, col, color = "white"):
		
		vMove = -1 #vertical movement down
		if row < self.midRow:
			vMove = 1 #Vertical movement up
		
		#Move horizontaly
		cRow = row + vMove
		
		while True:
			
			canvas = self.canvases[cRow][col]
			if vMove is 1:
				self.drawToTop(canvas, color)
			else:
				self.drawToBot(canvas, color)
			if cRow == (self.midRow + (-1) * vMove):
				break;
			if vMove is -1:
				self.drawToTop(canvas, color)
			else:
				self.drawToBot(canvas, color)
			cRow += vMove
		
		hMove = 0
		if col < self.midCol:
			hMove = 1
		elif col > self.midCol:
			hMove = -1
			
		#continue from where vertical movement ended
		if hMove is 1:
			self.drawToRight(self.canvases[cRow][col], color)
		elif hMove is -1:
			self.drawToLeft(self.canvases[cRow][col], color)
		
		col += hMove
		#draw horizontal line until above midpoint
		while col != self.midCol:
			
			self.drawToRight(self.canvases[cRow][col], color)
			self.drawToLeft(self.canvases[cRow][col], color)
			col += hMove
		
		#finish last half line
		if hMove is -1:
			self.drawToRight(self.canvases[cRow][col], color)
		elif hMove is 1:
			self.drawToLeft(self.canvases[cRow][col], color)
			
		#lastly make connector to router
		if vMove is 1:
			self.drawToBot(self.canvases[self.midRow-1][self.midCol], color)
		else:
			self.drawToTop(self.canvases[self.midRow+1][self.midCol], color)
		
	def drawToTop(self, canvas, color):
		canvas.create_line(25, 25, 25, 0, fill=color, width = 1.5)
		
	def drawToBot(self, canvas, color):
		canvas.create_line(25, 25, 25, 50, fill=color, width = 1.5)
			
	def drawToLeft(self, canvas, color):
		canvas.create_line(25, 25, 0, 25, fill=color, width = 1.5)
		
	def drawToRight(self, canvas, color):
		canvas.create_line(25, 25, 50, 25, fill=color, width = 1.5)
		
	def createServer(self, dev, ip, protocol, time, dns, color):
		sIcon = self.serverImgGreen
		
		# check if packet is using unsecure protocol or unecrypted 
		if protocol.lower() == 'http' or protocol.lower()== 'tlsv1.1' or protocol.lower() == 'tlsv1.0' or protocol.lower() == 'telnet' or protocol.lower() == 'rsh' or protocol.lower() == 'snmpv1':
			sIcon = self.serverImgRed
			
		# make server button
		server = Server(dev, self.sRow, self.sCol, ip, protocol, time, dns, color)
		sButton = tk.Button(self.mFrame)
		sButton.config(image=sIcon, bg = "#414244", borderwidth = 0, command = lambda server = server: self.showMoreServerInfo(server))
		sButton.grid(column = self.sCol, row = self.sRow, sticky = "nsew")
		self.sButtons.append(sButton)
		
		self.sCol += 2
		
		if self.sCol > 8:
			self.sRow += 1
			if self.sRow%2 == 0:
				self.sCol = 0
			else:
				self.sCol = 1
		
		return server
		
	def beginCapture(self, selection, captureFilter):
		self.thread_queue = queue.Queue()
		self.new_thread = threading.Thread(target=self.readPackets, kwargs={'selection':selection, 'captureFilter':captureFilter, 'thread_queue':self.thread_queue})
		self.new_thread.start()
		self.after(100,self.listenForPackets)
		
		
	def highlightPath(self, device, server, color="white"):
		self.drawToRouter(device.row, device.col, color)
		self.drawToRouter(server.row, server.col, color)
		
		if color is not "white":
			self.after(800, self.highlightPath, device, server)

	
	def processPacket(self, packet):
		destinationDev = None
		destinationServer = None
		

		pktDNS = ' ' 
		if 'standard query' in (packet.info).lower():
			splitDNS = (packet.info).split(" ")
			pktDNS = splitDNS[len(splitDNS) - 1]
			if('192.168.1.78' in packet.source or '192.168.1.78' in packet.destin):
				print("Looking at string lights")
				print(pktDNS)

			
		for dev in self.deviceList:
			if packet.source == dev.host:
				if ('192.168.' not in packet.destination) and (packet.protocol.lower() != 'mdns') and (packet.protocol.lower() != 'ssdp') and (packet.protocol.lower() != 'icmp') and (packet.protocol.lower() != 'dns'):
					#add new server if it is a new server
					
					server = None
					if packet.destination not in self.serverIPs:
						serverDNS = socket.gethostbyaddr(packet.destination)[0]
						server = self.createServer(dev, packet.destination, packet.protocol, packet.time, serverDNS, dev.color)
						self.serverList.append(server)
						self.serverIPs.append(packet.destination)
						destinationDev = dev
					else:
						k3y = self.serverIPs.index(packet.destination)
						server = self.serverList[k3y]
						destinationDev = dev
					dev.addServer(server)
					destinationServer = server
					
			elif packet.destination == dev.host:
				if ('192.168.' not in packet.source) and (packet.protocol.lower() != 'mdns') and (packet.protocol.lower() != 'ssdp') and (packet.protocol.lower() != 'icmp') and (packet.protocol.lower() != 'dns'):
					#add new server if it is a new server
					server = None
					if packet.source not in self.serverIPs:
						serverDNS = socket.gethostbyaddr(packet.source)[0]
						server = self.createServer(dev, packet.source, packet.protocol, packet.time, serverDNS, dev.color)
						self.serverList.append(server)
						self.serverIPs.append(packet.source)
						destinationDev = dev
					else:
						k3y = self.serverIPs.index(packet.source)
						server = self.serverList[k3y]
						destinationDev = dev
					dev.addServer(server)
					destinationServer = server
					
				break
		
		#Link packet info to a server and device, then send to highlightPath
		if destinationDev != None and destinationServer != None:
			
			self.highlightPath(destinationDev, destinationServer, destinationDev.color)
			destinationServer.time = packet.time			
		
	
	def readPackets(self, selection, captureFilter, thread_queue):
		#do things
		capture = pyshark.LiveCapture(interface=selection, only_summaries = True)
		
		while self.sniff_control:
			capture.sniff(packet_count=100)
			for packet in capture:
				thread_queue.put(packet)
		
	def listenForPackets(self):
		try:
			for packet in iter(self.thread_queue.get_nowait, None):
				self.processPacket(packet)
		except:
			pass
			
		self.after(100,self.listenForPackets)
		
		
	def stopReadPackets(self, butt=None):
		self.sniff_control = False
		
		
			
	def getDevices(self):
		deviceInfoList = list()
		netdis = NetworkDiscovery()
		try:
			netdis.scan()
		except FileNotFoundError as e:
			print( e)
		i = 0

		for dev in netdis.discover():
			temp = netdis.get_info(dev)
			for entry in temp:
				# try to resolve device name 
				if entry.get('name') is None:
					try:
						properties = entry.get('properties')
						if 'fn' in properties:
							dname = properties.get('fn')
						else:
							dname = 'None'
					except:
						dname = 'None'
				else:
					dname = entry.get('name')
					
				# try to resolve model name 
				if entry.get('model_name') is None:
					try:
						properties = entry.get('properties')
						if 'md' in properties:
							model = properties.get('md')
						else:
							model = 'None'
					except:
						model = 'None'
				else:
					model = entry.get('model_name')
					
				# try to resolve mac address
				if entry.get('mac_address') is None:
					try:
						properties = entry.get('properties')
						if 'mac' in properties:
							mac = properties.get('mac')
						else:
							mac = 'None'
					except:
						mac = 'None'
				else:
					mac = entry.get('mac_address')

				if '(' in dname:
					dname = dname.split('(')[0]
				device = Device(dev, dname, mac, entry.get("host"), model, self.colorPicker.giveColor())
				deviceInfoList.append(device)
		netdis.stop()
		
		cleanDevList = list()
		
		for d in deviceInfoList:
			if ('group' not in d.name) and ('None' not in d.name):
				cleanDevList.append(d)
				
		self.deviceList = cleanDevList
		return cleanDevList

class MoreDetailsDev(tk.Frame):
	def __init__(self, master, dev):
		tk.Frame.__init__(self, master)
		
		self.master = master
		master.title("Device Information")
		
		self.devFrame = tk.Frame(self.master, borderwidth = 5, bg = "#414244")
		self.devFrame.grid(column = 0, row = 0, sticky = "nsew")
		
		self.devFrame.grid_rowconfigure(1, minsize = 10)
		
		self.header = tk.Label(self.devFrame, text="Information for " + dev.name, bg = "#414244", fg = "#cecece")
		self.hostL = tk.Label(self.devFrame, text="Host IP: ", bg = "#414244", fg = "#cecece")
		self.modelL = tk.Label(self.devFrame, text="Model: ", bg = "#414244", fg = "#cecece")
		self.macL = tk.Label(self.devFrame, text="MAC: ", bg = "#414244", fg = "#cecece")
		
		self.hostInfoL = tk.Label(self.devFrame, text=dev.host, bg = "#414244", fg = "#cecece")
		self.modelInfoL = tk.Label(self.devFrame, text=dev.model, bg = "#414244", fg = "#cecece")
		self.macInfoL = tk.Label(self.devFrame, text=dev.mac, bg = "#414244", fg = "#cecece")
		
		self.header.grid(column=0, row = 0, padx = 10, sticky = "w")
		self.hostL.grid(column=0, row = 2, padx = 10, sticky = "w")
		self.modelL.grid(column=0, row = 3, padx = 10, sticky = "w")
		self.macL.grid(column=0, row = 4, padx = 10, sticky = "w")
		
		self.hostInfoL.grid(column=1, row = 2, padx = 10, sticky = "w")
		self.modelInfoL.grid(column=1, row = 3, padx = 10, sticky = "w")
		self.macInfoL.grid(column=1, row = 4, padx = 10, sticky = "w")
		
		
		currentRow = 1
		for serv in dev.dest:
			servFrame = tk.Frame(self.master, borderwidth = 5, bg = "#414244")
			servFrame.grid(column = 0, row = currentRow, sticky = "nsew")
			
			destL = tk.Label(servFrame, text="Destination IP: ", bg = "#414244", fg = "#cecece")
			dnsL = tk.Label(servFrame, text="DNS: ", bg = "#414244", fg = "#cecece")
			protoL = tk.Label(servFrame, text="Protocol: ", bg = "#414244", fg = "#cecece")
			timeL = tk.Label(servFrame, text="Time Between First and Current Packet: ", bg = "#414244", fg = "#cecece")
			
			
			destInfoL = tk.Label(servFrame, text=serv.ip, bg = "#414244", fg = "#cecece")
			dnsInfoL = tk.Label(servFrame, text=serv.dns, bg = "#414244", fg = "#cecece")
			protoInfoL = tk.Label(servFrame, text=serv.protocol, bg = "#414244", fg = "#cecece")
			timeInfoL = tk.Label(servFrame, text=serv.time, bg = "#414244", fg = "#cecece")
			
			destL.grid(column=0, row = 5, padx = 10, sticky = "w")
			dnsL.grid(column=0, row = 6, padx = 10, sticky = "w")
			protoL.grid(column=0, row = 7, padx = 10, sticky = "w")
			timeL.grid(column=0, row = 8, padx = 10, sticky = "w")
			
			destInfoL.grid(column=1, row = 5, padx = 10, sticky = "w")
			dnsInfoL.grid(column=1, row = 6, padx = 10, sticky = "w")
			protoInfoL.grid(column=1, row = 7, padx = 10, sticky = "w")
			timeInfoL.grid(column=1, row = 8, padx = 10, sticky = "w")
			currentRow += 1
		
	
	
class MoreDetailsServer(tk.Frame):
	def __init__(self, master, serv):
		tk.Frame.__init__(self, master)
		self.master = master
		master.title("Server Information")
	
		servFrame = tk.Frame(self.master, borderwidth = 5, bg = "#414244")
		servFrame.grid(column = 0, row = 0, sticky = "nsew")
		
		destL = tk.Label(servFrame, text="Server IP: ", bg = "#414244", fg = "#cecece")
		dnsL = tk.Label(servFrame, text="DNS: ", bg = "#414244", fg = "#cecece")
		protoL = tk.Label(servFrame, text="Protocol: ", bg = "#414244", fg = "#cecece")
		timeL = tk.Label(servFrame, text="Time Between First and Current Packet: ", bg = "#414244", fg = "#cecece")
		
		destInfoL = tk.Label(servFrame, text=serv.ip, bg = "#414244", fg = "#cecece")
		dnsInfoL = tk.Label(servFrame, text=serv.dns, bg = "#414244", fg = "#cecece")
		protoInfoL = tk.Label(servFrame, text=serv.protocol, bg = "#414244", fg = "#cecece")
		timeInfoL = tk.Label(servFrame, text=serv.time, bg = "#414244", fg = "#cecece")
		
		destL.grid(column=0, row = 5, padx = 10, sticky = "w")
		dnsL.grid(column=0, row = 6, padx = 10, sticky = "w")
		protoL.grid(column=0, row = 7, padx = 10, sticky = "w")
		timeL.grid(column=0, row = 8, padx = 10, sticky = "w")
		
		destInfoL.grid(column=1, row = 5, padx = 10, sticky = "w")
		dnsInfoL.grid(column=1, row = 6, padx = 10, sticky = "w")
		protoInfoL.grid(column=1, row = 7, padx = 10, sticky = "w")
		timeInfoL.grid(column=1, row = 8, padx = 10, sticky = "w")
		
		
def getInterfaces():
	iList = list()
	addrs = psutil.net_if_addrs()
	for x in addrs.keys():
		iList.append(x)
	return iList
	
def getListBoxWidth(list):
		len_max = 0
		for m in list:
			if len(m) > len_max:
				len_max = len(m)
		return len_max
		

	

def main():
	root = tk.Tk()
	app = InterfaceSelect(root)
	root.protocol('WM_DELETE_WINDOW', lambda: test(app, root))
	root.mainloop()
	
def test(app, root):
	try:
		app.app.stopReadPackets()
	except:
		pass
	root.destroy()
	sys.exit()
	
	
if __name__ == '__main__':
	main()
	
