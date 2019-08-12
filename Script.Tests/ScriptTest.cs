using NUnit.Framework;

namespace H3ml.Script
{
    public class ScriptTest
    {
        [Test]
        public void EngineTest()
        {
            var engine = new ScriptEngine();
            engine.Test();
        }
    }
}
