<img src="https://fakeiteasy.github.io/img/fakeiteasy_logo_512.png" width="256px" alt="Are you mocking me?" />

[![NuGet version](https://img.shields.io/nuget/v/FakeItEasy.svg?style=flat)](https://www.nuget.org/packages/FakeItEasy)
[![Build status](https://github.com/FakeItEasy/FakeItEasy/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/FakeItEasy/FakeItEasy/actions/workflows/ci.yml?query=branch%3Amaster)

A .NET dynamic fake library for creating all types of fake objects, mocks, stubs etc.

* Easier semantics, all fake objects are just that - fakes - the use of the fakes determines whether they're mocks or stubs.
* Context-aware fluent interface guides the developer.
* Designed for ease of use.
* Full compatibility with both C# and VB.Net.

## Faking amazing example

```c#
// Creating a fake object is very easy!
// No mocks, or stubs; everything's a fake.
var shop = A.Fake<ICandyShop>();

// Easily set up a call to return a value.
var lollipop = new Lollipop();
A.CallTo(() => shop.GetTopSellingCandy()).Returns(lollipop);

// Exercise your system under test by using the fake as you
// would an instance of the faked type.
var customer = new SweetTooth();
customer.BuyTastiestCandy(shop);

// Asserting uses the same syntax as configuring calls.
A.CallTo(() => shop.BuyCandy(lollipop)).MustHaveHappened();
```

## Resources

* [Website](https://fakeiteasy.github.io/)
* [Quickstart](https://fakeiteasy.readthedocs.io/en/stable/quickstart/)
* [Documentation](https://fakeiteasy.readthedocs.io/en/stable/)
* [Chat](https://gitter.im/FakeItEasy/FakeItEasy)
* [NuGet packages on NuGet.org](https://www.nuget.org/profiles/FakeItEasy "FakeItEasy's packages on NuGet.org"), targeting:
  * .NET Standard 2.0
  * .NET Standard 2.1
  * .NET 5.0
  * .NET Framework 4.5

---

FakeItEasy logo designed by [Vanja Pakaski](https://github.com/vanpak).
