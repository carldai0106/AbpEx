using Newtonsoft.Json;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Batch.Tests.Others
{
    public class Other_Tests : BatchTestBase
    {
        public Other_Tests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public void Timezone_Test()
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            Output.WriteLine(JsonConvert.SerializeObject(info, Formatting.Indented));

            string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Output.WriteLine(s.Substring(0, 15).Length.ToString());
        }
    }
}
