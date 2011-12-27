// Tween - Client of Twitter
// Copyright (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
// All rights reserved.
//
// This file is part of Tween.
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
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Tween
{
    internal static class DynamicExpression
    {
        public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
        {
            var TweenDynamicExpression = Assembly.GetEntryAssembly().GetType("Tween.DynamicExpression");
            var method = TweenDynamicExpression.GetMethod("ParseLambda", new[] { typeof(Type), typeof(Type), typeof(string), typeof(object[]) });
            return (Expression<Func<T, S>>)method.Invoke(null, new object[] { typeof(T), typeof(S), expression, values });
        }
    }

    internal class TweenMain : Form
    {
        private static dynamic instance = Assembly.GetEntryAssembly().GetType("System.Windows.Form.Application").GetProperty("OpenForms").GetValue(null, new object[] {0});

        public Twitter TwitterInstance
        {
            get
            {
                return instance.TwitterInstance;
            }
        }

        public void OpenUriAsync(string uri)
        {
            instance.OpenUriAsync(uri);
        }

        public static bool IsNetworkAvailable()
        {
            var TweenMain = Assembly.GetEntryAssembly().GetType("Tween.TweenMain");
            var property = TweenMain.GetProperty("IsNetworkAvailable", typeof(bool));
            return (bool)property.GetValue(null, null);
        }
    }
}