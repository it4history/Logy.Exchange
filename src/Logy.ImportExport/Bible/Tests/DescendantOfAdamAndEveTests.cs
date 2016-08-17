#if DEBUG
using Logy.MwAgent.DotNetWikiBot;

using NUnit.Framework;

namespace Logy.ImportExport.Bible.Tests
{
    [TestFixture]
    public class DescendantOfAdamAndEveTests
    {
        private const string S160817 = @"
'''[[Adam]]''' ({{lang-he|אָדָם}}, ''ʼĀḏām'', ""dust; man; mankind""; {{lang-ar|آدم}}, {{transl|ar|DIN|''ʼĀdam''}}) and '''[[Eve]]''' ({{lang-he|חַוָּה}}, ''Ḥawwā'', ""living one""; {{lang-ar|حواء}}, {{transl|ar|DIN|''Ḥawwāʼ''}}) were, according to the [[Book of Genesis]] of the [[Bible]], the [[List of first men or women in mythology and religion|first man and woman]] created by [[God]]. The following is an outline of their descendants as presented in the [[Bible]].

==Legend==
*The '''number''' on each line denotes the generation number, starting at 1 for Adam.
*The '''+ m.''' indicates a marriage.
*The '''?.''' indicates a descendant in which the exact lineage is unknown, and thus the generation number is unknown.

==Descendants==
{{Wikipedia-Books|Genealogy from Adam to Zerubbabel}}
Genealogy from Adam to Zerubbabel:

.1. [[Adam]]<ref>Genesis</ref><br>
.+ m. [[Eve]]<ref>Genesis 3:20</ref><br>
..2. [[Cain]]<ref>Genesis 4:1</ref><br>
.. 3. [[Enoch (son of Cain)|Enoch]]<ref>Genesis 4:17</ref><br>
....4. [[Irad]]<ref name=""Ge 4:18"">Genesis 4:18</ref><br>
.....5. [[Mehujael]]<ref name=""Ge 4:18""/><br>
......6. [[Methushael]]<ref name=""Ge 4:18""/><br>
.......7. [[Lamech (descendant of Cain)|Lamech]]<ref name=""Ge 4:18""/><br>
.......+ m. [[List of minor Biblical figures#Adah|Adah]]<ref name=""Ge 4:19"">Genesis 4:19</ref><br>
........8. [[Jabal (Bible)|Jabal]]<ref>Genesis 4:20</ref><br>
........8. [[Jubal (Bible)|Jubal]]<ref>Genesis 4:21</ref><br>
.......+ m. [[List of minor Biblical figures#Zillah|Zillah]]<ref name=""Ge 4:19""/><br>
........8. [[Tubal-Cain]]<ref name=""Ge 4:22"">Genesis 4:22</ref><br>
........8. [[Naamah (Genesis)|Naamah]]<ref name=""Ge 4:22""/><br>
..2. [[Abel]]<ref>Genesis 4:2</ref><br>
..2. [[Seth]]<ref>Genesis 4:25</ref><br>
.. 3. [[Enos (biblical figure)|Enos]]<ref>Genesis 4:26</ref><br>
....4. [[Kenan]]<ref>Genesis 5:9</ref><br>
.....5. [[Mahalalel]]<ref>Genesis 5:12</ref><br>
......6. [[Jared (biblical figure)|Jared]]<ref>Genesis 5:15</ref><br>
.......7. [[Enoch (ancestor of Noah)|Enoch]]<ref>Genesis 5:18</ref><br>
........8. [[Methuselah]]<ref>Genesis 5:21</ref><br>
.........9. [[Lamech (father of Noah)|Lamech]]<ref>Genesis 5:25</ref><br>
..........10. [[Noah]]<ref>Genesis 5:29</ref><br>
...........11. [[Shem]]<ref name=""Ge 5:32"">Genesis 5:32</ref><br>
............12. [[Elam]]<ref name=""Ge 10:22"">Genesis 10:22</ref><br>
............12. [[Asshur]]<ref name=""Ge 10:22""/><br>
............12. [[Arpachshad]]<ref name=""Ge 10:22""/><br>
.............13. [[Salah (biblical figure)|Shelah]]<ref name=""Ge 10:24"">Genesis 10:24</ref><br>
..............14. [[Eber]]<ref name=""Ge 10:24""/><br>
...............15. [[Peleg]]<ref name=""Ge 10:25"">Genesis 10:25</ref><br>
................16. [[Reu]]<ref name=""Ge 11:18"">Genesis 11:18</ref><br>
.................17. [[Serug]]<ref name=""Ge 11:20"">Genesis 11:20</ref><br>
..................18. [[Nahor, son of Serug|Nahor]]<ref name=""Ge 11:22"">Genesis 11:22</ref><br>
...................19. [[Terah]]<ref name=""Ge 11:24"">Genesis 11:24</ref><br>
....................20. [[Abraham]]<ref name=""Ge 11:26"">Genesis 11:26</ref><br>
....................+ m. [[Sarah]]<ref name=""Ge 11:29"">Genesis 11:29</ref><br>
.....................21. [[Isaac]]<ref name=""Ge 21:3"">Genesis 21:3</ref><br>
.....................+ m. [[Rebekah]]<ref name=""Ge 22:23"">Genesis 22:23</ref><br>
......................22. [[Esau]]<ref name=""Ge 25:25"">Genesis 25:25</ref><br>
.......................+ m. [[Judith]]<ref name=""Ge 26:34"">Genesis 26:34</ref><br>
.......................+ m. [[Basemath]]<ref name=""Ge 26:34""/><br>
.......................+ m. [[List of minor Biblical figures#Adah|Adah]]<ref name=""Ge 36:2"">Genesis 36:2</ref><br>
........................23. [[Eliphaz]]<ref name=""Ge 36:4"">Genesis 36:4</ref><br>
.........................24. Teman{{ref|Teman|1}}<ref name=""Ge 36:11"">Genesis 36:11</ref><br>
.........................24. [[Omar (Bible)|Omar]]<ref name=""Ge 36:11""/><br>
.........................24. [[Zepho]]<ref name=""Ge 36:11""/><br>
.........................24. [[Gatam]]<ref name=""Ge 36:11""/><br>
.........................24. [[Kenaz]]<ref name=""Ge 36:11""/><br>
........................+ m. [[Timna]]<ref name=""Ge 36:12"">Genesis 36:12</ref><br>
.........................24. [[Amalek]]<ref name=""Ge 36:12""/><br>
.......................+ m. [[Oholibamah]]<ref name=""Ge 36:2""/><br>
........................23. [[Jeush]]<ref name=""Ge 36:5"">Genesis 36:5</ref><br>
........................23. [[Jalam]]<ref name=""Ge 36:5""/><br>
........................23. [[Korah]]<ref name=""Ge 36:5""/><br>
.......................+ m. [[Basemath]]<ref name=""Ge 36:3"">Genesis 36:3</ref><br>
........................23. [[Reuel]] <ref name=""Ge 36:4""/><br>
.........................24. [[Nahath]]<ref name=""Ge 36:13"">Genesis 36:13</ref><br>
.........................24. [[Zerah]]<ref name=""Ge 36:13""/><br>
..........................25. [[Jobab]], King of Edom<ref name=""Ge 36:33"">Genesis 36:33</ref><br>
.........................24. [[Shammah]]<ref name=""Ge 36:13""/><br>
.........................24. [[Mizzah]]<ref name=""Ge 36:13""/><br>
......................22. [[Jacob]]<ref name=""Ge 25:26"">Genesis 25:26</ref><br>
......................+ m. [[Leah]]<ref name=""Ge 29:16"">Genesis 29:16</ref><br>
.......................23. [[Reuben (Bible)|Reuben]]{{ref|Reuben|2}}<ref name=""Ge 29:32"">Genesis 29:32</ref><br>
.........................24. [[Hanoch (Bible)|Hanoch]]<ref name=""Ge 46:9"">Genesis 46:9</ref><br>
.........................24. [[Pallu (Biblical figure)|Pallu]]<ref name=""Ge 46:9""/><br>
..........................25. [[Eliab]]<ref name=""Nu 26:8"">Numbers 26:8</ref><br>
...........................26. [[Dathan]]<ref name=""Nu 16:1"">Numbers 16:1</ref><br>
...........................26. [[Abiram]]<ref name=""Nu 16:1""/><br>
...........................26. [[Nemuel]]<ref name=""Nu 26:9"">Numbers 26:9</ref><br>
.........................24. [[Hezron]]<ref name=""Ge 46:9""/><br>
.........................24. [[List of minor biblical figures#Carmi|Carmi]]<ref name=""Ge 46:9""/><br>
.........................24. [[Bohan]]<ref name=""Jos 15:6"">Joshua 15:6</ref><br>
.......................23. [[Simeon (Hebrew Bible)|Simeon]]{{ref|Simeon|3}}<ref name=""Ge 29:33"">Genesis 29:33</ref><br>
.........................24. [[Jemuel]]<ref name=""Ge 46:10"">Genesis 46:10</ref><br>
.........................24. [[List of minor Biblical figures#Jamin|Jamin]]<ref name=""Ge 46:10""/><br>
.........................24. [[Ohad]]<ref name=""Ge 46:10""/><br>
.........................24. Jakin<ref name=""Ge 46:10""/><br>
.........................24. [[Zohar]]<ref name=""Ge 46:10""/><br>
.........................24. [[Shaul]]<ref name=""Ge 46:10""/>

.......................23. [[Levi]]{{ref|Levi|4}}<ref name=""Ge 29:34"">Genesis 29:34</ref><br>
.........................24. [[Gershon]]<ref name=""Ge 46:11"">Genesis 46:11</ref><br>
..........................25. [[Libni]]<ref name=""Ex 6:17"">Exodus 6:17</ref><br>
...........................26. [[Jehath]]<ref name=""1Ch 6:20"">1 Chronicles 6:20</ref><br>
............................27. [[Zimmah]]<ref name=""1Ch 6:20""/><br>
.............................28. [[Joah]]<ref name=""1Ch 6:21"">1 Chronicles 6:21</ref><br>
..............................29. [[Iddo (prophet)|Iddo]]<ref name=""1Ch 6:21""/><br>
...............................30. [[Zerah]]<ref name=""1Ch 6:21""/><br>
................................31. [[Jeatherai]]<ref name=""1Ch 6:21""/><br>
..........................25. [[Shimei]]<ref name=""Ex 6:17""/><br>
..........................25. [[Jahath]]<ref name=""1Ch 6:43"">1 Chronicles 6:43</ref><br>
...........................26. [[Shimei]]<ref name=""1Ch 6:42"">1 Chronicles 6:42</ref><br>
............................27. [[Zimmah]]<ref name=""1Ch 6:42""/><br>
.............................28. Ethan <ref name=""1Ch 6:42""/><br>
..............................29. [[Adaiah]]<ref name=""1Ch 6:41"">1 Chronicles 6:41</ref><br>
...............................30. [[Zerah]]<ref name=""1Ch 6:41""/><br>
................................31. [[Ethni]]<ref name=""1Ch 6:41""/><br>
.................................32. [[Malkijah]]<ref name=""1Ch 6:40"">1 Chronicles 6:40</ref><br>
..................................33. [[Baaseiah]]<ref name=""1Ch 6:40""/><br>
...................................34. [[Michael]]<ref name=""1Ch 6:40""/><br>
....................................35. [[Shimea]]<ref name=""1Ch 6:39"">1 Chronicles 6:39</ref><br>
.....................................36. [[Berekiah]]<ref name=""1Ch 6:39""/><br>
......................................37. [[Asaph (Bible)|Asaph]]<ref name=""1Ch 6:39""/><br>
.......................................38. [[Zikri]]<ref name=""1Ch 9:15"">1 Chronicles 9:15</ref><br>
........................................39. [[Mika (Bible)|Mika]]<ref name=""1Ch 9:15""/><br>
.........................................40. [[Mattaniah]]<ref name=""1Ch 9:15""/>

.........................24. [[Kohath]]<ref name=""Ge 46:11""/><br>
..........................25. [[Amram]]<ref name=""Ex 6:18"">Exodus 6:18</ref><br>
..........................+ m. [[Jochebed]]<ref name=""Ex 6:20"">Exodus 6:20</ref><br>
...........................26. [[Aaron]]<ref name=""Ex 6:20"" /><br>
...........................+ m. [[Elisheba]]<ref name=""Ex 6:23"">Exodus 6:23</ref><br>
............................27. [[Nadab and Abihu|Nadab]]<ref name=""Ex 6:23""/><br>
............................27. [[Nadab and Abihu|Abihu]]<ref name=""Ex 6:23""/><br>
............................27. [[Eleazar]]<ref name=""Ex 6:23""/><br>
.............................28. [[Phinehas]]<ref name=""Ex 6:25"">Exodus 6:25</ref><br>
..............................29. [[Abishua]]<ref name=""1Ch 6:4"">1 Chronicles 6:4</ref><br>
...............................30. [[Bukki]]<ref name=""1Ch 6:5"">1 Chronicles 6:5</ref><br>
................................31. [[Uzzi]]<ref name=""1Ch 6:5""/><br>
.................................32. [[Zerahiah]]<ref name=""1Ch 6:6"">1 Chronicles 6:6</ref><br>
..................................33. [[Meraioth]]<ref name=""1Ch 6:6""/><br>
...................................34. [[Amariah]]<ref name=""1Ch 6:7"">1 Chronicles 6:7</ref><br>
....................................35. [[Ahitub]]<ref name=""1Ch 6:7""/><br>
.....................................36. [[Zadok]]<ref name=""1Ch 6:8"">1 Chronicles 6:8</ref><br>
......................................37. [[Ahimaaz]]<ref name=""1Ch 6:8""/><br>
.......................................38. Azariah<ref name=""1Ch 6:9"">1 Chronicles 6:9</ref><br>
........................................39. [[Johanan]]<ref name=""1Ch 6:9""/><br>
.........................................40. Azariah<ref name=""1Ch 6:10"">1 Chronicles 6:10</ref><br>
..........................................41. [[Amariah]]<ref name=""1Ch 6:11"">1 Chronicles 6:11</ref><br>
...........................................42. [[Ahitub]]<ref name=""1Ch 6:11""/><br>
............................................43. [[Zadok]]<ref name=""1Ch 6:12"">1 Chronicles 6:12</ref><br>
.............................................44. [[Shallum]]<ref name=""1Ch 6:12""/><br>
..............................................45. [[Hilkiah]]<ref name=""1Ch 6:13"">1 Chronicles 6:13</ref><br>
...............................................46. Azariah<ref name=""1Ch 6:13""/><br>
................................................47. [[Seraiah]]<ref name=""1Ch 6:14"">1 Chronicles 6:14</ref><br>
.................................................48. [[Jehozadak]]<ref name=""1Ch 6:14""/><br>
............................27. [[Ithamar]]<ref name=""Ex 6:23""/><br>
...........................26. [[Moses]]<ref name=""Ex 6:20"" /><br>
...........................+ m. [[Zipporah]]<ref name=""Ex 2:21"">Exodus 2:21</ref><br>
............................27. [[Gershom]]<ref name=""Ex 2:22"">Exodus 2:22</ref><br>
.............................28. [[Jonathan (Judges)|Jonathan]]<ref name=""Judges 18:30"">Judges 18:30</ref><br>
............................27. [[Eliezer]]<ref name=""Ex 18:4"">Exodus 18:4</ref><br>
...........................26. [[Miriam]]<ref name=""Ex 15:20"">Exodus 15:20</ref><br>
..........................25. [[Izhar]]<ref name=""Ex 6:18""/><br>
...........................26. [[Korah]]<ref name=""Ex 6:21"">Exodus 6:21</ref><br>
............................27. [[List of minor biblical figures#Assir|Assir]]<ref name=""Ex 6:24"">Exodus 6:24</ref><br>
............................27. [[Elkanah]]<ref name=""Ex 6:24""/><br>
............................27. [[Abiasaph]]<ref name=""Ex 6:21"" /><br>
............................27. [[Ebiasaph]]<ref name=""1Ch 6:37"">1 Chronicles 6:37</ref><br>
.............................28. [[List of minor biblical figures#Assir|Assir]]<ref name=""1Ch 6:37""/><br>
..............................29. [[Tahath]]<ref name=""1Ch 6:37""/><br>
...............................30. [[Zephaniah]]<ref name=""1Ch 6:36"">1 Chronicles 6:36</ref><br>
................................31. Azariah<ref name=""1Ch 6:36""/><br>
.................................32. Joel<ref name=""1Ch 6:36""/><br>
..................................33. [[Elkanah]]<ref name=""1Ch 6:36""/><br>
...................................34. [[Amasai]]<ref name=""1Ch 6:35"">1 Chronicles 6:35</ref><br>
....................................35. [[Mahath]]<ref name=""1Ch 6:35""/><br>
.....................................36. [[Elkanah]]<ref name=""1Ch 6:35""/><br>
......................................37. [[Zuph]]<ref name=""1Ch 6:35""/><br>
.......................................38. [[Toah]]<ref name=""1Ch 6:34"">1 Chronicles 6:34</ref><br>
........................................39. [[Eliel]]<ref name=""1Ch 6:34""/><br>
.........................................40. [[Jeroham]]<ref name=""1Ch 6:34""/><br>
..........................................41. [[Elkanah]]<ref name=""1Ch 6:34""/><br>
...........................................42. [[Samuel]]<ref name=""1Ch 6:33"">1 Chronicles 6:33</ref><br>
............................................43. [[Joel (son of Samuel)|Joel]]<ref name=""1Ch 6:33""/><br>
.............................................44. [[Heman (Bible)|Heman]]<ref name=""1Ch 6:33""/><br>
..............................................45. [[Kore (Bible)|Kore]]<ref name=""1Ch 9:19"">1 Chronicles 9:19</ref><br>
...............................................46. [[Shallum]]<ref name=""1Ch 9:19""/><br>
...............................................46. [[Meshelemiah]]<ref name=""1Ch 26:1"">1 Chronicles 26:1</ref><br>
................................................47. Zechariah<ref name=""1Ch 26:2"">1 Chronicles 26:2</ref><br>
................................................47. [[Jediael]]<ref name=""1Ch 26:2""/><br>
................................................47. [[Zebadiah (Bible)|Zebadiah]]<ref name=""1Ch 26:2""/><br>
................................................47. [[Jathniel]]<ref name=""1Ch 26:2""/><br>
................................................47. [[Elam]]<ref name=""1Ch 26:3"">1 Chronicles 26:3</ref><br>
................................................47. [[Jehohanan]]<ref name=""1Ch 26:3""/><br>
................................................47. [[Eliehoenai]]<ref name=""1Ch 26:3""/><br>
...........................26. [[Nepheg]]<ref name=""Ex 6:21""/><br>
...........................26. [[Zicri]]<ref name=""Ex 6:21""/><br>
..........................25. [[Hebron (biblical figure)|Hebron]]<ref name=""Ex 6:18""/><br>
..........................25. [[Uzziel]]<ref name=""Ex 6:18""/><br>
...........................26. Mishael<ref name=""Ex 6:22"">Exodus 6:22</ref><br>
...........................26. [[Elzaphan]]<ref name=""Ex 6:22""/><br>
...........................26. [[Sithri]]<ref name=""Ex 6:22""/><br>
..........................25. [[Amminadab]]<ref name=""1Ch 6:22"">1 Chronicles 6:22</ref><br>
...........................26. [[Korah]]<ref name=""1Ch 6:22""/><br>
............................27. [[List of minor biblical figures#Assir|Assir]]<ref name=""1Ch 6:22""/><br>
.............................28. [[Elkanah]]<ref name=""1Ch 6:23"">1 Chronicles 6:23</ref><br>
..............................29. [[Ebiasaph]]<ref name=""1Ch 6:23""/><br>
...............................30. [[List of minor biblical figures#Assir|Assir]]<ref name=""1Ch 6:23""/><br>
................................31. [[Tahath]]<ref name=""1Ch 6:24"">1 Chronicles 6:24</ref><br>
.................................32. [[Uriel]]<ref name=""1Ch 6:24""/><br>
..................................33. [[Uzziah]]<ref name=""1Ch 6:24""/><br>
...................................34. [[Shaul]]<ref name=""1Ch 6:24""/><br>
..............................29. [[Amasai]]<ref name=""1Ch 6:25"">1 Chronicles 6:25</ref><br>
..............................29. [[Ahimoth]]<ref name=""1Ch 6:25""/><br>
..............................29. [[Elkanah]]<ref name=""1Ch 6:26"">1 Chronicles 6:26</ref><br>
...............................30. [[Zophai]]<ref name=""1Ch 6:26""/><br>
................................31. [[Nahath]]<ref name=""1Ch 6:26""/><br>
.................................32. [[Eliab]]<ref name=""1Ch 6:27"">1 Chronicles 6:27</ref><br>
..................................33. [[Jeroham]]<ref name=""1Ch 6:27""/><br>
...................................34. [[Elkanah]]<ref name=""1Ch 6:27""/><br>
....................................35. [[Samuel]]<ref name=""1Ch 6:27""/><br>
.....................................36. Joel<ref name=""1Ch 6:28"">1 Chronicles 6:28</ref><br>
.....................................36. [[Abijah]]<ref name=""1Ch 6:28""/><br>
.........................24. [[Merari]]{{ref|Merari|5}}<ref name=""Ge 46:11""/>

..........................25. [[List of minor Biblical figures#Mahali|Mahli]]<ref name=""Ex 6:19"">Exodus 6:19</ref><br>
...........................26. [[Libni]]<ref name=""1Ch 6:29"">1 Chronicles 6:29</ref><br>
............................27. [[Shimei]]<ref name=""1Ch 6:29""/><br>
.............................28. [[Uzzah]]<ref name=""1Ch 6:29""/><br>
..............................29. [[Shimea]]<ref name=""1Ch 6:30"">1 Chronicles 6:30</ref><br>
...............................30. [[Haggiah]]<ref name=""1Ch 6:30""/><br>
................................31. [[Asaiah]]<ref name=""1Ch 6:30""/><br>
..........................25. [[List of minor Biblical figures#Mushi|Mushi]]<ref name=""Ex 6:19""/><br>
...........................26. [[List of minor Biblical figures#Mahali|Mahli]]<ref name=""1Ch 6:47"">1 Chronicles 6:47</ref><br>
............................27. [[Shemer]]<ref name=""1Ch 6:46"">1 Chronicles 6:46</ref><br>
.............................28. [[Bani (biblical figure)|Bani]]<ref name=""1Ch 6:46""/><br>
..............................29. [[Amzi]]<ref name=""1Ch 6:46""/><br>
...............................30. [[Hilkiah]]<ref name=""1Ch 6:45"">1 Chronicles 6:45</ref><br>
................................31. [[Amaziah]]<ref name=""1Ch 6:45""/><br>
.................................32. [[Hashabiah]]<ref name=""1Ch 6:45""/><br>
..................................33. [[Malluch]]<ref name=""1Ch 6:44"">1 Chronicles 6:44</ref><br>
...................................34. [[Abdi]]<ref name=""1Ch 6:44""/> <br>
....................................35. [[Kishi (Bible)|Kishi]]<ref name=""1Ch 6:44""/> <br>
.....................................36. [[Ethan (Hebrew Bible)|Ethan]]<ref name=""1Ch 6:44""/> <br>
..................................33. [[Azrikam]]<ref name=""1Ch 9:14"" >1 Chronicles 9:14""</ref> <br>
...................................34. [[Hasshub]]<ref name=""1Ch 9:14""/> <br>
....................................35. [[Shemaiah (prophet)|Shemaiah]]<ref name=""1Ch 9:14""/> <br>
.........................24. [[Jochebed]]<ref name=""Ex 6:20""/><br>
.......................23. [[Judah (biblical person)|Judah]]<ref name=""Ge 29:35"">Genesis 29:35</ref><br>
........................24. [[Er (biblical person)|Er]]<ref name=""Ge 38:3"">Genesis 38:3</ref><br>
........................+ m. [[Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"">Genesis 38:6</ref><br>
........................24. [[Onan]]<ref name=""Ge 38:4"">Genesis 38:4</ref><br>
........................+ m. [[Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"" /><br>
........................24. [[Shelah (son of Judah)|Shelah]]<ref name=""Ge 38:5"">Genesis 38:5</ref><br>
.........................25. [[Er (Biblical name)|Er]]<ref name=""1Ch 4:21"">1 Chronicles 4:21</ref><br>
..........................26. [[Lecah]]<ref name=""1Ch 4:21""/><br>
.........................25. [[Laadah]]<ref name=""1Ch 4:21""/><br>
..........................26. [[Mareshah]]<ref name=""1Ch 4:21""/><br>
.........................25. [[Jokim]]<ref name=""1Ch 4:22"">1 Chronicles 4:22</ref><br>
.........................25. Men of Cozeba<ref name=""1Ch 4:22""/><br>
.........................25. Joash<ref name=""1Ch 4:22""/><br>
.........................25. [[Saraph]]<ref name=""1Ch 4:22""/><br>
...................... + m. [[Tamar (Genesis)|Tamar]]<ref name=""Ge 38:6"" /><br>
........................24. [[Perez (son of Judah)|Perez]]<ref name=""Ge 38:29"">Genesis 38:29</ref><br>
.........................25. [[Hezron]]<ref name=""Ge 46:12"">Genesis 46:12</ref><br>
..........................26. [[Jerahmeel]]<ref name=""1Ch 2:9"">1 Chronicles 2:9</ref><br>
...........................27. [[Ram (Biblical figure)|Ram]]<ref name=""1Ch 2:9"" /><br>
............................28. [[Maaz]]<ref name=""1Ch 2:27"">1 Chronicles 2:27</ref><br>
............................28. [[List of minor Biblical figures#Jamin|Jamin]]<ref name=""1Ch 2:27""/><br>
............................28. [[Eker]]<ref name=""1Ch 2:27""/><br>
...........................27. [[Bunah]]<ref name=""1Ch 2:25"">1 Chronicles 2:25</ref><br>
...........................27. [[Oren]]<ref name=""1Ch 2:25""/><br>
...........................27. [[Ozem]]<ref name=""1Ch 2:25""/><br>
...........................27. [[Ahijah]]<ref name=""1Ch 2:25""/><br>
...........................+ m. [[Atarah]]<ref name=""1Ch 2:25""/><br>
...........................27. [[Onam]]<ref name=""1Ch 2:25""/><br>
............................28. [[Shammai]]<ref name=""1Ch 2:28"">1 Chronicles 2:28</ref><br>
.............................29. Nadab<ref name=""1Ch 2:28""/><br>
..............................30. [[Seled]]<ref name=""1Ch 2:30"">1 Chronicles 2:30</ref><br>
..............................30. [[Appaim]]<ref name=""1Ch 2:30""/><br>
...............................31. [[Ishi]]<ref name=""1Ch 2:31"">1 Chronicles 2:31</ref><br>
................................32. Sheshan<ref name=""1Ch 2:31""/><br>
.................................33. [[Ahlai]]<ref name=""1Ch 2:31""/><br>
.................................33. Daughter<ref name=""1Ch 2:35"">1 Chronicles 2:35</ref><br>
.................................+ m. [[Jarha]]<ref name=""1Ch 2:35""/><br>
..................................34. [[Attai]]<ref name=""1Ch 2:35""/><br>
...................................35. Nathan<ref name=""1Ch 2:36"">1 Chronicles 2:36</ref><br>
....................................36. [[Zabad (Bible)|Zabad]]<ref name=""1Ch 2:36""/><br>
.....................................37. [[Ephlal]]<ref name=""1Ch 2:37"">1 Chronicles 2:37</ref><br>
......................................38. Obed<ref name=""1Ch 2:37""/><br>
.......................................39. [[Jehu]]<ref name=""1Ch 2:38"">1 Chronicles 2:38</ref><br>
........................................40. Azariah<ref name=""1Ch 2:38""/><br>
.........................................41. [[Helez]]<ref name=""1Ch 2:39"">1 Chronicles 2:39</ref><br>
..........................................42. [[Eleasah]]<ref name=""1Ch 2:39""/><br>
...........................................43. [[Sismai]]<ref name=""1Ch 2:40"">1 Chronicles 2:40</ref><br>
............................................44. [[Shallum]]<ref name=""1Ch 2:40""/><br>
.............................................45. [[Jekamiah]]<ref name=""1Ch 2:41"">1 Chronicles 2:41</ref><br>
..............................................46. [[Elishaama]]<ref name=""1Ch 2:41""/><br>
................................32. [[Pelatiah]]<ref name=""1Ch 4:42"">1 Chronicles 4:42</ref><br>
................................32. [[Neariah]]<ref name=""1Ch 4:42""/><br>
................................32. [[Rephaiah]]<ref name=""1Ch 4:42""/><br>
................................32. [[Uzziel]]<ref name=""1Ch 4:42""/><br>
................................?. [[Zoheth]]<ref name=""1Ch 4:20"">1 Chronicles 4:20</ref><br>
................................?. [[Ben-Zoheth]]<ref name=""1Ch 4:20""/><br>
.............................29. [[Abisur]]<ref name=""1Ch 2:28""/><br>
.............................+ m. [[Abihail]]<ref name=""1Ch 2:29"">1 Chronicles 2:29</ref><br>
..............................30. [[Ahban]]<ref name=""1Ch 2:29""/><br>
..............................30. [[List of minor Biblical figures#Molid|Molid]]<ref name=""1Ch 2:29""/><br>
............................28. [[Jada (biblical)|Jada]]<ref name=""1Ch 2:28""/><br>
.............................29. [[Jether]]<ref name=""1Ch 2:32"">1 Chronicles 2:32</ref><br>
.............................29. Jonathan<ref name=""1Ch 2:32""/><br>
..............................30. [[Peleth]]<ref name=""1Ch 2:33"">1 Chronicles 2:33</ref><br>
..............................30. Zaza<ref name=""1Ch 2:33""/><br>
..........................26. [[Ram (biblical figure)|Ram]]<ref name=""Ruth 4:19"">Ruth 4:19</ref><br>
...........................27. [[Amminadab]]<ref name=""Nu 1:7"">Numbers 1:7</ref><br>
............................28. [[Nahshon]]<ref name=""Nu 1:7""/><br>
.............................29. [[Salmon (biblical figure)|Salmon]]<ref name=""Ruth 4:20"">Ruth 4:20</ref><br>
..............................30. [[Boaz]]<ref name=""Ruth 4:21"">Ruth 4:21</ref><br>
..............................+ m. [[Book of Ruth|Ruth]]<ref name=""Ruth 4:13"">Ruth 4:13</ref><br>
...............................31. [[Obed (biblical figure)|Obed]]<ref name=""Ruth 4:21""/><br>
................................32. [[Jesse]]<ref name=""Ruth 4:22"">Ruth 4:22</ref><br>
.................................33. [[Eliab]]<ref name=""1Sa 17:13"">1 Samuel 17:13</ref><br>
..................................34. [[Abihail]]<ref name=""2Ch 11:18"">2 Chronicles 11:18</ref><br>
..................................+ m. [[Jerimoth]]<ref name=""2Ch 11:18""/><br>
...................................35. [[Mahalath]]<ref name=""2Ch 11:19"">2 Chronicles 11:19</ref><br>
...................................+ m. [[Rehoboam]], King of Judah<ref name=""1Ki 11:43"">1 Kings 11:43</ref><br>
.................................33. [[Abinadab]]<ref name=""1Sa 17:13""/><br>
.................................33. [[Shammah]]<ref name=""1Sa 17:13""/><br>
.................................33. [[Shimeah]]<ref name=""2Sa 13:3"">2 Samuel 13:3</ref><br>
..................................34. [[Jonadab]]<ref name=""2Sa 13:3""/><br>
.................................33. [[Nethanel]]<ref name=""1Ch 2:14"">1 Chronicles 2:14</ref><br>
.................................33. [[Raddai]]<ref name=""1Ch 2:14""/><br>
.................................33. [[Ozem]]<ref name=""1Ch 2:15"">1 Chronicles 2:15</ref><br>
.................................33. [[David]]<ref name=""Ruth 4:22""/>

.................................+ m. [[Michal]]<ref name=""1Sa 14:49""/><br>
.................................+ m. [[Ahinoam]] of Jezreel<ref name=""2Sa 3:2"">2 Samuel 3:2</ref><br>
..................................34. [[Amnon]]<ref name=""2Sa 3:2""/><br>
.................................+ m. [[Abigail]]<ref name=""2Sa 3:3"">2 Samuel 3:3</ref><br>
..................................34. [[Daniel (biblical figure)|Daniel]]<ref name=""1Ch 3:1"">1 Chronicles 3:1</ref><br>
..................................34. [[Kileab]]<ref name=""2Sa 3:3""/><br>
.................................+ m. [[Maacah]]<ref name=""2Sa 3:3""/><br>
..................................34. [[Absalom]]<ref name=""1Ch 3:2"">1 Chronicles 3:2</ref><br>
..................................34. [[Absalom]]<ref name=""2Sa 3:3""/><br>
..................................34. [[Tamar (2 Samuel)|Tamar]]<ref name=""2Sa 13:1"">2 Samuel 13:1</ref><br>
..................................34. [[Ibhar]]<ref name=""2Sa 5:15"">2 Samuel 5:15</ref><br>
..................................34. [[Elishua]]<ref name=""2Sa 5:15""/><br>
..................................34. [[Nepheg]]<ref name=""2Sa 5:15""/><br>
..................................34. [[Japhia]]<ref name=""2Sa 5:15""/><br>
..................................34. [[List of minor Biblical figures#Elishama|Elishama]]<ref name=""2Sa 5:16"">2 Samuel 5:16</ref><br>
..................................34. [[Eliada]]<ref name=""2Sa 5:16""/><br>
..................................34. [[Eliphelet]]<ref name=""2Sa 5:16""/><br>
..................................34. Nogah<ref name=""1Ch 3:7"">1 Chronicles 3:7</ref><br>
.................................+ m. [[Haggith]]<ref name=""2Sa 3:4"">2 Samuel 3:4</ref><br>
..................................34. [[Adonijah]]<ref name=""2Sa 3:4""/><br>
.................................+ m. [[Abital]]<ref name=""2Sa 3:4""/><br>
..................................34. [[Shephatiah]]<ref name=""2Sa 3:4""/><br>
.................................+ m. [[Eglah]]<ref name=""2Sa 3:5"">2 Samuel 3:5</ref><br>
..................................34. [[Ithream]]<ref name=""2Sa 3:5""/><br>
.................................+ m. [[Bathsheba]]<ref name=""1Ch 3:5"">1 Chronicles 3:5</ref><br>
..................................34. [[Shammua]]<ref name=""2Sa 5:14"">2 Samuel 5:14</ref><br>
..................................34. [[Shobab]]<ref name=""2Sa 5:14""/><br>
..................................34. [[Nathan (son of David)|Nathan]]<ref name=""2Sa 5:14""/><br>
...................................35. Azariah<ref name=""1Ki 4:5"">1 Kings 4:5</ref><br>
...................................35. [[Zabud]]<ref name=""1Ki 4:5""/><br>
..................................34. [[Solomon]], King of Judah and Israel<ref name=""2Sa 5:14""/><br>
..................................+ m. [[Naamah (wife of Solomon)|Naamah]]<ref name=""1Ki 14:21"">1 Kings 14:21</ref><br>
...................................35. [[Rehoboam]], King of Judah<ref name=""1Ki 11:43"" /><br>
...................................+ m1. [[List_of_minor_biblical_figures,_L–Z#Mahalath|Mahalath]]<ref name=""2Ch 11:19"" /><br>
....................................36. [[Jeush]]<ref name=""2Ch 11:19""/><br>
....................................36. [[Shemariah]]<ref name=""2Ch 11:19""/><br>
....................................36. [[Zaham]]<ref name=""2Ch 11:19""/><br>
...................................+ m2. [[Maakah]]<ref name=""2Ch 11:20"">2 Chronicles 11:20</ref><br>
....................................36. [[Abijah of Judah|Abijah]], King of Judah<ref name=""1Ki 14:31"">1 Kings 14:31</ref><br>
.....................................37. [[Asa of Judah|Asa]], King of Judah<ref name=""1Ki 15:9"">1 Kings 15:9</ref><br>
......................................38. [[Jehoshaphat]], King of Judah<ref name=""1Ki 15:24"">1 Kings 15:24</ref><br>
.......................................39. [[Jehoram of Judah|Jehoram]], King of Judah<ref name=""2Ki 8:16"">2 Kings 8:16</ref><br>
.......................................+ m. [[Athaliah]]<ref name=""2Ki 11:1"">2 Kings 11:1</ref><br>
........................................40. [[Ahaziah of Judah|Ahaziah]], King of Judah<ref name=""2Ki 8:24"">2 Kings 8:24</ref><br>
.........................................41. [[Jehoash of Judah|Joash]], King of Judah<ref name=""2Ki 11:1""/><br>
..........................................42. [[Amaziah of Judah|Amaziah]], King of Judah<ref name=""2Ki 14:13"">2 Kings 14:13</ref><br>
...........................................43. [[Uzziah|Azariah]], King of Judah<ref name=""2Ki 15:1"">2 Kings 15:1</ref><br>
............................................44. [[Jotham of Judah|Jotham]], King of Judah<ref name=""2Ki 15:5"">2 Kings 15:5</ref><br>
.............................................45. [[Ahaz]], King of Judah<ref name=""2Ki 15:38"">2 Kings 15:38</ref><br>
..............................................46. [[Hezekiah]], King of Judah<ref name=""2Ki 18:1"">2 Kings 18:1</ref><br>
...............................................47. [[Manasseh of Judah|Manasseh]], King of Judah<ref name=""2Ki 20:21"">2 Kings 20:21</ref><br>
................................................48. [[Amon of Judah|Amon]], King of Judah<ref name=""2Ki 21:18"">2 Kings 21:18</ref><br>
.................................................49. [[Josiah]], King of Judah<ref name=""2Ki 21:24"">2 Kings 21:24</ref><br>
..................................................50. [[Jehoahaz of Judah|Johanan]]<ref name=""1Ch 3:15"">1 Chronicles 3:15</ref><br>
..................................................50. [[Jehoiakim]], King of Judah<ref name=""2Ki 23:34"">2 Kings 23:34</ref><br>
...................................................51. [[Jeconiah|Jehoiachin]], King of Judah<ref name=""2Ki 24:6"">2 Kings 24:6</ref><br>
....................................................52. [[Shealtiel]]<ref name=""1Ch 3:17"">1 Chronicles 3:17</ref><br>
....................................................52. [[Malkiram]]<ref name=""1Ch 3:18"">1 Chronicles 3:18</ref><br>
....................................................52. [[Pedaiah]]<ref name=""1Ch 3:18""/><br>
.....................................................53. [[Zerubbabel]]<ref name=""1Ch 3:19"">1 Chronicles 3:19</ref><br>
......................................................54. [[Meshullam]]<ref name=""1Ch 3:19""/><br>
......................................................54. [[Hananiah, son of Azzur|Hananiah]]<ref name=""1Ch 3:19""/><br>
.......................................................55. [[Pelatiah]]<ref name=""1Ch 3:21"">1 Chronicles 3:21</ref><br>
.......................................................55. [[Jeshaiah]]<ref name=""1Ch 3:21""/><br>
.......................................................55. [[Rephaiah]]<ref name=""1Ch 3:21""/><br>
.......................................................55. Arnan<ref name=""1Ch 3:21""/><br>
.......................................................55. [[Obadiah]]<ref name=""1Ch 3:21""/><br>
.......................................................55. [[Shecaniah]]<ref name=""1Ch 3:21""/><br>
........................................................56. [[Shemaiah (prophet)|Shemaiah]]<ref name=""1Ch 3:22"">1 Chronicles 3:22</ref><br>
........................................................56. [[Hattush]]<ref name=""1Ch 3:22""/><br>
........................................................56. Igal<ref name=""1Ch 3:22""/><br>
........................................................56. [[Bariah]]<ref name=""1Ch 3:22""/><br>
........................................................56. [[Neariah]]<ref name=""1Ch 3:22""/><br>
.........................................................57. [[Elioenai]]<ref name=""1Ch 3:23"">1 Chronicles 3:23</ref><br>
..........................................................58. [[Hodaviah]]<ref name=""1Ch 3:24"">1 Chronicles 3:24</ref><br>
..........................................................58. [[Eliashib]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[Pelaiah]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[Akkub]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[Johanan]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[Delaiah]]<ref name=""1Ch 3:24""/><br>
..........................................................58. [[Anani]]<ref name=""1Ch 3:24""/><br>
.........................................................57. [[Hizkiah]]<ref name=""1Ch 3:23""/><br>
.........................................................57. [[Azrikam]]<ref name=""1Ch 3:23""/><br>
........................................................56. [[Shaphat]]<ref name=""1Ch 3:22""/><br>
......................................................54. [[Shelomith]]<ref name=""1Ch 3:19""/><br>
.......................................................55. [[Hashubah]]<ref name=""1Ch 3:20"">1 Chronicles 3:20</ref><br>
.......................................................55. [[Ohel (Bible)|Ohel]]<ref name=""1Ch 3:20""/><br>
.......................................................55. [[Berekiah]]<ref name=""1Ch 3:20""/><br>
.......................................................55. [[Hasadiah]]<ref name=""1Ch 3:20""/><br>
.......................................................55. [[Jushab-Hesed]]<ref name=""1Ch 3:20""/><br>
.....................................................53. [[Shimei]]<ref name=""1Ch 3:19""/><br>
....................................................52. [[Shenazzar]]<ref name=""1Ch 3:18""/><br>
....................................................52. [[Jekamiah]]<ref name=""1Ch 3:18""/><br>
....................................................52. [[Hoshama]]<ref name=""1Ch 3:18""/><br>
....................................................52. [[Nedabiah]]<ref name=""1Ch 3:18""/><br>
..................................................50. [[Zedekiah]], King of Judah<ref name=""1Ch 3:15""/><br>
..................................................50. [[Shallum]]<ref name=""1Ch 3:15""/><br>
........................................40. [[Jehosheba]]<ref name=""2Ki 11:2"">2 Kings 11:2</ref><br>
....................................36. [[Attai]]<ref name=""2Ch 11:20""/><br>
....................................36. [[Zizah|Ziza]]<ref name=""2Ch 11:20""/><br>
....................................36. [[Shelomith]]<ref name=""2Ch 11:20""/><br>
.................................33. [[Zeruiah]]<ref name=""1Ch 2:16"">1 Chronicles 2:16</ref><br>
..................................34. [[Abishai (Biblical figure)|Abishai]]<ref name=""1Ch 2:16""/><br>
..................................34. [[Joab]]<ref name=""1Ch 2:16""/><br>
..................................34. [[Asahel]]<ref name=""1Ch 2:16""/><br>
.................................33. [[Abigail]]<ref name=""1Ch 2:16""/><br>
.................................+ m. [[Jether]]<ref name=""1Ch 2:17"">1 Chronicles 2:17</ref><br>
..................................34. [[Amasa]]<ref name=""1Ch 2:17""/><br>
..........................26. [[Caleb]]<ref name=""1Ch 2:9""/><br>
...........................27. [[Mesha]]<ref name=""1Ch 2:42"">1 Chronicles 2:42</ref><br>
............................28. Ziph<ref name=""1Ch 2:42""/><br>
.............................29. [[Mareshah]]<ref name=""1Ch 2:42""/><br>
..............................30. [[Hebron (biblical figure)|Hebron]]<ref name=""1Ch 2:42""/><br>
...............................31. [[Korah]]<ref name=""1Ch 2:43"">1 Chronicles 2:43</ref><br>
...............................31. Tappuah<ref name=""1Ch 2:43""/><br>
...............................31. Rekem<ref name=""1Ch 2:43""/><br>
................................32. [[Shammai]]<ref name=""1Ch 2:44"">1 Chronicles 2:44</ref><br>
.................................33. Maon<ref name=""1Ch 2:45"">1 Chronicles 2:45</ref><br>
..................................34. [[Beth Zur]]<ref name=""1Ch 2:45""/><br>
...............................31. [[Shema]]<ref name=""1Ch 2:43""/><br>
................................32. [[Raham]]<ref name=""1Ch 2:44""/><br>
.................................33. [[Jorkeam]]<ref name=""1Ch 2:44""/><br>
...........................27. [[Jesher]]<ref name=""1Ch 2:18"">1 Chronicles 2:18</ref><br>
...........................27. [[Shobab]]<ref name=""1Ch 2:18""/><br>
...........................27. Ardon<ref name=""1Ch 2:18""/><br>
..........................+ m. [[Ephrath]]<ref name=""1Ch 2:19"">1 Chronicles 2:19</ref><br>
............................28. [[Hur (Bible)|Hur]]<ref name=""Ex 31:2"">Exodus 31:2</ref><br>
.............................29. [[Uri (Bible)|Uri]]<ref name=""Ex 31:2""/><br>
..............................30. [[Bezalel]]<ref name=""Ex 31:2""/><br>
.............................29. [[Shobal]]<ref name=""1Ch 2:50"">1 Chronicles 2:50</ref><br>
..............................30. [[Kiriath Jearim]]<ref name=""1Ch 2:50""/><br>
...............................31. [[Ithrites]]<ref name=""1Ch 2:53"">1 Chronicles 2:53</ref><br>
...............................31. [[Puthites]]<ref name=""1Ch 2:53""/><br>
...............................31. [[Shumathites]]<ref name=""1Ch 2:53""/><br>
...............................31. [[Mishraites]]<ref name=""1Ch 2:53""/><br>
................................?. [[Zorathites]]<ref name=""1Ch 2:53""/><br>
................................?. [[Eshtaolites]]<ref name=""1Ch 2:53""/><br>
..............................30. [[Haroeh]]<ref name=""1Ch 2:52"">1 Chronicles 2:52</ref><br>
..............................30. [[Manahathite]]<ref name=""1Ch 2:52""/><br>
..............................30. [[Reaiah]]<ref name=""1Ch 4:2"">1 Chronicles 4:2</ref><br>
...............................31. [[Jahath]]<ref name=""1Ch 4:2""/><br>
................................32. [[Ahumai]]<ref name=""1Ch 4:2""/><br>
................................32. [[Lahad]]<ref name=""1Ch 4:2""/><br>
.............................29. [[Salma]]<ref name=""1Ch 2:51"">1 Chronicles 2:51</ref><br>
..............................30. [[Bethlehem]]<ref name=""1Ch 2:51""/><br>
..............................?. Netophathites<ref name=""1Ch 2:54"">1 Chronicles 2:54</ref><br>
..............................?. Atroth Beth Joab<ref name=""1Ch 2:54""/><br>
..............................?. Half the Manahathites<ref name=""1Ch 2:54""/><br>
..............................?. Zorites<ref name=""1Ch 2:54""/><br>
..............................?. Tirathites<ref name=""1Ch 2:55"">1 Chronicles 2:55</ref><br>
..............................?. Shimeathites<ref name=""1Ch 2:55""/><br>
..............................?. Sucathites<ref name=""1Ch 2:55""/><br>
.............................29. [[Hareph]]<ref name=""1Ch 2:51""/><br>
..............................30. [[Beth Gader]]<ref name=""1Ch 2:51""/><br>
.............................?. [[Etam (Bible)|Etam]]<ref name=""1Ch 4:3"">1 Chronicles 4:3</ref><br>
..............................?. [[Jezreel]]<ref name=""1Ch 4:3""/><br>
..............................?. [[Ishma]]<ref name=""1Ch 4:3""/><br>
..............................?. [[Idbash]]<ref name=""1Ch 4:3""/><br>
..............................?. [[Hazzelelponi]]<ref name=""1Ch 4:3""/><br>
.............................?. [[Penuel]]<ref name=""1Ch 4:4"">1 Chronicles 4:4</ref><br>
..............................?. [[Gedor]]<ref name=""1Ch 4:4""/><br>
..............................?. [[Ezer]]<ref name=""1Ch 4:4""/><br>
...............................?. [[Hushah]]<ref name=""1Ch 4:4""/><br>
..........................+ m. [[Ephah]]<ref name=""1Ch 2:46"">1 Chronicles 2:46</ref><br>
...........................27. [[Haran]]<ref name=""1Ch 2:46""/><br>
............................28. [[Gezez]]<ref name=""1Ch 2:46""/><br>
...........................27. Moza<ref name=""1Ch 2:46""/><br>
...........................27. [[Gezez]]<ref name=""1Ch 2:46""/><br>
..........................+ m. [[Maacah]]<ref name=""1Ch 2:48"">1 Chronicles 2:48</ref><br>
...........................27. [[Sheber]]<ref name=""1Ch 2:48""/><br>
...........................27. [[Tirhanah]]<ref name=""1Ch 2:48""/><br>
...........................27. [[Shaaph]]<ref name=""1Ch 2:49"">1 Chronicles 2:49</ref><br>
............................28. [[Madmannah]]<ref name=""1Ch 2:49""/><br>
...........................27. [[Sheva (Bible)|Sheva]]<ref name=""1Ch 2:49""/><br>
............................28. [[Macbenah]]<ref name=""1Ch 2:49""/><br>
............................28. [[Gibea]]<ref name=""1Ch 2:49""/><br>
...........................27. [[Acsah]]<ref name=""1Ch 2:49""/><br>
..........................26. [[Segub]]<ref name=""1Ch 2:21"">1 Chronicles 2:21</ref><br>
...........................27. [[Jair]], Judge of Israel<ref name=""1Ch 2:22"">1 Chronicles 2:22</ref><br> 
.........................+ m. [[Abijah]]<ref name=""1Ch 2:24"">1 Chronicles 2:24</ref><br> 
..........................26. [[Ashhur]]<ref name=""1Ch 2:24""/><br> 
...........................27. Tekoa<ref name=""1Ch 2:24""/><br> 
..........................?. [[Jephunneh]]<ref name=""Nu 13:6"">Numbers 13:6</ref><br>
...........................?. [[Caleb]]<ref name=""Nu 13:6""/><br>
............................?. [[Acsah]]<ref name=""Jos 15:7"">Joshua 15:7</ref><br>
............................?. Iru<ref name=""1Ch 4:15"">1 Chronicles 4:15</ref><br> 
............................?. Elah<ref name=""1Ch 4:15""/><br> 
.............................? [[Kenaz]]<ref name=""1Ch 4:15""/><br> 
............................?. [[Naam]]<ref name=""1Ch 4:15""/><br> 
.........................25. [[Hamul]]<ref name=""Ge 46:12""/><br>
.........................?. [[Bani (biblical figure)|Bani]]<ref name=""1Ch 9:4"">1 Chronicles 9:4</ref><br> 
..........................?. [[Imri]]<ref name=""1Ch 9:4""/><br> 
...........................?. [[Omri]]<ref name=""1Ch 9:4""/><br> 
............................?. [[Ammihud]]<ref name=""1Ch 9:4""/><br> 
.............................?. [[Uthai]]<ref name=""1Ch 9:4""/><br> 
........................24. [[Zerah]]<ref name=""Ge 38:30"">Genesis 38:30</ref><br>
.........................25. Zimri<ref name=""Jos 7:1"">Joshua 7:1</ref><br>
..........................26. [[List of minor biblical figures#Carmi|Carmi]]<ref name=""Jos 7:1""/><br>
...........................27. [[Achan (Biblical figure)|Achan]]<ref name=""Jos 7:1""/><br>
.........................25. Ethan<ref name=""1Ch 2:6"">1 Chronicles 2:6</ref><br>
..........................26. Azariah<ref name=""1Ch 2:8"">1 Chronicles 2:8</ref><br>
.........................25. Heman<ref name=""1Ch 2:6""/><br>
.........................25. [[Calcol]]<ref name=""1Ch 2:6""/><br>
.........................25. Darda<ref name=""1Ch 2:6""/><br>
.........................?. [[Jeuel]]<ref name=""1Ch 9:6"">1 Chronicles 9:6</ref><br>
........................?. [[Ashhur]]<ref name=""1Ch 4:5"">1 Chronicles 4:5</ref><br>
.........................?. Tekoa<ref name=""1Ch 4:5""/><br>
........................+ m. [[Helah]]<ref name=""1Ch 4:5""/><br>
.........................?. [[Zereth]]<ref name=""1Ch 4:7"">1 Chronicles 4:7</ref><br>
.........................?. [[Zohar]]<ref name=""1Ch 4:7""/><br>
.........................?. [[Ethnan]]<ref name=""1Ch 4:7""/><br>
.........................?. Koz<ref name=""1Ch 4:8"">1 Chronicles 4:8</ref><br>
..........................?. [[Anub]]<ref name=""1Ch 4:8""/><br>
..........................?. [[Hazzobebah]]<ref name=""1Ch 4:8""/><br>
..........................?. Harum<ref name=""1Ch 4:8""/><br>
...........................?. [[Aharhel]]<ref name=""1Ch 4:8""/><br>
........................+ m. [[Naarah]]<ref name=""1Ch 4:5""/><br>
.........................?. [[Ahuzzam]]<ref name=""1Ch 4:6"">1 Chronicles 4:6</ref><br>
.........................?. [[Hepher]]<ref name=""1Ch 4:6""/><br>
.........................?. [[Temeni]]<ref name=""1Ch 4:6""/><br>
.........................?. [[Haahashtari]]<ref name=""1Ch 4:6""/><br>
........................?. [[Jabez (Bible)|Jabez]]<ref name=""1Ch 4:9"">1 Chronicles 4:9</ref><br>
........................?. [[Shuhah]]<ref name=""1Ch 4:11"">1 Chronicles 4:11</ref><br>
........................?. [[Kelub]]<ref name=""1Ch 4:11""/><br>
.........................?. [[Mehir]]<ref name=""1Ch 4:11""/><br>
..........................?. [[Eshton]]<ref name=""1Ch 4:11""/><br>
...........................?. [[Beth Rapha]]<ref name=""1Ch 4:12"">1 Chronicles 4:12</ref><br>
...........................?. [[Paseah]]<ref name=""1Ch 4:12""/><br>
...........................?. [[Tehinnah]]<ref name=""1Ch 4:12""/><br>
............................?. [[Ir Nahash]]<ref name=""1Ch 4:12""/><br>
........................?. [[Kenaz]]<ref name=""1Ch 4:13"">1 Chronicles 4:13</ref><br>
.........................?. [[Othniel]], Judge of Israel<ref name=""1Ch 4:13""/><br>
..........................?. [[Hathath]]<ref name=""1Ch 4:13""/><br>
..........................?. [[Meonothai]]<ref name=""1Ch 4:13""/><br>
...........................?. [[Ophrah]]<ref name=""1Ch 4:14"">1 Chronicles 4:14</ref><br>
.........................?. [[Seraiah]]<ref name=""1Ch 4:13""/><br>
..........................?. [[Joab]]<ref name=""1Ch 4:14""/><br>
...........................?. [[Ge Harashim]]<ref name=""1Ch 4:14""/><br>
........................?. [[Jehallelel]]<ref name=""1Ch 4:16"">1 Chronicles 4:16</ref><br>
.........................?. [[Ziph (son of Jehallelel)|Ziph]]<ref name=""1Ch 4:16""/><br>
.........................?. [[Ziphah]]<ref name=""1Ch 4:16""/><br>
.........................?. [[Tiria]]<ref name=""1Ch 4:16""/><br>
.........................?. [[Asarel]]<ref name=""1Ch 4:16""/><br>
........................?. [[Ezrah]]<ref name=""1Ch 4:17"">1 Chronicles 4:17</ref><br>
.........................?. [[Jether]]<ref name=""1Ch 4:17""/><br>
.........................?. [[Mered]]<ref name=""1Ch 4:17""/><br>
.........................+ m. [[Bithiah]], daughter of Pharaoh<ref name=""1Ch 4:18"">1 Chronicles 4:18</ref><br>
..........................?. [[Miriam]]<ref name=""1Ch 4:17""/><br>
..........................?. [[Shammai]]<ref name=""1Ch 4:17""/><br>
..........................?. [[Ishbah]]<ref name=""1Ch 4:17""/><br>
...........................?. [[Eshtamoa (son of Ishbah)|Eshtemoa]]<ref name=""1Ch 4:17""/><br>
.........................+ m. Unknown<ref name=""1Ch 4:18""/><br>
..........................?. [[Jered]]<ref name=""1Ch 4:18""/><br>
...........................?. [[Gedor]]<ref name=""1Ch 4:18""/><br>
..........................?. [[Hebre (son of Gedor)|Heber]]<ref name=""1Ch 4:18""/><br>
...........................?. [[Soco (son of Hebre)|Soco]]<ref name=""1Ch 4:18""/><br>
..........................?. [[Jekuthiel]]<ref name=""1Ch 4:18""/><br>
...........................?. [[Zanoah]]<ref name=""1Ch 4:18""/><br>
.........................?. [[Epher]]<ref name=""1Ch 4:17""/><br>
.........................?. [[List of minor Biblical figures#Jalon|Jalon]]<ref name=""1Ch 4:17""/><br>
........................?. Hodiah's wife<ref name=""1Ch 4:19"">1 Chronicles 4:19</ref><br>
.........................?. Unknown<ref name=""1Ch 4:19""/><br>
..........................?. [[Keilah]]<ref name=""1Ch 4:19""/><br>
.........................?. Unknown<ref name=""1Ch 4:19""/><br>
..........................?. [[Eshtamoa the Maachathite|Eshtemoa]]<ref name=""1Ch 4:19""/><br>
........................?. [[Naham]]<ref name=""1Ch 4:19""/><br>
........................?. [[List of minor Biblical figures#Shimon|Shimon]]<ref name=""1Ch 4:20""/><br>
.........................?. [[Amnon]]<ref name=""1Ch 4:20""/><br>
.........................?. [[Rinnah]]<ref name=""1Ch 4:20""/><br>
.........................?. [[Ben-Hanan]]<ref name=""1Ch 4:20""/><br>
.........................?. [[Tilon]]<ref name=""1Ch 4:20""/><br>
.......................23. [[Issachar]]<ref name=""Ge 30:18"">Genesis 30:18</ref><br>
........................24. [[List of minor Biblical figures#Tola|Tola]]<ref name=""Ge 46:13"">Genesis 46:13</ref><br>
.........................25. [[Uzzi]]<ref name=""1Ch 7:2"">1 Chronicles 7:2</ref><br>
...........................26. [[Izrahiah]]<ref name=""1Ch 7:3"">1 Chronicles 7:3</ref><br>
............................27. [[Michael]]<ref name=""1Ch 7:3""/><br>
............................27. [[Obadiah]]<ref name=""1Ch 7:3""/><br>
............................27. [[Joel (prophet)|Joel]]<ref name=""1Ch 7:3""/><br>
............................27. [[Isshiah]]<ref name=""1Ch 7:3""/><br>
.........................25. [[Rephaiah]]<ref name=""1Ch 7:2""/><br>
.........................25. [[Jeriel]]<ref name=""1Ch 7:2""/><br>
.........................25. [[Jahmai]]<ref name=""1Ch 7:2""/><br>
.........................25. [[Ibsam]]<ref name=""1Ch 7:2""/><br>
.........................25. [[Samuel]]<ref name=""1Ch 7:2""/><br>
........................24. [[Puah]]<ref name=""Ge 46:13""/><br>
........................24. [[Jashub]]<ref name=""Ge 46:13""/><br>
........................24. [[List of minor Biblical figures#Shimron|Shimron]]<ref name=""Ge 46:13""/><br>
........................?. [[Zuar]]<ref name=""Nu 1:8"">Numbers 1:8</ref><br>
.........................?. [[Nethanel]]<ref name=""Nu 1:8""/><br>
........................?. [[Joseph (son of Jacob)|Joseph]]<ref name=""Nu 13:7"">Numbers 13:7</ref><br>
.........................?. [[List of minor Biblical figures#Igal|Igal]]<ref name=""Nu 13:7""/><br>
........................?. [[Azzan]]<ref name=""Nu 34:26"">Numbers 34:26</ref><br>
.........................?. [[List of minor Biblical figures#Paltiel|Paltiel]]<ref name=""Nu 34:26""/><br>
........................?. [[Dodo]]<ref name=""Judges 10:1"">Judges 10:1</ref><br>
.........................?. [[Puah]]<ref name=""Judges 10:1""/><br>
..........................?. [[Tola (Bible)|Tola]], Judge of Israel<ref name=""Judges 10:1""/><br>
.......................23. [[Zebulun]]<ref name=""Ge 30:20"">Genesis 30:20</ref><br>
........................24. [[Sered]]<ref name=""Ge 46:14"">Genesis 46:14</ref><br>
........................24. [[Elon]]<ref name=""Ge 46:14""/><br>
........................24. [[Jahleel]]<ref name=""Ge 46:14""/><br>
........................?. [[Helon]]<ref name=""Nu 1:9"">Numbers 1:9</ref><br>
.........................?. [[Eliab]]<ref name=""Nu 1:9""/><br>
........................?. [[Sodi]]<ref name=""Nu 13:10"">Numbers 13:10</ref><br>
.........................?. [[Gaddiel]]<ref name=""Nu 13:10""/><br>
........................?. [[Parnach]]<ref name=""Nu 34:25"">Numbers 34:25</ref><br>
.........................?. [[Elizaphan]]<ref name=""Nu 34:25""/><br>
.......................23. [[Dinah]]<ref name=""Ge 30:21"">Genesis 30:21</ref><br>
......................+ m. [[Rachel]]<ref name=""Ge 29:6"">Genesis 29:6</ref><br>
.......................23. [[Joseph (son of Jacob)|Joseph]]<ref name=""Ge 30:24"">Genesis 30:24</ref><br>
.......................+ m. [[Asenath]]<ref name=""Ge 46:20"">Genesis 46:20</ref><br>
........................24. [[Manasseh (tribal patriarch)|Manasseh]]<ref name=""Ge 46:20""/><br>
.........................25. [[Makir]]<ref name=""Nu 27:1"">Numbers 27:1</ref><br>
..........................26. [[Gilead]]<ref name=""Nu 27:1""/><br>
...........................27. [[Hepher]]<ref name=""Nu 27:1""/><br>
............................28. [[Zelophehad]]<ref name=""Nu 27:1""/><br>
.............................29. [[Mahlah]]<ref name=""Nu 27:1""/><br>
.............................29. [[Noah]]<ref name=""Nu 27:1""/><br>
.............................29. [[Hoglah]]<ref name=""Nu 27:1""/><br>
.............................29. [[Milcah]]<ref name=""Nu 27:1""/><br>
.............................29. [[Tirzah]]<ref name=""Nu 27:1""/><br>
..........................26. [[Hammoleketh]]<ref name=""1Ch 7:18"">1 Chronicles 7:18</ref><br>
...........................27. [[Ishhod]]<ref name=""1Ch 7:18""/><br>
...........................27. [[Abiezer]]<ref name=""1Ch 7:18""/><br>
...........................27. [[Mahlah]]<ref name=""1Ch 7:18""/><br>
.........................+ m. [[Maacah]]<ref name=""1Ch 7:16"">1 Chronicles 7:16</ref><br>
..........................26. [[Peresh]]<ref name=""1Ch 7:16""/><br>
...........................27. [[Ulam (Bible)|Ulam)]]<ref name=""1Ch 7:16""/><br>
............................28. [[Bedan]]<ref name=""1Ch 7:17"">1 Chronicles 7:17</ref><br>
...........................27. [[Rakem]]<ref name=""1Ch 7:16""/><br>
..........................26. [[Sheresh]]<ref name=""1Ch 7:16""/><br>
.........................25. [[Maacah]]<ref name=""1Ch 7:15"">1 Chronicles 7:15</ref><br>
.........................25. [[Jair]]<ref name=""1Ki 4:13"">1 Kings 4:13</ref><br>
.........................?. [[Pedahzur]]<ref name=""Nu 1:10"">Numbers 1:10</ref><br>
..........................?. [[Gamaliel]]<ref name=""Nu 1:10""/><br>
.........................?. [[List of minor biblical figures#Susi|Susi]]<ref name=""Nu 13:11"">Numbers 13:11</ref><br>
..........................?. Gaddi<ref name=""Nu 13:11""/><br>
..........................?. [[Ephod]]<ref name=""Nu 34:23"">Numbers 34:23</ref><br>
..........................?. [[Hanniel]]<ref name=""Nu 34:23""/><br>
.........................?. [[Epher]]<ref name=""1Ch 5:24"">1 Chronicles 5:24</ref><br>
.........................?. [[Ishi]]<ref name=""1Ch 5:24""/><br>
.........................?. [[Eliel]]<ref name=""1Ch 5:24""/><br>
.........................?. [[List of minor Biblical figures#Azriel|Azriel]]<ref name=""1Ch 5:24""/><br>
.........................?. [[Jeremiah]]<ref name=""1Ch 5:24""/><br>
.........................?. [[Hodaviah]]<ref name=""1Ch 5:24""/><br>
.........................?. [[Jahdiel]]<ref name=""1Ch 5:24""/><br>
.........................?. [[Asriel]]<ref name=""1Ch 7:14"">1 Chronicles 7:14</ref><br>
.........................?. [[Shemida]]<ref name=""1Ch 7:19"">1 Chronicles 7:19</ref><br>
..........................?. [[Ahian]]<ref name=""1Ch 7:19""/><br>
..........................?. [[Shechem]]<ref name=""1Ch 7:19""/><br>
..........................?. [[Likhi]]<ref name=""1Ch 7:19""/><br>
..........................?. [[Aniam]]<ref name=""1Ch 7:19""/><br>
........................24. [[Ephraim]]<ref name=""Ge 46:20""/><br>
.........................25. [[Ezer]]<ref name=""1Ch 7:21"">1 Chronicles 7:21</ref><br>
.........................25. [[Elead]]<ref name=""1Ch 7:21""/><br>
.........................25. [[List of minor biblical figures#Beriah|Beriah]]<ref name=""1Ch 7:23"">1 Chronicles 7:23</ref><br>
..........................26. [[Sheerah]]<ref name=""1Ch 7:24"">1 Chronicles 7:24</ref><br>
..........................26. [[Rephah]]<ref name=""1Ch 7:25"">1 Chronicles 7:25</ref><br>
...........................27. [[Resheph]]<ref name=""1Ch 7:25""/><br>
............................28. [[Telah]]<ref name=""1Ch 7:25""/><br>
.............................29. [[Tahan]]<ref name=""1Ch 7:25""/><br>
..............................30. Ladan<ref name=""1Ch 7:26"">1 Chronicles 7:26</ref><br>
...............................31. [[Ammihud]]<ref name=""1Ch 7:26""/><br>
................................32. [[List of minor Biblical figures#Elishama|Elishama]]<ref name=""1Ch 7:26""/><br>
.................................33. [[Nun]]<ref name=""Nu 13:8"">Numbers 13:8</ref><ref name=""1Ch 7:27"">1 Chronicles 7:27</ref><br>
..................................34. [[Joshua]]<ref name=""Nu 13:8""/><ref name=""1Ch 7:27""/><br> 
.........................?. [[Ammihud]]<ref name=""Nu 1:10""/><br>
..........................?. [[List of minor Biblical figures#Elishama|Elishama]]<ref name=""Nu 1:10""/><br>
.........................?. [[Shiphtan]]<ref name=""Nu 34:24"">Numbers 34:24</ref><br>
..........................?. [[Kemuel]]<ref name=""Nu 34:24""/><br>
.........................?. [[Zuph]]<ref name=""1Sa 1:1"">1 Samuel 1:1</ref><br>
..........................?. [[List of minor Biblical figures#Tohu|Tohu]]<ref name=""1Sa 1:1""/><br>
...........................?. [[Elihu (Job)|Elihu]]<ref name=""1Sa 1:1""/><br>
............................?. [[Jeroham]]<ref name=""1Sa 1:1""/><br>
.............................?. [[Elkanah]]<ref name=""1Sa 1:1""/><br>
.............................+ m. [[Hannah (Bible)|Hannah]]<ref name=""1Sa 1:8"">1 Samuel 1:8</ref><br>
..............................?. [[Samuel]]<ref name=""1Sa 1:20"">1 Samuel 1:20</ref><br>
.........................?. [[Nebat]]<ref name=""1Ki 11:26"">1 Kings 11:26</ref><br>
..........................?. [[Jeroboam]]<ref name=""1Ki 11:26""/><br>
...........................?. [[List of minor Biblical figures#Nadab|Nadab]]<ref name=""1Ki 14:20"">1 Kings 14:20</ref><br>
.........................?. [[Shuthelah]]<ref name=""1Ch 7:20"">1 Chronicles 7:20</ref><br>
..........................?. [[Bered]]<ref name=""1Ch 7:20""/><br>
...........................?. [[Tahath]]<ref name=""1Ch 7:20""/><br>
............................?. [[Eleadah]]<ref name=""1Ch 7:20""/><br>
.............................?. [[Tahath]]<ref name=""1Ch 7:20""/><br>
..............................?. [[Zabad (Bible)|Zabad]]<ref name=""1Ch 7:20""/><br>
...............................?. [[Shuthelah]]<ref name=""1Ch 7:20""/><br>
.......................23. [[Benjamin]]<ref name=""Ge 35:18"">Genesis 35:18</ref><br>
........................24. [[List of minor Biblical figures#Bela|Bela]]<ref name=""Ge 46:21"">Genesis 46:21</ref><br>
.........................25. [[Ezbon]]<ref name=""1Ch 7:7"">1 Chronicles 7:7</ref><br>
.........................25. [[Uzzi]]<ref name=""1Ch 7:7""/><br>
.........................25. [[Uzziel]]<ref name=""1Ch 7:7""/><br>
.........................25. [[Jerimoth]]<ref name=""1Ch 7:7""/><br>
.........................25. [[Iri]]<ref name=""1Ch 7:7""/><br>
.........................25. [[Addar]]<ref name=""1Ch 8:3"">1 Chronicles 8:3</ref><br>
.........................25. [[Gera]]<ref name=""1Ch 8:3""/><br>
.........................25. [[Abihud]]<ref name=""1Ch 8:3""/><br>
.........................25. [[Abishua]]<ref name=""1Ch 8:4"">1 Chronicles 8:4</ref><br>
.........................25. [[Naaman]]<ref name=""1Ch 8:4""/><br>
.........................25. [[Ahoah]]<ref name=""1Ch 8:4""/><br>
.........................25. [[Gera]]<ref name=""1Ch 8:5"">1 Chronicles 8:5</ref><br>
.........................25. [[Shephuphan]]<ref name=""1Ch 8:5""/><br>
.........................25. [[Huram (biblical figure)|Huram]]<ref name=""1Ch 8:5""/><br>
........................24. [[Ashbel]]<ref name=""Ge 46:21""/><br>
........................24. [[Aharah]]<ref name=""1Ch 8:1"">1 Chronicles 8:1</ref><br>
........................24. [[Nohah]]<ref name=""1Ch 8:2"">1 Chronicles 8:2</ref><br>
........................24. [[Rapha (biblical figure)|Rapha]]<ref name=""1Ch 8:2""/><br>
........................24. [[Beker]]<ref name=""Ge 46:21""/><br>
.........................25. [[Zemirah]]<ref name=""1Ch 7:8"">1 Chronicles 7:8</ref><br>
.........................25. [[List of minor Biblical figures#Joash|Joash]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Eliezer]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Elioenai]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Omri]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Jeremoth]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Abijah]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Anathoth]]<ref name=""1Ch 7:8""/><br>
.........................25. [[Alemeth]]<ref name=""1Ch 7:8""/><br>
........................24. [[Gera]]<ref name=""Ge 46:21""/><br>
........................24. [[Naaman]]<ref name=""Ge 46:21""/><br>
........................24. [[Minor characters in the Book of Genesis|Ehi]]<ref name=""Ge 46:21""/><br>
........................24. [[List of minor Biblical figures#Rosh|Rosh]]<ref name=""Ge 46:21""/><br>
........................24. [[Muppim]]<ref name=""Ge 46:21""/><br>
........................24. [[Huppim]]<ref name=""Ge 46:21""/><br>
........................24. [[List of minor Biblical figures#Ard|Ard]]<ref name=""Ge 46:21""/><br>
........................24. [[Jediael]]<ref name=""1Ch 7:6"">1 Chronicles 7:6</ref><br>
.........................25. [[Bilhan]]<ref name=""1Ch 7:10"">1 Chronicles 7:10</ref><br>
..........................26. [[Jeush]]<ref name=""1Ch 7:11"">1 Chronicles 7:11</ref><br>
..........................26. [[Benjamin]]<ref name=""1Ch 7:11""/><br>
..........................26. [[Ehud]]<ref name=""1Ch 7:11""/><br>
...........................27. [[Naaman]]<ref name=""1Ch 8:7"">1 Chronicles 8:7</ref><br>
...........................27. [[Ahijah]]<ref name=""1Ch 8:7""/><br>
...........................27. [[Gera]]<ref name=""1Ch 8:7""/><br>
............................28. [[Uzza]]<ref name=""1Ch 8:7""/><br>
............................28. [[Ahihud]]<ref name=""1Ch 8:7""/><br>
..........................26. [[Kenaanah]]<ref name=""1Ch 7:11""/><br>
..........................26. [[Zethan]]<ref name=""1Ch 7:11""/><br>
..........................26. [[Tarshish]]<ref name=""1Ch 7:11""/><br>
..........................26. [[Ahishahar]]<ref name=""1Ch 7:11""/><br>
........................?. [[Gideoni]]<ref name=""Nu 1:11"">Numbers 1:11</ref><br>
.........................?. [[Abidan]]<ref name=""Nu 1:11""/><br>
........................?. [[Raphu]]<ref name=""Nu 13:9"">Numbers 13:9</ref><br>
.........................?. [[Palti]]<ref name=""Nu 13:9""/><br>
........................?. [[Kison]]<ref name=""Nu 34:21"">Numbers 34:21</ref><br>
.........................?. [[Elidad]]<ref name=""Nu 34:21""/><br>
........................?. [[Gera]]<ref name=""Judges 3:15"">Judges 3:15</ref><br>
.........................?. [[Ehud]]<ref name=""Judges 3:15""/><br>
..........................?. [[Naaman]]<ref name=""1Ch 8:7"" /><br>
..........................?. [[Ahijah]]<ref name=""1Ch 8:7""/><br>
..........................?. [[Gera]]<ref name=""1Ch 8:7""/><br>
...........................?. [[Uzza]]<ref name=""1Ch 8:7""/><br>
...........................?. [[Ahihud]]<ref name=""1Ch 8:7""/><br>
........................?. [[Aphiah]]<ref name=""1Sa 9:1"">1 Samuel 9:1</ref><br>
.........................?. [[Becorath]]<ref name=""1Sa 9:1""/><br>
..........................?. [[Zeror]]<ref name=""1Sa 9:1""/><br>
...........................?. [[Abiel]]<ref name=""1Sa 9:1""/><br>
............................?. [[Kish (Bible)|Kish]]<ref name=""1Sa 9:1""/><br>
.............................?. [[Saul]]<ref name=""1Sa 9:2"">1 Samuel 9:2</ref><br>
.............................+ m. [[Ahinoam]]<ref name=""1Sa 14:50"">1 Samuel 14:50</ref><br>
..............................?. [[List of minor Biblical figures#Jonathan|Jonathan]]<ref name=""1Sa 13:16"">1 Samuel 13:16</ref><br>
...............................?. [[Mephibosheth]]<ref name=""2Sa 4:4"">2 Samuel 4:4</ref><br>
................................?. [[Mica]]<ref name=""2Sa 9:12"">2 Samuel 9:12</ref><br>
..............................?. [[Ishvi]]<ref name=""1Sa 14:49"">1 Samuel 14:49</ref><br>
..............................?. [[Malki-Shua]]<ref name=""1Sa 14:49""/><br>
..............................?. [[Merab]]<ref name=""1Sa 14:49""/><br>
..............................?. [[Michal]]<ref name=""1Sa 14:49""/><br>
..............................?. [[Abinadab]]<ref name=""1Sa 31:2"">1 Samuel 31:2</ref><br>
..............................?. [[Ish-Bosheth]]<ref name=""2Sa 2:8"">2 Samuel 2:8</ref><br>
..............................?. [[Merab]]<ref name=""2Sa 21:8"">2 Samuel 21:8</ref><br>
............................?. [[Ner]]<ref name=""1Sa 14:50""/><br>
.............................?. [[Abner]]<ref name=""1Sa 14:50""/><br>
........................?. [[Shaharaim]]<ref name=""1Ch 8:8"">1 Chronicles 8:8</ref><br>
........................+ m. [[Hushim]]<ref name=""1Ch 8:8""/><br>
.........................?. [[Abitub]]<ref name=""1Ch 8:11"">1 Chronicles 8:11</ref><br>
.........................?. [[Elpaal]]<ref name=""1Ch 8:11""/><br>
..........................?. [[Eber]]<ref name=""1Ch 8:12"">1 Chronicles 8:12</ref><br>
..........................?. [[Misham]]<ref name=""1Ch 8:12""/><br>
..........................?. [[Shemed]]<ref name=""1Ch 8:12""/><br>
..........................?. [[List of minor biblical figures#Beriah|Beriah]]<ref name=""1Ch 8:13"">1 Chronicles 8:13</ref><br>
...........................?. [[Ahio]]<ref name=""1Ch 8:14"">1 Chronicles 8:14</ref><br>
...........................?. [[Shashak]]<ref name=""1Ch 8:14""/><br>
............................?. [[Ishpan]]<ref name=""1Ch 8:22"">1 Chronicles 8:22</ref><br>
............................?. [[Eber]]<ref name=""1Ch 8:22""/><br>
............................?. [[Eliel]]<ref name=""1Ch 8:22""/><br>
............................?. [[List of minor biblical figures#Abdon|Abdon]]<ref name=""1Ch 8:23"">1 Chronicles 8:23</ref><br>
............................?. [[Zicri]]<ref name=""1Ch 8:23""/><br>
............................?. [[List of minor Biblical figures#Hanan|Hanan]]<ref name=""1Ch 8:23""/><br>
............................?. [[Hananiah, son of Azzur|Hananiah]]<ref name=""1Ch 8:24"">1 Chronicles 8:24</ref><br>
............................?. [[Elam]]<ref name=""1Ch 8:24""/><br>
............................?. [[Anthothijah]]<ref name=""1Ch 8:24""/><br>
............................?. [[Iphdeiah]]<ref name=""1Ch 8:25"">1 Chronicles 8:25</ref><br>
............................?. [[Penuel]]<ref name=""1Ch 8:25""/><br>
...........................?. [[Jeremoth]]<ref name=""1Ch 8:14""/><br>
...........................?. [[List of minor Biblical figures#Zebadiah|Zebadiah]]<ref name=""1Ch 8:15"">1 Chronicles 8:15</ref><br>
...........................?. [[Arad (Bibl)|Arad]]<ref name=""1Ch 8:15""/><br>
...........................?. [[Eder]]<ref name=""1Ch 8:15""/><br>
...........................?. [[Michael]]<ref name=""1Ch 8:16"">1 Chronicles 8:16</ref><br>
...........................?. [[Ishpah]]<ref name=""1Ch 8:16""/><br>
...........................?. [[Joha]]<ref name=""1Ch 8:16""/><br>
..........................?. [[Shema]]<ref name=""1Ch 8:13""/><br>
..........................?. [[List of minor Biblical figures#Zebadiah|Zebadiah]]<ref name=""1Ch 8:17"">1 Chronicles 8:17</ref><br>
..........................?. [[Meshullam]]<ref name=""1Ch 8:17""/><br>
..........................?. [[Hizki]]<ref name=""1Ch 8:17""/><br>
..........................?. [[Minor characters in the Book of Genesis|Heber]]<ref name=""1Ch 8:17""/><br>
..........................?. [[Ishmerai]]<ref name=""1Ch 8:18"">1 Chronicles 8:18</ref><br>
..........................?. [[Izliah]]<ref name=""1Ch 8:18""/><br>
..........................?. [[Jobab]]<ref name=""1Ch 8:18""/><br>
........................+ m. [[Baara]]<ref name=""1Ch 8:8""/><br>
........................+ m. [[Hodesh]]<ref name=""1Ch 8:9"">1 Chronicles 8:9</ref><br>
.........................?. [[Jobab]]<ref name=""1Ch 8:9""/><br>
.........................?. [[Zibia]]<ref name=""1Ch 8:9""/><br>
.........................?. [[Mesha]]<ref name=""1Ch 8:9""/><br>
.........................?. [[Malcam]]<ref name=""1Ch 8:9""/><br>
.........................?. [[Jeuz]]<ref name=""1Ch 8:10"">1 Chronicles 8:10</ref><br>
.........................?. [[Sakia]]<ref name=""1Ch 8:10""/><br>
.........................?. [[Mirmah]]<ref name=""1Ch 8:10""/><br>
........................?. [[Shimei]]<ref name=""1Ch 8:21"">1 Chronicles 8:21</ref><br>
.........................?. [[Jakim]]<ref name=""1Ch 8:19"">1 Chronicles 8:19</ref><br>
.........................?. [[Zicri]]<ref name=""1Ch 8:19""/><br>
.........................?. [[Zabdi]]<ref name=""1Ch 8:19""/><br>
.........................?. [[Elienai]]<ref name=""1Ch 8:20"">1 Chronicles 8:20</ref><br>
.........................?. [[Zillethai]]<ref name=""1Ch 8:20""/><br>
.........................?. [[Eliel]]<ref name=""1Ch 8:20""/><br>
.........................?. [[Adaiah]]<ref name=""1Ch 8:21""/><br>
.........................?. [[Beraiah]]<ref name=""1Ch 8:21""/><br>
.........................?. [[Shimrath]]<ref name=""1Ch 8:21""/><br>
........................?. [[Jeiel]]<ref name=""1Ch 8:29"">1 Chronicles 8:29</ref><br>
.........................?. [[List of minor Biblical figures#Gibeon|Gibeon]]<ref name=""1Ch 8:29""/><br>
.........................+ m. [[Maacah]]<ref name=""1Ch 8:29""/><br>
..........................?. [[List of minor biblical figures#Abdon|Abdon]]<ref name=""1Ch 8:30"">1 Chronicles 8:30</ref><br>
..........................?. [[Zur]]<ref name=""1Ch 8:30""/><br>
..........................?. Kish<ref name=""1Ch 8:30""/><br>
..........................?. [[Baal]]<ref name=""1Ch 8:30""/><br>
..........................?. [[Ner]]<ref name=""1Ch 8:30""/><br>
...........................?. Kish<ref name=""1Ch 8:33"">1 Chronicles 8:33</ref><br>
............................?. [[Saul]]<ref name=""1Ch 8:33""/><br>
.............................?. [[List of minor Biblical figures#Jonathan|Jonathan]]<ref name=""1Ch 8:33""/><br>
..............................?. [[Merib-Baal]]<ref name=""1Ch 8:34"">1 Chronicles 8:34</ref><br>
...............................?. [[Micah]]<ref name=""1Ch 8:34""/><br>
................................?. [[Pithon]]<ref name=""1Ch 8:35"">1 Chronicles 8:35</ref><br>
................................?. [[List of minor Biblical figures#Melech|Melech]]<ref name=""1Ch 8:35""/><br>
................................?. [[Tarea]]<ref name=""1Ch 8:35""/><br>
................................?. [[Ahaz]]<ref name=""1Ch 8:35""/><br>
.................................?. [[Jehoaddah]]<ref name=""1Ch 8:36"">1 Chronicles 8:36</ref><br>
..................................?. [[Alemeth]]<ref name=""1Ch 8:36""/><br>
..................................?. [[Azmaveth]]<ref name=""1Ch 8:36""/><br>
...................................?. [[Jeziel]]<ref name=""1Ch 12:3"">1 Chronicles 12:3</ref><br>
...................................?. [[Pelet]]<ref name=""1Ch 12:3""/><br>
..................................?. [[Zimri (prince)|Zimri]]<ref name=""1Ch 8:36""/><br>
...................................?. [[List of minor Biblical figures#Moza|Moza]]<ref name=""1Ch 8:36""/><br>
....................................?. [[Binea]]<ref name=""1Ch 8:37"">1 Chronicles 8:37</ref><br>
.....................................?. [[Raphah]]<ref name=""1Ch 8:37""/><br>
......................................?. [[Eleasah]]<ref name=""1Ch 8:37""/><br>
.......................................?. [[List of minor Biblical figures#Azel|Azel]]<ref name=""1Ch 8:37""/><br>
........................................?. [[Azrikam]]<ref name=""1Ch 8:38"">1 Chronicles 8:38</ref><br>
........................................?. [[Bokeru]]<ref name=""1Ch 8:38""/><br>
........................................?. [[Ishmael]]<ref name=""1Ch 8:38""/><br>
........................................?. [[Sheariah]]<ref name=""1Ch 8:38""/><br>
........................................?. [[Obadiah]]<ref name=""1Ch 8:38""/><br>
........................................?. [[List of minor Biblical figures#Hanan|Hanan]]<ref name=""1Ch 8:38""/><br>
.......................................?. [[Eshek]]<ref name=""1Ch 8:39"">1 Chronicles 8:39</ref><br>
........................................?. [[Ulam (Bible)|Ulam]]<ref name=""1Ch 8:39""/><br>
........................................?. [[Jeush]]<ref name=""1Ch 8:39""/><br>
........................................?. [[Eliphelet]]<ref name=""1Ch 8:39""/><br>
.............................?. [[Malki-Shua]]<ref name=""1Ch 8:33""/><br>
.............................?. [[Abinadab]]<ref name=""1Ch 8:33""/><br>
.............................?. [[Esh-Baal]]<ref name=""1Ch 8:33""/><br>
..........................?. [[Nadab (son of Aaron)|Nadab]]<ref name=""1Ch 8:30""/><br>
..........................?. [[Gedor]]<ref name=""1Ch 8:31"">1 Chronicles 8:31</ref><br>
..........................?. [[Ahio]]<ref name=""1Ch 8:31""/><br>
..........................?. [[Zeker]]<ref name=""1Ch 8:31""/><br>
..........................?. [[Mikloth]]<ref name=""1Ch 8:32"">1 Chronicles 8:32</ref><br>
...........................?. [[Shimeah]]<ref name=""1Ch 8:32""/><br>
........................?. [[Hassenuah]]<ref name=""1Ch 9:7"">1 Chronicles 9:7</ref><br>
.........................?. [[Hodaviah]]<ref name=""1Ch 9:7""/><br>
..........................?. [[Meshullam]]<ref name=""1Ch 9:7""/><br>
...........................?. [[Sallu]]<ref name=""1Ch 9:7""/><br>
........................?. [[Jeroham]]<ref name=""1Ch 9:8"">1 Chronicles 9:8</ref><br>
.........................?. [[Ibneiah]]<ref name=""1Ch 9:8""/><br>
........................?. [[Micri]]<ref name=""1Ch 9:8""/><br>
.........................?. [[Uzzi]]<ref name=""1Ch 9:8""/><br>
..........................?. [[King Elah|Elah]]<ref name=""1Ch 9:8""/><br>
........................?. [[Ibnijah]]<ref name=""1Ch 9:8""/><br>
.........................?. [[Reuel]]<ref name=""1Ch 9:8""/><br>
..........................?. [[Shephatiah]]<ref name=""1Ch 9:8""/><br>
...........................?. [[Meshullam]]<ref name=""1Ch 9:8""/><br>
......................+ m. [[Bilhah]]<ref name=""Ge 30:4"">Genesis 30:4</ref><br>
.......................23. [[Dan (Bible)|Dan]]<ref name=""Ge 30:6"">Genesis 30:6</ref><br>
........................24. [[Hushim]]<ref name=""Ge 46:23"">Genesis 46:23</ref><br>
........................?. [[Ahisamach (Bible)|Ahisamach]]<ref name=""Ex 31:6"">Exodus 31:6</ref><br>
.........................?. [[Oholiab]]<ref name=""Ex 31:6""/><br>
........................?. [[Ammishaddai]]<ref name=""Nu 1:12"">Numbers 1:12</ref><br>
.........................?. [[Ahiezer]]<ref name=""Nu 1:12""/><br>
........................?. [[Gemalli]]<ref name=""Nu 13:12"">Numbers 13:12</ref><br>
.........................?. [[Ammiel]]<ref name=""Nu 13:12""/><br>
........................?. [[Jogli]]<ref name=""Nu 34:22"">Numbers 34:22</ref><br>
.........................?. [[Bukki]]<ref name=""Nu 34:22""/><br>
.......................23. [[Naphtali]]<ref name=""Ge 30:8"">Genesis 30:8</ref><br>
........................24. [[Jahziel]]<ref name=""Ge 46:24"">Genesis 46:24</ref><br>
........................24. Guni<ref name=""Ge 46:24""/><br>
........................24. [[Jezer]]<ref name=""Ge 46:24""/><br>
........................24. [[Shillem]]<ref name=""Ge 46:24""/><br>
........................?. [[Enan]]<ref name=""Nu 1:15"">Numbers 1:15</ref><br>
.........................?. [[Ahira]]<ref name=""Nu 1:15""/><br>
........................?. [[Vophsi]]<ref name=""Nu 13:14"">Numbers 13:14</ref><br>
.........................?. [[Nahbi]]<ref name=""Nu 13:14""/><br>
........................?. [[Ammihud]]<ref name=""Nu 34:28"">Numbers 34:28</ref><br>
.........................?. [[Pedahel]]<ref name=""Nu 34:28""/><br>
......................+ m. [[Zilpah]]<ref name=""Ge 30:8"" /><br>
.......................23. [[Gad (son of Jacob)|Gad]]<ref name=""Ge 30:11"">Genesis 30:11</ref><br>
........................24. [[Zephon]]<ref name=""Ge 46:16"">Genesis 46:16</ref><br>
........................24. [[Haggi]]<ref name=""Ge 46:16""/><br>
........................24. [[Shuni]]<ref name=""Ge 46:16""/><br>
........................24. [[Ezbon]]<ref name=""Ge 46:16""/><br>
........................24. [[List of minor Biblical figures#Eri|Eri]]<ref name=""Ge 46:16""/><br>
........................24. [[Arodi]]<ref name=""Ge 46:16""/><br>
........................24. [[Areli]]<ref name=""Ge 46:16""/><br>
........................?. [[List of minor Biblical figures#Deuel|Deuel]]<ref name=""Nu 1:14"">Numbers 1:14</ref><br>
.........................?. [[Eliasaph]]<ref name=""Nu 1:14""/><br>
........................?. [[List of minor Biblical figures#Maki|Maki]]<ref name=""Nu 13:15"">Numbers 13:15</ref><br>
.........................?. [[Geuel]]<ref name=""Nu 13:15""/><br>
........................?. [[Joel (prophet)|Joel]]<ref name=""1Ch 5:12"">1 Chronicles 5:12</ref><br>
........................?. [[Shapham]]<ref name=""1Ch 5:12""/><br>
........................?. [[Janai]]<ref name=""1Ch 5:12""/><br>
........................?. [[Shaphat]]<ref name=""1Ch 5:12""/><br>
.........................?. [[List of minor Biblical figures#Buz|Buz]]<ref name=""1Ch 5:14"">1 Chronicles 5:14</ref><br>
..........................?. [[Jahdo]]<ref name=""1Ch 5:14""/><br>
...........................?. [[Jeshishai]]<ref name=""1Ch 5:14""/><br>
............................?. [[Michael]]<ref name=""1Ch 5:14""/><br>
.............................?. [[Gilead]]<ref name=""1Ch 5:14""/><br>
..............................?. [[Jaroah]]<ref name=""1Ch 5:14""/><br>
...............................?. [[Huri]]<ref name=""1Ch 5:14""/><br>
................................?. [[Abihail]]<ref name=""1Ch 5:14""/><br>
.................................?. [[Michael]]<ref name=""1Ch 5:13"">1 Chronicles 5:13</ref><br>
.................................?. [[Meshullam]]<ref name=""1Ch 5:13""/><br>
.................................?. [[Sheba]]<ref name=""1Ch 5:13""/><br>
.................................?. [[Jorai]]<ref name=""1Ch 5:13""/><br>
.................................?. [[Jacan]]<ref name=""1Ch 5:13""/><br>
.................................?. [[List of minor Biblical figures#Zia|Zia]]<ref name=""1Ch 5:13""/><br>
.................................?. [[Eber]]<ref name=""1Ch 5:13""/><br>
........................?. Guni<ref name=""1Ch 5:15"">1 Chronicles 5:15</ref><br>
.........................?. [[Abdiel]]<ref name=""1Ch 5:15""/><br>
..........................?. [[List of minor biblical figures#Ahi|Ahi]]<ref name=""1Ch 5:15""/><br>
.......................23. [[Asher]]<ref name=""Ge 30:13"">Genesis 30:13</ref><br>
........................24. [[Imnah]]<ref name=""Ge 46:17"">Genesis 46:17</ref><br>
........................24. [[Ishvah]]<ref name=""Ge 46:17""/><br>
........................24. [[Ishvi]]<ref name=""Ge 46:17""/><br>
........................24. [[List of minor biblical figures#Beriah|Beriah]]<ref name=""Ge 46:17""/><br>
.........................25. [[Minor characters in the Book of Genesis|Heber]]<ref name=""Ge 46:17""/><br>
..........................26. [[Japhlet]]<ref name=""1Ch 7:32"">1 Chronicles 7:32</ref><br>
...........................27. [[Pasach]]<ref name=""1Ch 7:33"">1 Chronicles 7:33</ref><br>
...........................27. [[Bimhal]]<ref name=""1Ch 7:33""/><br>
...........................27. [[Ashvath]]<ref name=""1Ch 7:33""/><br>
..........................26. [[Shomer]]<ref name=""1Ch 7:32""/><br>
...........................27. [[List of minor biblical figures#Ahi|Ahi]]<ref name=""1Ch 7:34"">1 Chronicles 7:34</ref><br>
...........................27. [[Rohgah]]<ref name=""1Ch 7:34""/><br>
...........................27. [[Hubbah]]<ref name=""1Ch 7:34""/><br>
...........................27. [[Aram, son of Shem|Aram]]<ref name=""1Ch 7:34""/><br>
..........................26. [[List of minor Biblical figures#Hotham|Hotham]]<ref name=""1Ch 7:32""/><br>
..........................26. [[Shua]]<ref name=""1Ch 7:32""/><br>
..........................26. [[Helem]]<ref name=""1Ch 7:35"">1 Chronicles 7:35</ref><br>
...........................27. [[Zophah]]<ref name=""1Ch 7:35""/><br>
............................28. [[Suah]]<ref name=""1Ch 7:36"">1 Chronicles 7:36</ref><br>
............................28. [[Harnepher]]<ref name=""1Ch 7:36""/><br>
............................28. [[Shual]]<ref name=""1Ch 7:36""/><br>
............................28. [[List of minor Biblical figures#Beri|Beri]]<ref name=""1Ch 7:36""/><br>
............................28. [[Imrah]]<ref name=""1Ch 7:36""/><br>
............................28. [[Bezer]]<ref name=""1Ch 7:37"">1 Chronicles 7:37</ref><br>
............................28. [[List of minor Biblical figures#Hod|Hod]]<ref name=""1Ch 7:37""/><br>
............................28. [[Shamma]]<ref name=""1Ch 7:37""/><br>
............................28. [[Shilshah]]<ref name=""1Ch 7:37""/><br>
............................28. [[Ithran]]<ref name=""1Ch 7:37""/><br>
............................28. [[Beera]]<ref name=""1Ch 7:37""/><br>
...........................27. [[Imna (Bible)]]<ref name=""1Ch 7:35""/><br>
...........................27. [[Shelesh]]<ref name=""1Ch 7:35""/><br>
...........................27. [[List of minor Biblical figures#Amal|Amal]]<ref name=""1Ch 7:35""/><br>
.........................25. [[List of minor Biblical figures#Malkiel|Malkiel]]<ref name=""Ge 46:17""/><br>
..........................26. [[Birzaith]]<ref name=""1Ch 7:31"">1 Chronicles 7:31</ref><br>
........................24. [[Serah]]<ref name=""Ge 46:17""/><br>
........................?. [[Ocran]]<ref name=""Nu 1:13"">Numbers 1:13</ref><br>
.........................?. [[Pagiel]]<ref name=""Nu 1:13""/><br>
........................?. [[Michael]]<ref name=""Nu 13:1"">Numbers 13:1</ref><br>
.........................?. [[Sethur]]<ref name=""Nu 13:1""/><br>
........................?. [[List of minor Biblical figures#Shelomi|Shelomi]]<ref name=""Nu 34:27"">Numbers 34:27</ref><br>
.........................?. [[Ahihud]]<ref name=""Nu 34:27""/><br>
........................?. [[Jether]]<ref name=""1Ch 7:38"">1 Chronicles 7:38</ref><br>
.........................?. [[Jephunneh]]<ref name=""1Ch 7:38""/><br>
.........................?. [[Pispah]]<ref name=""1Ch 7:38""/><br>
.........................?. [[List of minor Biblical figures#Ara|Ara]]<ref name=""1Ch 7:38""/><br>
........................?. [[List of minor Biblical figures#Ulla|Ulla]]<ref name=""1Ch 7:39"">1 Chronicles 7:39</ref><br>
.........................?. [[Arah]]<ref name=""1Ch 7:39""/><br>
.........................?. [[Hanniel]]<ref name=""1Ch 7:39""/><br>
.........................?. [[Rizia]]<ref name=""1Ch 7:39""/><br>
....................+ m. [[Hagar (Bible)|Hagar]]<ref name=""Ge 16:3"">Genesis 16:3</ref><br>
.....................21. [[Ishmael]]<ref name=""Ge 16:15"">Genesis 16:15</ref><br>
......................22. [[Nebaioth]]<ref name=""Ge 25:13"">Genesis 25:13</ref><br>
......................22. [[Qedarite#Biblical|Kedar]]<ref name=""Ge 25:13""/><br>
......................22. [[Adbeel]]<ref name=""Ge 25:13""/><br>
......................22. [[Mibsam]]<ref name=""Ge 25:13""/><br>
......................22. [[Mishma]]<ref name=""Ge 25:14"">Genesis 25:14</ref><br>
......................22. [[Dumah (son of Ishmael)|Dumah]]<ref name=""Ge 25:14""/><br>
......................22. [[Massa]]<ref name=""Ge 25:14""/><br>
......................22. [[Hadad]]<ref name=""Ge 25:15"">Genesis 25:15</ref><br>
......................22. [[Tema]]<ref name=""Ge 25:15""/><br>
......................22. [[Jetur]]<ref name=""Ge 25:15""/><br>
......................22. [[Naphish]]<ref name=""Ge 25:15""/><br>
......................22. [[Kedemah]]<ref name=""Ge 25:15""/><br>
......................22. [[Basemath]]<ref name=""Ge 36:3""/><br>
....................+ m. [[Keturah]]<ref name=""Ge 25:1"">Genesis 25:1</ref><br>
.....................21. [[Zimran]]<ref name=""Ge 25:2"">Genesis 25:2</ref><br>
.....................21. [[Jokshan]]<ref name=""Ge 25:2""/><br>
......................22. [[Sheba]]<ref name=""Ge 25:3"">Genesis 25:3</ref><br>
......................22. [[Dedan]]<ref name=""Ge 25:3""/><br>
.......................23. [[Asshurites]]<ref name=""Ge 25:3""/><br>
.......................23. [[Letushites]]<ref name=""Ge 25:3""/><br>
.......................23. [[Leummites]]<ref name=""Ge 25:3""/><br>
.....................21. [[Medan]]<ref name=""Ge 25:2""/><br>
.....................21. [[Midian]]<ref name=""Ge 25:2""/><br>
......................22. [[Ephah]]<ref name=""Ge 25:4"">Genesis 25:4</ref><br>
......................22. [[Epher]]<ref name=""Ge 25:4""/><br>
......................22. [[Hanoch (Bible)|Hanoch]]<ref name=""Ge 25:4""/><br>
......................22. [[Abida Midian|Abida]]<ref name=""Ge 25:4""/><br>
......................22. [[Eldaah]]<ref name=""Ge 25:4""/><br>
.....................21. [[Ishbak]]<ref name=""Ge 25:2""/><br>
.....................21. [[Shuah]]<ref name=""Ge 25:2""/><br>
....................20. [[Nahor, son of Terah|Nahor]]<ref name=""Ge 11:26""/><br>
....................+ m. [[Milcah]]<ref name=""Ge 11:29""/><br>
.....................21. [[Uz (son of Aram)|Uz]]<ref name=""Ge 22:21"">Genesis 22:21</ref><br>
.....................21. [[Children of Eber|Buz]]<ref name=""Ge 22:21""/><br>
.....................21. [[Kemuel]]<ref name=""Ge 22:21""/><br>
......................22. [[Aram, son of Shem|Aram]]<ref name=""Ge 22:21""/><br>
.....................21. [[Kesed]]<ref name=""Ge 22:22"">Genesis 22:22</ref><br>
.....................21. [[Hazo]]<ref name=""Ge 22:22""/><br>
.....................21. [[Pildash]]<ref name=""Ge 22:22""/><br>
.....................21. [[Jidlaph]]<ref name=""Ge 22:22""/><br>
.....................21. [[Bethuel]]<ref name=""Ge 22:22""/><br>
......................22. [[Rebekah]]<ref name=""Ge 22:23"" /><br>
......................22. [[Laban (Bible)|Laban]]<ref name=""Ge 24:29"">Genesis 24:29</ref><br>
.......................23. [[Leah]]<ref name=""Ge 29:16"" /><br>
.......................23. [[Rachel]]<ref name=""Ge 29:6"" /><br>
....................+ m. [[Reumah]]<ref name=""Ge 22:24"">Genesis 22:24</ref><br>
.....................21. [[List of minor Biblical figures#Tebah|Tebah]]<ref name=""Ge 22:24""/><br>
.....................21. [[Gaham]]<ref name=""Ge 22:24""/><br>
.....................21. [[Tahash]]<ref name=""Ge 22:24""/><br>
.....................21. [[Maacah]]<ref name=""Ge 22:24""/><br>
....................20. [[Haran]]<ref name=""Ge 11:26""/><br>
.....................21. [[Lot (biblical person)|Lot]]<ref name=""Ge 11:27"">Genesis 11:27</ref><br>
......................22. Older daughter<ref name=""Ge 19:8"">Genesis 19:8</ref><br>
......................22. Younger daughter<ref name=""Ge 19:8""/><br>
......................+ m. Older daughter<ref name=""Ge 19:8""/><br>
......................22. [[Moab]]<ref name=""Ge 19:37"">Genesis 19:37</ref><br>
.......................23. Moabites<ref name=""Ge 19:37""/><br>
......................+ m. Younger daughter<ref name=""Ge 19:8""/><br>
......................22. [[Ben-Ammi]]<ref name=""Ge 19:38"">Genesis 19:38</ref><br>
.......................23. Ammonites<ref name=""Ge 19:38""/><br>
.....................21. [[Milcah]]<ref name=""Ge 11:29""/><br>
.....................21. [[Iscah]]<ref name=""Ge 11:29""/><br>
....................20. [[Sarah]]<ref name=""Ge 11:29""/><br>
...............15. [[Joktan]]<ref name=""Ge 10:25""/><br>
................16. [[Almodad]]<ref name=""Ge 10:26"">Genesis 10:26</ref><br>
................16. [[Sheleph]]<ref name=""Ge 10:26""/><br>
................16. [[Hazarmaveth]]<ref name=""Ge 10:26""/><br>
................16. [[Jerah]]<ref name=""Ge 10:26""/><br>
................16. [[Hadoram]]<ref name=""Ge 10:27"">Genesis 10:27</ref><br>
................16. [[Uzal]]<ref name=""Ge 10:27""/><br>
................16. [[Diklah]]<ref name=""Ge 10:27""/><br>
................16. [[Obal]]<ref name=""Ge 10:28"">Genesis 10:28</ref><br>
................16. [[Abimael]]<ref name=""Ge 10:28""/><br>
................16. [[Sheba]]<ref name=""Ge 10:28""/><br>
................16. [[Ophir]]<ref name=""Ge 10:29"">Genesis 10:29</ref><br>
................16. [[Havilah]]<ref name=""Ge 10:29""/><br>
................16. [[Jobab]]<ref name=""Ge 10:29""/><br>
............12. [[Lud son of Shem|Lud]]<ref name=""Ge 10:22""/><br>
............12. [[Aram, son of Shem|Aram]]<ref name=""Ge 10:22""/><br>
.............13. [[Uz (son of Aram)|Uz]]<ref name=""Ge 10:23"">Genesis 10:23</ref><br>
.............13. [[Hul]]<ref name=""Ge 10:23""/><br>
.............13. [[Gether]]<ref name=""Ge 10:23""/><br>
.............13. [[Meshech]]<ref name=""Ge 10:23""/><br>
...........11. [[Ham (son of Noah)|Ham]]<ref name=""Ge 5:32""/><br>
............12. [[Cush (Bible)|Cush]]<ref name=""Ge 10:6"">Genesis 10:6</ref><br>
.............13. [[List of minor Biblical figures#Seba|Seba]]<ref name=""Ge 10:7"">Genesis 10:7</ref><br>
.............13. [[Havilah]]<ref name=""Ge 10:7""/><br>
.............13. [[Sabtah]]<ref name=""Ge 10:7""/><br>
.............13. [[Raamah]]<ref name=""Ge 10:7""/><br>
..............14. [[Sheba]]<ref name=""Ge 10:7""/><br>
..............14. [[Dedan]]<ref name=""Ge 10:7""/><br>
.............13. [[Sabteca]]<ref name=""Ge 10:7""/><br>
.............13. [[Nimrod]]<ref name=""Ge 10:8"">Genesis 10:8</ref><br>
............12. [[Mizraim]]<ref name=""Ge 10:6""/><br>
.............13. [[Ludites]]<ref name=""Ge 10:13"">Genesis 10:13</ref><br>
.............13. [[Anamites]]<ref name=""Ge 10:13""/><br>
.............13. [[Lehabites]]<ref name=""Ge 10:13""/><br>
.............13. [[Naphtuhites]]<ref name=""Ge 10:13""/><br>
.............13. [[Pathrusites]]<ref name=""Ge 10:14"">Genesis 10:14</ref><br>
.............13. [[Casluhites]]<ref name=""Ge 10:14""/><br>
..............14. [[Philistines]]<ref name=""Ge 10:14""/><br>
.............13. [[Caphtorites]]<ref name=""Ge 10:14""/><br>
............12. [[Phut|Put]]<ref name=""Ge 10:6""/><br>
............12. [[Canaan]]{{ref|Canaan|6}}<ref name=""Ge 10:6""/><br>
.............13. [[Sidon]]<ref name=""Ge 10:15"">Genesis 10:15</ref><br>
.............13. [[Hittites]]{{ref|Hittites|7}}<ref name=""Ge 10:15""/><br>
.............13. [[Jebusites]]<ref name=""Ge 10:16"">Genesis 10:16</ref><br>
.............13. [[Amorites]]<ref name=""Ge 10:16""/><br>
.............13. [[Girgashites]]<ref name=""Ge 10:16""/><br>
.............13. [[Hivites]]{{ref|Hivites|8}}<ref name=""Ge 10:17"">Genesis 10:17</ref><br>
.............13. [[Arkites]]<ref name=""Ge 10:17""/><br>
.............13. [[List of minor Biblical figures#Sinites|Sinites]]<ref name=""Ge 10:17""/><br>
.............13. [[Arvadites]]<ref name=""Ge 10:18"">Genesis 10:18</ref><br>
.............13. [[Zemarites]]<ref name=""Ge 10:18""/><br>
.............13. [[Hamathites]]<ref name=""Ge 10:18""/><br>
...........11. [[Japheth]]<ref name=""Ge 5:32""/><br>
............12. [[Gomer]]<ref name=""Ge 10:2"">Genesis 10:2</ref><br>
.............13. [[Ashkenaz]]<ref name=""Ge 10:3"">Gemesis 10:3</ref><br>
.............13. [[Riphath]]<ref name=""Ge 10:3""/><br>
.............13. [[Togarmah]]<ref name=""Ge 10:3""/><br>
............12. [[Magog (Bible)|Magog]]<ref name=""Ge 10:2""/><br>
............12. [[Madai]]<ref name=""Ge 10:2""/><br>
............12. [[Javan]]<ref name=""Ge 10:2""/><br>
.............13. [[Elishah]]<ref name=""Ge 10:4"">Genesis 10:4</ref><br>
.............13. [[Tarshish]]<ref name=""Ge 10:4""/><br>
.............13. [[Kittim]]<ref name=""Ge 10:4""/><br>
.............13. [[Rodanim]]<ref name=""Ge 10:4""/><br>
............12. [[Tubal]]<ref name=""Ge 10:2""/><br>
............12. [[Meshech]]<ref name=""Ge 10:2""/><br>
............12. [[Tiras]]<ref name=""Ge 10:2""/>

Genealogy from Zerubbabel to Jesus:

.....................................................53. [[Zerubbabel]]<ref name=""Mat 1:1-17"">Matthew 1:1-17</ref><br>
......................................................54. [[Abiud]]<ref name=""Mat 1:1-17"" /><br>
.......................................................55. [[Eliakim]]<ref name=""Mat 1:1-17"" /><br>
........................................................56. [[Azor]]<ref name=""Mat 1:1-17"" /><br>
.........................................................57. [[Zadok]]<ref name=""Mat 1:1-17"" /><br>
..........................................................58. [[Achim]]<ref name=""Mat 1:1-17"" /><br>
...........................................................59. [[Eliud]]<ref name=""Mat 1:1-17"" /><br>
............................................................60. [[Eleazar]]<ref name=""Mat 1:1-17"" /><br>
.............................................................61. [[Matthan]]<ref name=""Mat 1:1-17"" /><br>
..............................................................62. [[Genealogy of Jesus|Jacob]]<ref name=""Mat 1:1-17"" /><br>
...............................................................63. [[Saint Joseph|Joseph]]<ref name=""Mat 1:1-17"" /><br>
...............................................................+ m. [[Mary (mother of Jesus)|Mary]]<br>
................................................................64. [[Jesus]]<!-- Jesus is at least the 42nd generation after Abraham (20th generation) according to literal reading of Matthew 1 -->

==References==
{{reflist|colwidth=20em}}

{{Religious family trees}}
{{Adam and Eve}}
{{Cain and Abel}}

{{DEFAULTSORT:Descendants Of Adam And Eve}}
[[Category:Bible genealogy]]
";

        private const string Best = @"1. [[Adam]]<ref>Genesis</ref><br>
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
.............................? [[Kenaz]]<ref name=""1Ch 4:15""/><br>
...............................................................63. [[Saint Joseph|Joseph]]<ref name=""Mat 1:1-17"" /><br>
...............................................................+ m. [[Mary (mother of Jesus)|Mary]]<br>
................................................................64. [[Jesus]]<!-- Jesus is at least the 42nd generation after Abraham (20th generation) according to literal reading of Matthew 1 -->
";

        [Test]
        public void Parse()
        {
            var des = DescendantOfAdamAndEve.Parse(Best);
            Assert.AreEqual(33, des[9].GenerationNumber);
            Assert.AreEqual("Azrikam", des[9].Title);
            Assert.AreEqual("1Ch 9:14", des[9].RefName);
            Assert.AreEqual("1 Chronicles 9:14", des[9].RefCaption);
            Assert.IsNull(des[9].Parent);

            Assert.AreEqual(33, des[10].GenerationNumber);
            Assert.AreEqual("Nun", des[10].Title);
            Assert.AreEqual("Nu 13:8", des[10].Ref2Name);
            Assert.AreEqual("Numbers 13:8", des[10].Ref2Caption);
            Assert.AreEqual("1Ch 7:27", des[10].RefName);
            Assert.AreEqual("1 Chronicles 7:27", des[10].RefCaption);
            Assert.IsNull(des[10].Parent);

            Assert.IsTrue(des[11].GenerationNumberUnknown);
            Assert.AreEqual("Kenaz", des[11].Title);
            Assert.AreEqual("1Ch 4:15", des[11].RefName);
            Assert.IsNull(des[11].Parent);

            Assert.AreEqual(63, des[12].GenerationNumber);
            Assert.AreEqual("Saint Joseph", des[12].Title);
            Assert.AreEqual("Joseph", des[12].TitleShort);
            Assert.AreEqual("Mat 1:1-17", des[12].RefName);
            Assert.IsNull(des[12].Parent);

            Assert.AreEqual(63, des[13].GenerationNumber);
            Assert.AreEqual("Mary (mother of Jesus)", des[13].Title);
            Assert.AreEqual("Mary", des[13].TitleShort);
            Assert.AreEqual(des[12], des[13].Husband);
            Assert.IsNull(des[13].Parent);

            Assert.AreEqual(64, des[14].GenerationNumber);
            Assert.AreEqual("Jesus", des[14].Title);
            Assert.AreEqual(des[12], des[14].Parent);

            Assert.AreEqual(1, des[0].GenerationNumber);
            Assert.AreEqual("Adam", des[0].Title);
            Assert.AreEqual("Genesis", des[0].RefCaption);
            Assert.AreEqual("Ge", des[0].RefName);
            Assert.IsNull(des[0].Parent);

            Assert.AreEqual(1, des[1].GenerationNumber);
            Assert.AreEqual("Eve", des[1].Title);
            Assert.AreEqual(des[0], des[1].Husband);
            Assert.AreEqual("Genesis 3:20", des[1].RefCaption);
            Assert.IsNull(des[1].Parent);

            Assert.AreEqual(3, des[2].GenerationNumber);
            Assert.AreEqual("Enoch (son of Cain)", des[2].Title);
            Assert.AreEqual("Enoch", des[2].TitleShort);
            Assert.AreEqual("Genesis 4:17", des[2].RefCaption);
            Assert.IsNull(des[2].Parent);

            Assert.AreEqual(4, des[3].GenerationNumber);
            Assert.AreEqual("Irad", des[3].Title);
            Assert.AreEqual("Ge 4:18", des[3].RefName);
            Assert.AreEqual("Genesis 4:18", des[3].RefCaption);
            Assert.AreEqual(des[2], des[3].Parent);

            Assert.AreEqual(32, des[4].GenerationNumber);
            Assert.AreEqual("Uzziel", des[4].Title);
            Assert.AreEqual("1Ch 4:42", des[4].RefName);
            Assert.IsNull(des[4].Parent);

            Assert.IsTrue(des[5].GenerationNumberUnknown);
            Assert.AreEqual("Zoheth", des[5].Title);
            Assert.AreEqual("1Ch 4:20", des[5].RefName);
            Assert.AreEqual("1 Chronicles 4:20", des[5].RefCaption);
            Assert.IsNull(des[5].Parent);

            Assert.AreEqual(24, des[6].GenerationNumber);
            Assert.AreEqual("Teman", des[6].OtherCaption);
            Assert.AreEqual("Ge 36:11", des[6].RefName);
            Assert.AreEqual("Genesis 36:11", des[6].RefCaption);
            Assert.IsNull(des[6].Parent);

            Assert.AreEqual(23, des[7].GenerationNumber);
            Assert.AreEqual("Reuel", des[7].Title);
            Assert.AreEqual("Ge 36:4", des[7].RefName);
            Assert.IsNull(des[7].Parent);

            Assert.AreEqual(26, des[8].GenerationNumber);
            Assert.AreEqual("Aaron", des[8].Title);
            Assert.AreEqual("Ex 6:20", des[8].RefName);
            Assert.IsNull(des[8].Parent);
        }

        [Test]
        public void Check()
        {
            Assert.AreEqual("1Ch 9:14", new DescendantOfAdamAndEve { RefCaption = "1 Chronicles 9:14" }.Check().RefName);
        }

        [Test]
        public void ParseM1()
        {
            var des = DescendantOfAdamAndEve.Parse(@"
...................................35. [[Rehoboam]], King of Judah<ref name=""1Ki 11:43"" /><br>
...................................+ m1. [[List_of_minor_biblical_figures,_L–Z#Mahalath|Mahalath]]<ref name=""2Ch 11:19"" /><br>");
            Assert.AreEqual(35, des[0].GenerationNumber);
            Assert.AreEqual(des[0], des[1].Husband);
            Assert.AreEqual("List_of_minor_biblical_figures,_L–Z#Mahalath", des[1].Title);
            Assert.AreEqual("Mahalath", des[1].TitleShort);
            Assert.AreEqual("2Ch 11:19", des[1].RefName);
        }

        [Test]
        public void ParseWives()
        {
            var des = DescendantOfAdamAndEve.Parse(@"
......................22. [[Esau]]<ref name=""Ge 25:25"">Genesis 25:25</ref><br>
.......................+ m. [[Judith]]<ref name=""Ge 26:34"">Genesis 26:34</ref><br>
.......................+ m. [[Basemath]]<ref name=""Ge 26:34""/><br>");
            Assert.AreEqual(22, des[2].GenerationNumber);
            Assert.AreEqual(des[0], des[2].Husband);
        }

        [Test]
        public void ParseUnknown()
        {
            var des = DescendantOfAdamAndEve.Parse(@"
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
            Assert.IsNull(des[7].Parent);
            Assert.AreEqual(des[7], des[8].Parent);
        }

        [Test]
        public void ParseDescendants()
        {
            Assert.AreEqual(1150, DescendantOfAdamAndEve.ParseDescendants(new Page { Text = S160817 }).Count);
        }

        [Test]
        public void Duplicates()
        {
            Assert.AreEqual(1150, DescendantOfAdamAndEve.ParseDescendants(new Page { Text = S160817 }, true).Count);
        }
    }
}
#endif
