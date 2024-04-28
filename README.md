# DependencyFaker
DependencyFaker lets you write unit tests without worrying about the dependencies of the Sut (system/subject under test) or any other subsequent dependencies. You can also register your own dependencies with ease.
Currently the package supports FakeItEasy but it's easy to add more fakeing frameworks.

To use DependencyFaker, you just inherit from the DependencyFaker<TSut> class.
## Example
```
public class UnitTestClass : FakeItEasyDependencyFaker<TheTestedClass>
{
  [Fact]
  public void SomeUnitTest
  {
    // Arrange
    var id = Guid.NewGuid();
    A.CallTo(() => Fake<IFakedClass>().GetUser(A<Guid>._))
      .Returns(new User(id));

    // Act
    var result = Sut.GetOrRegisterUser(id);

    // Assert
    result.IsActive.Should()
      .BeTrue();
  }
}
```
In the example, we have `TheTestedClass` as the Sut. We also mocks the call to `GetUser`. We aquire a faked dependency with the method `Fake<T>()`. Lastly the Sut property is the instance of the class we are testing, injected with faked (or registered) dependencies.

## Author
DependencyFaker is written by Johnny Sjöö johnny.sjoo@gmail.com.

## License
The MIT License (MIT)

Copyright (c) Johnny Sjöö and contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
