tel: Protocol Handler
=====================

Accepts `tel:` links on Windows Hosts and dials the given number on the configured TAPI line.

The first time you start the application, it will ask for the TAPI line to use.

![](screenshots/select-line.png)

All future calls will then directly be dialed on that line.

> To reconfigure, just start the application again, without parameters.

Versioning
----------
This project uses [Automatic Versions](https://marketplace.visualstudio.com/items?itemName=PrecisionInfinity.AutomaticVersions) for versioning.

Signing
-------
Historically, it is required that binary signing with Visual Studio is as fucked up of a task as humanly imaginable.
Thus, there are a few common errors you are guaranteed to run into when attempting to publish a release of this code. These are usually all caused by having the `.pfx` be slightly off of what Visual Studio expects.

In general, you're going to want to place your certificate in the `.certificates` folder and then go through the steps outlined in https://stackoverflow.com/a/39196724/259953

License
-------
GNU GPL v3.0
