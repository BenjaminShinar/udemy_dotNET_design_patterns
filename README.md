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

the goal is to avoid redundancy when storing data. avoid duplication across objects. [String Interning](https://en.wikipedia.org/wiki/String_interning) is an example of it, string literals (which are immutable) that have the same text are stored just once. we can use references, keys, pointers, indices or some other way to save the precious memory.

example of storing strings in a static array and having each object keep indices of the strings it uses. it's probably a bit longer to construct with all the string actions but we use less memory.  
another example of using ranges (start end pairs) instead of of keeping a separate flag for each position.

we try to store any data externally and minimize how much of it we store. 

*turns out I've been using this all the time!*
</details>

### Proxy

<details>
<summary>
A class that functions as an interface to a particular resource. the resource may be remote, expensive to construct, or may require logging or some some other added functionality.
</summary>

avoid changing code by using the same interface with different behavior, allow for remote calls (calls to a different process),logged calls (write to log) or guarded calls (check for validity of arguments) without changing the interface.

#### Protection Proxy

a class that controls access to a different class and performs additional checks on it. can be used to authentication resources. we keep the same core functionality the same, but we control if it's possible to call on it or not.

#### Property Proxy

rather than use a value as property, we can use a class as that property, and this class controls the value. we can avoid setting the value if the existing value is the same.

``` csharp
public class Property<T>
{
    private T value;
    public T Value
    {
        get=> value;
        set {
            if (Equals(this.value, value))
            {
                //do nothing
            }
            else
            {
                this.value = value;
            }
        }
    }

    public static implicit operator T(Property<T> property)
    {
        return property.value;
    }
    public static implicit operator Property<t>(T value)
    {
        return new Property<T>(value);
    }
    //equality operators... 
}
```
there is an issue with the = operator. C# doesn't allow us to overload the assignment operator (unlike C++), so it would create a new property rather than mutate the value. so we actually keep the property<T> class private and provide property T that has a custom setter to change the private property.

``` csharp
public class Creature{
    private Property<int> agility= new Property<int>();
    public int Agility {
    get{return agility.Value;}
    set{agility.Value= value;}
    }
}
```

*in Landa we had ParameterValue\<T\> and ParameterConstraint\<T\> classes*

#### Value Proxy

a wrapper on a primitive type that provides some extra juice and consolidates synthetic sugar into the class.
example for creating a 'Percentage' class.
use extension method to allow construction of class from value

``` csharp
    public struct Percentage
    {
        private readonly float value;
        internal Percentage(float value)
        {
            this.value=value;
        }
        public static float operator *(float f, Percentage p)
        {
            return f*p.value;
        }
        public static Percentage operator +(Percentage p1, Percentage p2)
        {
            return new Percentage(p1.value+ p2.value);
        }
    }
    
    public static Percentage Percent(this int value)
    {
        return new Percentage (value/100.f);
    }
    
```

#### Composite Proxy: SoA/AoS

*structure of arrays* or *array of structures*?

different storage for a class that behave the same. instead of having an array of N objects with 3 properties, we can have 3 N sized Arrays. the external object simply stores the reference and all it's actions are performed with the index on the arrays. this way we can perform actions on a single proxy element and all the similar data is located near each other.

#### Composite Proxy with Array backed Properties

grouping properties together with a composite proxy.
we can have a property All that does all the 'set' commands together. the get command should return a nullable object (bool?), if they are all the same, we return the value, otherwise, we return a null.

but rather than use names properties, we can use array backed properties, and now we can use array methods, so it's easier to extend this.

#### Dynamic Proxy for logging

we can have a dynamic proxy, example with using dynamicObject and reflection. we override the 'TryInvokeMember' method. we use a generic factory method and the .ActLike() method from some *ImpromptuInterface* library and .As\<Interface\> (we need to override somethings)

#### Proxy vs Decorator

* Proxy provides an identical interface, Decorator provides an enchanted interface,
* Decorator typically aggregates (or has reference to) what it's decorating, a proxy doesn't necessarily.
* a proxy might not even be working with a real materialized object. like Lazy object.

#### View Model

we can use a proxy to add validation to a class and keep the UI concerns separate from the class itself. 
MVVM (model, view, view model)
Model - the data itself
View - the UI stuff
View Model - usually a proxy over the Model, provides the 'onPropertyChanged' stuff.
we can use this together with a decorator to get some additional functionality.

#### Bit Fragging

Boolean isn't a really stored as a bit. that's a problem if you have too many bool properties it might be a memory issue. we have BitArray ov VectorBit\<32\> in the standard library. but if we want a different fragmentation (like, 0,1,2,3) we can store them in bits and use a proxy over them. this is similar to the bit fields in classic C.

example of using a set of operators to find if an array of numbers and can reach an value.
> 1,3,5,7 -> 0.  
which set of 4 operators allows us to reach zero?
1 - 3 - 5 - 7 =0.
can we find for each number if there is an operator set that reaches this value?

an example of using an index property and enums to do a formula evaluation.

</details>

</details>

## Behavioral Patterns

<details>
<summary>
No central theme, particular ways to solve common problems.
</summary>

### Chain Of Responsibility

<details>
<summary>
A chain of components who all get a chance to process a command or a query, optionally having default processing implementation and an ability to terminate the processing chain.
</summary>

which component handles events? where does the handling stop? in which order does this happen? how are effects compounded across elements.

**CQS** Command Query Separation :  
Query: get information  
Command: ask for action or change  
therefore, QCS means having different means of sending commands and queries.
in the chain of command pattern we can have listeners that listen on the commands and modify them.

something like a linked list of Modifying elements (is this not the decorator pattern?) that request handling from one another.

an example using a cardGame with modifiers on the creature with method Chain. including the interception of calls in a modifier class.

the problem in that example is that the inner object is mutated, and also exposed outside, so we can't easily remove a layer from it. the better implementation uses a Mediator pattern and events. this called a *Broker Chain*.  we have a query object that acts as the 'getter' method. we use an abstract base class. the modifier class listens to events and if it's relevant right now than we modify the result of the query. we can also make them *IDisposable* so that they remove themselves from the chain when they're done.


note: I did the exercise and I thought it was a mess, my code wasn't accepted at all, so i included the solution code instead with my comments. I think it's a stupid implementation. 

</details>

### Command

<details>
<summary>
An object which represents an instruction to perform a particular action. contains all the information necessary for the action to be taken.
</summary>

There is no built-in 'undo' action in C#, there is no easy way to serialize a sequence of actions. are 'queries' commands? that depends. they are an object that performs a thing, but we still have the Query-Command Separation concept.

we can string several commands together to perform complex actions, or write a single command that contains the whole logic of an operation. the commands encapsulate the 'main' of the program, and can be used from a UI, stored in a database, and even have an 'undo' operation (be sure to only allow rollbacks if operation was successful, and only do this once!). commands can also hold state. this allows us to create a transactional model.

we need a command, a command processor, and then we can add an undo action or composite commands (called macros).

#### Composite Commands

a combination of the composite pattern and the command pattern. a single command that is a composite of several commands together.

An example of commands in a bank account program. need to handle a situation where some commands were successful and some were not.


</details>

### Interpreter

<details>
<summary>
A Component that process **structured** text data. Does so by turning it into separate lexical tokens (*lexing*) and then interpreting sequences of said tokens (*parsing*).
</summary>

processing textual input into executables actions. parsing text into something structured. this is what fuels regex, IDEs, python code, HTML, XML, code suggestions and suggestions, and of course: Compilers from source code to Binary code.

an example of parsing a text string containing simple mathematical expressions, numbers, parentheses, plus and minus operators.  
a token is any single element, in our case: numbers, operations, brackets.

first stage it to lex - takes an string input and turns it into a collection of tokens.
the next stage is parsing. how are do the tokens interact with one another? how do they relate to each other, and which order?

``` csharp
// warning! don't rely on this code! it didn't work well for me in the exercise!
public class Token
{
    public Enum Type
    {
        Integer, Plus, Minus, LeftParentheses, RightParentheses;
    }

    public Type tokenType;
    public string Text;

    public override string ToString()
    {
        return $"`{Text}`";
    }
    public static List<Token> Lex(string input)
    {
        var results = new List<Token>();
        for (int i =0; i < input.Length ;++i)
        {
            switch(input[i])
            {
                case '+':
                results.Add(new Token(Token.Type.Plus),"+"));
                break;
                case '-':
                results.Add(new Token(Token.Type.Minus,"-"));
                break;
                case '(':
                results.Add(new Token(Token.Type.LeftParentheses,"("));
                break;
                case ')':
                results.Add(new Token(Token.Type.RightParentheses,")"));
                break;
                default:
                var sb = new StringBuilder(input[i].ToString())
                {
                    for (int j=i+1; j <input.Length;++j)
                    {
                        if (char.IsDigit(input[j]))
                        {
                            sb.Append(input[j]);
                            ++i; //this is for the next big loop!
                        }
                        else
                        {
                            results.Add(new Token(Token.Type.Integer,sb.ToString()));
                            break; //break the char processing loop!
                        }
                    }
                }
                break; //break the switch statement.
            }
        }
        return results;
    }
}

```

ANTLR - Another Tool For Language Recognition. a parser to generate structured data from input,
</details>

### Iterator

<details>
<summary>
An object (or, in .NET, a method), that facilitates the traversal of a data structure.
</summary>

Traversal of data functions. Iterator is the class that facilitates the traversal by keeping a reference to the current element and knows how to move to the next element. C# has the implicit iterator construct with the IEnumerable interfaces and the *yield return* statements.

An example of binary tree, doing in-order-traversal:

Traversals of tree 
``` json
{
"root":{
    "value":1,
    "left": {"value":2},
    "right": {"value":3}
    }
};
```
>   
      1  
     / \ 
    2   3

* In-Order: left, this, right: 2,1,3
* Pre-Order: this, left, right: 1,2,3
* Post-Order: right,this,left: 3,2,1


the basic form of an iterator has the current Data Node, a moveNext method that returns true and moves the iterator forward. this is the tr
traditional form, like how it's implemented in c or c++.

``` csharp
public class Node<T>
{
    T data;
    Node<T> Parent,Left,Right; //bi directional Node
}
public class InOrderIterator<T>
{
    private readonly Node<T> root;
    public Node<T> Current;
    private bool yieldedStart = false; //used just once;
    public InOrderIterator<T>(Node<T> root)
    {
        this.root = root;
        Current = root;
        while (Current.left != null)
        {
            Current=Current.Left; //go all the way left!
        }
    }
    public bool MoveNext()
    {
        if (!yieldedStart)
        {
            yieldedStart= true; //happens once!
            return true;
        }

        if (Current.Right != null)
        {
            Current = Current.Right; // one to the right
            while (Current.left != null)
            {
                Current=Current.Left; //go all the way left!
            }
            return true;
        }
        else
        {
            var p = Current.Parent;
            while (p!= null && Current == p.Right) // we have a parent, and we are coming from the right side of it.
            {
                Current =p;
                p = p.parent;
            }
            Current = p;
            return Current != null;

        }
    }

    public void Reset(){
        //additional operations
        //Current = root;
        //yieldedStart= false;
    }
}

```

but we can do better in C#, we have the yield return keyword for this. this creates a state machine for us. this is recursive, readable and simple

``` csharp
public class BinaryTree<T>
{
    private Node<T> root;

    public IEnumerable<Node<T>> InOrder
    {
        get
        {
            //local method?
            IEnumerable<Node<T>> TraverseInOrder(Node<T> current)
            {
                // left elements
                if (current.Left != null)
                {
                    foreach (var left in TraverseInOrder(current.Left))
                    {
                        yield return left;
                    }
                }
                // this element
                yield return current;
                // right elements
                if (current.Right != null)
                {
                    foreach (var right in TraverseInOrder(current.Right))
                    {
                        yield return right;
                    }
                }
            }

            foreach (var node in TraverseInOrder(root))
            {
                yield return node;
            }
        }
    }
}
```


#### Duck Typing

an IEnumerable type is a class that has a *GetEnumerator* method which returns an *Iterator* with *MoveNext* method that returns a bool and a *Current* property. we don't need to declare the class as IEnumerable. as long as we have GetEnumerator method, we can do a *foreach* loop.  
most of the actions in c# require a formal Interface, but duck typing doesn't.


#### Array-backed Properties (revisited)

if we stick our elements in an array, we can use linq to take advantage of the array methods rather than access them directly.  
*not sure about this*

</details>

### Mediator

<details>
<summary>
A component that facilitates communication between components without them necessarily being aware of each other or having direct (reference) access to each other.
</summary>

allowing components to be unaware of one another. we don't want direct reference to one another, but rather a single component that controls the flows.

example of a chatroom, or any service with multiple clients (online games). we use something called *event broker* (or *event bus*). the event broker is an *IObservable* object with publisher/subscriber with the *.OfType\<EventType\>().Subscribe(()=>{});* syntax. also uses dependency injection.  
we have base actor class, base event class and a broker class. the actors publish an event to the broker, and are subscribed to events on it. no actor is directly aware of the other actors. they only operate through the events.

library [MediatR](https://github.com/jbogard/MediatR) example demo. uses interfaces *Command:IRequest\<Response\>, IRequestHandler\<Command,Response\>* that uses async await syntax. using dependency injection again with \<ServiceFactory\> to register the command handler. there are some requirement to use async, so this needs to handled.
</details>

### Memento

<details>
<summary>
A token/handle representing the system state. Lets us roll back to the state when the token was generated. May or may not directly expose state information.
</summary>
the goal is to allow an object to return to a previous state with a token. save a snapshot of the objects and allow the system to restore the object to that state. typically an immutable object so it can't be changed from outside.

our object is separated from it's state, and can restore the state from a token at anytime.
Undo and Redo: keep all the tokens in the class, including the initial state.

Memento for [interop](https://en.wikipedia.org/wiki/COM_Interop),[Interoperability](https://en.wikipedia.org/wiki/Interoperability): P/Invoke (parallel invocation). *this feels more like the flyweight pattern*.
</details>

### Null Object

<details>
<summary>
A no-op object that conforms to the required interface, satisfying a dependency requirement of some other object.
</summary>
*not part of the gang of 4 pattern. sometimes classified as a structural pattern.*
we assume the objects are generally not null. but what happens if we don't have the object and someone depends on it? we need something to call on at all cases.

this feels like a mock object that simply does nothing. works well with open closed principle. if the object has a dependency on an interface, we have to give it something, and passing a null value is dangerous (unless we were explicitly told it's safe). passing null might cause problems in dependency injection.  
in this pattern, we implement a class that follows all the requirement of the interface but does nothing. if we have properties it's a bit more complicated. null objects aren't always the right solution, sometimes the object should return a result and we simply can't rely on a defaulted result, but that depends on the software requirements.  
we can make this null object a sealed class (to avoid having it inherited) or a singleton (there's no point to have more than one). in modern C# we can have a private sealed class inside the interface and static properties. this way we can implement the default class inside the interface, and expose it solely through the interface. however, this is weird because developers don't expect interfaces to have properties, which makes the code unnatural.

dynamic null object - using DLR (dynamic language runtime). of course, this has worse performance than static classes. example using the ImpromptuInterface library and the INull class (see [ImpromptuInterface] below(#Dynamic-Run-Time-ImpromptuInterface)).
</details>

### Observer

<details>
<summary>
An observer is an object that wishes to be informed about events happening in the system. The entity generating the events is an observable.
</summary>

We want to be informed(notified) when certain things happen, we  want to know when *events* happen. it's partially built in to the language with the *event* keyword, and there are also *IObservable\<T\> ,IObserver\<T\>* interfaces in the system library. there are also *INotifyPropertyChanging, INotifyPropertyChanged* and *BindingList\<T\>, ObservableCollection\<T\>*.

#### The Event Keyword

the event keyword specifies a special operation that other can subscribe onto and.
the signature of the handling method has the invoking object (the observable) and the *EventArgs* object, or a derived object of it, which can contain additional information.

we use the *+= and -=* operators to subscribe to events, there are some weird behavior for multiple subscriptions on the same event by the same method.

``` csharp
public class Person
{
    public event EventHandler<EventArgs> FallsIll; //event
    public void CatchCall()
    {
        FallsIll?.Invoke(this,EventArgs.Empty); //might fail on null exception if no one subscribed

    }
    public static void Main(string[] args)
    {
        var person = new Person();
        person.FallsIll += CallDoctor; //subscribe to the event
    }
    public static void CallDoctor(object sender, EventArgs eventArgs)
    {

    }
}
```

#### Weak Event Pattern

even though C# is managed, it can still have memory leaks when using events. so C# has a design pattern to avoid it.
if we subscribe to something, our observer can't be collected while the observable exists.
we can use *WeakReference*  to check if the item system exists.


``` csharp
public class Button
{
    public event EventHandler Click; //event
    public void Fire()
    {
        Click?.Invoke(this,EventArgs.Empty); //

    }

    public static void CallDoctor(object sender, EventArgs eventArgs)
    {

    }
}
public class Window
{
    public Window(Button button)
    {
        button.Click += Action; // something
    }
    private void Action(object sender, EventArgs eventArgs)
    {

    }
    ~Window()
    {

        //this is the destructor/ finalize method in c#, we rarely see it!
    }
}
public class Demo
{
    public static void Main(string[] args)
    {
        var btn = new Button();
        var win = new Window(win);
        
        //setting window to null;
        window =null;
        
        // run garbage collector
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
```

to handle this issue we can either use IDisposable to unsubscribe from events. but there is also a *WeakEventManager\<obj,EvenArgs\>* which controls this itself. this means we no longer use the *+= or -=*  syntax.

``` csharp
public class Button2
{
    public event EventHandler Click; //event
    public void Fire()
    {
        Click?.Invoke(this,EventArgs.Empty); //

    }

    public static void CallDoctor(object sender, EventArgs eventArgs)
    {

    }
}
public class Window2
{
    public Window2(Button2 button)
    {
        WeakEventManager<Button2,EventArgs>
        .AddHandler(button,"Clicked",Action);
    }
    private void Action(object sender, EventArgs eventArgs)
    {

    }
    ~Window2()
    {

        //this is the destructor/ finalize method in c#, we rarely see it!
    }
}
public class Demo
{
    public static void Main(string[] args)
    {
        var btn = new Button2();
        var win = new Window2(win);
        
        //setting window to null;
        window =null;
        
        // run garbage collector
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
```

#### Observer via Special Interfaces

RX:Reactive extensions.  
ordinary events have event leaks, once we subscribe to something, the subscription is invisible. in RX there is pattern of IDisposable. The reactive extensions allow us some simplified stuff for easier use, but the explicit form is detailed here:

``` csharp
public class Event
{

}

public class FallsIllEvent: Event
{
    public string Address {get;set;}
}
public class Person : IObservable<Event> //
{
    private readonly HashSet<Subscription> Subscriptions = new HashSet<Subscription>();
    public void CatchCold()
    {
        foreach (var s in Subscriptions)
        {
            s.OnNext( new FallsIllEvent{Address = "street"}); //fire events;
        }

    }
    public IDisposable Subscribe(IObserver<Event> observer)
    {
        var sub = new Subscription(this,observer);
        Subscriptions.Add(sub);
        return sub;
    }

// a private class
    private class Subscription : IDisposable
    {
        private readonly Person Person;
        public readonly IObserver<event> Observer;
        public Subscription(Person person,IObserver<Event> observer)
        {
            Person=person;
            Observer = observer;
        }
        public void Dispose()
        {
            Person.Subscriptions.Remove(this);
        }
    }
}
public class Program:IObserver<Event>
{

    public Program()
    {
        var person = new Person();
        var sub = Person.Subscribe(this);
        
        person.CatchCold();
    }
    public static void Main(string[] args)
    {
        var program = new Program();
        
    }
    public void OnCompleted()
    {
        // no more events to be generated

    }
    public void OnError(exception error)
    {
        
    }
    public void OnNext(Event value)
    {
        //this is the serious one.
        if (value is FallsIllEvent args)        
        {
            //do something with args
        }
    }
}

//the RX syntax 
public class RXProgram:IObserver<Event>
{   

    public RXProgram()
    {
        var p =new Person();        
        //simplified form
        p.OfType<FallsIllEvent>()
            .Subscribe(args => {/*do something*/});
        p.CatchCold();
    }

}
```

#### Observable Collections

still in the world of reactive components.
classes that implement *INotifyPropertyChanged* have *PropertyChanged* event and *OnPropertyChanged* Invoker. we can be notified on one property change. here are all sorts of ways to do this. there are BindingLists and observable collections and stuff like that. this is really important fo UI elements.


``` csharp
public class Market: INotifyPropertyChanged
{
    private float volatility;
    public float Volatility
    {
        get{return volatility;}
        set{
            if(volatility.Equals(value)) return;
            volatility= value;
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvoker]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
     PropertyChanged?.Invoke(This, new PropertyChangedEventArgs(propertyName));   
    }
}
public class MarketList 
{
    private list<float> prices = new list<float>();
    
    public void AddPrice(float price)
    {
        prices.Add(price);
        PriceAdded?.Invoke(this,price);
    }
    public event EventHandler<float> PriceAdded;
}

public class BindingMarketList
{
    public BindingList<float> Prices = new BindingList<float>();
}

public class Program
{
    public static void Main(string[] args)
    {
        var m = new Market();
        market.PropertyChanged += (sender, eventArgs) =>
        {
            if (eventArgs.PropertyName == "Volatility")
            {
                //do something;
            }
        };
        m.Volatility = 100.0f;

        var ml = new MarketList();
        ml.PriceAdded += (sender,price) =
        {
            // do something;
        };
        ml.AddPrice(50.0f);

        var mlb = new BindingMarketList();
        mlb.Prices.ListChanged += (sender,eventArgs) =>
        {
            if (eventArgs.ListChangedType == listChangedType.ItemAdded)
            {
                float price = ((BindingList<float>)sender)[eventArgs.newIndex];
            }
        };

        mlb.Prices.Add(25.0f);
    }
}

```

#### Bidirectional Observers

a bit more control than bindingList; trying to synchronize data between objects. we need the case guards to prevent infinite loops. in windows forms we have this built in as 'binding'. in our example we use Expression\<Func\<object\>\>.

``` csharp
public class Program
{
    public class Product : INotifyProductChanged
    {
        private string name;
        public string Name 
        {
            get=>name;
            set
            {
                if (value== name)return;
                name= value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvoker]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(This, new PropertyChangedEventArgs (propertyName));   
        }
    }

    public class Window
    {
        private string productName;
        public string ProductName 
        {
            get=>productName;
            set
            {
                if (value== productName)return;
                productName= value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvoker]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(This, new PropertyChangedEventArgs (propertyName));   
        }
    }

        public sealed class BidirectionalBinding:IDisposable
    {
        private bool disposed;
        public BidirectionalBinding(INotifyPropertyChanged first, Expression<Func<object>> firstProperty,
        INotifyPropertyChanged second, Expression<Func<object>> secondProperty)
        {
            //properties must be memberExpressions, and must be property Info
            id (firstProperty.Body is MemberExpression firstExpression && secondProperty.Body is MemberExpression secondExpression)
            {
                if(firstExpression.Member is PropertyInfo firstProp && secondExpression.Member is PropertyInfo secondProp) 
                {
                    //subscribing
                    first.PropertyChanged +=(sender,arg)=>
                    {
                        if(!disposed)
                        {
                            //reflection
                            secondProp.SetValue(second,firstProp.GetValue(first));
                        }
                    }

                    second.PropertyChanged +=(sender,arg)=>
                    {
                        if(!disposed)
                        {
                            firstProp.SetValue(first,secondProp.GetValue(second));
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            dispose = true;
        }
    }

    public static void Main(string[] args)
    {
        var product = new Product{Name = "Book"};
        var window = new Window{ProductName= "Book"};
        //subscribe to both;
        product.PropertyChanged += (sender,eventArgs) =>
        {
            if (eventArgs.PropertyName == "Name")
            {
                window.ProductName=product.Name;
            }
        };

        window.PropertyChanged += (sender,eventArgs) =>
        {
            if (eventArgs.PropertyName == "ProductName")
            {
                product.Name=window.ProductName;
            }
        };

        var product2 = new Product{Name="BetterBook"};
        var window2 = new Window{ProductName="BetterBook"};
        using var binding = new BidirectionalBinding(
            product2, () => product2.Name,
            window2,() => Window2.ProductName);
    }
}

```



#### Property Dependencies

there is a problem of dependencies between properties. it's a real mess; we can call OnPropertyChange inside setters, but this doesn't scale well. we can try and make this work by employing a visitor.

*I Might need to revisit this*

``` csharp
public class Program
{
    public static void Main(string[] args)
    {
    }
    
    public class Person : INotifyProductChanged
    {
        public bool CanVote => Age >= 16;
        private int age;
        public int Age 
        {
            get=>age;
            set
            {
                if (value== age)return;
                age= value;
                OnPropertyChanged();
                //very simple, but not scalable
                OnPropertyChanged(nameof(CanVote));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvoker]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(This, new PropertyChangedEventArgs (propertyName));   
        }
    }

    //person can inherit from this?
    public class PropertyNotificationSupport : INotifyProductChanged
    {
        private readonly Dictionary<string, HashSet<string>> affectedBy = new Dictionary<string, HashSet<string>>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvoker]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(This, new PropertyChangedEventArgs (propertyName));   
            //this is recursive call to all dependencies;
            foreach (var affected in affectedBy.Keys)
            {
                if (affectedBy[affected].Contains(propertyName))
                {
                    OnPropertyChanged(affectedBy); //
                }
            }
        }
    }

    public class BetterPerson : PropertyNotificationSupport
    {
        private readonly Func<bool> canVote;
        public bool CanVote => canVote();
        private int age;
        public int Age 
        {
            get=>age;
            set
            {
                if (value== age)return;
                age= value;
                //this will call the recursive
                OnPropertyChanged();
            }
        }
        public BetterPerson()
        {
            //this is an expression that can be traversed
            canVote = property(nameof(CanVote),()=> Age >=16);

        }
    }
}
```

#### Declarative Event Subscription with Interfaces

so far we only did manual subscription, but here is a convention of doing this automatically. this can be done by attributes or by reflection (as done below).  
we will use autofac again for dependency injection. this is a huge mess of work and contains many shortcuts and hacks. we use singletons to avoid a problem of adding new senders (what about the hold handlers?), we also don't have any way to unsubscribe.

``` csharp
using autofac;
public class Program
{
    //base interface
    public interface IEvent
    {
    }

    //base Sender interface
    public interface ISend<TEvent> where TEvent: IEvent
    {
        event EventHandler<TEvent> Sender;
    }
    //base Handler interface
    public interface IHandle<TEvent> where TEvent: IEvent
    {
        void Handle(object sender, TEvent args);
    }
    //concrete event class
    public class ButtonPressedEvent : IEvent
    {
        public int NumberOfClicks;
    }
    //concrete Sender Class
    public class Button: ISend<ButtonPressedEvent>
    {
        public event EventHandler<TEvent> Sender;
        public void Fire(int clicks)
        {
            Sender?.Invoke(this, new ButtonPressedEvent
            {
                NumberOfClicks = clicks
            });
        }
    }
    //concrete handler class
    public class Logging : IHandle<ButtonPressedEvent>
    {
        public void Handle(object sender, ButtonPressedEvent args)
        {
            //do something
        }
    }
    public static void Main(string[] args)
    {
        var cb = new ContainerBuilder(); //autofac builder;
        var assembly = Assembly.GetExecutingAssembly();
        //register senders
        cb.RegisterAssemblyTypes(assembly)
            .AsClosedTypeOf(typeof(ISend<>))
            .SingleInstance(); // this is for ease of use;
        //register handlers
        cb.RegisterAssemblyTypes(assembly)
            .Where(t=> t.GetInterfaces() //interfaces of type t
                .Any(i=> 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandle<>)
            ))
            .OnActivated(act => //after creation
            {
                //if instance is IHandle<foo>, we want to subscribe to any ISender<foo>
                var instanceType = act.Instance.GetType();
                var interfaces = instanceType.GetInterfaces();
                foreach (var i in interfaces)
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandle<>) )
                    {
                        var arg0 = i.GetGenericArguments()[0]; // this is the foo type;
                        var senderType = typeof(ISend<>).MakeGenericType(arg0);
                        //now we want every instance of an object that is ISend<foo>
                        //luckily, we can get it. somehow
                        var allSenderTypes = typeof(IEnumerable<>)
                            .MakeGenericType(senderType);
                        var allServices = act.Context.Resolve(allSenderTypes);

                        foreach (var service in (IEnumerable)allServices)
                        {
                            var eventInfo = service.GetType().GetEvent("Sender");
                            var handleMethod = instanceType.GetMethod("Handle");
                            var handler = Delegate.CreateDelegate(
                                eventInfo.EventHandlerType,null, handleMethod);
                            eventInfo.AddEventHandler(service, handler);
                        }
                    }
                }
            })
            .SingleInstance()
            .AsSelf();


            var container = cb.Build();
            var button = container.Resolve<Button>();
            var logging = container.Resolve<Logging>();
            button.Fire(1);
            button.Fire(2);
            
     }
}    
```

a different approach is to have an event broker, all the work is done by the broker.
</details>

### State

<details>
<summary>
A pattern in which the object's behavior is determined by its state. An object transitions from one state to another (something needs to  trigger a transition).
A formalized construct which manages states and transitions is calls a state machine.
</summary>

software as a state machine. transitions can be explicit (from inside the object) or in response to events.

if the machine is complex enough, it's probably worth to define all the transitions in a clear and concise way: state entry and exit behavior. actions on event transitions, guard conditions on external factors, default actions when no transition.

#### Classic GOF

the classic example in the GOF is different than what we would do to today. the light switch example. they used classes to represent the state.this might be heavy because each switch required building a new object? also uses bidirectional control flow.

``` csharp
public abstract class State
{
    public virtual void On (Switch switch)
    {
        //do nothing
    }

    public virtual void Off (Switch switch)
    {
        //do nothing
    }
}

public class OnState : State
{
    public override void Off(Switch switch)
    {
        //transition
        switch.state= new OffState();
    }
}

public class OffState : State
{
    public override void On(Switch switch)
    {
        //transition
        switch.state= new OnState();
    }
}
public class Switch
{
    public State state = new OffState();
    public void On()
    {
        //bi-directional control
        state.On(this);
    }
    public void Off()
    {
        //bi-directional control
        state.Off(this);
    }
}
```

#### More Modern Example

if we didn't have any libraries, today we would do something like this:  
example for a phone state machine. we keep our state as an enum and we have different actions (triggers) to switch between them. there is less overhead of build objects at each transition.

``` csharp
public class PhoneStateMachine
{
    public enum State
    {
        OffHook,
        Connecting,
        Connected,
        OnHold
    }

    public enum Triggers
    {
        CallDialed,
        HungUp,
        CallConnected,
        PlaceOnHold,
        TakenOffHold,
        LeftMessage
    }
    //using (a,b) syntax for pairs. we can also use dictionary<state, dictionary<trigger,state>>
    // which triggers moves us to which state.
    private static Dictionary<State,List<(Trigger,State)>> Transitions = new Dictionary<State,List<(Trigger,State)>>
    {
        [State.OffHook] = new List<(Trigger,State)> {
            (Trigger.CallDialed,State.Connecting)
        },
        [State.Connecting] = new List<(Trigger,State)> {
            (Trigger.CallConnected,State.Connected)
            (Trigger.HungUp,State.OffHook),
        },
        [State.Connected] = new List<(Trigger,State)> {
            (Trigger.LeftMessage,State.OffHook),
            (Trigger.PlaceOnHold,State.OnHold),
            (Trigger.HungUp,State.OffHook)
        },
        [State.OnHold] = new List<(Trigger,State)> {
            (Trigger.TakenOffHold,State.Connected),
            (Trigger.HungUp,State.OffHook)
        }
    };
    public static void Main(string[] args)     
    {
        var state = State.OffHook;
        while (true) //infinite loop
        {
            Console.WriteLine($"The phone is currently at state {state}");
            Console.WriteLine($"select one of the following triggers:");
          
            //display options, we use this form because we want access to element by index.
            for (var i =0; i < Transitions[state].Count; ++i)
            {
                var (trigger, _) = Transitions[state][i];
                Console.WriteLine($"{i}. {trigger}:");
            }

            var input = int.Parse(Console.ReadLine()); //no error checking!
            var (_,nextState) =Transitions[state][input]
            state = nextState;
        }
    }
}
```

#### Switch Based State Machine

Combination Lock Example. this time using a switch case for transitions. rather than a dictionary. we can also use goto statements inside the switch (to move into other cases) instead of an infinite loop. this approach is easier in simle cases and we can have some actions in the transitions. this is still a problematic way to define a state machine.

``` csharp
public class CombinationLock
{
    public enum State
    {
        Locked,
        Failed,
        Unlocked
    }

    public static void Main(string[] args)
    {
        string code = "1234";
        var state= State.Locked;
        var entry = new StringBuilder();

        while (true) //infinite loop
        {
            switch (state)
            {
                case State.Locked:
                entry.append(Console.ReadKey().KeyChar);
                if (entry.ToString() == code)
                {
                    state = State.Unlocked;
                    break;
                }
                if (!code.StartsWith(entry.ToString()))
                {
                    state = State.Failed; 
                }
                break;
                case State.Failed:
                Console.CursorLeft = 0; // this will make the console overwrite the existing characters.
                Console.WriteLine("FAILED");
                entry.Clear();
                state= State.Locked;
                break;
                case State.Unlocked:
                Console.CursorLeft = 0; // this will make the console overwrite the existing characters.
                Console.WriteLine("OPENED!");
                return; //end the program
                break;
            }
        }
    }
}
```

#### Switch Expression State Machine

a [switch expression](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression) (from C# 8.0) isn't the same as a switch statement; it can switch on multiple arguments, use or ignore values in some cases, and even have further constrains with the *when* keyword, the default case is called a *guard case*.
This form is a bit easier to read and have complex conditions for switching between states, but we don't have an easy way to add behavior for different states. we can add wrappers that do stuff, but that's an entire mess by itself.

``` csharp
public class TreasureChest
{
    enum ChestState
    {
        Open,
        Closed,
        Locked
    }

    enum Action
    {
        Open,
        Close
    }

    //this is the switch expression. using pattern matching.
    static ChestState Manipulate(ChestState chestState, Action action, bool haveKey) =>
    (chestState,action, haveKey) switch 
    {
        (ChestState.Locked,Action.Open,true) => ChestState.Open,
        (ChestState.Closed,Action.Open,_) => ChestState.Open, //we don't care about the third argument!
        (ChestState.Open,Action.Closed,true) => ChestState.Locked,
        (ChestState.Open,Action.Closed,false) => ChestState.Closed,
        _ => chestState // default value
    };
    public static void Main(string[] args)
    {
        var chestState = ChestState.Locked;
        chestState = Manipulate(chestState,Action.Open,true);
        chestState = Manipulate(chestState,Action.Closed,false);
        chestState = Manipulate(chestState,Action.Closed,false);
    }
}
```

#### State Machine With the Stateless Library

see [bellow](#Stateless-Library). Microsoft also has library for this.



</details>

### Strategy

<details>
<summary>
Enables the exact behavior of a system to be selected either at run-time (dynamic) or at compile-time(static). Also known as a policy (esp. in the C++ World).
</summary>
Partially specify behavior, decompose algorithms into higher and level parts. reuse parts of algorithms and specialize other parts.
This is what we do with sorting algorithms,

example of creating a string from a list for html or markdown syntax with one interface.

``` csharp
public enum OutputFormat
{
    Markdown,
    Html
}
public interface IListStrategy
{
    void Start(StringBuilder sb);
    void End(StringBuilder sb);
    void AddListItem(StringBuilder sb,string item);
}
public class HtmlListStrategy: IListStrategy
{
    public void Start(StringBuilder sb)
    {
        sb.AppendLine("<ul>");
    }
    public void End(StringBuilder sb)
    {
        sb.AppendLine("</ul>");
    }
    public void AddListItem(StringBuilder sb,string item)
    {
        sb.AppendLine($"\t<li>{item}</li>");
    }
}
public class MarkDownListStrategy: IListStrategy
{
    public void Start(StringBuilder sb)
    {

    }
    public void End(StringBuilder sb)
    {

    }
    public void AddListItem(StringBuilder sb,string item)
    {
        sb.AppendLine($" * {item}");
    }
}

public class TextProcessor
{
    private StringBuilder sb = new StringBuilder();
    private IListStrategy listStrategy;

    public TextProcessor (OutputFormat format)
    {
        switch (format)
        {
            case OutputFormat.Markdown:
            listStrategy= new MarkDownListStrategy();
            break;
            case OutputFormat.Html:
            listStrategy= new HtmlListStrategy();
            break;
        }
    }
    public void AppendList(IEnumerable<string> items)
    {
        listStrategy.Start(sb);
        items.ForEach(item => listStrategy.Append(sb,item));
        listStrategy.End(sb);
    }
    public override string ToString()
    {
        return sb.ToString();
    }
}
```

we change the performance by substituting a single component. we can use dependency injection, use generic arguments and template, or simple pass the object in the constructor.

we see the strategy pattern a lot in equality and comparisons algorithms. a class can have default comparison policy (*IComparable/<T/>,IComparable*) or provide a custom policy for the current sort request with a specific policy (lambda, linq, comparator object);

</details>

### Template Method

<details>
<summary>
Allows us to define the 'skelton' of the algorithm, with concrete implementations defined in subclasses
</summary>

similar to the strategy, but using inheritance rather than composition.

Template method: a high level description of the algorithm in the abstract class. combining base class methods with abstract methods in the derived classes. not exactly PIMPL idiom, but using virtual (abstract) methods for implementations from the derived classes.

in the functional programming paradigm, we would used functions as parameters with closure types and lambdas (or nested/local functions).
</details>

### Visitor

<details>
<summary>
A pattern where a component (visitor) is allowed to traverse the entire inheritance hierarchy. Implemented by propagating a single visit() method throughout the entire hierarchy.
</summary>
Define a new method on an entire class hierarchy without modifying every class in the hierarchy and while still having access to special non common members of each class.

dispatch and double dispatch: determine which function to call, depending on the caller and the name of the request, double dispatch allows to the call to depend on two elements, the visitor and the type of element being visited.

we propagate a method that connects to the visitor, and allow the visitor to do the work, we can either use double dispatch or dynamic casting.

#### Base example without visitor

non visitor example: printing mathematical expressions.
``` csharp
public class Program
{
    //could have been an interface.
    public abstract class Expression
    {
        public abstract void Print(StringBuilder sb);
    }

    public class DoubleExpression : Expression
    {
        public double Value {get;set;}
        public override vod Print(StringBuilder sb)
        {
            sb.Append(Value);
        }
    }

    public class AdditionExpression : Expression
    {
        public Expression Left,Right;
        public override vod Print(StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(Left);    
            sb.Append("+");
            sb.Append(Right);    
            sb.Append(")");
        }

    }
    public static void Main(string[] args) TextProcessor 
    {
        var sb = new StringBuilder();
        var ex = new AdditionExpression
        {
            Left = new DoubleExpression{Value =5},
            Right = new AdditionExpression
            {
                Left = new DoubleExpression{Value = 10},
                Right = new DoubleExpression{Value = 7}
            }
        };

        ex.Print(sb);
        Console.WriteLine(sb.ToString());
    }

}

```

but what would we do if we couldn't add the 'print' method? this is what the visitor pattern is for. here is a version that uses switch statement and a version that uses a predefined table with actions. it's just a semantic difference, we still need to change the dictionary/switch for each new type.

``` csharp
public class Program
{

    //like typedef, defining a type of dictionary that maps a type to an action that that takes an expression and a string builder (doesn't return anything)
    using DictType = Dictionary<Type,Action<Expression, StringBuilder>>;
    //could have been an interface.
    public abstract class Expression
    {
    }

    public class DoubleExpression : Expression
    {
        public double Value {get;set;}
    }

    public class AdditionExpression : Expression
    {
        public Expression Left,Right;

    }

    public static class ExpressionPrinterIf
    {
        public static void Print(Expression e,StringBuilder sb)
        {
            //using reflection/ switch statement
            if (e is DoubleExpression de)
            {
                sb.Append(de.Value); //note that Value must be public
            }
            else if (e is AdditionExpression ae)
            {
            sb.Append("(");
            sb.Append(ae.Left,sb);    
            sb.Append("+");
            sb.Append(ae.Right,sb);    
            sb.Append(")");
            }
            else if (/* other types*/)
            {
                //
            }

        }
    }
    //using a table
    public static class ExpressionPrinterTable
    {
        // our typedef type
        private static DictType actions = new DictType
        {
            //mapping type to action with lambda
            [typeof(DoubleExpression)]= (e,sb) => 
            {
                var de = (DoubleExpression) e; //must cast!
                sb.Append(e.Value);
            }
            [typeof(AdditionExpression)]= (e,sb) => 
            {
                var ae = (AdditionExpression) e; //must cast!
                sb.Append("(");
                Print(ae.Left,sb);    
                sb.Append("+");
                Print(ae.Right,sb);
                sb.Append(")");
            }
        };
        public static void Print (Expression e, StringBuilder sb)
        {
            //get type from expression, access dictionary, call action;
            actions[e.GetType()](e,sb);
        }
    }
    public static void Main(string[] args) TextProcessor 
    {
        var sb = new StringBuilder();
        var ex = new AdditionExpression
        {
            Left = new DoubleExpression{Value =5},
            Right = new AdditionExpression
            {
                Left = new DoubleExpression{Value = 10},
                Right = new DoubleExpression{Value = 7}
            }
        };

        ExpressionPrinterIf.Print(ex,sb);
        Console.WriteLine(sb.ToString());
    }

}
```

#### Double Dispatch Real Visitor Example

following is a real example of using a visitor, the classic double dispatch method. we all of the changes go inside a visitor class/interface, without touching the classes in the hierarchy themselves. this also allows us to define any number of visitors for any purpose. we still have a problem with the return type, whatever we first decide as our return type for the visitor,we're stuck with it and have to compensate for it with the visitor implementations.

``` csharp
public class Program
{

    public interface IExpressionVisitor
    {
        void Visit (DoubleExpression de);
        void Visit (AdditionExpression ae);
        // any new class comes here
        // we should have a Visit(Expression e) for the base class, because we want compile time errors
    }
    public abstract class Expression
    {
        public abstract void Accept(IExpressionVisitor visitor);
    }
    
    public class DoubleExpression : Expression
    {
        public double Value {get;set;}
        public override void Accept(IExpressionVisitor visitor)
        {
            //double dispatch, this will call the correct override
            visitor.Visit(this);
        }
    }

    public class AdditionExpression : Expression
    {
        public Expression Left,Right;
        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ExpressionVisitorPrinter :IExpressionVisitor
    {
        private StringBuilder Sb {get;} = new StringBuilder;
        void Visit (DoubleExpression de)
        {
            Sb.Append(de.Value);
        }
        void Visit (AdditionExpression ae)
        {
            sb.Append("(");
            ae.Left.Accept(this);    
            sb.Append("+");
            ae.Right.Accept(this);    
            sb.Append(")");
        }
    }

    //we made our visitor not return a value, sorry!
    public class ExpressionVisitorCalculator :IExpressionVisitor
    {
        public double Result;
        void Visit (DoubleExpression de)
        {
            Result = de.Value;
        }
        void Visit (AdditionExpression ae)
        {
            ae.Left.Accept(this); //mutate our result value;
            var leftSide = Result;
            ae.Right.Accept(this);//mutate our result value;
            var rightSide = Result;
            Result = leftSide + RightSide;
        }
    }
    public static void Main(string[] args) TextProcessor 
    {
        var ex = new AdditionExpression
        {
            Left = new DoubleExpression{Value =5},
            Right = new AdditionExpression
            {
                Left = new DoubleExpression{Value = 10},
                Right = new DoubleExpression{Value = 7}
            }
        };
        var printerVisitor = new ExpressionVisitorPrinter();
        printerVisitor.Visit(ex); //or ex.Accept(printerVisitor), same thing
        Console.WriteLine(printerVisitor.Sb.ToString());
        // a different kind of visitor!
        var calculatorVisitor = new ExpressionVisitorCalculator();
        calculatorVisitor.Visit(ex);
        Console.WriteLine(calculatorVisitor.Result);
    }

}
```

#### Dynamic Visitor via the DLR

DLR = dynamic language runtime.  
the best case scenario would be to somehow remove the visit/accept operations. we can use the *(dynamic)* cast for the calls in the visitor.  the advantage is that we remove the double hop from accept to visit and we no longer need to change the types. the disadvantage are:
* we take a significant performance hit, 
* we no longer have typechecking after dynamic casting. 
* we won't be aware of the missing overloads until we encounter them in runtime (*Runtime Binder Exceptions*).
* inheritance: (not clear what is the issue).

``` csharp
public class Program
{


    public abstract class Expression
    {
    }
    
    public class DoubleExpression : Expression
    {
        public double Value {get;set;}
    }

    public class AdditionExpression : Expression
    {
        public Expression Left,Right;
    }

    public class ExpressionVisitorPrinter
    {
        
        public void Print (DoubleExpression de,StringBuilder sb)
        {
            sbAppend(de.Value);
        }
        public void Print (AdditionExpression ae,StringBuilder sb)
        {
            sb.Append("(");
            Print((dynamic)ae.Left,sb);
            sb.Append("+");
            Print((dynamic)ae.Right,sb);        
            sb.Append(")");
        }

    }

    public static void Main(string[] args) TextProcessor 
    {
        Expression ex = new AdditionExpression
        {
            Left = new DoubleExpression{Value =5},
            Right = new AdditionExpression
            {
                Left = new DoubleExpression{Value = 10},
                Right = new DoubleExpression{Value = 7}
            }
        };
        var sb = new StringBuilder();
        var printerVisitor = new ExpressionVisitorPrinter();
        printerVisitor.Print((dynamic)ex,sb); //dynamic cast to a supported overload.
         Console.WriteLine(sb.ToString()); 
        // a different kind of visitor!
        var calculatorVisitor = new ExpressionVisitorCalculator();
        Console.WriteLine(calculatorVisitor.Result);
    }
}
```

#### Acyclic Visitor

the double dispatch pattern increases code complexity. we can use Acyclic Visitor to mitigate that complexity at the cost of some performance hit. we check the type before the dispatch.

``` csharp
public class Program
{
    public interface IVisitor<TVisitable>
    {
        void Visit (TVisitable obj);
    }
    //non generic, just a marker interface.
    public interface IVisitor
    {
    }


    public abstract class Expression
    {
        //accept any visitor;
        public virtual void Accept(IVisitor)
        {
            //work just with visitors of this class
            if (visitor is IVisitor<Expression> typed)
            {
                typed.Visit(This);
            }
        }
    }
    
    public class DoubleExpression : Expression
    {
        public double Value {get;set;}
        public override void Accept(IVisitor)
        {
            //not just visitor to Expression, but a visitor that can do work on this concrete class!
            if (visitor is IVisitor<DoubleExpression> typed)
            {
                typed.Visit(This);
            }
        }
    }

    public class AdditionExpression : Expression
    {
        public Expression Left,Right;
        public override void Accept(IVisitor)
        {
            if (visitor is IVisitor<AdditionExpression> typed)
            {
                typed.Visit(This);
            }
        }
    }

    public class ExpressionVisitorPrinter : IVisitor, IVisitor<Expression>,IVisitor<DoubleExpression>,IVisitor<AdditionExpression>,
    {
        StringBuilder Sb = new StringBuilder;
        public void Visit(Expression obj)
        {
            //base class,
        }
        public void Visit (DoubleExpression de)
        {
            Sb.Append(de.Value);
        }
        public void Visit (AdditionExpression ae)
        {
            Sb.Append("(");
            ae.Left.Accept(this);
            Sb.Append("+");
            ae.Right.Accept(this);   
            Sb.Append(")");
        }

    }

    public static void Main(string[] args) TextProcessor 
    {
        Expression ex = new AdditionExpression
        {
            Left = new DoubleExpression{Value =5},
            Right = new AdditionExpression
            {
                Left = new DoubleExpression{Value = 10},
                Right = new DoubleExpression{Value = 7}
            }
        };
        var printerVisitor = new ExpressionVisitorPrinter();
        printerVisitor.Visit(ex);
        Console.WriteLine(printerVisitor.Sb.ToString()); 
    }

}
```

</details>

</details>

## Related Concepts

<details>
<summary>
TODO: add Summary
</summary>

### ASCII C# String

### Continuation Passing Style

### Local Inversion of Control

### DI Container and Event Broker Integration

### Beyond the Elvis Operator

### CQRS and event Sourcing

### Functional Patterns in F#
</details>


## Extra Stuff
<details>
<summary>
Stuff that i didn't know about until now.
</summary>

### Attributes
<details>
<summary>
Attributes to consider using
</summary>

the [\[DebuggerDisplay\] attribute](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggerdisplayattribute?view=net-5.0) that controls how class appears during debug sessions. if we want something other than the 'toString()' override.

the [\[UsedImplicitly\] attribute](https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#UsedImplicitlyAttribute) from resharper is a way to exclude some classes that are used internally (via reflection or something like that) from static analysis usage checks.


the [\[CanBeNull\] attribute](https://www.jetbrains.com/help/resharper/Reference__Code_Annotation_Attributes.html#CanBeNullAttribute) from resharper creates warnings when using an object which we annotated as 'CanBeNull' so it forces us to use the  *foo?.bar()* syntax.

</details>

### Libraries 

<details>
<summary>
Libraries to consider using
</summary>

#### Dependency Injection Framework Autofac

Dependency injection framework [Autofac](https://autofac.org/) is used throughout the code (inversion of control container, dependency inversion?). the container has a builder class *ContainerBuilder* with the *Build()* method. 
* we can register concrete classes.
* classes that implement an interface.
* register all class from the assembly.
* control lifetime and have a singleton object.
* the container implements *IDisposable*, so it can be used inside a using block.

``` csharp
using Autofac;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<BankAccount>(); // concrete registration of a class.
        builder.RegisterType<NullLog>
            .As<ILog>(); // register concrete class for instance

        builder.Register(ctx=> new BankAccount(null)); //register with lambda.
        
        builder.RegisterType<Mediator>()
            .As<IMediator>() // as interface
            .InstancePerLifetimeScope(); //singleton

        builder.Register<ServiceFactory>( ctx =>
        {
            var c = ctx.Resolve<IComponentContext>();
            return t=> c.Resolve(t);
        });

        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces(); //all classes from assembly
        
        var assembly = Assembly.GetExecutingAssembly();
        builder.RegisterAssemblyTypes(ass)
            .AsClosedTypeOf(typeof(ISend<>))
            .SingleInstance();
        using (var container = builder.Build())
        {
            var ba = container.Resolve<BankAccount>(); // get the class from the container.
        }
    }
}

```


#### Dynamic Run Time ImpromptuInterface

[ImpromptuInterface](https://github.com/ekonbenefits/impromptu-interface).
a library that does dynamic runtime stuff, like duck typing, constructing interfaces or force objects to behave as if they were of some interface.

``` csharp
using ImpromptuInterface;
using Dynamitey;

public class Null<TInterface>: DynamicObject where TInterface: class
{
    public static TInterface Instance
    {
        get 
        {
            return new Null<TInterface>().ActLike<TInterface>();// this is now recognized as TInterface object.
        }
    }
    //override all methods.;
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args,out object result)
    {
        result = Activator.CreateInstance(binder.ReturnType); //assuming the return type is default constructable.
        return true;
    }
}
```

#### Stateless Library

Hierarchical state machine library [Stateless](https://github.com/dotnet-state-machine/stateless). the state machine has the starting state, and we configure transitions with *.Configure()* followed by *.Permit()* or *.PermitIf()* to depend on some property.

``` csharp
using Stateless;
public class Program
{
    public enum Health
    {
        NonReproductive,
        Reproductive,
        Pregnant
    }
    public enum Activity
    {
        GiveBirth,
        ReachPuberty,
        HaveAbortion,
        HaveUnProtectedSex,
        Hysterectomy
    }
    public static bool NobodyWatching{
        get;
        set;
    }
    public static void Main (string[] args)
    {
        var machine = new StateMachine<Health,Activity>(Health.NonReproductive); //define state machine with starting State.
        machine.Configure(Health.NonReproductive) //from state
            .Permit(Activity.ReachPuberty,Health.Reproductive); // transition.
        machine.Configure(Health.Reproductive) //from state
            .Permit(Activity.Hysterectomy,Health.NonReproductive) // transition.
            .PermitIf(Activity.HaveUnProtectedSex,Health.Pregnant,()=>NobodyWatching); //permitIf based on condition
        machine.Configure(Health.Pregnant) //from state
            .Permit(Activity.GiveBirth,Health.Reproductive) // transition.
            .Permit(Activity.HaveAbortion,Health.Reproductive); // transition.
    }

}
```

#### Reactive Extension RX

[RX Reactive Extension](https://github.com/dotnet/reactive)
provides some reactive elements to dotNet.
> The Reactive Extensions (Rx) is a library for composing asynchronous and event-based programs using observable sequences and LINQ-style query operators.

</details>

### Misc
<details>
<summary>
Even more extra than the extra!
</summary>

ACID in databases - requirements for transactional operations:
* Atomicity
* Consistency
* Isolation
* Durability

</details>

</details>