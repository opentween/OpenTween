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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class LRUCacheDictionaryTest
    {
        private static AnyOrderComparer<string> collComparer = AnyOrderComparer<string>.Instance;

        [Fact]
        public void InnerListTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            var node = dict.innerList.First;
            Assert.Equal("key3", node.Value.Key);
            node = node.Next;
            Assert.Equal("key2", node.Value.Key);
            node = node.Next;
            Assert.Equal("key1", node.Value.Key);

            // 2 -> 3 -> 1 の順に値を参照
            var x = dict["key2"];
            x = dict["key3"];
            x = dict["key1"];

            // 直近に参照した順で並んでいるかテスト
            node = dict.innerList.First;
            Assert.Equal("key1", node.Value.Key);
            node = node.Next;
            Assert.Equal("key3", node.Value.Key);
            node = node.Next;
            Assert.Equal("key2", node.Value.Key);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void TrimLimitTest(int trimLimit)
        {
            var dict = new LRUCacheDictionary<string, string>()
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            dict.TrimLimit = trimLimit;
            var ret = dict.Trim();

            if (trimLimit >= 3)
            {
                // trimLimit がアイテムの件数より大きい場合、Trim メソッドは動作せずに false を返す。
                Assert.False(ret);
                Assert.Equal(3, dict.Count);
            }
            else
            {
                Assert.True(ret);
                Assert.Equal(trimLimit, dict.Count);
            }
        }

        [Fact]
        public void TrimOrderTest()
        {
            var dict = new LRUCacheDictionary<string, string>()
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
                ["key4"] = "value4",
                ["key5"] = "value5",
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
            Assert.True(dict.ContainsKey("key1"));
            Assert.False(dict.ContainsKey("key2"));
            Assert.True(dict.ContainsKey("key3"));
            Assert.False(dict.ContainsKey("key4"));
            Assert.True(dict.ContainsKey("key5"));
        }

        [Fact]
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
            Assert.Equal<IEnumerable<string>>(new[] { "key2", "key3", "key4" }, dict.innerDict.Keys, collComparer);

            dict["key5"] = "value5";         // 5アクセス目
            dict.Add("key6", "value6");      // 6アクセス目
            var x = dict["key2"];            // 7アクセス目
            dict.TryGetValue("key4", out x); // 8アクセス目 (この直後にTrim)

            // 5 -> 6 -> 2 -> 4 の順でアクセスしたため、直近 3 件の 6, 2, 4 だけが残る
            Assert.Equal<IEnumerable<string>>(new[] { "key6", "key2", "key4" }, dict.innerDict.Keys, collComparer);
        }

        [Fact]
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
            Assert.Equal<IEnumerable<string>>(new[] { "key1", "key2" }, removedList, collComparer);
        }

        // ここから下は IDictionary としての機能が正しく動作するかのテスト

        [Fact]
        public void AddTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            dict.Add("key1", "value1");

            Assert.Equal(1, dict.innerDict.Count);
            Assert.True(dict.innerDict.ContainsKey("key1"));
            var internalNode = dict.innerDict["key1"];
            Assert.Equal("key1", internalNode.Value.Key);
            Assert.Equal("value1", internalNode.Value.Value);

            dict.Add("key2", "value2");

            Assert.Equal(2, dict.innerDict.Count);
            Assert.True(dict.innerDict.ContainsKey("key2"));
            internalNode = dict.innerDict["key2"];
            Assert.Equal("key2", internalNode.Value.Key);
            Assert.Equal("value2", internalNode.Value.Value);
        }

        [Fact]
        public void ContainsKeyTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            Assert.True(dict.ContainsKey("key1"));
            Assert.True(dict.ContainsKey("key3"));
            Assert.False(dict.ContainsKey("value1"));
            Assert.False(dict.ContainsKey("hogehoge"));
        }

        [Fact]
        public void ContainsTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            Assert.True(dict.Contains(new KeyValuePair<string, string>("key1", "value1")));
            Assert.False(dict.Contains(new KeyValuePair<string, string>("key3", "value2")));
            Assert.False(dict.Contains(new KeyValuePair<string, string>("value3", "key3")));
            Assert.False(dict.Contains(new KeyValuePair<string, string>("hogehoge", "hogehoge")));
        }

        [Fact]
        public void RemoveTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            var ret = dict.Remove("key1");

            Assert.True(ret);
            Assert.Equal(2, dict.innerDict.Count);
            Assert.Equal(2, dict.innerList.Count);
            Assert.False(dict.innerDict.ContainsKey("key1"));
            Assert.True(dict.innerDict.ContainsKey("key2"));
            Assert.True(dict.innerDict.ContainsKey("key3"));

            dict.Remove("key2");
            dict.Remove("key3");

            Assert.Empty(dict.innerDict);
            Assert.Empty(dict.innerList);

            ret = dict.Remove("hogehoge");
            Assert.False(ret);
        }

        [Fact]
        public void Remove2Test()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            var ret = dict.Remove(new KeyValuePair<string, string>("key1", "value1"));

            Assert.True(ret);
            Assert.Equal(2, dict.innerDict.Count);
            Assert.Equal(2, dict.innerList.Count);
            Assert.False(dict.innerDict.ContainsKey("key1"));
            Assert.True(dict.innerDict.ContainsKey("key2"));
            Assert.True(dict.innerDict.ContainsKey("key3"));

            ret = dict.Remove(new KeyValuePair<string, string>("key2", "hogehoge"));
            Assert.False(ret);

            dict.Remove(new KeyValuePair<string, string>("key2", "value2"));
            dict.Remove(new KeyValuePair<string, string>("key3", "value3"));

            Assert.Empty(dict.innerDict);
            Assert.Empty(dict.innerList);

            ret = dict.Remove(new KeyValuePair<string, string>("hogehoge", "hogehoge"));
            Assert.False(ret);
        }

        public void GetterTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            Assert.Equal("value1", dict["key1"]);
            Assert.Equal("value2", dict["key2"]);
            Assert.Equal("value3", dict["key3"]);

            Assert.Throws<KeyNotFoundException>(() => { var x = dict["hogehoge"]; });
        }

        [Fact]
        public void SetterTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            dict["key1"] = "foo";
            Assert.Equal("foo", dict.innerDict["key1"].Value.Value);

            dict["hogehoge"] = "bar";
            Assert.True(dict.innerDict.ContainsKey("hogehoge"));
            Assert.Equal("bar", dict.innerDict["hogehoge"].Value.Value);
        }

        [Fact]
        public void TryGetValueTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            string value;
            var ret = dict.TryGetValue("key1", out value);
            Assert.True(ret);
            Assert.Equal("value1", value);

            ret = dict.TryGetValue("hogehoge", out value);
            Assert.False(ret);
            Assert.Null(value);
        }

        [Fact]
        public void KeysTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            Assert.Equal(new[] { "key1", "key2", "key3" }, dict.Keys, collComparer);

            dict.Add("foo", "bar");
            Assert.Equal(new[] { "key1", "key2", "key3", "foo" }, dict.Keys, collComparer);

            dict.Remove("key2");
            Assert.Equal(new[] { "key1", "key3", "foo" }, dict.Keys, collComparer);

            dict.Clear();
            Assert.Empty(dict.Keys);
        }

        [Fact]
        public void ValuesTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            Assert.Equal(new[] { "value1", "value2", "value3" }, dict.Values, collComparer);

            dict.Add("foo", "bar");
            Assert.Equal(new[] { "value1", "value2", "value3", "bar" }, dict.Values, collComparer);

            dict.Remove("key2");
            Assert.Equal(new[] { "value1", "value3", "bar" }, dict.Values, collComparer);

            dict.Clear();
            Assert.Empty(dict.Values);
        }

        [Fact]
        public void CountTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            Assert.Equal(0, dict.Count);

            dict.Add("key1", "value1");
            Assert.Equal(1, dict.Count);

            dict.Add("key2", "value2");
            Assert.Equal(2, dict.Count);

            dict.Remove("key1");
            Assert.Equal(1, dict.Count);

            dict.Clear();
            Assert.Equal(0, dict.Count);
        }

        [Fact]
        public void CopyToTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            var array = new KeyValuePair<string, string>[5];
            dict.CopyTo(array, 0);
            Assert.Equal(new KeyValuePair<string, string>("key1", "value1"), array[0]);
            Assert.Equal(new KeyValuePair<string, string>("key2", "value2"), array[1]);
            Assert.Equal(new KeyValuePair<string, string>("key3", "value3"), array[2]);
            Assert.Equal(new KeyValuePair<string, string>(), array[3]);
            Assert.Equal(new KeyValuePair<string, string>(), array[4]);

            array = new KeyValuePair<string, string>[5];
            dict.CopyTo(array, 1);
            Assert.Equal(new KeyValuePair<string, string>(), array[0]);
            Assert.Equal(new KeyValuePair<string, string>("key1", "value1"), array[1]);
            Assert.Equal(new KeyValuePair<string, string>("key2", "value2"), array[2]);
            Assert.Equal(new KeyValuePair<string, string>("key3", "value3"), array[3]);
            Assert.Equal(new KeyValuePair<string, string>(), array[4]);

            array = new KeyValuePair<string, string>[5];
            Assert.Throws<ArgumentException>(() => dict.CopyTo(array, 3));
            Assert.Throws<ArgumentException>(() => dict.CopyTo(array, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => dict.CopyTo(array, -1));
            Assert.Throws<ArgumentNullException>(() => dict.CopyTo(null, 0));
        }

        [Fact]
        public void IsReadOnlyTest()
        {
            var dict = new LRUCacheDictionary<string, string>();

            Assert.False(dict.IsReadOnly);
        }

        [Fact]
        public void EnumeratorTest()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            var enumerator = dict.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<string, string>("key1", "value1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<string, string>("key2", "value2"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<string, string>("key3", "value3"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Enumerator2Test()
        {
            var dict = new LRUCacheDictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
                ["key3"] = "value3",
            };

            var enumerator = ((IEnumerable)dict).GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<string, string>("key1", "value1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<string, string>("key2", "value2"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(new KeyValuePair<string, string>("key3", "value3"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }
    }
}
