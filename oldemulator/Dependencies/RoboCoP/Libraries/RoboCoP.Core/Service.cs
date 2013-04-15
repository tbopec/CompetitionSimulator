using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoboCoP.Helpers;
using RoboCoP.Implementation;
using RoboCoP.Internal;
using RoboCoP.Messages;
using RoboCoP.Protocols;

namespace RoboCoP
{
    /// <summary>
    /// Class that represents a RoboCoP server and provides all interfaces for communications via RoboCoP.
    /// </summary>
    public class Service
    {



        /// <summary>
        /// Create a service using it's <see cref="IServiceSettings"/>. Usually this settings are obtained from config file.
        /// </summary>
        /// <example>
        /// Quickstart example:
        /// <code>
        /// var settings = new ServiceSettings();
        /// settings.Name = "MyServiceName";
        /// ServiceSettingsIO.Ini.Parse(settings, "MyConfig.ini", settings.Name);
        /// var service = new Service(settings);
        /// </code>
        /// </example>
        public Service(IServiceSettings settings)
            : this(settings, delegate { }) {}

        /// <summary>
        /// Create a service using it's <see cref="IServiceSettings"/>.
        /// Use <see cref="callback"/> for informing about <see cref="ServiceStates"/> changes.
        /// </summary>
        public Service(IServiceSettings settings, Action<ServiceStates> callback)
        {
            if(settings == null)
                throw new ArgumentNullException("settings");
            if(callback == null)
                throw new ArgumentNullException("callback");
            callback(ServiceStates.ServiceInitStart);

            ServiceName = settings.Name;

            callback(ServiceStates.OutListInit);

            var sList = new List<IOut>();
            var datamsgBuilder = new DataMessageFactory();
            if(settings.Out != null)
                for(int index = 0; index < settings.Out.Length; index++) {
                    INetworkAddress ep = settings.Out[index];
                    if(ep == null)
                        continue;
                    var sen = new DataOut(index,
                        new MultiSender<DataMessage>(ProtocolProviderFactory.GetConnectionManager(ep)),
                        datamsgBuilder);
                    sList.AddAt(index, sen);
                }
            Out = sList.AsReadOnly();

            callback(ServiceStates.InListInit);

            var rList = new List<IIn>();
            if(settings.In != null)
                for(int index = 0; index < settings.In.Length; index++) {
                    INetworkAddress ep = settings.In[index];
                    if(ep == null)
                        continue;
                    var rec = new DataIn(index,
                        new Receiver<DataMessage>(new StableConnection(ProtocolProviderFactory.GetConnectionFunc(ep))));
                    rList.AddAt(index, rec);
                }
            In = rList.AsReadOnly();

            callback(ServiceStates.CommanderInit);

            Com = new CommanderMock(settings.Name);
            if(!ReferenceEquals(settings.Switch, null)) {
                var connectFunc = ProtocolProviderFactory.GetConnectionFunc(settings.Switch);
                var connection = new StableConnection(connectFunc);
                var receiver = new Receiver<AddressableMessage>(connection);
                var sender = new SingleSender<AddressableMessage>(connection);
                var negotiator = new SwitchNegotiator(sender, new MessageFactory(ServiceName));
                Com = new Commander(settings.Name, sender, receiver, negotiator);
            }

            callback(ServiceStates.ServiceReady);
        }

        /// <summary>
        /// Command interface of the service.
        /// </summary>
        public ICommander Com { get; private set; }

