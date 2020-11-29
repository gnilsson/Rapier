# Rapier

### What is Rapier?

Rapier is a completely generic REST API put inside a library, it's purpose is to give you a headstart in API development with several key boilerplate features included.
It works by scanning the assembly for three key pieces per entity: EntityResponse, GetRequest and IModifyRequest. 
From there five endpoints are constructed for that entity with basic CRUD operations aswell as query management from GET:.

### How do I use it?

Startup configuration:
```C#
services.AddRapierControllers(opt =>
{
    opt.ContextType = typeof(RapierDbContext);
    opt.Add(typeof(Blog), "api/blogs");
});
```
..Database initalization etc..
```C#
services.AddRapier();
```

We need to mark our entities:
```C#
public class Blog : IEntity
{
  ...
}
```
Aswell as construct three pieces that our controller is going to work with:
```C#
public class BlogResponse : EntityResponse, IBlogResponseSimplified
{
  ...
}

public class GetBlogsRequest : GetRequest
{
  ...
}

public class ModifyBlogRequest : IModifyRequest
{
  ...
}
```
Rapier.Server is a demo project for further details.

### Where can I get it?
Soon! 
