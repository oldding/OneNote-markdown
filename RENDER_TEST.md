# OneNote Markdown 渲染测试

这是一份覆盖全部已实现语法的测试文档，用于验证导入与实时渲染效果。

## 一、标题层级

# 一级标题 H1
## 二级标题 H2
### 三级标题 H3
#### 四级标题 H4
##### 五级标题 H5
###### 六级标题 H6

## 二、行内样式

这一行包含 **粗体**、*斜体*、~~删除线~~、==黄色高亮==、++下划线++ 以及 `行内代码`。

混合使用：**粗体里有 `代码`**，*斜体里有 ==高亮==*，普通文字 ++下划线++ 收尾。

中文测试：这是**重点内容**，请==特别注意==这段 `OneNote` 文字。

## 三、列表

无序列表：

- 第一项 **加粗**
- 第二项 `代码`
- 第三项 ==高亮==
  - 嵌套第一项
  - 嵌套第二项 *斜体*

有序列表：

1. 步骤一
2. 步骤二
3. 步骤三

任务列表：

- [ ] 未完成任务
- [x] 已完成任务

## 四、代码块（语法高亮）

JavaScript：

```js
var message = "hello world"
const count = 42
function greet(name) {
    // 这是一行注释
    console.log(message)
    return `Hi, ${name}`
}
```

Python：

```python
def fibonacci(n):
    # 计算斐波那契数列
    a, b = 0, 1
    for i in range(n):
        a, b = b, a + b
    return a

print(fibonacci(10))
```

C#：

```csharp
public class Demo
{
    private int _count = 0;
    // 入口方法
    public void Run()
    {
        string msg = "done";
        Console.WriteLine(msg);
    }
}
```

SQL：

```sql
-- 查询用户
SELECT id, name FROM users WHERE age > 18 ORDER BY name;
```

## 五、引用

> 这是一段引用文字。
> 引用可以包含 **粗体** 和 `代码`。

## 六、LaTeX 数学公式

行内公式：质能方程 $E = mc^2$ 很著名。

块级公式：

$$
\int_{0}^{\infty} e^{-x^2} \, dx = \frac{\sqrt{\pi}}{2}
$$

## 七、分隔线

上半部分

---

下半部分

## 八、综合段落

最后一段用于测试普通文字与各类行内样式的混排：在 OneNote 中编写 **Markdown** 是一种 ==高效== 的方式，你可以使用 `快捷键` 实时预览，按 *回车* 即可触发渲染。这段~~旧方案~~已被新方案取代。
