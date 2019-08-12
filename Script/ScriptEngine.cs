using System;
using V8.Net;

namespace H3ml.Script
{
    public class ScriptEngine
    {
        V8Engine _v8 = new V8Engine();

        public void Test()
        {
            _v8.Execute("function foo(s) { /* Some JavaScript Code Here */ return s; }", "My V8.NET Console");
            var result = _v8.DynamicGlobalObject.foo("bar!");
            var result2 = (string)result;
        }
    }
}
