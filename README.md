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
In the example, we have TheTestedClass as the Sut. We also mocks the call to `GetUser`. We aquire the faked instance with the method `Fake<T>()`.

## Author
DependencyFaker is written by Johnny Sjöö johnny.sjoo@gmail.com.

## License
DependencyFaker Copyright (C) 2024 Johnny Sjöö

This library is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

See http://www.gnu.org/licenses/.
