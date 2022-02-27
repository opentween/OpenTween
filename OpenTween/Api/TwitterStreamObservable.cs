// OpenTween - Client of Twitter
// Copyright (c) 2018 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OpenTween.Api.DataModel;

namespace OpenTween.Api
{
    public class TwitterStreamObservable : IObservable<ITwitterStreamMessage>
    {
        private readonly Func<Task<Stream>> streamOpener;

        public TwitterStreamObservable(Func<Task<Stream>> streamOpener)
            => this.streamOpener = streamOpener;

        public IDisposable Subscribe(IObserver<ITwitterStreamMessage> observer)
        {
            var cts = new CancellationTokenSource();

            this.StreamLoop(observer, cts.Token);

            return new Unsubscriber(cts);
        }

        private async void StreamLoop(IObserver<ITwitterStreamMessage> observer, CancellationToken cancellationToken)
        {
            try
            {
                using var stream = await this.streamOpener().ConfigureAwait(false);
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var line = await reader.ReadLineAsync()
                        .ConfigureAwait(false);

                    var message = ParseLine(line);

                    observer.OnNext(message);
                }
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        }

        public static ITwitterStreamMessage ParseLine(string line)
        {
            if (MyCommon.IsNullOrEmpty(line))
                return new StreamMessageKeepAlive();

            if (line.First() != '{' || line.Last() != '}')
            {
                MyCommon.TraceOut("Invalid JSON (ParseLine):" + Environment.NewLine + line);
                return new StreamMessageUnknown(line);
            }

            try
            {
                var bytes = Encoding.UTF8.GetBytes(line);
                using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(bytes, XmlDictionaryReaderQuotas.Max);
                var xElm = XElement.Load(jsonReader);

                if (xElm.Element("text") != null)
                    return StreamMessageStatus.ParseJson(line);

                return new StreamMessageUnknown(line);
            }
            catch (XmlException)
            {
                MyCommon.TraceOut("XmlException (ParseLine): " + line);
                return new StreamMessageUnknown(line);
            }
            catch (SerializationException)
            {
                MyCommon.TraceOut("SerializationException (ParseLine): " + line);
                return new StreamMessageUnknown(line);
            }
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly CancellationTokenSource cts;

            public Unsubscriber(CancellationTokenSource cts)
                => this.cts = cts;

            public void Dispose()
                => this.cts.Cancel();
        }
    }
}
