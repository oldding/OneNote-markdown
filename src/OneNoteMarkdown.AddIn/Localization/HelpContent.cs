namespace OneNoteMarkdown.Localization
{
    /// <summary>
    /// Provides localized HTML help content.
    /// </summary>
    internal static class HelpContent
    {
        public static string GetHtml(string section)
        {
            if (LocalizationManager.CurrentLanguage == "en")
                return GetEnglish(section);
            return GetChinese(section);
        }

        private static string GetChinese(string section)
        {
            switch (section)
            {
                case "about": return GetAboutZh();
                case "quickstart": return GetQuickStartZh();
                case "features": return GetFeaturesZh();
                case "feat_renderpage": return GetRenderPageZh();
                case "feat_rendersel": return GetRenderSelZh();
                case "feat_live": return GetLiveZh();
                case "feat_io": return GetIoZh();
                case "feat_latex": return GetLatexZh();
                case "feat_code": return GetCodeZh();
                case "settings": return GetSettingsZh();
                case "shortcuts": return GetShortcutsZh();
                case "faq": return GetFaqZh();
                default: return GetAboutZh();
            }
        }

        private static string GetEnglish(string section)
        {
            switch (section)
            {
                case "about": return GetAboutEn();
                case "quickstart": return GetQuickStartEn();
                case "features": return GetFeaturesEn();
                case "feat_renderpage": return GetRenderPageEn();
                case "feat_rendersel": return GetRenderSelEn();
                case "feat_live": return GetLiveEn();
                case "feat_io": return GetIoEn();
                case "feat_latex": return GetLatexEn();
                case "feat_code": return GetCodeEn();
                case "settings": return GetSettingsEn();
                case "shortcuts": return GetShortcutsEn();
                case "faq": return GetFaqEn();
                default: return GetAboutEn();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CHINESE CONTENT
        // ═══════════════════════════════════════════════════════════════

        private static string GetAboutZh()
        {
            return "<h1>关于 OneNote Markdown</h1>"
                + "<p class='version'>版本：1.1.0</p>"
                + "<h2>开发背景</h2>"
                + "<p>本插件由 OneNote MVP 开发。</p>"
                + "<p>Microsoft OneNote 是一款优秀的笔记软件，但遗憾的是 OneNote 原生并不支持 Markdown 语法。"
                + "许多用户在日常工作和学习中已经习惯了 Markdown 的高效写作方式，却无法在 OneNote 中直接使用。</p>"
                + "<p>为了解决这一痛点，我们开发了 OneNote Markdown —— "
                + "一款完全独立的 Markdown 渲染插件，让所有 OneNote 桌面版用户都能在笔记中使用 Markdown 语法，"
                + "享受高效、美观的格式化写作体验。</p>"
                + "<p>本插件不依赖任何第三方在线服务，所有渲染均在本地完成，数据安全可控。</p>"
                + "<h2>主要功能</h2>"
                + "<ul>"
                + "<li>Markdown 渲染 —— 标题、列表、代码块、表格、引用块等</li>"
                + "<li>实时预览模式 —— 源码区与预览区自动同步</li>"
                + "<li>LaTeX 公式 —— 块级数学公式渲染为高清图片</li>"
                + "<li>代码高亮 —— 多种编程语言语法着色</li>"
                + "<li>图表源码块 —— Mermaid 等图表保留格式显示</li>"
                + "<li>导入/导出 —— .md 文件与 OneNote 页面互转</li>"
                + "</ul>"
                + "<h2>技术栈</h2>"
                + "<ul>"
                + "<li>C# / .NET Framework 4.8</li>"
                + "<li>COM Add-in (IDTExtensibility2)</li>"
                + "<li>OneNote Interop API</li>"
                + "<li>WPF-Math（LaTeX 渲染引擎）</li>"
                + "</ul>"
                + "<h2>联系方式</h2>"
                + "<ul>"
                + "<li>GitHub：github.com/oldding/OneNote-markdown</li>"
                + "<li>开发者：OneNote MVP</li>"
                + "</ul>"
                + "<h2>许可证</h2>"
                + "<p>MIT License</p>";
        }

        private static string GetQuickStartZh()
        {
            return "<h1>快速入门</h1>"
                + "<h2>第一步：确认 OneNote 位数</h2>"
                + "<p>打开 OneNote → <code>文件</code> → <code>帐户</code> → 点击右侧 <code>关于 OneNote</code>，"
                + "在弹出窗口标题栏查看是 32 位还是 64 位。</p>"
                + "<p><b>重要：</b>安装包必须匹配 <b>OneNote 的位数</b>，不是 Windows 的位数。"
                + "很多 64 位 Windows 系统上安装的是 32 位 OneNote（尤其是 Microsoft 365 默认安装）。</p>"
                + "<h2>第二步：安装插件</h2>"
                + "<ol>"
                + "<li>32 位 OneNote → 运行 <code>OneNoteMarkdownSetup-x86.exe</code></li>"
                + "<li>64 位 OneNote → 运行 <code>OneNoteMarkdownSetup-x64.exe</code></li>"
                + "<li>安装完成后，重新启动 OneNote</li>"
                + "<li>功能区（Ribbon）会出现 \"Markdown\" 选项卡</li>"
                + "</ol>"
                + "<h2>第三步：开始使用</h2>"
                + "<h3>方式一：渲染整页</h3>"
                + "<ol>"
                + "<li>在 OneNote 页面中直接输入 Markdown 文本</li>"
                + "</ol>"
                + "<pre><code># 这是标题\n\n这是一段正文，支持 **粗体** 和 *斜体*。\n\n- 列表项 1\n- 列表项 2\n\n```python\nprint(\"Hello World\")\n```</code></pre>"
                + "<ol start='2'>"
                + "<li>点击功能区 \"渲染整页\" 或按 <code>F5</code></li>"
                + "<li>页面末尾将生成格式化的渲染结果</li>"
                + "</ol>"
                + "<h3>方式二：实时模式</h3>"
                + "<ol>"
                + "<li>点击功能区 \"实时模式\" 或按 <code>Ctrl+\\</code></li>"
                + "<li>页面自动创建 \"源码区\" 和 \"预览区\"</li>"
                + "<li>在源码区编写 Markdown，回车后预览区自动更新</li>"
                + "</ol>"
                + "<h2>卸载</h2>"
                + "<p>通过 Windows 的 \"程序和功能\" 或 \"设置 → 应用\" 卸载 \"OneNote Markdown\" 即可。</p>";
        }

        private static string GetFeaturesZh()
        {
            return "<h1>功能说明</h1>"
                + "<p>OneNote Markdown 提供以下功能，请从左侧目录选择查看详情：</p>"
                + "<ul>"
                + "<li><b>渲染整页</b> —— 将当前页正文按 Markdown 重新渲染</li>"
                + "<li><b>渲染选区</b> —— 渲染选中的 Markdown 文本</li>"
                + "<li><b>实时模式</b> —— 源码/预览双区实时同步</li>"
                + "<li><b>导入/导出</b> —— .md 文件与 OneNote 页面互转</li>"
                + "<li><b>LaTeX 公式</b> —— 行内与块级数学公式渲染</li>"
                + "<li><b>代码高亮</b> —— 多语言语法着色</li>"
                + "</ul>";
        }

        private static string GetRenderPageZh()
        {
            return "<h1>渲染整页</h1>"
                + "<p>将当前页面的全部正文内容按 Markdown 语法解析，并在页面末尾生成渲染后的格式化内容。</p>"
                + "<h2>使用方法</h2>"
                + "<ol>"
                + "<li>在 OneNote 页面中编写 Markdown 文本</li>"
                + "<li>点击功能区 \"渲染整页\" 或按快捷键 <code>F5</code></li>"
                + "<li>渲染结果追加到页面末尾</li>"
                + "</ol>"
                + "<h2>支持的 Markdown 语法</h2>"
                + "<table>"
                + "<tr><th>语法</th><th>写法</th><th>示例</th></tr>"
                + "<tr><td>标题</td><td><code># ~ ######</code></td><td><code># 一级标题</code></td></tr>"
                + "<tr><td>粗体</td><td><code>**文本**</code></td><td><code>**重要**</code></td></tr>"
                + "<tr><td>斜体</td><td><code>*文本*</code></td><td><code>*强调*</code></td></tr>"
                + "<tr><td>删除线</td><td><code>~~文本~~</code></td><td><code>~~废弃~~</code></td></tr>"
                + "<tr><td>高亮</td><td><code>==文本==</code></td><td><code>==重点==</code></td></tr>"
                + "<tr><td>下划线</td><td><code>++文本++</code></td><td><code>++下划线++</code></td></tr>"
                + "<tr><td>行内代码</td><td><code>`代码`</code></td><td><code>`var x = 1`</code></td></tr>"
                + "<tr><td>无序列表</td><td><code>- / * / +</code></td><td><code>- 项目</code></td></tr>"
                + "<tr><td>有序列表</td><td><code>1. / 1)</code></td><td><code>1. 第一项</code></td></tr>"
                + "<tr><td>任务列表</td><td><code>- [ ] / - [x]</code></td><td><code>- [x] 已完成</code></td></tr>"
                + "<tr><td>引用块</td><td><code>&gt; 文本</code></td><td><code>&gt; 这是引用</code></td></tr>"
                + "<tr><td>代码块</td><td><code>``` / ~~~</code></td><td>见代码高亮章节</td></tr>"
                + "<tr><td>分隔线</td><td><code>---</code></td><td><code>---</code></td></tr>"
                + "<tr><td>链接</td><td><code>[文本](url)</code></td><td><code>[百度](https://baidu.com)</code></td></tr>"
                + "<tr><td>图片</td><td><code>![alt](url)</code></td><td><code>![logo](img.png)</code></td></tr>"
                + "<tr><td>表格</td><td><code>| 列 | 列 |</code></td><td>见下方示例</td></tr>"
                + "<tr><td>TOC 目录</td><td><code>[TOC]</code></td><td><code>[TOC]</code></td></tr>"
                + "<tr><td>LaTeX 公式</td><td><code>$$ ... $$</code></td><td>见 LaTeX 章节</td></tr>"
                + "</table>"
                + "<h2>表格示例</h2>"
                + "<pre><code>| 姓名 | 分数 | 等级 |\n|------|------|------|\n| 张三 | 95   | A    |\n| 李四 | 82   | B    |</code></pre>"
                + "<h2>注意事项</h2>"
                + "<ul>"
                + "<li>渲染结果追加到页面末尾，不会覆盖原有内容</li>"
                + "<li>如果页面中有实时模式的区域，这些区域不会被重复渲染</li>"
                + "<li>渲染后可继续编辑源文本，再次 F5 会追加新的渲染结果</li>"
                + "</ul>";
        }

        private static string GetRenderSelZh()
        {
            return "<h1>渲染选区</h1>"
                + "<p>仅渲染当前选中的 Markdown 文本，结果追加到页面末尾。</p>"
                + "<h2>使用方法</h2>"
                + "<ol>"
                + "<li>在 OneNote 页面中用鼠标选中一段 Markdown 文本</li>"
                + "<li>点击功能区 \"渲染选区\" 按钮</li>"
                + "<li>选中的文本将被解析为 Markdown 并渲染到页面末尾</li>"
                + "</ol>"
                + "<h2>使用场景</h2>"
                + "<ul>"
                + "<li>页面中混合了普通笔记和 Markdown 片段，只想渲染其中一部分</li>"
                + "<li>测试某段 Markdown 语法是否正确</li>"
                + "<li>逐段渲染长文档</li>"
                + "</ul>";
        }

        private static string GetLiveZh()
        {
            return "<h1>实时模式</h1>"
                + "<p>实时模式是本插件的核心特色功能，提供所见即所得的 Markdown 编辑体验。</p>"
                + "<h2>工作原理</h2>"
                + "<p>开启实时模式后，当前页面会自动创建两块独立的内容区域（Outline）：</p>"
                + "<ul>"
                + "<li><b>Markdown 源码（实时）</b> —— 你在此区域编写 Markdown 原始文本</li>"
                + "<li><b>Markdown 预览（实时）</b> —— 插件自动将源码渲染为格式化内容显示在此区域</li>"
                + "</ul>"
                + "<p>当你在源码区按下回车键提交一行编辑后，插件会自动检测到内容变化并刷新预览区。</p>"
                + "<h2>开启/关闭实时模式</h2>"
                + "<ol>"
                + "<li>点击功能区的 \"实时模式\" 按钮（按钮会高亮表示已开启）</li>"
                + "<li>或使用快捷键 <code>Ctrl+\\</code></li>"
                + "<li>再次点击或按快捷键即可关闭</li>"
                + "</ol>"
                + "<h2>详细操作步骤</h2>"
                + "<ol>"
                + "<li><b>开启实时模式</b> —— 页面中会出现带标题的两块内容区</li>"
                + "<li><b>定位到源码区</b> —— 点击 \"Markdown 源码（实时）\" 下方的文本区域</li>"
                + "<li><b>编写 Markdown</b> —— 正常输入 Markdown 文本</li>"
                + "</ol>"
                + "<pre><code># 今日笔记\n\n## 会议要点\n\n- 项目进度 **正常**\n- 下周需要完成：\n  1. 接口开发\n  2. 单元测试\n\n> 注意：截止日期是周五\n\n$$\nf(x) = \\int_0^x t^2 dt\n$$</code></pre>"
                + "<ol start='4'>"
                + "<li><b>按回车提交</b> —— 每次按回车后，插件会自动检测并渲染当前行</li>"
                + "<li><b>查看预览</b> —— 预览区会自动显示渲染后的格式化内容</li>"
                + "<li><b>手动刷新</b> —— 如需强制刷新整个预览，按 <code>F5</code></li>"
                + "</ol>"
                + "<h2>实时模式下的注意事项</h2>"
                + "<table>"
                + "<tr><th>注意项</th><th>说明</th></tr>"
                + "<tr><td>只在源码区编辑</td><td>预览区的内容会被自动覆盖，不要手动修改预览区</td></tr>"
                + "<tr><td>刷新时机</td><td>插件在回车后触发渲染，受 OneNote 提交时机影响，可能有 0.2~0.5 秒延迟</td></tr>"
                + "<tr><td>多行内容</td><td>建议每写完一段按回车确认，插件逐行检测渲染</td></tr>"
                + "<tr><td>复杂公式</td><td>LaTeX 块级公式需要 <code>$$</code> 独占一行，写完整块后回车触发渲染</td></tr>"
                + "<tr><td>代码块</td><td>围栏代码块（<code>```</code>）在关闭标记行回车后触发渲染</td></tr>"
                + "<tr><td>未自动刷新时</td><td>按 <code>F5</code> 可手动刷新，或按 <code>Ctrl+Enter</code> 渲染当前行</td></tr>"
                + "</table>"
                + "<h2>实时模式 vs 渲染整页</h2>"
                + "<table>"
                + "<tr><th>对比项</th><th>实时模式</th><th>渲染整页</th></tr>"
                + "<tr><td>触发方式</td><td>自动（回车触发）</td><td>手动（F5 或按钮）</td></tr>"
                + "<tr><td>编辑区域</td><td>专用源码区</td><td>页面任意位置</td></tr>"
                + "<tr><td>输出位置</td><td>专用预览区（自动更新）</td><td>页面末尾（追加）</td></tr>"
                + "<tr><td>适用场景</td><td>持续编辑、实时查看效果</td><td>一次性渲染完整文档</td></tr>"
                + "</table>";
        }

        private static string GetIoZh()
        {
            return "<h1>导入/导出</h1>"
                + "<h2>导入 Markdown 文件</h2>"
                + "<p>将外部 <code>.md</code> 文件导入并渲染到当前 OneNote 页面。</p>"
                + "<h3>操作步骤</h3>"
                + "<ol>"
                + "<li>确保当前已打开一个 OneNote 页面</li>"
                + "<li>点击功能区 \"导入 Markdown\" 按钮</li>"
                + "<li>在文件选择对话框中选择 <code>.md</code> 或 <code>.markdown</code> 文件</li>"
                + "<li>插件读取文件内容，解析 Markdown，将渲染结果追加到当前页面</li>"
                + "</ol>"
                + "<h3>支持的文件类型</h3>"
                + "<ul>"
                + "<li><code>*.md</code> —— 标准 Markdown 文件</li>"
                + "<li><code>*.markdown</code> —— Markdown 文件（备选扩展名）</li>"
                + "<li>文件大小限制：10 MB</li>"
                + "<li>文件编码：UTF-8（推荐）</li>"
                + "</ul>"
                + "<h2>导出 Markdown 文件</h2>"
                + "<p>将当前 OneNote 页面的文本内容导出为 <code>.md</code> 文件。</p>"
                + "<h3>操作步骤</h3>"
                + "<ol>"
                + "<li>打开要导出的 OneNote 页面</li>"
                + "<li>点击功能区 \"导出 Markdown\" 按钮</li>"
                + "<li>选择保存位置和文件名</li>"
                + "<li>文件以 UTF-8 编码保存</li>"
                + "</ol>"
                + "<h2>复制到剪贴板</h2>"
                + "<p>将当前页面导出为 Markdown 格式并直接复制到系统剪贴板。</p>"
                + "<ul>"
                + "<li>点击功能区 \"复制 Markdown\" 按钮</li>"
                + "<li>或按快捷键 <code>F8</code></li>"
                + "</ul>";
        }

        private static string GetLatexZh()
        {
            return "<h1>LaTeX 公式</h1>"
                + "<p>本插件支持将 LaTeX 数学公式渲染为高清 PNG 图片并嵌入 OneNote 页面。</p>"
                + "<h2>块级公式（Display Math）</h2>"
                + "<p>使用独占行的 <code>$$</code> 标记包裹公式：</p>"
                + "<pre><code>$$\nE = mc^2\n$$</code></pre>"
                + "<p><b>注意：</b><code>$$</code> 必须各自独占一行，公式内容在中间行。</p>"
                + "<h2>输入示例</h2>"
                + "<h3>基本运算</h3>"
                + "<pre><code>$$\na^2 + b^2 = c^2\n$$</code></pre>"
                + "<h3>分数与根号</h3>"
                + "<pre><code>$$\nx = \\frac{-b \\pm \\sqrt{b^2 - 4ac}}{2a}\n$$</code></pre>"
                + "<h3>积分</h3>"
                + "<pre><code>$$\n\\int_0^\\infty e^{-x^2} dx = \\frac{\\sqrt{\\pi}}{2}\n$$</code></pre>"
                + "<h3>矩阵</h3>"
                + "<pre><code>$$\nA = \\begin{pmatrix}\na &amp; b \\\\\nc &amp; d\n\\end{pmatrix}\n$$</code></pre>"
                + "<h2>常用 LaTeX 符号速查</h2>"
                + "<table>"
                + "<tr><th>输入</th><th>含义</th></tr>"
                + "<tr><td><code>\\frac{a}{b}</code></td><td>分数 a/b</td></tr>"
                + "<tr><td><code>\\sqrt{x}</code></td><td>平方根</td></tr>"
                + "<tr><td><code>x^{n}</code></td><td>上标（幂）</td></tr>"
                + "<tr><td><code>x_{i}</code></td><td>下标</td></tr>"
                + "<tr><td><code>\\sum</code></td><td>求和 Σ</td></tr>"
                + "<tr><td><code>\\int</code></td><td>积分 ∫</td></tr>"
                + "<tr><td><code>\\infty</code></td><td>无穷 ∞</td></tr>"
                + "<tr><td><code>\\alpha, \\beta, \\gamma</code></td><td>希腊字母</td></tr>"
                + "<tr><td><code>\\leq, \\geq, \\neq</code></td><td>≤, ≥, ≠</td></tr>"
                + "</table>"
                + "<h2>在实时模式中输入公式</h2>"
                + "<ol>"
                + "<li>在源码区输入 <code>$$</code> 并回车</li>"
                + "<li>输入公式内容，如 <code>E = mc^2</code></li>"
                + "<li>输入结束的 <code>$$</code> 并回车</li>"
                + "<li>插件检测到完整的公式块后自动渲染为图片</li>"
                + "</ol>"
                + "<h2>配置选项</h2>"
                + "<table>"
                + "<tr><th>配置项</th><th>默认值</th><th>说明</th></tr>"
                + "<tr><td><code>enable.latex.image</code></td><td>true</td><td>设为 false 则公式以纯文本保留</td></tr>"
                + "<tr><td><code>font.math</code></td><td>Cambria Math</td><td>公式渲染使用的数学字体</td></tr>"
                + "</table>";
        }

        private static string GetCodeZh()
        {
            return "<h1>代码高亮</h1>"
                + "<p>围栏代码块支持多种编程语言的语法高亮。</p>"
                + "<h2>基本语法</h2>"
                + "<p>使用三个反引号 <code>```</code> 包裹代码块，在开头标记后指定语言名称：</p>"
                + "<pre><code>```javascript\nfunction greet(name) {\n    return `Hello, ${name}!`;\n}\n```</code></pre>"
                + "<h2>支持的语言</h2>"
                + "<table>"
                + "<tr><th>语言标识</th><th>语言</th></tr>"
                + "<tr><td><code>csharp</code> / <code>cs</code></td><td>C#</td></tr>"
                + "<tr><td><code>python</code> / <code>py</code></td><td>Python</td></tr>"
                + "<tr><td><code>javascript</code> / <code>js</code></td><td>JavaScript</td></tr>"
                + "<tr><td><code>typescript</code> / <code>ts</code></td><td>TypeScript</td></tr>"
                + "<tr><td><code>java</code></td><td>Java</td></tr>"
                + "<tr><td><code>cpp</code> / <code>c++</code></td><td>C++</td></tr>"
                + "<tr><td><code>sql</code></td><td>SQL</td></tr>"
                + "<tr><td><code>bash</code> / <code>shell</code></td><td>Shell</td></tr>"
                + "<tr><td><code>json</code></td><td>JSON</td></tr>"
                + "<tr><td><code>xml</code></td><td>XML</td></tr>"
                + "</table>"
                + "<h2>配置选项</h2>"
                + "<table>"
                + "<tr><th>配置项</th><th>默认值</th><th>说明</th></tr>"
                + "<tr><td><code>font.monospace</code></td><td>Consolas</td><td>代码块字体</td></tr>"
                + "<tr><td><code>font.size.code</code></td><td>10</td><td>代码字号</td></tr>"
                + "<tr><td><code>enable.code.lineNumber</code></td><td>false</td><td>显示行号</td></tr>"
                + "</table>"
                + "<h2>图表源码块</h2>"
                + "<ul>"
                + "<li><code>```mermaid</code> —— Mermaid 流程图/时序图</li>"
                + "<li><code>```flow</code> —— 流程图</li>"
                + "<li><code>```sequence</code> —— 时序图</li>"
                + "</ul>";
        }

        private static string GetSettingsZh()
        {
            return "<h1>设置说明</h1>"
                + "<p>设置文件位于：<code>%AppData%\\OneNoteMarkdown\\settings\\theme.ini</code></p>"
                + "<p>点击功能区 \"设置\" 按钮可直接打开设置对话框。</p>"
                + "<h2>可配置项</h2>"
                + "<table>"
                + "<tr><th>配置项</th><th>默认值</th><th>说明</th></tr>"
                + "<tr><td><code>font.family</code></td><td>Calibri</td><td>正文字体</td></tr>"
                + "<tr><td><code>font.monospace</code></td><td>Consolas</td><td>代码字体</td></tr>"
                + "<tr><td><code>font.math</code></td><td>Cambria Math</td><td>数学公式字体</td></tr>"
                + "<tr><td><code>font.size.paragraph</code></td><td>11</td><td>正文字号</td></tr>"
                + "<tr><td><code>font.size.code</code></td><td>10</td><td>代码字号</td></tr>"
                + "<tr><td><code>enable.latex.image</code></td><td>true</td><td>是否渲染 LaTeX 为图片</td></tr>"
                + "<tr><td><code>enable.code.lineNumber</code></td><td>false</td><td>是否显示代码行号</td></tr>"
                + "<tr><td><code>language</code></td><td>auto</td><td>界面语言（auto/zh/en）</td></tr>"
                + "</table>"
                + "<p>修改后重新渲染页面即可生效。</p>";
        }

        private static string GetShortcutsZh()
        {
            return "<h1>快捷键</h1>"
                + "<table>"
                + "<tr><th>快捷键</th><th>功能</th></tr>"
                + "<tr><td><code>F5</code></td><td>渲染整页</td></tr>"
                + "<tr><td><code>F8</code></td><td>复制 Markdown 到剪贴板</td></tr>"
                + "<tr><td><code>Ctrl+\\</code></td><td>开启/关闭实时模式</td></tr>"
                + "<tr><td><code>Ctrl+Enter</code></td><td>渲染当前行</td></tr>"
                + "</table>"
                + "<h2>说明</h2>"
                + "<ul>"
                + "<li>快捷键为全局注册，仅在 OneNote 窗口激活时触发</li>"
                + "<li>当前版本不再拦截 Enter 键，避免影响正常换行输入</li>"
                + "</ul>";
        }

        private static string GetFaqZh()
        {
            return "<h1>常见问题</h1>"
                + "<h2>Q: 安装后 OneNote 中没有出现 Markdown 选项卡</h2>"
                + "<p><b>A:</b> 请检查以下几点：</p>"
                + "<ol>"
                + "<li>确认安装包位数与 OneNote 位数一致</li>"
                + "<li>安装后必须重启 OneNote</li>"
                + "<li>检查 OneNote 是否禁用了插件：文件 → 选项 → 加载项</li>"
                + "</ol>"
                + "<h2>Q: 实时模式回车后预览没有更新</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>实时模式依赖 OneNote 的内容提交时机，可能有 0.2~0.5 秒延迟</li>"
                + "<li>如果等待后仍无变化，按 <code>F5</code> 手动触发刷新</li>"
                + "<li>确认光标在 \"Markdown 源码（实时）\" 区域内</li>"
                + "</ul>"
                + "<h2>Q: LaTeX 公式没有渲染为图片</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>确认 <code>$$</code> 各自独占一行</li>"
                + "<li>检查设置 <code>enable.latex.image=true</code></li>"
                + "<li>公式语法错误时会保留原始文本</li>"
                + "</ul>"
                + "<h2>Q: 快捷键无响应</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>快捷键可能与其他软件冲突</li>"
                + "<li>确认 OneNote 窗口处于前台激活状态</li>"
                + "</ul>"
                + "<h2>日志文件</h2>"
                + "<p><code>%AppData%\\OneNoteMarkdown\\logs\\onenotemarkdown.log</code></p>";
        }

        // ═══════════════════════════════════════════════════════════════
        // ENGLISH CONTENT
        // ═══════════════════════════════════════════════════════════════

        private static string GetAboutEn()
        {
            return "<h1>About OneNote Markdown</h1>"
                + "<p class='version'>Version: 1.1.0</p>"
                + "<h2>Background</h2>"
                + "<p>This plugin is developed by a OneNote MVP.</p>"
                + "<p>Microsoft OneNote is an excellent note-taking application, but unfortunately it does not natively support Markdown syntax. "
                + "Many users have become accustomed to Markdown's efficient writing workflow in their daily work and study, "
                + "yet cannot use it directly in OneNote.</p>"
                + "<p>To address this gap, we developed OneNote Markdown — "
                + "a fully standalone Markdown rendering plugin that enables all OneNote desktop users to write with Markdown syntax "
                + "and enjoy an efficient, beautifully formatted note-taking experience.</p>"
                + "<p>This plugin does not rely on any third-party online services. All rendering is done locally, keeping your data secure.</p>"
                + "<h2>Key Features</h2>"
                + "<ul>"
                + "<li>Markdown Rendering — headings, lists, code blocks, tables, block quotes, etc.</li>"
                + "<li>Live Preview Mode — source and preview areas auto-sync</li>"
                + "<li>LaTeX Formulas — block-level math rendered as high-resolution images</li>"
                + "<li>Code Highlighting — syntax coloring for multiple programming languages</li>"
                + "<li>Diagram Source Blocks — Mermaid and other diagrams preserved in formatted display</li>"
                + "<li>Import/Export — convert between .md files and OneNote pages</li>"
                + "</ul>"
                + "<h2>Tech Stack</h2>"
                + "<ul>"
                + "<li>C# / .NET Framework 4.8</li>"
                + "<li>COM Add-in (IDTExtensibility2)</li>"
                + "<li>OneNote Interop API</li>"
                + "<li>WPF-Math (LaTeX rendering engine)</li>"
                + "</ul>"
                + "<h2>Contact</h2>"
                + "<ul>"
                + "<li>GitHub: github.com/oldding/OneNote-markdown</li>"
                + "<li>Developer: OneNote MVP</li>"
                + "</ul>"
                + "<h2>License</h2>"
                + "<p>MIT License</p>";
        }

        private static string GetQuickStartEn()
        {
            return "<h1>Quick Start</h1>"
                + "<h2>Step 1: Check OneNote Architecture</h2>"
                + "<p>Open OneNote → <code>File</code> → <code>Account</code> → Click <code>About OneNote</code> on the right, "
                + "and check the title bar of the popup window for 32-bit or 64-bit.</p>"
                + "<p><b>Important:</b> The installer must match <b>OneNote's architecture</b>, not Windows. "
                + "Many 64-bit Windows systems have 32-bit OneNote installed (especially Microsoft 365 default installs).</p>"
                + "<h2>Step 2: Install the Plugin</h2>"
                + "<ol>"
                + "<li>32-bit OneNote → Run <code>OneNoteMarkdownSetup-x86.exe</code></li>"
                + "<li>64-bit OneNote → Run <code>OneNoteMarkdownSetup-x64.exe</code></li>"
                + "<li>After installation, restart OneNote</li>"
                + "<li>A \"Markdown\" tab will appear in the Ribbon</li>"
                + "</ol>"
                + "<h2>Step 3: Start Using</h2>"
                + "<h3>Method 1: Render Page</h3>"
                + "<ol>"
                + "<li>Type Markdown text directly in a OneNote page</li>"
                + "</ol>"
                + "<pre><code># This is a heading\n\nThis is body text with **bold** and *italic*.\n\n- Item 1\n- Item 2\n\n```python\nprint(\"Hello World\")\n```</code></pre>"
                + "<ol start='2'>"
                + "<li>Click \"Render Page\" in the Ribbon or press <code>F5</code></li>"
                + "<li>Formatted output will appear at the end of the page</li>"
                + "</ol>"
                + "<h3>Method 2: Live Mode</h3>"
                + "<ol>"
                + "<li>Click \"Live Mode\" in the Ribbon or press <code>Ctrl+\\</code></li>"
                + "<li>The page automatically creates \"Source\" and \"Preview\" areas</li>"
                + "<li>Write Markdown in the source area; preview updates on Enter</li>"
                + "</ol>"
                + "<h2>Uninstall</h2>"
                + "<p>Uninstall via Windows \"Programs and Features\" or \"Settings → Apps\".</p>";
        }

        private static string GetFeaturesEn()
        {
            return "<h1>Features</h1>"
                + "<p>OneNote Markdown provides the following features. Select from the left panel for details:</p>"
                + "<ul>"
                + "<li><b>Render Page</b> — Re-render the entire page as Markdown</li>"
                + "<li><b>Render Selection</b> — Render selected Markdown text</li>"
                + "<li><b>Live Mode</b> — Real-time source/preview sync</li>"
                + "<li><b>Import/Export</b> — Convert between .md files and OneNote pages</li>"
                + "<li><b>LaTeX Formulas</b> — Inline and block math rendering</li>"
                + "<li><b>Code Highlighting</b> — Multi-language syntax coloring</li>"
                + "</ul>";
        }

        private static string GetRenderPageEn()
        {
            return "<h1>Render Page</h1>"
                + "<p>Parses the entire page content as Markdown and appends formatted output at the end.</p>"
                + "<h2>Usage</h2>"
                + "<ol>"
                + "<li>Write Markdown text in a OneNote page</li>"
                + "<li>Click \"Render Page\" in the Ribbon or press <code>F5</code></li>"
                + "<li>Rendered result is appended to the page</li>"
                + "</ol>"
                + "<h2>Supported Markdown Syntax</h2>"
                + "<table>"
                + "<tr><th>Syntax</th><th>Format</th><th>Example</th></tr>"
                + "<tr><td>Heading</td><td><code># ~ ######</code></td><td><code># Heading 1</code></td></tr>"
                + "<tr><td>Bold</td><td><code>**text**</code></td><td><code>**important**</code></td></tr>"
                + "<tr><td>Italic</td><td><code>*text*</code></td><td><code>*emphasis*</code></td></tr>"
                + "<tr><td>Strikethrough</td><td><code>~~text~~</code></td><td><code>~~removed~~</code></td></tr>"
                + "<tr><td>Highlight</td><td><code>==text==</code></td><td><code>==key point==</code></td></tr>"
                + "<tr><td>Underline</td><td><code>++text++</code></td><td><code>++underlined++</code></td></tr>"
                + "<tr><td>Inline code</td><td><code>`code`</code></td><td><code>`var x = 1`</code></td></tr>"
                + "<tr><td>Unordered list</td><td><code>- / * / +</code></td><td><code>- Item</code></td></tr>"
                + "<tr><td>Ordered list</td><td><code>1. / 1)</code></td><td><code>1. First</code></td></tr>"
                + "<tr><td>Task list</td><td><code>- [ ] / - [x]</code></td><td><code>- [x] Done</code></td></tr>"
                + "<tr><td>Block quote</td><td><code>&gt; text</code></td><td><code>&gt; A quote</code></td></tr>"
                + "<tr><td>Code block</td><td><code>``` / ~~~</code></td><td>See Code section</td></tr>"
                + "<tr><td>Horizontal rule</td><td><code>---</code></td><td><code>---</code></td></tr>"
                + "<tr><td>Link</td><td><code>[text](url)</code></td><td><code>[Google](https://google.com)</code></td></tr>"
                + "<tr><td>Image</td><td><code>![alt](url)</code></td><td><code>![logo](img.png)</code></td></tr>"
                + "<tr><td>Table</td><td><code>| Col | Col |</code></td><td>See example below</td></tr>"
                + "<tr><td>TOC</td><td><code>[TOC]</code></td><td><code>[TOC]</code></td></tr>"
                + "<tr><td>LaTeX</td><td><code>$$ ... $$</code></td><td>See LaTeX section</td></tr>"
                + "</table>"
                + "<h2>Notes</h2>"
                + "<ul>"
                + "<li>Rendered output is appended; it does not overwrite existing content</li>"
                + "<li>Live mode areas are not re-rendered</li>"
                + "<li>You can edit source and press F5 again to append new output</li>"
                + "</ul>";
        }

        private static string GetRenderSelEn()
        {
            return "<h1>Render Selection</h1>"
                + "<p>Renders only the selected Markdown text, appending the result to the page.</p>"
                + "<h2>Usage</h2>"
                + "<ol>"
                + "<li>Select a block of Markdown text in the OneNote page</li>"
                + "<li>Click \"Render Selection\" in the Ribbon</li>"
                + "<li>The selected text is parsed and rendered at the page end</li>"
                + "</ol>"
                + "<h2>Use Cases</h2>"
                + "<ul>"
                + "<li>Only render a portion of mixed content</li>"
                + "<li>Test whether a Markdown snippet is correct</li>"
                + "<li>Render a long document section by section</li>"
                + "</ul>";
        }

        private static string GetLiveEn()
        {
            return "<h1>Live Mode</h1>"
                + "<p>Live Mode is the core feature of this plugin, providing a WYSIWYG Markdown editing experience.</p>"
                + "<h2>How It Works</h2>"
                + "<p>When Live Mode is enabled, the page automatically creates two independent content areas (Outlines):</p>"
                + "<ul>"
                + "<li><b>Markdown Source (Live)</b> — Write your raw Markdown text here</li>"
                + "<li><b>Markdown Preview (Live)</b> — The plugin renders formatted content here automatically</li>"
                + "</ul>"
                + "<p>After pressing Enter in the source area, the plugin detects changes and refreshes the preview.</p>"
                + "<h2>Enable/Disable</h2>"
                + "<ol>"
                + "<li>Click \"Live Mode\" in the Ribbon (button highlights when active)</li>"
                + "<li>Or use the shortcut <code>Ctrl+\\</code></li>"
                + "<li>Click again or press the shortcut to disable</li>"
                + "</ol>"
                + "<h2>Step-by-Step</h2>"
                + "<ol>"
                + "<li><b>Enable Live Mode</b> — Two titled content areas appear on the page</li>"
                + "<li><b>Go to source area</b> — Click the text area below \"Markdown Source (Live)\"</li>"
                + "<li><b>Write Markdown</b> — Type normally</li>"
                + "</ol>"
                + "<pre><code># Today's Notes\n\n## Meeting Points\n\n- Progress is **on track**\n- Next week:\n  1. API development\n  2. Unit tests\n\n> Note: Deadline is Friday\n\n$$\nf(x) = \\int_0^x t^2 dt\n$$</code></pre>"
                + "<ol start='4'>"
                + "<li><b>Press Enter</b> — The plugin detects and renders after each Enter</li>"
                + "<li><b>View preview</b> — Preview area shows formatted content automatically</li>"
                + "<li><b>Manual refresh</b> — Press <code>F5</code> to force a full refresh</li>"
                + "</ol>"
                + "<h2>Important Notes</h2>"
                + "<table>"
                + "<tr><th>Note</th><th>Details</th></tr>"
                + "<tr><td>Edit only in source</td><td>Preview content is auto-overwritten; do not edit the preview area</td></tr>"
                + "<tr><td>Refresh timing</td><td>Rendering triggers on Enter; there may be a 0.2~0.5s delay</td></tr>"
                + "<tr><td>Multi-line content</td><td>Press Enter after each paragraph for detection</td></tr>"
                + "<tr><td>Complex formulas</td><td><code>$$</code> must be on its own line; Enter after closing <code>$$</code></td></tr>"
                + "<tr><td>Code blocks</td><td>Fenced blocks (<code>```</code>) render after Enter on the closing line</td></tr>"
                + "<tr><td>No auto-refresh</td><td>Press <code>F5</code> or <code>Ctrl+Enter</code> to render manually</td></tr>"
                + "</table>"
                + "<h2>Live Mode vs Render Page</h2>"
                + "<table>"
                + "<tr><th>Comparison</th><th>Live Mode</th><th>Render Page</th></tr>"
                + "<tr><td>Trigger</td><td>Auto (on Enter)</td><td>Manual (F5 or button)</td></tr>"
                + "<tr><td>Edit area</td><td>Dedicated source area</td><td>Anywhere on page</td></tr>"
                + "<tr><td>Output</td><td>Dedicated preview (auto-updated)</td><td>Page end (appended)</td></tr>"
                + "<tr><td>Best for</td><td>Continuous editing with live feedback</td><td>One-time full document render</td></tr>"
                + "</table>";
        }

        private static string GetIoEn()
        {
            return "<h1>Import/Export</h1>"
                + "<h2>Import Markdown File</h2>"
                + "<p>Import an external <code>.md</code> file and render it to the current OneNote page.</p>"
                + "<h3>Steps</h3>"
                + "<ol>"
                + "<li>Ensure a OneNote page is open</li>"
                + "<li>Click \"Import Markdown\" in the Ribbon</li>"
                + "<li>Select a <code>.md</code> or <code>.markdown</code> file in the dialog</li>"
                + "<li>The plugin reads, parses, and renders the content to the page</li>"
                + "</ol>"
                + "<h3>Supported Files</h3>"
                + "<ul>"
                + "<li><code>*.md</code> — Standard Markdown files</li>"
                + "<li><code>*.markdown</code> — Alternative extension</li>"
                + "<li>Size limit: 10 MB</li>"
                + "<li>Encoding: UTF-8 (recommended)</li>"
                + "</ul>"
                + "<h2>Export Markdown File</h2>"
                + "<p>Export the current OneNote page as a <code>.md</code> file.</p>"
                + "<h3>Steps</h3>"
                + "<ol>"
                + "<li>Open the page you want to export</li>"
                + "<li>Click \"Export Markdown\" in the Ribbon</li>"
                + "<li>Choose save location and filename</li>"
                + "<li>File is saved in UTF-8 encoding</li>"
                + "</ol>"
                + "<h2>Copy to Clipboard</h2>"
                + "<p>Export page as Markdown and copy directly to the system clipboard.</p>"
                + "<ul>"
                + "<li>Click \"Copy Markdown\" in the Ribbon</li>"
                + "<li>Or press <code>F8</code></li>"
                + "</ul>";
        }

        private static string GetLatexEn()
        {
            return "<h1>LaTeX Formulas</h1>"
                + "<p>This plugin renders LaTeX math formulas as high-resolution PNG images embedded in OneNote pages.</p>"
                + "<h2>Block Formulas (Display Math)</h2>"
                + "<p>Wrap formulas with <code>$$</code> markers on their own lines:</p>"
                + "<pre><code>$$\nE = mc^2\n$$</code></pre>"
                + "<p><b>Note:</b> Each <code>$$</code> must be on its own line.</p>"
                + "<h2>Examples</h2>"
                + "<h3>Basic Operations</h3>"
                + "<pre><code>$$\na^2 + b^2 = c^2\n$$</code></pre>"
                + "<h3>Fractions and Roots</h3>"
                + "<pre><code>$$\nx = \\frac{-b \\pm \\sqrt{b^2 - 4ac}}{2a}\n$$</code></pre>"
                + "<h3>Integrals</h3>"
                + "<pre><code>$$\n\\int_0^\\infty e^{-x^2} dx = \\frac{\\sqrt{\\pi}}{2}\n$$</code></pre>"
                + "<h3>Matrices</h3>"
                + "<pre><code>$$\nA = \\begin{pmatrix}\na &amp; b \\\\\nc &amp; d\n\\end{pmatrix}\n$$</code></pre>"
                + "<h2>Common LaTeX Symbols</h2>"
                + "<table>"
                + "<tr><th>Input</th><th>Meaning</th></tr>"
                + "<tr><td><code>\\frac{a}{b}</code></td><td>Fraction a/b</td></tr>"
                + "<tr><td><code>\\sqrt{x}</code></td><td>Square root</td></tr>"
                + "<tr><td><code>x^{n}</code></td><td>Superscript (power)</td></tr>"
                + "<tr><td><code>x_{i}</code></td><td>Subscript</td></tr>"
                + "<tr><td><code>\\sum</code></td><td>Summation Σ</td></tr>"
                + "<tr><td><code>\\int</code></td><td>Integral ∫</td></tr>"
                + "<tr><td><code>\\infty</code></td><td>Infinity ∞</td></tr>"
                + "<tr><td><code>\\alpha, \\beta, \\gamma</code></td><td>Greek letters</td></tr>"
                + "<tr><td><code>\\leq, \\geq, \\neq</code></td><td>≤, ≥, ≠</td></tr>"
                + "</table>"
                + "<h2>Entering Formulas in Live Mode</h2>"
                + "<ol>"
                + "<li>Type <code>$$</code> in the source area and press Enter</li>"
                + "<li>Type the formula, e.g. <code>E = mc^2</code></li>"
                + "<li>Type the closing <code>$$</code> and press Enter</li>"
                + "<li>The plugin detects the complete formula block and renders it as an image</li>"
                + "</ol>"
                + "<h2>Configuration</h2>"
                + "<table>"
                + "<tr><th>Setting</th><th>Default</th><th>Description</th></tr>"
                + "<tr><td><code>enable.latex.image</code></td><td>true</td><td>Set to false to keep formulas as plain text</td></tr>"
                + "<tr><td><code>font.math</code></td><td>Cambria Math</td><td>Math font for rendering</td></tr>"
                + "</table>";
        }

        private static string GetCodeEn()
        {
            return "<h1>Code Highlighting</h1>"
                + "<p>Fenced code blocks support syntax highlighting for multiple programming languages.</p>"
                + "<h2>Basic Syntax</h2>"
                + "<p>Use triple backticks <code>```</code> with a language identifier:</p>"
                + "<pre><code>```javascript\nfunction greet(name) {\n    return `Hello, ${name}!`;\n}\n```</code></pre>"
                + "<h2>Supported Languages</h2>"
                + "<table>"
                + "<tr><th>Identifier</th><th>Language</th></tr>"
                + "<tr><td><code>csharp</code> / <code>cs</code></td><td>C#</td></tr>"
                + "<tr><td><code>python</code> / <code>py</code></td><td>Python</td></tr>"
                + "<tr><td><code>javascript</code> / <code>js</code></td><td>JavaScript</td></tr>"
                + "<tr><td><code>typescript</code> / <code>ts</code></td><td>TypeScript</td></tr>"
                + "<tr><td><code>java</code></td><td>Java</td></tr>"
                + "<tr><td><code>cpp</code> / <code>c++</code></td><td>C++</td></tr>"
                + "<tr><td><code>sql</code></td><td>SQL</td></tr>"
                + "<tr><td><code>bash</code> / <code>shell</code></td><td>Shell</td></tr>"
                + "<tr><td><code>json</code></td><td>JSON</td></tr>"
                + "<tr><td><code>xml</code></td><td>XML</td></tr>"
                + "</table>"
                + "<h2>Configuration</h2>"
                + "<table>"
                + "<tr><th>Setting</th><th>Default</th><th>Description</th></tr>"
                + "<tr><td><code>font.monospace</code></td><td>Consolas</td><td>Code block font</td></tr>"
                + "<tr><td><code>font.size.code</code></td><td>10</td><td>Code font size</td></tr>"
                + "<tr><td><code>enable.code.lineNumber</code></td><td>false</td><td>Show line numbers</td></tr>"
                + "</table>"
                + "<h2>Diagram Source Blocks</h2>"
                + "<ul>"
                + "<li><code>```mermaid</code> — Mermaid flowcharts/sequence diagrams</li>"
                + "<li><code>```flow</code> — Flowcharts</li>"
                + "<li><code>```sequence</code> — Sequence diagrams</li>"
                + "</ul>";
        }

        private static string GetSettingsEn()
        {
            return "<h1>Settings</h1>"
                + "<p>Settings file: <code>%AppData%\\OneNoteMarkdown\\settings\\theme.ini</code></p>"
                + "<p>Click \"Settings\" in the Ribbon to open the settings dialog.</p>"
                + "<h2>Available Settings</h2>"
                + "<table>"
                + "<tr><th>Setting</th><th>Default</th><th>Description</th></tr>"
                + "<tr><td><code>font.family</code></td><td>Calibri</td><td>Body font</td></tr>"
                + "<tr><td><code>font.monospace</code></td><td>Consolas</td><td>Code font</td></tr>"
                + "<tr><td><code>font.math</code></td><td>Cambria Math</td><td>Math formula font</td></tr>"
                + "<tr><td><code>font.size.paragraph</code></td><td>11</td><td>Body font size</td></tr>"
                + "<tr><td><code>font.size.code</code></td><td>10</td><td>Code font size</td></tr>"
                + "<tr><td><code>enable.latex.image</code></td><td>true</td><td>Render LaTeX as images</td></tr>"
                + "<tr><td><code>enable.code.lineNumber</code></td><td>false</td><td>Show code line numbers</td></tr>"
                + "<tr><td><code>language</code></td><td>auto</td><td>UI language (auto/zh/en)</td></tr>"
                + "</table>"
                + "<p>Changes take effect after re-rendering the page.</p>";
        }

        private static string GetShortcutsEn()
        {
            return "<h1>Keyboard Shortcuts</h1>"
                + "<table>"
                + "<tr><th>Shortcut</th><th>Function</th></tr>"
                + "<tr><td><code>F5</code></td><td>Render Page</td></tr>"
                + "<tr><td><code>F8</code></td><td>Copy Markdown to Clipboard</td></tr>"
                + "<tr><td><code>Ctrl+\\</code></td><td>Toggle Live Mode</td></tr>"
                + "<tr><td><code>Ctrl+Enter</code></td><td>Render Current Line</td></tr>"
                + "</table>"
                + "<h2>Notes</h2>"
                + "<ul>"
                + "<li>Shortcuts are globally registered and only trigger when OneNote is in focus</li>"
                + "<li>The current version no longer intercepts the Enter key to avoid affecting normal input</li>"
                + "</ul>";
        }

        private static string GetFaqEn()
        {
            return "<h1>FAQ</h1>"
                + "<h2>Q: Markdown tab doesn't appear after installation</h2>"
                + "<p><b>A:</b> Please check:</p>"
                + "<ol>"
                + "<li>Installer architecture matches OneNote (32-bit vs 64-bit)</li>"
                + "<li>OneNote must be restarted after installation</li>"
                + "<li>Check if OneNote disabled the plugin: File → Options → Add-ins</li>"
                + "</ol>"
                + "<h2>Q: Preview doesn't update after pressing Enter in Live Mode</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>There may be a 0.2~0.5 second delay due to OneNote's commit timing</li>"
                + "<li>Press <code>F5</code> to manually refresh if it doesn't update</li>"
                + "<li>Make sure your cursor is in the \"Markdown Source (Live)\" area</li>"
                + "</ul>"
                + "<h2>Q: LaTeX formula not rendered as image</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>Ensure <code>$$</code> markers are each on their own line</li>"
                + "<li>Check that <code>enable.latex.image=true</code> in settings</li>"
                + "<li>Syntax errors cause the original text to be preserved</li>"
                + "</ul>"
                + "<h2>Q: Shortcuts not responding</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>Shortcuts may conflict with other software</li>"
                + "<li>Ensure OneNote window is active and in the foreground</li>"
                + "</ul>"
                + "<h2>Log File</h2>"
                + "<p><code>%AppData%\\OneNoteMarkdown\\logs\\onenotemarkdown.log</code></p>";
        }
    }
}
