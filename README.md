
<p align="center">
<img src="docs/logo.svg" alt="logo" height="100">
</p>

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/gwendallg/rsql4net/blob/develop/LICENSE)

RSql4Net is AspNet Core extension that will make it easier for you to write your REST APIs. Its purpose is to convert a query in RSQL format to lambda expression.

## Continuous integration

| Branch                      |  Build | Quality Gate | Coverage |
|-----------------------------|--------|--------------|----------|
| master                      | ![](https://api.travis-ci.org/gwendallg/rsql4net.png?branch=master)| [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=gwendallg_rsql4net&branch=master&metric=alert_status)](https://sonarcloud.io/dashboard?id=gwendallg_rsql4net&branch=master) | [![Coverage Status](https://coveralls.io/repos/github/gwendallg/rsql4net/badge.svg?branch=master)](https://coveralls.io/github/gwendallg/rsql4net?branch=master) |
| develop                     | ![](https://api.travis-ci.org/gwendallg/rsql4net.png?branch=develop) | | [![Coverage Status](https://coveralls.io/repos/github/gwendallg/rsql4net/badge.svg?branch=develop)](https://coveralls.io/github/gwendallg/rsql4net?branch=develop) |

## Table of Contents

1. Installation

2. Quick start

3. Documentation

4. Samples

## Installation

```shell

dotnet add package RSql4Net

```

## Quick Start

1. Add the RSql4Ndt nuget packahe in your project.

2. Modify Startup.cs class in your project to add the RSql Extension

```csharp

    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services
            .AddControllers()
            .AddRSql();
        ...
    }

```

3. Add a new operation in your mvc controller

```csharp

    // like Get operation
    [HttpGet]
    public IActionResult Get([FromQuery] IRSqlQuery<[your model]> query,[FromQuery] IRSqlPageable<[your model]> pageable)
    {
        // the query parameters are correctly parsed ?
        if (ModelState.IsValid)
        {
            // your repository to filter
            IQueryable<[your model]> repository; 
            
            // the C# expression from query string parsing
            var filter = query.Value();

            // your filtered repository
            var filteredData = repository.Where(filter);

            // create Http result response
            
            ...
        }
        // your code here
    }

```

## Documentation

### Customize query string parameter names

To change query string parameter names, you would modify the RSql configuration.

```csharp

    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services
            .AddControllers()
            .AddRSql(
                options =>
                    options
                        // change the query string parameter name for RSql query field
                        .QueryField("q")
                        // change the query string parameter name for page size field
                        .PageSizeField("si")
                        // change the query string parameter name for page number field
                        .PageNumberField("of")
                        // change the default page size
                        .PageSize(50)
            );
        ...
    }
```

### Add Memory cache for RSql queries

To add a cache Memory for RSqk quries, you would modify the RSql configuration.

```csharp

    public void ConfigureServices(IServiceCollection services)
    {
        ...

        // create the memory cache
        var memoryCache = new MemoryCache(
            new MemoryCacheOptions()
            {
                SizeLimit = 1024
            }
        );

        services
            .AddRSql(
                options =>
                    // define the memory cache used for RSql queries
                    .QueryCache(memoryCache)
                    // invoked when register a new Rsql query in memory cache
                    .OnCreateCacheEntry((o) =>
                    {
                        o.Size = 1024;
                        o.SlidingExpiration = TimeSpan.FromSeconds(25);
                        o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    })
            );
        ...
```

1. Use for filter your data

Examples of RSql expressions in both FIQL-like:

* return all items where **name equal Kill Bill and year > 2003"**
    - **?query=name=="Kill Bill";year=gt=2003** *( or ?search=name=="Kill Bill";year>2003 )*

* return all items where **name start with "Kill" or address.suite is not null**
    - ?query=name=Kill*,address.suite=is-null=false

* return all items where **debit<=credit**
    - ?query=debit<=credit *( or ?query=debit=le=credit )*

* return all items where **address.suite equals 'L' or equals 'E'**
    - ?query==address.suite=in=(L,E)

| Binary operator||
|-|-|
|and| **;**|
|or| **,**|

| Comparison operator||
|-|-|
|equals|**==** or **=eq=**|
|not equals|**!=** or **=neq=**|
|lower than|**<** or **=lt=**|
|lower than or equals|**<=** or **=le=**|
|greater than|**>** or **=gt=**|
|greater than or equals|**>=** or **=ge=**|
|in|**=in=(,)**|
|not in| **=out=(,)**|
|is null|**=is-null='true\|false'**|


Examples of Pagination criteria :

* return second page and page size equals 50 items max

- with default naming strategy

**?PageNumber=2&PageSize=50**
    - with camel case strategy
    **?pageNumber=2&pageSize=50**
    - with snake case stategy
    **?page_number=2&page_size=50**

Examples of Sort criteria

* sort by "name" desc
    - **?sort=name;decs**
* sort by "id" and sort by desc year
    - **?sort=id,year;desc**

## And ...

* See RSql4Net.Samples project for samples
* See https://github.com/jirutka/rsql-parser
* See http://tools.ietf.org/html/draft-nottingham-atompub-fiql-00
