using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class IsInCheckTests
{

    private TestGameFactory testGameFactory;

    public IsInCheckTests()
    {
        this.testGameFactory = new TestGameFactory();
    }
    // A Test behaves as an ordinary method
    [Test]
    public void IsInCheckTests_SimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator IsInCheckTests_EnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }


}
