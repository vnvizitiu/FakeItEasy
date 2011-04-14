namespace FakeItEasy.IntegrationTests
{
    using NUnit.Framework;
    using NSubstitute;
    using Tests;

    [TestFixture]
    public class NSubstituteSyntaxTests
    {
        [Test]
        public void Should_be_able_to_specify_return_value()
        {
                // Arrange
                var fake = Substitute.For<IFoo>();

                // Act
                fake.Baz().Returns(() => 10);

                // Assert
                Assert.That(fake.Baz(), Is.EqualTo(10));
            
        }
    }
}