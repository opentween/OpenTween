// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;

namespace OpenTween
{
    [TestFixture]
    class LRUCacheDictionaryTest
    {
        [Test]
        public void InnerListTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            var node = dict.innerList.First;
            Assert.That(node.Value.Key, Is.EqualTo("key3"));
            node = node.Next;
            Assert.That(node.Value.Key, Is.EqualTo("key2"));
            node = node.Next;
            Assert.That(node.Value.Key, Is.EqualTo("key1"));

            // 2 -> 3 -> 1 の順に値を参照
            var x = dict["key2"];
            x = dict["key3"];
            x = dict["key1"];

            // 直近に参照した順で並んでいるかテスト
            node = dict.innerList.First;
            Assert.That(node.Value.Key, Is.EqualTo("key1"));
            node = node.Next;
            Assert.That(node.Value.Key, Is.EqualTo("key3"));
            node = node.Next;
            Assert.That(node.Value.Key, Is.EqualTo("key2"));
        }

        [Test]
        public void TrimLimitTest([Range(1, 5)] int trimLimit)
        {
            var dict = new LRUCacheDictionary<string, string>()
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            dict.TrimLimit = trimLimit;
            var ret = dict.Trim();

            if (trimLimit >= 3)
            {
                // trimLimit がアイテムの件数より大きい場合、Trim メソッドは動作せずに false を返す。
                Assert.That(ret, Is.False);
                Assert.That(dict.Count, Is.EqualTo(3));
            }
            else
            {
                Assert.That(ret, Is.True);
                Assert.That(dict.Count, Is.EqualTo(trimLimit));
            }
        }

        [Test]
        public void TrimOrderTest()
        {
            var dict = new LRUCacheDictionary<string, string>()
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
                {"key4", "value4"},
                {"key5", "value5"},
            };

            // 4 -> 2 -> 3 -> 1 -> 5 の順で参照
            var x = dict["key4"];
            x = dict["key2"];
            x = dict["key3"];
            x = dict["key1"];
            x = dict["key5"];

            // 3 個までに縮小
            dict.TrimLimit = 3;
            dict.Trim();

