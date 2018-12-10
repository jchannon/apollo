// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System;
    using Apollo.Features.Verification;
    using Xunit;

    public class VerificationCodeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void VerificationCodeCanNotBeNullOrEmpty(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new VerificationCode(value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void VerificationCodeCanNotHaveLessThan4Digits(int length)
        {
            Assert.Throws<ArgumentException>(() => new VerificationCode(new string('0', length)));
        }

        [Theory]
        [InlineData(5)]
        [InlineData(100)]
        public void VerificationCodeCanNotHaveMoreThan4Digits(int length)
        {
            Assert.Throws<ArgumentException>(() => new VerificationCode(new string('0', length)));
        }

        [Theory]
        [InlineData("A000")]
        [InlineData("0A00")]
        [InlineData("00A0")]
        [InlineData("000A")]
        public void VerificationCodeCanNotContainCharactersThatAreNotDigits(string value)
        {
            Assert.Throws<ArgumentException>(() => new VerificationCode(value));
        }

        [Fact]
        public void DoesEqualSelf()
        {
            var sut = VerificationCode.Generate();

            Assert.True(sut.Equals(sut));
        }

        [Fact]
        public void DoesNotEqualNull()
        {
            var sut = VerificationCode.Generate();

            Assert.False(sut.Equals((object)null));
        }

        [Fact]
        public void DoesNotEqualOtherType()
        {
            var sut = VerificationCode.Generate();

            Assert.False(sut.Equals(new object()));
        }

        [Fact]
        public void DoesNotEquatableEqualNull()
        {
            var sut = VerificationCode.Generate();

            Assert.False(sut.Equals(null));
        }

        [Fact]
        public void GenerateReturnsExpectedResult()
        {
            Assert.IsType<VerificationCode>(VerificationCode.Generate());
        }

        [Fact]
        public void IsEquatable()
        {
            var sut = VerificationCode.Generate();

            Assert.IsAssignableFrom<IEquatable<VerificationCode>>(sut);
        }

        [Fact]
        public void SuccessiveGetHashCodeReturnsSameValue()
        {
            var sut = VerificationCode.Generate();

            Assert.True(sut.GetHashCode().Equals(sut.GetHashCode()));
        }

        [Fact]
        public void ToStringReturnsExpectedResult()
        {
            const string value = "0000";
            var sut = new VerificationCode(value);

            var result = sut.ToString();

            Assert.Equal(value, result);
        }

        [Fact]
        public void TwoInstanceAreEqualWhenTheirValuesAreEqual()
        {
            var sut = VerificationCode.Generate();
            var other = new VerificationCode(sut.ToString());

            Assert.True(sut.Equals(other));
        }

        [Fact]
        public void TwoInstanceAreNotEqualWhenTheirValuesAreNotEqual()
        {
            var sut = VerificationCode.Generate();
            var other = VerificationCode.Generate();

            Assert.False(sut.Equals(other));
        }

        [Fact]
        public void TwoInstanceDoNotHaveTheSameHashCodeWhenTheirValuesAreNotEqual()
        {
            var sut = VerificationCode.Generate();
            var other = VerificationCode.Generate();

            Assert.False(sut.GetHashCode().Equals(other.GetHashCode()));
        }

        [Fact]
        public void TwoInstanceHaveTheSameHashCodeWhenTheirValuesAreEqual()
        {
            var sut = VerificationCode.Generate();
            var other = VerificationCode.Generate();

            Assert.False(sut.GetHashCode().Equals(other.GetHashCode()));
        }
    }
}
