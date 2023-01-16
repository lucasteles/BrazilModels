[![CI](https://github.com/lucasteles/BrazilModels/actions/workflows/ci.yml/badge.svg)](https://github.com/lucasteles/BrazilModels/actions/workflows/ci.yml)
[![Nuget](https://img.shields.io/nuget/v/BrazilModels.svg?style=flat)](https://www.nuget.org/packages/BrazilModels)
![](https://raw.githubusercontent.com/lucasteles/BrazilModels/badges/badge_linecoverage.svg)
![](https://raw.githubusercontent.com/lucasteles/BrazilModels/badges/badge_branchcoverage.svg)
![](https://raw.githubusercontent.com/lucasteles/BrazilModels/badges/test_report_badge.svg)
![](https://raw.githubusercontent.com/lucasteles/BrazilModels/badges/lines_badge.svg)

![](https://raw.githubusercontent.com/lucasteles/BrazilModels/badges/dotnet_version_badge.svg)
![](https://img.shields.io/badge/Lang-C%23-green)
![https://editorconfig.org/](https://img.shields.io/badge/style-EditorConfig-black)

# BrazilModels

This library contains Models, Formatters and Validator for common Brazilian documents

## Getting started

[NuGet package](https://www.nuget.org/packages/BrazilModels) available:

```ps
$ dotnet add package BrazilModels
```

## Validation and Formatting

### Cpf

```cs
using BrazilModels;

Cpf.Validate("00123456797");    // True
Cpf.Validate("99912345606");    // True
Cpf.Validate("999.123.456-06"); // True
Cpf.Validate("00000000000");    // False
Cpf.Validate("invalid");        // False

Cpf.Format("99912345606", withMask: true);    // "999.123.456-06"
Cpf.Format("319.818.120-83", withMask: true); // "319.818.120-83"
Cpf.Format("1234567890", withMask: true);     // "012.345.678-90"
Cpf.Format("12345601", withMask: true);       // "000.123.456-01"

Cpf.Format("319.818.120-83"); // "31981812083"
Cpf.Format("085.974.710-77"); // "08597471077"
Cpf.Format("08597471077");    // "08597471077"
Cpf.Format("12345601");       // "00012345601"
```

### Cnpj

```cs
using BrazilModels;

Cnpj.Validate("49.020.406/0001-25");// True
Cnpj.Validate("49020406000125");    // True
Cnpj.Validate("invalid");           // False

Cnpj.Format("49020406000125", withMask: true);     // "49.020.406/0001-25"
Cnpj.Format("49.020.406/0001-25", withMask: true); // "49.020.406/0001-25"
Cnpj.Format("1123456000101", withMask: true);      // "01.123.456/0001-01"

Cnpj.Format("49020406000125");     // "49020406000125"
Cnpj.Format("49.020.406/0001-25"); // "49020406000125"
Cnpj.Format("01.123.456/0001-01"); // "1123456000101"
```

### CpfCnpj (can be CPF or CNPJ)

```cs
using BrazilModels;

CpfCnpj.Validate("49.020.406/0001-25");// True
CpfCnpj.Validate("49020406000125");    // True
CpfCnpj.Validate("99912345606");       // True
CpfCnpj.Validate("999.123.456-06");    // True

CpfCnpj.Format("49020406000125", withMask: true); // "49.020.406/0001-25"
CpfCnpj.Format("99912345606", withMask: true);    // "999.123.456-06"

CpfCnpj.Format("085.974.710-77");         // "08597471077"
CpfCnpj.Format("49.020.406/0001-25");     // "49020406000125"
```

## Models

You can use some value types defined in this lib to strongly type your domain:

- `Cpf`
- `Cnpj`
- `CpfCnpj`
- `Email`

How to use it?

```cs
var cpf = new Cpf("319.818.120-83");
var cnpj = new Cnpj("49.020.406/0001-25");

class Person {
    public Guid Id {get;init;}
    public Cpf Cpf {get;init;}
    public Email Email {get;init;}
}

class Company {
    public Guid Id {get;init;}
    public Cnpj Cnpj {get;init;}
}

```

### Serialization

This already contain converters for:

- `System.Text.Json`
- `Swashbuckle.AspNetCore.Annotations`
- `System.ComponentModel.TypeConverter`

### Using with EntityFramework

You can easily define a `ValueConverter` for any of the types defined in this library as:

```cs

public class YourDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cliente = modelBuilder.Entity<Person>();
        cliente.Property(x => x.Cpf).HasConversion(t => t.Value, t => new(t));
        cliente.Property(x => x.Email).HasConversion(t => t.Value, t => new(t));
    }
}

```
