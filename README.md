# NamingFormatter
## What is this?
* NamingFormatter is extended System.String.Format method on .NET Framework.
* Standard Format method required numbering indexed place-holder.
  * You probably understand this:

``` csharp
var formatted = string.Format(
    "Index0:{0}, Index1:{1}",
    arg0,
    arg1);
```

* NamingFormatter can use named key-value arguments. For example:

``` csharp
var keyValues = new Dictionary<string, object>
{
    { "lastName", "Matsui" }
    { "firstName", "Kouji" },
};
var formatted = Named.Format(
    "FirstName:{firstName}, LastName:{lastName}",
    keyValues);
```

* Of cource, format option can use.

``` csharp
var keyValues = new Dictionary<string, object>
{
    { "date", DateTime.Now },
    { "Value", 123.456 }
};
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}",
    keyValues);
```

## Features
* Easy standard replacement from System.String.Format method.
* TextWriter version included (WriteFormat extension method).
* Any variation overloads (Dictionary, IReadOnlyDictionary, Predicate delegate, Selector delegate, IFormatProvider).
* Can use structual-key, traverse public properties.

## Benefits
* Flexible argument matching. Useful dynamic interpretation.
* Format string human-readable/customizable improvement.

## Environments
* .NET Framework Portable class library (Profile1 or Profile259)
* .NET Framework 2.0/3.5

## How to use
* Search NuGet package and install "CenterCLR.NamingFormatter". https://www.nuget.org/packages/CenterCLR.NamingFormatter
* View more sample:

``` csharp
// Easy parametric helper.
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    Named.Pair("value", 123.456),
    Named.Pair("name", "Kouji"),
    Named.Pair("date", DateTime.Now));
    
// Structual-key (Traverse properties by dot-notation)
var formatted = Named.Format(
    "TOD-Millisec:{date.TimeOfDay.TotalMilliseconds}",
    Named.Pair("date", DateTime.Now));
    
// Format to TextWriter.
var sw = new StreamWriter(stream);
sw.WriteFormat(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    Named.Pair("value", 123.456),
    Named.Pair("name", "Kouji"),
    Named.Pair("date", DateTime.Now));
sw.Flush();
    
// Full-interactive (callbacked) format.
var formatted = Named.Format(
    "Date:{date:R}, Value:{value:E}, Name:{name}",
    key =>
    {
        switch (key)
        {
            case "name": return "Kouji";
            case "date": return DateTime.Now;
            case "value": return 123.456;
            default: throw new FormatException();
        }
    });
```
## TODO
* F# friendly version.

## License
* Copyright (c) 2016 Kouji Matsui
* Under Apache v2

## History
* 0.9.5: Add nuget package, Support structual-key, Support .NET 2/.NET 3.5.
* 0.0.0: Initial commit.
