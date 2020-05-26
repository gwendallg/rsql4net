
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

3. Samples

## Installation

```shell

dotnet add package RSql4Net

```

## Quick Start

1. Add a reference to the NuGet RSql4Net package in your project.

2. Modify Startup class in your project

```csharp

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ...
            services
                .AddRSql();
            ...
        }
```

3. Add a new operation in your mvc controller

```csharp

    // like Get operation
    [HttpGet]
    public IActionResult Get(IRSqlQuery<[your model]> query, IRSqlPageable<[your model]> pageable)
    {
        if (ModelState.IsValid)
        {
            var filter = query.Value();
            ...
        }
        // your code here
    }

```

4. Use for filter your data

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
