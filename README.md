# SimpleDb.NET [![Build Status](https://travis-ci.org/rexm/SimpleDb.Net.svg?branch=master)](https://travis-ci.org/rexm/SimpleDb.Net)

A .NET library that lets you work with [AWS SimpleDB][1] using a familiar entity-like model and LINQ queries.

The goals of SimpleDb.NET are:
* Surface 100% of SimpleDB's capabilities through an easy, "**just works**" .NET API - no falling back to "low-level" REST for some operations.
* Provide robust **LINQ querying** and session-based **change tracking**.
* Good or better **performance** compared to rolling your own AWS REST calls from your app.
* **No dependencies** on third-party libraries (including the AWS SDK).
* Platform-independence (basically, runs anywhere [Mono][3] does :) )

## Installation

### Nuget

    nuget install SimpleDb.NET
    
### From this repo

[Download `Cucumber.SimpleDb.dll` from the root directory](https://github.com/rexm/SimpleDb.Net/raw/master/Cucumber.SimpleDb.dll). That always reflects the current state of the master.

## Usage

SimpleDb.NET doesn't require any configuration. To begin using it, just call `SimpleDbContext.Create`:

```C#
using(var simpleDb = SimpleDbContext.Create("publicKey", "privateKey"))
{
  var items = from item in simpleDb.Domains["myDomain"].Items
              where item["Status"] == "Hot" && item["LastUpdated"] > DateTime.Now.AddDays(-1)
              orderby item => item["LastUpdated"] descending
              select item;
    
  foreach(var item in items)
  {
    Console.WriteLine(item["MyOtherAttribute"]);
  }
}
```

If you've used LINQ to SQL (or pretty much any LINQ-friendly ORM) this should look fairly familiar. We've left it up to you to decide where to store your AWS public and private keys.

### Querying

The LINQ API will implicitly convert all the types supported by SimpleDB to the corresponding types in .NET - such as `string`, `int`, and `DateTime`. It also natively supports queries with SimpleDB's concept of multi-value attributes.

Although AWS SimpleDB is not a relational data store, SimpleDb.NET supports relation-y LINQ queries such as `JOIN` by batching multiple queries to AWS in parallel, and transforming the results on the local machine. *Note: this may change in v3*, because it allows the default behavior to cause very high resource consumption, if the developer does not carefully consider the implications of a given LINQ query. Please weigh in if you have an opinion.

### CRUD

All changes to items, as well as additions and deletions from a domain, are tracked and batch-sent over the wire once `SubmitChanges()` is called:

```C#
var newItem = myDomain.Items.Add("SomeItem");
newItem["SomeAttribute"] = "we really are schema-less!";
simpleDb.SubmitChanges();
```
    
Creating and deleting Domains are executed immediately.

You can find more details on the [wiki](https://github.com/rexm/SimpleDb.Net/wiki).

## Contributing

Check out the [contribution guidelines on the wiki](https://github.com/rexm/SimpleDb.Net/wiki/About-the-Project#how-to-contribute).

## Acknowledgements

The LINQ provider stands on [Matt Warren's shoulders][2]. Every LINQ provider that exists today (and LINQ itself) is thanks to him and the brilliant guys and girls at Microsoft.

And of course, Amazon for creating and operating such a powerful cloud database service.


[1]: http://aws.amazon.com/simpledb
[2]: http://blogs.msdn.com/b/mattwar/archive/2008/11/18/linq-links.aspx
[3]: http://www.mono-project.com/
