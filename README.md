# BrazilModels

This library contains Models, Formatters and Validator for common Brazilian documents

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

## Models

You can use the value types `Cpf` and `Cnpj` to strongly type your domain:

```cs
var cpf = new Cpf("319.818.120-83");
var cnpj = new Cnpj("49.020.406/0001-25");

class Person {
    public Guid Id {get;init;}
    public Cpf Cpf {get;init;}
}

class Company {
    public Guid Id {get;init;}
    public Cnpj Cnpj {get;init;}
}

```

They are:
  - System.Text.Json compatible
  - Swagger Annotations compatible

