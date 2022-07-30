using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("OpenTween (dev)")]
[assembly: AssemblyDescription("Client of Twitter. Free software(GPLv3)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("OpenTween (dev)")]
[assembly: AssemblyCopyright("(C) 2011 OpenTween contributors")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です
[assembly: Guid("2d0ae0ba-adac-49a2-9b10-26fd69e695bf")]

[assembly: AssemblyVersion("2.7.0.1")]

[assembly: InternalsVisibleTo("OpenTween.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // for Moq
