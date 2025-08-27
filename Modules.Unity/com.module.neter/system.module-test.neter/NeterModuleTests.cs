using System;
using System.Net;
using System.Threading.Tasks;
using ECS;
using NUnit.Framework;
using ET;
using System.Linq;

namespace ECSGame.Module.Neter.Tests
{
    [TestFixture]
    public class NeterModuleTests
    {
        private EcsNode ecsNode;

        public class TestEcsNode : EcsNode
        {
            public TestEcsNode(ushort id) : base(id) { }
        }

        private class TestEchoHandler : IHandleNetworkMessage
        {
            // 简化系统下不再处理消息内容，直接按MessageId回发以完成请求-响应
            public ETTask<bool> HandleNetworkMessage(object neterApp, NeterSession session, NetworkMessage message)
            {
                var reply = new NetworkMessage
                {
                    SessionId = message.SessionId,
                    MessageId = message.MessageId
                };
                NeterSessionSystem.Send(session, reply);

                var t = ETTask<bool>.Create();
                t.SetResult(true);
                return t;
            }
        }

        [SetUp]
        public void SetUp()
        {
            ecsNode = new TestEcsNode(1);
        }

        // 已移除基于消息内容与类型映射的测试，因系统已简化为仅传输 NetworkMessage，不再提供内容打包/解包与类型注册API

        [Test]
        public async Task NS_101_103_StartAndConnect_Update_Progresses_Session()
        {
            var driveTypes = typeof(IAwake).Assembly.GetTypes();
            var allTypes = typeof(NeterSystem).Assembly.GetTypes().ToList();
            allTypes.AddRange(driveTypes);
            // create two nodes with different ports
            var a = NeterSystem.Create(1, "A", NodeType.Gateway, allTypes.ToArray(), new NodeConfig());
            var b = NeterSystem.Create(1, "B", NodeType.Game, allTypes.ToArray(), new NodeConfig());

            var portA = 9000;
            var portB = 9001;

            Assert.That(await NeterSystem.StartAsync(a, portA), Is.True);
            Assert.That(await NeterSystem.StartAsync(b, portB), Is.True);

            // prepare session A->B
            var remoteNode = NodeInfo.Create("B", NodeType.Game, new IPEndPoint(IPAddress.Loopback, b.LocalPort));
            var session = NeterSessionSystem.CreateSession(a, remoteNode.EndPoint);
            _ = NeterSessionSystem.ConnectAsync(session);

            // pump events for both sides up to 5s
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(5))
            {
                a.DriveEntityUpdate();
                b.DriveEntityUpdate();
                await Task.Delay(50);
            }

            // 验证连接已建立（会话连通且双方记录到 ConnectedNodes）
            Assert.That(session.IsConnected, Is.True);
            Assert.That(a.ConnectedNodes.Count, Is.GreaterThan(0));
            Assert.That(b.ConnectedNodes.Count, Is.GreaterThan(0));

            await NeterSystem.StopAsync(a);
            await NeterSystem.StopAsync(b);
        }

        [Test]
        public void NSS_203_Send_RequiresConnected()
        {
            var driveTypes = typeof(IAwake).Assembly.GetTypes();
            var allTypes = typeof(NeterSystem).Assembly.GetTypes().ToList();
            allTypes.AddRange(driveTypes);
            var a = NeterSystem.Create(1, "A", NodeType.Gateway, allTypes.ToArray(), new NodeConfig());
            var b = NodeInfo.Create("B", NodeType.Game, new IPEndPoint(IPAddress.Loopback, 9001));
            var session = NeterSessionSystem.CreateSession(a, b.EndPoint);

            Assert.Throws<Exception>(() => NeterSessionSystem.Send(session, new NetworkMessage()));
        }

        [Test]
        public async Task NSS_204_Ask_Response_EndToEnd()
        {
            var driveTypes = typeof(IAwake).Assembly.GetTypes();
            var allTypes = typeof(NeterSystem).Assembly.GetTypes().ToList();
            allTypes.AddRange(driveTypes);
            var a = NeterSystem.Create(1, "A", NodeType.Gateway, allTypes.ToArray(), new NodeConfig());
            var b = NeterSystem.Create(1, "B", NodeType.Game, allTypes.ToArray(), new NodeConfig());

            var portA = 9002;
            var portB = 9003;

            Assert.That(await NeterSystem.StartAsync(a, portA), Is.True);
            Assert.That(await NeterSystem.StartAsync(b, portB), Is.True);

            // 简化系统下不依赖处理器，改为B侧基于连接创建会话并手动回发响应

            // 建立 A->B 会话
            var remoteNode = NodeInfo.Create("B", NodeType.Game, new IPEndPoint(IPAddress.Loopback, b.LocalPort));
            var session = NeterSessionSystem.CreateSession(a, remoteNode.EndPoint);
            _ = NeterSessionSystem.ConnectAsync(session);

            // 等待连接就绪
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(5) && !session.IsConnected)
            {
                a.DriveEntityUpdate();
                b.DriveEntityUpdate();
                await Task.Delay(50);
            }
            Assert.That(session.IsConnected, Is.True, "session should be connected before Ask");

            // 发送请求并等待响应
            // 发送请求（简化版本仅基于MessageId，不携带内容）并等待响应
            var askTask = NeterSessionSystem.Ask(session, new NetworkMessage());

            // 等待B端记录连接节点
            sw.Restart();
            while (sw.Elapsed < TimeSpan.FromSeconds(2) && b.ConnectedNodes.Count == 0)
            {
                a.DriveEntityUpdate();
                b.DriveEntityUpdate();
                await Task.Delay(20);
            }

            // 在B端基于Peer创建会话，并回发匹配的响应（首个请求ID为1）
            var remoteFromA = b.ConnectedNodes.Values.FirstOrDefault();
            Assert.That(remoteFromA, Is.Not.Null, "B should see A as a connected node");
            var bSessionToA = NeterSessionSystem.CreateSession(b, remoteFromA!.Peer);
            var reply = new NetworkMessage { SessionId = session.Id, MessageId = 1 };
            NeterSessionSystem.Send(bSessionToA, reply);

            // 泵事件直到应答返回或超时
            sw.Restart();
            NetworkMessage? resp = null;
            while (sw.Elapsed < TimeSpan.FromSeconds(5))
            {
                a.DriveEntityUpdate();
                b.DriveEntityUpdate();
                if (askTask.IsCompleted)
                {
                    resp = await askTask;
                    break;
                }
                await Task.Delay(20);
            }

            Assert.That(resp, Is.Not.Null, "Ask should receive response");
            Assert.That(resp!.MessageId, Is.EqualTo(1u));

            await NeterSystem.StopAsync(a);
            await NeterSystem.StopAsync(b);
        }
    }
}
