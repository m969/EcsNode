# Project Overview

这个工程包含EcsNode框架核心实现（基于ECS 实体-组件-系统 编码范式），以及EcsNode框架的使用示例帧同步demo

## Folder Structure

- `/UnityApp/Assets/App.Model`: 存放实体（EcsEntity）和组件（EcsComponent）脚本
- `/UnityApp/Assets/App.System`: 存放系统（EcsSystem）脚本
- `/UnityApp/Assets/Game.ThirdParty/EcsNode`: EcsNode框架核心实现

---
applyTo: "UnityApp/Assets/App.Model/**/*.cs"
---

App.Model存放实体（EcsEntity）和组件（EcsComponent）

实体（EcsEntity）和组件（EcsComponent）只实现属性数据，不实现方法逻辑

---
applyTo: "UnityApp/Assets/App.System/**/*.cs"
---

App.System存放系统（EcsSystem），与App.Model下的实体（EcsEntity）和组件（EcsComponent）一一对应

系统（EcsSystem）只实现方法逻辑，不实现属性数据