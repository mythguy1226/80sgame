using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasicTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void BasicTestsSimplePasses()
    {
        // Arrange
        int summandA = 2;
        int summandB = 2;
        int expectedValue = 4;

        // Act
        int result = summandA + summandB;

        // Assert
        Assert.AreEqual(expectedValue, result);

    }

}
