using System;
using System.Drawing;
using System.Windows.Forms;

namespace OneNoteMarkdown.UI
{
    internal sealed class HelpWindow : Form
    {
        private TreeView _treeView;
        private WebBrowser _browser;
        private Panel _headerPanel;
        private Button _closeButton;

        public HelpWindow()
        {
            InitializeComponents();
            PopulateTree();
            ShowContent("about");
        }

        public static void ShowHelp()
        {
            HelpWindow win = new HelpWindow();
            IWin32Window anchor = UiThread.Anchor;
            if (anchor != null)
            {
                win.ShowDialog(new WindowWrapper(anchor.Handle));
            }
            else
            {
                win.ShowDialog();
            }
        }

        private void InitializeComponents()
        {
            Text = "OneNote Markdown - 帮助";
            Size = new Size(900, 640);
            MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterScreen;
            Icon = null;
            ShowIcon = false;

            // Header panel
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(104, 33, 122)
            };
            Label headerLabel = new Label
            {
                Text = "✦  使用帮助",
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 12f, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            _headerPanel.Controls.Add(headerLabel);
            Controls.Add(_headerPanel);

            // Bottom panel with close button
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(0, 8, 16, 8)
            };
            _closeButton = new Button
            {
                Text = "关闭",
                Size = new Size(80, 32),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                FlatStyle = FlatStyle.System
            };
            _closeButton.Click += (s, e) => Close();
            bottomPanel.Controls.Add(_closeButton);
            _closeButton.Location = new Point(bottomPanel.Width - _closeButton.Width - 16, 9);
            bottomPanel.Resize += (s, e) =>
            {
                _closeButton.Location = new Point(bottomPanel.Width - _closeButton.Width - 16, 9);
            };
            Controls.Add(bottomPanel);

            // Split container
            SplitContainer splitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 180,
                FixedPanel = FixedPanel.Panel1,
                BorderStyle = BorderStyle.None
            };

