# OneNote Markdown 插件测试说明

本文档用于手工验证 `OneNoteMarkdown` 插件当前版本是否工作正常。

## 1. 测试目标

- OneNote 可以正常启动（不被插件阻塞）
- 插件在 Ribbon 显示 `Markdown` 标签
- 4 个核心按钮可用：
  - 导入 Markdown
  - 导出 Markdown
  - 渲染选区
  - 渲染整页

## 2. 安装包路径

当前安装包：

`D:\OpenCode\OneNote markdown\src\OneNoteMarkdown.Installer\Output\OneNoteMarkdownSetup.exe`

## 3. 安装前准备

1. 退出 OneNote（确认任务管理器里没有 `ONENOTE.EXE`）
2. 如果装过旧版 `OneNote Markdown`，先卸载
3. 安装新的 `OneNoteMarkdownSetup.exe`
4. 启动 OneNote

## 4. 启动与加载验证

### 4.1 OneNote 启动稳定性

- 预期：OneNote 能直接进入主界面，不出现“无法顺利启动”提示

### 4.2 加载项可见性

打开：`文件 -> 选项 -> 加载项`

- 预期：`OneNote Markdown` 出现在活动应用程序加载项中

### 4.3 Ribbon

- 预期：顶部功能区出现 `Markdown` 标签

## 5. 功能测试用例

新建一个测试页，粘贴下面的样例 Markdown 文本：

```markdown
# 项目测试

这是普通段落，包含 **粗体**、*斜体*、`行内代码`。

## 列表

- 第一项
- 第二项
1. 有序一
2. 有序二

## 任务

- [ ] 待办 A
- [x] 已完成 B

## 引用

> 这是引用内容

## 代码块

```csharp
Console.WriteLine("Hello Markdown");
```

---

结束。
```

---

### 用例 A：渲染选区

1. 选中上面 Markdown 的一部分
2. 点击 `渲染选区`

预期：

- 页面末尾新增一个块（标题类似“Markdown 渲染（选区）”）
- 能看到标题、列表、任务符号、代码块样式等

### 用例 B：渲染整页

1. 点击 `渲染整页`

预期：

- 页面末尾新增一个块（标题类似“Markdown 渲染（整页）”）
- 整页内容按 Markdown 规则渲染

### 用例 C：导入 Markdown

1. 准备一个 `.md` 文件（可用上面的样例）
2. 点击 `导入 Markdown` 并选择文件

预期：

- 页面末尾新增“Markdown 导入”块
- 文件内容被渲染后插入当前页

### 用例 D：导出 Markdown

1. 点击 `导出 Markdown`
2. 保存到本地
3. 打开导出的 `.md`

预期：

- 导出文件存在且可打开
- 含页面标题和正文内容

## 6. 当前已支持语法（第一阶段）

- 标题：`#` 到 `######`
- 无序列表：`-` `*` `+`
- 有序列表：`1.`
- 任务列表：`- [ ]` / `- [x]`
- 引用：`>`
- 代码块：``` 和 `~~~`
- 行内：`**粗体**` `*斜体*` `` `代码` ``
- 分隔线：`---` `***` `___`

## 7. 已知限制（当前版本）

- 暂未实现完整 GFM 表格
- 暂未实现图片/链接富渲染
- 暂未实现 live mode（实时渲染）
- 暂未实现源码/渲染双向无损编辑

## 8. 故障排查

### 8.1 看日志

日志路径：

`C:\Users\dinghuqiang\AppData\Roaming\OneNoteMarkdown\logs\onenotemarkdown.log`

### 8.2 常见问题

- **OneNote 启动失败**：先卸载插件并重启 OneNote
- **看不到插件**：检查 `文件 -> 选项 -> 加载项` 是否有 `OneNote Markdown`
- **按钮点了没反应**：先看日志是否有异常堆栈

### 8.3 关键注册表（仅排障时用）

- `HKCU\Software\Microsoft\Office\OneNote\AddIns\OneNoteMarkdown.Connect`
- `HKCR\OneNoteMarkdown.Connect`
- `HKCR\CLSID\{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32`

## 9. 测试结果记录模板

可按下面格式记录并反馈：

```text
测试时间：
OneNote 版本：
系统版本：

[启动与加载]
- OneNote 启动：通过/失败
- 加载项可见：通过/失败
- Ribbon 显示：通过/失败

[功能]
- 渲染选区：通过/失败（现象）
- 渲染整页：通过/失败（现象）
- 导入 Markdown：通过/失败（现象）
- 导出 Markdown：通过/失败（现象）

[日志]
- 是否有报错：有/无
- 关键报错行：
```
