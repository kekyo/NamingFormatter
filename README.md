# NamingFormatter
## What is this?
* NamingFormatter is extended System.String.Format method on .NET Framework.
* Standard Format method required numbering indexed place-holder.

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

## Benefits
* Flexible argument matching. Useful dynamic interpretation.
* Format string human-readable/customizable improvement.

## Environments
* .NET Framework Portable class library (Profile1 or Profile259)

## TODO
* Current no NuGet package.
* F# friendly version.

## License
* Under Apache v2
* COpyright (c) Kouji Matsui

## History
0.0.0.0: Initial commit.
