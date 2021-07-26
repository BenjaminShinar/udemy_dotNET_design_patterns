# udemy_dotNET_design_patterns

Design Patterns in C# and .NET
Udemy course [Design Patterns in C# and .NET](https://www.udemy.com/course/design-patterns-csharp-dotnet).
this document will be where I write what i learned.

the code was compiled and tested on [dotnetFiddle](https://dotnetfiddle.net/) rather than messing around with local installation.

## The SOLID Design Principles
basics of OOP programming.

### Single Resposability Principle

Any particular class / function / object should have only one reason to change.  
External functionalities aren't part of the core class and should go in a helper module. A red flag for violation of the principal is use of external resources like files, streams or sockets.

### Open Closed Principle

Class should be open for extention (derived classes), closed for changes (modificiations). Changes to derived classes should not require changes in the base class. 

example of using ISpecification and Comibnators.

### Liskov Substition Principle

You can always use a derived class when a base class is expected. the classic example of squares and rectangles (which aren't fit for the liskov substition!).

### Interface Segregation Principle

Interfaces should require the minimal functionality, and nothing else. don't require more functionality in the interface then needed. "don't pay for what you don't need!", seperate interfaces to the minimal requirements and combine them as higher level interfaces if needed (interfaces can inherit).  

*YAGNI*: you ain't going to need it.  
A red flag is functions that aren't supported (throw exceptions, do no-ops, always error).

### Dependency Inversion Principle
High level modules should not depend of low level modules. Use abstractions.  
Consume classes as interfaces, so they are decoupled from other classes which uses them. Don't depend on concrete classes in input / member variables. Prefer using interfaces (for both levels).

## The Gamma Classification

Based on gang of four (GOF) classification: Creational, Structural and Behavioral patterns. named after Eric Gamma (one of the authors).


### Creational Patterns

explicit creation with construcor, implicit creation (Dependenct injection, reflection,etc). creation with a single statement (common case) or piecewise creation (initlazation steps).

#### Builder

Some objects are complicated to build. but a constrctor with too many arguemnts isn't a reasonble behavior. A builder class is a s seperate class that is used to hold all the pieces together until finally calling the real class constuctor.

Think about the C# StringBuilder which is used to build a string object. 
``` csharp
static void Main(string[] args)
{
    var sb = New StringBilder();
    sb.append("str1");
    sb.append("str2");
    var str = sb.toString();
    };
}
```
we can imagine a specialized HTMLBuilder class that nows how to write elements inside a tag and construct the final string, and it can add children elements and create the final html text. this is similar to what react does with javaX.


**Fluent Interface**: an interface for chainning together commands by always returning the refernce to the active object.

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

There is a problem in inheriting from fluent interface. if we methods from the base class, they return the refernce as the base class, which has limited functionality. we can use the [Curiously Recurring Template Pattern](https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern) to alwas return the most derived class.

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

public class DerivedCRTP<SELF> : BaseCRTI<DerivedCRTP<SELF>>
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
##### Functional Builder
An exapmle of using a functional builder: a builder object with a list of functions, extention mehtods and open/closed principal. then we can make this an abstract builder class that can work for any type of class

##### Faceted Builder

using a *facade* design pattern to hold the refernce to class, and then using more than one builder on it. the containning object exposes differnt builders (with the same object as the refernce) and allows the user to switch between differnt 'builder' mechanisms.


#### Factoreis

regular and abstract factory

#### Prototype
#### Singleton

### Structural Patterns

The structure of the classes (class members), mimicing class interfaces like wrappers, stressing the importance of good api design.

#### Adapter
#### Bridge
#### Composite
#### Decorator
#### Facade
#### Flyweight
#### Proxy

### Behaviroal Patterns

No central theme, particular ways to solve comon problems.

#### Chain Of Responsability
#### Command
#### Interpeter
#### Mediator
#### Memento
#### Null Object
#### Observer
#### State
#### Strategy
#### Template Method
#### Visitor
