# udemy_dotNET_design_patterns

Design Patterns in C# and .NET
UDemy course [Design Patterns in C# and .NET](https://www.udemy.com/course/design-patterns-csharp-dotnet).
this document will be where I write what i learned.

the code was compiled and tested on [dotnetFiddle](https://dotnetfiddle.net/) rather than messing around with local installation. use DotNet 5.0 to get string interpolation.

## The SOLID Design Principles
basics of OOP programming.

### Single Responsibility Principle

Any particular class / function / object should have only one reason to change.  
External functionalities aren't part of the core class and should go in a helper module. A red flag for violation of the principal is use of external resources like files, streams or sockets.

### Open Closed Principle

Class should be open for extension (derived classes), closed for changes (modifications). Changes to derived classes should not require changes in the base class. 

example of using ISpecification and Combinators.

### Liskov Substitution Principle

You can always use a derived class when a base class is expected. the classic example of squares and rectangles (which aren't fit for the Liskov substitution!).

### Interface Segregation Principle

Interfaces should require the minimal functionality, and nothing else. don't require more functionality in the interface then needed. "don't pay for what you don't need!", separate interfaces to the minimal requirements and combine them as higher level interfaces if needed (interfaces can inherit).  

*YAGNI*: you ain't going to need it.  
A red flag is functions that aren't supported (throw exceptions, do no-ops, always error).

### Dependency Inversion Principle
High level modules should not depend of low level modules. Use abstractions.  
Consume classes as interfaces, so they are decoupled from other classes which uses them. Don't depend on concrete classes in input / member variables. Prefer using interfaces (for both levels).

## The Gamma Classification

Based on gang of four (GOF) classification: Creational, Structural and Behavioral patterns. named after Eric Gamma (one of the authors).


## Creational Patterns

explicit creation with constructor, implicit creation (Dependency injection, reflection,etc). creation with a single statement (common case) or piecewise creation (initialization steps).

### Builder

Some objects are complicated to build. but a constructor with too many arguments isn't a reasonable behavior. A builder class is a separate class that is used to hold all the pieces together until finally calling the real class constructor.

Think about the C# StringBuilder which is used to build a string object. 
``` csharp
static void Main(string[] args)
{
    var sb = New StringBuilder();
    sb.append("str1");
    sb.append("str2");
    var str = sb.toString();
    };
}
```
we can imagine a specialized HTMLBuilder class that nows how to write elements inside a tag and construct the final string, and it can add children elements and create the final html text. this is similar to what react does with javaX.


**Fluent Interface**: an interface for chaining together commands by always returning the reference to the active object.

``` csharp
public class MyClass
{
    public MyClass Foo(int x)
    {
        //do something with x
        return this;
    }

    static void Main(string[] args)
    {
    var myclass = New MyClass();
    my.class.foo(1).foo(2);
    };
}
```

There is a problem in inheriting from fluent interface. if we methods from the base class, they return the reference as the base class, which has limited functionality. we can use the [Curiously Recurring Template Pattern](https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern) to always return the most derived class.

``` csharp
public class MyClass
{
    public MyClass Foo(int x)
    {
        //do something with x
        return this;
    }

}

public class MyClassDerived : MyClass
{
    public MyClassDerived bar(string x)
    {
        //do something with x
        return this;
    }
}

public class BaseCRTP<SELF>: BaseCRTP<SELF}>
{
    public SELF Foo(int x)
    {
        //do something with x
        return (SELF)this;
    }

}

public class DerivedCRTP<SELF> : BaseCRTP<DerivedCRTP<SELF>>
{
    public SELF bar(string x)
    {
        //do something with x
        return (SELF)this;
    }

}
static void Main(string[] args)
{
    var myclass = New MyClassDerived();
    // this will fail!
    //my.class.foo(1).bar("laa");
    // this will work
    var myCRTP = new DerivedCRTP<DerivedCRTP>();
    myCRTP.foo(1).bar("laa");
};
```
we can add a method *.build()* that actually constructs the final object in the end.  
#### Functional Builder
An example of using a functional builder: a builder object with a list of functions, extension methods and open/closed principal. then we can make this an abstract builder class that can work for any type of class

#### Faceted Builder

using a *facade* design pattern to hold the reference to class, and then using more than one builder on it. the containing object exposes different builders (with the same object as the reference) and allows the user to switch between different 'builder' mechanisms.


### Factories

regular and abstract factory.
> A component responsible solely for the wholesale (not piecewise) creation of objects.


#### Factory Method

normal constrictors must have the same (none descriptive) name, if you want to provide defaults this can turn into a mess ("optional parameters hell"). you can't have the same parameters for different functions because that's not possible in overloading functions (all the ctors have the same name!) and you can't give a derived class without explicitly calling for that derived class ctor.

##### Point Example 

we have a Point constructor that take x,y coordinates, and we want a constructor that can take polar points (rho, theta), but we already have a constructor with (double,double) arguments. so we can start adding parameters to determine how to use the doubles. but this is uncomfortable, and we lose the explicit naming of the parameters and we must have documentation explaining this.  
If we want to be explicit, we can have derived class (cartesian point,polar point) that have properly named constructors, but that feels like a misuse of inheritance. the functionality is still the same and still exists solely in the base class.  
C# resharper actually had a quick action to refactor a constructor into a factory method. This is a static class function that calls the (preferably private/protected) constructor.

##### Asynchronous Factory Method

We can't do a-synchronized stuff inside a constructor. We can have a separate init async function that is used severalty. but then we relay on the user to call it after each creation. to ensue this we can add factory method that calls the constructor and then the async initialization method before returning it to the user.



#### Factory Class

if we have big enough class, it might be better in terms of SRP (single responsibility principle) to separate the class creation methods (factory methods) from the class itself. so we can have a Factory class with a sole reputability of providing ways to create the object.  
we can fiddle around with the access modifiers by making it internal (so only the package classes can create it). Alternatively, we can make the factory a static inner class (and the constructor private) so that no external code can create the class, and the only way to create instances of this object is via the static factory class, which has access to other private methods of the containing class.

we can see this in action in C# [Task.Factory](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskfactory?view=net-5.0).

we can have factory properties: always create an object with some specified parameters. if this object can't be changed, then it's better to have it as a singleton/static class member (initialled just once)
``` csharp
public class Point
{
    private Point(x,y)
    {
        // constructor..
    }
    public static Point OriginProperty => new Point(0,0); // new point each call
    public static Point OriginMember = new Point(0,0); // created once
}
```

#### Abstract Factory (Interface Factory)

**name is misleading, abstract in this context means interface, not a 'base class that can't be instantiated on its' own'.**  
Give out ~~abstract~~ interface objects (rather than concrete objects). we can also have interface for the abstract factory itself, so that's an hierarchical factory design.  
the example in the video is a hot drinks machine which uses Activator.CreateInstance() to create classes with reflection.

the example has a bit of violating the OCP (open close principle) by using enums. it can be fixed with reflections again (on with dependency injection, as it should be used in production code), we take all classes that implement the interface from the assembly (avoiding the interface itself) and create them as our factories. to create an actual drink we have a method to expose the available options with a primitive type identifer (index number, string name) that we can accept from the user (don't forget to validate it!) and access the correct factory.

### Prototype

All about object copying. We don't design object from scratch. we make a copy and then change it. sometimes it's called a 'clone' of the object, we need deep copying.
> A partially or fully initialized object that you copy (clone) and make use of.

### Singleton

## Structural Patterns

The structure of the classes (class members), mimicking class interfaces like wrappers, stressing the importance of good api design.

### Adapter
### Bridge
### Composite
### Decorator
### Facade
### Flyweight
### Proxy

## Behavioral Patterns

No central theme, particular ways to solve common problems.

### Chain Of Responsibility
### Command
### Interpreter
### Mediator
### Memento
### Null Object
### Observer
### State
### Strategy
### Template Method
#### Visitor
