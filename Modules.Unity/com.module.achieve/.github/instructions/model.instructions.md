---
applyTo: "com.model.**/**/*.cs"
---

## 实体和组件：
- 只实现属性数据，不实现方法逻辑
- 实体继承自EcsEntity，组件继承自EcsComponent
- 一个类一个文件
- 不实现派发接口的定义，由system系统层实现