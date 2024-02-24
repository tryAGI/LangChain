using System.Net;
using System.Net.WebSockets;
using H.Engine.IO;
using H.Socket.IO.Utilities;
using H.WebSockets.Utilities;
using Newtonsoft.Json;
using EventGenerator;

namespace H.Socket.IO;

/// <summary>
/// Socket.IO Client.
/// </summary>
[Event<string, string, bool>("Connected", Description = "Occurs after a successful connection to each namespace.",
    PropertyNames = new []{ "Value", "Namespace", "IsHandled" })]
[Event<string, WebSocketCloseStatus?>("Disconnected", Description = "Occurs after a disconnection.",
    PropertyNames = new []{ "Reason", "Status" })]
[Event<string, string, bool>("EventReceived", Description = "Occurs after new event.",
    PropertyNames = new []{ "Value", "Namespace", "IsHandled" })]
[Event<string, string, bool>("HandledEventReceived", Description = "Occurs after new handled event(captured by any On).",
    PropertyNames = new []{ "Value", "Namespace", "IsHandled" })]
[Event<string, string, bool>("UnhandledEventReceived", Description = "Occurs after new unhandled event(not captured by any On).",
    PropertyNames = new []{ "Value", "Namespace", "IsHandled" })]
[Event<string, string>("ErrorReceived", Description = "Occurs after new error.",
    PropertyNames = new []{ "Value", "Namespace" })]
[Event<Exception>("ExceptionOccurred", Description = "Occurs after new exception.", PropertyNames = new []{ "Exception" })]
public sealed partial class SocketIoClient : IDisposable
#if NETSTANDARD2_1
    , IAsyncDisposable
