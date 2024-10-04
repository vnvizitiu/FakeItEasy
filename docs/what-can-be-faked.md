# What can be faked

## What types can be faked?

FakeItEasy uses
[Castle DynamicProxy](https://www.castleproject.org/projects/dynamicproxy/)
to create fakes. Thus, it can fake just about anything that could
normally be overridden, extended, or implemented.  This means that the
following entities can be faked:

* interfaces
* classes that
    * are not sealed,
    * are not static, and
    * have at least one public or protected constructor whose arguments FakeItEasy can construct or obtain
* delegates

Note that special steps will need to be taken to
[fake internal interfaces and classes](how-to-fake-internal-types.md).

### Types whose methods have `in` parameters

Due to deficiencies in earlier .NET framework releases, generic types that contain methods having
a parameter modified by the `in` keyword cannot be faked by FakeItEasy running on target frameworks
earlier than .NET 6.

### Where do the constructor arguments come from?
  
* they can be supplied via `WithArgumentsForConstructor` as shown in
  [creating fakes](creating-fakes.md), or
* FakeItEasy will use [dummies](dummies.md) as arguments

## What members can be overridden?

Once a fake has been constructed, its methods and properties can be
overridden if they are:

* virtual,
* abstract, or
* an interface method when an interface is being faked

Note that this means that static members, including extension methods,
**cannot** be overridden.

### Methods that return values by reference

Methods that return values by reference (officially called "[reference return values](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/declarations#what-is-a-reference-return-value)") cannot be invoked on a Fake. Any attempt to do so will result in a `NullReferenceException` being thrown.