            // 直近に参照された 3 -> 1 -> 5 のみ残っているはず
            Assert.That(dict.ContainsKey("key1"), Is.True);
            Assert.That(dict.ContainsKey("key2"), Is.False);
            Assert.That(dict.ContainsKey("key3"), Is.True);
            Assert.That(dict.ContainsKey("key4"), Is.False);
            Assert.That(dict.ContainsKey("key5"), Is.True);
        }

        [Test]
        public void AutoTrimTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            dict.AutoTrimCount = 4; // 4アクセス毎に
            dict.TrimLimit = 3; // 3個を越えるアイテムを削除する

            dict["key1"] = "value1"; // 1アクセス目
            dict["key2"] = "value2"; // 2アクセス目
            dict["key3"] = "value3"; // 3アクセス目
            dict["key4"] = "value4"; // 4アクセス目 (この直後にTrim)

            // 1 -> 2 -> 3 -> 4 の順にアクセスしたため、直近 3 件の 2, 3, 4 だけが残る
            Assert.That(dict.innerDict.Keys, Is.EquivalentTo(new[] { "key2", "key3", "key4" }));

            dict["key5"] = "value5";         // 5アクセス目
            dict.Add("key6", "value6");      // 6アクセス目
            var x = dict["key2"];            // 7アクセス目
            dict.TryGetValue("key4", out x); // 8アクセス目 (この直後にTrim)

            // 5 -> 6 -> 2 -> 4 の順でアクセスしたため、直近 3 件の 6, 2, 4 だけが残る
            Assert.That(dict.innerDict.Keys, Is.EquivalentTo(new[] { "key6", "key2", "key4" }));
        }

        [Test]
        public void CacheRemovedEventTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            dict["key1"] = "value1";
            dict["key2"] = "value2";
            dict["key3"] = "value3";
            dict["key4"] = "value4";

            // イベント設定
            var removedList = new List<string>();
            dict.CacheRemoved += (s, e) =>
            {
                removedList.Add(e.Item.Key);
            };

            // 2 個までに縮小
            dict.TrimLimit = 2;
            dict.Trim();

            // 直近に参照された 3, 4 以外のアイテムに対してイベントが発生しているはず
            Assert.That(removedList, Is.EquivalentTo(new[] { "key1", "key2" }));
        }

        // ここから下は IDictionary としての機能が正しく動作するかのテスト

        [Test]
        public void AddTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            dict.Add("key1", "value1");

            Assert.That(dict.innerDict.Count, Is.EqualTo(1));
            Assert.That(dict.innerDict.ContainsKey("key1"), Is.True);
            var internalNode = dict.innerDict["key1"];
            Assert.That(internalNode.Value.Key, Is.EqualTo("key1"));
            Assert.That(internalNode.Value.Value, Is.EqualTo("value1"));

            dict.Add("key2", "value2");

            Assert.That(dict.innerDict.Count, Is.EqualTo(2));
            Assert.That(dict.innerDict.ContainsKey("key2"), Is.True);
            internalNode = dict.innerDict["key2"];
            Assert.That(internalNode.Value.Key, Is.EqualTo("key2"));
            Assert.That(internalNode.Value.Value, Is.EqualTo("value2"));
        }

        [Test]
        public void ContainsKeyTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            Assert.That(dict.ContainsKey("key1"), Is.True);
            Assert.That(dict.ContainsKey("key3"), Is.True);
            Assert.That(dict.ContainsKey("value1"), Is.False);
            Assert.That(dict.ContainsKey("hogehoge"), Is.False);
        }

        [Test]
        public void ContainsTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            Assert.That(dict.Contains(new KeyValuePair<string, string>("key1", "value1")), Is.True);
            Assert.That(dict.Contains(new KeyValuePair<string, string>("key3", "value2")), Is.False);
            Assert.That(dict.Contains(new KeyValuePair<string, string>("value3", "key3")), Is.False);
            Assert.That(dict.Contains(new KeyValuePair<string, string>("hogehoge", "hogehoge")), Is.False);
        }

        [Test]
        public void RemoveTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            var ret = dict.Remove("key1");

            Assert.That(ret, Is.True);
            Assert.That(dict.innerDict.Count, Is.EqualTo(2));
            Assert.That(dict.innerList.Count, Is.EqualTo(2));
            Assert.That(dict.innerDict.ContainsKey("key1"), Is.False);
            Assert.That(dict.innerDict.ContainsKey("key2"), Is.True);
            Assert.That(dict.innerDict.ContainsKey("key3"), Is.True);

            dict.Remove("key2");
            dict.Remove("key3");

            Assert.That(dict.innerDict, Is.Empty);
            Assert.That(dict.innerList, Is.Empty);

            ret = dict.Remove("hogehoge");
            Assert.That(ret, Is.False);
        }

        [Test]
        public void Remove2Test()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            var ret = dict.Remove(new KeyValuePair<string, string>("key1", "value1"));

            Assert.That(ret, Is.True);
            Assert.That(dict.innerDict.Count, Is.EqualTo(2));
            Assert.That(dict.innerList.Count, Is.EqualTo(2));
            Assert.That(dict.innerDict.ContainsKey("key1"), Is.False);
            Assert.That(dict.innerDict.ContainsKey("key2"), Is.True);
            Assert.That(dict.innerDict.ContainsKey("key3"), Is.True);

            ret = dict.Remove(new KeyValuePair<string, string>("key2", "hogehoge"));
            Assert.That(ret, Is.False);

            dict.Remove(new KeyValuePair<string, string>("key2", "value2"));
            dict.Remove(new KeyValuePair<string, string>("key3", "value3"));

            Assert.That(dict.innerDict, Is.Empty);
            Assert.That(dict.innerList, Is.Empty);

            ret = dict.Remove(new KeyValuePair<string, string>("hogehoge", "hogehoge"));
            Assert.That(ret, Is.False);
        }

        [Test]
        public void GetterTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            Assert.That(dict["key1"], Is.EqualTo("value1"));
            Assert.That(dict["key2"], Is.EqualTo("value2"));
            Assert.That(dict["key3"], Is.EqualTo("value3"));

            Assert.Throws<KeyNotFoundException>(() => { var x = dict["hogehoge"]; });
        }

        [Test]
        public void SetterTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            dict["key1"] = "foo";
            Assert.That(dict.innerDict["key1"].Value.Value, Is.EqualTo("foo"));

            dict["hogehoge"] = "bar";
            Assert.That(dict.innerDict.ContainsKey("hogehoge"), Is.True);
            Assert.That(dict.innerDict["hogehoge"].Value.Value, Is.EqualTo("bar"));
        }

        [Test]
        public void TryGetValueTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            string value;
            var ret = dict.TryGetValue("key1", out value);
            Assert.That(ret, Is.True);
            Assert.That(value, Is.EqualTo("value1"));

            ret = dict.TryGetValue("hogehoge", out value);
            Assert.That(ret, Is.False);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void KeysTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            Assert.That(dict.Keys, Is.EquivalentTo(new[] { "key1", "key2", "key3" }));

            dict.Add("foo", "bar");
            Assert.That(dict.Keys, Is.EquivalentTo(new[] { "key1", "key2", "key3", "foo" }));

            dict.Remove("key2");
            Assert.That(dict.Keys, Is.EquivalentTo(new[] { "key1", "key3", "foo" }));

            dict.Clear();
            Assert.That(dict.Keys, Is.Empty);
        }

        [Test]
        public void ValuesTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            Assert.That(dict.Values, Is.EquivalentTo(new[] { "value1", "value2", "value3" }));

            dict.Add("foo", "bar");
            Assert.That(dict.Values, Is.EquivalentTo(new[] { "value1", "value2", "value3", "bar" }));

            dict.Remove("key2");
            Assert.That(dict.Values, Is.EquivalentTo(new[] { "value1", "value3", "bar" }));

            dict.Clear();
            Assert.That(dict.Values, Is.Empty);
        }

        [Test]
        public void CountTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            Assert.That(dict.Count, Is.EqualTo(0));

            dict.Add("key1", "value1");
            Assert.That(dict.Count, Is.EqualTo(1));

            dict.Add("key2", "value2");
            Assert.That(dict.Count, Is.EqualTo(2));

            dict.Remove("key1");
            Assert.That(dict.Count, Is.EqualTo(1));

            dict.Clear();
            Assert.That(dict.Count, Is.EqualTo(0));
        }

        [Test]
        public void CopyToTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            var array = new KeyValuePair<string, string>[5];
            dict.CopyTo(array, 0);
            Assert.That(array[0], Is.EqualTo(new KeyValuePair<string, string>("key1", "value1")));
            Assert.That(array[1], Is.EqualTo(new KeyValuePair<string, string>("key2", "value2")));
            Assert.That(array[2], Is.EqualTo(new KeyValuePair<string, string>("key3", "value3")));
            Assert.That(array[3], Is.EqualTo(new KeyValuePair<string, string>()));
            Assert.That(array[4], Is.EqualTo(new KeyValuePair<string, string>()));

            array = new KeyValuePair<string, string>[5];
            dict.CopyTo(array, 1);
            Assert.That(array[0], Is.EqualTo(new KeyValuePair<string, string>()));
            Assert.That(array[1], Is.EqualTo(new KeyValuePair<string, string>("key1", "value1")));
            Assert.That(array[2], Is.EqualTo(new KeyValuePair<string, string>("key2", "value2")));
            Assert.That(array[3], Is.EqualTo(new KeyValuePair<string, string>("key3", "value3")));
            Assert.That(array[4], Is.EqualTo(new KeyValuePair<string, string>()));

            array = new KeyValuePair<string, string>[5];
            Assert.Throws<ArgumentException>(() => dict.CopyTo(array, 3));
            Assert.Throws<ArgumentException>(() => dict.CopyTo(array, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => dict.CopyTo(array, -1));
            Assert.Throws<ArgumentNullException>(() => dict.CopyTo(null, 0));
        }

        [Test]
        public void IsReadOnlyTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            Assert.That(dict.IsReadOnly, Is.False);
        }

        [Test]
        public void EnumeratorTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            var enumerator = dict.GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, string>("key1", "value1")));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, string>("key2", "value2")));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, string>("key3", "value3")));
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void Enumerator2Test()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"},
            };

            var enumerator = ((IEnumerable)dict).GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, string>("key1", "value1")));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, string>("key2", "value2")));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, string>("key3", "value3")));
            Assert.That(enumerator.MoveNext(), Is.False);
        }
    }
}
