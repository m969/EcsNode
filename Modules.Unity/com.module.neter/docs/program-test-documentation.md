# （Neter 网络节点通信）模块程序单元测试设计文档

> 目标：以最小复杂度覆盖核心行为，优先验证数据结构与状态转换，不写“聪明”的测试，写稳定可重复的测试。

## 范围与被测对象

- 模型（com.model.neter）
	- `Neter`、`NeterSession`
	- 消息与序列化：`NetworkMessage`、`MessageSerializer`、`UintMessageSerializer`、`MessageTypeIds`
	- 基础消息：`NodeInfoMessage`、`HeartbeatMessage`、`DiscoveryMessage`、`NodeOfflineMessage`、`CustomDataMessage`、`RpcCallMessage`、`RpcResponseMessage`
	- 处理器：`NetworkMessageHandler`（按类型/接口动态分发）
- 系统（com.system.neter）
	- `NeterSystem`：启动/停止、连接、事件轮询、消息分发、响应路由、发现广播、负载估算、参数配置
	- `NeterSessionSystem`：会话创建、连接、发送、Ask/Response、关闭、回调
	- `NeterNetEventListener`：LiteNetLib 事件到模型/系统的桥接

不在本轮覆盖：与外部框架强耦合或未实现的应用层逻辑（例如真实业务 `NeterApp`），仅以最小桩实现验证接口契约。

## 测试策略

- 单元测试（纯逻辑、无网络）：
	- `NetworkMessage` 的 Set/Get、TypeId 映射、Headers 行为
	- `MessageSerializer`/`UintMessageSerializer` 的序列化/反序列化、类型注册冲突检测、TypeId 探测
	- `MessageTypeIds` 与映射表的一致性
- 组件/集成测试（本机回环网络）：
	- 在两个端口上启动两个 `Neter` 实例，使用 LiteNetLib 回环通信
	- 验证连接建立、自动会话创建、消息往返、Ask/Response、广播发现发起连接
	- 使用 `Neter.Update()` 与 `NetManager.PollEvents()` 稳定推进状态
- 异常与边界：
	- 未注册类型、空内容、重复注册、未连接发送、超时响应、超过最大连接数
- 非功能：
	- 基础吞吐冒烟（几十～几百条消息）与无崩溃保证；不追求性能基准，只做健康检查。

## 测试夹具与约定

- 端口规划：使用动态端口或固定对（如 9000/9001），避免与本机占用冲突
- 超时控制：连接与等待响应设定上限（建议 3–5s），避免 CI 卡死
- App/处理器桩：
	- 提供一个实现 `IHandleNetworkMessage` 的桩，回显或填充响应
	- 如需类型化处理，构造简单的消息类型（可复用 `CustomDataMessage`）
- 隔离性：每个测试独立启动/停止 `Neter`，清理连接，避免跨用例状态污染

## 用例矩阵（精选）

### 消息与序列化（NM-xxx）

- NM-001 NetworkMessage-SetGetContent
	- 前置：注册内置类型已就绪（默认）
	- 步骤：`SetContent(new HeartbeatMessage{...})` -> `GetContent<HeartbeatMessage>()`
	- 断言：`MessageTypeId` 匹配、`RawContent` 非空、取回对象等值

- NM-002 NetworkMessage-TypeIdRoundtrip
	- 步骤：`NetworkMessage.Create(new DiscoveryMessage{...})` -> `MessageSerializer.Serialize` -> `Deserialize`
	- 断言：`MessageTypeId`、`GetContent<DiscoveryMessage>()` 成功

- NM-003 UintMessageSerializer-GetMessageTypeId
	- 步骤：序列化 `NetworkMessage(RpcCallMessage)` 的字节，调用 `GetMessageTypeId`
	- 断言：返回内置 `RpcCall` 的 ID

- NM-004 UintMessageSerializer-Register-DuplicateGuard
	- 步骤：对已存在类型/ID再次注册
	- 断言：抛出 `ArgumentException`

- NM-005 NetworkMessage-Headers
	- 步骤：SetHeader/GetHeader（含默认值）
	- 断言：值与类型匹配

### NeterSystem（NS-xxx）

- NS-101 StartAsync-OnlineState
	- 步骤：`StartAsync(neter, port)`
	- 断言：返回 true，`Status==Online`，`LocalPort>0`，`_localNodeInfo` 填充

- NS-102 ConnectToNodeAsync-Loopback
	- 前置：节点 A/B 启动（不同端口）
	- 步骤：A 连接到 B 的端点
	- 断言：返回 `NodeInfo` 非空，B 的 `ConnectedNodes` 含 A 的临时节点

- NS-103 Update-ConnectingToConnected
	- 前置：创建 `NeterSession`，调用 `NeterSessionSystem.ConnectAsync`
	- 步骤：周期性调用 A/B 各自的 `NeterSystem.Update`
	- 断言：会话从 `ConnectingSessions` 迁移至 `ConnectedSessions`，`PendingConnectTask` 完成