            // Left: TreeView
            _treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 9.5f),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(252, 252, 252),
                ShowLines = true,
                ShowRootLines = true,
                HideSelection = false,
                ItemHeight = 24
            };
            _treeView.AfterSelect += TreeView_AfterSelect;
            splitter.Panel1.Controls.Add(_treeView);
            splitter.Panel1.BackColor = Color.FromArgb(252, 252, 252);

            // Right: WebBrowser
            _browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                IsWebBrowserContextMenuEnabled = false,
                ScriptErrorsSuppressed = true,
                AllowNavigation = false
            };
            splitter.Panel2.Controls.Add(_browser);

            Controls.Add(splitter);

            // Ensure Z-order: header on top, bottom on bottom
            _headerPanel.BringToFront();
        }

        private void PopulateTree()
        {
            TreeNode root = new TreeNode("目录") { Tag = "about" };

            TreeNode quickStart = new TreeNode("快速入门") { Tag = "quickstart" };
            root.Nodes.Add(quickStart);

            TreeNode features = new TreeNode("功能说明") { Tag = "features" };
            features.Nodes.Add(new TreeNode("渲染整页") { Tag = "feat_renderpage" });
            features.Nodes.Add(new TreeNode("渲染选区") { Tag = "feat_rendersel" });
            features.Nodes.Add(new TreeNode("实时模式") { Tag = "feat_live" });
            features.Nodes.Add(new TreeNode("导入/导出") { Tag = "feat_io" });
            features.Nodes.Add(new TreeNode("LaTeX 公式") { Tag = "feat_latex" });
            features.Nodes.Add(new TreeNode("代码高亮") { Tag = "feat_code" });
            root.Nodes.Add(features);

            TreeNode settings = new TreeNode("设置说明") { Tag = "settings" };
            root.Nodes.Add(settings);

            TreeNode shortcuts = new TreeNode("快捷键") { Tag = "shortcuts" };
            root.Nodes.Add(shortcuts);

            TreeNode faq = new TreeNode("常见问题") { Tag = "faq" };
            root.Nodes.Add(faq);

            TreeNode about = new TreeNode("关于") { Tag = "about" };
            root.Nodes.Add(about);

            _treeView.Nodes.Add(root);
            root.ExpandAll();
            _treeView.SelectedNode = root.Nodes[root.Nodes.Count - 1]; // select "关于"
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string tag = e.Node?.Tag as string;
            if (!string.IsNullOrEmpty(tag))
            {
                ShowContent(tag);
            }
        }

        private void ShowContent(string section)
        {
            string html = BuildHtml(section);
            _browser.DocumentText = html;
        }

        private static string BuildHtml(string section)
        {
            string body;
            switch (section)
            {
                case "about":
                    body = GetAboutHtml();
                    break;
                case "quickstart":
                    body = GetQuickStartHtml();
                    break;
                case "features":
                    body = GetFeaturesOverviewHtml();
                    break;
                case "feat_renderpage":
                    body = GetRenderPageHtml();
                    break;
                case "feat_rendersel":
                    body = GetRenderSelHtml();
                    break;
                case "feat_live":
                    body = GetLiveHtml();
                    break;
                case "feat_io":
                    body = GetIoHtml();
                    break;
                case "feat_latex":
                    body = GetLatexHtml();
                    break;
                case "feat_code":
                    body = GetCodeHtml();
                    break;
                case "settings":
                    body = GetSettingsHtml();
                    break;
                case "shortcuts":
                    body = GetShortcutsHtml();
                    break;
                case "faq":
                    body = GetFaqHtml();
                    break;
                default:
                    body = GetAboutHtml();
                    break;
            }

            return "<!DOCTYPE html><html><head><meta charset='utf-8'/><style>"
                + "body{font-family:'Microsoft YaHei UI','Segoe UI',sans-serif;font-size:14px;color:#333;margin:24px 28px;line-height:1.7;}"
                + "h1{font-size:22px;color:#333;margin-bottom:4px;}"
                + "h2{font-size:16px;color:#68217A;margin-top:20px;border-bottom:none;}"
                + "h3{font-size:14px;color:#68217A;margin-top:14px;}"
                + "p{margin:8px 0;}"
                + "ul,ol{padding-left:20px;}"
                + "li{margin:4px 0;}"
                + "code{background:#f5f5f5;padding:2px 5px;border-radius:3px;font-family:Consolas,'Courier New',monospace;font-size:13px;}"
                + "pre{background:#f8f8f8;border:1px solid #e0e0e0;border-radius:4px;padding:12px 16px;overflow-x:auto;margin:10px 0;}"
                + "pre code{background:none;padding:0;font-size:12px;line-height:1.5;}"
                + "table{border-collapse:collapse;margin:12px 0;width:100%;}"
                + "th,td{border:1px solid #ddd;padding:6px 12px;text-align:left;}"
                + "th{background:#f0f0f0;font-weight:bold;}"
                + ".version{color:#666;font-size:13px;margin-bottom:16px;}"
                + ".highlight{color:#68217A;}"
                + "b{color:#333;}"
                + "</style></head><body>" + body + "</body></html>";
        }

        private static string GetAboutHtml()
        {
            return "<h1>关于 OneNote Markdown</h1>"
                + "<p class='version'>版本: 1.1.0</p>"
                + "<h2>开发背景</h2>"
                + "<p>本插件由 OneNote MVP 开发。</p>"
                + "<p>在日常使用 OneNote 的过程中，我们发现很多用户习惯了 Markdown 的写作方式，"
                + "但 OneNote 原生并不支持 Markdown 语法。为了解决这一痛点，我们开发了 OneNote Markdown —— "
                + "一款在 Microsoft OneNote 桌面版中直接编写和渲染 Markdown 的插件。</p>"
                + "<p>本插件支持实时预览、LaTeX 数学公式渲染、图表源码块保留、代码语法高亮等特性，"
                + "为 OneNote 用户提供流畅、高效的 Markdown 写作与笔记体验。</p>"
                + "<h2>主要功能</h2>"
                + "<ul>"
                + "<li><b>Markdown 渲染</b> —— 支持标题、列表、代码块、表格、引用块等标准语法</li>"
                + "<li><b>实时预览模式</b> —— 页面分为源码区和预览区，编辑回车后自动刷新渲染</li>"
                + "<li><b>LaTeX 公式</b> —— 支持块级数学公式 <code>$$ ... $$</code>，渲染为高清 PNG 图片</li>"
                + "<li><b>图表源码块</b> —— 自动识别 Mermaid、流程图等围栏代码块，保留格式显示</li>"
                + "<li><b>代码高亮</b> —— 多种编程语言语法着色（C#、Python、JavaScript 等）</li>"
                + "<li><b>导入/导出</b> —— 从 .md 文件导入渲染到页面，或将页面导出为 Markdown 文件</li>"
                + "<li><b>剪贴板支持</b> —— 一键复制当前页为 Markdown 格式</li>"
                + "</ul>"
                + "<h2>技术栈</h2>"
                + "<ul>"
                + "<li>C# / .NET Framework 4.8</li>"
                + "<li>OneNote COM Interop API</li>"
                + "<li>WPF-Math（LaTeX 渲染引擎）</li>"
                + "</ul>"
                + "<h2>许可证</h2>"
                + "<p>MIT License —— 自由使用、修改和分发。</p>";
        }

        private static string GetQuickStartHtml()
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
                + "<li>在 OneNote 页面中直接输入 Markdown 文本，例如：</li>"
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

        private static string GetFeaturesOverviewHtml()
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

        private static string GetRenderPageHtml()
        {
            return "<h1>渲染整页</h1>"
                + "<p>将当前页面的全部正文内容按 Markdown 语法解析，并在页面末尾生成渲染后的格式化内容。"
                + "适合一次性将整篇 Markdown 文档渲染为 OneNote 原生格式。</p>"
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

        private static string GetRenderSelHtml()
        {
            return "<h1>渲染选区</h1>"
                + "<p>仅渲染当前选中的 Markdown 文本，结果追加到页面末尾。适合只渲染页面中的部分内容。</p>"
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
                + "</ul>"
                + "<h2>注意事项</h2>"
                + "<ul>"
                + "<li>如果没有选中任何内容，会提示 \"当前没有选中文本\"</li>"
                + "<li>选区可以跨多行，插件会将选中的所有行合并为一个 Markdown 文档进行渲染</li>"
                + "</ul>";
        }

        private static string GetLiveHtml()
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
                + "<li><b>编写 Markdown</b> —— 正常输入 Markdown 文本，例如：</li>"
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
                + "</table>"
                + "<h2>常见操作流程示例</h2>"
                + "<h3>示例 1：写带公式的笔记</h3>"
                + "<pre><code>1. Ctrl+\\ 开启实时模式\n2. 在源码区输入：\n   # 物理公式\n   动能公式：\n   $$\n   E_k = \\frac{1}{2}mv^2\n   $$\n3. 回车后预览区自动显示渲染的公式图片</code></pre>"
                + "<h3>示例 2：写代码笔记</h3>"
                + "<pre><code>1. 在源码区输入：\n   ## Python 示例\n   ```python\n   def hello():\n       print(\"Hello, OneNote!\")\n   ```\n2. 在 ``` 关闭行后回车，预览区显示语法高亮的代码块</code></pre>";
        }

        private static string GetIoHtml()
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
                + "<h3>导出说明</h3>"
                + "<ul>"
                + "<li>导出的是页面的纯文本内容，会尽量保留 Markdown 格式</li>"
                + "<li>OneNote 格式化内容（加粗、列表等）会转换回 Markdown 语法</li>"
                + "<li>图片和附件不包含在导出中</li>"
                + "</ul>"
                + "<h2>复制到剪贴板</h2>"
                + "<p>将当前页面导出为 Markdown 格式并直接复制到系统剪贴板，方便粘贴到其他编辑器。</p>"
                + "<h3>操作方式</h3>"
                + "<ul>"
                + "<li>点击功能区 \"复制 Markdown\" 按钮</li>"
                + "<li>或按快捷键 <code>F8</code></li>"
                + "</ul>"
                + "<p>复制后可直接粘贴到 VS Code、Typora、语雀等 Markdown 编辑器中。</p>";
        }

        private static string GetLatexHtml()
        {
            return "<h1>LaTeX 公式</h1>"
                + "<p>本插件支持将 LaTeX 数学公式渲染为高清 PNG 图片并嵌入 OneNote 页面，让你的数学笔记更加美观专业。</p>"
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
                + "<h3>求和与极限</h3>"
                + "<pre><code>$$\n\\sum_{n=1}^{\\infty} \\frac{1}{n^2} = \\frac{\\pi^2}{6}\n$$</code></pre>"
                + "<h3>矩阵</h3>"
                + "<pre><code>$$\nA = \\begin{pmatrix}\na &amp; b \\\\\nc &amp; d\n\\end{pmatrix}\n$$</code></pre>"
                + "<h3>多行公式（对齐）</h3>"
                + "<pre><code>$$\n\\begin{aligned}\nf(x) &amp;= x^2 + 2x + 1 \\\\\n     &amp;= (x+1)^2\n\\end{aligned}\n$$</code></pre>"
                + "<h2>常用 LaTeX 符号速查</h2>"
                + "<table>"
                + "<tr><th>输入</th><th>含义</th></tr>"
                + "<tr><td><code>\\frac{a}{b}</code></td><td>分数 a/b</td></tr>"
                + "<tr><td><code>\\sqrt{x}</code></td><td>平方根</td></tr>"
                + "<tr><td><code>\\sqrt[n]{x}</code></td><td>n 次根</td></tr>"
                + "<tr><td><code>x^{n}</code></td><td>上标（幂）</td></tr>"
                + "<tr><td><code>x_{i}</code></td><td>下标</td></tr>"
                + "<tr><td><code>\\sum</code></td><td>求和 Σ</td></tr>"
                + "<tr><td><code>\\prod</code></td><td>连乘 Π</td></tr>"
                + "<tr><td><code>\\int</code></td><td>积分 ∫</td></tr>"
                + "<tr><td><code>\\lim</code></td><td>极限</td></tr>"
                + "<tr><td><code>\\infty</code></td><td>无穷 ∞</td></tr>"
                + "<tr><td><code>\\alpha, \\beta, \\gamma</code></td><td>希腊字母</td></tr>"
                + "<tr><td><code>\\leq, \\geq, \\neq</code></td><td>≤, ≥, ≠</td></tr>"
                + "<tr><td><code>\\rightarrow</code></td><td>→</td></tr>"
                + "<tr><td><code>\\forall, \\exists</code></td><td>∀, ∃</td></tr>"
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
                + "<tr><td><code>enable.latex.image</code></td><td>true</td><td>设为 false 则公式以纯文本形式保留，不渲染为图片</td></tr>"
                + "<tr><td><code>font.math</code></td><td>Cambria Math</td><td>公式渲染使用的数学字体</td></tr>"
                + "</table>"
                + "<h2>注意事项</h2>"
                + "<ul>"
                + "<li>公式渲染基于 WPF-Math 引擎，支持大部分常用 LaTeX 数学命令</li>"
                + "<li>不支持 <code>\\usepackage</code> 等文档级命令</li>"
                + "<li>复杂宏包命令（如 tikz）不受支持</li>"
                + "<li>如果公式语法有误，将保留原始文本不做渲染</li>"
                + "<li>渲染后的公式以 PNG 图片形式嵌入 OneNote，支持打印和同步</li>"
                + "</ul>";
        }

        private static string GetCodeHtml()
        {
            return "<h1>代码高亮</h1>"
                + "<p>围栏代码块支持多种编程语言的语法高亮，让代码笔记更加清晰易读。</p>"
                + "<h2>基本语法</h2>"
                + "<p>使用三个反引号 <code>```</code> 或波浪号 <code>~~~</code> 包裹代码块，"
                + "在开头标记后指定语言名称：</p>"
                + "<pre><code>```javascript\nfunction greet(name) {\n    return `Hello, ${name}!`;\n}\nconsole.log(greet(\"OneNote\"));\n```</code></pre>"
                + "<h2>支持的语言</h2>"
                + "<p>常见编程语言均已支持，例如：</p>"
                + "<table>"
                + "<tr><th>语言标识</th><th>语言</th></tr>"
                + "<tr><td><code>csharp</code> / <code>cs</code></td><td>C#</td></tr>"
                + "<tr><td><code>python</code> / <code>py</code></td><td>Python</td></tr>"
                + "<tr><td><code>javascript</code> / <code>js</code></td><td>JavaScript</td></tr>"
                + "<tr><td><code>typescript</code> / <code>ts</code></td><td>TypeScript</td></tr>"
                + "<tr><td><code>java</code></td><td>Java</td></tr>"
                + "<tr><td><code>cpp</code> / <code>c++</code></td><td>C++</td></tr>"
                + "<tr><td><code>html</code></td><td>HTML</td></tr>"
                + "<tr><td><code>css</code></td><td>CSS</td></tr>"
                + "<tr><td><code>sql</code></td><td>SQL</td></tr>"
                + "<tr><td><code>bash</code> / <code>shell</code></td><td>Shell 脚本</td></tr>"
                + "<tr><td><code>json</code></td><td>JSON</td></tr>"
                + "<tr><td><code>xml</code></td><td>XML</td></tr>"
                + "<tr><td><code>markdown</code> / <code>md</code></td><td>Markdown</td></tr>"
                + "</table>"
                + "<h2>更多示例</h2>"
                + "<h3>Python</h3>"
                + "<pre><code>```python\nimport numpy as np\n\ndef fibonacci(n):\n    \"\"\"生成斐波那契数列\"\"\"\n    a, b = 0, 1\n    for _ in range(n):\n        yield a\n        a, b = b, a + b\n\nprint(list(fibonacci(10)))\n```</code></pre>"
                + "<h3>SQL</h3>"
                + "<pre><code>```sql\nSELECT u.name, COUNT(o.id) AS order_count\nFROM users u\nLEFT JOIN orders o ON u.id = o.user_id\nWHERE u.created_at > '2024-01-01'\nGROUP BY u.name\nORDER BY order_count DESC;\n```</code></pre>"
                + "<h2>配置选项</h2>"
                + "<table>"
                + "<tr><th>配置项</th><th>默认值</th><th>说明</th></tr>"
                + "<tr><td><code>font.monospace</code></td><td>Consolas</td><td>代码块使用的等宽字体</td></tr>"
                + "<tr><td><code>font.size.code</code></td><td>10</td><td>代码字号（磅）</td></tr>"
                + "<tr><td><code>enable.code.lineNumber</code></td><td>false</td><td>设为 true 显示行号</td></tr>"
                + "</table>"
                + "<h2>图表源码块</h2>"
                + "<p>特殊语言标识的代码块将以图表形式渲染：</p>"
                + "<ul>"
                + "<li><code>```mermaid</code> —— Mermaid 流程图/时序图/甘特图</li>"
                + "<li><code>```mindmap</code> —— 思维导图</li>"
                + "<li><code>```flow</code> —— 流程图</li>"
                + "<li><code>```sequence</code> —— 时序图</li>"
                + "</ul>"
                + "<p>图表源码块将以带边框的预览图形式显示，保留源码可读性。</p>";
        }

        private static string GetSettingsHtml()
        {
            return "<h1>设置说明</h1>"
                + "<p>设置文件位于：</p>"
                + "<p><code>%AppData%\\OneNoteMarkdown\\settings\\theme.ini</code></p>"
                + "<p>点击功能区 \"设置\" 按钮可直接打开编辑。</p>"
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
                + "</table>"
                + "<p>修改后重新渲染页面即可生效。</p>";
        }

        private static string GetShortcutsHtml()
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

        private static string GetFaqHtml()
        {
            return "<h1>常见问题</h1>"
                + "<h2>Q: 安装后 OneNote 中没有出现 Markdown 选项卡</h2>"
                + "<p><b>A:</b> 请检查以下几点：</p>"
                + "<ol>"
                + "<li>确认安装包位数与 OneNote 位数一致（文件 → 帐户 → 关于 OneNote 查看）</li>"
                + "<li>安装后必须重启 OneNote</li>"
                + "<li>检查 OneNote 是否禁用了插件：文件 → 选项 → 加载项，查看是否在 \"禁用项\" 列表中</li>"
                + "<li>如果在禁用列表中，点击 \"管理\" 下拉选择 \"已禁用项\"，点击 \"转到\" 启用</li>"
                + "</ol>"
                + "<h2>Q: 点击 \"渲染整页\" 后没有任何变化</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>确认页面中确实包含 Markdown 语法内容（如 # 标题、- 列表等）</li>"
                + "<li>纯文本（没有任何 Markdown 标记）不会产生渲染结果</li>"
                + "<li>检查日志文件 <code>%AppData%\\OneNoteMarkdown\\logs\\onenotemarkdown.log</code> 排查错误</li>"
                + "</ul>"
                + "<h2>Q: 实时模式回车后预览没有更新</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>实时模式依赖 OneNote 的内容提交时机，按回车后可能有 0.2~0.5 秒延迟</li>"
                + "<li>如果等待后仍无变化，按 <code>F5</code> 手动触发刷新</li>"
                + "<li>确认光标在 \"Markdown 源码（实时）\" 区域内，不要在预览区编辑</li>"
                + "<li>确认实时模式已开启（功能区按钮应为高亮/按下状态）</li>"
                + "</ul>"
                + "<h2>Q: LaTeX 公式没有渲染为图片</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>确认公式格式正确：<code>$$</code> 必须各自独占一行</li>"
                + "<li>检查设置文件中 <code>enable.latex.image=true</code></li>"
                + "<li>公式语法错误时会保留原始文本，检查 LaTeX 语法是否正确</li>"
                + "<li>极少数情况下字体问题可能导致失败，确认系统安装了 Cambria Math 字体</li>"
                + "</ul>"
                + "<h2>Q: 快捷键无响应</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>快捷键为全局注册，可能与其他软件冲突</li>"
                + "<li>如果 F5 与其他插件冲突，请使用功能区按钮操作</li>"
                + "<li>确认 OneNote 窗口处于前台激活状态</li>"
                + "</ul>"
                + "<h2>Q: 代码块没有语法高亮</h2>"
                + "<p><b>A:</b></p>"
                + "<ul>"
                + "<li>确认在 <code>```</code> 后指定了语言名称，如 <code>```python</code></li>"
                + "<li>未指定语言的代码块将以普通等宽字体显示</li>"
                + "</ul>"
                + "<h2>Q: 如何更换字体和字号？</h2>"
                + "<p><b>A:</b> 点击功能区 \"设置\" 按钮打开 <code>theme.ini</code>，修改对应配置项后重新渲染即可。"
                + "详见 \"设置说明\" 章节。</p>"
                + "<h2>日志文件位置</h2>"
                + "<p><code>%AppData%\\OneNoteMarkdown\\logs\\onenotemarkdown.log</code></p>"
                + "<p>日志文件记录了插件的运行状态和错误信息，遇到问题时请查看日志帮助排查。"
                + "日志文件最大 5 MB，超出后自动轮转。</p>"
                + "<h2>已知限制</h2>"
                + "<ul>"
                + "<li>实时模式目前非完全逐字符即时渲染，依赖回车触发</li>"
                + "<li>图表源码块（Mermaid 等）以预览图展示，暂不支持完整图表渲染</li>"
                + "<li>不支持 LaTeX 文档级命令和复杂宏包</li>"
                + "<li>较旧版本 OneNote（2013 以前）可能有兼容性差异</li>"
                + "<li>OneNote UWP 版（Windows 10 应用商店版）不受支持，仅支持桌面版 OneNote</li>"
                + "</ul>";
        }
    }
}
