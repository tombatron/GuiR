# GuiR 
## Graphical User Interface for Redis

[![Build status](https://ci.appveyor.com/api/projects/status/f8grd87h6nemtu8q/branch/master?svg=true)](https://ci.appveyor.com/project/tombatron/guir/branch/master)

### What is this?

So basically, what we have here is a WPF wrapper around the [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) library. 

The application is intended to serve as a simple means at inspecting a Redis database. There's no installer, it's completely open source, and completely free. 

This project actually started as a joke. I initially intended to create the user interface using the [gui.cs](https://github.com/migueldeicaza/gui.cs) library by Miguel de Icaza. But I could just never get it to work the way I intended. So I decided that it might be fun to hack around with WPF. 

And here we are. 

### What is with the name?

That's simple. I needed to pick a name, I'm awful at naming things, so we're left with an awful name. 

(ﾉ☉ヮ⚆)ﾉ ⌒*:･ﾟ✧

### What can it do?

Currently has read-only support for the following Redis types...

* String
* List
* Hash
* Set
* SortedSet
  * **Geo** - This data type isn't really anything other than a sorted set with built in handing that mutates the score of an item into a coordinate pair. 
* HyperLogLog
* Stream

Additionally, the application will retrieve for format server information retrieved via the Redis `INFO` command.

#### What about large databases?

Since I have a professional interest in analyzing Redis databases that contain more than a million keys, I've paid special attention to ensure that this application can handle that without becoming unresponsive. 

### What will it be able to do in the future?

In the future I intend to add the following:

* Support for adding/updating values for all Redis data types. 

* Ability to display JSON data in a "pretty" way. 

* Ability to see the `SLOWLOG`.

* Ability to see the current client list.

* Support for the RediSearch plugin.

* Ability to supply custom value handlers. 

  * Imagine if you stored binary ProtoBuf encoding blobs in Redis and you'd like to be able to audit them...

(•_•) ( •_•)>⌐■-■ (⌐■_■)

### Requirements

Windows

.NET Framework 4.7+ (There's no installer so you need to ensure you have this before executing the exe)

- [Install .NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)

A Redis server to connect to.