# PersonalAR
Sandbox for testing experimental AR-focused personal computing interfaces in HoloLens 2.

# Getting Started
Follow [these instructions](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/install-the-tools) to install necessary software. Don't forgot to make the modifications to the UWP workload (i.e. Windows 10 SDK 18632) which is required to build this project. These instructions have been tested on the following versions:
   - Unity 2020.3.20f1 LTS
   - Windows 10 SDK 18632
   - Visual Studio Community 2019 ver. 16.11.4

Once installed, clone this repo and open the project in Unity or Unity Hub.

# Project Structure
The main scene can be found under Assets > PersonalAR > Scenes.

```
PersonalAR
|__ Anchors               create and manage anchors/reference points, usually user-annotated objects
|
|__ Applications
    |__ Core              application system and life cycle management
    |__ ExperimentApps    individual application implementations for multitasking experiments
    |__ MiscApps          other application implementations not currently used
    |__ Utilities         misc. code shared accross apps
|
|__ Common                components shared throughout the project
|__ Debug                 components only used for debugging
|__ Scenes                
|__ UI                    components used for UI
```

# Testing in Unity
MRTK provides in-editor input simulation in Unity. More detailed information [can be found here](https://docs.microsoft.com/en-us/learn/modules/learn-mrtk-tutorials/1-6-interaction-models). Here is a brief summary of how it works:

How to move around in the scene:

- Use **W/A/S/D** keys to move the camera forward/left/back/right.
- Use **Q/E** to move the camera vertically.
- Press and hold the **right mouse button** to rotate the camera.

How to simulate hand input:

- Press and hold the **space bar** to enable the right hand.
- While holding the space bar, move your mouse to move the hand.
- Use the **mouse scroll wheel** to adjust the depth of the hand.
- Click the **left mouse button** to simulate the pinch gesture.
- Use **T/Y** keys to make the hand persistent in the view.
- Hold the **CTRL** key and move the mouse to rotate the hand.
- Press and hold the **left shift key** to enable the left hand.

It is recommended that you turn on left hand persistence in the game view using **T** during play mode. This is because the main mode of user interaction is through the hand menu, which appears by rotating your left palm towards you. Hand persistence allows the left hand to remain in its position so you can interact with it using the simulated right hand.

# Building and Deploying an App Package
```As of MRTK 2.7.2, there is a bug with the MRTK Build Window. Until it's fixed, use the instructions below.```

Open the build settings window in Unity (Ctrl + Shift + B).

Switch the platform to Universal Windows Platform. Use the below settings:
```
Target Device: HoloLens
Architecture: ARM64
Build Type: DRD Project
Target SDK Version: Latest Installed
Minimum Platform Version: 10.0.10240.0
Visual Studio Version: Latest Installed
Build and Run on: USB Device
Build configuration: Release
Copy References: Check
Copy PDB files: Check
Development Build: Check
```

When building the Unity Project, **only select "Build"**. Do not use "Build And Run". When building, Unity will prompt you for a directory to build in. I recommended creating a separate Builds directory in your project.

Once Unity completes the build, it should open a window with a Visual Studio file (**PersonalAR.sln** file) in it. Double click that to open it in Visual Studio 2019.

In Visual Studio, on the right, there should be a Solution Explorer. If you instead see Git Changes, just change the tab over to Solution Explorer. Right click the bolded and highlighed PersonalAR (Universal Windows) solution and go to **Publish > Create App Packages**. It may take a few minutes to load before the option appears.

Use the defaults and keep clicking next until you see "Select and Configure packages". Under architectures, check **ARM and ARM64** and uncheck the rest. Under Solution Configuration, choose **Release (ARM) and Release (ARM64)**. Click Create.

Once it's done compiling, you should be able to find your app packages (**.appx** files) at the specified output location.

In order to deploy the app package onto a HoloLens, follow the steps [listed here using the Device Portal](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/advanced-concepts/using-the-windows-device-portal) from the beginning up until "Sideloading applications". You only need to set up the device portal the first time you deploy, the connection settings should be saved on your PC for the next time.

Additionally, pre-built app packages can be found [under releases](https://github.com/b-huynh/PersonalAR/releases).
