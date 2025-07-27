# Project Overview

这个工程包是EcsNode的模块工程，用于实现可插入EcsNode框架的模块代码

## Folder Structure

- `/com.model.**`: 存放模块的实体（EcsEntity）和组件（EcsComponent）脚本
- `/com.system.**`: 存放模块的系统（EcsSystem）脚本
- `/com.view-model.**`: 存放模块的视图实体（EcsEntity）和视图组件（EcsComponent）脚本
- `/com.view-system.**`: 存放模块的视图系统（EcsSystem）脚本

---
applyTo: "com.model.**/**/*.cs"
---

com.model.** 目录存放实体（EcsEntity）和组件（EcsComponent）

实体（EcsEntity）和组件（EcsComponent）只实现属性数据，不实现方法逻辑

---
applyTo: "com.system.**/**/*.cs"
---

com.system.** 目录存放系统（EcsSystem），与com.model.** 目录下的实体（EcsEntity）和组件（EcsComponent）一一对应

系统（EcsSystem）只实现方法逻辑，不实现属性数据