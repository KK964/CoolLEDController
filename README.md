# CoolLED Controller

API for interfacing with CoolLED devices.

## Methods

### CoolLEDController.BLEController

```cs
BLEDevice GetDevice(string manufacturerID);
List<AdDevice> GetNearbyPanels();
```

### CoolLEDController.BLEDevice

```cs
BLEDevice(ulong bluetoothAddress, CoolLEDController.BLECommandWriter  writer);
void SendText(string text);
void SendImage(CoolLEDProtocols.Frame frame);
void SendAnimation(CoolLEDProtocols.Frames frames);
void SendMode(CoolLEDController.Utils.AnimationState state);
void SendSpeed(int speed);
```

### CoolLEDProtocols.LEDState

```cs
LEDState(bool on);
```

### CoolLEDProtocols.Frame

```cs
Frame();
Frame(List<CoolLEDProtocols.LEDState> data);
voic AddRow(List<CoolLEDProtocols.LEDState> row);
```

### CoolLEDProtocols.Frames

```cs
Frames(List<CoolLEDProtocols.Frame> frames, int speed);
void AddFrame(CoolLEDProtocols.Frame frame);
```

### CoolLEDController.Utils.AnimationState

```cs
STATIC,
SCROLL_LEFT,
SCROLL_RIGHT,
SCROLL_UP,
SCROLL_DOWN,
DROP_DOWN,
SPREAD_OUT,
LASER
```

### CoolLEDController.Utils.ImageUtils

```cs
Frames ConvertGifToFrames(Image image, int speed, bool invert = false);
Frame ConvertImageToFrame(Image image, bool invert = false);
Frame ConvertBitmapToFrame(Bitmap bitmap, bool invert = false);
List<Bitmap> ConvertGif(Image image);
Bitmap MakeGrayScale3(Bitmap bitmap);
Bitmap Resize(Bitmap original);
Image DownloadImage(string url);
```

---

## Usage

```cs
// Top
using System;
using System.Drawing;
using System.Collections.Generic;
using CoolLEDController;
using CoolLEDController.Utils;
using CoolLEDProtocols;

// In main

// Search for nearby panels
List<AdDevice> nearbyPanels = BLEController.GetNearbyPanels();

// List panels by manufacturer ID
foreach (AdDevice panel in nearbyPanels)
{
    Console.WriteLine(panel.ManufacturerID);
}


// Connect to a panel
BLEDevice panel = BLEController.GetDevice("0xC78D: 8D-FC-02-26-FF-FF-01");


// Displaying content

// NOTE: You should only be using 1 of these send methods at a time, otherwise it will just overwrite the previous output.

// Send text
panel.SendText("Hello World!");

// Send image
Image image = ImageUtils.DownloadImage("https://cdn.discordapp.com/attachments/879030223914561588/998079521767030824/Untitled-2.png");
Frame frame = ImageUtils.ConvertImageToFrame(image);
panel.SendImage(frame);

// Send animation
Image gif = ImageUtils.DownloadImage("https://cdn.discordapp.com/attachments/879030223914561588/998080020876623983/Untitled-3.gif");
Frames frames = ImageUtils.ConvertGifToFrames(gif, 40);
panel.SendAnimation(frames);

// Send mode
panel.SendMode(AnimationState.SCROLL_LEFT);

// Send speed
panel.SendSpeed(40);
```
