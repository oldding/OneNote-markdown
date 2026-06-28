# OneNote Markdown 完整渲染演示

这是一份覆盖常用 Markdown 语法的实际测试文档。

## 一、标题层级

### 三级标题：功能验证

#### 四级标题：细节说明

##### 五级标题：补充信息

###### 六级标题：最小层级

## 二、行内样式

普通文字中可以组合 **粗体**、*斜体*、~~删除线~~、==黄色高亮==、++下划线++ 和 `inline code`。

混合示例：**重点里有 `代码`**，*强调里有 ==高亮==*，行内公式 $E = mc^2$。

## 三、无序列表

- 需求分析
- 功能开发
  - Markdown 解析
  - OneNote 写入
- 测试与发布

## 四、有序列表

1. 编写 Markdown
2. 点击“渲染整页”
3. 检查 OneNote 渲染结果

## 五、任务列表

- [x] 标题与段落
- [x] 行内样式
- [x] 代码高亮
- [ ] 发布演示视频

## 六、引用

> Markdown 负责结构，OneNote 负责整理、批注与长期沉淀。

## 七、代码高亮

```csharp
public static string Render(string markdown)
{
    // Parse Markdown and write styled content to OneNote.
    var blocks = MarkdownRenderer.RenderToBlocks(markdown);
    return $"Rendered {blocks.Count} blocks";
}
```

## 八、LaTeX 数学公式

行内公式会保留数学字体，例如 $a^2 + b^2 = c^2$。

$$
\frac{-b \pm \sqrt{b^2 - 4ac}}{2a}
$$

$$
\int_{0}^{\infty} e^{-x^2}\,dx = \frac{\sqrt{\pi}}{2}
$$

---

渲染完成：原始 Markdown 保留在上方，格式化结果追加到页面下方。
