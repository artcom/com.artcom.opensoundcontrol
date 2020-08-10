# About Open Sound Control

Use the Open Sound Control package to send and receive OSC Messages via UDP. For example, use Open Sound control to receive touch control messages from media control hardware. The Open Sound Control package also includes the ability to latch onto network devices and send debug messages.


# Installing Open Stage Control

To install this package, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html). 


<a name="UsingPackageName"></a>
# Using &lt;package name&gt;
## Input
Simply add an ``Osc Input`` to a gameobject, specify the ``In Port`` to receive OSC messages on that port. In Editor
you're able to click ``Learn API`` in which all API paths for the remote device can be mapped out. For extra convenience,
you should max out values to both ends, which is then able to normalize these values for you.

## Output
Adding an ``Osc Output`` to a gameobject allows for sending OSC Messages to a specified client.

To send a message, you can do so as following:

```csharp
GetComponent<OscOutput>().Send(new OscMessage("/foo", 10f, 12, "message", ...));
```

Alternatively you can in Code initialize a new ``OscClient``, which can then send on any port any message. 


# Technical details
## Requirements

This version of Open Sound Control is compatible with the following versions of the Unity Editor:

* 2019.1 and later (recommended)

Ensure firewall settings and similar environment settings of the editor and player when deploying.

## Document revision history
| Date        | Reason                                         |
|-------------|------------------------------------------------|
| Aug 6, 2020 | Document created. Matches package version 1.0. |