- NS-104 HandleResponseMessage-RouteToPending
	- 前置：`neter._pendingRequestSessions` 存入某 MessageId -> Session 映射
	- 步骤：调用 `HandleResponseMessage`
	- 断言：对应 `NeterSessionSystem.HandleResponseMessage` 被触发，返回 true

- NS-105 StartServiceDiscoveryAsync-Broadcast
	- 步骤：节点 A/B 启动，调用 `StartServiceDiscoveryAsync(A)`
	- 断言：B 的 `OnNetworkReceiveUnconnected` 触发连接尝试，最终 A/B 互联（允许一定时间轮询）

- NS-106 ShouldAcceptConnection-Capacity
	- 前置：`MaxConnections=1` 且已有一连接
	- 步骤：新的连接请求
	- 断言：返回 false（或 `OnConnectionRequest` 中被 Reject）

### NeterSessionSystem（NSS-xxx）

- NSS-201 CreateSession-Basic
	- 断言：`Id` 非零，`LocalNode/RemoteNode` 正确绑定

- NSS-202 ConnectAsync-AssignPeerAndTask
	- 步骤：对远端端点调用 `ConnectAsync`
	- 断言：`RemoteNode.Peer` 赋值，`PendingConnectTask` 存在，`ConnectingSessions` 含该会话

- NSS-203 Send-RequiresConnected
	- 步骤：在未连接时调用 `Send`
	- 断言：抛出异常消息“会话未连接”

- NSS-204 AskResponse-Roundtrip
	- 前置：对端桩处理器收到请求即回包（复用 `IHandleNetworkMessage`）
	- 步骤：`Ask<TReq,TResp>`
	- 断言：收到期望的响应对象，`_pendingRequestSessions`/`PendingRequests` 条目被清理

- NSS-205 Close-DisconnectAndCallback
	- 步骤：设置 `OnDisconnected`，调用 `Close`
	- 断言：对端收到断开/本端回调触发

### NeterNetEventListener（NEL-xxx）

- NEL-301 OnPeerConnected-NodeInfoHandshake
	- 断言：`ConnectedNodes` 增加临时节点，发送 `NodeInfoMessage` 到对端

- NEL-302 OnNetworkReceive-AutoSessionCreate
	- 前置：从对端收到带 `SessionId` 的消息且本端无该会话
	- 断言：自动创建 `NeterSession` 加入 `ConnectedSessions`

- NEL-303 OnNetworkReceive-ResponseShortCircuit
	- 前置：`_pendingRequestSessions` 包含 `MessageId`
	- 断言：直接路由至 `NeterSessionSystem.HandleResponseMessage`，不进入业务分发

- NEL-304 OnNetworkReceiveUnconnected-DiscoveryConnect
	- 步骤：接收 `DiscoveryMessage` 广播
	- 断言：发起 `NeterSystem.ConnectToNodeAsync`（可借助探针或最终连通验证）

## 关键断言与边界

- 未注册类型：`NetworkMessage.Create<T>` 时 `MessageTypeId` 可为 0，反序列化 `GetContent()` 应返回 null 并记录 Warning，不崩溃
- 重复注册：`UintMessageSerializer.RegisterMessageType` 抛出异常
- 超时：`Ask` 需具备可配置的超时（当前实现未包含超时取消，建议后续用例标记为“待增强”）
- 会话状态：`Send/Ask` 在未连接时抛出异常；`HandleResponseMessage` 对未知 `MessageId` 返回 false
- 资源释放：`StopAsync/Dispose/Close` 不应抛出异常

## 验收清单（执行即通过）

- 启停：两个 `Neter` 能稳定启动/停止，无未捕获异常
- 连接：`ConnectToNodeAsync` 在本机回环成功连通
- 轮询：`Update` 能将会话从连接中推进到已连接
- 消息：能完成一次请求-响应往返，并正确清理挂起映射
- 广播：`StartServiceDiscoveryAsync` 能促成另一端发起连接（在允许时窗内）
- 边界：重复注册抛异常；未连接发送抛异常；超过最大连接数被拒绝

## 实施备注

- 测试工程建议：新增 `system.module-test.neter`（xUnit/NUnit 任一），遵循仓库测试规范
- 运行提示：网络用例需适当 `PollEvents/Update`（100ms 粒度）推进状态；必要时使用重试/等待上限
- CI 稳定性：避免硬编码本地网段（代码中含 `192.168.1.x` 广播），在 CI 上可跳过广播用例或改为本地回环定向测试

## 后续增强（非阻断）

- `Ask` 增加超时取消与错误传播
- 统计信息完善：记录每次 `Send/Receive` 的速率用于更可测的吞吐断言
- `HandleNetworkMessageSystem` 的应用层契约样例与更多类型化处理验证

—— 保持测试小而准，先让系统跑起来，再逐步加压。