        /// <summary>
        /// <see cref="IList{T}"/> of <see cref="IReceiver{TMessage}"/> interfaces for reading <see cref="DataMessage"/>s
        /// from RoboCoP data receive interfaces.
        /// </summary>
        /// <example>
        /// Use service.In[0].<see cref="ReceiverExtension.ReceiveSync{TMessage}"/> for synchronously receiving package of data from first interface.
        /// Using of asynchronous methods: service.In[0].<see cref="ReceiverExtension.ReceiveTextAsync{TMessage}(RoboCoP.IReceiver{TMessage},System.Action{TMessage})"/>
        /// and service.In[0].<see cref="ReceiverExtension.ReceiveTextAsync{TMessage}(RoboCoP.IReceiver{TMessage})"/> is preferable.
        /// 
        /// You can use them to Receives one <see cref="Message"/> in separate thread asynchronously:
        /// <code>
        /// var task = service.In[0].ReceiveTextAsync();
        /// ... do smth else ...
        /// var msg = task.Result; // here we'll wait until <see cref="Task{TResult}.Result"/> is ready.
        /// </code>
        /// In such scenario it can be useful to check if the async operation is complete or not. You can do in two ways:
        /// 1) Using <see cref="Task{TResult}.IsCompleted"/> flag:
        /// <code>
        /// var task = service.In[0].ReceiveTextAsync();
        /// while(!task.IsComplete) {
        ///     ... do smth else ...
        /// }
        /// var msg = task.Result;
        /// </code>
        /// 2) Or using <see cref="Task.Wait()"/>:
        /// <code>
        /// var task = service.In[0].ReceiveTextAsync();
        /// while(!result.Wait(100)) {
        ///     ... do smth else ...
        /// }
        /// var msg = task.Result;
        /// </code>
        /// 
        /// You can read about other features of the <see cref="Task{TResult}"/> 
        /// (such as <see cref="Task.Wait(System.Threading.CancellationToken)"/> or <see cref="Task.ContinueWith(System.Action{System.Threading.Tasks.Task})"/>) here:
        ///  - in msdn: <seealso cref="Task{TResult}"/> and <see cref="Task"/>.
        ///  - or in the Joseph Albahari's book: <seealso cref="http://www.albahari.com/threading/part5.aspx#_Task_Parallelism"/>
        /// 
        /// Also you can receive one <see cref="Message"/> asynchronously using callback:
        /// <code>
        /// service.In[0].ReceiveTextAsync(callback);
        /// 
        /// protected void callback(DataMessage result) {
        ///     ...
        /// }
        /// </code>
        /// 
        /// Also you can start receiving of all <see cref="DataMessage"/> using <see cref="ReceiverExtension.ReceiveTextAll{TMessage}"/>:
        /// <code>
        /// var unsubscribe = service.In[0].ReceiveTextAll(onReceivedCallback, onErrorCallback);
        /// </code>
        /// where unsubscribe is a <see cref="IDisposable"/> which can be used for cancelling of receiving - just call <see cref="IDisposable.Dispose"/> on it.
        /// You should be accurate with <see cref="ReceiverExtension.ReceiveTextAll{TMessage}"/> method, because after you called it _all_ messages directed
        /// to specified <see cref="In"/> interface will be caught by specified delegates and by no one else.
        /// At least until you'll cancel receiving via <see cref="IDisposable.Dispose"/>.
        /// Be accurate - never receive any message from <see cref="IReceiver{TMessage}"/> which's messages are caught by non-cancelled <see cref="ReceiverExtension.ReceiveTextAll{TMessage}"/>.
        /// 
        /// You also can use <see cref="IReceiver{TMessage}.Receive"/> directly if you want to obtain some <see cref="IObservable{T}"/> functionality.
        /// But in this case you should know what you are doing.
        /// For some faq about <see cref="IObservable{T}"/> and Rx see:
        /// - <seealso cref="Observable"/>, <seealso cref="System.ObservableExtensions"/>
        /// - creator's site <seealso cref="http://msdn.microsoft.com/en-us/devlabs/ee794896.aspx"/> and blog <seealso cref="http://blogs.msdn.com/b/rxteam/"/>
        /// </example>
        public ReadOnlyCollection<IIn> In { get; private set; }

        /// <summary>
        /// <see cref="IList{T}"/> of <see cref="ISender{TMessage}"/> interfaces for writing <see cref="Message"/>s
        /// to RoboCoP data sending interfaces.
        /// </summary>
        /// <example>
        /// Use service.Out[0].SendSync(message) for synchronously sending package of data.
        /// Or service.Out[0].SendAsync(message) or service.Out[0].SendAsync(message, callback) for async (which is preferable).
        /// </example>
        /// <seealso cref="In"/>
        public ReadOnlyCollection<IOut> Out { get; private set; }

        /// <summary>
        /// Gets the service full name. It should be unique in the whole network.
        /// </summary>
        public string ServiceName { get; private set; }
    }
}