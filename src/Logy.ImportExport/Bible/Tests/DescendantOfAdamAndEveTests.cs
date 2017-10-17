#if DEBUG
using System.Collections.Generic;
using Logy.Entities.Import;
using Logy.Entities.Products;
using Logy.MwAgent.DotNetWikiBot;

using NUnit.Framework;

namespace Logy.ImportExport.Bible.Tests
{
    [TestFixture]
    public class DescendantOfAdamAndEveTests
    {
        private readonly List<DescendantOfAdamAndEve> _descendants = Descendant
            .ParseDescendants<DescendantOfAdamAndEve>(new Page(new Site(LogyEventsDb.LogyBaseUrl)) { Text = S1708 }, true);

        private const string S1708 = @"
Genealogy from Adam to Zerubbabel:  

* Cain is not listed as a descendant in the genealogy of Adam and neither are any of his children.The Bible records a family tree for Cain(Genesis 4:17-4:22) and a separate genealogy for Adam(Genesis 5:1-5:32) which is described as Mankind(Genesis 5:2). These fathered seed lines are separate up to Genesis 6:4 for good reason and should remain separate if one is to rightly divide the word of God.**  
= Descendants =
.1. [[wikipedia:Adam|Adam]]<ref>Genesis</ref><br>
.+ m. [[wikipedia:Eve|Eve]]<ref>Genesis 3:20</ref><br>
..2. [[wikipedia:Cain|Cain]]<ref>Genesis 4:1</ref><br>
...3. [[wikipedia:Enoch (son of Cain)|Enoch]]<ref>Genesis 4:17</ref><br>
....4. [[wikipedia:Irad|Irad]]<ref name=""Ge 4:18"">Genesis 4:18</ref><br>
.....5. [[wikipedia:Mehujael|Mehujael]]<ref name=""Ge 4:18""/><br>
......6. [[wikipedia:Methushael|Methushael]]<ref name=""Ge 4:18""/><br>
.......7. [[wikipedia:Lamech (descendant of Cain)|Lamech]]<ref name=""Ge 4:18""/><br>
.......+ m. [[wikipedia:List of minor biblical figures#Adah|Adah]]<ref name=""Ge 4:19"">Genesis 4:19</ref><br>
........8. [[wikipedia:Jabal (Bible)|Jabal]]<ref>Genesis 4:20</ref><br>
........8. [[wikipedia:Jubal (Bible)|Jubal]]<ref>Genesis 4:21</ref><br>
.......+ m. [[wikipedia:List of minor biblical figures,_L–Z#Zillah|Zillah]]<ref name=""Ge 4:19""/><br>
........8. [[wikipedia:Tubal-Cain|Tubal-Cain]]<ref name=""Ge 4:22"">Genesis 4:22</ref><br>
........8. [[wikipedia:Naamah (Genesis)|Naamah]]<ref name=""Ge 4:22""/><br>
..2. [[wikipedia:Abel|Abel]]<ref>Genesis 4:2</ref><br>
..2. [[wikipedia:Seth|Seth]]<ref>Genesis 4:25</ref><br>
...3. [[wikipedia:Enos (biblical figure)|Enos]]<ref>Genesis 4:26</ref><br>
....4. [[wikipedia:Kenan|Kenan]]<ref>Genesis 5:9</ref><br>
.....5. [[wikipedia:Mahalalel|Mahalalel]]<ref>Genesis 5:12</ref><br>
......6. [[wikipedia:Jared (biblical figure)|Jared]]<ref>Genesis 5:15</ref><br>
.......7. [[wikipedia:Enoch (ancestor of Noah)|Enoch]]<ref>Genesis 5:18</ref><br>
........8. [[wikipedia:Methuselah|Methuselah]]<ref>Genesis 5:21</ref><br>
.........9. [[wikipedia:Lamech (father of Noah)|Lamech]]<ref>Genesis 5:25</ref><br>
..........10. [[wikipedia:Noah|Noah]]<ref>Genesis 5:29</ref><br>
...........11. [[wikipedia:Shem|Shem]]<ref name=""Ge 5:32"">Genesis 5:32</ref><br>
............12. [[wikipedia:Elam|Elam]]<ref name=""Ge 10:22"">Genesis 10:22</ref><br>
............12. [[wikipedia:Asshur|Asshur]]<ref name=""Ge 10:22""/><br>
............12. [[wikipedia:Arpachshad|Arpachshad]]<ref name=""Ge 10:22""/><br>
.............13. [[wikipedia:Salah (biblical figure)|Shelah]]<ref name=""Ge 10:24"">Genesis 10:24</ref><br>
..............14. [[wikipedia:Eber|Eber]]<ref name=""Ge 10:24""/><br>
...............15. [[wikipedia:Peleg|Peleg]]<ref name=""Ge 10:25"">Genesis 10:25</ref><br>
................16. [[wikipedia:Reu|Reu]]<ref name=""Ge 11:18"">Genesis 11:18</ref><br>
.................17. [[wikipedia:Serug|Serug]]<ref name=""Ge 11:20"">Genesis 11:20</ref><br>
..................18. [[Nahor, son of Serug|Nahor]]<ref name=""Ge 11:22"">Genesis 11:22</ref><br>
...................19. [[wikipedia:Terah|Terah]]<ref name=""Ge 11:24"">Genesis 11:24</ref><br>
....................20. [[wikipedia:Abraham|Abraham]]<ref name=""Ge 11:26"">Genesis 11:26</ref><br>
....................+ m. [[wikipedia:Sarah|Sarah]]<ref name=""Ge 11:29"">Genesis 11:29</ref><br>
.....................21. [[wikipedia:Isaac|Isaac]]<ref name=""Ge 21:3"">Genesis 21:3</ref><br>
.....................+ m. [[wikipedia:Rebekah|Rebekah]]<ref name=""Ge 22:23"">Genesis 22:23</ref><br>
......................22. [[wikipedia:Esau|Esau]]<ref name=""Ge 25:25"">Genesis 25:25</ref><br>
......................+ m. [[wikipedia:Judith|Judith]]<ref name=""Ge 26:34"">Genesis 26:34</ref><br>
......................+ m. [[wikipedia:Basemath|Basemath]]<ref name=""Ge 26:34""/><br>
......................+ m. [[wikipedia:List of minor biblical figures#Adah|Adah]]<ref name=""Ge 36:2"">Genesis 36:2</ref><br>
.......................23. [[wikipedia:Eliphaz|Eliphaz]]<ref name=""Ge 36:4"">Genesis 36:4</ref><br>
........................24. Teman<ref name=""Ge 36:11""> Genesis 36:11</ref><br>
........................24. [[wikipedia:Omar (Bible)|Omar]]<ref name=""Ge 36:11""/><br>
........................24. [[wikipedia:Zepho|Zepho]]<ref name=""Ge 36:11""/><br>
........................24. [[wikipedia:Gatam|Gatam]]<ref name=""Ge 36:11""/><br>
........................24. [[wikipedia:Kenaz|Kenaz]]<ref name=""Ge 36:11""/><br>
.......................+ m. [[wikipedia:Timna|Timna]]<ref name=""Ge 36:12"">Genesis 36:12</ref><br>
........................24. [[wikipedia:Amalek|Amalek]]<ref name=""Ge 36:12""/><br>
......................+ m. [[wikipedia:Oholibamah|Oholibamah]]<ref name=""Ge 36:2""/><br>
.......................23. [[wikipedia:Jeush|Jeush]]<ref name=""Ge 36:5"">Genesis 36:5</ref><br>
.......................23. [[wikipedia:Jalam|Jalam]]<ref name=""Ge 36:5""/><br>
.......................23. [[wikipedia:Korah|Korah]]<ref name=""Ge 36:5""/><br>
......................+ m. [[wikipedia:Basemath|Basemath]]<ref name=""Ge 36:3"">Genesis 36:3</ref><br>
.......................23. [[wikipedia:Reuel|Reuel]] <ref name=""Ge 36:4""/><br>
........................24. [[wikipedia:Nahath|Nahath]]<ref name=""Ge 36:13"">Genesis 36:13</ref><br>
........................24. [[wikipedia:Zerah|Zerah]]<ref name=""Ge 36:13""/><br>
.........................25. [[wikipedia:Jobab|Jobab]], King of Edom<ref name=""Ge 36:33""> Genesis 36:33</ref><br>
........................24. [[wikipedia:Shammah|Shammah]]<ref name=""Ge 36:13""/><br>
........................24. [[wikipedia:Mizzah|Mizzah]]<ref name=""Ge 36:13""/><br>
......................22. [[wikipedia:Jacob|Jacob]]<ref name=""Ge 25:26"">Genesis 25:26</ref><br>
......................+ m. [[wikipedia:Leah|Leah]]<ref name=""Ge 29:16"">Genesis 29:16</ref><br>
.......................23. [[wikipedia:Reuben (Bible)|Reuben]]{{ref|Reuben|2}}<ref name=""Ge 29:32"">Genesis 29:32</ref><br>
.........................24. [[wikipedia:Hanoch (Bible)|Hanoch]]<ref name=""Ge 46:9"">Genesis 46:9</ref><br>
.........................24. [[wikipedia:Pallu (Biblical figure)|Pallu]]<ref name=""Ge 46:9""/><br>
..........................25. [[wikipedia:Eliab|Eliab]]<ref name=""Nu 26:8"">Numbers 26:8</ref><br>
...........................26. [[wikipedia:Dathan|Dathan]]<ref name=""Nu 16:1"">Numbers 16:1</ref><br>
...........................26. [[wikipedia:Abiram|Abiram]]<ref name=""Nu 16:1""/><br>
...........................26. [[wikipedia:Nemuel|Nemuel]]<ref name=""Nu 26:9"">Numbers 26:9</ref><br>
.........................24. [[wikipedia:Hezron|Hezron]]<ref name=""Ge 46:9""/><br>
.........................24. [[wikipedia:List of minor biblical figures#Carmi|Carmi]]<ref name=""Ge 46:9""/><br>
.........................24. [[wikipedia:Bohan|Bohan]]<ref name=""Jos 15:6"">Joshua 15:6</ref><br>
.......................23. [[wikipedia:Simeon (Hebrew Bible)|Simeon]]{{ref|Simeon|3}}<ref name=""Ge 29:33"">Genesis 29:33</ref><br>
.........................24. [[wikipedia:Jemuel|Jemuel]]<ref name=""Ge 46:10"">Genesis 46:10</ref><br>
.........................24. [[wikipedia:List of minor Biblical figures#Jamin|Jamin]]<ref name=""Ge 46:10""/><br>
.........................24. [[wikipedia:Ohad|Ohad]]<ref name=""Ge 46:10""/><br>
.........................24. Jakin<ref name=""Ge 46:10""/><br>
.........................24. [[wikipedia:Zohar|Zohar]]<ref name=""Ge 46:10""/><br>
.........................24. [[wikipedia:Shaul|Shaul]]<ref name=""Ge 46:10""/>
.......................23. [[wikipedia:Levi|Levi]]{{ref|Levi|4}}<ref name=""Ge 29:34"">Genesis 29:34</ref><br>
.........................24. [[wikipedia:Gershon|Gershon]]<ref name=""Ge 46:11"">Genesis 46:11</ref><br>
..........................25. [[wikipedia:Libni|Libni]]<ref name=""Ex 6:17"">Exodus 6:17</ref><br>
...........................26. [[wikipedia:Jehath|Jehath]]<ref name=""1Ch 6:20"">1 Chronicles 6:20</ref><br>
............................27. [[wikipedia:Zimmah|Zimmah]]<ref name=""1Ch 6:20""/><br>
.............................28. [[wikipedia:Joah|Joah]]<ref name=""1Ch 6:21"">1 Chronicles 6:21</ref><br>
..............................29. [[wikipedia:Iddo (prophet)|Iddo]]<ref name=""1Ch 6:21""/><br>
...............................30. [[wikipedia:Zerah|Zerah]]<ref name=""1Ch 6:21""/><br>
................................31. [[wikipedia:Jeatherai|Jeatherai]]<ref name=""1Ch 6:21""/><br>
..........................25. [[wikipedia:Shimei|Shimei]]<ref name=""Ex 6:17""/><br>
..........................25. [[wikipedia:Jahath|Jahath]]<ref name=""1Ch 6:43"">1 Chronicles 6:43</ref><br>
...........................26. [[wikipedia:Shimei|Shimei]]<ref name=""1Ch 6:42"">1 Chronicles 6:42</ref><br>
............................27. [[wikipedia:Zimmah|Zimmah]]<ref name=""1Ch 6:42""/><br>
.............................28. Ethan<ref name=""1Ch 6:42""/><br>
..............................29. [[wikipedia:Adaiah|Adaiah]]<ref name=""1Ch 6:41"">1 Chronicles 6:41</ref><br>
...............................30. [[wikipedia:Zerah|Zerah]]<ref name=""1Ch 6:41""/><br>
................................31. [[wikipedia:Ethni|Ethni]]<ref name=""1Ch 6:41""/><br>
.................................32. [[wikipedia:Malkijah|Malkijah]]<ref name=""1Ch 6:40"">1 Chronicles 6:40</ref><br>
..................................33. [[wikipedia:Baaseiah|Baaseiah]]<ref name=""1Ch 6:40""/><br>
...................................34. [[wikipedia:Michael|Michael]]<ref name=""1Ch 6:40""/><br>
....................................35. [[wikipedia:Shimea|Shimea]]<ref name=""1Ch 6:39"">1 Chronicles 6:39</ref><br>
.....................................36. [[wikipedia:Berekiah|Berekiah]]<ref name=""1Ch 6:39""/><br>
......................................37. [[wikipedia:Asaph (Bible)|Asaph]]<ref name=""1Ch 6:39""/><br>
.......................................38. [[wikipedia:Zikri|Zikri]]<ref name=""1Ch 9:15"">1 Chronicles 9:15</ref><br>
........................................39. [[wikipedia:Mika (Bible)|Mika]]<ref name=""1Ch 9:15""/><br>
.........................................40. [[wikipedia:Mattaniah|Mattaniah]]<ref name=""1Ch 9:15""/><br>
.........................24. [[wikipedia:Kohath|Kohath]]<ref name=""Ge 46:11""/><br>
..........................25. [[wikipedia:Amram|Amram]]<ref name=""Ex 6:18"">Exodus 6:18</ref><br>
..........................+ m. [[wikipedia:Jochebed|Jochebed]]<ref name=""Ex 6:20"">Exodus 6:20</ref><br>
...........................26. [[wikipedia:Aaron|Aaron]]<ref name=""Ex 6:20"" /><br>
...........................+ m. [[wikipedia:Elisheba|Elisheba]]<ref name=""Ex 6:23"">Exodus 6:23</ref><br>
............................27. [[wikipedia:Nadab and Abihu|Nadab]]<ref name=""Ex 6:23""/><br>
............................27. [[wikipedia:Nadab and Abihu|Abihu]]<ref name=""Ex 6:23""/><br>
............................27. [[wikipedia:Eleazar|Eleazar]]<ref name=""Ex 6:23""/><br>
.............................28. [[wikipedia:Phinehas|Phinehas]]<ref name=""Ex 6:25"">Exodus 6:25</ref><br>
..............................29. [[wikipedia:Abishua|Abishua]]<ref name=""1Ch 6:4"">1 Chronicles 6:4</ref><br>
...............................30. [[wikipedia:Bukki|Bukki]]<ref name=""1Ch 6:5"">1 Chronicles 6:5</ref><br>
................................31. [[wikipedia:Uzzi|Uzzi]]<ref name=""1Ch 6:5""/><br>
.................................32. [[wikipedia:Zerahiah|Zerahiah]]<ref name=""1Ch 6:6"">1 Chronicles 6:6</ref><br>
..................................33. [[wikipedia:Meraioth|Meraioth]]<ref name=""1Ch 6:6""/><br>
...................................34. [[wikipedia:Amariah|Amariah]]<ref name=""1Ch 6:7"">1 Chronicles 6:7</ref><br>
....................................35. [[wikipedia:Ahitub|Ahitub]]<ref name=""1Ch 6:7""/><br>
.....................................36. [[wikipedia:Zadok|Zadok]]<ref name=""1Ch 6:8"">1 Chronicles 6:8</ref><br>
......................................37. [[wikipedia:Ahimaaz|Ahimaaz]]<ref name=""1Ch 6:8""/><br>
.......................................38. Azariah<ref name=""1Ch 6:9"">1 Chronicles 6:9</ref><br>
........................................39. [[wikipedia:Johanan|Johanan]]<ref name=""1Ch 6:9""/><br>
.........................................40. Azariah<ref name=""1Ch 6:10"">1 Chronicles 6:10</ref><br>
..........................................41. [[wikipedia:Amariah|Amariah]]<ref name=""1Ch 6:11"">1 Chronicles 6:11</ref><br>
...........................................42. [[wikipedia:Ahitub|Ahitub]]<ref name=""1Ch 6:11""/><br>
............................................43. [[wikipedia:Zadok|Zadok]]<ref name=""1Ch 6:12"">1 Chronicles 6:12</ref><br>
.............................................44. [[wikipedia:Shallum|Shallum]]<ref name=""1Ch 6:12""/><br>
..............................................45. [[wikipedia:Hilkiah|Hilkiah]]<ref name=""1Ch 6:13"">1 Chronicles 6:13</ref><br>
...............................................46. Azariah<ref name=""1Ch 6:13""/><br>
................................................47. [[wikipedia:Seraiah|Seraiah]]<ref name=""1Ch 6:14"">1 Chronicles 6:14</ref><br>
.................................................48. [[wikipedia:Jehozadak|Jehozadak]]<ref name=""1Ch 6:14""/><br>
............................27. [[wikipedia:Ithamar|Ithamar]]<ref name=""Ex 6:23""/><br>
...........................26. [[wikipedia:Moses|Moses]]<ref name=""Ex 6:20"" /><br>
...........................+ m. [[wikipedia:Zipporah|Zipporah]]<ref name=""Ex 2:21"">Exodus 2:21</ref><br>
............................27. [[wikipedia:Gershom|Gershom]]<ref name=""Ex 2:22"">Exodus 2:22</ref><br>
.............................28. [[wikipedia:Jonathan (Judges)|Jonathan]]<ref name=""Judges 18:30"">Judges 18:30</ref><br>
............................27. [[wikipedia:Eliezer|Eliezer]]<ref name=""Ex 18:4"">Exodus 18:4</ref><br>
...........................26. [[wikipedia:Miriam|Miriam]]<ref name=""Ex 15:20"">Exodus 15:20</ref><br>
..........................25. [[wikipedia:Izhar|Izhar]]<ref name=""Ex 6:18""/><br>
...........................26. [[wikipedia:Korah|Korah]]<ref name=""Ex 6:21"">Exodus 6:21</ref><br>
............................27. [[wikipedia:List of minor biblical figures#Assir|Assir]]<ref name=""Ex 6:24"">Exodus 6:24</ref><br>
............................27. [[wikipedia:Elkanah|Elkanah]]<ref name=""Ex 6:24""/><br>
............................27. [[wikipedia:Abiasaph|Abiasaph]]<ref name=""Ex 6:21"" /><br>
............................27. [[wikipedia:Ebiasaph|Ebiasaph]]<ref name=""1Ch 6:37"">1 Chronicles 6:37</ref><br>
.............................28. [[wikipedia:List of minor biblical figures#Assir|Assir]]<ref name=""1Ch 6:37""/><br>
..............................29. [[wikipedia:Tahath|Tahath]]<ref name=""1Ch 6:37""/><br>
...............................30. [[wikipedia:Zephaniah|Zephaniah]]<ref name=""1Ch 6:36"">1 Chronicles 6:36</ref><br>
................................31. Azariah<ref name=""1Ch 6:36""/><br>
.................................32. Joel<ref name=""1Ch 6:36""/><br>
..................................33. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Ch 6:36""/><br>
...................................34. [[wikipedia:Amasai|Amasai]]<ref name=""1Ch 6:35"">1 Chronicles 6:35</ref><br>
....................................35. [[wikipedia:Mahath|Mahath]]<ref name=""1Ch 6:35""/><br>
.....................................36. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Ch 6:35""/><br>
......................................37. [[wikipedia:Zuph|Zuph]]<ref name=""1Ch 6:35""/><br>
.......................................38. [[wikipedia:Toah|Toah]]<ref name=""1Ch 6:34"">1 Chronicles 6:34</ref><br>
........................................39. [[wikipedia:Eliel|Eliel]]<ref name=""1Ch 6:34""/><br>
.........................................40. [[wikipedia:Jeroham|Jeroham]]<ref name=""1Ch 6:34""/><br>
..........................................41. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Ch 6:34""/><br>
...........................................42. [[wikipedia:Samuel|Samuel]]<ref name=""1Ch 6:33"">1 Chronicles 6:33</ref><br>
............................................43. [[wikipedia:Joel (son of Samuel)|Joel]]<ref name=""1Ch 6:33""/><br>
.............................................44. [[wikipedia:Heman (Bible)|Heman]]<ref name=""1Ch 6:33""/><br>
..............................................45. [[wikipedia:Kore (Bible)|Kore]]<ref name=""1Ch 9:19"">1 Chronicles 9:19</ref><br>
...............................................46. [[wikipedia:Shallum|Shallum]]<ref name=""1Ch 9:19""/><br>
...............................................46. [[wikipedia:Meshelemiah|Meshelemiah]]<ref name=""1Ch 26:1"">1 Chronicles 26:1</ref><br>
................................................47. Zechariah<ref name=""1Ch 26:2"">1 Chronicles 26:2</ref><br>
................................................47. [[wikipedia:Jediael|Jediael]]<ref name=""1Ch 26:2""/><br>
................................................47. [[wikipedia:Zebadiah (Bible)|Zebadiah]]<ref name=""1Ch 26:2""/><br>
................................................47. [[wikipedia:Jathniel|Jathniel]]<ref name=""1Ch 26:2""/><br>
................................................47. [[wikipedia:Elam|Elam]]<ref name=""1Ch 26:3"">1 Chronicles 26:3</ref><br>
................................................47. [[wikipedia:Jehohanan|Jehohanan]]<ref name=""1Ch 26:3""/><br>
................................................47. [[wikipedia:Eliehoenai|Eliehoenai]]<ref name=""1Ch 26:3""/><br>
...........................26. [[wikipedia:Nepheg|Nepheg]]<ref name=""Ex 6:21""/><br>
...........................26. [[wikipedia:Zicri|Zicri]]<ref name=""Ex 6:21""/><br>
..........................25. [[wikipedia:Hebron (biblical figure)|Hebron]]<ref name=""Ex 6:18""/><br>
..........................25. [[wikipedia:Uzziel|Uzziel]]<ref name=""Ex 6:18""/><br>
...........................26. Mishael<ref name=""Ex 6:22""> Exodus 6:22</ref><br>
...........................26. [[wikipedia:Elzaphan|Elzaphan]]<ref name=""Ex 6:22""/><br>
...........................26. [[wikipedia:Sithri|Sithri]]<ref name=""Ex 6:22""/><br>
..........................25. [[wikipedia:Amminadab|Amminadab]]<ref name=""1Ch 6:22"">1 Chronicles 6:22</ref><br>
...........................26. [[wikipedia:Korah|Korah]]<ref name=""1Ch 6:22""/><br>
............................27. [[wikipedia:List of minor biblical figures#Assir|Assir]]<ref name=""1Ch 6:22""/><br>
.............................28. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Ch 6:23"">1 Chronicles 6:23</ref><br>
..............................29. [[wikipedia:Ebiasaph|Ebiasaph]]<ref name=""1Ch 6:23""/><br>
...............................30. [[wikipedia:List of minor biblical figures#Assir|Assir]]<ref name=""1Ch 6:23""/><br>
................................31. [[wikipedia:Tahath|Tahath]]<ref name=""1Ch 6:24"">1 Chronicles 6:24</ref><br>
.................................32. [[wikipedia:Uriel|Uriel]]<ref name=""1Ch 6:24""/><br>
..................................33. [[wikipedia:Uzziah|Uzziah]]<ref name=""1Ch 6:24""/><br>
...................................34. [[wikipedia:Shaul|Shaul]]<ref name=""1Ch 6:24""/><br>
..............................29. [[wikipedia:Amasai|Amasai]]<ref name=""1Ch 6:25"">1 Chronicles 6:25</ref><br>
..............................29. [[wikipedia:Ahimoth|Ahimoth]]<ref name=""1Ch 6:25""/><br>
..............................29. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Ch 6:26"">1 Chronicles 6:26</ref><br>
...............................30. [[wikipedia:Zophai|Zophai]]<ref name=""1Ch 6:26""/><br>
................................31. [[wikipedia:Nahath|Nahath]]<ref name=""1Ch 6:26""/><br>
.................................32. [[wikipedia:Eliab|Eliab]]<ref name=""1Ch 6:27"">1 Chronicles 6:27</ref><br>
..................................33. [[wikipedia:Jeroham|Jeroham]]<ref name=""1Ch 6:27""/><br>
...................................34. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Ch 6:27""/><br>
....................................35. [[wikipedia:Samuel|Samuel]]<ref name=""1Ch 6:27""/><br>
.....................................36. Joel<ref name=""1Ch 6:28"">1 Chronicles 6:28</ref><br>
.....................................36. [[wikipedia:Abijah|Abijah]]<ref name=""1Ch 6:28""/><br>
.........................24. [[wikipedia:Merari|Merari]]{{ref|Merari|5}}<ref name=""Ge 46:11""/>
..........................25. [[wikipedia:List of minor biblical figures,_L–Z#Mahali|Mahli]]<ref name=""Ex 6:19"">Exodus 6:19</ref><br>
...........................26. [[wikipedia:Libni|Libni]]<ref name=""1Ch 6:29"">1 Chronicles 6:29</ref><br>
............................27. [[wikipedia:Shimei|Shimei]]<ref name=""1Ch 6:29""/><br>
.............................28. [[wikipedia:Uzzah|Uzzah]]<ref name=""1Ch 6:29""/><br>
..............................29. [[wikipedia:Shimea|Shimea]]<ref name=""1Ch 6:30"">1 Chronicles 6:30</ref><br>
...............................30. [[wikipedia:Haggiah|Haggiah]]<ref name=""1Ch 6:30""/><br>
................................31. [[wikipedia:Asaiah|Asaiah]]<ref name=""1Ch 6:30""/><br>
..........................25. [[wikipedia:List of minor biblical figures,_L–Z#Mushi|Mushi]]<ref name=""Ex 6:19""/><br>
...........................26. [[wikipedia:List of minor biblical figures,_L–Z#Mahali|Mahli]]<ref name=""1Ch 6:47"">1 Chronicles 6:47</ref><br>
............................27. [[wikipedia:Shemer|Shemer]]<ref name=""1Ch 6:46"">1 Chronicles 6:46</ref><br>
.............................28. [[wikipedia:Bani (biblical figure)|Bani]]<ref name=""1Ch 6:46""/><br>
..............................29. [[wikipedia:Amzi|Amzi]]<ref name=""1Ch 6:46""/><br>
...............................30. [[wikipedia:Hilkiah|Hilkiah]]<ref name=""1Ch 6:45"">1 Chronicles 6:45</ref><br>
................................31. [[wikipedia:Amaziah|Amaziah]]<ref name=""1Ch 6:45""/><br>
.................................32. [[wikipedia:Hashabiah|Hashabiah]]<ref name=""1Ch 6:45""/><br>
..................................33. [[wikipedia:Malluch|Malluch]]<ref name=""1Ch 6:44"">1 Chronicles 6:44</ref><br>
...................................34. [[wikipedia:Abdi|Abdi]]<ref name=""1Ch 6:44""/> <br>
....................................35. [[wikipedia:Kishi (Bible)|Kishi]]<ref name=""1Ch 6:44""/> <br>
.....................................36. [[wikipedia:Ethan (Hebrew Bible)|Ethan]]<ref name=""1Ch 6:44""/> <br>
..................................33. [[wikipedia:Azrikam|Azrikam]]<ref name=""1Ch 9:14"" >1 Chronicles 9:14""</ref> <br>
...................................34. [[wikipedia:Hasshub|Hasshub]]<ref name=""1Ch 9:14""/> <br>
....................................35. [[wikipedia:Shemaiah (prophet)|Shemaiah]]<ref name=""1Ch 9:14""/> <br>
.........................24. [[wikipedia:Jochebed|Jochebed]]<ref name=""Ex 6:20""/><br>
.......................23. [[wikipedia:Judah (biblical person)|Judah]]<ref name=""Ge 29:35"">Genesis 29:35</ref><br>
........................24. [[wikipedia:Er (biblical person)|Er]]<ref name=""Ge 38:3"">Genesis 38:3</ref><br>
........................+ m. [[wikipedia:Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"">Genesis 38:6</ref><br>
........................24. [[wikipedia:Onan|Onan]]<ref name=""Ge 38:4"">Genesis 38:4</ref><br>
........................+ m. [[wikipedia:Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"" /><br>
........................24. [[wikipedia:Shelah (son of Judah)|Shelah]]<ref name=""Ge 38:5"">Genesis 38:5</ref><br>
.........................25. [[wikipedia:Er (Biblical name)|Er]]<ref name=""1Ch 4:21"">1 Chronicles 4:21</ref><br>
..........................26. [[wikipedia:Lecah|Lecah]]<ref name=""1Ch 4:21""/><br>
.........................25. [[wikipedia:Laadah|Laadah]]<ref name=""1Ch 4:21""/><br>
..........................26. [[wikipedia:Mareshah|Mareshah]]<ref name=""1Ch 4:21""/><br>
.........................25. [[wikipedia:Jokim|Jokim]]<ref name=""1Ch 4:22"">1 Chronicles 4:22</ref><br>
.........................25. Men of Cozeba<ref name=""1Ch 4:22""/><br>
.........................25. Joash<ref name=""1Ch 4:22""/><br>
.........................25. [[wikipedia:Saraph|Saraph]]<ref name=""1Ch 4:22""/><br>
...................... + m. [[wikipedia:Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"" /><br>
........................24. [[wikipedia:Perez (son of Judah)|Perez]]<ref name=""Ge 38:29"">Genesis 38:29</ref><br>
.........................25. [[wikipedia:Hezron|Hezron]]<ref name=""Ge 46:12"">Genesis 46:12</ref><br>
..........................26. [[wikipedia:Jerahmeel|Jerahmeel]]<ref name=""1Ch 2:9"">1 Chronicles 2:9</ref><br>
...........................27. [[wikipedia:Ram (biblical figure)|Ram]]<ref name=""1Ch 2:9"" /><br>
............................28. [[wikipedia:Maaz|Maaz]]<ref name=""1Ch 2:27"">1 Chronicles 2:27</ref><br>
............................28. [[wikipedia:List of minor Biblical figures#Jamin|Jamin]]<ref name=""1Ch 2:27""/><br>
............................28. [[wikipedia:Eker|Eker]]<ref name=""1Ch 2:27""/><br>
...........................27. [[wikipedia:Bunah|Bunah]]<ref name=""1Ch 2:25"">1 Chronicles 2:25</ref><br>
...........................27. [[wikipedia:Oren|Oren]]<ref name=""1Ch 2:25""/><br>
...........................27. [[wikipedia:List of minor biblical figures, L–Z#Ozem|Ozem]]<ref name=""1Ch 2:25""/><br>
...........................27. [[wikipedia:Ahijah|Ahijah]]<ref name=""1Ch 2:25""/><br>
...........................+ m. [[wikipedia:Atarah|Atarah]]<ref name=""1Ch 2:25""/><br>
...........................27. [[wikipedia:Onam|Onam]]<ref name=""1Ch 2:25""/><br>
............................28. [[wikipedia:Shammai|Shammai]]<ref name=""1Ch 2:28"">1 Chronicles 2:28</ref><br>
.............................29. Nadab<ref name=""1Ch 2:28""/><br>
..............................30. [[wikipedia:Seled|Seled]]<ref name=""1Ch 2:30"">1 Chronicles 2:30</ref><br>
..............................30. [[wikipedia:Appaim|Appaim]]<ref name=""1Ch 2:30""/><br>
...............................31. [[wikipedia:Ishi|Ishi]]<ref name=""1Ch 2:31"">1 Chronicles 2:31</ref><br>
................................32. Sheshan<ref name=""1Ch 2:31""/><br>
.................................33. [[wikipedia:Ahlai|Ahlai]]<ref name=""1Ch 2:31""/><br>
.................................33. Daughter<ref name=""1Ch 2:35"">1 Chronicles 2:35</ref><br>
.................................+ m. [[wikipedia:Jarha|Jarha]]<ref name=""1Ch 2:35""/><br>
..................................34. [[wikipedia:Attai|Attai]]<ref name=""1Ch 2:35""/><br>
...................................35. Nathan<ref name=""1Ch 2:36"">1 Chronicles 2:36</ref><br>
....................................36. [[wikipedia:Zabad (Bible)|Zabad]]<ref name=""1Ch 2:36""/><br>
.....................................37. [[wikipedia:Ephlal|Ephlal]]<ref name=""1Ch 2:37"">1 Chronicles 2:37</ref><br>
......................................38. Obed<ref name=""1Ch 2:37""/><br>
.......................................39. [[wikipedia:Jehu|Jehu]]<ref name=""1Ch 2:38"">1 Chronicles 2:38</ref><br>
........................................40. Azariah<ref name=""1Ch 2:38""/><br>
.........................................41. [[wikipedia:Helez|Helez]]<ref name=""1Ch 2:39"">1 Chronicles 2:39</ref><br>
..........................................42. [[wikipedia:Eleasah|Eleasah]]<ref name=""1Ch 2:39""/><br>
...........................................43. [[wikipedia:Sismai|Sismai]]<ref name=""1Ch 2:40"">1 Chronicles 2:40</ref><br>
............................................44. [[wikipedia:Shallum|Shallum]]<ref name=""1Ch 2:40""/><br>
.............................................45. [[wikipedia:Jekamiah|Jekamiah]]<ref name=""1Ch 2:41"">1 Chronicles 2:41</ref><br>
..............................................46. [[wikipedia:Elishaama|Elishaama]]<ref name=""1Ch 2:41""/><br>
................................32. [[wikipedia:Pelatiah|Pelatiah]]<ref name=""1Ch 4:42"">1 Chronicles 4:42</ref><br>
................................32. [[wikipedia:Neariah|Neariah]]<ref name=""1Ch 4:42""/><br>
................................32. [[wikipedia:Rephaiah|Rephaiah]]<ref name=""1Ch 4:42""/><br>
................................32. [[wikipedia:Uzziel|Uzziel]]<ref name=""1Ch 4:42""/><br>
................................?. [[wikipedia:Zoheth|Zoheth]]<ref name=""1Ch 4:20"">1 Chronicles 4:20</ref><br>
................................?. [[wikipedia:Ben-Zoheth|Ben-Zoheth]]<ref name=""1Ch 4:20""/><br>
.............................29. [[wikipedia:Abisur|Abisur]]<ref name=""1Ch 2:28""/><br>
.............................+ m. [[wikipedia:Abihail|Abihail]]<ref name=""1Ch 2:29"">1 Chronicles 2:29</ref><br>
..............................30. [[wikipedia:Ahban|Ahban]]<ref name=""1Ch 2:29""/><br>
..............................30. [[wikipedia:List of minor biblical figures,_L–Z#Molid|Molid]]<ref name=""1Ch 2:29""/><br>
............................28. [[wikipedia:Jada (biblical)|Jada]]<ref name=""1Ch 2:28""/><br>
.............................29. [[wikipedia:Jether|Jether]]<ref name=""1Ch 2:32"">1 Chronicles 2:32</ref><br>
.............................29. Jonathan<ref name=""1Ch 2:32""/><br>
..............................30. [[wikipedia:Peleth|Peleth]]<ref name=""1Ch 2:33"">1 Chronicles 2:33</ref><br>
..............................30. Zaza<ref name=""1Ch 2:33""/><br>
..........................26. [[wikipedia:Ram (biblical figure)|Ram]]<ref name=""Ruth 4:19"">Ruth 4:19</ref><br>
...........................27. [[wikipedia:Amminadab|Amminadab]]<ref name=""Nu 1:7"">Numbers 1:7</ref><br>
............................28. [[wikipedia:Nahshon|Nahshon]]<ref name=""Nu 1:7""/><br>
.............................29. [[wikipedia:Salmon (biblical figure)|Salmon]]<ref name=""Ruth 4:20"">Ruth 4:20</ref><br>
..............................30. [[wikipedia:Boaz|Boaz]]<ref name=""Ruth 4:21"">Ruth 4:21</ref><br>
..............................+ m. [[wikipedia:Book of Ruth|Ruth]]<ref name=""Ruth 4:13"">Ruth 4:13</ref><br>
...............................31. [[wikipedia:Obed (biblical figure)|Obed]]<ref name=""Ruth 4:21""/><br>
................................32. [[wikipedia:Jesse|Jesse]]<ref name=""Ruth 4:22"">Ruth 4:22</ref><br>
.................................33. [[wikipedia:Eliab|Eliab]]<ref name=""1Sa 17:13"">1 Samuel 17:13</ref><br>
..................................34. [[wikipedia:Abihail|Abihail]]<ref name=""2Ch 11:18"">2 Chronicles 11:18</ref><br>
..................................+ m. [[wikipedia:Jerimoth|Jerimoth]]<ref name=""2Ch 11:18""/><br>
...................................35. [[wikipedia:Mahalath|Mahalath]]<ref name=""2Ch 11:19"">2 Chronicles 11:19</ref><br>
...................................+ m. [[wikipedia:Rehoboam|Rehoboam]], King of Judah<ref name=""1Ki 11:43"">1 Kings 11:43</ref><br>
.................................33. [[wikipedia:Abinadab|Abinadab]]<ref name=""1Sa 17:13""/><br>
.................................33. [[wikipedia:Shammah|Shammah]]<ref name=""1Sa 17:13""/><br>
.................................33. [[wikipedia:Shimeah|Shimeah]]<ref name=""2Sa 13:3"">2 Samuel 13:3</ref><br>
..................................34. [[wikipedia:Jonadab|Jonadab]]<ref name=""2Sa 13:3""/><br>
.................................33. [[wikipedia:Nethanel|Nethanel]]<ref name=""1Ch 2:14"">1 Chronicles 2:14</ref><br>
.................................33. [[wikipedia:Raddai|Raddai]]<ref name=""1Ch 2:14""/><br>
.................................33. [[wikipedia:List of minor biblical figures, L–Z#Ozem|Ozem]]<ref name=""1Ch 2:15"">1 Chronicles 2:15</ref><br>
.................................33. [[wikipedia:David|David]]<ref name=""Ruth 4:22""/><br>
.................................+ m. [[wikipedia:Michal|Michal]]<ref name=""1Sa 14:49""/><br>
.................................+ m. [[wikipedia:Ahinoam|Ahinoam]] of Jezreel<ref name=""2Sa 3:2"">2 Samuel 3:2</ref><br>
..................................34. [[wikipedia:Amnon|Amnon]]<ref name=""2Sa 3:2""/><br>
.................................+ m. [[wikipedia:Abigail|Abigail]]<ref name=""2Sa 3:3"">2 Samuel 3:3</ref><br>
..................................34. [[wikipedia:Daniel (biblical figure)|Daniel]]<ref name=""1Ch 3:1"">1 Chronicles 3:1</ref><br>
..................................34. [[wikipedia:Kileab|Kileab]]<ref name=""2Sa 3:3""/><br>
.................................+ m. [[wikipedia:Maacah|Maacah]]<ref name=""2Sa 3:3""/><br>
..................................34. [[wikipedia:Absalom|Absalom]]<ref name=""1Ch 3:2"">1 Chronicles 3:2</ref><br>
..................................34. [[wikipedia:Absalom|Absalom]]<ref name=""2Sa 3:3""/><br>
..................................34. [[wikipedia:Tamar (2 Samuel)|Tamar]]<ref name=""2Sa 13:1"">2 Samuel 13:1</ref><br>
..................................34. [[wikipedia:Ibhar|Ibhar]]<ref name=""2Sa 5:15"">2 Samuel 5:15</ref><br>
..................................34. [[wikipedia:Elishua|Elishua]]<ref name=""2Sa 5:15""/><br>
..................................34. [[wikipedia:Nepheg|Nepheg]]<ref name=""2Sa 5:15""/><br>
..................................34. [[wikipedia:Japhia|Japhia]]<ref name=""2Sa 5:15""/><br>
..................................34. [[wikipedia:List of minor Biblical figures#Elishama|Elishama]]<ref name=""2Sa 5:16"">2 Samuel 5:16</ref><br>
..................................34. [[wikipedia:Eliada|Eliada]]<ref name=""2Sa 5:16""/><br>
..................................34. [[wikipedia:Eliphelet|Eliphelet]]<ref name=""2Sa 5:16""/><br>
..................................34. Nogah<ref name=""1Ch 3:7"">1 Chronicles 3:7</ref><br>
.................................+ m. [[wikipedia:Haggith|Haggith]]<ref name=""2Sa 3:4"">2 Samuel 3:4</ref><br>
..................................34. [[wikipedia:Adonijah|Adonijah]]<ref name=""2Sa 3:4""/><br>
.................................+ m. [[wikipedia:Abital|Abital]]<ref name=""2Sa 3:4""/><br>
..................................34. [[wikipedia:Shephatiah|Shephatiah]]<ref name=""2Sa 3:4""/><br>
.................................+ m. [[wikipedia:Eglah|Eglah]]<ref name=""2Sa 3:5"">2 Samuel 3:5</ref><br>
..................................34. [[wikipedia:Ithream|Ithream]]<ref name=""2Sa 3:5""/><br>
.................................+ m. [[wikipedia:Bathsheba|Bathsheba]]<ref name=""1Ch 3:5"">1 Chronicles 3:5</ref><br>
..................................34. [[wikipedia:Shammua|Shammua]]<ref name=""2Sa 5:14"">2 Samuel 5:14</ref><br>
..................................34. [[wikipedia:Shobab|Shobab]]<ref name=""2Sa 5:14""/><br>
..................................34. [[wikipedia:Nathan (son of David)|Nathan]]<ref name=""2Sa 5:14""/><br>
...................................35. Azariah<ref name=""1Ki 4:5"">1 Kings 4:5</ref><br>
...................................35. [[wikipedia:Zabud|Zabud]]<ref name=""1Ki 4:5""/><br>
..................................34. [[wikipedia:Solomon|Solomon]], King of Judah and Israel<ref name=""2Sa 5:14""/><br>
..................................+ m. [[wikipedia:Naamah (wife of Solomon)|Naamah]]<ref name=""1Ki 14:21"">1 Kings 14:21</ref><br>
...................................35. [[wikipedia:Rehoboam|Rehoboam]], King of Judah<ref name=""1Ki 11:43"" /><br>
...................................+ m1. [[wikipedia:List_of_minor_biblical_figures,_L–Z#Mahalath|Mahalath]]<ref name=""2Ch 11:19"" /><br>
....................................36. [[wikipedia:Jeush|Jeush]]<ref name=""2Ch 11:19""/><br>
....................................36. [[wikipedia:Shemariah|Shemariah]]<ref name=""2Ch 11:19""/><br>
....................................36. [[wikipedia:Zaham|Zaham]]<ref name=""2Ch 11:19""/><br>
...................................+ m2. [[wikipedia:Maakah|Maakah]]<ref name=""2Ch 11:20"">2 Chronicles 11:20</ref><br>
....................................36. [[wikipedia:Abijah of Judah|Abijah]], King of Judah<ref name=""1Ki 14:31"">1 Kings 14:31</ref><br>
.....................................37. [[wikipedia:Asa of Judah|Asa]], King of Judah<ref name=""1Ki 15:9"">1 Kings 15:9</ref><br>
......................................38. [[wikipedia:Jehoshaphat|Jehoshaphat]], King of Judah<ref name=""1Ki 15:24"">1 Kings 15:24</ref><br>
.......................................39. [[wikipedia:Jehoram of Judah|Jehoram]], King of Judah<ref name=""2Ki 8:16"">2 Kings 8:16</ref><br>
.......................................+ m. [[wikipedia:Athaliah|Athaliah]]<ref name=""2Ki 11:1"">2 Kings 11:1</ref><br>
........................................40. [[wikipedia:Ahaziah of Judah|Ahaziah]], King of Judah<ref name=""2Ki 8:24"">2 Kings 8:24</ref><br>
.........................................41. [[wikipedia:Jehoash of Judah|Joash]], King of Judah<ref name=""2Ki 11:1""/><br>
..........................................42. [[wikipedia:Amaziah of Judah|Amaziah]], King of Judah<ref name=""2Ki 14:13"">2 Kings 14:13</ref><br>
...........................................43. [[wikipedia:Uzziah|Azariah]], King of Judah<ref name=""2Ki 15:1"">2 Kings 15:1</ref><br>
............................................44. [[wikipedia:Jotham of Judah|Jotham]], King of Judah<ref name=""2Ki 15:5"">2 Kings 15:5</ref><br>
.............................................45. [[wikipedia:Ahaz|Ahaz]], King of Judah<ref name=""2Ki 15:38"">2 Kings 15:38</ref><br>
..............................................46. [[wikipedia:Hezekiah|Hezekiah]], King of Judah<ref name=""2Ki 18:1"">2 Kings 18:1</ref><br>
...............................................47. [[wikipedia:Manasseh of Judah|Manasseh]], King of Judah<ref name=""2Ki 20:21"">2 Kings 20:21</ref><br>
................................................48. [[wikipedia:Amon of Judah|Amon]], King of Judah<ref name=""2Ki 21:18"">2 Kings 21:18</ref><br>
.................................................49. [[wikipedia:Josiah|Josiah]], King of Judah<ref name=""2Ki 21:24"">2 Kings 21:24</ref><br>
..................................................50. [[wikipedia:Jehoahaz of Judah|Johanan]]<ref name=""1Ch 3:15"">1 Chronicles 3:15</ref><br>
..................................................50. [[wikipedia:Jehoiakim|Jehoiakim]], King of Judah<ref name=""2Ki 23:34"">2 Kings 23:34</ref><br>
...................................................51. [[wikipedia:Jeconiah|Jehoiachin]], King of Judah<ref name=""2Ki 24:6"">2 Kings 24:6</ref><br>
....................................................52. [[wikipedia:Shealtiel|Shealtiel]]<ref name=""1Ch 3:17"">1 Chronicles 3:17</ref><br>
....................................................52. [[wikipedia:Malkiram|Malkiram]]<ref name=""1Ch 3:18"">1 Chronicles 3:18</ref><br>
....................................................52. [[Pedaiah]]<ref name=""1Ch 3:18""/><br>
.....................................................53. [[wikipedia:Zerubbabel|Zerubbabel]]<ref name=""1Ch 3:19"">1 Chronicles 3:19</ref><br>
......................................................54. [[wikipedia:Meshullam|Meshullam]]<ref name=""1Ch 3:19""/><br>
......................................................54. [[wikipedia:Hananiah, son of Azzur|Hananiah]]<ref name=""1Ch 3:19""/><br>
.......................................................55. [[wikipedia:Pelatiah|Pelatiah]]<ref name=""1Ch 3:21"">1 Chronicles 3:21</ref><br>
.......................................................55. [[wikipedia:Jeshaiah|Jeshaiah]]<ref name=""1Ch 3:21""/><br>
.......................................................55. [[wikipedia:Rephaiah|Rephaiah]]<ref name=""1Ch 3:21""/><br>
.......................................................55. Arnan<ref name=""1Ch 3:21""/><br>
.......................................................55. [[wikipedia:Obadiah|Obadiah]]<ref name=""1Ch 3:21""/><br>
.......................................................55. [[wikipedia:Shecaniah|Shecaniah]]<ref name=""1Ch 3:21""/><br>
........................................................56. [[wikipedia:Shemaiah (prophet)|Shemaiah]]<ref name=""1Ch 3:22"">1 Chronicles 3:22</ref><br>
........................................................56. [[wikipedia:Hattush|Hattush]]<ref name=""1Ch 3:22""/><br>
........................................................56. Igal<ref name=""1Ch 3:22""/><br>
........................................................56. [[wikipedia:Bariah|Bariah]]<ref name=""1Ch 3:22""/><br>
........................................................56. [[wikipedia:Neariah|Neariah]]<ref name=""1Ch 3:22""/><br>
.........................................................57. [[wikipedia:Elioenai|Elioenai]]<ref name=""1Ch 3:23"">1 Chronicles 3:23</ref><br>
..........................................................58. [[wikipedia:Hodaviah|Hodaviah]]<ref name=""1Ch 3:24"">1 Chronicles 3:24</ref><br>
..........................................................58. [[wikipedia:Eliashib|Eliashib]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[wikipedia:Pelaiah|Pelaiah]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[wikipedia:Akkub|Akkub]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[wikipedia:Johanan|Johanan]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[wikipedia:Delaiah|Delaiah]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[wikipedia:Anani|Anani]]<ref name=""1Ch 3:24""/><br>
.........................................................57. [[wikipedia:Hizkiah|Hizkiah]]<ref name=""1Ch 3:23""/><br>
.........................................................57. [[wikipedia:Azrikam|Azrikam]]<ref name=""1Ch 3:23""/><br>
........................................................56. [[wikipedia:Shaphat|Shaphat]]<ref name=""1Ch 3:22""/><br>
......................................................54. [[wikipedia:Shelomith|Shelomith]]<ref name=""1Ch 3:19""/><br>
.......................................................55. [[wikipedia:Hashubah|Hashubah]]<ref name=""1Ch 3:20"">1 Chronicles 3:20</ref><br>
.......................................................55. [[wikipedia:Ohel (Bible)|Ohel]]<ref name=""1Ch 3:20""/><br>
.......................................................55. [[wikipedia:Berekiah|Berekiah]]<ref name=""1Ch 3:20""/><br>
.......................................................55. [[wikipedia:Hasadiah|Hasadiah]]<ref name=""1Ch 3:20""/><br>
.......................................................55. [[wikipedia:Jushab-Hesed|Jushab-Hesed]]<ref name=""1Ch 3:20""/><br>
.....................................................53. [[wikipedia:Shimei|Shimei]]<ref name=""1Ch 3:19""/><br>
....................................................52. [[wikipedia:Shenazzar|Shenazzar]]<ref name=""1Ch 3:18""/><br>
....................................................52. [[wikipedia:Jekamiah|Jekamiah]]<ref name=""1Ch 3:18""/><br>
....................................................52. [[wikipedia:Hoshama|Hoshama]]<ref name=""1Ch 3:18""/><br>
....................................................52. [[wikipedia:Nedabiah|Nedabiah]]<ref name=""1Ch 3:18""/><br>
..................................................50. [[wikipedia:Zedekiah|Zedekiah]], King of Judah<ref name=""1Ch 3:15""/><br>
..................................................50. [[wikipedia:Shallum|Shallum]]<ref name=""1Ch 3:15""/><br>
........................................40. [[wikipedia:Jehosheba|Jehosheba]]<ref name=""2Ki 11:2"">2 Kings 11:2</ref><br>
....................................36. [[wikipedia:Attai|Attai]]<ref name=""2Ch 11:20""/><br>
....................................36. [[wikipedia:Zizah|Ziza]]<ref name=""2Ch 11:20""/><br>
....................................36. [[wikipedia:Shelomith|Shelomith]]<ref name=""2Ch 11:20""/><br>
.................................33. [[wikipedia:Zeruiah|Zeruiah]]<ref name=""1Ch 2:16"">1 Chronicles 2:16</ref><br>
..................................34. [[wikipedia:Abishai (Biblical figure)|Abishai]]<ref name=""1Ch 2:16""/><br>
..................................34. [[wikipedia:Joab|Joab]]<ref name=""1Ch 2:16""/><br>
..................................34. [[wikipedia:Asahel|Asahel]]<ref name=""1Ch 2:16""/><br>
.................................33. [[wikipedia:Abigail|Abigail]]<ref name=""1Ch 2:16""/><br>
.................................+ m. [[wikipedia:Jether|Jether]]<ref name=""1Ch 2:17"">1 Chronicles 2:17</ref><br>
..................................34. [[wikipedia:Amasa|Amasa]]<ref name=""1Ch 2:17""/><br>
..........................26. [[wikipedia:Caleb|Caleb]]<ref name=""1Ch 2:9""/><br>
...........................27. [[wikipedia:Mesha|Mesha]]<ref name=""1Ch 2:42"">1 Chronicles 2:42</ref><br>
............................28. Ziph<ref name=""1Ch 2:42""/><br>
.............................29. [[wikipedia:Mareshah|Mareshah]]<ref name=""1Ch 2:42""/><br>
..............................30. [[wikipedia:Hebron (biblical figure)|Hebron]]<ref name=""1Ch 2:42""/><br>
...............................31. [[wikipedia:Korah|Korah]]<ref name=""1Ch 2:43"">1 Chronicles 2:43</ref><br>
...............................31. Tappuah<ref name=""1Ch 2:43""/><br>
...............................31. Rekem<ref name=""1Ch 2:43""/><br>
................................32. [[wikipedia:Shammai|Shammai]]<ref name=""1Ch 2:44"">1 Chronicles 2:44</ref><br>
.................................33. Maon<ref name=""1Ch 2:45"">1 Chronicles 2:45</ref><br>
..................................34. [[wikipedia:Beth Zur|Beth Zur]]<ref name=""1Ch 2:45""/><br>
...............................31. [[wikipedia:Shema|Shema]]<ref name=""1Ch 2:43""/><br>
................................32. [[wikipedia:Raham|Raham]]<ref name=""1Ch 2:44""/><br>
.................................33. [[wikipedia:Jorkeam|Jorkeam]]<ref name=""1Ch 2:44""/><br>
...........................27. [[wikipedia:Jesher|Jesher]]<ref name=""1Ch 2:18"">1 Chronicles 2:18</ref><br>
...........................27. [[wikipedia:Shobab|Shobab]]<ref name=""1Ch 2:18""/><br>
...........................27. Ardon<ref name=""1Ch 2:18""/><br>
..........................+ m. [[wikipedia:Ephrath|Ephrath]]<ref name=""1Ch 2:19"">1 Chronicles 2:19</ref><br>
............................28. [[wikipedia:Hur (Bible)|Hur]]<ref name=""Ex 31:2"">Exodus 31:2</ref><br>
.............................29. [[wikipedia:Uri (Bible)|Uri]]<ref name=""Ex 31:2""/><br>
..............................30. [[wikipedia:Bezalel|Bezalel]]<ref name=""Ex 31:2""/><br>
.............................29. [[wikipedia:Shobal|Shobal]]<ref name=""1Ch 2:50"">1 Chronicles 2:50</ref><br>
..............................30. [[wikipedia:Kiriath Jearim|Kiriath Jearim]]<ref name=""1Ch 2:50""/><br>
...............................31. [[wikipedia:Ithrites|Ithrites]]<ref name=""1Ch 2:53"">1 Chronicles 2:53</ref><br>
...............................31. [[wikipedia:Puthites|Puthites]]<ref name=""1Ch 2:53""/><br>
...............................31. [[wikipedia:Shumathites|Shumathites]]<ref name=""1Ch 2:53""/><br>
...............................31. [[wikipedia:Mishraites|Mishraites]]<ref name=""1Ch 2:53""/><br>
................................?. [[wikipedia:Zorathites|Zorathites]]<ref name=""1Ch 2:53""/><br>
................................?. [[wikipedia:Eshtaolites|Eshtaolites]]<ref name=""1Ch 2:53""/><br>
..............................30. [[wikipedia:Haroeh|Haroeh]]<ref name=""1Ch 2:52"">1 Chronicles 2:52</ref><br>
..............................30. [[wikipedia:Manahathite|Manahathite]]<ref name=""1Ch 2:52""/><br>
..............................30. [[wikipedia:Reaiah|Reaiah]]<ref name=""1Ch 4:2"">1 Chronicles 4:2</ref><br>
...............................31. [[wikipedia:Jahath|Jahath]]<ref name=""1Ch 4:2""/><br>
................................32. [[wikipedia:Ahumai|Ahumai]]<ref name=""1Ch 4:2""/><br>
................................32. [[wikipedia:Lahad|Lahad]]<ref name=""1Ch 4:2""/><br>
.............................29. [[wikipedia:Salma|Salma]]<ref name=""1Ch 2:51"">1 Chronicles 2:51</ref><br>
..............................30. [[wikipedia:Bethlehem|Bethlehem]]<ref name=""1Ch 2:51""/><br>
..............................?. Netophathites<ref name=""1Ch 2:54"">1 Chronicles 2:54</ref><br>
..............................?. Atroth Beth Joab<ref name=""1Ch 2:54""/><br>
..............................?. Half the Manahathites<ref name=""1Ch 2:54""/><br>
..............................?. Zorites<ref name=""1Ch 2:54""/><br>
..............................?. Tirathites<ref name=""1Ch 2:55"">1 Chronicles 2:55</ref><br>
..............................?. Shimeathites<ref name=""1Ch 2:55""/><br>
..............................?. Sucathites<ref name=""1Ch 2:55""/><br>
.............................29. [[wikipedia:Hareph|Hareph]]<ref name=""1Ch 2:51""/><br>
..............................30. [[wikipedia:Beth Gader|Beth Gader]]<ref name=""1Ch 2:51""/><br>
.............................?. [[wikipedia:Etam (Bible)|Etam]]<ref name=""1Ch 4:3"">1 Chronicles 4:3</ref><br>
..............................?. [[wikipedia:Jezreel|Jezreel]]<ref name=""1Ch 4:3""/><br>
..............................?. [[wikipedia:Ishma|Ishma]]<ref name=""1Ch 4:3""/><br>
..............................?. [[wikipedia:Idbash|Idbash]]<ref name=""1Ch 4:3""/><br>
..............................?. [[wikipedia:Hazzelelponi|Hazzelelponi]]<ref name=""1Ch 4:3""/><br>
.............................?. [[wikipedia:Penuel|Penuel]]<ref name=""1Ch 4:4"">1 Chronicles 4:4</ref><br>
..............................?. [[wikipedia:Gedor|Gedor]]<ref name=""1Ch 4:4""/><br>
..............................?. [[wikipedia:Ezer|Ezer]]<ref name=""1Ch 4:4""/><br>
...............................?. [[wikipedia:Hushah|Hushah]]<ref name=""1Ch 4:4""/><br>
..........................+ m. [[wikipedia:Ephah|Ephah]]<ref name=""1Ch 2:46"">1 Chronicles 2:46</ref><br>
...........................27. [[wikipedia:Haran|Haran]]<ref name=""1Ch 2:46""/><br>
............................28. [[wikipedia:Gezez|Gezez]]<ref name=""1Ch 2:46""/><br>
...........................27. Moza<ref name=""1Ch 2:46""/><br>
...........................27. [[wikipedia:Gezez|Gezez]]<ref name=""1Ch 2:46""/><br>
..........................+ m. [[wikipedia:Maacah|Maacah]]<ref name=""1Ch 2:48"">1 Chronicles 2:48</ref><br>
...........................27. [[wikipedia:Sheber|Sheber]]<ref name=""1Ch 2:48""/><br>
...........................27. [[wikipedia:Tirhanah|Tirhanah]]<ref name=""1Ch 2:48""/><br>
...........................27. [[wikipedia:Shaaph|Shaaph]]<ref name=""1Ch 2:49"">1 Chronicles 2:49</ref><br>
............................28. [[wikipedia:Madmannah|Madmannah]]<ref name=""1Ch 2:49""/><br>
...........................27. [[wikipedia:Sheva (Bible)|Sheva]]<ref name=""1Ch 2:49""/><br>
............................28. [[wikipedia:Macbenah|Macbenah]]<ref name=""1Ch 2:49""/><br>
............................28. [[wikipedia:Gibea|Gibea]]<ref name=""1Ch 2:49""/><br>
...........................27. [[wikipedia:Acsah|Acsah]]<ref name=""1Ch 2:49""/><br>
..........................26. [[wikipedia:Segub|Segub]]<ref name=""1Ch 2:21"">1 Chronicles 2:21</ref><br>
...........................27. [[wikipedia:Jair|Jair]], Judge of Israel<ref name=""1Ch 2:22"">1 Chronicles 2:22</ref><br> 
.........................+ m. [[wikipedia:Abijah|Abijah]]<ref name=""1Ch 2:24"">1 Chronicles 2:24</ref><br> 
..........................26. [[wikipedia:Ashhur|Ashhur]]<ref name=""1Ch 2:24""/><br> 
...........................27. Tekoa<ref name=""1Ch 2:24""/><br> 
..........................?. [[wikipedia:Jephunneh|Jephunneh]]<ref name=""Nu 13:6"">Numbers 13:6</ref><br>
...........................?. [[wikipedia:Caleb|Caleb]]<ref name=""Nu 13:6""/><br>
............................?. [[wikipedia:Acsah|Acsah]]<ref name=""Jos 15:7"">Joshua 15:7</ref><br>
............................?. Iru<ref name=""1Ch 4:15"">1 Chronicles 4:15</ref><br> 
............................?. Elah<ref name=""1Ch 4:15""/><br> 
.............................?. [[wikipedia:Kenaz|Kenaz]]<ref name=""1Ch 4:15""/><br> 
............................?. [[wikipedia:Naam|Naam]]<ref name=""1Ch 4:15""/><br> 
.........................25. [[wikipedia:Hamul|Hamul]]<ref name=""Ge 46:12""/><br>
.........................?. [[wikipedia:Bani (biblical figure)|Bani]]<ref name=""1Ch 9:4"">1 Chronicles 9:4</ref><br> 
..........................?. [[wikipedia:Imri|Imri]]<ref name=""1Ch 9:4""/><br> 
...........................?. [[wikipedia:Omri|Omri]]<ref name=""1Ch 9:4""/><br> 
............................?. [[wikipedia:Ammihud|Ammihud]]<ref name=""1Ch 9:4""/><br> 
.............................?. [[wikipedia:Uthai|Uthai]]<ref name=""1Ch 9:4""/><br> 
........................24. [[wikipedia:Zerah|Zerah]]<ref name=""Ge 38:30"">Genesis 38:30</ref><br>
.........................25. Zimri<ref name=""Jos 7:1""> Joshua 7:1</ref><br>
..........................26. [[wikipedia:List of minor biblical figures#Carmi|Carmi]]<ref name=""Jos 7:1""/><br>
...........................27. [[wikipedia:Achan (Biblical figure)|Achan]]<ref name=""Jos 7:1""/><br>
.........................25. Ethan<ref name=""1Ch 2:6"">1 Chronicles 2:6</ref><br>
..........................26. Azariah<ref name=""1Ch 2:8"">1 Chronicles 2:8</ref><br>
.........................25. Heman<ref name=""1Ch 2:6""/><br>
.........................25. [[wikipedia:Calcol|Calcol]]<ref name=""1Ch 2:6""/><br>
.........................25. Darda<ref name=""1Ch 2:6""/><br>
.........................?. [[wikipedia:Jeuel|Jeuel]]<ref name=""1Ch 9:6"">1 Chronicles 9:6</ref><br>
........................?. [[wikipedia:Ashhur|Ashhur]]<ref name=""1Ch 4:5"">1 Chronicles 4:5</ref><br>
.........................?. Tekoa<ref name=""1Ch 4:5""/><br>
........................+ m. [[wikipedia:Helah|Helah]]<ref name=""1Ch 4:5""/><br>
.........................?. [[wikipedia:Zereth|Zereth]]<ref name=""1Ch 4:7"">1 Chronicles 4:7</ref><br>
.........................?. [[wikipedia:Zohar|Zohar]]<ref name=""1Ch 4:7""/><br>
.........................?. [[wikipedia:Ethnan|Ethnan]]<ref name=""1Ch 4:7""/><br>
.........................?. Koz<ref name=""1Ch 4:8"">1 Chronicles 4:8</ref><br>
..........................?. [[wikipedia:Anub|Anub]]<ref name=""1Ch 4:8""/><br>
..........................?. [[wikipedia:Hazzobebah|Hazzobebah]]<ref name=""1Ch 4:8""/><br>
..........................?. Harum<ref name=""1Ch 4:8""/><br>
...........................?. [[wikipedia:Aharhel|Aharhel]]<ref name=""1Ch 4:8""/><br>
........................+ m. [[wikipedia:Naarah|Naarah]]<ref name=""1Ch 4:5""/><br>
.........................?. [[wikipedia:Ahuzzam|Ahuzzam]]<ref name=""1Ch 4:6"">1 Chronicles 4:6</ref><br>
.........................?. [[wikipedia:Hepher|Hepher]]<ref name=""1Ch 4:6""/><br>
.........................?. [[wikipedia:Temeni|Temeni]]<ref name=""1Ch 4:6""/><br>
.........................?. [[wikipedia:Haahashtari|Haahashtari]]<ref name=""1Ch 4:6""/><br>
........................?. [[wikipedia:Jabez (Bible)|Jabez]]<ref name=""1Ch 4:9"">1 Chronicles 4:9</ref><br>
........................?. [[wikipedia:Shuhah|Shuhah]]<ref name=""1Ch 4:11"">1 Chronicles 4:11</ref><br>
........................?. [[wikipedia:Kelub|Kelub]]<ref name=""1Ch 4:11""/><br>
.........................?. [[wikipedia:Mehir|Mehir]]<ref name=""1Ch 4:11""/><br>
..........................?. [[wikipedia:Eshton|Eshton]]<ref name=""1Ch 4:11""/><br>
...........................?. [[wikipedia:Beth Rapha|Beth Rapha]]<ref name=""1Ch 4:12"">1 Chronicles 4:12</ref><br>
...........................?. [[wikipedia:Paseah|Paseah]]<ref name=""1Ch 4:12""/><br>
...........................?. [[wikipedia:Tehinnah|Tehinnah]]<ref name=""1Ch 4:12""/><br>
............................?. [[wikipedia:Ir Nahash|Ir Nahash]]<ref name=""1Ch 4:12""/><br>
........................?. [[wikipedia:Kenaz|Kenaz]]<ref name=""1Ch 4:13"">1 Chronicles 4:13</ref><br>
.........................?. [[wikipedia:Othniel|Othniel]], Judge of Israel<ref name=""1Ch 4:13""/><br>
..........................?. [[wikipedia:Hathath|Hathath]]<ref name=""1Ch 4:13""/><br>
..........................?. [[wikipedia:Meonothai|Meonothai]]<ref name=""1Ch 4:13""/><br>
...........................?. [[wikipedia:Ophrah|Ophrah]]<ref name=""1Ch 4:14"">1 Chronicles 4:14</ref><br>
.........................?. [[wikipedia:Seraiah|Seraiah]]<ref name=""1Ch 4:13""/><br>
..........................?. [[wikipedia:Joab|Joab]]<ref name=""1Ch 4:14""/><br>
...........................?. [[wikipedia:Ge Harashim|Ge Harashim]]<ref name=""1Ch 4:14""/><br>
........................?. [[wikipedia:Jehallelel|Jehallelel]]<ref name=""1Ch 4:16"">1 Chronicles 4:16</ref><br>
.........................?. [[wikipedia:Ziph (son of Jehallelel)|Ziph]]<ref name=""1Ch 4:16""/><br>
.........................?. [[wikipedia:Ziphah|Ziphah]]<ref name=""1Ch 4:16""/><br>
.........................?. [[wikipedia:Tiria|Tiria]]<ref name=""1Ch 4:16""/><br>
.........................?. [[wikipedia:Asarel|Asarel]]<ref name=""1Ch 4:16""/><br>
........................?. [[wikipedia:Ezrah|Ezrah]]<ref name=""1Ch 4:17"">1 Chronicles 4:17</ref><br>
.........................?. [[wikipedia:Jether|Jether]]<ref name=""1Ch 4:17""/><br>
.........................?. [[wikipedia:Mered|Mered]]<ref name=""1Ch 4:17""/><br>
.........................+ m. [[wikipedia:Bithiah|Bithiah]], daughter of Pharaoh<ref name=""1Ch 4:18"">1 Chronicles 4:18</ref><br>
..........................?. [[wikipedia:Miriam|Miriam]]<ref name=""1Ch 4:17""/><br>
..........................?. [[wikipedia:Shammai|Shammai]]<ref name=""1Ch 4:17""/><br>
..........................?. [[wikipedia:Ishbah|Ishbah]]<ref name=""1Ch 4:17""/><br>
...........................?. [[wikipedia:Eshtamoa (son of Ishbah)|Eshtemoa]]<ref name=""1Ch 4:17""/><br>
.........................+ m.Unknown<ref name=""1Ch 4:18""/><br>
..........................?. [[wikipedia:Jered|Jered]]<ref name=""1Ch 4:18""/><br>
...........................?. [[wikipedia:Gedor|Gedor]]<ref name=""1Ch 4:18""/><br>
..........................?. [[wikipedia:Hebre (son of Gedor)|Heber]]<ref name=""1Ch 4:18""/><br>
...........................?. [[wikipedia:Soco (son of Hebre)|Soco]]<ref name=""1Ch 4:18""/><br>
..........................?. [[wikipedia:Jekuthiel|Jekuthiel]]<ref name=""1Ch 4:18""/><br>
...........................?. [[wikipedia:Zanoah|Zanoah]]<ref name=""1Ch 4:18""/><br>
.........................?. [[wikipedia:Epher|Epher]]<ref name=""1Ch 4:17""/><br>
.........................?. [[wikipedia:List of minor Biblical figures#Jalon|Jalon]]<ref name=""1Ch 4:17""/><br>
........................?. Hodiah's wife<ref name=""1Ch 4:19"">1 Chronicles 4:19</ref><br>
.........................?. Unknown<ref name=""1Ch 4:19""/><br>
..........................?. [[wikipedia:Keilah|Keilah]]<ref name=""1Ch 4:19""/><br>
.........................?. Unknown<ref name=""1Ch 4:19""/><br>
..........................?. [[wikipedia:Eshtamoa the Maachathite|Eshtamoa the Maachathite]]<ref name=""1Ch 4:19""/><br>
........................?. [[wikipedia:Naham|Naham]]<ref name=""1Ch 4:19""/><br>
........................?. [[wikipedia:List of minor biblical figures,_L–Z#Shimon|Shimon]]<ref name=""1Ch 4:20""/><br>
.........................?. [[wikipedia:Amnon|Amnon]]<ref name=""1Ch 4:20""/><br>
.........................?. [[wikipedia:Rinnah|Rinnah]]<ref name=""1Ch 4:20""/><br>
.........................?. [[wikipedia:Ben-Hanan|Ben-Hanan]]<ref name=""1Ch 4:20""/><br>
.........................?. [[wikipedia:Tilon|Tilon]]<ref name=""1Ch 4:20""/><br>
.......................23. [[wikipedia:Issachar|Issachar]]<ref name=""Ge 30:18"">Genesis 30:18</ref><br>
........................24. [[wikipedia:List of minor biblical figures,_L–Z#Tola|Tola]]<ref name=""Ge 46:13"">Genesis 46:13</ref><br>
.........................25. [[wikipedia:Uzzi|Uzzi]]<ref name=""1Ch 7:2"">1 Chronicles 7:2</ref><br>
...........................26. [[wikipedia:Izrahiah|Izrahiah]]<ref name=""1Ch 7:3"">1 Chronicles 7:3</ref><br>
............................27. [[wikipedia:Michael|Michael]]<ref name=""1Ch 7:3""/><br>
............................27. [[wikipedia:Obadiah|Obadiah]]<ref name=""1Ch 7:3""/><br>
............................27. [[wikipedia:Joel (prophet)|Joel]]<ref name=""1Ch 7:3""/><br>
............................27. [[wikipedia:Isshiah|Isshiah]]<ref name=""1Ch 7:3""/><br>
.........................25. [[wikipedia:Rephaiah|Rephaiah]]<ref name=""1Ch 7:2""/><br>
.........................25. [[wikipedia:Jeriel|Jeriel]]<ref name=""1Ch 7:2""/><br>
.........................25. [[wikipedia:Jahmai|Jahmai]]<ref name=""1Ch 7:2""/><br>
.........................25. [[wikipedia:Ibsam|Ibsam]]<ref name=""1Ch 7:2""/><br>
.........................25. [[wikipedia:Samuel|Samuel]]<ref name=""1Ch 7:2""/><br>
........................24. [[wikipedia:Puah|Puah]]<ref name=""Ge 46:13""/><br>
........................24. [[wikipedia:Jashub|Jashub]]<ref name=""Ge 46:13""/><br>
........................24. [[wikipedia:List of minor biblical figures,_L–Z#Shimron|Shimron]]<ref name=""Ge 46:13""/><br>
........................?. [[wikipedia:Zuar|Zuar]]<ref name=""Nu 1:8"">Numbers 1:8</ref><br>
.........................?. [[wikipedia:Nethanel|Nethanel]]<ref name=""Nu 1:8""/><br>
........................?. [[wikipedia:Joseph (son of Jacob)|Joseph]]<ref name=""Nu 13:7"">Numbers 13:7</ref><br>
.........................?. [[wikipedia:List of minor Biblical figures#Igal|Igal]]<ref name=""Nu 13:7""/><br>
........................?. [[wikipedia:Azzan|Azzan]]<ref name=""Nu 34:26"">Numbers 34:26</ref><br>
.........................?. [[wikipedia:List of minor biblical figures,_L–Z#Paltiel|Paltiel]]<ref name=""Nu 34:26""/><br>
........................?. [[wikipedia:Dodo|Dodo]]<ref name=""Judges 10:1"">Judges 10:1</ref><br>
.........................?. [[wikipedia:Puah|Puah]]<ref name=""Judges 10:1""/><br>
..........................?. [[wikipedia:Tola (Bible)|Tola]], Judge of Israel<ref name=""Judges 10:1""/><br>
.......................23. [[wikipedia:Zebulun|Zebulun]]<ref name=""Ge 30:20"">Genesis 30:20</ref><br>
........................24. [[wikipedia:Sered|Sered]]<ref name=""Ge 46:14"">Genesis 46:14</ref><br>
........................24. [[wikipedia:Elon|Elon]]<ref name=""Ge 46:14""/><br>
........................24. [[wikipedia:Jahleel|Jahleel]]<ref name=""Ge 46:14""/><br>
........................?. [[wikipedia:Helon|Helon]]<ref name=""Nu 1:9"">Numbers 1:9</ref><br>
.........................?. [[wikipedia:Eliab|Eliab]]<ref name=""Nu 1:9""/><br>
........................?. [[wikipedia:Sodi|Sodi]]<ref name=""Nu 13:10"">Numbers 13:10</ref><br>
.........................?. [[wikipedia:Gaddiel|Gaddiel]]<ref name=""Nu 13:10""/><br>
........................?. [[wikipedia:Parnach|Parnach]]<ref name=""Nu 34:25"">Numbers 34:25</ref><br>
.........................?. [[wikipedia:Elizaphan|Elizaphan]]<ref name=""Nu 34:25""/><br>
.......................23. [[wikipedia:Dinah|Dinah]]<ref name=""Ge 30:21"">Genesis 30:21</ref><br>
......................+ m. [[wikipedia:Rachel|Rachel]]<ref name=""Ge 29:6"">Genesis 29:6</ref><br>
.......................23. [[wikipedia:Joseph (son of Jacob)|Joseph]]<ref name=""Ge 30:24"">Genesis 30:24</ref><br>
.......................+ m. [[wikipedia:Asenath|Asenath]]<ref name=""Ge 46:20"">Genesis 46:20</ref><br>
........................24. [[wikipedia:Manasseh (tribal patriarch)|Manasseh]]<ref name=""Ge 46:20""/><br>
.........................25. [[wikipedia:Makir|Makir]]<ref name=""Nu 27:1"">Numbers 27:1</ref><br>
..........................26. [[wikipedia:Gilead|Gilead]]<ref name=""Nu 27:1""/><br>
...........................27. [[wikipedia:Hepher|Hepher]]<ref name=""Nu 27:1""/><br>
............................28. [[wikipedia:Zelophehad|Zelophehad]]<ref name=""Nu 27:1""/><br>
.............................29. [[wikipedia:Mahlah|Mahlah]]<ref name=""Nu 27:1""/><br>
.............................29. [[wikipedia:Noah|Noah]]<ref name=""Nu 27:1""/><br>
.............................29. [[wikipedia:Hoglah|Hoglah]]<ref name=""Nu 27:1""/><br>
.............................29. [[wikipedia:Milcah|Milcah]]<ref name=""Nu 27:1""/><br>
.............................29. [[wikipedia:Tirzah|Tirzah]]<ref name=""Nu 27:1""/><br>
..........................26. [[wikipedia:Hammoleketh|Hammoleketh]]<ref name=""1Ch 7:18"">1 Chronicles 7:18</ref><br>
...........................27. [[wikipedia:Ishhod|Ishhod]]<ref name=""1Ch 7:18""/><br>
...........................27. [[wikipedia:Abiezer|Abiezer]]<ref name=""1Ch 7:18""/><br>
...........................27. [[wikipedia:Mahlah|Mahlah]]<ref name=""1Ch 7:18""/><br>
.........................+ m. [[wikipedia:Maacah|Maacah]]<ref name=""1Ch 7:16"">1 Chronicles 7:16</ref><br>
..........................26. [[wikipedia:Peresh|Peresh]]<ref name=""1Ch 7:16""/><br>
...........................27. [[wikipedia:Ulam (Bible)|Ulam]]<ref name=""1Ch 7:16""/><br>
............................28. [[wikipedia:Bedan|Bedan]]<ref name=""1Ch 7:17"">1 Chronicles 7:17</ref><br>
...........................27. [[wikipedia:Rakem|Rakem]]<ref name=""1Ch 7:16""/><br>
..........................26. [[wikipedia:Sheresh|Sheresh]]<ref name=""1Ch 7:16""/><br>
.........................25. [[wikipedia:Maacah|Maacah]]<ref name=""1Ch 7:15"">1 Chronicles 7:15</ref><br>
.........................25. [[wikipedia:Jair|Jair]]<ref name=""1Ki 4:13"">1 Kings 4:13</ref><br>
.........................?. [[wikipedia:Pedahzur|Pedahzur]]<ref name=""Nu 1:10"">Numbers 1:10</ref><br>
..........................?. [[wikipedia:Gamaliel|Gamaliel]]<ref name=""Nu 1:10""/><br>
.........................?. [[wikipedia:List of minor biblical figures,_L–Z#Susi|Susi]]<ref name=""Nu 13:11"">Numbers 13:11</ref><br>
..........................?. Gaddi<ref name=""Nu 13:11""/><br>
..........................?. [[wikipedia:Ephod|Ephod]]<ref name=""Nu 34:23"">Numbers 34:23</ref><br>
..........................?. [[wikipedia:Hanniel|Hanniel]]<ref name=""Nu 34:23""/><br>
.........................?. [[wikipedia:Epher|Epher]]<ref name=""1Ch 5:24"">1 Chronicles 5:24</ref><br>
.........................?. [[wikipedia:Ishi|Ishi]]<ref name=""1Ch 5:24""/><br>
.........................?. [[wikipedia:Eliel|Eliel]]<ref name=""1Ch 5:24""/><br>
.........................?. [[wikipedia:List of minor Biblical figures#Azriel|Azriel]]<ref name=""1Ch 5:24""/><br>
.........................?. [[wikipedia:Jeremiah|Jeremiah]]<ref name=""1Ch 5:24""/><br>
.........................?. [[wikipedia:Hodaviah|Hodaviah]]<ref name=""1Ch 5:24""/><br>
.........................?. [[wikipedia:Jahdiel|Jahdiel]]<ref name=""1Ch 5:24""/><br>
.........................?. [[wikipedia:Asriel|Asriel]]<ref name=""1Ch 7:14"">1 Chronicles 7:14</ref><br>
.........................?. [[wikipedia:Shemida|Shemida]]<ref name=""1Ch 7:19"">1 Chronicles 7:19</ref><br>
..........................?. [[wikipedia:Ahian|Ahian]]<ref name=""1Ch 7:19""/><br>
..........................?. [[wikipedia:Shechem|Shechem]]<ref name=""1Ch 7:19""/><br>
..........................?. [[wikipedia:Likhi (Bible)|Likhi]]<ref name=""1Ch 7:19""/><br>
..........................?. [[wikipedia:Aniam|Aniam]]<ref name=""1Ch 7:19""/><br>
........................24. [[wikipedia:Ephraim|Ephraim]]<ref name=""Ge 46:20""/><br>
.........................25. [[wikipedia:Ezer|Ezer]]<ref name=""1Ch 7:21"">1 Chronicles 7:21</ref><br>
.........................25. [[wikipedia:Elead|Elead]]<ref name=""1Ch 7:21""/><br>
.........................25. [[wikipedia:List of minor biblical figures#Beriah|Beriah]]<ref name=""1Ch 7:23"">1 Chronicles 7:23</ref><br>
..........................26. [[wikipedia:Sheerah|Sheerah]]<ref name=""1Ch 7:24"">1 Chronicles 7:24</ref><br>
..........................26. [[wikipedia:Rephah|Rephah]]<ref name=""1Ch 7:25"">1 Chronicles 7:25</ref><br>
...........................27. [[wikipedia:Resheph|Resheph]]<ref name=""1Ch 7:25""/><br>
............................28. [[wikipedia:Telah|Telah]]<ref name=""1Ch 7:25""/><br>
.............................29. [[wikipedia:Tahan|Tahan]]<ref name=""1Ch 7:25""/><br>
..............................30. Ladan<ref name=""1Ch 7:26"">1 Chronicles 7:26</ref><br>
...............................31. [[wikipedia:Ammihud|Ammihud]]<ref name=""1Ch 7:26""/><br>
................................32. [[wikipedia:List of minor Biblical figures#Elishama|Elishama]]<ref name=""1Ch 7:26""/><br>
.................................33. [[wikipedia:Nun|Nun]]<ref name=""Nu 13:8"">Numbers 13:8</ref><ref name=""1Ch 7:27"">1 Chronicles 7:27</ref><br>
..................................34. [[wikipedia:Joshua|Joshua]]<ref name=""Nu 13:8""/><ref name=""1Ch 7:27""/><br> 
.........................?. [[wikipedia:Ammihud|Ammihud]]<ref name=""Nu 1:10""/><br>
..........................?. [[wikipedia:List of minor Biblical figures#Elishama|Elishama]]<ref name=""Nu 1:10""/><br>
.........................?. [[wikipedia:Shiphtan|Shiphtan]]<ref name=""Nu 34:24"">Numbers 34:24</ref><br>
..........................?. [[wikipedia:Kemuel|Kemuel]]<ref name=""Nu 34:24""/><br>
.........................?. [[wikipedia:Zuph|Zuph]]<ref name=""1Sa 1:1"">1 Samuel 1:1</ref><br>
..........................?. [[wikipedia:List of minor biblical figures,_L–Z#Tohu|Tohu]]<ref name=""1Sa 1:1""/><br>
...........................?. [[wikipedia:Elihu (Job)|Elihu]]<ref name=""1Sa 1:1""/><br>
............................?. [[wikipedia:Jeroham|Jeroham]]<ref name=""1Sa 1:1""/><br>
.............................?. [[wikipedia:Elkanah|Elkanah]]<ref name=""1Sa 1:1""/><br>
.............................+ m. [[wikipedia:Hannah (Bible)|Hannah]]<ref name=""1Sa 1:8"">1 Samuel 1:8</ref><br>
..............................?. [[wikipedia:Samuel|Samuel]]<ref name=""1Sa 1:20"">1 Samuel 1:20</ref><br>
.........................?. [[wikipedia:Nebat|Nebat]]<ref name=""1Ki 11:26"">1 Kings 11:26</ref><br>
..........................?. [[wikipedia:Jeroboam|Jeroboam]]<ref name=""1Ki 11:26""/><br>
...........................?. [[wikipedia:List of minor biblical figures,_L–Z#Nadab|Nadab]]<ref name=""1Ki 14:20"">1 Kings 14:20</ref><br>
.........................?. [[wikipedia:Shuthelah|Shuthelah]]<ref name=""1Ch 7:20"">1 Chronicles 7:20</ref><br>
..........................?. [[wikipedia:Bered|Bered]]<ref name=""1Ch 7:20""/><br>
...........................?. [[wikipedia:Tahath|Tahath]]<ref name=""1Ch 7:20""/><br>
............................?. [[wikipedia:Eleadah|Eleadah]]<ref name=""1Ch 7:20""/><br>
.............................?. [[wikipedia:Tahath|Tahath]]<ref name=""1Ch 7:20""/><br>
..............................?. [[wikipedia:Zabad (Bible)|Zabad]]<ref name=""1Ch 7:20""/><br>
...............................?. [[wikipedia:Shuthelah|Shuthelah]]<ref name=""1Ch 7:20""/><br>
.......................23. [[wikipedia:Benjamin|Benjamin]]<ref name=""Ge 35:18"">Genesis 35:18</ref><br>
........................24. [[wikipedia:List of minor Biblical figures#Bela|Bela]]<ref name=""Ge 46:21"">Genesis 46:21</ref><br>
.........................25. [[wikipedia:Ezbon|Ezbon]]<ref name=""1Ch 7:7"">1 Chronicles 7:7</ref><br>
.........................25. [[wikipedia:Uzzi|Uzzi]]<ref name=""1Ch 7:7""/><br>
.........................25. [[wikipedia:Uzziel|Uzziel]]<ref name=""1Ch 7:7""/><br>
.........................25. [[wikipedia:Jerimoth|Jerimoth]]<ref name=""1Ch 7:7""/><br>
.........................25. [[wikipedia:Iri|Iri]]<ref name=""1Ch 7:7""/><br>
.........................25. [[wikipedia:Addar|Addar]]<ref name=""1Ch 8:3"">1 Chronicles 8:3</ref><br>
.........................25. [[wikipedia:List of minor biblical figures, A–K#Gera|Gera]]<ref name=""1Ch 8:3""/><br>
.........................25. [[wikipedia:Abihud|Abihud]]<ref name=""1Ch 8:3""/><br>
.........................25. [[wikipedia:Abishua|Abishua]]<ref name=""1Ch 8:4"">1 Chronicles 8:4</ref><br>
.........................25. [[wikipedia:Naaman|Naaman]]<ref name=""1Ch 8:4""/><br>
.........................25. [[wikipedia:Ahoah|Ahoah]]<ref name=""1Ch 8:4""/><br>
.........................25. [[wikipedia:List of minor biblical figures, A–K#Gera|Gera]]<ref name=""1Ch 8:5"">1 Chronicles 8:5</ref><br>
.........................25. [[wikipedia:Shephuphan|Shephuphan]]<ref name=""1Ch 8:5""/><br>
.........................25. [[wikipedia:Huram (biblical figure)|Huram]]<ref name=""1Ch 8:5""/><br>
........................24. [[wikipedia:Ashbel|Ashbel]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:Aharah|Aharah]]<ref name=""1Ch 8:1"">1 Chronicles 8:1</ref><br>
........................24. [[wikipedia:Nohah|Nohah]]<ref name=""1Ch 8:2"">1 Chronicles 8:2</ref><br>
........................24. [[wikipedia:Rapha (biblical figure)|Rapha]]<ref name=""1Ch 8:2""/><br>
........................24. [[wikipedia:Beker|Beker]]<ref name=""Ge 46:21""/><br>
.........................25. [[wikipedia:Zemirah|Zemirah]]<ref name=""1Ch 7:8"">1 Chronicles 7:8</ref><br>
.........................25. [[wikipedia:List of minor Biblical figures#Joash|Joash]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Eliezer|Eliezer]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Elioenai|Elioenai]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Omri|Omri]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Jeremoth|Jeremoth]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Abijah|Abijah]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Anathoth|Anathoth]]<ref name=""1Ch 7:8""/><br>
.........................25. [[wikipedia:Alemeth|Alemeth]]<ref name=""1Ch 7:8""/><br>
........................24. [[wikipedia:List of minor biblical figures, A–K#Gera|Gera]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:Naaman|Naaman]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:Minor characters in the Book of Genesis|Ehi]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:List of minor biblical figures,_L–Z#Rosh|Rosh]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:Muppim|Muppim]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:Huppim|Huppim]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:List of minor Biblical figures#Ard|Ard]]<ref name=""Ge 46:21""/><br>
........................24. [[wikipedia:Jediael|Jediael]]<ref name=""1Ch 7:6"">1 Chronicles 7:6</ref><br>
.........................25. [[wikipedia:Bilhan|Bilhan]]<ref name=""1Ch 7:10"">1 Chronicles 7:10</ref><br>
..........................26. [[wikipedia:Jeush|Jeush]]<ref name=""1Ch 7:11"">1 Chronicles 7:11</ref><br>
..........................26. [[wikipedia:Benjamin|Benjamin]]<ref name=""1Ch 7:11""/><br>
..........................26. [[wikipedia:Ehud|Ehud]]<ref name=""1Ch 7:11""/><br>
...........................27. [[wikipedia:Naaman|Naaman]]<ref name=""1Ch 8:7"">1 Chronicles 8:7</ref><br>
...........................27. [[wikipedia:Ahijah|Ahijah]]<ref name=""1Ch 8:7""/><br>
...........................27. [[wikipedia:List of minor biblical figures, A–K#Gera|Gera]]<ref name=""1Ch 8:7""/><br>
............................28. [[wikipedia:Uzza|Uzza]]<ref name=""1Ch 8:7""/><br>
............................28. [[wikipedia:Ahihud|Ahihud]]<ref name=""1Ch 8:7""/><br>
..........................26. [[wikipedia:Kenaanah|Kenaanah]]<ref name=""1Ch 7:11""/><br>
..........................26. [[wikipedia:Zethan|Zethan]]<ref name=""1Ch 7:11""/><br>
..........................26. [[wikipedia:Tarshish|Tarshish]]<ref name=""1Ch 7:11""/><br>
..........................26. [[wikipedia:Ahishahar|Ahishahar]]<ref name=""1Ch 7:11""/><br>
........................?. [[wikipedia:Gideoni|Gideoni]]<ref name=""Nu 1:11"">Numbers 1:11</ref><br>
.........................?. [[wikipedia:Abidan|Abidan]]<ref name=""Nu 1:11""/><br>
........................?. [[wikipedia:Raphu|Raphu]]<ref name=""Nu 13:9"">Numbers 13:9</ref><br>
.........................?. [[wikipedia:Palti|Palti]]<ref name=""Nu 13:9""/><br>
........................?. [[wikipedia:Kison|Kison]]<ref name=""Nu 34:21"">Numbers 34:21</ref><br>
.........................?. [[wikipedia:Elidad|Elidad]]<ref name=""Nu 34:21""/><br>
........................?. [[wikipedia:List of minor biblical figures, A–K#Gera|Gera]]<ref name=""Judges 3:15"">Judges 3:15</ref><br>
.........................?. [[wikipedia:Ehud|Ehud]]<ref name=""Judges 3:15""/><br>
..........................?. [[wikipedia:Naaman|Naaman]]<ref name=""1Ch 8:7"" /><br>
..........................?. [[wikipedia:Ahijah|Ahijah]]<ref name=""1Ch 8:7""/><br>
..........................?. [[wikipedia:List of minor biblical figures, A–K#Gera|Gera]]<ref name=""1Ch 8:7""/><br>
...........................?. [[wikipedia:Uzza|Uzza]]<ref name=""1Ch 8:7""/><br>
...........................?. [[wikipedia:Ahihud|Ahihud]]<ref name=""1Ch 8:7""/><br>
........................?. [[wikipedia:Aphiah|Aphiah]]<ref name=""1Sa 9:1"">1 Samuel 9:1</ref><br>
.........................?. [[wikipedia:Becorath|Becorath]]<ref name=""1Sa 9:1""/><br>
..........................?. [[wikipedia:Zeror|Zeror]]<ref name=""1Sa 9:1""/><br>
...........................?. [[wikipedia:Abiel|Abiel]]<ref name=""1Sa 9:1""/><br>
............................?. [[wikipedia:Kish (Bible)|Kish]]<ref name=""1Sa 9:1""/><br>
.............................?. [[wikipedia:Saul|Saul]]<ref name=""1Sa 9:2"">1 Samuel 9:2</ref><br>
.............................+ m. [[wikipedia:Ahinoam|Ahinoam]]<ref name=""1Sa 14:50"">1 Samuel 14:50</ref><br>
..............................?. [[wikipedia:List of minor Biblical figures#Jonathan|Jonathan]]<ref name=""1Sa 13:16"">1 Samuel 13:16</ref><br>
...............................?. [[wikipedia:Mephibosheth|Mephibosheth]]<ref name=""2Sa 4:4"">2 Samuel 4:4</ref><br>
................................?. [[wikipedia:Mica|Mica]]<ref name=""2Sa 9:12"">2 Samuel 9:12</ref><br>
..............................?. [[wikipedia:Ishvi|Ishvi]]<ref name=""1Sa 14:49"">1 Samuel 14:49</ref><br>
..............................?. [[wikipedia:Malki-Shua|Malki-Shua]]<ref name=""1Sa 14:49""/><br>
..............................?. [[wikipedia:Merab|Merab]]<ref name=""1Sa 14:49""/><br>
..............................?. [[wikipedia:Michal|Michal]]<ref name=""1Sa 14:49""/><br>
..............................?. [[wikipedia:Abinadab|Abinadab]]<ref name=""1Sa 31:2"">1 Samuel 31:2</ref><br>
..............................?. [[wikipedia:Ish-Bosheth|Ish-Bosheth]]<ref name=""2Sa 2:8"">2 Samuel 2:8</ref><br>
..............................?. [[wikipedia:Merab|Merab]]<ref name=""2Sa 21:8"">2 Samuel 21:8</ref><br>
............................?. [[wikipedia:Ner|Ner]]<ref name=""1Sa 14:50""/><br>
.............................?. [[wikipedia:Abner|Abner]]<ref name=""1Sa 14:50""/><br>
........................?. [[wikipedia:Shaharaim|Shaharaim]]<ref name=""1Ch 8:8"">1 Chronicles 8:8</ref><br>
........................+ m. [[wikipedia:Hushim|Hushim]]<ref name=""1Ch 8:8""/><br>
.........................?. [[wikipedia:Abitub|Abitub]]<ref name=""1Ch 8:11"">1 Chronicles 8:11</ref><br>
.........................?. [[wikipedia:Elpaal|Elpaal]]<ref name=""1Ch 8:11""/><br>
..........................?. [[wikipedia:Eber|Eber]]<ref name=""1Ch 8:12"">1 Chronicles 8:12</ref><br>
..........................?. [[wikipedia:Misham|Misham]]<ref name=""1Ch 8:12""/><br>
..........................?. [[wikipedia:Shemed|Shemed]]<ref name=""1Ch 8:12""/><br>
..........................?. [[wikipedia:List of minor biblical figures#Beriah|Beriah]]<ref name=""1Ch 8:13"">1 Chronicles 8:13</ref><br>
...........................?. [[wikipedia:Ahio|Ahio]]<ref name=""1Ch 8:14"">1 Chronicles 8:14</ref><br>
...........................?. [[wikipedia:Shashak|Shashak]]<ref name=""1Ch 8:14""/><br>
............................?. [[wikipedia:Ishpan|Ishpan]]<ref name=""1Ch 8:22"">1 Chronicles 8:22</ref><br>
............................?. [[wikipedia:Eber|Eber]]<ref name=""1Ch 8:22""/><br>
............................?. [[wikipedia:Eliel|Eliel]]<ref name=""1Ch 8:22""/><br>
............................?. [[wikipedia:List of minor biblical figures#Abdon|Abdon]]<ref name=""1Ch 8:23"">1 Chronicles 8:23</ref><br>
............................?. [[wikipedia:Zicri|Zicri]]<ref name=""1Ch 8:23""/><br>
............................?. [[wikipedia:List of minor Biblical figures#Hanan|Hanan]]<ref name=""1Ch 8:23""/><br>
............................?. [[wikipedia:Hananiah, son of Azzur|Hananiah]]<ref name=""1Ch 8:24"">1 Chronicles 8:24</ref><br>
............................?. [[wikipedia:Elam|Elam]]<ref name=""1Ch 8:24""/><br>
............................?. [[wikipedia:Anthothijah|Anthothijah]]<ref name=""1Ch 8:24""/><br>
............................?. [[wikipedia:Iphdeiah|Iphdeiah]]<ref name=""1Ch 8:25"">1 Chronicles 8:25</ref><br>
............................?. [[wikipedia:Penuel|Penuel]]<ref name=""1Ch 8:25""/><br>
...........................?. [[wikipedia:Jeremoth|Jeremoth]]<ref name=""1Ch 8:14""/><br>
...........................?. [[wikipedia:List of minor biblical figures,_L–Z#Zebadiah|Zebadiah]]<ref name=""1Ch 8:15"">1 Chronicles 8:15</ref><br>
...........................?. [[wikipedia:Arad (Bibl)|Arad]]<ref name=""1Ch 8:15""/><br>
...........................?. [[wikipedia:Eder|Eder]]<ref name=""1Ch 8:15""/><br>
...........................?. [[wikipedia:Michael|Michael]]<ref name=""1Ch 8:16"">1 Chronicles 8:16</ref><br>
...........................?. [[wikipedia:Ishpah|Ishpah]]<ref name=""1Ch 8:16""/><br>
...........................?. [[wikipedia:Joha|Joha]]<ref name=""1Ch 8:16""/><br>
..........................?. [[wikipedia:Shema|Shema]]<ref name=""1Ch 8:13""/><br>
..........................?. [[wikipedia:List of minor biblical figures,_L–Z#Zebadiah|Zebadiah]]<ref name=""1Ch 8:17"">1 Chronicles 8:17</ref><br>
..........................?. [[wikipedia:Meshullam|Meshullam]]<ref name=""1Ch 8:17""/><br>
..........................?. [[wikipedia:Hizki|Hizki]]<ref name=""1Ch 8:17""/><br>
..........................?. [[wikipedia:Minor characters in the Book of Genesis|Heber]]<ref name=""1Ch 8:17""/><br>
..........................?. [[wikipedia:Ishmerai|Ishmerai]]<ref name=""1Ch 8:18"">1 Chronicles 8:18</ref><br>
..........................?. [[wikipedia:Izliah|Izliah]]<ref name=""1Ch 8:18""/><br>
..........................?. [[wikipedia:Jobab|Jobab]]<ref name=""1Ch 8:18""/><br>
........................+ m. [[wikipedia:Baara|Baara]]<ref name=""1Ch 8:8""/><br>
........................+ m. [[wikipedia:Hodesh|Hodesh]]<ref name=""1Ch 8:9"">1 Chronicles 8:9</ref><br>
.........................?. [[wikipedia:Jobab|Jobab]]<ref name=""1Ch 8:9""/><br>
.........................?. [[wikipedia:Zibia|Zibia]]<ref name=""1Ch 8:9""/><br>
.........................?. [[wikipedia:Mesha|Mesha]]<ref name=""1Ch 8:9""/><br>
.........................?. [[wikipedia:Malcam|Malcam]]<ref name=""1Ch 8:9""/><br>
.........................?. [[wikipedia:Jeuz|Jeuz]]<ref name=""1Ch 8:10"">1 Chronicles 8:10</ref><br>
.........................?. [[wikipedia:Sakia|Sakia]]<ref name=""1Ch 8:10""/><br>
.........................?. [[wikipedia:Mirmah|Mirmah]]<ref name=""1Ch 8:10""/><br>
........................?. [[wikipedia:Shimei|Shimei]]<ref name=""1Ch 8:21"">1 Chronicles 8:21</ref><br>
.........................?. [[wikipedia:Jakim|Jakim]]<ref name=""1Ch 8:19"">1 Chronicles 8:19</ref><br>
.........................?. [[wikipedia:Zicri|Zicri]]<ref name=""1Ch 8:19""/><br>
.........................?. [[wikipedia:Zabdi|Zabdi]]<ref name=""1Ch 8:19""/><br>
.........................?. [[wikipedia:Elienai|Elienai]]<ref name=""1Ch 8:20"">1 Chronicles 8:20</ref><br>
.........................?. [[wikipedia:Zillethai|Zillethai]]<ref name=""1Ch 8:20""/><br>
.........................?. [[wikipedia:Eliel|Eliel]]<ref name=""1Ch 8:20""/><br>
.........................?. [[wikipedia:Adaiah|Adaiah]]<ref name=""1Ch 8:21""/><br>
.........................?. [[wikipedia:Beraiah|Beraiah]]<ref name=""1Ch 8:21""/><br>
.........................?. [[wikipedia:Shimrath|Shimrath]]<ref name=""1Ch 8:21""/><br>
........................?. [[wikipedia:Jeiel|Jeiel]]<ref name=""1Ch 8:29"">1 Chronicles 8:29</ref><br>
.........................?. [[wikipedia:List of minor Biblical figures#Gibeon|Gibeon]]<ref name=""1Ch 8:29""/><br>
.........................+ m. [[wikipedia:Maacah|Maacah]]<ref name=""1Ch 8:29""/><br>
..........................?. [[wikipedia:List of minor biblical figures#Abdon|Abdon]]<ref name=""1Ch 8:30"">1 Chronicles 8:30</ref><br>
..........................?. [[wikipedia:Zur|Zur]]<ref name=""1Ch 8:30""/><br>
..........................?. Kish<ref name=""1Ch 8:30""/><br>
..........................?. [[wikipedia:Baal|Baal]]<ref name=""1Ch 8:30""/><br>
..........................?. [[wikipedia:Ner|Ner]]<ref name=""1Ch 8:30""/><br>
...........................?. Kish<ref name=""1Ch 8:33"">1 Chronicles 8:33</ref><br>
............................?. [[wikipedia:Saul|Saul]]<ref name=""1Ch 8:33""/><br>
.............................?. [[wikipedia:List of minor Biblical figures#Jonathan|Jonathan]]<ref name=""1Ch 8:33""/><br>
..............................?. [[wikipedia:Merib-Baal|Merib-Baal]]<ref name=""1Ch 8:34"">1 Chronicles 8:34</ref><br>
...............................?. [[wikipedia:Micah|Micah]]<ref name=""1Ch 8:34""/><br>
................................?. [[wikipedia:Pithon|Pithon]]<ref name=""1Ch 8:35"">1 Chronicles 8:35</ref><br>
................................?. [[wikipedia:List of minor biblical figures,_L–Z#Melech|Melech]]<ref name=""1Ch 8:35""/><br>
................................?. [[wikipedia:Tarea|Tarea]]<ref name=""1Ch 8:35""/><br>
................................?. [[wikipedia:Ahaz|Ahaz]]<ref name=""1Ch 8:35""/><br>
.................................?. [[wikipedia:Jehoaddah|Jehoaddah]]<ref name=""1Ch 8:36"">1 Chronicles 8:36</ref><br>
..................................?. [[wikipedia:Alemeth|Alemeth]]<ref name=""1Ch 8:36""/><br>
..................................?. [[wikipedia:Azmaveth|Azmaveth]]<ref name=""1Ch 8:36""/><br>
...................................?. [[wikipedia:Jeziel|Jeziel]]<ref name=""1Ch 12:3"">1 Chronicles 12:3</ref><br>
...................................?. [[wikipedia:Pelet|Pelet]]<ref name=""1Ch 12:3""/><br>
..................................?. [[wikipedia:Zimri (prince)|Zimri]]<ref name=""1Ch 8:36""/><br>
...................................?. [[wikipedia:List of minor biblical figures,_L–Z#Moza|Moza]]<ref name=""1Ch 8:36""/><br>
....................................?. [[wikipedia:Binea|Binea]]<ref name=""1Ch 8:37"">1 Chronicles 8:37</ref><br>
.....................................?. [[wikipedia:Raphah|Raphah]]<ref name=""1Ch 8:37""/><br>
......................................?. [[wikipedia:Eleasah|Eleasah]]<ref name=""1Ch 8:37""/><br>
.......................................?. [[wikipedia:List of minor Biblical figures#Azel|Azel]]<ref name=""1Ch 8:37""/><br>
........................................?. [[wikipedia:Azrikam|Azrikam]]<ref name=""1Ch 8:38"">1 Chronicles 8:38</ref><br>
........................................?. [[wikipedia:Bokeru|Bokeru]]<ref name=""1Ch 8:38""/><br>
........................................?. [[wikipedia:Ishmael|Ishmael]]<ref name=""1Ch 8:38""/><br>
........................................?. [[wikipedia:Sheariah|Sheariah]]<ref name=""1Ch 8:38""/><br>
........................................?. [[wikipedia:Obadiah|Obadiah]]<ref name=""1Ch 8:38""/><br>
........................................?. [[wikipedia:List of minor Biblical figures#Hanan|Hanan]]<ref name=""1Ch 8:38""/><br>
.......................................?. [[wikipedia:Eshek|Eshek]]<ref name=""1Ch 8:39"">1 Chronicles 8:39</ref><br>
........................................?. [[wikipedia:Ulam (Bible)|Ulam]]<ref name=""1Ch 8:39""/><br>
........................................?. [[wikipedia:Jeush|Jeush]]<ref name=""1Ch 8:39""/><br>
........................................?. [[wikipedia:Eliphelet|Eliphelet]]<ref name=""1Ch 8:39""/><br>
.............................?. [[wikipedia:Malki-Shua|Malki-Shua]]<ref name=""1Ch 8:33""/><br>
.............................?. [[wikipedia:Abinadab|Abinadab]]<ref name=""1Ch 8:33""/><br>
.............................?. [[wikipedia:Esh-Baal|Esh-Baal]]<ref name=""1Ch 8:33""/><br>
..........................?. [[wikipedia:Nadab (son of Aaron)|Nadab]]<ref name=""1Ch 8:30""/><br>
..........................?. [[wikipedia:Gedor|Gedor]]<ref name=""1Ch 8:31"">1 Chronicles 8:31</ref><br>
..........................?. [[wikipedia:Ahio|Ahio]]<ref name=""1Ch 8:31""/><br>
..........................?. [[wikipedia:Zeker|Zeker]]<ref name=""1Ch 8:31""/><br>
..........................?. [[wikipedia:Mikloth|Mikloth]]<ref name=""1Ch 8:32"">1 Chronicles 8:32</ref><br>
...........................?. [[wikipedia:Shimeah|Shimeah]]<ref name=""1Ch 8:32""/><br>
........................?. [[wikipedia:Hassenuah|Hassenuah]]<ref name=""1Ch 9:7"">1 Chronicles 9:7</ref><br>
.........................?. [[wikipedia:Hodaviah|Hodaviah]]<ref name=""1Ch 9:7""/><br>
..........................?. [[wikipedia:Meshullam|Meshullam]]<ref name=""1Ch 9:7""/><br>
...........................?. [[wikipedia:Sallu|Sallu]]<ref name=""1Ch 9:7""/><br>
........................?. [[wikipedia:Jeroham|Jeroham]]<ref name=""1Ch 9:8"">1 Chronicles 9:8</ref><br>
.........................?. [[wikipedia:Ibneiah|Ibneiah]]<ref name=""1Ch 9:8""/><br>
........................?. [[wikipedia:Micri|Micri]]<ref name=""1Ch 9:8""/><br>
.........................?. [[wikipedia:Uzzi|Uzzi]]<ref name=""1Ch 9:8""/><br>
..........................?. [[wikipedia:King Elah|Elah]]<ref name=""1Ch 9:8""/><br>
........................?. [[wikipedia:Ibnijah|Ibnijah]]<ref name=""1Ch 9:8""/><br>
.........................?. [[wikipedia:Reuel|Reuel]]<ref name=""1Ch 9:8""/><br>
..........................?. [[wikipedia:Shephatiah|Shephatiah]]<ref name=""1Ch 9:8""/><br>
...........................?. [[wikipedia:Meshullam|Meshullam]]<ref name=""1Ch 9:8""/><br>
......................+ m. [[wikipedia:Bilhah|Bilhah]]<ref name=""Ge 30:4"">Genesis 30:4</ref><br>
.......................23. [[wikipedia:Dan (Bible)|Dan]]<ref name=""Ge 30:6"">Genesis 30:6</ref><br>
........................24. [[wikipedia:Hushim|Hushim]]<ref name=""Ge 46:23"">Genesis 46:23</ref><br>
........................?. [[wikipedia:Ahisamach (Bible)|Ahisamach]]<ref name=""Ex 31:6"">Exodus 31:6</ref><br>
.........................?. [[wikipedia:Oholiab|Oholiab]]<ref name=""Ex 31:6""/><br>
........................?. [[wikipedia:Ammishaddai|Ammishaddai]]<ref name=""Nu 1:12"">Numbers 1:12</ref><br>
.........................?. [[wikipedia:Ahiezer|Ahiezer]]<ref name=""Nu 1:12""/><br>
........................?. [[wikipedia:Gemalli|Gemalli]]<ref name=""Nu 13:12"">Numbers 13:12</ref><br>
.........................?. [[wikipedia:Ammiel|Ammiel]]<ref name=""Nu 13:12""/><br>
........................?. [[wikipedia:Jogli|Jogli]]<ref name=""Nu 34:22"">Numbers 34:22</ref><br>
.........................?. [[wikipedia:Bukki|Bukki]]<ref name=""Nu 34:22""/><br>
.......................23. [[wikipedia:Naphtali|Naphtali]]<ref name=""Ge 30:8"">Genesis 30:8</ref><br>
........................24. [[wikipedia:Jahziel|Jahziel]]<ref name=""Ge 46:24"">Genesis 46:24</ref><br>
........................24. Guni<ref name=""Ge 46:24""/><br>
........................24. [[wikipedia:Jezer|Jezer]]<ref name=""Ge 46:24""/><br>
........................24. [[wikipedia:Shillem|Shillem]]<ref name=""Ge 46:24""/><br>
........................?. [[wikipedia:Enan|Enan]]<ref name=""Nu 1:15"">Numbers 1:15</ref><br>
.........................?. [[wikipedia:Ahira|Ahira]]<ref name=""Nu 1:15""/><br>
........................?. [[wikipedia:Vophsi|Vophsi]]<ref name=""Nu 13:14"">Numbers 13:14</ref><br>
.........................?. [[wikipedia:Nahbi|Nahbi]]<ref name=""Nu 13:14""/><br>
........................?. [[wikipedia:Ammihud|Ammihud]]<ref name=""Nu 34:28"">Numbers 34:28</ref><br>
.........................?. [[wikipedia:Pedahel|Pedahel]]<ref name=""Nu 34:28""/><br>
......................+ m. [[wikipedia:Zilpah|Zilpah]]<ref name=""Ge 30:8"" /><br>
.......................23. [[wikipedia:Gad (son of Jacob)|Gad]]<ref name=""Ge 30:11"">Genesis 30:11</ref><br>
........................24. [[wikipedia:Zephon|Zephon]]<ref name=""Ge 46:16"">Genesis 46:16</ref><br>
........................24. [[wikipedia:Haggi|Haggi]]<ref name=""Ge 46:16""/><br>
........................24. [[wikipedia:Shuni|Shuni]]<ref name=""Ge 46:16""/><br>
........................24. [[wikipedia:Ezbon|Ezbon]]<ref name=""Ge 46:16""/><br>
........................24. [[wikipedia:List of minor Biblical figures#Eri|Eri]]<ref name=""Ge 46:16""/><br>
........................24. [[wikipedia:Arodi|Arodi]]<ref name=""Ge 46:16""/><br>
........................24. [[wikipedia:Areli|Areli]]<ref name=""Ge 46:16""/><br>
........................?. [[wikipedia:List of minor Biblical figures#Deuel|Deuel]]<ref name=""Nu 1:14"">Numbers 1:14</ref><br>
.........................?. [[wikipedia:Eliasaph|Eliasaph]]<ref name=""Nu 1:14""/><br>
........................?. [[wikipedia:List of minor biblical figures,_L–Z#Maki|Maki]]<ref name=""Nu 13:15"">Numbers 13:15</ref><br>
.........................?. [[wikipedia:Geuel|Geuel]]<ref name=""Nu 13:15""/><br>
........................?. [[wikipedia:Joel (prophet)|Joel]]<ref name=""1Ch 5:12"">1 Chronicles 5:12</ref><br>
........................?. [[wikipedia:Shapham|Shapham]]<ref name=""1Ch 5:12""/><br>
........................?. [[wikipedia:Janai|Janai]]<ref name=""1Ch 5:12""/><br>
........................?. [[wikipedia:Shaphat|Shaphat]]<ref name=""1Ch 5:12""/><br>
.........................?. [[wikipedia:List of minor Biblical figures#Buz|Buz]]<ref name=""1Ch 5:14"">1 Chronicles 5:14</ref><br>
..........................?. [[wikipedia:Jahdo|Jahdo]]<ref name=""1Ch 5:14""/><br>
...........................?. [[wikipedia:Jeshishai|Jeshishai]]<ref name=""1Ch 5:14""/><br>
............................?. [[wikipedia:Michael|Michael]]<ref name=""1Ch 5:14""/><br>
.............................?. [[wikipedia:Gilead|Gilead]]<ref name=""1Ch 5:14""/><br>
..............................?. [[wikipedia:Jaroah|Jaroah]]<ref name=""1Ch 5:14""/><br>
...............................?. [[wikipedia:Huri|Huri]]<ref name=""1Ch 5:14""/><br>
................................?. [[wikipedia:Abihail|Abihail]]<ref name=""1Ch 5:14""/><br>
.................................?. [[wikipedia:Michael|Michael]]<ref name=""1Ch 5:13"">1 Chronicles 5:13</ref><br>
.................................?. [[wikipedia:Meshullam|Meshullam]]<ref name=""1Ch 5:13""/><br>
.................................?. [[wikipedia:Sheba|Sheba]]<ref name=""1Ch 5:13""/><br>
.................................?. [[wikipedia:Jorai|Jorai]]<ref name=""1Ch 5:13""/><br>
.................................?. [[wikipedia:Jacan|Jacan]]<ref name=""1Ch 5:13""/><br>
.................................?. [[wikipedia:List of minor biblical figures,_L–Z#Zia|Zia]]<ref name=""1Ch 5:13""/><br>
.................................?. [[wikipedia:Eber|Eber]]<ref name=""1Ch 5:13""/><br>
........................?. Guni<ref name=""1Ch 5:15"">1 Chronicles 5:15</ref><br>
.........................?. [[wikipedia:Abdiel|Abdiel]]<ref name=""1Ch 5:15""/><br>
..........................?. [[wikipedia:List of minor biblical figures#Ahi|Ahi]]<ref name=""1Ch 5:15""/><br>
.......................23. [[wikipedia:Asher|Asher]]<ref name=""Ge 30:13"">Genesis 30:13</ref><br>
........................24. [[wikipedia:Imnah|Imnah]]<ref name=""Ge 46:17"">Genesis 46:17</ref><br>
........................24. [[wikipedia:Ishvah|Ishvah]]<ref name=""Ge 46:17""/><br>
........................24. [[wikipedia:Ishvi|Ishvi]]<ref name=""Ge 46:17""/><br>
........................24. [[wikipedia:List of minor biblical figures#Beriah|Beriah]]<ref name=""Ge 46:17""/><br>
.........................25. [[wikipedia:Minor characters in the Book of Genesis|Heber]]<ref name=""Ge 46:17""/><br>
..........................26. [[wikipedia:Japhlet|Japhlet]]<ref name=""1Ch 7:32"">1 Chronicles 7:32</ref><br>
...........................27. [[wikipedia:Pasach|Pasach]]<ref name=""1Ch 7:33"">1 Chronicles 7:33</ref><br>
...........................27. [[wikipedia:Bimhal|Bimhal]]<ref name=""1Ch 7:33""/><br>
...........................27. [[wikipedia:Ashvath|Ashvath]]<ref name=""1Ch 7:33""/><br>
..........................26. [[wikipedia:Shomer|Shomer]]<ref name=""1Ch 7:32""/><br>
...........................27. [[wikipedia:List of minor biblical figures#Ahi|Ahi]]<ref name=""1Ch 7:34"">1 Chronicles 7:34</ref><br>
...........................27. [[wikipedia:Rohgah|Rohgah]]<ref name=""1Ch 7:34""/><br>
...........................27. [[wikipedia:Hubbah|Hubbah]]<ref name=""1Ch 7:34""/><br>
...........................27. [[wikipedia:Aram, son of Shem|Aram]]<ref name=""1Ch 7:34""/><br>
..........................26. [[wikipedia:List of minor Biblical figures#Hotham|Hotham]]<ref name=""1Ch 7:32""/><br>
..........................26. [[wikipedia:Shua|Shua]]<ref name=""1Ch 7:32""/><br>
..........................26. [[wikipedia:Helem|Helem]]<ref name=""1Ch 7:35"">1 Chronicles 7:35</ref><br>
...........................27. [[wikipedia:Zophah|Zophah]]<ref name=""1Ch 7:35""/><br>
............................28. [[wikipedia:Suah|Suah]]<ref name=""1Ch 7:36"">1 Chronicles 7:36</ref><br>
............................28. [[wikipedia:Harnepher|Harnepher]]<ref name=""1Ch 7:36""/><br>
............................28. [[wikipedia:Shual|Shual]]<ref name=""1Ch 7:36""/><br>
............................28. [[wikipedia:List of minor Biblical figures#Beri|Beri]]<ref name=""1Ch 7:36""/><br>
............................28. [[wikipedia:Imrah|Imrah]]<ref name=""1Ch 7:36""/><br>
............................28. [[wikipedia:Bezer|Bezer]]<ref name=""1Ch 7:37"">1 Chronicles 7:37</ref><br>
............................28. [[wikipedia:List of minor Biblical figures#Hod|Hod]]<ref name=""1Ch 7:37""/><br>
............................28. [[wikipedia:Shamma|Shamma]]<ref name=""1Ch 7:37""/><br>
............................28. [[wikipedia:Shilshah|Shilshah]]<ref name=""1Ch 7:37""/><br>
............................28. [[wikipedia:Ithran|Ithran]]<ref name=""1Ch 7:37""/><br>
............................28. [[wikipedia:Beera|Beera]]<ref name=""1Ch 7:37""/><br>
...........................27. [[wikipedia:Imna (Bible)|Imna(Bible)]]<ref name=""1Ch 7:35""/><br>
...........................27. [[wikipedia:Shelesh|Shelesh]]<ref name=""1Ch 7:35""/><br>
...........................27. [[wikipedia:List of minor Biblical figures#Amal|Amal]]<ref name=""1Ch 7:35""/><br>
.........................25. [[wikipedia:List of minor biblical figures,_L–Z#Malkiel|Malkiel]]<ref name=""Ge 46:17""/><br>
..........................26. [[wikipedia:Birzaith|Birzaith]]<ref name=""1Ch 7:31"">1 Chronicles 7:31</ref><br>
........................24. [[wikipedia:Serah|Serah]]<ref name=""Ge 46:17""/><br>
........................?. [[wikipedia:Ocran|Ocran]]<ref name=""Nu 1:13"">Numbers 1:13</ref><br>
.........................?. [[wikipedia:Pagiel|Pagiel]]<ref name=""Nu 1:13""/><br>
........................?. [[wikipedia:Michael|Michael]]<ref name=""Nu 13:1"">Numbers 13:1</ref><br>
.........................?. [[wikipedia:Sethur|Sethur]]<ref name=""Nu 13:1""/><br>
........................?. [[wikipedia:List of minor biblical figures,_L–Z#Shelomi|Shelomi]]<ref name=""Nu 34:27"">Numbers 34:27</ref><br>
.........................?. [[wikipedia:Ahihud|Ahihud]]<ref name=""Nu 34:27""/><br>
........................?. [[wikipedia:Jether|Jether]]<ref name=""1Ch 7:38"">1 Chronicles 7:38</ref><br>
.........................?. [[wikipedia:Jephunneh|Jephunneh]]<ref name=""1Ch 7:38""/><br>
.........................?. [[wikipedia:Pispah|Pispah]]<ref name=""1Ch 7:38""/><br>
.........................?. [[wikipedia:List of minor Biblical figures#Ara|Ara]]<ref name=""1Ch 7:38""/><br>
........................?. [[wikipedia:List of minor biblical figures,_L–Z#Ulla|Ulla]]<ref name=""1Ch 7:39"">1 Chronicles 7:39</ref><br>
.........................?. [[wikipedia:Arah|Arah]]<ref name=""1Ch 7:39""/><br>
.........................?. [[wikipedia:Hanniel|Hanniel]]<ref name=""1Ch 7:39""/><br>
.........................?. [[wikipedia:Rizia|Rizia]]<ref name=""1Ch 7:39""/><br>
....................+ m. [[wikipedia:Hagar (Bible)|Hagar]]<ref name=""Ge 16:3"">Genesis 16:3</ref><br>
.....................21. [[wikipedia:Ishmael|Ishmael]]<ref name=""Ge 16:15"">Genesis 16:15</ref><br>
......................22. [[wikipedia:Nebaioth|Nebaioth]]<ref name=""Ge 25:13"">Genesis 25:13</ref><br>
......................22. [[wikipedia:Qedarite#Biblical|Kedar]]<ref name=""Ge 25:13""/><br>
......................22. [[wikipedia:Adbeel|Adbeel]]<ref name=""Ge 25:13""/><br>
......................22. [[wikipedia:Mibsam|Mibsam]]<ref name=""Ge 25:13""/><br>
......................22. [[wikipedia:Mishma|Mishma]]<ref name=""Ge 25:14"">Genesis 25:14</ref><br>
......................22. [[wikipedia:Dumah (son of Ishmael)|Dumah]]<ref name=""Ge 25:14""/><br>
......................22. [[wikipedia:Massa|Massa]]<ref name=""Ge 25:14""/><br>
......................22. [[wikipedia:Hadad|Hadad]]<ref name=""Ge 25:15"">Genesis 25:15</ref><br>
......................22. [[wikipedia:Tema|Tema]]<ref name=""Ge 25:15""/><br>
......................22. [[wikipedia:Jetur|Jetur]]<ref name=""Ge 25:15""/><br>
......................22. [[wikipedia:Naphish|Naphish]]<ref name=""Ge 25:15""/><br>
......................22. [[wikipedia:Kedemah|Kedemah]]<ref name=""Ge 25:15""/><br>
......................22. [[wikipedia:Basemath|Basemath]]<ref name=""Ge 36:3""/><br>
....................+ m. [[wikipedia:Keturah|Keturah]]<ref name=""Ge 25:1"">Genesis 25:1</ref><br>
.....................21. [[wikipedia:Zimran|Zimran]]<ref name=""Ge 25:2"">Genesis 25:2</ref><br>
.....................21. [[wikipedia:Jokshan|Jokshan]]<ref name=""Ge 25:2""/><br>
......................22. [[wikipedia:Sheba|Sheba]]<ref name=""Ge 25:3"">Genesis 25:3</ref><br>
......................22. [[wikipedia:Dedanites|Dedan]]<ref name=""Ge 25:3""/><br>
.......................23. [[wikipedia:Asshurites|Asshurites]]<ref name=""Ge 25:3""/><br>
.......................23. [[wikipedia:Letushites|Letushites]]<ref name=""Ge 25:3""/><br>
.......................23. [[wikipedia:Leummites|Leummites]]<ref name=""Ge 25:3""/><br>
.....................21. [[wikipedia:Medan|Medan]]<ref name=""Ge 25:2""/><br>
.....................21. [[wikipedia:Midian|Midian]]<ref name=""Ge 25:2""/><br>
......................22. [[wikipedia:Ephah|Ephah]]<ref name=""Ge 25:4"">Genesis 25:4</ref><br>
......................22. [[wikipedia:Epher|Epher]]<ref name=""Ge 25:4""/><br>
......................22. [[wikipedia:Hanoch (Bible)|Hanoch]]<ref name=""Ge 25:4""/><br>
......................22. [[wikipedia:Abida Midian|Abida]]<ref name=""Ge 25:4""/><br>
......................22. [[wikipedia:Eldaah|Eldaah]]<ref name=""Ge 25:4""/><br>
.....................21. [[wikipedia:Ishbak|Ishbak]]<ref name=""Ge 25:2""/><br>
.....................21. [[wikipedia:Shuah|Shuah]]<ref name=""Ge 25:2""/><br>
....................20. [[wikipedia:Nahor, son of Terah|Nahor]]<ref name=""Ge 11:26""/><br>
....................+ m. [[wikipedia:Milcah|Milcah]]<ref name=""Ge 11:29""/><br>
.....................21. [[wikipedia:Uz (son of Aram)|Uz]]<ref name=""Ge 22:21"">Genesis 22:21</ref><br>
.....................21. [[wikipedia:Children of Eber|Buz]]<ref name=""Ge 22:21""/><br>
.....................21. [[wikipedia:Kemuel|Kemuel]]<ref name=""Ge 22:21""/><br>
......................22. [[wikipedia:Aram, son of Shem|Aram]]<ref name=""Ge 22:21""/><br>
.....................21. [[wikipedia:Kesed|Kesed]]<ref name=""Ge 22:22"">Genesis 22:22</ref><br>
.....................21. [[wikipedia:Hazo, son of Nahor|Hazo]]<ref name=""Ge 22:22""/><br>
.....................21. [[wikipedia:Pildash|Pildash]]<ref name=""Ge 22:22""/><br>
.....................21. [[wikipedia:Jidlaph|Jidlaph]]<ref name=""Ge 22:22""/><br>
.....................21. [[wikipedia:Bethuel|Bethuel]]<ref name=""Ge 22:22""/><br>
......................22. [[wikipedia:Rebekah|Rebekah]]<ref name=""Ge 22:23"" /><br>
......................22. [[wikipedia:Laban (Bible)|Laban]]<ref name=""Ge 24:29"">Genesis 24:29</ref><br>
.......................23. [[wikipedia:Leah|Leah]]<ref name=""Ge 29:16"" /><br>
.......................23. [[wikipedia:Rachel|Rachel]]<ref name=""Ge 29:6"" /><br>
....................+ m. [[wikipedia:Reumah|Reumah]]<ref name=""Ge 22:24"">Genesis 22:24</ref><br>
.....................21. [[wikipedia:List of minor biblical figures,_L–Z#Tebah|Tebah]]<ref name=""Ge 22:24""/><br>
.....................21. [[wikipedia:Gaham|Gaham]]<ref name=""Ge 22:24""/><br>
.....................21. [[wikipedia:Tahash|Tahash]]<ref name=""Ge 22:24""/><br>
.....................21. [[wikipedia:Maacah|Maacah]]<ref name=""Ge 22:24""/><br>
....................20. [[wikipedia:Haran|Haran]]<ref name=""Ge 11:26""/><br>
.....................21. [[wikipedia:Lot (biblical person)|Lot]]<ref name=""Ge 11:27"">Genesis 11:27</ref><br>
......................22. Older daughter<ref name=""Ge 19:8"">Genesis 19:8</ref><br>
......................22. Younger daughter<ref name= ""Ge 19:8"" />< br >
......................+ m.Older daughter<ref name=""Ge 19:8""/><br>
......................22. [[wikipedia:Moab|Moab]]<ref name=""Ge 19:37"">Genesis 19:37</ref><br>
.......................23. Moabites<ref name=""Ge 19:37""/><br>
......................+ m.Younger daughter<ref name=""Ge 19:8""/><br>
......................22. [[wikipedia:Ben-Ammi|Ben-Ammi]]<ref name=""Ge 19:38"">Genesis 19:38</ref><br>
.......................23. Ammonites<ref name=""Ge 19:38""/><br>
.....................21. [[wikipedia:Milcah|Milcah]]<ref name=""Ge 11:29""/><br>
.....................21. [[wikipedia:Iscah|Iscah]]<ref name=""Ge 11:29""/><br>
....................20. [[wikipedia:Sarah|Sarah]]<ref name=""Ge 11:29""/><br>
...............15. [[wikipedia:Joktan|Joktan]]<ref name=""Ge 10:25""/><br>
................16. [[wikipedia:Almodad|Almodad]]<ref name=""Ge 10:26"">Genesis 10:26</ref><br>
................16. [[wikipedia:Sheleph|Sheleph]]<ref name=""Ge 10:26""/><br>
................16. [[wikipedia:Hazarmaveth|Hazarmaveth]]<ref name=""Ge 10:26""/><br>
................16. [[wikipedia:Jerah|Jerah]]<ref name=""Ge 10:26""/><br>
................16. [[wikipedia:Hadoram|Hadoram]]<ref name=""Ge 10:27"">Genesis 10:27</ref><br>
................16. [[wikipedia:Uzal|Uzal]]<ref name=""Ge 10:27""/><br>
................16. [[wikipedia:Diklah|Diklah]]<ref name=""Ge 10:27""/><br>
................16. [[wikipedia:Obal|Obal]]<ref name=""Ge 10:28"">Genesis 10:28</ref><br>
................16. [[wikipedia:Abimael|Abimael]]<ref name=""Ge 10:28""/><br>
................16. [[wikipedia:Sheba|Sheba]]<ref name=""Ge 10:28""/><br>
................16. [[wikipedia:Ophir|Ophir]]<ref name=""Ge 10:29"">Genesis 10:29</ref><br>
................16. [[wikipedia:Havilah|Havilah]]<ref name=""Ge 10:29""/><br>
................16. [[wikipedia:Jobab|Jobab]]<ref name=""Ge 10:29""/><br>
............12. [[wikipedia:Lud son of Shem|Lud]]<ref name=""Ge 10:22""/><br>
............12. [[wikipedia:Aram, son of Shem|Aram]]<ref name=""Ge 10:22""/><br>
.............13. [[wikipedia:Uz (son of Aram)|Uz]]<ref name=""Ge 10:23"">Genesis 10:23</ref><br>
.............13. [[wikipedia:Hul|Hul]]<ref name=""Ge 10:23""/><br>
.............13. [[wikipedia:Gether|Gether]]<ref name=""Ge 10:23""/><br>
.............13. [[wikipedia:Meshech|Meshech]]<ref name=""Ge 10:23""/><br>
...........11. [[wikipedia:Ham (son of Noah)|Ham]]<ref name=""Ge 5:32""/><br>
............12. [[wikipedia:Cush (Bible)|Cush]]<ref name=""Ge 10:6"">Genesis 10:6</ref><br>
.............13. [[wikipedia:List of minor biblical figures,_L–Z#Seba|Seba]]<ref name=""Ge 10:7"">Genesis 10:7</ref><br>
.............13. [[wikipedia:Havilah|Havilah]]<ref name=""Ge 10:7""/><br>
.............13. [[wikipedia:Sabtah|Sabtah]]<ref name=""Ge 10:7""/><br>
.............13. [[wikipedia:Raamah|Raamah]]<ref name=""Ge 10:7""/><br>
..............14. [[wikipedia:Sheba|Sheba]]<ref name=""Ge 10:7""/><br>
..............14. [[wikipedia:Dedanites|Dedan]]<ref name=""Ge 10:7""/><br>
.............13. [[wikipedia:Sabteca|Sabteca]]<ref name=""Ge 10:7""/><br>
.............13. [[wikipedia:Nimrod|Nimrod]]<ref name=""Ge 10:8"">Genesis 10:8</ref><br>
............12. [[wikipedia:Mizraim|Mizraim]]<ref name=""Ge 10:6""/><br>
.............13. [[wikipedia:Ludites|Ludites]]<ref name=""Ge 10:13"">Genesis 10:13</ref><br>
.............13. [[wikipedia:Anamites|Anamites]]<ref name=""Ge 10:13""/><br>
.............13. [[wikipedia:Lehabites|Lehabites]]<ref name=""Ge 10:13""/><br>
.............13. [[wikipedia:Naphtuhites|Naphtuhites]]<ref name=""Ge 10:13""/><br>
.............13. [[wikipedia:Pathrusites|Pathrusites]]<ref name=""Ge 10:14"">Genesis 10:14</ref><br>
.............13. [[wikipedia:Casluhites|Casluhites]]<ref name=""Ge 10:14""/><br>
..............14. [[wikipedia:Philistines|Philistines]]<ref name=""Ge 10:14""/><br>
.............13. [[wikipedia:Caphtorites|Caphtorites]]<ref name=""Ge 10:14""/><br>
............12. [[wikipedia:Phut|Put]]<ref name=""Ge 10:6""/><br>
............12. [[wikipedia:Canaan|Canaan]]{{ref|Canaan|6}}<ref name=""Ge 10:6""/><br>
.............13. [[wikipedia:Sidon|Sidon]]<ref name=""Ge 10:15"">Genesis 10:15</ref><br>
.............13. [[wikipedia:Hittites|Hittites]]{{ref|Hittites|7}}<ref name=""Ge 10:15""/><br>
.............13. [[wikipedia:Jebusites|Jebusites]]<ref name=""Ge 10:16"">Genesis 10:16</ref><br>
.............13. [[wikipedia:Amorites|Amorites]]<ref name=""Ge 10:16""/><br>
.............13. [[wikipedia:Girgashites|Girgashites]]<ref name=""Ge 10:16""/><br>
.............13. [[wikipedia:Hivites|Hivites]]{{ref|Hivites|8}}<ref name=""Ge 10:17"">Genesis 10:17</ref><br>
.............13. [[wikipedia:Arkites|Arkites]]<ref name=""Ge 10:17""/><br>
.............13. [[wikipedia:List of minor biblical figures,_L–Z#Sinites|Sinites]]<ref name=""Ge 10:17""/><br>
.............13. [[wikipedia:Arvadites|Arvadites]]<ref name=""Ge 10:18"">Genesis 10:18</ref><br>
.............13. [[wikipedia:Zemarites|Zemarites]]<ref name=""Ge 10:18""/><br>
.............13. [[wikipedia:Hamathites|Hamathites]]<ref name=""Ge 10:18""/><br>
...........11. [[wikipedia:Japheth|Japheth]]<ref name=""Ge 5:32""/><br>
............12. [[wikipedia:Gomer|Gomer]]<ref name=""Ge 10:2"">Genesis 10:2</ref><br>
.............13. [[wikipedia:Ashkenaz|Ashkenaz]]<ref name=""Ge 10:3"">Gemesis 10:3</ref><br>
.............13. [[wikipedia:Riphath|Riphath]]<ref name=""Ge 10:3""/><br>
.............13. [[wikipedia:Togarmah|Togarmah]]<ref name=""Ge 10:3""/><br>
............12. [[wikipedia:Magog (Bible)|Magog]]<ref name=""Ge 10:2""/><br>
............12. [[wikipedia:Madai|Madai]]<ref name=""Ge 10:2""/><br>
............12. [[wikipedia:Javan|Javan]]<ref name=""Ge 10:2""/><br>
.............13. [[wikipedia:Elishah|Elishah]]<ref name=""Ge 10:4"">Genesis 10:4</ref><br>
.............13. [[wikipedia:Tarshish|Tarshish]]<ref name=""Ge 10:4""/><br>
.............13. [[wikipedia:Kittim|Kittim]]<ref name=""Ge 10:4""/><br>
.............13. [[wikipedia:Rodanim|Rodanim]]<ref name=""Ge 10:4""/><br>
............12. [[wikipedia:Tubal|Tubal]]<ref name=""Ge 10:2""/><br>
............12. [[wikipedia:Meshech|Meshech]]<ref name=""Ge 10:2""/><br>
............12. [[wikipedia:Tiras|Tiras]]<ref name=""Ge 10:2""/>

[[wikipedia:Genealogy of Jesus|Genealogy of Jesus]] from Zerubbabel according to [[:Category:Gospel of Matthew|Matthew]]:


.....................................................53. [[wikipedia:Zerubbabel|Zerubbabel]]<ref name=""Mat 1:1-17"">Matthew 1:1-17</ref><br>
......................................................54. [[wikipedia:Abiud|Abiud]]<ref name=""Mat 1:1-17"" /><br>
.......................................................55. [[Jews from Gospel of Matthew#Eliakim|Eliakim]]<ref name=""Mat 1:1-17"" /><br>
........................................................56. [[Jews from Gospel of Matthew#Azor|Azor]]<ref name=""Mat 1:1-17"" /><br>
.........................................................57. [[Jews from Gospel of Matthew#Zadok|Zadok]]<ref name=""Mat 1:1-17"" /><br>
..........................................................58. [[Jews from Gospel of Matthew#Achim|Achim]]<ref name=""Mat 1:1-17"" /><br>
...........................................................59. [[Jews from Gospel of Matthew#Eliud|Eliud]]<ref name=""Mat 1:1-17"" /><br>
............................................................60. [[Jews from Gospel of Matthew#Eleazar|Eleazar]]<ref name=""Mat 1:1-17"" /><br>
.............................................................61. [[Jews from Gospel of Matthew#Matthan|Matthan]]<ref name=""Mat 1:1-17"" /><br>
..............................................................62. [[Jews from Gospel of Matthew#Jacob|Jacob]]<ref name=""Mat 1:1-17"" /><br>
...............................................................63. [[wikipedia:Saint Joseph|Joseph]]<ref name=""Mat 1:1-17"" /><br>
...............................................................+ m. [[wikipedia:Mary (mother of Jesus)|Mary]]<br>
................................................................64. [[wikipedia:Jesus|Jesus]]<!-- Jesus is at least the 42nd generation after Abraham(20th generation) according to literal reading of Matthew 1 -->
==Legend==
*The '''number''' on each line denotes the generation number, starting at 1 for Adam.
* The '''+ m.''' indicates a marriage.
 * The '''?.''' indicates a descendant in which the exact lineage is unknown, and thus the generation number is unknown.

==References==
{{reflist|colwidth=20em}}
";


        private const string Best = @".1. [[Adam]]<ref>Genesis</ref><br>
.+ m. [[Eve]]<ref>Genesis 3:20</ref><br>
.. 3. [[Enoch (son of Cain)|Enoch]]<ref>Genesis 4:17</ref><br>
....4. [[Irad]]<ref name=""Ge 4:18"">Genesis 4:18</ref><br>
................................32. [[Uzziel]]<ref name=""1Ch 4:42""/><br>
................................?. [[Zoheth]]<ref name=""1Ch 4:20"">1 Chronicles 4:20</ref><br>
.........................24. Teman{{ref|Teman|1}}<ref name=""Ge 36:11"">Genesis 36:11</ref><br>
........................23. [[Reuel]] <ref name=""Ge 36:4""/><br>
...........................26. [[Aaron]]<ref name=""Ex 6:20"" /><br>
..................................33. [[Azrikam]]<ref name=""1Ch 9:14"" >1 Chronicles 9:14</ref> <br>
.................................33. [[Nun]]<ref name=""Nu 13:8"">Numbers 13:8</ref><ref name=""1Ch 7:27"">1 Chronicles 7:27</ref><br>
.............................?. [[Kenaz]]<ref name=""1Ch 4:15""/><br>
...............................................................63. [[Saint Joseph|Joseph]]<ref name=""Mat 1:1-17"" /><br>
...............................................................+ m. [[Mary (mother of Jesus)|Mary]]<br>
................................................................64. [[Jesus]]<!-- Jesus is at least the 42nd generation after Abraham (20th generation) according to literal reading of Matthew 1 -->
";
        [Test]
        public void Parse()
        {
            var des = Descendant.Parse<DescendantOfAdamAndEve>(Best);
            Assert.AreEqual(64, des[14].GenerationNumber);
            Assert.AreEqual("Jesus", des[14].Title);
            Assert.AreEqual(des[12], des[14].Father);
            Assert.AreEqual(des[13], des[14].Mother);

            Assert.AreEqual(33, des[9].GenerationNumber);
            Assert.AreEqual("Azrikam", des[9].Title);
            Assert.AreEqual("1Ch 9:14", des[9].RefName);
            Assert.AreEqual("1 Chronicles 9:14", des[9].RefCaption);
            Assert.IsNull(des[9].Father);

            Assert.AreEqual(33, des[10].GenerationNumber);
            Assert.AreEqual("Nun", des[10].Title);
            Assert.AreEqual("Nu 13:8", des[10].Ref2Name);
            Assert.AreEqual("Numbers 13:8", des[10].Ref2Caption);
            Assert.AreEqual("1Ch 7:27", des[10].RefName);
            Assert.AreEqual("1 Chronicles 7:27", des[10].RefCaption);
            Assert.IsNull(des[10].Father);

            Assert.IsTrue(des[11].GenerationNumberUnknown);
            Assert.AreEqual("Kenaz", des[11].Title);
            Assert.AreEqual("1Ch 4:15", des[11].RefName);
            Assert.IsNull(des[11].Father);

            Assert.AreEqual(63, des[12].GenerationNumber);
            Assert.AreEqual("Saint Joseph", des[12].Title);
            Assert.AreEqual("Joseph", des[12].TitleShort);
            Assert.AreEqual("Mat 1:1-17", des[12].RefName);
            Assert.IsNull(des[12].Father);

            Assert.AreEqual(63, des[13].GenerationNumber);
            Assert.AreEqual("Mary (mother of Jesus)", des[13].Title);
            Assert.AreEqual("Mary", des[13].TitleShort);
            Assert.AreEqual(des[12], des[13].Husband);
            Assert.IsNull(des[13].Father);

            Assert.AreEqual(1, des[0].GenerationNumber);
            Assert.AreEqual("Adam", des[0].Title);
            Assert.AreEqual("Genesis", des[0].RefCaption);
            Assert.AreEqual("Ge", des[0].RefName);
            Assert.IsNull(des[0].Father);

            Assert.AreEqual(1, des[1].GenerationNumber);
            Assert.AreEqual("Eve", des[1].Title);
            Assert.AreEqual(des[0], des[1].Husband);
            Assert.AreEqual("Genesis 3:20", des[1].RefCaption);
            Assert.IsNull(des[1].Father);

            Assert.AreEqual(3, des[2].GenerationNumber);
            Assert.AreEqual("Enoch (son of Cain)", des[2].Title);
            Assert.AreEqual("Enoch", des[2].TitleShort);
            Assert.AreEqual("Genesis 4:17", des[2].RefCaption);
            Assert.IsNull(des[2].Father);

            Assert.AreEqual(4, des[3].GenerationNumber);
            Assert.AreEqual("Irad", des[3].Title);
            Assert.AreEqual("Ge 4:18", des[3].RefName);
            Assert.AreEqual("Genesis 4:18", des[3].RefCaption);
            Assert.AreEqual(des[2], des[3].Father);

            Assert.AreEqual(32, des[4].GenerationNumber);
            Assert.AreEqual("Uzziel", des[4].Title);
            Assert.AreEqual("1Ch 4:42", des[4].RefName);
            Assert.IsNull(des[4].Father);

            Assert.IsTrue(des[5].GenerationNumberUnknown);
            Assert.AreEqual("Zoheth", des[5].Title);
            Assert.AreEqual("1Ch 4:20", des[5].RefName);
            Assert.AreEqual("1 Chronicles 4:20", des[5].RefCaption);
            Assert.IsNull(des[5].Father);

            Assert.AreEqual(24, des[6].GenerationNumber);
            Assert.AreEqual("Teman", des[6].OtherCaption);
            Assert.AreEqual("Ge 36:11", des[6].RefName);
            Assert.AreEqual("Genesis 36:11", des[6].RefCaption);
            Assert.IsNull(des[6].Father);

            Assert.AreEqual(23, des[7].GenerationNumber);
            Assert.AreEqual("Reuel", des[7].Title);
            Assert.AreEqual("Ge 36:4", des[7].RefName);
            Assert.IsNull(des[7].Father);

            Assert.AreEqual(26, des[8].GenerationNumber);
            Assert.AreEqual("Aaron", des[8].Title);
            Assert.AreEqual("Ex 6:20", des[8].RefName);
            Assert.IsNull(des[8].Father);
        }

        [Test]
        public void Check()
        {
            Assert.AreEqual("1Ch 9:14", new DescendantOfAdamAndEve { RefCaption = "1 Chronicles 9:14" }.Check().RefName);
        }

        [Test]
        public void ParseRefName()
        {
            var des = Descendant.Parse<DescendantOfAdamAndEve>(@"
.........................................40. [[Mattaniah]]<ref name=""1Ch 9:15""/><br>
");
            Assert.AreEqual("1Ch 9:15", des[0].RefName);

            des = Descendant.RemoveDuplicates(Descendant.Parse<DescendantOfAdamAndEve>(@"
.....................................................53. [[wikipedia:Zerubbabel|Zerubbabel]]<ref name=""1Ch 3:19"">1 Chronicles 3:19</ref><br>
.....................................................53. [[wikipedia:Zerubbabel|Zerubbabel]]<ref name=""Mat 1:1-17"">Matthew 1:1-17</ref><br>
"));
            Assert.AreEqual("1Ch 3:19", des[0].RefName);
            Assert.AreEqual("Mat 1:1-17", des[0].Ref2Name);
        }

        [Test]
        public void ParseM1()
        {
            var des = Descendant.Parse<DescendantOfAdamAndEve>(@"
...................................35. [[Rehoboam]], King of Judah<ref name=""1Ki 11:43"" /><br>
...................................+ m1. [[List_of_minor_biblical_figures,_L–Z#Mahalath|Mahalath]]<ref name=""2Ch 11:19"" /><br>
");
            Assert.AreEqual(35, des[0].GenerationNumber);
            Assert.AreEqual(des[0], des[1].Husband);
            Assert.AreEqual("List_of_minor_biblical_figures,_L–Z#Mahalath", des[1].Title);
            Assert.AreEqual("Mahalath", des[1].TitleShort);
            Assert.AreEqual("2Ch 11:19", des[1].RefName);
        }

        [Test]
        public void ParseWives()
        {
            var des = Descendant.RemoveDuplicates(Descendant.Parse<DescendantOfAdamAndEve>(@"
......................22. [[Esau]]<ref name=""Ge 25:25"">Genesis 25:25</ref><br>
.......................+ m. [[Judith]]<ref name=""Ge 26:34"">Genesis 26:34</ref><br>
.......................+ m. [[Basemath]]<ref name=""Ge 26:34""/><br>
......................22. [[Jacob]]<ref name=""Ge 25:26"">Genesis 25:26</ref><br>
......................+ m. [[Leah]]<ref name=""Ge 29:16"">Genesis 29:16</ref><br>
...........................?. [[Meshullam]]<ref name=""1Ch 9:8""/><br>
......................+ m. [[Bilhah]]<ref name=""Ge 30:4"">Genesis 30:4</ref><br>

.........................24. [[Kohath]]<ref name=""Ge 46:11""/><br>
..........................25. [[Amram]]<ref name=""Ex 6:18"">Exodus 6:18</ref><br>
..........................+ m. [[Jochebed]]<ref name=""Ex 6:20"">Exodus 6:20</ref><br>
...........................26. [[Aaron]]<ref name=""Ex 6:20"" /><br>

.......................23. [[Judah (biblical person)|Judah]]<ref name=""Ge 29:35"">Genesis 29:35</ref><br>
........................24. [[Er (biblical person)|Er]]<ref name=""Ge 38:3"">Genesis 38:3</ref><br>
........................+ m. [[Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"">Genesis 38:6</ref><br>
........................24. [[Onan]]<ref name=""Ge 38:4"">Genesis 38:4</ref><br>
........................+ m. [[Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"" /><br>
........................24. [[Shelah (son of Judah)|Shelah]]<ref name=""Ge 38:5"">Genesis 38:5</ref><br>
.........................25. [[Er (Biblical name)|Er]]<ref name=""1Ch 4:21"">1 Chronicles 4:21</ref><br>
...................... + m. [[Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"" /><br>
........................24. [[Perez (son of Judah)|Perez]]<ref name=""Ge 38:29"">Genesis 38:29</ref><br>
"));
            Assert.AreEqual(22, des[2].GenerationNumber);
            Assert.AreEqual(des[0], des[2].Husband);

            Assert.AreEqual(des[3], des[4].Husband);
            Assert.AreEqual(des[3], des[6].Husband);

            Assert.AreEqual(des[8], des[9].Husband);
            Assert.AreEqual(des[9], des[10].Mother);
            Assert.AreEqual(des[8], des[10].Father);

            Assert.AreEqual(18, des.Count);
            Assert.IsTrue(des[12].Wives.Count == 1);
            Assert.IsTrue(des[13].Husbands.Contains(des[11]));
            Assert.IsTrue(des[13].Husbands.Contains(des[12]));
            Assert.IsTrue(des[13].Husbands.Contains(des[14]));
            Assert.AreEqual(1, des[13].Kids.Count);
            Assert.AreEqual(des[17], des[13].Kids[0]);
        }

        [Test]
        public void ParseKids()
        {
            var des = Descendant.RemoveDuplicates(Descendant.Parse<DescendantOfAdamAndEve>(@"
......6. [[wikipedia:Methushael]]<ref name=""Ge 4:18""/><br>
.......7. [[wikipedia:Lamech (descendant of Cain)|Lamech]]<ref name=""Ge 4:18""/><br>
.......+ m. [[wikipedia:List of minor Biblical figures#Adah|Adah]]<ref name=""Ge 4:19"">Genesis 4:19</ref><br>
........8. [[wikipedia:Jabal (Bible)|Jabal]]<ref>Genesis 4:20</ref><br>
........8. [[wikipedia:Jubal (Bible)|Jubal]]<ref>Genesis 4:21</ref><br>
.......+ m. [[wikipedia:List of minor biblical figures,_L–Z#Zillah|Zillah]]<ref name=""Ge 4:19""/><br>

..........10. [[wikipedia:Noah]]<ref>Genesis 5:29</ref><br>
...........11. [[wikipedia:Shem]]<ref name=""Ge 5:32"">Genesis 5:32</ref><br>
...................19. [[wikipedia:Terah]]<ref name=""Ge 11:24"">Genesis 11:24</ref><br>
....................20. [[wikipedia:Abraham]]<ref name=""Ge 11:26"">Genesis 11:26</ref><br>
....................+ m. [[wikipedia:Sarah]]<ref name=""Ge 11:29"">Genesis 11:29</ref><br>
.....................21. [[wikipedia:Isaac]]<ref name=""Ge 21:3"">Genesis 21:3</ref><br>
.....................+ m. [[wikipedia:Rebekah]]<ref name=""Ge 22:23"">Genesis 22:23</ref><br>

......................22. [[wikipedia:Esau]]<ref name=""Ge 25:25"">Genesis 25:25</ref><br>
......................+ m. [[wikipedia:Judith]]<ref name=""Ge 26:34"">Genesis 26:34</ref><br>
......................+ m. [[wikipedia:Basemath]]<ref name=""Ge 26:34""/><br>
......................+ m. [[wikipedia:List of minor biblical figures#Adah|Adah]]<ref name=""Ge 36:2"">Genesis 36:2</ref><br>
.......................23. [[wikipedia:Eliphaz]]<ref name=""Ge 36:4"">Genesis 36:4</ref><br>
......................+ m. [[wikipedia:Oholibamah]]<ref name=""Ge 36:2""/><br>
.......................23. [[wikipedia:Jeush]]<ref name=""Ge 36:5"">Genesis 36:5</ref><br>
......................+ m. [[wikipedia:Basemath]]<ref name=""Ge 36:3""/><br>
.......................23. [[wikipedia:Reuel]] <ref name=""Ge 36:4""/><br>
....................20. [[wikipedia:Sarah]]<ref name=""Ge 11:29""/><br>
"));
            Assert.AreEqual(des[1], des[5].Husband);

            Assert.AreEqual(des[6], des[7].Father);
            Assert.AreEqual(1, des[10].Kids.Count);
            Assert.AreEqual(des[8], des[10].Father);
            Assert.AreEqual(des[9], des[11].Father);
            Assert.AreEqual(des[10], des[11].Mother);
            Assert.IsNull(des[10].Mother);

            Assert.AreEqual(des[13], des[17].Father); // 13 - Esau, 17 - Eliphaz
            Assert.AreEqual(des[16], des[17].Mother);
            Assert.AreEqual(des[18], des[19].Mother);
            Assert.AreEqual(des[13], des[18].Husband);

            Assert.AreEqual(des[13], des[21].Father);
            Assert.AreEqual(des[20], des[21].Mother);
        }

        [Test]
        public void ParseUnknownGeneration()
        {
            var des = Descendant.Parse<DescendantOfAdamAndEve>(@"
...............................31. [[Ishi]]<ref name=""1Ch 2:31"">1 Chronicles 2:31</ref><br>
................................32. Sheshan<ref name=""1Ch 2:31""/><br>
.................................33. [[Ahlai]]<ref name=""1Ch 2:31""/><br>
.................................33. Daughter<ref name=""1Ch 2:35"">1 Chronicles 2:35</ref><br>
.................................+ m. [[Jarha]]<ref name=""1Ch 2:35""/><br>
..................................34. [[Attai]]<ref name=""1Ch 2:35""/><br>
................................32. [[Pelatiah]]<ref name=""1Ch 4:42"">1 Chronicles 4:42</ref><br>
................................32. [[Neariah]]<ref name=""1Ch 4:42""/><br>
................................32. [[Rephaiah]]<ref name=""1Ch 4:42""/><br>
................................32. [[Uzziel]]<ref name=""1Ch 4:42""/><br>
................................?. [[Zoheth]]<ref name=""1Ch 4:20"">1 Chronicles 4:20</ref><br>
");
            Assert.AreEqual(des[0], des[10].Father);
        }

        [Test]
        public void ParseUnknownPerson()
        {
            var des = Descendant.Parse<DescendantOfAdamAndEve>(@"
........................?. Hodiah's wife<ref name=""1Ch 4:19"">1 Chronicles 4:19</ref><br>
.........................?. Unknown<ref name=""1Ch 4:19""/><br>
..........................?. [[Keilah]]<ref name=""1Ch 4:19""/><br>
.........................?. Unknown<ref name=""1Ch 4:19""/><br>
..........................?. [[Eshtamoa the Maachathite]]<ref name=""1Ch 4:19""/><br>
.......................23. [[Benjamin]]<ref name=""Ge 35:18"">Genesis 35:18</ref><br>
........................24. [[List of minor Biblical figures#Bela|Bela]]<ref name=""Ge 46:21"">Genesis 46:21</ref><br>
........................?. [[Gera]]<ref name=""Judges 3:15"">Judges 3:15</ref><br>
.........................?. [[Ehud]]<ref name=""Judges 3:15""/><br>
");
            des[3].SetTitleUnique();
            Assert.AreEqual("parent of Eshtamoa the Maachathite", des[3].TitleUnique);
            Assert.AreEqual(des[5], des[7].Father);
            Assert.AreEqual(des[7], des[8].Father);
        }

        [Test]
        public void ParseDescendants()
        {
            Assert.AreEqual(1150, Descendant.Parse<DescendantOfAdamAndEve>(S1708).Count);
        }

        [Test]
        public void SpaceInName()
        {
            var des = Descendant.Parse<DescendantOfAdamAndEve>(@"
.........................25. Ethan<ref name=""1Ch 2:6"">1 Chronicles 2:6</ref><br>
.............................28. Ethan <ref name=""1Ch 6:42""/><br>
");
            Assert.AreEqual("Ethan", des[1].TitleUnique);
        }

        [Test]
        public void Duplicates()
        {
            foreach (var descendant in _descendants)
            {
                if (descendant.Title.StartsWith("Gera"))
                {
                }
                else
                if (descendant.Title == "Zerubbabel")
                {
                    Assert.IsNotEmpty(descendant.Ref2Name);
                }
                else if (descendant.Title == "Jochebed")
                {
                    Assert.AreEqual("Amram", descendant.Husband.Title);
                    Assert.AreEqual(4, descendant.Father.Kids.Count);
                }
            }
            Assert.AreEqual(1144,_descendants.Count);
        }

        [Test]
        public void FamiliesAndGenerations()
        {
            foreach (var descendant in _descendants)
            {
                if (!descendant.IsWife)
                    Assert.AreEqual(descendant.GenerationNumber, descendant.GenerationCalculated);
                foreach (var wife in descendant.Wives)
                    Assert.IsTrue(wife.Husbands.Contains(descendant));

                foreach (var kid in descendant.Kids)
                {
                    Assert.AreEqual(descendant, descendant.IsWife ? kid.Mother : kid.Father);
                    if (descendant.IsWife)
                        Assert.IsNotNull(kid.Father);
                    else if (descendant.Wives.Count == 0)
                        Assert.IsNull(kid.Mother);

                    Assert.IsNotNull(kid.Father);
                    if (kid.Mother != null)
                    {
                        var s = kid.Father.Wives;
                        Assert.IsTrue(kid.Father.Wives.Contains(kid.Mother));
                    }
                }

                if (descendant.Title == "wikipedia:Jacob")
                {
                    Assert.AreEqual(4, descendant.Wives.Count);
                    Assert.AreEqual(13, descendant.Kids.Count);
                }

                if (descendant.TitleUnique == "wikipedia:Noah")
                {
                    Assert.AreEqual(3, descendant.Kids.Count);
                }

                if (descendant.TitleUnique == "wikipedia:List of minor Biblical figures#Adah")
                {
                    Assert.AreEqual("Ge 4:19", descendant.RefName);
                    Assert.AreEqual("Ge 36:2", descendant.Ref2Name);
                }
            }
        }
    }
}
#endif
