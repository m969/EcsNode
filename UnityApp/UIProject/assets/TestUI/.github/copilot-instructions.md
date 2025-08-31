# 本 UI 包的 Copilot 指南

# 永远用中文回答

目的：让 AI 代理在编辑这个最小化的 UI 包时可立即上手。只记录此处可验证的事实。

## 范围与架构
- 此目录是一个自包含的 UI 包，当前包含文件：`package.xml`、`TestWindow.xml`、`GameWindow.xml`、`GameWindow.svg`。
- `package.xml` 注册由宿主/运行时加载的资源；资源位于包根路径（`/`）。
- 组件 XML 至少声明一个根 `<component .../>`，通常通过 `size` 属性定义画布尺寸。当前 `TestWindow.xml` 还包含用于演示的子节点（见下文示例）。
- `GameWindow.svg` 已作为 image 资源注册，可作参考或素材使用（见 `package.xml`）。

## 关键文件与模式
- `package.xml`
  - 根元素：`<packageDescription id="...">`；`id` 需唯一且保持稳定。当前为 `aseijf3n`。
  - 在 `<resources>` 下注册组件与资源：
    - 组件：`<component id="psy30" name="TestWindow.xml" path="/" exported="true"/>`
    - 组件：`<component id="game01" name="GameWindow.xml" path="/" exported="true"/>`
    - 图片：`<image id="psy32" name="GameWindow.svg" path="/" width="1280" height="720"/>`
  - 存在 `<publish name=""/>`；除非发布流程需要名称，否则保持不变。
- 组件 XML（示例与现状）
  - 最小形式：
    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <component size="1280,720"/>
    ```
  - 使用 UTF-8，保持 XML 格式正确。尺寸以像素表示，格式为 `宽,高`。
  - 现有组件：
    - `TestWindow.xml`：根 `component` 的 `size="1280,720"`，包含一个 `displayList`，其中含有：
      - `component` 子节点：`id="n0_psy3"`、`name="nTestBtn"`、`src="psy30"`、`fileName="StyleAButton.xml"`、`pkg="aqsec6vq"`、`xy="153,297"`，内部含 `<Button title="标题"/>`；
      - `text` 子节点：`id="n1_psy3"`、`name="nTestText"`、`xy="154,146"`、`size="148,65"`、`fontSize="12"`、`autoSize="none"`、`text="测试文本"`；
      - `graph` 子节点：`id="n2_psy3"`、`name="nRect"`、`xy="1010,127"`、`size="100,100"`、`type="rect"`。
  - `list` 子节点：
    - 纵向列表示例：`id="n3_psy3"`、`name="nVerticalList"`、`xy="156,369"`、`size="223,171"`、`overflow="scroll"`、`lineGap="15"`、`defaultItem="ui://aqsec6vqpsy30"`、`autoClearItems="true"`，内部含 `<relation .../>` 与三个带 `title` 文本的 `item` 子节点（分别为 `item标题1`、`item标题2`、`item标题3`）。
    - 横向列表示例：`id="n4_psy3"`、`name="nHorizontalList"`、`xy="359,628"`、`size="544,40"`、`layout="row"`、`overflow="scroll"`、`lineGap="15"`、`colGap="15"`、`defaultItem="ui://aqsec6vqpsy30"`、`align="center"`、`vAlign="middle"`、`autoClearItems="true"`，内部含三个带 `title` 文本的 `item` 子节点。
- 在 Windows 上也使用正斜杠路径（`/`）。

### 列表（list）节点格式（基于当前 TestWindow.xml）
- 位置与尺寸通过 `xy` 与 `size` 指定；滚动行为通过 `overflow` 控制。
- 当前示例中出现的属性（仅记录已在 XML 出现者）：
  - `defaultItem`：示例为 `ui://aqsec6vqpsy30`。
  - `autoClearItems`：示例为 `true`。
  - `overflow`：示例为 `scroll`。
  - `lineGap`：行距，示例为 `15`。
  - `colGap`：列距，示例为 `15`。
  - `layout`：列表布局，示例为 `row`（单行横排）。
  - `align`：示例为 `center`（横向列表出现）。
  - `vAlign`：示例为 `middle`（横向列表出现）。
- `defaultItem` 使用共用按钮资源索引 `ui://aqsec6vqpsy30`。
- `item` 节点支持 `title` 属性用于展示文本，类型为字符串；不设置时可省略该属性（默认为空）。

示例（均摘自本包 `TestWindow.xml`）：

```xml
<!-- 纵向列表 -->
<list id="n3_psy3" name="nVerticalList" xy="156,369" size="223,171" overflow="scroll" lineGap="15" defaultItem="ui://aqsec6vqpsy30" autoClearItems="true">
  <relation target="" sidePair="left-left"/>
  <item title="item标题1"/>
  <item title="item标题2"/>
  <item title="item标题3"/>
</list>

<!-- 横向列表 -->
<list id="n4_psy3" name="nHorizontalList" xy="359,628" size="544,40" layout="row" overflow="scroll" lineGap="15" colGap="15" defaultItem="ui://aqsec6vqpsy30" align="center" vAlign="middle" autoClearItems="true">
  <relation target="" sidePair="center-center,bottom-bottom"/>
  <item title="item标题1"/>
  <item title="item标题2"/>
  <item title="item标题3"/>
</list>
```

## 常见任务
- 新增 UI 窗口/组件
  1) 新建 `NewWindow.xml`，内容为根 `<component size="WIDTH,HEIGHT"/>`。
  2) 在 `package.xml` 的 `<resources>` 中添加：`<component id="uniqueId" name="NewWindow.xml" path="/" exported="true"/>`。
- 注册图片资源（可选）
  - 在 `<resources>` 中添加：`<image id="uniqueImageId" name="YourImage.svg" path="/" width="W" height="H"/>`。
- 重命名/删除组件
  - 同步修改磁盘上的文件名与 `package.xml` 中对应的 `<component .../>` 条目，保持一致。
- 修改窗口尺寸
  - 在目标组件 XML 中编辑 `size` 属性。

## 校验与注意事项
- 确认 `package.xml` 中每个 `name` 在声明的 `path` 下真实存在。
- 一旦被外部工具引用，请勿频繁变更组件或资源的 `id`。
- SVG 使用：当前 `GameWindow.svg` 已注册为图片资源；若仅作线框/参考稿，可选择不注册。

## 构建/运行/测试
- 此处没有构建脚本；由上层宿主/编辑器通过 `package.xml` 进行加载。
- 可视化校验在宿主中完成；本目录不包含本地测试。

当引入新属性、目录或资源类型时，请同步维护本说明。
