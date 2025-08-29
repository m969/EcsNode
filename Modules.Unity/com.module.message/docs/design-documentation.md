# （消息）模块功能设计文档

# 组件
- NeterMessageComponent:Neter消息组件，存储消息Id和消息类型的映射

# 组件系统
- NeterMessageSystem:Neter消息系统
    - GetMessageType，获取消息Id对应的消息类型
    - GetMessageTypeId，获取消息类型对应的消息Id