using UnityEditor;

namespace Tests
{
	public class Tests
	{

		[NUnit.Framework.Test]
		public void TestsSimplePasses()
		{
			// Use the Assert class to test conditions.
			
		}

		// A UnityTest behaves like a coroutine in PlayMode
		// and allows you to yield null to skip a frame in EditMode
		[UnityEngine.TestTools.UnityTest]
		public System.Collections.IEnumerator TestsWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// yield to skip a frame
			yield return null;
		}
	}
}