#endif
{
    #region Properties

    /// <summary>
    /// Internal Engine.IO Client.
    /// </summary>
    public EngineIoClient EngineIoClient { get; private set; }

    /// <summary>
    /// Using proxy.
    /// </summary>
    public IWebProxy? Proxy
    {
        get => EngineIoClient.Proxy;
        set
        {
            EngineIoClient = EngineIoClient ?? throw new ObjectDisposedException(nameof(EngineIoClient));
            EngineIoClient.Proxy = value;
        }
    }

    /// <summary>
    /// An unique identifier for the socket session.. <br/>
    /// Set after the <seealso cref="Connected"/> event is triggered.
    /// </summary>
    public string? Id => EngineIoClient.Id;

    /// <summary>
    /// Will be sent with all messages(Unless otherwise stated). <br/>
    /// Also automatically connects to it. <br/>
    /// Default is <see langword="null"/>.
    /// </summary>
    public string? DefaultNamespace { get; set; }

    private Dictionary<string, List<(Action<object, string> Action, Type Type)>> JsonDeserializeActions { get; } = new();

    private Dictionary<string, List<Action<string>>> TextActions { get; } = new();

    private Dictionary<string, List<Action>> Actions { get; } = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Creates Engine.IO client internally.
    /// </summary>
    public SocketIoClient()
    {
        EngineIoClient = new EngineIoClient("socket.io");
        EngineIoClient.MessageReceived += EngineIoClient_MessageReceived;
        EngineIoClient.ExceptionOccurred += (_, args) => OnExceptionOccurred(args.Exception);
        EngineIoClient.Closed += (_, args) => OnDisconnected(args.Reason, args.Status);
    }

    #endregion

    #region Event Handlers

    private void EngineIoClient_MessageReceived(object? sender, EngineIoClient.MessageReceivedEventArgs args)
    {
        try
        {
            if (args?.Message == null)
            {
                throw new InvalidOperationException("Engine.IO message is null");
            }

            if (args.Message.Length < 1)
            {
                // ignore - it's Engine.IO message
                return;
            }

            var packet = SocketIoPacket.Decode(args.Message);
            switch (packet.Prefix)
            {
                case SocketIoPacket.ConnectPrefix:
                    OnConnected(value: string.Empty, @namespace: packet.Namespace, isHandled: false);
                    break;

                case SocketIoPacket.DisconnectPrefix:
                    OnDisconnected(
                        reason: "Received disconnect message from server",
                        status: null);
                    break;

                case SocketIoPacket.EventPrefix:
                    var isHandled = false;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(packet.Value))
                        {
                            break;
                        }

                        var values = packet.Value.GetJsonArrayValues();
                        var name = values.ElementAt(0);
                        var text = values.ElementAtOrDefault(1);

                        var key = GetOnKey(name, packet.Namespace);
                        if (Actions.TryGetValue(key, out var actions))
                        {
                            foreach (var action in actions)
                            {
                                isHandled = true;

                                try
                                {
                                    action?.Invoke();
                                }
                                catch (Exception exception)
                                {
                                    OnExceptionOccurred(exception);
                                }
                            }
                        }

                        if (TextActions.TryGetValue(key, out var textActions))
                        {
                            foreach (var action in textActions)
                            {
                                isHandled = true;

                                try
                                {
                                    if (text == null)
                                    {
                                        throw new InvalidOperationException($"Received json text for event named \"{name}\" is null");
                                    }

                                    action?.Invoke(text);
                                }
                                catch (Exception exception)
                                {
                                    OnExceptionOccurred(exception);
                                }
                            }
                        }

                        if (JsonDeserializeActions.TryGetValue(key, out var jsonDeserializeActions))
                        {
                            foreach (var (action, type) in jsonDeserializeActions)
                            {
                                isHandled = true;

                                try
                                {
                                    if (text == null)
                                    {
                                        throw new InvalidOperationException($"Received json text for event named \"{name}\" is null");
                                    }

                                    var obj = JsonConvert.DeserializeObject(text, type);
                                    if (obj == null)
                                    {
                                        throw new InvalidOperationException($"Deserialized object for json text(\"{text}\") and for event named \"{name}\" is null");
                                    }

                                    action(obj, text);
                                }
                                catch (Exception exception)
                                {
                                    OnExceptionOccurred(exception);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        OnExceptionOccurred(exception);
                    }
                    finally
                    {
                        OnEventReceived(value: packet.Value, @namespace: packet.Namespace, isHandled: isHandled);
                        if (isHandled)
                        {
                            OnHandledEventReceived(value: packet.Value, @namespace: packet.Namespace, isHandled: isHandled);
                        }
                        else
                        {
                            OnUnhandledEventReceived(value: packet.Value, @namespace: packet.Namespace, isHandled: isHandled);
                        }
                    }
                    break;

                case SocketIoPacket.ErrorPrefix:
                    OnErrorReceived(
                        value: packet.Value.Trim('\"'),
                        @namespace: packet.Namespace);
                    break;
            }
        }
        catch (Exception exception)
        {
            OnExceptionOccurred(exception);
        }
    }

    #endregion

    #region Private methods

    private static string GetOnKey(string name, string? customNamespace = null)
    {
        return $"{name}{customNamespace ?? "/"}";
    }

    #endregion

    #region Public methods

    /// <summary>
    /// It connects to the server and asynchronously waits for a connection message.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="namespaces"></param>
    /// <exception cref="InvalidOperationException">if AfterError event occurs while wait connect message</exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public async Task<bool> ConnectAsync(Uri uri, CancellationToken cancellationToken = default, params string[] namespaces)
    {
        uri = uri ?? throw new ArgumentNullException(nameof(uri));
        EngineIoClient = EngineIoClient ?? throw new ObjectDisposedException(nameof(EngineIoClient));

        if (!EngineIoClient.IsOpened)
        {
            var results = await this.WaitAnyEventAsync<EventArgs>(async () =>
            {
                await EngineIoClient.OpenAsync(uri, cancellationToken).ConfigureAwait(false);
            }, cancellationToken, nameof(Connected), nameof(ErrorReceived)).ConfigureAwait(false);

            if (results[nameof(ErrorReceived)] is ErrorReceivedEventArgs errorArgs)
            {
                throw new InvalidOperationException($"Socket.IO returns error: {errorArgs.Value}");
            }
            if (results[nameof(Connected)] == null)
            {
                return false;
            }
        }

        return await ConnectToNamespacesAsync(cancellationToken, DefaultNamespace != null
            ? namespaces.Concat(new[] { DefaultNamespace }).Distinct().ToArray()
            : namespaces).ConfigureAwait(false);
    }

    /// <summary>
    /// It connects to selected namespaces and asynchronously waits for a connection message.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="namespaces"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <returns></returns>
    public async Task<bool> ConnectToNamespacesAsync(CancellationToken cancellationToken = default, params string[] namespaces)
    {
        EngineIoClient = EngineIoClient ?? throw new ObjectDisposedException(nameof(EngineIoClient));

        if (!EngineIoClient.IsOpened)
        {
            return false;
        }

        if (!namespaces.Any())
        {
            return true;
        }

        return await this.WaitEventAsync<ConnectedEventArgs>(async () =>
        {
            foreach (var @namespace in namespaces)
            {
                var packet = new SocketIoPacket(SocketIoPacket.ConnectPrefix, @namespace: @namespace);

                await EngineIoClient.SendMessageAsync(packet.Encode(), cancellationToken).ConfigureAwait(false);
            }
        }, nameof(Connected), cancellationToken).ConfigureAwait(false) != null;
    }

    /// <summary>
    /// It connects to selected namespaces and asynchronously waits for a connection message.
    /// </summary>
    /// <param name="customNamespace"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <returns></returns>
    public async Task<bool> ConnectToNamespaceAsync(string customNamespace, CancellationToken cancellationToken = default)
    {
        customNamespace = customNamespace ?? throw new ArgumentNullException(nameof(customNamespace));
        EngineIoClient = EngineIoClient ?? throw new ObjectDisposedException(nameof(EngineIoClient));

        return await ConnectToNamespacesAsync(cancellationToken, customNamespace).ConfigureAwait(false);
    }

    /// <summary>
    /// It connects to the server and asynchronously waits for a connection message with the selected timeout. <br/>
    /// Throws <see cref="OperationCanceledException"/> if the waiting time exceeded
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="namespaces"></param>
    /// <exception cref="InvalidOperationException">if AfterError event occurs while wait connect message</exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public async Task<bool> ConnectAsync(
        Uri uri,
        TimeSpan timeout,
        CancellationToken cancellationToken = default,
        params string[] namespaces)
    {
        uri = uri ?? throw new ArgumentNullException(nameof(uri));

        using var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationSource.CancelAfter(timeout);

        return await ConnectAsync(uri, cancellationSource.Token, namespaces).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a disconnect message and closes the connection.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <returns></returns>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        EngineIoClient = EngineIoClient ?? throw new ObjectDisposedException(nameof(EngineIoClient));

        if (DefaultNamespace != null)
        {
            var packet = new SocketIoPacket(SocketIoPacket.DisconnectPrefix, @namespace: DefaultNamespace);

            await EngineIoClient.SendMessageAsync(packet.Encode(), cancellationToken).ConfigureAwait(false);
        }

        {
            var packet = new SocketIoPacket(SocketIoPacket.DisconnectPrefix);

            await EngineIoClient.SendMessageAsync(packet.Encode(), cancellationToken).ConfigureAwait(false);
        }

        await EngineIoClient.CloseAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a new raw message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="customNamespace"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <returns></returns>
    public async Task SendEventAsync(string message, string? customNamespace = null, CancellationToken cancellationToken = default)
    {
        EngineIoClient = EngineIoClient ?? throw new ObjectDisposedException(nameof(EngineIoClient));

        var packet = new SocketIoPacket(SocketIoPacket.EventPrefix, message, customNamespace ?? DefaultNamespace);

        await EngineIoClient.SendMessageAsync(packet.Encode(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for the next event or error asynchronously <br/>
    /// Returns <see cref="EventReceivedEventArgs"/> if event was received <br/>
    /// Returns <see cref="ErrorReceivedEventArgs"/> if error was received <br/>
    /// Returns null if no event was received and the method was canceled <br/>
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<EventArgs?> WaitEventOrErrorAsync(Func<Task>? func = null, CancellationToken cancellationToken = default)
    {
        var dictionary = await this.WaitAnyEventAsync<EventArgs?>(
            func ?? (() => Task.FromResult(false)),
            cancellationToken,
            nameof(EventReceived), nameof(ErrorReceived))
            .ConfigureAwait(false);

        return dictionary[nameof(EventReceived)] ??
               dictionary[nameof(ErrorReceived)];
    }

    /// <summary>
    /// Waits for the next event or error asynchronously with specified timeout <br/>
    /// Returns <see cref="EventReceivedEventArgs"/> if event was received <br/>
    /// Returns <see cref="ErrorReceivedEventArgs"/> if error was received <br/>
    /// Returns null if no event was received and the method was canceled <br/>
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<EventArgs?> WaitEventOrErrorAsync(TimeSpan timeout, Func<Task>? func = null)
    {
        using var tokenSource = new CancellationTokenSource(timeout);

        return await WaitEventOrErrorAsync(func, tokenSource.Token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a new event where name is the name of the event <br/>
    /// the object can be <see langword="string"/> - so it will be send as simple message <br/>
    /// any other will be serialized to json.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="customNamespace"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Emit(string name, object? value = null, string? customNamespace = null, CancellationToken cancellationToken = default)
    {
        var messages = value switch
        {
            null => new[] { $"\"{name}\"" },
            string message => new[] { $"\"{name}\"", $"\"{message}\"" },
            _ => new[] { $"\"{name}\"", JsonConvert.SerializeObject(value) },
        };

        await SendEventAsync($"[{string.Join(",", messages)}]", customNamespace, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes On handle.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="customNamespace"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Off(string name, string? customNamespace = null)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));

        var key = GetOnKey(name, customNamespace);
        if (JsonDeserializeActions.ContainsKey(key))
        {
            JsonDeserializeActions.Remove(key);
        }
        if (TextActions.ContainsKey(key))
        {
            TextActions.Remove(key);
        }
        if (Actions.ContainsKey(key))
        {
            Actions.Remove(key);
        }
    }

    /// <summary>
    /// Performs an action after receiving a specific event. <br/>
    /// <paramref name="action"/>.<typeparamref name="T"/> is a json deserialized object, <br/>
    /// <paramref name="action"/>.<see langword="string"/> is raw text.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <param name="customNamespace"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void On<T>(string name, Action<T, string> action, string? customNamespace = null) where T : class
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        action = action ?? throw new ArgumentNullException(nameof(action));

        var key = GetOnKey(name, customNamespace);
        if (!JsonDeserializeActions.ContainsKey(key))
        {
            JsonDeserializeActions[key] = new List<(Action<object, string> Action, Type Type)>();
        }

        JsonDeserializeActions[key].Add(((obj, text) => action((T)obj, text), typeof(T)));
    }

    /// <summary>
    /// Performs an action after receiving a specific event. <br/>
    /// <paramref name="action"/>.<typeparamref name="T"/> is a json deserialized object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <param name="customNamespace"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void On<T>(string name, Action<T> action, string? customNamespace = null) where T : class
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        action = action ?? throw new ArgumentNullException(nameof(action));

        On<T>(name, (obj, _) => action(obj), customNamespace);
    }

    /// <summary>
    /// Performs an action after receiving a specific event. <br/>
    /// <paramref name="action"/>.<see langword="string"/> is a raw text.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <param name="customNamespace"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void On(string name, Action<string> action, string? customNamespace = null)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        action = action ?? throw new ArgumentNullException(nameof(action));

        var key = GetOnKey(name, customNamespace);
        if (!TextActions.ContainsKey(key))
        {
            TextActions[key] = new List<Action<string>>();
        }

        TextActions[key].Add(action);
    }

    /// <summary>
    /// Performs an action after receiving a specific event.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <param name="customNamespace"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void On(string name, Action action, string? customNamespace = null)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        action = action ?? throw new ArgumentNullException(nameof(action));

        var key = GetOnKey(name, customNamespace);
        if (!Actions.ContainsKey(key))
        {
            Actions[key] = new List<Action>();
        }

        Actions[key].Add(action);
    }

    /// <summary>
    /// Disposes an object. <br/>
    /// Prefer DisposeAsync if possible
    /// </summary>
    /// <returns></returns>
    public void Dispose()
    {
        EngineIoClient.Dispose();
    }

#if NETSTANDARD2_1
    /// <summary>
    /// Asynchronously disposes an object.
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        await EngineIoClient.DisposeAsync().ConfigureAwait(false);
    }
#endif

    #endregion
}