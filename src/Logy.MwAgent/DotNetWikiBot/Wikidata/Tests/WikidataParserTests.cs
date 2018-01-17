﻿#if DEBUG

using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata.Tests
{
    [TestFixture]
    public class WikidataParserTests
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            SitelinksConverter.Languages = new[] { "en", "uk", "ru" };
        }

        [Test]
        public void ParseWikidataItem()
        {
            var json = @"
{ ""entities"":{""Q70899"":{""pageid"":73608,""ns"":0,""title"":""Q70899"",""lastrevid"":418017969,""modified"":""2016-12-19T12:06:48Z"",""type"":""item"",""id"":""Q70899"",""labels"":{""zh"":{""language"":""zh"",""value"":""\u4e9e\u7576""},""ky"":{""language"":""ky"",""value"":""\u0410\u0434\u0430\u043c""},""bo"":{""language"":""bo"",""value"":""\u0f68\u0f0b\u0f51\u0f58\u0f0d""},""jv"":{""language"":""jv"",""value"":""Adam""},""eu"":{""language"":""eu"",""value"":""Adam""},""bs"":{""language"":""bs"",""value"":""Adem""},""uz"":{""language"":""uz"",""value"":""Odam Ato""},""es"":{""language"":""es"",""value"":""Ad\u00e1n""},""ta"":{""language"":""ta"",""value"":""\u0b86\u0ba4\u0bbe\u0bae\u0bcd""},""oc"":{""language"":""oc"",""value"":""Adam""},""ce"":{""language"":""ce"",""value"":""Adam""},""sw"":{""language"":""sw"",""value"":""Adamu""},""et"":{""language"":""et"",""value"":""Aadam""},""bn"":{""language"":""bn"",""value"":""\u0986\u09a6\u09ae""},""sq"":{""language"":""sq"",""value"":""Adami""},""br"":{""language"":""br"",""value"":""Adam""},""rw"":{""language"":""rw"",""value"":""Adamu""},""el"":{""language"":""el"",""value"":""\u0391\u03b4\u03ac\u03bc""},""ty"":{""language"":""ty"",""value"":""Adamu""},""nl"":{""language"":""nl"",""value"":""Adam""},""ar"":{""language"":""ar"",""value"":""\u0622\u062f\u0645""},""eo"":{""language"":""eo"",""value"":""Adamo""},""yi"":{""language"":""yi"",""value"":""\u05d0\u05d3\u05dd \u05d4\u05e8\u05d0\u05e9\u05d5\u05df""},""ru"":{""language"":""ru"",""value"":""\u0410\u0434\u0430\u043c""},""diq"":{""language"":""diq"",""value"":""Adem""},""tr"":{""language"":""tr"",""value"":""\u00c2dem""},""ckb"":{""language"":""ckb"",""value"":""\u0626\u0627\u062f\u06d5\u0645""},""fi"":{""language"":""fi"",""value"":""Aadam""},""uk"":{""language"":""uk"",""value"":""\u0410\u0434\u0430\u043c""},""be-tarask"":{""language"":""be-tarask"",""value"":""\u0410\u0434\u0430\u043c""},""az"":{""language"":""az"",""value"":""Ad\u0259m""},""hr"":{""language"":""hr"",""value"":""Adam""},""tl"":{""language"":""tl"",""value"":""Adan""},""da"":{""language"":""da"",""value"":""Adam""},""kk"":{""language"":""kk"",""value"":""\u0410\u0434\u0430\u043c \u0430\u0442\u0430""},""kab"":{""language"":""kab"",""value"":""Adam""},""wa"":{""language"":""wa"",""value"":""Adan""},""so"":{""language"":""so"",""value"":""Nabi Aadam C.S.""},""be"":{""language"":""be"",""value"":""\u0410\u0434\u0430\u043c""},""arz"":{""language"":""arz"",""value"":""\u0622\u062f\u0645""},""fr"":{""language"":""fr"",""value"":""Adam""},""ko"":{""language"":""ko"",""value"":""\uc544\ub2f4""},""ug"":{""language"":""ug"",""value"":""\u0626\u0627\u062f\u06d5\u0645 \u0626\u06d5\u0644\u06d5\u064a\u06be\u0649\u0633\u0633\u0627\u0644\u0627\u0645""},""lbe"":{""language"":""lbe"",""value"":""\u0410\u0434\u0430\u043c \u0438\u0434\u0430\u0432\u0441""},""ace"":{""language"":""ace"",""value"":""Adam""},""lv"":{""language"":""lv"",""value"":""\u0100dams""},""it"":{""language"":""it"",""value"":""Adamo""},""ps"":{""language"":""ps"",""value"":""\u0622\u062f\u0645""},""id"":{""language"":""id"",""value"":""Adam""},""ja"":{""language"":""ja"",""value"":""\u30a2\u30c0\u30e0""},""ml"":{""language"":""ml"",""value"":""\u0d06\u0d26\u0d3e\u0d02""},""sh"":{""language"":""sh"",""value"":""Adam""},""scn"":{""language"":""scn"",""value"":""Addamu""},""wo"":{""language"":""wo"",""value"":""Aadama""},""ku"":{""language"":""ku"",""value"":""Adem""},""mn"":{""language"":""mn"",""value"":""\u0410\u0434\u0430\u043c""},""hy"":{""language"":""hy"",""value"":""\u0531\u0564\u0561\u0574""},""en"":{""language"":""en"",""value"":""Adam""},""th"":{""language"":""th"",""value"":""\u0e2d\u0e32\u0e14\u0e31\u0e21""},""ca"":{""language"":""ca"",""value"":""Adam""},""la"":{""language"":""la"",""value"":""Adam""},""vep"":{""language"":""vep"",""value"":""Adam""},""fo"":{""language"":""fo"",""value"":""\u00c1dam""},""cy"":{""language"":""cy"",""value"":""Adda""},""war"":{""language"":""war"",""value"":""Adan""},""bg"":{""language"":""bg"",""value"":""\u0410\u0434\u0430\u043c""},""fa"":{""language"":""fa"",""value"":""\u0622\u062f\u0645""},""te"":{""language"":""te"",""value"":""\u0c06\u0c26\u0c3e\u0c2e\u0c41""},""arc"":{""language"":""arc"",""value"":""\u0710\u0715\u0721""},""ka"":{""language"":""ka"",""value"":""\u10d0\u10d3\u10d0\u10db\u10d8""},""de"":{""language"":""de"",""value"":""Adam""},""gl"":{""language"":""gl"",""value"":""Ad\u00e1n""},""cs"":{""language"":""cs"",""value"":""Adam""},""mk"":{""language"":""mk"",""value"":""\u0410\u0434\u0430\u043c""},""ro"":{""language"":""ro"",""value"":""Adam""},""vi"":{""language"":""vi"",""value"":""Adam""},""sv"":{""language"":""sv"",""value"":""Adam""},""pt"":{""language"":""pt"",""value"":""Ad\u00e3o""},""he"":{""language"":""he"",""value"":""\u05d0\u05d3\u05dd""},""ba"":{""language"":""ba"",""value"":""\u04d8\u0499\u04d9\u043c""},""pl"":{""language"":""pl"",""value"":""Adam""},""zh-hk"":{""language"":""zh-hk"",""value"":""\u4e9e\u7576""},""zh-tw"":{""language"":""zh-tw"",""value"":""\u4e9e\u7576""},""zh-hans"":{""language"":""zh-hans"",""value"":""\u4e9a\u5f53""},""zh-cn"":{""language"":""zh-cn"",""value"":""\u4e9a\u5f53""},""zh-hant"":{""language"":""zh-hant"",""value"":""\u4e9e\u7576""},""sco"":{""language"":""sco"",""value"":""Adam""},""pa"":{""language"":""pa"",""value"":""\u0a06\u0a26\u0a2e""},""cdo"":{""language"":""cdo"",""value"":""\u0100-d\u014fng""},""yue"":{""language"":""yue"",""value"":""\u4e9e\u7576""},""sr"":{""language"":""sr"",""value"":""\u0410\u0434\u0430\u043c""},""azb"":{""language"":""azb"",""value"":""\u0622\u062f\u0627\u0645""},""av"":{""language"":""av"",""value"":""\u0410\u0434\u0430\u043c \u0430\u0432\u0430\u0440\u0430\u0433""},""en-ca"":{""language"":""en-ca"",""value"":""Adam""},""en-gb"":{""language"":""en-gb"",""value"":""Adam""},""nan"":{""language"":""nan"",""value"":""A-tong""},""mg"":{""language"":""mg"",""value"":""Adam""},""sd"":{""language"":""sd"",""value"":""\u0622\u062f\u0645 \u0639\u0644\u064a\u06c1 \u0627\u0644\u0633\u0644\u0627\u0645""},""nb"":{""language"":""nb"",""value"":""Adam""},""ast"":{""language"":""ast"",""value"":""Ad\u00e1n""},""tg"":{""language"":""tg"",""value"":""\u041e\u0434\u0430\u043c""}},""descriptions"":{""en"":{""language"":""en"",""value"":""biblical figure in the Book of Genesis""},""fr"":{""language"":""fr"",""value"":""premier homme, selon la Bible""},""de"":{""language"":""de"",""value"":""Biblische Figur""},""hr"":{""language"":""hr"",""value"":""Biblijski lik""},""cs"":{""language"":""cs"",""value"":""biblick\u00e1 postava""},""mk"":{""language"":""mk"",""value"":""\u0431\u0438\u0431\u043b\u0438\u0441\u043a\u0438 \u043b\u0438\u043a""},""es"":{""language"":""es"",""value"":""primer hombre, seg\u00fan la Biblia""},""pt"":{""language"":""pt"",""value"":""figura b\u00edblica do livro de G\u00eanesis""},""gl"":{""language"":""gl"",""value"":""segundo o relato b\u00edblico foi a primeira persoa creada por Deus""},""ru"":{""language"":""ru"",""value"":""\u0441\u043e\u0433\u043b\u0430\u0441\u043d\u043e \u0411\u0438\u0431\u043b\u0438\u0438, \u043f\u0435\u0440\u0432\u044b\u0439 \u0447\u0435\u043b\u043e\u0432\u0435\u043a \u043d\u0430 \u0417\u0435\u043c\u043b\u0435""},""he"":{""language"":""he"",""value"":""\u05d3\u05de\u05d5\u05ea \u05ea\u05e0\u05db\u05d9\u05ea""},""uk"":{""language"":""uk"",""value"":""\u0432 \u0421\u0442\u0430\u0440\u043e\u043c\u0443 \u0417\u0430\u043f\u043e\u0432\u0456\u0442\u0456 \u043f\u0435\u0440\u0448\u0430 \u043b\u044e\u0434\u0438\u043d\u0430, \u0431\u0430\u0442\u044c\u043a\u043e \u0440\u043e\u0434\u0443 \u043b\u044e\u0434\u0441\u044c\u043a\u043e\u0433\u043e""}},""aliases"":{""zh"":[{""language"":""zh"",""value"":""\u4e9a\u5f53""},{""language"":""zh"",""value"":""Adam""}],""ky"":[{""language"":""ky"",""value"":""\u0410\u0434\u0430\u043c \u0410\u0442\u0430""}],""bo"":[{""language"":""bo"",""value"":""\u0f68\u0f0b\u0f51\u0f58\u0f0b""}],""jv"":[{""language"":""jv"",""value"":""Nabi Adam""}],""eu"":[{""language"":""eu"",""value"":""Adan""}],""bs"":[{""language"":""bs"",""value"":""Adam""}],""es"":[{""language"":""es"",""value"":""Ad\u00e1nico""},{""language"":""es"",""value"":""Adam""}],""ce"":[{""language"":""ce"",""value"":""\u0410\u0434\u0430\u043c""}],""sw"":[{""language"":""sw"",""value"":""Adam""}],""sq"":[{""language"":""sq"",""value"":""Adam""}],""rw"":[{""language"":""rw"",""value"":""Adam""}],""ar"":[{""language"":""ar"",""value"":""\u0622\u062f\u064e\u0645\u064e""},{""language"":""ar"",""value"":""\u0627\u0644\u0646\u0628\u064a \u0622\u062f\u0645""},{""language"":""ar"",""value"":""\u0623\u062f\u0645""},{""language"":""ar"",""value"":""\u0627\u062f\u0645""},{""language"":""ar"",""value"":""\u0622\u062f\u0645 \u0639\u0644\u064a\u0647 \u0627\u0644\u0633\u0644\u0627\u0645""},{""language"":""ar"",""value"":""\u0623\u0628\u0648 \u0627\u0644\u0628\u0634\u0631""},{""language"":""ar"",""value"":""\u062e\u0637\u064a\u0626\u0629 \u0627\u062f\u0645""}],""diq"":[{""language"":""diq"",""value"":""Hz Adem""},{""language"":""diq"",""value"":""Hz. Adem""}],""tr"":[{""language"":""tr"",""value"":""Hz.Adem'in hayati""},{""language"":""tr"",""value"":""Hz Adem""},{""language"":""tr"",""value"":""Hz.Adem""},{""language"":""tr"",""value"":""Adem""},{""language"":""tr"",""value"":""Hz. Adem""},{""language"":""tr"",""value"":""Adam""}],""fi"":[{""language"":""fi"",""value"":""Aatami""}],""az"":[{""language"":""az"",""value"":""Ad\u0259m pey\u011f\u0259mb\u0259r""}],""hr"":[{""language"":""hr"",""value"":""Adem""}],""so"":[{""language"":""so"",""value"":""Nabi Aadam C.S""}],""ko"":[{""language"":""ko"",""value"":""\uc560\ub364""}],""lbe"":[{""language"":""lbe"",""value"":""\u0410\u0434\u0430\u043c""}],""ace"":[{""language"":""ace"",""value"":""Nabi Adam""}],""it"":[{""language"":""it"",""value"":""Sant'Adamo""}],""ps"":[{""language"":""ps"",""value"":""\u062d\u0636\u0631\u062a \u0622\u062f\u0645 \u0639\u0644\u064a\u0647 \u0627\u0644\u0633\u0644\u0627\u0645""}],""id"":[{""language"":""id"",""value"":""Nabi Adam""},{""language"":""id"",""value"":""Adam AS""}],""ja"":[{""language"":""ja"",""value"":""\u30a2\u30fc\u30c0\u30e0""},{""language"":""ja"",""value"":""Adam""}],""ml"":[{""language"":""ml"",""value"":""Adam""},{""language"":""ml"",""value"":""\u0d06\u0d26\u0d02 \u0d28\u0d2c\u0d3f""},{""language"":""ml"",""value"":""\u0d06\u0d26\u0d02""}],""sh"":[{""language"":""sh"",""value"":""Adem""}],""ku"":[{""language"":""ku"",""value"":""P\u00eaxamber Adem""}],""th"":[{""language"":""th"",""value"":""\u0e2d\u0e14\u0e31\u0e21""}],""war"":[{""language"":""war"",""value"":""Adam""}],""fa"":[{""language"":""fa"",""value"":""\u06cc\u0645""},{""language"":""fa"",""value"":""\u062d\u0636\u0631\u062a \u0622\u062f\u0645""},{""language"":""fa"",""value"":""\u0627\u062f\u0645 \u0627\u0628\u0648\u0627\u0644\u0628\u0634\u0631""},{""language"":""fa"",""value"":""\u062d\u0636\u0631\u062a \u0627\u062f\u0645""},{""language"":""fa"",""value"":""\u0622\u062f\u0645 \u0627\u0628\u0648\u0627\u0644\u0628\u0634\u0631""}],""he"":[{""language"":""he"",""value"":""\u05d0\u05b8\u05d3\u05b8\u05dd""}]},""claims"":{""P21"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P21"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":6581097,""id"":""Q6581097""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""q70899$93DB783B-2C97-4895-B755-D5DE4D407536"",""rank"":""normal""}],""P26"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P26"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":830183,""id"":""Q830183""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""q70899$7D634E5F-9B43-4136-94EC-AE9F927DF9E5"",""rank"":""normal""}],""P227"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P227"",""datavalue"":{""value"":""118646877"",""type"":""string""},""datatype"":""external-id""},""type"":""statement"",""id"":""q70899$6DDDA6A7-8AEE-40BC-8E48-E33543583BB7"",""rank"":""normal""}],""P373"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P373"",""datavalue"":{""value"":""Adam"",""type"":""string""},""datatype"":""string""},""type"":""statement"",""id"":""q70899$F5CADEA1-7263-4A65-A266-A99594FC5ACE"",""rank"":""normal""}],""P40"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P40"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":205365,""id"":""Q205365""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""q70899$0E4897B7-01D5-4EDE-B274-5497329E1C48"",""rank"":""normal"",""references"":[{""hash"":""51025e5a30427c85b700a863987657c41010e317"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":9184,""id"":""Q9184""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}],""P792"":[{""snaktype"":""value"",""property"":""P792"",""datavalue"":{""value"":""4"",""type"":""string""},""datatype"":""string""}],""P958"":[{""snaktype"":""value"",""property"":""P958"",""datavalue"":{""value"":""1"",""type"":""string""},""datatype"":""string""}]},""snaks-order"":[""P248"",""P792"",""P958""]}]},{""mainsnak"":{""snaktype"":""value"",""property"":""P40"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":313421,""id"":""Q313421""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""q70899$DBF182EA-6C8E-401F-A492-A487F80741D1"",""rank"":""normal"",""references"":[{""hash"":""2f46ac1342e0ead4c35b54939a483246e3d96575"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":9184,""id"":""Q9184""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}],""P792"":[{""snaktype"":""value"",""property"":""P792"",""datavalue"":{""value"":""4"",""type"":""string""},""datatype"":""string""}],""P958"":[{""snaktype"":""value"",""property"":""P958"",""datavalue"":{""value"":""2"",""type"":""string""},""datatype"":""string""}]},""snaks-order"":[""P248"",""P792"",""P958""]}]},{""mainsnak"":{""snaktype"":""value"",""property"":""P40"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":107626,""id"":""Q107626""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""q70899$475C2AAE-1EEB-4760-8E32-D09A7F1A020F"",""rank"":""normal"",""references"":[{""hash"":""dd440ddb219883973a00159967be0e6bc1071de0"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":9184,""id"":""Q9184""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}],""P792"":[{""snaktype"":""value"",""property"":""P792"",""datavalue"":{""value"":""4"",""type"":""string""},""datatype"":""string""}],""P958"":[{""snaktype"":""value"",""property"":""P958"",""datavalue"":{""value"":""25"",""type"":""string""},""datatype"":""string""}]},""snaks-order"":[""P248"",""P792"",""P958""]}]}],""P361"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P361"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":58701,""id"":""Q58701""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""q70899$918601C9-B848-44DC-BDEB-6A075399C824"",""rank"":""normal""}],""P18"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P18"",""datavalue"":{""value"":""God2-Sistine Chapel.png"",""type"":""string""},""datatype"":""commonsMedia""},""type"":""statement"",""id"":""q70899$DC995DC4-5274-490D-A9C1-4FEED5B98123"",""rank"":""normal"",""references"":[{""hash"":""7eb64cf9621d34c54fd4bd040ed4b61a88c4a1a0"",""snaks"":{""P143"":[{""snaktype"":""value"",""property"":""P143"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":328,""id"":""Q328""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""snaks-order"":[""P143""]}]}],""P31"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P31"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":20643955,""id"":""Q20643955""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$A411FCA0-1A8A-4C77-90F0-C68C909D5B49"",""rank"":""normal""}],""P214"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P214"",""datavalue"":{""value"":""102322856"",""type"":""string""},""datatype"":""external-id""},""type"":""statement"",""id"":""Q70899$3DE60FF4-8758-43E3-827C-DC3D5F96F50A"",""rank"":""normal"",""references"":[{""hash"":""a51d6594fee36c7452eaed2db35a4833613a7078"",""snaks"":{""P143"":[{""snaktype"":""value"",""property"":""P143"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":54919,""id"":""Q54919""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""snaks-order"":[""P143""]}]}],""P646"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P646"",""datavalue"":{""value"":""/m/09_c5v"",""type"":""string""},""datatype"":""external-id""},""type"":""statement"",""id"":""Q70899$51DCCEC8-84F0-4E7E-93C6-24B99257FF63"",""rank"":""normal"",""references"":[{""hash"":""af38848ab5d9d9325cffd93a5ec656cc6ca889ed"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":15241312,""id"":""Q15241312""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}],""P577"":[{""snaktype"":""value"",""property"":""P577"",""datavalue"":{""value"":{""time"":""+2013-10-28T00:00:00Z"",""timezone"":0,""before"":0,""after"":0,""precision"":11,""calendarmodel"":""http://www.wikidata.org/entity/Q1985727""},""type"":""time""},""datatype"":""time""}]},""snaks-order"":[""P248"",""P577""]}]}],""P1422"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P1422"",""datavalue"":{""value"":""199"",""type"":""string""},""datatype"":""external-id""},""type"":""statement"",""id"":""Q70899$64CE400E-680F-453A-955B-9046388D7F79"",""rank"":""normal""}],""P1438"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P1438"",""datavalue"":{""value"":""10077"",""type"":""string""},""datatype"":""string""},""type"":""statement"",""id"":""Q70899$1DCB6FD7-1FAD-40B0-8DC5-D25483AC6B5C"",""rank"":""normal"",""references"":[{""hash"":""d6e3ab4045fb3f3feea77895bc6b27e663fc878a"",""snaks"":{""P143"":[{""snaktype"":""value"",""property"":""P143"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":206855,""id"":""Q206855""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""snaks-order"":[""P143""]}]}],""P460"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P460"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":2001710,""id"":""Q2001710""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$A4C09AB8-2B1F-40E0-8E3A-E7DB1694305F"",""rank"":""normal""}],""P1441"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P1441"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":1845,""id"":""Q1845""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$6BB5952C-EC7F-47FB-B67B-2B3F18E565F4"",""rank"":""normal""}],""P735"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P735"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":347181,""id"":""Q347181""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$b254d287-4911-3539-9c67-6ce6d9db9c0f"",""rank"":""normal""}],""P1343"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":602358,""id"":""Q602358""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""6352b63855f88244988cbe426dc906c3219dc7b9"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":21000388,""id"":""Q21000388""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$F5B7BDBB-F832-4537-9673-0630C8673323"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":19180675,""id"":""Q19180675""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""b19f51f729ef7aa2beb7e3fa2437c41c2d6a4b2e"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":21000390,""id"":""Q21000390""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$4718FB79-4676-4AE9-8EEF-C68682D45570"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":4086271,""id"":""Q4086271""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""7ff82a3d089477b3ed98e562551f33abecc4032c"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":21000405,""id"":""Q21000405""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$20CD5295-976C-43EE-AE38-1CECC839C3B0"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":4173137,""id"":""Q4173137""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""99d582a1dc8da630b3d34eb7518f88e76ab3d75e"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":21000404,""id"":""Q21000404""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$CBD2A24C-F288-4D45-AEDC-BFD330D4BEE1"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":19211082,""id"":""Q19211082""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""66808ea1ee1914f342eb7ef6e5c61f2b9ae59477"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":21000470,""id"":""Q21000470""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$BBE3711E-58D2-45C6-9DCC-574DD85BEC00"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":2041543,""id"":""Q2041543""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""afde57c847178c1adc8669440381b87798add669"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":23849622,""id"":""Q23849622""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$63708B61-4401-4344-B956-3666D8AFC779"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":20961706,""id"":""Q20961706""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""29166890ee46155a9e85577eb4cc7fd5d1dbc71e"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":24451316,""id"":""Q24451316""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$998C9FE7-A483-4666-B56F-EE46CC78B653"",""rank"":""normal""},{""mainsnak"":{""snaktype"":""value"",""property"":""P1343"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":4532135,""id"":""Q4532135""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""qualifiers"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""hash"":""6e8e59f2da1055c0b373390fa9e182b032f776e6"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":26237939,""id"":""Q26237939""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""qualifiers-order"":[""P248""],""id"":""Q70899$D29CB3DD-3161-4028-B120-E89B93074EFB"",""rank"":""normal""}],""P345"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P345"",""datavalue"":{""value"":""ch0034736"",""type"":""string""},""datatype"":""external-id""},""type"":""statement"",""id"":""Q70899$2c449e6c-4a69-7d9b-886e-d5d271c4b50b"",""rank"":""normal""}],""P22"":[{""mainsnak"":{""snaktype"":""novalue"",""property"":""P22"",""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$86dc4845-47ee-9649-b749-6dc5303b9019"",""rank"":""normal""}],""P25"":[{""mainsnak"":{""snaktype"":""novalue"",""property"":""P25"",""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$09131d6b-4173-f14e-37ee-011086c92edb"",""rank"":""normal""}],""P551"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P551"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":19014,""id"":""Q19014""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$D0FF420A-E154-4EA5-BCF4-8341046CC282"",""rank"":""normal"",""references"":[{""hash"":""bb2cb046acf04a2ecca2912ac7dcafa13b85751b"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":19786,""id"":""Q19786""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""snaks-order"":[""P248""]}]}],""P19"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P19"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":19014,""id"":""Q19014""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$E2F941E6-FCDA-4A4C-BFC7-E388E941C5F7"",""rank"":""normal"",""references"":[{""hash"":""bb2cb046acf04a2ecca2912ac7dcafa13b85751b"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":19786,""id"":""Q19786""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}]},""snaks-order"":[""P248""]}]}],""P1711"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P1711"",""datavalue"":{""value"":""52658"",""type"":""string""},""datatype"":""external-id""},""type"":""statement"",""id"":""Q70899$6C7E4188-B7AC-460A-923A-457D54CF02D8"",""rank"":""normal""}],""P569"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P569"",""datavalue"":{""value"":{""time"":""-3760-00-00T00:00:00Z"",""timezone"":0,""before"":0,""after"":0,""precision"":9,""calendarmodel"":""http://www.wikidata.org/entity/Q1985727""},""type"":""time""},""datatype"":""time""},""type"":""statement"",""id"":""Q70899$23c50aee-44a8-f1ca-bec5-64b3c25723d0"",""rank"":""normal"",""references"":[{""hash"":""1148d9104fa0dccadca811349e9d03753809cd07"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":27893345,""id"":""Q27893345""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}],""P304"":[{""snaktype"":""value"",""property"":""P304"",""datavalue"":{""value"":""7"",""type"":""string""},""datatype"":""string""}]},""snaks-order"":[""P248"",""P304""]}]}],""P570"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P570"",""datavalue"":{""value"":{""time"":""-2831-00-00T00:00:00Z"",""timezone"":0,""before"":0,""after"":0,""precision"":9,""calendarmodel"":""http://www.wikidata.org/entity/Q1985727""},""type"":""time""},""datatype"":""time""},""type"":""statement"",""id"":""Q70899$e6c08da5-4e14-db90-9f3e-6c7729defe23"",""rank"":""normal"",""references"":[{""hash"":""cf867b8c7a1ab0fcd404ef384615947c40145826"",""snaks"":{""P248"":[{""snaktype"":""value"",""property"":""P248"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":27893345,""id"":""Q27893345""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""}],""P304"":[{""snaktype"":""value"",""property"":""P304"",""datavalue"":{""value"":""8"",""type"":""string""},""datatype"":""string""}]},""snaks-order"":[""P248"",""P304""]}]}],""P186"":[{""mainsnak"":{""snaktype"":""value"",""property"":""P186"",""datavalue"":{""value"":{""entity-type"":""item"",""numeric-id"":42302,""id"":""Q42302""},""type"":""wikibase-entityid""},""datatype"":""wikibase-item""},""type"":""statement"",""id"":""Q70899$cba96330-472c-14c4-35ea-6933ac474de6"",""rank"":""normal""}]},""sitelinks"":{""acewiki"":{""site"":""acewiki"",""title"":""Nabi Adam"",""badges"":[],""url"":""https://ace.wikipedia.org/wiki/Nabi_Adam""},""arcwiki"":{""site"":""arcwiki"",""title"":""\u0710\u0715\u0721"",""badges"":[],""url"":""https://arc.wikipedia.org/wiki/%DC%90%DC%95%DC%A1""},""arwiki"":{""site"":""arwiki"",""title"":""\u0622\u062f\u0645"",""badges"":[],""url"":""https://ar.wikipedia.org/wiki/%D8%A2%D8%AF%D9%85""},""arzwiki"":{""site"":""arzwiki"",""title"":""\u0627\u062f\u0645"",""badges"":[],""url"":""https://arz.wikipedia.org/wiki/%D8%A7%D8%AF%D9%85""},""astwiki"":{""site"":""astwiki"",""title"":""Ad\u00e1n"",""badges"":[],""url"":""https://ast.wikipedia.org/wiki/Ad%C3%A1n""},""avwiki"":{""site"":""avwiki"",""title"":""\u0410\u0434\u0430\u043c \u0430\u0432\u0430\u0440\u0430\u0433"",""badges"":[],""url"":""https://av.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC_%D0%B0%D0%B2%D0%B0%D1%80%D0%B0%D0%B3""},""azbwiki"":{""site"":""azbwiki"",""title"":""\u0622\u062f\u0627\u0645"",""badges"":[],""url"":""https://azb.wikipedia.org/wiki/%D8%A2%D8%AF%D8%A7%D9%85""},""azwiki"":{""site"":""azwiki"",""title"":""Ad\u0259m"",""badges"":[],""url"":""https://az.wikipedia.org/wiki/Ad%C9%99m""},""bawiki"":{""site"":""bawiki"",""title"":""\u04d8\u0499\u04d9\u043c"",""badges"":[],""url"":""https://ba.wikipedia.org/wiki/%D3%98%D2%99%D3%99%D0%BC""},""be_x_oldwiki"":{""site"":""be_x_oldwiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://be-tarask.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""bewiki"":{""site"":""bewiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://be.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""bgwiki"":{""site"":""bgwiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://bg.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""bnwiki"":{""site"":""bnwiki"",""title"":""\u0986\u09a6\u09ae"",""badges"":[],""url"":""https://bn.wikipedia.org/wiki/%E0%A6%86%E0%A6%A6%E0%A6%AE""},""bowiki"":{""site"":""bowiki"",""title"":""\u0f68\u0f0b\u0f51\u0f58\u0f0d"",""badges"":[],""url"":""https://bo.wikipedia.org/wiki/%E0%BD%A8%E0%BC%8B%E0%BD%91%E0%BD%98%E0%BC%8D""},""brwiki"":{""site"":""brwiki"",""title"":""Adam"",""badges"":[],""url"":""https://br.wikipedia.org/wiki/Adam""},""bswiki"":{""site"":""bswiki"",""title"":""Adem"",""badges"":[],""url"":""https://bs.wikipedia.org/wiki/Adem""},""cawiki"":{""site"":""cawiki"",""title"":""Adam"",""badges"":[],""url"":""https://ca.wikipedia.org/wiki/Adam""},""cdowiki"":{""site"":""cdowiki"",""title"":""\u0100-d\u014fng"",""badges"":[],""url"":""https://cdo.wikipedia.org/wiki/%C4%80-d%C5%8Fng""},""ckbwiki"":{""site"":""ckbwiki"",""title"":""\u0626\u0627\u062f\u06d5\u0645"",""badges"":[],""url"":""https://ckb.wikipedia.org/wiki/%D8%A6%D8%A7%D8%AF%DB%95%D9%85""},""commonswiki"":{""site"":""commonswiki"",""title"":""Category:Adam"",""badges"":[],""url"":""https://commons.wikimedia.org/wiki/Category:Adam""},""cswiki"":{""site"":""cswiki"",""title"":""Adam"",""badges"":[],""url"":""https://cs.wikipedia.org/wiki/Adam""},""cywiki"":{""site"":""cywiki"",""title"":""Adda"",""badges"":[],""url"":""https://cy.wikipedia.org/wiki/Adda""},""dawiki"":{""site"":""dawiki"",""title"":""Adam (bibelsk person)"",""badges"":[],""url"":""https://da.wikipedia.org/wiki/Adam_(bibelsk_person)""},""dewiki"":{""site"":""dewiki"",""title"":""Adam"",""badges"":[],""url"":""https://de.wikipedia.org/wiki/Adam""},""dewikiquote"":{""site"":""dewikiquote"",""title"":""Adam"",""badges"":[],""url"":""https://de.wikiquote.org/wiki/Adam""},""diqwiki"":{""site"":""diqwiki"",""title"":""Adem"",""badges"":[],""url"":""https://diq.wikipedia.org/wiki/Adem""},""elwiki"":{""site"":""elwiki"",""title"":""\u0391\u03b4\u03ac\u03bc"",""badges"":[],""url"":""https://el.wikipedia.org/wiki/%CE%91%CE%B4%CE%AC%CE%BC""},""enwiki"":{""site"":""enwiki"",""title"":""Adam"",""badges"":[],""url"":""https://en.wikipedia.org/wiki/Adam""},""enwikiquote"":{""site"":""enwikiquote"",""title"":""Adam"",""badges"":[],""url"":""https://en.wikiquote.org/wiki/Adam""},""enwikisource"":{""site"":""enwikisource"",""title"":""Author:Adam"",""badges"":[],""url"":""https://en.wikisource.org/wiki/Author:Adam""},""eowiki"":{""site"":""eowiki"",""title"":""Adamo (Biblio)"",""badges"":[],""url"":""https://eo.wikipedia.org/wiki/Adamo_(Biblio)""},""eowikiquote"":{""site"":""eowikiquote"",""title"":""Adamo"",""badges"":[],""url"":""https://eo.wikiquote.org/wiki/Adamo""},""eswiki"":{""site"":""eswiki"",""title"":""Ad\u00e1n"",""badges"":[],""url"":""https://es.wikipedia.org/wiki/Ad%C3%A1n""},""etwiki"":{""site"":""etwiki"",""title"":""Aadam"",""badges"":[],""url"":""https://et.wikipedia.org/wiki/Aadam""},""euwiki"":{""site"":""euwiki"",""title"":""Adam"",""badges"":[],""url"":""https://eu.wikipedia.org/wiki/Adam""},""fawiki"":{""site"":""fawiki"",""title"":""\u0622\u062f\u0645"",""badges"":[],""url"":""https://fa.wikipedia.org/wiki/%D8%A2%D8%AF%D9%85""},""fiwiki"":{""site"":""fiwiki"",""title"":""Aadam"",""badges"":[],""url"":""https://fi.wikipedia.org/wiki/Aadam""},""fiwikiquote"":{""site"":""fiwikiquote"",""title"":""Aatami"",""badges"":[],""url"":""https://fi.wikiquote.org/wiki/Aatami""},""fowiki"":{""site"":""fowiki"",""title"":""\u00c1dam"",""badges"":[],""url"":""https://fo.wikipedia.org/wiki/%C3%81dam""},""frwiki"":{""site"":""frwiki"",""title"":""Adam"",""badges"":[],""url"":""https://fr.wikipedia.org/wiki/Adam""},""glwiki"":{""site"":""glwiki"",""title"":""Ad\u00e1n"",""badges"":[],""url"":""https://gl.wikipedia.org/wiki/Ad%C3%A1n""},""hewiki"":{""site"":""hewiki"",""title"":""\u05d0\u05d3\u05dd (\u05d3\u05de\u05d5\u05ea \u05de\u05e7\u05e8\u05d0\u05d9\u05ea)"",""badges"":[],""url"":""https://he.wikipedia.org/wiki/%D7%90%D7%93%D7%9D_(%D7%93%D7%9E%D7%95%D7%AA_%D7%9E%D7%A7%D7%A8%D7%90%D7%99%D7%AA)""},""hrwiki"":{""site"":""hrwiki"",""title"":""Adam"",""badges"":[],""url"":""https://hr.wikipedia.org/wiki/Adam""},""hywiki"":{""site"":""hywiki"",""title"":""\u0531\u0564\u0561\u0574"",""badges"":[],""url"":""https://hy.wikipedia.org/wiki/%D4%B1%D5%A4%D5%A1%D5%B4""},""idwiki"":{""site"":""idwiki"",""title"":""Adam"",""badges"":[],""url"":""https://id.wikipedia.org/wiki/Adam""},""itwiki"":{""site"":""itwiki"",""title"":""Adamo"",""badges"":[],""url"":""https://it.wikipedia.org/wiki/Adamo""},""itwikiquote"":{""site"":""itwikiquote"",""title"":""Adamo"",""badges"":[],""url"":""https://it.wikiquote.org/wiki/Adamo""},""jawiki"":{""site"":""jawiki"",""title"":""\u30a2\u30c0\u30e0"",""badges"":[],""url"":""https://ja.wikipedia.org/wiki/%E3%82%A2%E3%83%80%E3%83%A0""},""jvwiki"":{""site"":""jvwiki"",""title"":""Adam"",""badges"":[],""url"":""https://jv.wikipedia.org/wiki/Adam""},""kabwiki"":{""site"":""kabwiki"",""title"":""Adam"",""badges"":[],""url"":""https://kab.wikipedia.org/wiki/Adam""},""kawiki"":{""site"":""kawiki"",""title"":""\u10d0\u10d3\u10d0\u10db\u10d8"",""badges"":[],""url"":""https://ka.wikipedia.org/wiki/%E1%83%90%E1%83%93%E1%83%90%E1%83%9B%E1%83%98""},""kkwiki"":{""site"":""kkwiki"",""title"":""\u0410\u0434\u0430\u043c \u0430\u0442\u0430"",""badges"":[],""url"":""https://kk.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC_%D0%B0%D1%82%D0%B0""},""kowiki"":{""site"":""kowiki"",""title"":""\uc544\ub2f4"",""badges"":[],""url"":""https://ko.wikipedia.org/wiki/%EC%95%84%EB%8B%B4""},""kuwiki"":{""site"":""kuwiki"",""title"":""Adem"",""badges"":[],""url"":""https://ku.wikipedia.org/wiki/Adem""},""kywiki"":{""site"":""kywiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://ky.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""lawiki"":{""site"":""lawiki"",""title"":""Adam"",""badges"":[],""url"":""https://la.wikipedia.org/wiki/Adam""},""lbewiki"":{""site"":""lbewiki"",""title"":""\u0410\u0434\u0430\u043c \u0438\u0434\u0430\u0432\u0441"",""badges"":[],""url"":""https://lbe.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC_%D0%B8%D0%B4%D0%B0%D0%B2%D1%81""},""lvwiki"":{""site"":""lvwiki"",""title"":""\u0100dams"",""badges"":[],""url"":""https://lv.wikipedia.org/wiki/%C4%80dams""},""mgwiki"":{""site"":""mgwiki"",""title"":""Adam"",""badges"":[],""url"":""https://mg.wikipedia.org/wiki/Adam""},""mlwiki"":{""site"":""mlwiki"",""title"":""\u0d06\u0d26\u0d3e\u0d02"",""badges"":[],""url"":""https://ml.wikipedia.org/wiki/%E0%B4%86%E0%B4%A6%E0%B4%BE%E0%B4%82""},""mnwiki"":{""site"":""mnwiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://mn.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""nlwiki"":{""site"":""nlwiki"",""title"":""Adam"",""badges"":[],""url"":""https://nl.wikipedia.org/wiki/Adam""},""ocwiki"":{""site"":""ocwiki"",""title"":""Adam"",""badges"":[],""url"":""https://oc.wikipedia.org/wiki/Adam""},""pawiki"":{""site"":""pawiki"",""title"":""\u0a06\u0a26\u0a2e"",""badges"":[],""url"":""https://pa.wikipedia.org/wiki/%E0%A8%86%E0%A8%A6%E0%A8%AE""},""plwikiquote"":{""site"":""plwikiquote"",""title"":""Adam"",""badges"":[],""url"":""https://pl.wikiquote.org/wiki/Adam""},""pswiki"":{""site"":""pswiki"",""title"":""\u0622\u062f\u0645"",""badges"":[],""url"":""https://ps.wikipedia.org/wiki/%D8%A2%D8%AF%D9%85""},""rowiki"":{""site"":""rowiki"",""title"":""Adam"",""badges"":[],""url"":""https://ro.wikipedia.org/wiki/Adam""},""ruwiki"":{""site"":""ruwiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[""Q17437796""],""url"":""https://ru.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""ruwikiquote"":{""site"":""ruwikiquote"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://ru.wikiquote.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""rwwiki"":{""site"":""rwwiki"",""title"":""Adamu"",""badges"":[],""url"":""https://rw.wikipedia.org/wiki/Adamu""},""scnwiki"":{""site"":""scnwiki"",""title"":""Addamu (primu omu)"",""badges"":[],""url"":""https://scn.wikipedia.org/wiki/Addamu_(primu_omu)""},""scowiki"":{""site"":""scowiki"",""title"":""Adam"",""badges"":[],""url"":""https://sco.wikipedia.org/wiki/Adam""},""sdwiki"":{""site"":""sdwiki"",""title"":""\u0622\u062f\u0645 \u0639\u0644\u064a\u06c1 \u0627\u0644\u0633\u0644\u0627\u0645"",""badges"":[],""url"":""https://sd.wikipedia.org/wiki/%D8%A2%D8%AF%D9%85_%D8%B9%D9%84%D9%8A%DB%81_%D8%A7%D9%84%D8%B3%D9%84%D8%A7%D9%85""},""shwiki"":{""site"":""shwiki"",""title"":""Adam"",""badges"":[],""url"":""https://sh.wikipedia.org/wiki/Adam""},""sowiki"":{""site"":""sowiki"",""title"":""Nebi Aadam C.S"",""badges"":[],""url"":""https://so.wikipedia.org/wiki/Nebi_Aadam_C.S""},""sqwiki"":{""site"":""sqwiki"",""title"":""Adami"",""badges"":[],""url"":""https://sq.wikipedia.org/wiki/Adami""},""srwiki"":{""site"":""srwiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://sr.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""swwiki"":{""site"":""swwiki"",""title"":""Adamu"",""badges"":[],""url"":""https://sw.wikipedia.org/wiki/Adamu""},""tawiki"":{""site"":""tawiki"",""title"":""\u0b86\u0ba4\u0bbe\u0bae\u0bcd"",""badges"":[],""url"":""https://ta.wikipedia.org/wiki/%E0%AE%86%E0%AE%A4%E0%AE%BE%E0%AE%AE%E0%AF%8D""},""tewiki"":{""site"":""tewiki"",""title"":""\u0c06\u0c26\u0c3e\u0c2e\u0c41"",""badges"":[],""url"":""https://te.wikipedia.org/wiki/%E0%B0%86%E0%B0%A6%E0%B0%BE%E0%B0%AE%E0%B1%81""},""tgwiki"":{""site"":""tgwiki"",""title"":""\u041e\u0434\u0430\u043c"",""badges"":[],""url"":""https://tg.wikipedia.org/wiki/%D0%9E%D0%B4%D0%B0%D0%BC""},""thwiki"":{""site"":""thwiki"",""title"":""\u0e2d\u0e32\u0e14\u0e31\u0e21"",""badges"":[],""url"":""https://th.wikipedia.org/wiki/%E0%B8%AD%E0%B8%B2%E0%B8%94%E0%B8%B1%E0%B8%A1""},""tlwiki"":{""site"":""tlwiki"",""title"":""Adan"",""badges"":[],""url"":""https://tl.wikipedia.org/wiki/Adan""},""trwiki"":{""site"":""trwiki"",""title"":""\u00c2dem"",""badges"":[],""url"":""https://tr.wikipedia.org/wiki/%C3%82dem""},""tywiki"":{""site"":""tywiki"",""title"":""Adamu"",""badges"":[],""url"":""https://ty.wikipedia.org/wiki/Adamu""},""ugwiki"":{""site"":""ugwiki"",""title"":""\u0626\u0627\u062f\u06d5\u0645 \u0626\u06d5\u0644\u06d5\u064a\u06be\u0649\u0633\u0633\u0627\u0644\u0627\u0645"",""badges"":[],""url"":""https://ug.wikipedia.org/wiki/%D8%A6%D8%A7%D8%AF%DB%95%D9%85_%D8%A6%DB%95%D9%84%DB%95%D9%8A%DA%BE%D9%89%D8%B3%D8%B3%D8%A7%D9%84%D8%A7%D9%85""},""ukwiki"":{""site"":""ukwiki"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://uk.wikipedia.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""ukwikiquote"":{""site"":""ukwikiquote"",""title"":""\u0410\u0434\u0430\u043c"",""badges"":[],""url"":""https://uk.wikiquote.org/wiki/%D0%90%D0%B4%D0%B0%D0%BC""},""uzwiki"":{""site"":""uzwiki"",""title"":""Odamato"",""badges"":[],""url"":""https://uz.wikipedia.org/wiki/Odamato""},""vepwiki"":{""site"":""vepwiki"",""title"":""Adam"",""badges"":[],""url"":""https://vep.wikipedia.org/wiki/Adam""},""viwiki"":{""site"":""viwiki"",""title"":""Adam"",""badges"":[],""url"":""https://vi.wikipedia.org/wiki/Adam""},""warwiki"":{""site"":""warwiki"",""title"":""Adan"",""badges"":[],""url"":""https://war.wikipedia.org/wiki/Adan""},""wawiki"":{""site"":""wawiki"",""title"":""Adan"",""badges"":[],""url"":""https://wa.wikipedia.org/wiki/Adan""},""wowiki"":{""site"":""wowiki"",""title"":""Aadama"",""badges"":[],""url"":""https://wo.wikipedia.org/wiki/Aadama""},""yiwiki"":{""site"":""yiwiki"",""title"":""\u05d0\u05d3\u05dd \u05d4\u05e8\u05d0\u05e9\u05d5\u05df"",""badges"":[],""url"":""https://yi.wikipedia.org/wiki/%D7%90%D7%93%D7%9D_%D7%94%D7%A8%D7%90%D7%A9%D7%95%D7%9F""},""zh_min_nanwiki"":{""site"":""zh_min_nanwiki"",""title"":""A-tong"",""badges"":[],""url"":""https://zh-min-nan.wikipedia.org/wiki/A-tong""},""zh_yuewiki"":{""site"":""zh_yuewiki"",""title"":""\u4e9e\u7576"",""badges"":[],""url"":""https://zh-yue.wikipedia.org/wiki/%E4%BA%9E%E7%95%B6""},""zhwiki"":{""site"":""zhwiki"",""title"":""\u4e9e\u7576"",""badges"":[],""url"":""https://zh.wikipedia.org/wiki/%E4%BA%9E%E7%95%B6""}}}}}
    ";
            var result = WikidataParser.ParseWikidataItem(json);
            Assert.AreEqual(70899, result.ItemId);

            Assert.AreEqual("normal", result.Claims.P569.A.Rank);
            Assert.AreEqual("time", result.Claims.P569.A.Mainsnak.Datatype);
            Assert.AreEqual("-3760-00-00T00:00:00Z", result.Claims.Born.Time);
            Assert.AreEqual(19014, result.Claims.BirthPlace[0]);

            Assert.IsNull(result.Claims.Father);
            Assert.IsNull(result.Claims.Mother);
            Assert.AreEqual(830183, result.Claims.Spouses[0]);
            Assert.AreEqual(205365, result.Claims.Kids[0]);

            var item1 = (ValueItem)result.Claims.P21.A.Mainsnak.ValueTyped;
            Assert.AreEqual("item", item1.EntityType);
            Assert.IsTrue(result.Claims.IsMale);
            Assert.IsNull(result.Claims.P22.A.Mainsnak.ValueTyped);
            Assert.IsNull(result.Claims.P25.A.Mainsnak.ValueTyped);

            Assert.AreEqual("God2-Sistine Chapel.png", result.Claims.P18.A.Mainsnak.ValueTyped);

            var time1 = (ValueTime)result.Claims.P569.A.Mainsnak.ValueTyped;
            Assert.AreEqual(9, time1.Precision);

            var snaks = result.Claims.P569.A.References.A.SnaksList;
            Assert.AreEqual(2, snaks.Count);
            Assert.AreEqual(27893345, ((ValueItem)snaks[0].ValueTyped).NumericId);

            Assert.AreEqual(7, result.Sitelinks.Translations.Count);
        }

        [Test]
        public void ParseWikidataItem_NoTrans()
        {
            var json = @"
        {""entities"":{""Q6182499"":{""pageid"":5979175,""ns"":0,""title"":""Q6182499"",""lastrevid"":9428423,""modified"":""2013-03-04T21:02:09Z"",""type"":""item"",""id"":""Q6182499"",""labels"":{""en"":{""language"":""en"",""value"":""Jeroham""}},""descriptions"":[],""aliases"":[],""claims"":[],""sitelinks"":{""enwiki"":{""site"":""enwiki"",""title"":""Jeroham"",""badges"":[],""url"":""https://en.wikipedia.org/wiki/Jeroham""}
}}}}";
            var result = WikidataParser.ParseWikidataItem(json);
            Assert.AreEqual(1, result.Sitelinks.Translations.Count);
        }

        [Test]
        public void ParseWikidataItem_Qualifiers()
        {
            var json = @"
{
  ""entities"": {
    ""Q1742282"": {
      ""id"": ""Q1742282"",
      ""claims"": {
        ""P625"": [
          {
            ""mainsnak"": {
              ""snaktype"": ""value"",
              ""property"": ""P625"",
              ""datavalue"": {
                ""value"": {
                  ""latitude"": 47.638791,
                  ""longitude"": 35.263996,
                  ""altitude"": null,
                  ""precision"": 0.00000277777777778,
                  ""globe"": ""http://www.wikidata.org/entity/Q2""
                },
                ""type"": ""globecoordinate""
              },
              ""datatype"": ""globe-coordinate""
            },
            ""type"": ""statement"",
            ""id"": ""Q1742282$E70873D4-E5A0-4663-BE5E-A89B3998A748"",
            ""rank"": ""normal"",
            ""references"": [
              {
                ""hash"": ""9a24f7c0208b05d6be97077d855671d1dfdbc0dd"",
                ""snaks"": {
                  ""P143"": [
                    {
                      ""snaktype"": ""value"",
                      ""property"": ""P143"",
                      ""datavalue"": {
                        ""value"": {
                          ""entity-type"": ""item"",
                          ""numeric-id"": 48183,
                          ""id"": ""Q48183""
                        },
                        ""type"": ""wikibase-entityid""
                      },
                      ""datatype"": ""wikibase-item""
                    }
                  ]
                },
                ""snaks-order"": [
                  ""P143""
                ]
              }
            ]
          },
          {
            ""mainsnak"": {
              ""snaktype"": ""value"",
              ""property"": ""P625"",
              ""datavalue"": {
                ""value"": {
                  ""latitude"": 47.33,
                  ""longitude"": 36.4461,
                  ""altitude"": null,
                  ""precision"": 0.0001,
                  ""globe"": ""http://www.wikidata.org/entity/Q2""
                },
                ""type"": ""globecoordinate""
              },
              ""datatype"": ""globe-coordinate""
            },
            ""type"": ""statement"",
            ""qualifiers"": {
              ""P518"": [
                {
                  ""snaktype"": ""value"",
                  ""property"": ""P518"",
                  ""hash"": ""2c7de14bc888b5d4d007acc4940641fc46b79ba0"",
                  ""datavalue"": {
                    ""value"": {
                      ""entity-type"": ""item"",
                      ""numeric-id"": 7376362,
                      ""id"": ""Q7376362"",
                      ""type"": ""wikibase-entityid""
                    }
                  },
                  ""datatype"": ""wikibase-item""
                }
              ]
            },
            ""qualifiers-order"": [
              ""P518""
            ],
            ""id"": ""Q1742282$667F97F7-469B-41A2-A4DC-E83C2EA74AD2"",
            ""rank"": ""normal"",
            ""references"": [
              {
                ""hash"": ""288ab581e7d2d02995a26dfa8b091d96e78457fc"",
                ""snaks"": {
                  ""P143"": [
                    {
                      ""snaktype"": ""value"",
                      ""property"": ""P143"",
                      ""datavalue"": {
                        ""value"": {
                          ""entity-type"": ""item"",
                          ""numeric-id"": 206855,
                          ""id"": ""Q206855""
                        },
                        ""type"": ""wikibase-entityid""
                      },
                      ""datatype"": ""wikibase-item""
                    }
                  ]
                },
                ""snaks-order"": [
                  ""P143""
                ]
              }
            ]
          },
          {
            ""mainsnak"": {
              ""snaktype"": ""value"",
              ""property"": ""P625"",
              ""datavalue"": {
                ""value"": {
                  ""latitude"": 47.6308,
                  ""longitude"": 35.2669,
                  ""altitude"": null,
                  ""precision"": 0.0001,
                  ""globe"": ""http://www.wikidata.org/entity/Q2""
                },
                ""type"": ""globecoordinate""
              },
              ""datatype"": ""globe-coordinate""
            },
            ""type"": ""statement"",
            ""qualifiers"": {
              ""P518"": [
                {
                  ""snaktype"": ""value"",
                  ""property"": ""P518"",
                  ""hash"": ""544b353c2ef8ad17b20e5e38254f37c3b3858ebf"",
                  ""datavalue"": {
                    ""value"": {
                      ""entity-type"": ""item"",
                      ""numeric-id"": 1233637,
                      ""id"": ""Q1233637""
                    },
                    ""type"": ""wikibase-entityid""
                  },
                  ""datatype"": ""wikibase-item""
                }
              ]
            },
            ""qualifiers-order"": [
              ""P518""
            ],
            ""id"": ""Q1742282$F99E6235-E4E3-4429-A53C-5BC9DE640AF6"",
            ""rank"": ""normal"",
            ""references"": [
              {
                ""hash"": ""288ab581e7d2d02995a26dfa8b091d96e78457fc"",
                ""snaks"": {
                  ""P143"": [
                    {
                      ""snaktype"": ""value"",
                      ""property"": ""P143"",
                      ""datavalue"": {
                        ""value"": {
                          ""entity-type"": ""item"",
                          ""numeric-id"": 206855,
                          ""id"": ""Q206855""
                        },
                        ""type"": ""wikibase-entityid""
                      },
                      ""datatype"": ""wikibase-item""
                    }
                  ]
                },
                ""snaks-order"": [
                  ""P143""
                ]
              }
            ]
          }
        ]
      }
    }
  }
}";
            var result = WikidataParser.ParseWikidataItem(json);
            Assert.AreEqual(47.638791, result.Claims.Coor.Latitude);
            Assert.AreEqual(47.33, result.Claims.CoorRiverSource.Latitude);
            Assert.AreEqual(47.6308, result.Claims.CoorRiverMouth.Latitude);
        }
    }
}

#endif
