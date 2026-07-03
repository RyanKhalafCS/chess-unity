using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SetOccupierTests
{

    private TestGameFactory testGameFactory;

    public SetOccupierTests()
    {
        this.testGameFactory = new TestGameFactory();
    }
    // A Test behaves as an ordinary method
    [Test]
    public void SetOccupierTests_SimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SetOccupierTests_EnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}