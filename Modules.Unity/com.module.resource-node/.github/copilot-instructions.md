# Project Overview

这个工程包是EcsNode的模块工程，用于实现可插入EcsNode框架的模块代码

## Folder Structure

- `/com.model.**`: 存放模块的实体（EcsEntity）和组件（EcsComponent）脚本
- `/com.system.**`: 存放模块的系统（EcsSystem）脚本
- `/com.view-model.**`: 存放模块的视图实体（EcsEntity）和视图组件（EcsComponent）脚本
- `/com.view-system.**`: 存放模块的视图系统（EcsSystem）脚本
- `/**-design-documentation.md`: 模块功能策划文档
- `/**-program-documentation.md`: 模块程序设计文档

---
applyTo: "**-design-documentation.md"
---

模块功能策划文档，应以以下内容为基础：
1. 模块功能概述
2. 模块实体设计
3. 模块组件设计
4. 模块系统设计

---
applyTo: "**-program-documentation.md"
---

模块程序设计文档，应以以下内容为基础：

1. 模块功能概述
2. 模块接口设计
3. 模块数据结构
4. 模块逻辑流程
5. 模块测试方案

基于实体（EcsEntity）、组件（EcsComponent）、系统（EcsSystem）设计，应包含以下内容：
- 实体（Entities）设计
- 组件（Components）设计
- 系统（Systems）设计

---
applyTo: "com.model.**/**/*.cs"
---

com.model.** 目录存放实体（EcsEntity）和组件（EcsComponent）

实体（EcsEntity）和组件（EcsComponent）只实现属性数据，不实现方法逻辑

实体继承自EcsEntity

组件继承自EcsComponent

命名空间为ECSGame.Module.**，其中**为模块名称

---
applyTo: "com.system.**/**/*.cs"
---

com.system.** 目录存放系统（EcsSystem），与com.model.** 目录下的实体（EcsEntity）和组件（EcsComponent）一一对应

系统（EcsSystem）只实现方法逻辑，不实现属性数据

实体系统继承自AEntitySystem<T>，其中T为实体类型

实体系统生命周期接口方法：
- `Awake(T entity)`：唤醒实体时调用
- `Init(T entity)`：初始化实体时调用
- `AfterInit(T entity)`：初始化实体后调用
- `Enable(T entity)`：激活实体时调用
- `Disable(T entity)`：禁用实体时调用
- `Update(T entity)`：定时更新实体状态
- `Destroy(T entity)`：销毁实体时调用

组件系统继承自AComponentSystem<T, C>，其中T为实体类型，C为组件类型

组件系统生命周期接口方法：
- `Awake(T entity, C component)`：唤醒实体组件时调用
- `Init(T entity, C component)`：初始化实体组件时调用
- `AfterInit(T entity, C component)`：初始化实体组件后调用
- `Enable(T entity, C component)`：激活实体组件时调用
- `Disable(T entity, C component)`：禁用实体组件时调用
- `Destroy(T entity, C component)`：销毁实体组件时调用

命名空间为ECSGame.Module.**，其中**为模块名称