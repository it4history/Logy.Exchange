#if DEBUG
using System;
using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Smw.Tests
{
    [TestFixture]
    public class SemanticWikiParserTests
    {
        private Site _site;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _site = new Site("http://logy.gq/lw", "te", "test");
        }

        [Test]
        public void Get()
        {
            Assert.IsFalse(SemanticWikiParser.Get(_site, "[[Modification date::+]]|?Modification date")
                .Contains("readapidenied"));
        }

        private const string ApiResult = @"     {  ""query"": {
    ""printrequests"": [
      {
        ""label"": """",
        ""key"": """",
        ""redi"": """",
        ""typeid"": ""_wpg"",
        ""mode"": 2
      },
      {
        ""label"": ""Birthday"",
        ""key"": ""Birthday"",
        ""redi"": """",
        ""typeid"": ""_dat"",
        ""mode"": 1,
        ""format"": """"
      }
    ],
    ""results"": {
      ""н, Олександр"": {
        ""printouts"": {
          ""Birthday"": [
            {
              ""timestamp"": ""246758400"",
              ""raw"": ""1/1977/10/27""
            }
          ]
        },
        ""fulltext"": ""н, Олександр"",
        ""fullurl"": ""http://a.a/a/%BD,_%D0%9E%D0%BB%D0%B5%D0%BA%D1%81%D0%B0%D0%BD%D0%B4%D1%80"",
        ""namespace"": 0,
        ""exists"": ""1"",
        ""displaytitle"": """"
      }
    },
    ""serializer"": ""SMW\\Serializers\\QueryResultSerializer"",
    ""version"": 2,
    ""meta"": {
      ""hash"": ""93c039fcbe2a37ef2147e8849dd6fcd0"",
      ""count"": 4,
      ""offset"": 0,
      ""source"": """",
      ""time"": ""0.011534""
    }
  }}";

        [Test]
        public void Parse()
        {
            Assert.AreEqual(
                new DateTime(1977, 10, 27),
                SemanticWikiParser.Parse(ApiResult)["н, Олександр"]["Birthday"].DateTime);
        }
    }
}
#endif
