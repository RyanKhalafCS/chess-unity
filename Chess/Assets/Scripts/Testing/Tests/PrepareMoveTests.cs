using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PrepareMoveTests
{

    private TestGameFactory _testGameFactory;

    public PrepareMoveTests()
    {
        _testGameFactory = new TestGameFactory();
    }
    // A Test behaves as an ordinary method
    [Test]
    public void PrepareMoveTests_SimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PrepareMoveTests_EnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}