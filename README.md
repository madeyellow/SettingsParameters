# What is it?
A simple and easy-to-use wrapper around Unity's **PlayerPrefs** class, which allows you to read/write settings. In addition you'll also gain:
* Value **change event** for extra usability;
* Built-in **value caching** for maximum performance;
* Auto reading of last saved value;
Set this in pair lines of code - and you're ready to go!
# How to install Trackable to my Unity project?
Use the Unity Package Manager (in Unity’s top menu: **Window > Package Manager**), click "+" icon, select **"Add package from git URL"** and type URL of this repository.
# Getting started
To start using this library, you must firstly declare your parameters and then just use them.
## Step 1. Declare your parameters.
In the example below, I showed a static class with 2 parameters: **Camera rotation speed** and **Camera axis inversion flag**. If you want to see the list of all implemented parameters and how to use them — scroll to the next section.

```csharp
using MadeYellow.SettingsParameters;

public static class GameSettings
{
    /// <summary>Tells how fast camera should rotate</summary>
    public static FloatSettingsParameter CameraRotationSpeed = 
        new FloatSettingsParameter("CAMERA_ROTATION_SPEED", 200f);
    
    /// <summary>Should camera vertical axis be inverted or not</summary>
    public static BooleanSettingsParameter InvertCameraVerticalAxis = 
        new BooleanSettingsParameter("CAMERA_VERTICAL_AXIS_INVERSION", false);
}
```
Basically, each parameter (it doesn't matter what type it is, is constructed with use of the following arguments:
* **prefrencesKey** — this is a string key of a parameter (aka it's name). It must be unique per parameter;
* **defaultValue**  — when there is no value (for this parameter) previously assigned by the player provided - this one will be returned;

All you need is to provide **a unique name** for your parameter and a **default value* (if it's game first launch). That's all.
## Step 2. Reading/writing parameter value.
When you want to **get a value** of a certain parameter, you can do the following: 
```csharp
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Just call your parameter and ask for a Value
    private float RotationSpeed => GameSettings.CameraRotationSpeed.Value;

    private void LateUpdate()
    {
        transform.Rotate(new Vector3(0f, RotationSpeed, 0f) * Time.deltaTime);
    }
}
```
Setting a value into the parameter is easy as:
```csharp
private void ExampleOfSettingValueIntoTheParameter(float value)
{
    GameSettings.CameraRotationSpeed.Value = value;
}
```
Next time when you'll ask for that parameter's value it will return the last one you've (player) have left it there! Even after game restart! Or even after it's deletition. All thanks to internal use of Unity's **PlayerPrefs**.
# Supported types
We already have implemented several parameter types:
* **int** — use *IntegerSettingsParameter*;
* **float** — use *FloatSettingsParameter*;
* **bool** — use *BooleanSettingsParameter*;
* **string** — use *StringSettingsParameter*;
 
If you want to have a custom type you may either implement it yourself (it's fairly easy, jsut look at the source code) or create as issue and I'll do it and add to the library (if it'll be usefull for others).
# Change tracking
If you want to, you may use a built-in event that triggers each time a **Value** of parameter changes (except reading the value during construction).
```csharp
private void ExampleOfUsingTheChangeEvent()
{
    GameSettings.InvertCameraVerticalAxis.OnValueChanged.AddListener(() =>
    {
        // Your code here. It will be called only when player have changed value of parameter
    });
}
```
# Recomendations
## Use meaningfull names
Use a **prefrencesKey** that holds some meaning. Don't use spaces. It's just in case if you'll decide to remove that library and use **PlayerPrefs** yourself - this way your palyer's won't loose any settings they set up (in case you'll reuse same keys).
## Performance considerations
You may read **Value** of parameter as many times as you want, no need to cache it yourself, 'cause base class already caches it for you & updates only if **Value** is changed.
Write performance also shouldn't be an issue, but if it's - create an issue and describe your case.
