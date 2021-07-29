# udemy_dotNET_design_patterns

Design Patterns in C# and .NET
UDemy course [Design Patterns in C# and .NET](https://www.udemy.com/course/design-patterns-csharp-dotnet).
this document will be where I write what i learned.

the code was compiled and tested on [dotnetFiddle](https://dotnetfiddle.net/) rather than messing around with local installation. use DotNet 5.0 to get string interpolation.

## The SOLID Design Principles
<details>
<summary>
basics of OOP programming.
</summary>

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

</details>

## The Gamma Classification

Based on gang of four (GOF) classification: Creational, Structural and Behavioral patterns. named after Eric Gamma (one of the authors).


## Creational Patterns

<details>
<summary>
explicit creation with constructor, implicit creation (Dependency injection, reflection,etc). creation with a single statement (common case) or piecewise creation (initialization steps).
</summary>


### Builder
<details>
<summary>
When piecewise object construction is complicated, provide an API for doing it succinctly.
</summary>
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

</details>

### Factories

<details>
<summary>
A component responsible solely for the wholesale (not piecewise) creation of objects.
</summary>

#### Factory Method

Normal constructors must have the same (none descriptive) name, if you want to provide defaults this can turn into a mess ("optional parameters hell"). you can't have the same parameters for different functions because that's not possible in overloading functions (all the ctors have the same name!) and you can't give a derived class without explicitly calling for that derived class ctor.

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
</details>

### Prototype

<details>
<summary>
A partially or fully initialized object that you copy (clone) and make use of.
</summary>

All about object copying. We don't design object from scratch. we make a copy and then change it. sometimes it's called a 'clone' of the object, we need deep copying.  
we can either implement DeepCopy as a method/interface ourselves or use a serializer.

#### ICloneable is bad? what about Copy Constructors?

C# provides an interface ICloneable with the method Clone(), but it doesn't specify if it's a shallow copy or deep copy. and it always returns an object frm type Object (we need to explicitly cast it). Clone() sometimes specifies if it does a shallow copy.  
Another concept is taken from C++, the copy constructor. a constructor overload that takes an instance of the same class and calls the copy constructor on the members. but it's weird to do something c++ in c#.

#### IPrototype<T> interface

what about an interface that is both generic (DeepClone() returns T, no need to cast it) and explicitly does deep copying? it's possible, but still cumbersome, because it needs to be implemented in each member of the object.  
This approach doesn't scale well with large inheritance hierarchy. Each derived class must be able to pass parameters to the base class, and there's a lot of repetition going on. we can around it by requiring the class to have a empty default constructor, and implement a method to copy it's own properties into an object of the same class. we also have a default DeepCopy() method. the CopyTo() method copies it's own class properties and calls the base class CopyTo() method. there's an issue of casting to use the default implementation method. there is a problem that deepCopy() can not only copy a derived class, it can also copy a derived class into a base class.

``` csharp
public interface IDeepCopyable<T> where T: new()
{
    void CopyTo(T target);
    //default implementation?
    T DeepCopy()
    {
        T t = new T();
        CopyTo(t);
        return t;
    }
}
```


#### Copy Through Serialization

why bother with all the inheritance and interfaces when can simply use  extension methods on any type by serializing and deserializing. if we want to use the binary formatter, then all the classes and members must be using the [\[Serializable\] attribute](https://docs.microsoft.com/en-us/dotnet/api/system.serializableattribute?view=net-5.0). but we can choose other formatters, each serializer has different requirements. the xmlSerializer requires an empty parameterless constructor.

``` csharp
//extension method, takes this as argument, so can be called on anything?
public static T DeepCopy<T>(this T self)
{
    var stream = new MemoryStream();
    var formatter = new BinaryFormatter(); //requires the [Serializable] attribute
    formatter.serialize(stream, self); //write to stream
    stream.seek(0,SeekOrigin.Begin);  //start of stream;
    object copy = formatter.Deserialize(stream); //read from stream
    stream.close(); //maybe we could have used 'using'
    return (T) copy; //cast to T;
}

public static T DeepCopyXml<T>(this T self)
{
    using (var stream = new MemoryStream()) //will close the stream on it's own.
    {
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(stream,self);
        stream.Position = 0; //same as Seek, bring the stream back to the start;
        return (T)serializer.Deserialize(stream);
    }
}
```
</details>

### Singleton

<details>
<summary>
A component which is instantiated only once.
</summary>

The very hated pattern, even said that is often a design smell.

for some components, it doesn't makes sense to have more than one object of it's kind. important when construction is expensive, we don't want to allow more creations of it, and we want the entire system to use the same instance.
 
keep the constructor private and have a static instance, all the usual lazy or eager instantiation, we can use the system Lazy<> class if we want.

there is a problem: the singleton is a hard coded reference, so testing any component that uses it means testing on a 'live' component, and we can't write tests because the data might change, and we are using the live component (and one day, we will do something stupid to mess it up and the whole team will have to stop everything and fix it), so things are already in danger. we can mitigate this by using dependency injection. 

instead of implementing a singleton, we create a normal class, and use a dependency injection framework to treat it as such.

``` csharp
public void DependencyInjection()
{
    var cb = new ContainerBuilder(); //dependency 
    cb.RegisterType<OrdinaryDatabase>() //register the normal class or mock data
    .As<IDatabase> //the interface it implements
    .SingleInstance(); // require just one of them.
    cb.RegisterType<ConfigurableRecordFinder>(); //register a type that uses the interface.
    using (var c = cb.Build())
    {
        var rf = c.Resolve<ConfigurableRecordFinder>();
    }
}
```

why Singleton and not static? because we can't use dependency injection with static class. but there is something called [monostate pattern](https://stackoverflow.com/questions/624653/is-monostate-the-good-cousin-of-the-evil-singleton) which aims to have our cake and eat it. we can use 'new' to instantiate new objects, but all objects are referring to static fields. so maybe this means we can inherit from the class and still keep a single state.

thread safety: we can have a singleton for each thread by using a ThreadLocal<> wrapper and combine it with the other singleton implementations. we can also get the same results by using some container framework like we did with the dependency injection.

#### Ambient context pattern

some data that is changing, but also shared?
example in video. stack of contexts? scoping, disposing.   

</details>

</details>

## Structural Patterns

<details>
<summary>
The structure of the classes (class members), mimicking class interfaces like wrappers, stressing the importance of good api design.
</summary>

### Adapter

<details>
<summary>
A construct which adapts an existing interface X to conform to the required interface Y.
</summary>
imagine a electrical adapter for different power outlets.  
we take one class and force it to conform to some given interface, either by creating a new class or forwarding calls. can be as simple or as complicated as needed.

#### Caching

If our adapter uses a large amount of temporary data (creating objects), it might be more efficient to do some caching and retain the data internally. this of course assumes that we are going to reuse the same objects and that they are constant and not changing.

#### Generic Value Adapter

*not sure what's the point, actually*  
this would be trivial in c++. but in c# this requires quite a bit of work. there is a big example that. see file. basically, we need to propagate the type information in the entire hierarchy, we simply throw TSelf everywhere.

#### Adapter with dependency injection

an example with the container pattern and the command pattern. we use the *ContainerBuilder.RegisterAdapter()* method and the metadata feature.

</details>

### Bridge

<details>
<summary>
A mechanism that decouples an interface (hierarchy) from implementation (hierarchy).
</summary>
Avoiding a 'cartesian product complexity explosion' situation, if we have different features in a class hierarchy, each inheritance level can double the amount of classes. we rather use aggregation/composition than inheritance.  
if an interface has two options, we don't add the interface implementation to the class definition, we keep it as member so it doesn't require us to stack levels of inheritance classes.
should probably go along with dependency injection.

I think the difference between this a a decorator is that decorator is designed to hold itself in a nested level, while the bridge pattern is about horizontal levels. I think that this can be achieved with templates, but who am i to decide..?
</details>

### Composite

<details>
<summary>
A mechanism for treating individual (scalar) object and compositions of objects in a uniform matter.
</summary>

An example of a drawing application using a Composite to aggregate Graphic Objects together. the base class contains other object of itself. object can contain both it's own data and components, and the API doesn't care about it.  
An example of a neuron network containing a neuron class, a neuron layer and eventually a neuron rind. instead, we treat a single neuron as a collection of neuron as well. we define the neuron class an IEnumerable\<neuron\>, and **define the api around the collection of elements**, so we can treat a singular element the same as the aggregate.

going back to the open closed principle, where we created an 'AndSpecification' as a combinator of ISpecification, we can replace it with a 'CompositeSpecification' base class that can handle any number of CompositeSpecification (single, two, many) which can be combined in different ways.

based on the exercise, we should look at the IEnumerable interface.

</details>

### Decorator

<details>
<summary>
Facilitates the addition of behaviors to individual object without inheriting from them.
</summary>

sticking to the open code principle, extend functionality, keep the new changes separate (single responsibility principle), also work with sealed objects that can't be inherited.

it may or may not proxy calls to the decorated objects, it allows us to create runtime different decorators chains. we can use dynamic decorations (by passing around objects as references), or static decorations, which aren't as complex because of how the language treats generics (it's much more impressive in c++).

#### Decorating a Sealed class - CodeBuilder and StringBuilder

example of decorating the StringBuilder class. it's a sealed class (can't inherit from it) so if we need new functionality, we can't simply inherit and override. StringBuilder is actually a fluent class, so even if we delegate everything to the StringBuilder member, we need some manual changes.

#### Adapter - Decorator 

a class that uses both a StringBuilder member and adapts it to conform to regular string operations (constructor from string literal, concatenation with strings with the plus operator). this allows us quickly refactor any inefficient string objects into more efficient code (which is implemented via the StringBuilder) without changing any operations besides the creation of the object.

#### Pseudo Multiple Inheritance

C# and java don't support multiple inheritance. we use interfaces instead. but if we still need more than one base class (for member variables), we can use composition. the 'derived' class implements the interfaces, but delegates them to member variables.  
this brings back the diamond inheritance problem, if the two members (which are supposed to be base class) have a common property, we need to keep the values in sync. there isn't a 'clean' virtual inheritance like in c++.

#### Multiple Inheritance with Default interface members

modern c# allows us to have default implementations for interface methods;
``` csharp
public interface ICreature
{
    int Age {get; set;}
}

public interface IBird:ICreature
{
    void Fly()
    {
        if (Age >10)
        {
            Console.WriteLine("Flying!");
        }
    }
}
```
options of adding behaviors
1. Inheritance
2. Wrapper class
3. Extension methods.
4. C#8 default interface methods.

we can't actually call the default methods from the concrete class (if we the derived class didn't implement it), we must refer to our object as the interface via casting  or by using the "if (o **is** Obj obj)" syntax.

