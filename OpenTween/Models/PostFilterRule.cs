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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenTween.Models
{
    /// <summary>
    /// タブで使用する振り分けルールを表すクラス
    /// </summary>
    [XmlType("FiltersClass")]
    public class PostFilterRule : NotifyPropertyChangedBase, IEquatable<PostFilterRule>
    {
        /// <summary>
        /// Compile() メソッドの呼び出しが必要な状態か否か
        /// </summary>
        [XmlIgnore]
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// コンパイルされた振り分けルール
        /// </summary>
        [XmlIgnore]
        protected Func<PostClass, MyCommon.HITRESULT>? filterDelegate;

        /// <summary>
        /// 振り分けルールの概要
        /// </summary>
        [XmlIgnore]
        public string SummaryText
            => this.MakeSummary();

        /// <summary>
        /// ExecFilter() メソッドの実行時に自動でコンパイルを実行する
        /// </summary>
        /// <remarks>
        /// テスト用途以外では AutoCompile に頼らず事前に Compile() メソッドを呼び出すこと
        /// </remarks>
        internal static bool AutoCompile { get; set; }

        public bool Enabled
        {
            get => this.enabled;
            set => this.SetProperty(ref this.enabled, value);
        }

        private bool enabled;

        [XmlElement("NameFilter")]
        public string? FilterName
        {
            get => this.filterName;
            set => this.SetProperty(ref this.filterName, value);
        }

        private string? filterName;

        [XmlElement("ExNameFilter")]
        public string? ExFilterName
        {
            get => this.exFilterName;
            set => this.SetProperty(ref this.exFilterName, value);
        }

        private string? exFilterName;

        [XmlArray("BodyFilterArray")]
        public string[] FilterBody
        {
            get => this.filterBody;
            set => this.SetProperty(ref this.filterBody, value ?? throw new ArgumentNullException(nameof(value)));
        }

        private string[] filterBody = Array.Empty<string>();

        [XmlArray("ExBodyFilterArray")]
        public string[] ExFilterBody
        {
            get => this.exFilterBody;
            set => this.SetProperty(ref this.exFilterBody, value ?? throw new ArgumentNullException(nameof(value)));
        }

        private string[] exFilterBody = Array.Empty<string>();

        [XmlElement("SearchBoth")]
        public bool UseNameField
        {
            get => this.useNameField;
            set => this.SetProperty(ref this.useNameField, value);
        }

        private bool useNameField;

        [XmlElement("ExSearchBoth")]
        public bool ExUseNameField
        {
            get => this.exUseNameField;
            set => this.SetProperty(ref this.exUseNameField, value);
        }

        private bool exUseNameField;

        [XmlElement("MoveFrom")]
        public bool MoveMatches
        {
            get => this.moveMatches;
            set => this.SetProperty(ref this.moveMatches, value);
        }

        private bool moveMatches;

        [XmlElement("SetMark")]
        public bool MarkMatches
        {
            get => this.markMatches;
            set => this.SetProperty(ref this.markMatches, value);
        }

        private bool markMatches;

        [XmlElement("SearchUrl")]
        public bool FilterByUrl
        {
            get => this.filterByUrl;
            set => this.SetProperty(ref this.filterByUrl, value);
        }

        private bool filterByUrl;

        [XmlElement("ExSearchUrl")]
        public bool ExFilterByUrl
        {
            get => this.exFilterByUrl;
            set => this.SetProperty(ref this.exFilterByUrl, value);
        }

        private bool exFilterByUrl;

        public bool CaseSensitive
        {
            get => this.caseSensitive;
            set => this.SetProperty(ref this.caseSensitive, value);
        }

        private bool caseSensitive;

        public bool ExCaseSensitive
        {
            get => this.exCaseSensitive;
            set => this.SetProperty(ref this.exCaseSensitive, value);
        }

        private bool exCaseSensitive;

        public bool UseLambda
        {
            get => this.useLambda;
            set => this.SetProperty(ref this.useLambda, value);
        }

        private bool useLambda;

        public bool ExUseLambda
        {
            get => this.exUseLambda;
            set => this.SetProperty(ref this.exUseLambda, value);
        }

        private bool exUseLambda;

        public bool UseRegex
        {
            get => this.useRegex;
            set => this.SetProperty(ref this.useRegex, value);
        }

        private bool useRegex;

        public bool ExUseRegex
        {
            get => this.exUseRegex;
            set => this.SetProperty(ref this.exUseRegex, value);
        }

        private bool exUseRegex;

        [XmlElement("IsRt")]
        public bool FilterRt
        {
            get => this.filterRt;
            set => this.SetProperty(ref this.filterRt, value);
        }

        private bool filterRt;

        [XmlElement("IsExRt")]
        public bool ExFilterRt
        {
            get => this.exFilterRt;
            set => this.SetProperty(ref this.exFilterRt, value);
        }

        private bool exFilterRt;

        [XmlElement("Source")]
        public string? FilterSource
        {
            get => this.filterSource;
            set => this.SetProperty(ref this.filterSource, value);
        }

        private string? filterSource;

        [XmlElement("ExSource")]
        public string? ExFilterSource
        {
            get => this.exFilterSource;
            set => this.SetProperty(ref this.exFilterSource, value);
        }

        private string? exFilterSource;

        public PostFilterRule()
        {
            this.IsDirty = true;

            this.Enabled = true;
            this.MarkMatches = true;
            this.UseNameField = true;
            this.ExUseNameField = true;
        }

        // TODO: TabsClass とかの改修が終わるまでデフォルト有効
        static PostFilterRule()
            => PostFilterRule.AutoCompile = true;

        /// <summary>
        /// 振り分けルールをコンパイルします
        /// </summary>
        public void Compile()
        {
            if (!this.Enabled)
            {
                this.filterDelegate = x => MyCommon.HITRESULT.None;
                this.IsDirty = false;
                return;
            }

            var postParam = Expression.Parameter(typeof(PostClass), "x");

            var matchExpr = this.MakeFiltersExpr(
                postParam,
                this.FilterName,
                this.FilterBody,
                this.FilterSource,
                this.FilterRt,
                this.UseRegex,
                this.CaseSensitive,
                this.UseNameField,
                this.UseLambda,
                this.FilterByUrl);

            var excludeExpr = this.MakeFiltersExpr(
                postParam,
                this.ExFilterName,
                this.ExFilterBody,
                this.ExFilterSource,
                this.ExFilterRt,
                this.ExUseRegex,
                this.ExCaseSensitive,
                this.ExUseNameField,
                this.ExUseLambda,
                this.ExFilterByUrl);

            Expression<Func<PostClass, MyCommon.HITRESULT>> filterExpr;

            if (matchExpr != null)
            {
                MyCommon.HITRESULT hitResult;

                if (this.MoveMatches)
                    hitResult = MyCommon.HITRESULT.Move;
                else if (this.MarkMatches)
                    hitResult = MyCommon.HITRESULT.CopyAndMark;
                else
                    hitResult = MyCommon.HITRESULT.Copy;

                if (excludeExpr != null)
                {
                    // x => matchExpr ? (!excludeExpr ? hitResult : None) : None
                    filterExpr =
                        Expression.Lambda<Func<PostClass, MyCommon.HITRESULT>>(
                            Expression.Condition(
                                matchExpr,
                                Expression.Condition(
                                    Expression.Not(excludeExpr),
                                    Expression.Constant(hitResult),
                                    Expression.Constant(MyCommon.HITRESULT.None)),
                                Expression.Constant(MyCommon.HITRESULT.None)),
                            postParam);
                }
                else
                {
                    // x => matchExpr ? hitResult : None
                    filterExpr =
                        Expression.Lambda<Func<PostClass, MyCommon.HITRESULT>>(
                            Expression.Condition(
                                matchExpr,
                                Expression.Constant(hitResult),
                                Expression.Constant(MyCommon.HITRESULT.None)),
                            postParam);
                }
            }
            else if (excludeExpr != null)
            {
                // x => excludeExpr ? Exclude : None
                filterExpr =
                    Expression.Lambda<Func<PostClass, MyCommon.HITRESULT>>(
                        Expression.Condition(
                            excludeExpr,
                            Expression.Constant(MyCommon.HITRESULT.Exclude),
                            Expression.Constant(MyCommon.HITRESULT.None)),
                        postParam);
            }
            else // matchExpr == null && excludeExpr == null
            {
                filterExpr = x => MyCommon.HITRESULT.None;
            }

            this.filterDelegate = filterExpr.Compile();
            this.IsDirty = false;
        }

        protected virtual Expression? MakeFiltersExpr(
            ParameterExpression postParam,
            string? filterName,
            string[] filterBody,
            string? filterSource,
            bool filterRt,
            bool useRegex,
            bool caseSensitive,
            bool useNameField,
            bool useLambda,
            bool filterByUrl)
        {
            var filterExprs = new List<Expression>();

            if (useNameField && !MyCommon.IsNullOrEmpty(filterName))
            {
                filterExprs.Add(Expression.OrElse(
                    this.MakeGenericFilter(postParam, "ScreenName", filterName, useRegex, caseSensitive, exactMatch: true),
                    Expression.AndAlso(
                        Expression.Property(postParam, "IsRetweet"),
                        this.MakeGenericFilter(postParam, "RetweetedBy", filterName, useRegex, caseSensitive, exactMatch: true)
                    )
                ));
            }
            foreach (var body in filterBody)
            {
                if (MyCommon.IsNullOrEmpty(body))
                    continue;

                Expression bodyExpr;
                if (useLambda)
                {
                    // TODO DynamicQuery相当のGPLv3互換なライブラリで置換する
                    Expression<Func<PostClass, bool>> lambdaExpr = x => false;
                    bodyExpr = lambdaExpr.Body;
                }
                else
                {
                    if (filterByUrl)
                        bodyExpr = this.MakeGenericFilter(postParam, "Text", body, useRegex, caseSensitive);
                    else
                        bodyExpr = this.MakeGenericFilter(postParam, "TextFromApi", body, useRegex, caseSensitive);

                    // useNameField = false の場合は ScreenName と RetweetedBy も filterBody のマッチ対象となる
                    if (!useNameField)
                    {
                        bodyExpr = Expression.OrElse(
                            bodyExpr,
                            this.MakeGenericFilter(postParam, "ScreenName", body, useRegex, caseSensitive, exactMatch: true));

                        // bodyExpr || x.IsRetweet && <MakeGenericFilter()>
                        bodyExpr = Expression.OrElse(
                            bodyExpr,
                            Expression.AndAlso(
                                Expression.Property(postParam, "IsRetweet"),
                                this.MakeGenericFilter(postParam, "RetweetedBy", body, useRegex, caseSensitive, exactMatch: true)));
                    }
                }

                filterExprs.Add(bodyExpr);
            }
            if (!MyCommon.IsNullOrEmpty(filterSource))
            {
                if (filterByUrl)
                    filterExprs.Add(this.MakeGenericFilter(postParam, "SourceHtml", filterSource, useRegex, caseSensitive));
                else
                    filterExprs.Add(this.MakeGenericFilter(postParam, "Source", filterSource, useRegex, caseSensitive, exactMatch: true));
            }
            if (filterRt)
            {
                // x.IsRetweet
                filterExprs.Add(Expression.Property(postParam, "IsRetweet"));
            }

            if (filterExprs.Count == 0)
            {
                return null;
            }
            else
            {
                // filterExpr[0] && filterExpr[1] && ...
                var filterExpr = filterExprs[0];
                foreach (var expr in filterExprs.Skip(1))
                {
                    filterExpr = Expression.AndAlso(filterExpr, expr);
                }

                return filterExpr;
            }
        }

        protected Expression MakeGenericFilter(
            ParameterExpression postParam,
            string targetFieldName,
            string pattern,
            bool useRegex,
            bool caseSensitive,
            bool exactMatch = false)
        {
            // x.<targetFieldName>
            var targetField = Expression.Property(
                postParam,
                typeof(PostClass).GetProperty(targetFieldName));

            // targetField ?? ""
            var targetValue = Expression.Coalesce(targetField, Expression.Constant(string.Empty));

            if (useRegex)
            {
                var regex = new Regex(pattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);

                // regex.IsMatch(targetField)
                return Expression.Call(
                    Expression.Constant(regex),
                    typeof(Regex).GetMethod("IsMatch", new[] { typeof(string) }),
                    targetValue);
            }
            else
            {
                var compOpt = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (exactMatch)
                {
                    // 完全一致
                    // pattern.Equals(targetField, compOpt)
                    return Expression.Call(
                        Expression.Constant(pattern),
                        typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(StringComparison) }),
                        targetValue,
                        Expression.Constant(compOpt));
                }
                else
                {
                    // 部分一致
                    // targetField.IndexOf(pattern, compOpt) != -1
                    return Expression.NotEqual(
                        Expression.Call(
                            targetValue,
                            typeof(string).GetMethod("IndexOf", new[] { typeof(string), typeof(StringComparison) }),
                            Expression.Constant(pattern),
                            Expression.Constant(compOpt)),
                        Expression.Constant(-1));
                }
            }
        }

        /// <summary>
        /// ツイートの振り分けを実行する
        /// </summary>
        /// <param name="post">振り分けるツイート</param>
        /// <returns>振り分け結果</returns>
        public MyCommon.HITRESULT ExecFilter(PostClass post)
        {
            if (this.IsDirty)
            {
                if (!PostFilterRule.AutoCompile)
                    throw new InvalidOperationException("振り分け実行前に Compile() を呼び出す必要があります");

                this.Compile();
            }

            return this.filterDelegate!(post);
        }

        public PostFilterRule Clone()
        {
            return new PostFilterRule
            {
                FilterBody = this.FilterBody,
                FilterName = this.FilterName,
                FilterSource = this.FilterSource,
                FilterRt = this.FilterRt,
                FilterByUrl = this.FilterByUrl,
                UseLambda = this.UseLambda,
                UseNameField = this.UseNameField,
                UseRegex = this.UseRegex,
                CaseSensitive = this.CaseSensitive,
                ExFilterBody = this.ExFilterBody,
                ExFilterName = this.ExFilterName,
                ExFilterSource = this.ExFilterSource,
                ExFilterRt = this.ExFilterRt,
                ExFilterByUrl = this.ExFilterByUrl,
                ExUseLambda = this.ExUseLambda,
                ExUseNameField = this.ExUseNameField,
                ExUseRegex = this.ExUseRegex,
                ExCaseSensitive = this.ExCaseSensitive,
                MoveMatches = this.MoveMatches,
                MarkMatches = this.MarkMatches,
            };
        }

        /// <summary>
        /// 振り分けルールの文字列表現を返します
        /// </summary>
        /// <returns>振り分けルールの概要</returns>
        public override string ToString()
            => this.SummaryText;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.IsDirty = true;
            base.OnPropertyChanged(e);
        }

        #region from Tween v1.1.0.0

        // The code in this region block is based on code written by the following authors:
        //   (C) 2007 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
        //   (C) 2008 Moz (@syo68k)

        /// <summary>
        /// フィルタ一覧に表示する文言生成
        /// </summary>
        protected virtual string MakeSummary()
        {
            var fs = new StringBuilder();
            if (!this.Enabled)
            {
                fs.Append("<");
                fs.Append(Properties.Resources.Disabled);
                fs.Append("> ");
            }
            if (this.HasMatchConditions())
            {
                if (this.UseNameField)
                {
                    if (!MyCommon.IsNullOrEmpty(this.FilterName))
                    {
                        fs.AppendFormat(Properties.Resources.SetFiltersText1, this.FilterName);
                    }
                    else
                    {
                        fs.Append(Properties.Resources.SetFiltersText2);
                    }
                }
                if (this.FilterBody.Length > 0)
                {
                    fs.Append(Properties.Resources.SetFiltersText3);
                    foreach (var bf in this.FilterBody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length--;
                    fs.Append(Properties.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (this.UseNameField)
                {
                    fs.Append(Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Properties.Resources.SetFiltersText6);
                }
                if (this.UseRegex)
                {
                    fs.Append(Properties.Resources.SetFiltersText7);
                }
                if (this.FilterByUrl)
                {
                    fs.Append(Properties.Resources.SetFiltersText8);
                }
                if (this.CaseSensitive)
                {
                    fs.Append(Properties.Resources.SetFiltersText13);
                }
                if (this.FilterRt)
                {
                    fs.Append("RT/");
                }
                if (this.UseLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!MyCommon.IsNullOrEmpty(this.FilterSource))
                {
                    fs.AppendFormat("Src…{0}/", this.FilterSource);
                }
                fs.Length--;
                fs.Append(")");
            }
            if (this.HasExcludeConditions())
            {
                // 除外
                fs.Append(Properties.Resources.SetFiltersText12);
                if (this.ExUseNameField)
                {
                    if (!MyCommon.IsNullOrEmpty(this.ExFilterName))
                    {
                        fs.AppendFormat(Properties.Resources.SetFiltersText1, this.ExFilterName);
                    }
                    else
                    {
                        fs.Append(Properties.Resources.SetFiltersText2);
                    }
                }
                if (this.ExFilterBody.Length > 0)
                {
                    fs.Append(Properties.Resources.SetFiltersText3);
                    foreach (var bf in this.ExFilterBody)
                    {
                        fs.Append(bf);
                        fs.Append(" ");
                    }
                    fs.Length--;
                    fs.Append(Properties.Resources.SetFiltersText4);
                }
                fs.Append("(");
                if (this.ExUseNameField)
                {
                    fs.Append(Properties.Resources.SetFiltersText5);
                }
                else
                {
                    fs.Append(Properties.Resources.SetFiltersText6);
                }
                if (this.ExUseRegex)
                {
                    fs.Append(Properties.Resources.SetFiltersText7);
                }
                if (this.ExFilterByUrl)
                {
                    fs.Append(Properties.Resources.SetFiltersText8);
                }
                if (this.ExCaseSensitive)
                {
                    fs.Append(Properties.Resources.SetFiltersText13);
                }
                if (this.ExFilterRt)
                {
                    fs.Append("RT/");
                }
                if (this.ExUseLambda)
                {
                    fs.Append("LambdaExp/");
                }
                if (!MyCommon.IsNullOrEmpty(this.ExFilterSource))
                {
                    fs.AppendFormat("Src…{0}/", this.ExFilterSource);
                }
                fs.Length--;
                fs.Append(")");
            }

            fs.Append("(");
            if (this.MoveMatches)
            {
                fs.Append(Properties.Resources.SetFiltersText9);
            }
            else
            {
                fs.Append(Properties.Resources.SetFiltersText11);
            }
            if (!this.MoveMatches && this.MarkMatches)
            {
                fs.Append(Properties.Resources.SetFiltersText10);
            }
            else if (!this.MoveMatches)
            {
                fs.Length--;
            }

            fs.Append(")");

            return fs.ToString();
        }
        #endregion

        /// <summary>
        /// この振り分けルールにマッチ条件が含まれているかを返します
        /// </summary>
        public bool HasMatchConditions()
            => !MyCommon.IsNullOrEmpty(this.FilterName) ||
                this.FilterBody.Any(x => !MyCommon.IsNullOrEmpty(x)) ||
                !MyCommon.IsNullOrEmpty(this.FilterSource) ||
                this.FilterRt;

        /// <summary>
        /// この振り分けルールに除外条件が含まれているかを返します
        /// </summary>
        public bool HasExcludeConditions()
            => !MyCommon.IsNullOrEmpty(this.ExFilterName) ||
                this.ExFilterBody.Any(x => !MyCommon.IsNullOrEmpty(x)) ||
                !MyCommon.IsNullOrEmpty(this.ExFilterSource) ||
                this.ExFilterRt;

        public override bool Equals(object? obj)
            => this.Equals(obj as PostFilterRule);

        public bool Equals(PostFilterRule? other)
        {
            if (other == null)
                return false;

            if (other.HasMatchConditions() || this.HasMatchConditions())
            {
                if (other.FilterName != this.FilterName ||
                    !other.FilterBody.SequenceEqual(this.FilterBody) ||
                    other.FilterSource != this.FilterSource ||
                    other.FilterRt != this.FilterRt ||
                    other.FilterByUrl != this.FilterByUrl ||
                    other.CaseSensitive != this.CaseSensitive ||
                    other.UseNameField != this.UseNameField ||
                    other.UseLambda != this.UseLambda ||
                    other.UseRegex != this.UseRegex)
                {
                    return false;
                }
            }

            if (other.HasExcludeConditions() || this.HasExcludeConditions())
            {
                if (other.ExFilterName != this.ExFilterName ||
                    !other.ExFilterBody.SequenceEqual(this.ExFilterBody) ||
                    other.ExFilterSource != this.ExFilterSource ||
                    other.ExFilterRt != this.ExFilterRt ||
                    other.ExFilterByUrl != this.ExFilterByUrl ||
                    other.ExCaseSensitive != this.ExCaseSensitive ||
                    other.ExUseNameField != this.ExUseNameField ||
                    other.ExUseLambda != this.ExUseLambda ||
                    other.ExUseRegex != this.ExUseRegex)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
            => this.FilterName?.GetHashCode() ?? 0 ^
                this.FilterSource?.GetHashCode() ?? 0 ^
                this.FilterBody.Select(x => x?.GetHashCode() ?? 0).Sum() ^
                this.ExFilterName?.GetHashCode() ?? 0 ^
                this.ExFilterSource?.GetHashCode() ?? 0 ^
                this.ExFilterBody.Select(x => x?.GetHashCode() ?? 0).Sum();
    }
}
