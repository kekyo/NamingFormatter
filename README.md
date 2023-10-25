﻿# NamingFormatter

![NamingFormatter](https://raw.githubusercontent.com/kekyo/NamingFormatter/master/Images/NamingFormatter.128.png)

## Status

[![Project Status: Active – The project has reached a stable, usable state and is being actively developed.](https://www.repostatus.org/badges/latest/active.svg)](https://www.repostatus.org/#active)

| |Build|NuGet|
|:----|:----|:----|
|master|[![NamingFormatter CI build (master)](https://github.com/kekyo/NamingFormatter/workflows/.NET/badge.svg?branch=master)](https://github.com/kekyo/NamingFormatter/actions)|[![NuGet NamingFormatter (master)](https://img.shields.io/nuget/v/NamingFormatter.svg?style=flat)](https://www.nuget.org/packages/NamingFormatter)|
|devel|[![NamingFormatter CI build (master)](https://github.com/kekyo/NamingFormatter/workflows/.NET/badge.svg?branch=master)](https://github.com/kekyo/NamingFormatter/actions)|[![MyGet NamingFormatter (devel)](https://img.shields.io/myget/kekyo/v/NamingFormatter.svg?style=flat&label=myget)](https://www.myget.org/feed/kekyo/package/nuget/NamingFormatter)|

## What is this?

* NamingFormatter is extended System.String.Format method on .NET.
* Standard Format method required numbering indexed place-holder.
  * You probably understand this:

``` csharp
// C# String interporation style:
// (These argument variables fixedup at compile time)
var formatted =
    $"Index0:{arg0}, Index1:{arg1}";

// Old school style:
var formatted = string.Format(
    "Index0:{0}, Index1:{1}",
    arg0,
    arg1);
```

* NamingFormatter can use named key-value arguments, and will fixup at runtime. For example:

``` csharp
var keyValues = new Dictionary<string, object>
{
    { "lastName", "Matsui" },
    { "firstName", "Kouji" },
    { "foo", "bar" },
};
var formatted = Named.Format(
    "FirstName:{firstName}, LastName:{lastName}",
    keyValues);
```

* Of course, we can use the format options.

``` csharp
var keyValues = new Dictionary<string, object>
{
    { "date", DateTime.Now },
    { "value", 123.456 },
    { "foo", "bar" },
};
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}",
    keyValues);
```

* We can use easier with tuple expression:

``` csharp
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}",
    ( "date", DateTime.Now ),
    ( "value", 123.456 ),
    ( "foo", "bar" ));
```

## Features

* Easy standard replacement from System.String.Format method.
* TextWriter version included (WriteFormat extension method). And has asynchronous method overloads (Task).
* Many variation overloads (Dictionary, IReadOnlyDictionary, Predicate delegate, Selector delegate, IFormatProvider, KeyValuePair and ValueTuple with variable length parameters).
* Can use structual-key, traverse both public properties and fields.
* Applied C# nullable-reference type attribtues.

## Benefits

* Flexible argument matching. Useful dynamic interpretation.
* Format string human-readable/customizable improvement.

## Environments

* .NET 8 to 5
* .NET Standard 1.0, 2.0, 2.1 (Will effect .NET Core 1.0-3.1)
* .NET Framework 3.5, 4.0 with client profile, 4.5 to 4.8.1

## How to use

* Search NuGet package and install ["NamingFormatter"](https://www.nuget.org/packages/NamingFormatter).
* View more sample:

``` csharp
using NamingFormatter;

// Mostly standard key-value combination in manually.
// We can use with tuples (excepts net35-client and net40-client).
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    ("value", 123.456),
    ("name", "Kouji"),
    ("date", DateTime.Now));
```

``` csharp
using NamingFormatter;

// All overloads have an optional fallback delegate.
// You can handle unknown key identity when the format string contains it.
// (Default behavior will throw exception)
var formatted = Named.Format(
    "Date:{date}, Value:{VALUE}, Name:{name}",
    key => "***",      // fallback delegate makes safer from exceptions.
    ("value", 123.456),
    ("name", "Kouji"),
    ("date", DateTime.Now));
```

``` csharp
using NamingFormatter;

// Structual-key (Traverse properties and fields by dot-notation)
var formatted = Named.Format(
    "TOD-Millisec:{date.TimeOfDay.TotalMilliseconds}",
    ("date", DateTime.Now));
```

``` csharp
using NamingFormatter;

// Format to TextWriter.
var sw = new StreamWriter(stream);
sw.WriteFormat(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    ("value", 123.456),
    ("name", "Kouji"),
    ("date", DateTime.Now));
sw.Flush();

// Format to TextWriter with async-await
var sw = new StreamWriter(stream);
await sw.WriteFormatAsync(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    ("value", 123.456),
    ("name", "Kouji"),
    ("date", DateTime.Now));
await sw.FlushAsync();
```

``` csharp
using NamingFormatter;

// Full-interactive (callback) format.
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    key => key switch
    {
        "name" => "Kouji";
        "date" => DateTime.Now;
        "value" => 123.456;
        _ => throw new FormatException();
    });
```

``` csharp
using NamingFormatter;

// IFormatProvider supported.
var formatted = Named.Format(
    new CultureInfo("fr-FR"),
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    ("value", 123.456),
    ("name", "Kouji"),
    ("date", DateTime.Now));
```

``` csharp
using NamingFormatter;

// Easy parametric helper
// (Named.Pair() method will generate KeyValuePair<string, object?>)
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    Named.Pair("value", 123.456),
    Named.Pair("name", "Kouji"),
    Named.Pair("date", DateTime.Now));
```

## TODO

* F# friendly version.

## License

* Copyright (c) 2016-2020 Kouji Matsui
* Under Apache v2

## History

* 2.3.0:
  * Added .NET 8.0 RC2 assembly.
* 2.2.0:
  * Added field lookup ability.
  * Reduced package dependency.
* 2.1.0:
  * Added .NET 6.0 assembly.
  * Added source link attributes.
* 2.0.22:
  * Added fallback delegate features.
  * Included xml documents.
* 2.0.18: Added net461 and net47 assemblies because reduce conflict between netstandard2.0.
* 2.0.17:
  * Added ValueTuple overloads.
  * Added asynchronous overloads.
* 2.0.16: Fixed not including net35 assembly.
* 2.0.15:
  * Breaking change: Changed the NuGet package name from "NamingFormatter" to "NamingFormatter".
  * Breaking change: Changed namespace name from "CenterCLR" to "NamingFormatter".
  * Added some target frameworks.
  * Omitted strong-key signing.
  * Switched and aggregated CI to GitHub Actions.
* 2.0.0: Upgraded new MSBuild format and omit PCL versions.
* 1.1.1: Fixed via CI (AppVeyor, Fixed RelaxVersioner)
* 1.1.0: Add support platform .NET Core (formally "dnxcore").
* 1.0.0: Omit IFormatProvider method extension attribute.
* 0.9.6: Versioning fixed.
* 0.9.5: Add nuget package, Support structual-key, Support .NET 2/.NET 3.5.
* 0.0.0: Initial commit.