#### Dynamic Decorator Composition

this is probably the classic way to learn decorator design pattern,an object holds a reference to an object of the same interface, and delegates the operations after (or before) adding it's own special behavior.

there is a possible issue with Dynamic Decorator Composition, we can create a cycle that two decorators modify the same 'functionality', what does it mean that a shape has two 'color decorator'?. this can't be statically detected.  
this can be solved with a CyclePolicy:
``` csharp
public abstract class ShapeDecoratorCyclePolicy
{
    public abstract bool TypeAdditionAllowed(Type type,IList<Type> allTypes);
    public abstract bool ApplicationAllowed(Type type,IList<Type> allTypes);
}

public class ThrowOnCyclePolicy:ShapeDecoratorCyclePolicy
{
    private bool handler(Type type,IList<Type> allTypes)
    {
        if (allTypes.Contains(type))
        {
            throw new InvalidOperationsException($"cycle!");
        }
        return true;
    }
    public override bool TypeAdditionAllowed(Type type,IList<Type> allTypes)
    {
        return handler(type,allTypes);
    }
    public override bool ApplicationAllowed(Type type,IList<Type> allTypes)
    {
        return handler(type,allTypes);
    }
}
```
there is an common practice in c# of having both a generic and none generic classes with the same name. it has something to do with the *is* operator. see file. it's another **curious recursive template pattern** thing with inheriting from TSelf; the policy is a **strategy design pattern**.

#### Static Decorator Compositions

this is something that works in languages with compile time templates like c++.  in the example everything requires having a default constructor. we have a problem with the inner constructors. and how to access and expose the properties of the inner decorator. **this isn't a viable solution to C# production code**.

#### Decorator in dependency injection

using ContainerBuilder. we can register it as a named decorator, and supply it with a lambda to resolve the decorator properly.
</details>

### Facade

<details>
<summary>
Provides a simple, easy to understand user interface over a large and sophisticated body of code.
</summary>

exposing several components through a single interface.
DirectX and OpenGL are rendering images techniques that go faster than the regular console.
an example of pre-rendered console app, the custom console class is facade over a much more complicated code that is hidden from the user. the implementation itself depends on how simple/complicated we want to expose.  
we can additionally expose more internal operations for more experienced users to use.

</details>

### Flyweight

<details>
<summary>
A space optimization technique that lets use use less memory by storing externally the data associated with similar objects.
</summary>
</details>

the goal is to avoid redundancy when storing data. avoid duplication across objects. [String Interning](https://en.wikipedia.org/wiki/String_interning) is an example of it, string literals (which are immutable) that have the same text are stored just once. we can use references, keys, pointers, indices or some other way to save the precious memory.

example of storing strings in a static array and having each object keep indices of the strings it uses. it's probably a bit longer to construct with all the string actions but we use less memory.  
another example of using ranges (start end pairs) instead of of keeping a separate flag for each position.

we try to store any data externally and minimize how much of it we store. 

*turns out I've been using this all the time!*

### Proxy

</details>

## Behavioral Patterns

<details>
<summary>
No central theme, particular ways to solve common problems.
</summary>

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

<details>
<summary>
TODO: add Summary
</summary>
</details>
</details>
